﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Emit;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Fireasy.Common.Extensions
{
    /// <summary>
    /// 反射的扩展方法。
    /// </summary>
    public static class ReflectionExtension
    {
        /// <summary>
        /// 返回是否无符号的整型类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUnsignedInt(this Type type)
        {
            type = type.GetNonNullableType();
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 返回是否无符号类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUnsigned(this Type type)
        {
            type = type.GetNonNullableType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 返回是否数值类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumeric(this Type type)
        {
            type = type.GetNonNullableType();
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        return true;
                }
            }
            return false;
        }

        public static bool IsIntegerOrBool(this Type type)
        {
            type = type.GetNonNullableType();
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        public static bool IsStringOrDateTime(this Type type)
        {
            type = type.GetNonNullableType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                case TypeCode.DateTime:
                    return true;
            }

            return false;
        }

        public static bool IsInteger(this Type type)
        {
            type = type.GetNonNullableType();
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        public static bool IsConvertible(this Type type)
        {
            type = type.GetNonNullableType();
            if (type.IsEnum)
            {
                return true;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.String:
                case TypeCode.DateTime:
                    return true;
            }
            return false;
        }

        public static bool IsStringable(this Type type)
        {
            type = type.GetNonNullableType();
            if (type.IsEnum || type == typeof(Guid))
            {
                return true;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.String:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    return true;
            }
            return false;
        }

        public static bool IsArithmetic(this Type type)
        {
            type = type.GetNonNullableType();
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        return true;
                }
            }
            return false;
        }

        public static bool IsImplicitNumericConversion(this Type source, Type destination)
        {
            TypeCode typeCode = Type.GetTypeCode(source);
            TypeCode code2 = Type.GetTypeCode(destination);
            switch (typeCode)
            {
                case TypeCode.Char:
                    switch (code2)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.SByte:
                    switch (code2)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;

                case TypeCode.Byte:
                    switch (code2)
                    {
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.Int16:
                    switch (code2)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.UInt16:
                    switch (code2)
                    {
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.Int32:
                    switch (code2)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.UInt32:
                    switch (code2)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    switch (code2)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.Single:
                    return (code2 == TypeCode.Double);

                default:
                    return false;
            }
            return false;
        }

        /// <summary>
        /// 获取 <see cref="Type"/> 对象。
        /// </summary>
        /// <param name="typeName">类型的名称。</param>
        /// <returns>一个 <see cref="Type"/> 对象。</returns>
        public static Type? ParseType(this string typeName)
        {
            var type = Type.GetType(typeName, false, true);
            if (type != null)
            {
                return type;
            }

            if (typeName.IndexOf("Version") == -1)
            {
                return null;
            }

            //如果没有指定Version，则从当前装配件空间取出Version
            var assembly = typeof(string).Assembly.ToString();
            var index = assembly.IndexOf("Version");

            typeName += "," + assembly.Substring(index);

            return Type.GetType(typeName, false, true);
        }

        /// <summary>
        /// 获取类型的默认值。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <returns></returns>
        public static object? GetDefaultValue(this Type type)
        {
            Guard.ArgumentNull(type, nameof(type));

            var isNullable = !type.IsValueType || type.IsNullableType();
            if (!isNullable)
            {
                return System.Activator.CreateInstance(type);
            }

            return null;
        }

        /// <summary>
        /// 获取类型的空值。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <returns></returns>
        public static object GetEmptyValue(this Type type)
        {
            Guard.ArgumentNull(type, nameof(type));

            var fiend = type.GetField("Empty");
            if (fiend != null)
            {
                return fiend.GetValue(null);
            }

            return System.Activator.CreateInstance(type.GetNonNullableType());
        }

        /// <summary>
        /// 判断类型是否为可空。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            Guard.ArgumentNull(type, nameof(type));
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>
        /// 判断类型是否为实现类，即区别于抽象的类型。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <returns></returns>
        public static bool IsConcreteType(this Type type)
        {
            return !type.IsAbstract && !type.IsGenericTypeDefinition && !type.IsArray;
        }

        /// <summary>
        /// 获取指定类型的继承层次，包括实现的接口。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type[] GetHierarchyTypes(this Type type)
        {
            var types = new List<Type> { type };
            types.AddRange(type.EachBaseTypes());
            types.AddRange(type.GetInterfaces());
            return types.ToArray();
        }

        /// <summary>
        /// 枚举出指定类型的所有父类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> EachBaseTypes(this Type type)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        /// <summary>
        /// 获取 <see cref="Nullable"/> 类型中的类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNonNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// 包装类型的 <see cref="Nullable"/> 类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNullableType(this Type type)
        {
            if (type.IsValueType && !type.IsNullableType())
            {
                return typeof(Nullable<>).MakeGenericType(new[] { type });
            }

            return type;
        }

        /// <summary>
        /// 判断类型是否为数字类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumericType(this Type type)
        {
            type = type.GetNonNullableType();
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取实现泛型定义类型的基类或接口。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericDefinitionType"></param>
        /// <returns></returns>
        public static Type? GetGenericImplementType(this Type type, Type genericDefinitionType)
        {
            foreach (var t in type.GetHierarchyTypes())
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == genericDefinitionType)
                {
                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// 在父类结构中搜索实现接口类的类型。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <param name="interfaceType">接口类型。</param>
        /// <returns></returns>
        public static Type? GetImplementType(this Type type, Type interfaceType)
        {
            Guard.ArgumentNull(type, nameof(type));
            Guard.ArgumentNull(interfaceType, nameof(interfaceType));
            var baseType = type.BaseType;
            while (baseType != typeof(object))
            {
                if (baseType.IsImplementInterface(interfaceType))
                {
                    return baseType;
                }

                baseType = baseType.BaseType;
            }
            return null;
        }

        /// <summary>
        /// 判断类型是否实现了指定的接口类型。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <param name="interfaceType">接口类型。</param>
        /// <returns></returns>
        public static bool IsImplementInterface(this Type type, Type interfaceType)
        {
            Guard.ArgumentNull(type, nameof(type));
            Guard.ArgumentNull(interfaceType, nameof(interfaceType));
            Guard.Argument(interfaceType.IsInterface, nameof(interfaceType), $"非接口类型 {interfaceType.FullName}。");
            return interfaceType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 判断类型是否直接地实现了指定的接口类型。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <param name="interfaceType">接口类型。</param>
        /// <returns></returns>
        public static bool IsDirectImplementInterface(this Type type, Type interfaceType)
        {
            Guard.ArgumentNull(type, nameof(type));
            Guard.ArgumentNull(interfaceType, nameof(interfaceType));
            Guard.Argument(interfaceType.IsInterface, nameof(interfaceType), $"非接口类型 {interfaceType.FullName}。");
            foreach (var type1 in type.GetInterfaces())
            {
                if (type1 != interfaceType)
                {
                    continue;
                }

                var im = type.GetInterfaceMap(type1);
                if (im.TargetMethods.Length > 0 &&
                    im.TargetMethods[0].DeclaringType == type)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取直接实现的接口类型集合。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetDirectImplementInterfaces(this Type type)
        {
            Guard.ArgumentNull(type, nameof(type));
            foreach (var type1 in type.GetInterfaces())
            {
                var im = type.GetInterfaceMap(type1);
                if (im.TargetMethods.Length > 0 &&
                    im.TargetMethods[0].DeclaringType == type)
                {
                    yield return type1;
                }
            }
        }

        /// <summary>
        /// 获取直接实现指定接口类的接口。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <param name="interfaceType">接口类型。</param>
        /// <returns></returns>
        public static Type? GetDirectImplementInterface(this Type type, Type interfaceType)
        {
            Guard.ArgumentNull(type, nameof(type));
            Guard.ArgumentNull(interfaceType, nameof(interfaceType));
            Guard.Argument(interfaceType.IsInterface, nameof(interfaceType), $"非接口类型 {interfaceType.FullName}。");
            foreach (var type1 in type.GetInterfaces())
            {
                if (interfaceType.IsAssignableFrom(type1))
                {
                    return type1;
                }
            }
            return null;
        }

        /// <summary>
        /// 判断是否为匿名类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAnonymousType(this Type type)
        {
            Guard.ArgumentNull(type, nameof(type));

            var fullName = type.FullName;
            return fullName.Length > 18 && fullName.Substring(0, 18) == "<>f__AnonymousType";
        }

        /// <summary>
        /// 查找枚举器 <see cref="TValue:System.Collections.Generic.IEnumerable`1"/> 的元素类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type? GetEnumerableElementType(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.GetElementType();
            }

            var ienum = GetEnumerableType(type);
            return ienum?.GetGenericArguments()[0];
        }

        /// <summary>
        /// 获取元素的 <see cref="TValue:System.Collections.Generic.IEnumerable`1"/> 类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type? GetEnumerableType(this Type type)
        {
            if (type == null || type == typeof(string))
            {
                return null;
            }

            if (type.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());
            }

            if (type.IsGenericType)
            {
                foreach (var arg in type.GetGenericArguments())
                {
                    var ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(type))
                    {
                        return ienum;
                    }
                }
            }

            var ifaces = type.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (var iface in ifaces)
                {
                    var ienum = GetEnumerableType(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                return GetEnumerableType(type.BaseType);
            }

            return null;
        }

        /// <summary>
        /// 获取 <see cref="MemberInfo"/> 的类型。
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static Type? GetMemberType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return (member as PropertyInfo).PropertyType;
                case MemberTypes.Field:
                    return (member as FieldInfo).FieldType;
                case MemberTypes.Event:
                    return (member as EventInfo).EventHandlerType;
                case MemberTypes.Method:
                    return (member as MethodInfo).ReturnType;
                case MemberTypes.Constructor:
                    return (member as ConstructorInfo).DeclaringType;
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// 获取 <see cref="MemberInfo"/> 的值。
        /// </summary>
        /// <param name="member"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object? GetMemberValue(this MemberInfo member, object instance)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return (member as PropertyInfo).GetValue(instance, null);
                case MemberTypes.Field:
                    return (member as FieldInfo).GetValue(instance);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取自定义特性组。
        /// </summary>
        /// <typeparam name="T">自定义特性类型。</typeparam>
        /// <param name="provider">要搜索的特性类型。</param>
        /// <param name="inherit">指定是否搜索该成员的继承链以查找这些特性。</param>
        /// <returns></returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider provider, bool inherit = false) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        /// <summary>
        /// 判定是否定义了自定义特性。
        /// </summary>
        /// <typeparam name="T">自定义特性类型。</typeparam>
        /// <param name="provider">要搜索的特性类型。</param>
        /// <param name="inherit">指定是否搜索该成员的继承链以查找这些特性。</param>
        /// <returns></returns>
        public static bool IsDefined<T>(this ICustomAttributeProvider provider, bool inherit = false) where T : Attribute
        {
            return provider.IsDefined(typeof(T), inherit);
        }

        public static bool IsValidStructuralPropertyType(this Type type)
        {
            return ((!type.IsGenericTypeDefinition && !type.IsPointer) && !(type == typeof(object)));
        }

        public static bool IsValidStructuralType(this Type type)
        {
            return ((((!type.IsGenericType && !type.IsValueType) && (!type.IsPrimitive && !type.IsInterface)) && (!type.IsArray && !(type == typeof(string)))) && type.IsValidStructuralPropertyType());
        }

        /// <summary>
        /// 构造一个实现类。
        /// </summary>
        /// <param name="definedType">定义的接口或父类。</param>
        /// <returns></returns>
        public static Type BuildImplementType(this Type definedType)
        {
            Guard.ArgumentNull(definedType, nameof(definedType));

            return InternalBuildImplementType(definedType);
        }

        /// <summary>
        /// 获取 Task 中泛型的参数。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type? GetTaskReturnType(this Type type)
        {
            if (type.IsGenericType)
            {
                var gdtype = type.GetGenericTypeDefinition();
                if (gdtype == typeof(Task<>))
                {
                    return type.GetGenericArguments()[0];
                }

                if (gdtype == typeof(ValueTask<>))
                {
                    return type.GetGenericArguments()[0];
                }
            }

            return null;
        }

        /// <summary>
        /// 判断是否为 Task 返回的类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsTaskReturnType(this Type type)
        {
            if (type.IsGenericType)
            {
                var gdtype = type.GetGenericTypeDefinition();
                return gdtype == typeof(ValueTask<>) || gdtype == typeof(Task<>);
            }

            return type == typeof(Task) || type == typeof(ValueTask);
        }

        /// <summary>
        /// 判断是否为 ValueTask 返回的类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsValueTaskReturnType(this Type type)
        {
            if (type.IsGenericType)
            {
                var gdtype = type.GetGenericTypeDefinition();
                return gdtype == typeof(ValueTask<>);
            }

            return type == typeof(ValueTask);
        }

        /// <summary>
        /// 判断方法是否具有 async 标识。
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsAsynchronous(this MethodInfo method)
        {
            return method.IsDefined(typeof(AsyncStateMachineAttribute));
        }

        /// <summary>
        /// 获取接口定义的属性。
        /// </summary>
        /// <param name="interfaceType">接口类型。</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetDefinedProperties(this Type interfaceType)
        {
            var binding = BindingFlags.Public | BindingFlags.Instance;
            var properties = interfaceType.GetProperties(binding).AsEnumerable();
            foreach (var implType in interfaceType.GetInterfaces())
            {
                properties = properties.Union(implType.GetProperties(binding));
            }

            return properties;
        }

        /// <summary>
        /// 递归枚举所有引用的程序集。
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="finder"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static void ForEachAssemblies(this Assembly assembly, Action<Assembly> finder, Func<Assembly, bool> filter)
        {
            Guard.ArgumentNull(assembly, nameof(assembly));
            Guard.ArgumentNull(finder, nameof(finder));

            var assemblies = new List<Assembly>();
            ForEachAssemblies(assembly, assemblies, finder, filter);
        }

        private static void ForEachAssemblies(this Assembly assembly, List<Assembly> assemblies, Action<Assembly> finder, Func<Assembly, bool> filter)
        {
            foreach (var asb in assembly.GetReferencedAssemblies()
                .Select(s => LoadAssembly(s))
                .Where(s => s != null)
                .Where(filter))
            {
                if (!assemblies.Contains(asb))
                {
                    finder?.Invoke(asb);
                    assemblies.Add(asb);

                    ForEachAssemblies(asb, assemblies, finder, filter);
                }
            }
        }

        private static Assembly? LoadAssembly(AssemblyName assemblyName)
        {
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch
            {
                return null;
            }
        }

        private static Type InternalBuildImplementType(Type definedType)
        {
            var assemblyBuilder = new DynamicAssemblyBuilder("ImplAssembly");
            var typeBuilder = assemblyBuilder.DefineType(definedType.Name + "_Impl");

            if (definedType.IsInterface)
            {
                typeBuilder.ImplementInterface(definedType);
            }
            else
            {
                typeBuilder.BaseType = definedType;
            }

            //实现属性
            foreach (var property in definedType.GetDefinedProperties())
            {
                if (property.Name == "Item")
                {
                    continue;
                }

                var getMethod = property.GetGetMethod();
                var setMethod = property.GetSetMethod();
                if (definedType.IsInterface || (getMethod != null && getMethod.IsVirtual) || (setMethod != null && setMethod.IsVirtual))
                {
                    var propertyBuilder = typeBuilder.DefineProperty(property.Name, property.PropertyType).DefineGetSetMethods();
                }
            }

            //实现方法
            foreach (var method in definedType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if ((definedType.IsInterface || method.IsVirtual) && !Regex.IsMatch(method.Name, @"^set_|get_"))
                {
                    var parameters = method.GetParameters();
                    var methodBuilder = typeBuilder.DefineMethod(method.Name, method.ReturnType, parameters.Select(s => s.ParameterType).ToArray());

                    foreach (var par in parameters)
                    {
                        methodBuilder.DefineParameter(par.Name, par.IsOut, par.DefaultValue != DBNull.Value, par.DefaultValue);
                    }
                }
            }

            return typeBuilder.CreateType();
        }
    }
}
