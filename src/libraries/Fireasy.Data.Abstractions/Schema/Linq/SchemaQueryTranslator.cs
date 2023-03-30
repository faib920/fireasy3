// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Linq.Expressions;
using System.Linq.Expressions;
using System.Reflection;

namespace Fireasy.Data.Schema.Linq
{
    internal sealed class SchemaQueryTranslator : ExpressionVisitor
    {
        private Type? _metadataType;
        private string? _memberName;
        private List<MemberInfo>? _members = null;
        private bool _multipleQuerySupport;
        private readonly RestrictionDictionary _restrDict = new RestrictionDictionary();

        /// <summary>
        /// 对表达式进行解析，并返回限制值字典。
        /// </summary>
        /// <param name="expression">查询表达式。</param>
        /// <param name="dicRestrMbrs">限制成员表。</param>
        /// <param name="multipleQuerySupport">是否支持多个限制值进行查询。</param>
        /// <returns></returns>
        public static RestrictionDictionary GetRestrictions<T>(Expression? expression, Dictionary<Type, List<MemberInfo>> dicRestrMbrs, bool multipleQuerySupport)
        {
            if (expression == null)
            {
                return RestrictionDictionary.Empty;
            }

            var translator = new SchemaQueryTranslator { _metadataType = typeof(T), _multipleQuerySupport = multipleQuerySupport };

            if (!dicRestrMbrs.TryGetValue(typeof(T), out var properties))
            {
                throw new SchemaQueryTranslateException(typeof(T));
            }

            translator._members = properties;
            expression = PartialEvaluator.Eval(expression);
            translator.Visit(expression);
            return translator._restrDict;
        }

        /// <summary>
        /// 访问二元运算表达式。
        /// </summary>
        /// <param name="binaryExp"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression binaryExp)
        {
            _memberName = string.Empty;

            if (binaryExp.Right is MemberExpression rmbr && rmbr.Member.DeclaringType == _metadataType)
            {
                Visit(binaryExp.Right);
                Visit(binaryExp.Left);
            }
            else if (binaryExp.Left is MemberExpression lmbr && lmbr.Member.DeclaringType == _metadataType)
            {
                Visit(binaryExp.Left);
                Visit(binaryExp.Right);
            }
            else
            {
                Visit(binaryExp.Left);
                Visit(binaryExp.Right);
            }

            return binaryExp;
        }

        protected override Expression VisitMember(MemberExpression memberExp)
        {
            //如果属性是架构元数据类的成员
            if (memberExp.Member.DeclaringType == _metadataType)
            {
                if (_members?.Contains(memberExp.Member) == false)
                {
                    throw new SchemaQueryTranslateException(memberExp.Member, _members);
                }

                _memberName = memberExp.Member.Name;
                return memberExp;
            }

            //值或引用
            var exp = (Expression)memberExp;
            if (memberExp.Type.IsValueType)
            {
                exp = Expression.Convert(memberExp, typeof(object));
            }

            var lambda = Expression.Lambda<Func<object>>(exp);
            var fn = lambda.Compile();

            //转换为常量表达式
            return Visit(Expression.Constant(fn(), memberExp.Type));
        }

        protected override Expression VisitConstant(ConstantExpression constExp)
        {
            if (!string.IsNullOrEmpty(_memberName))
            {
                _restrDict[_memberName] = constExp.Value;
            }

            return constExp;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (_multipleQuerySupport &&
                node.Method.DeclaringType == typeof(Enumerable) && node.Method.Name == nameof(Enumerable.Contains))
            {
                Visit(node.Arguments[1]);
                Visit(node.Arguments[0]);

                return node;
            }
            if (_multipleQuerySupport &&
                node.Method.DeclaringType?.IsGenericType == true && node.Method.DeclaringType.GetGenericTypeDefinition() == typeof(List<>) &&
                   node.Method.Name == nameof(List<string>.Contains))
            {
                Visit(node.Object);
                Visit(node.Arguments[0]);

                return node;
            }
            else if (node.Method.DeclaringType == typeof(string))
            {
                if (node.Method.Name == nameof(string.Equals))
                {
                    Visit(node.Object);
                    Visit(node.Arguments[0]);

                    return node;
                }
            }

            throw new SchemaQueryTranslateException($"{node.Method.DeclaringType.Name}.{node.Method.Name} 方法不受支持。");
        }
    }
}
