// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Fireasy.Data.Converter;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;

namespace Fireasy.Data
{
    /// <summary>
    /// 一个缺省的数据行映射器。无法继承此类。
    /// </summary>
    /// <typeparam name="T">要转换的类型。</typeparam>
    public sealed class DefaultRowMapper<T> : FieldRowMapperBase<T>
    {
        private Func<IValueConvertManager?, IDataReader, T>? _funcDataRecd;
        private readonly IServiceProvider _serviceProvider;
        private readonly IValueConvertManager _valueConvertManager;

        /// <summary>
        /// 初始化 <see cref="DefaultRowMapper{T}"/> 类的新实例。
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultRowMapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _valueConvertManager = _serviceProvider.TryGetService<IValueConvertManager>()!;
        }

        private class MethodCache
        {
            internal protected static readonly MethodInfo IsDBNull = typeof(IDataRecord).GetMethod(nameof(IDataReader.IsDBNull), new[] { typeof(int) });
            internal protected static readonly MethodInfo ConvertFrom = typeof(IValueConverter).GetMethod(nameof(IValueConverter.ConvertFrom));
        }

        /// <summary>
        /// 将一个 <see cref="IDataReader"/> 转换为一个 <typeparamref name="T"/> 的对象。
        /// </summary>
        /// <param name="reader">一个 <see cref="IDataReader"/> 对象。</param>
        /// <returns>由当前 <see cref="IDataReader"/> 对象中的数据转换成的 <typeparamref name="T"/> 对象实例。</returns>
        public override T Map(IDataReader reader)
        {
            if (_funcDataRecd == null)
            {
                CompileFunction(reader);
            }

            return _funcDataRecd!(_valueConvertManager, reader);
        }

        private IEnumerable<PropertyInfo> GetProperties()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(s => s.PropertyType.IsDbTypeSupported());
        }

        private IEnumerable<PropertyMapping> GetMapping(string[] fields)
        {
            return from s in GetProperties()
                   let index = IndexOf(fields, s.Name)
                   where s.CanWrite && index != -1 && s.GetIndexParameters().Length == 0
                   select new PropertyMapping { Info = s, Index = index };
        }

        private void CompileFunction(IDataReader reader)
        {
            var newExp = Expression.New(typeof(T));
            var mapping = GetMapping(GetDataReaderFields(reader));

            var rowMapExp = Expression.Constant(RecordWrapper);
            var parExp = Expression.Parameter(typeof(IDataRecord), "s");
            var parExpm = Expression.Parameter(typeof(IValueConvertManager), "m");

            var bindings =
                mapping.Select(s =>
                {
                    var dbType = reader.GetFieldType(s.Index);
                    var getValueMethod = Data.RecordWrapper.RecordWrapHelper.GetMethodByOrdinal(dbType.GetDbType());

                    var expression = (Expression)Expression.Call(rowMapExp, getValueMethod, new Expression[] { parExp, Expression.Constant(s.Index) });

                    var convertManager = _serviceProvider?.GetService<IValueConvertManager>();

                    if (convertManager?.CanConvert(s.Info.PropertyType) == true)
                    {
                        expression = Expression.Call(parExpm, MethodCache.ConvertFrom, Expression.Convert(expression, typeof(object)), Expression.Constant(dbType.GetDbType()));
                        expression = Expression.Convert(expression, s.Info.PropertyType);
                    }
                    else if (s.Info.PropertyType.IsNullableType())
                    {
                        expression = Expression.Condition(
                            Expression.Call(parExp, MethodCache.IsDBNull, Expression.Constant(s.Index, typeof(int))),
                                Expression.Convert(Expression.Constant(null), s.Info.PropertyType),
                            Expression.Convert(expression, s.Info.PropertyType));
                    }
                    else if (dbType != s.Info.PropertyType)
                    {
                        expression = Expression.Convert(expression, s.Info.PropertyType);
                    }

                    return Expression.Bind(s.Info, expression);
                });

            var expr =
                Expression.Lambda<Func<IValueConvertManager, IDataReader, T>>(
                    Expression.MemberInit(
                        newExp,
                        bindings.ToArray()),
                    parExpm, parExp);

            _funcDataRecd = expr.Compile();
        }
    }
}
