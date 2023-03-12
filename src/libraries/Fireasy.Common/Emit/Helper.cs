// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Reflection;

namespace Fireasy.Common.Emit
{
    internal static class Helper
    {
        /// <summary>
        /// 在类里查找与参数相匹配的构造函数。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static ConstructorInfo MatchConstructor(Type type, Type[] parameterTypes)
        {
            if (parameterTypes == null || parameterTypes.Count() == 0)
            {
                return type.GetTypeInfo().GetConstructors().FirstOrDefault(s => !s.IsStatic);
            }
            else if (parameterTypes.Any(s => s is GtpType))
            {
                var gtypes = parameterTypes.Where(s => s is GtpType).Cast<GtpType>().ToArray();

                return type.GetTypeInfo().GetConstructors().FirstOrDefault(s => IsEquals(s.GetParameters(), gtypes));
            }
            else
            {
                return type.GetTypeInfo().GetConstructors().FirstOrDefault(s => IsEquals(s.GetParameters(), parameterTypes));
            }
        }

        /// <summary>
        /// 在类里查找与参数相匹配的方法。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static MethodInfo MatchMethod(Type type, string methodName, Type[] parameterTypes)
        {
            if (parameterTypes == null)
            {
                return type.GetTypeInfo().GetMethods().FirstOrDefault(s => s.Name == methodName);
            }
            else if (parameterTypes.Any(s => s is GtpType))
            {
                var gtypes = parameterTypes.Where(s => s is GtpType).Cast<GtpType>().ToArray();

                return type.GetTypeInfo().GetMethods().FirstOrDefault(s => s.Name == methodName && IsEquals(s.GetParameters(), gtypes));
            }
            else
            {
                return type.GetTypeInfo().GetMethods().FirstOrDefault(s => s.Name == methodName && IsEquals(s.GetParameters(), parameterTypes));
            }
        }

        private static bool IsEquals(ParameterInfo[] parameters, Type[] types2)
        {
            if (parameters.Length != types2.Length)
            {
                return false;
            }

            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != types2[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsEquals(ParameterInfo[] parameters, GtpType[] gtypes)
        {
            var types = parameters.Where(s => s.ParameterType.IsGenericParameter).Select(s => s.ParameterType).ToArray();

            if (types.Length != gtypes.Length)
            {
                return false;
            }

            for (var i = 0; i < types.Length; i++)
            {
                if (types[i].Name != gtypes[i].Name)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
