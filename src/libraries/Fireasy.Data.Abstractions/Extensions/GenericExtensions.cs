// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;

namespace Fireasy.Data
{
    /// <summary>
    /// 通用扩展类。
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// 获取类型所对应的 <see cref="DbType"/> 类型。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <returns></returns>
        public static DbType GetDbType(this Type type)
        {
            Guard.ArgumentNull(type, nameof(type));
            if (type.IsNullableType())
            {
                var baseType = type.GetGenericArguments()[0];
                return GetDbType(baseType);
            }

            if (type.IsArray)
            {
                return DbType.Binary;
            }

            return type.IsEnum ? DbType.Int32 : GetGenericDbType(type);
        }

        /// <summary>
        /// 获取数据类型的大小。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <param name="defaultSize">默认大小。</param>
        /// <returns></returns>
        public static int GetDbTypeSize(this Type type, int? defaultSize)
        {
            Guard.ArgumentNull(type, nameof(type));
            if (type == typeof(string))
            {
                if (defaultSize != null)
                {
                    return (int)defaultSize;
                }

                return 0;
            }

            if (type == typeof(DateTime))
            {
                return 8;
            }

            return System.Runtime.InteropServices.Marshal.SizeOf(type);
        }

        private static DbType GetGenericDbType(Type type)
        {
            var typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.String:
                    return DbType.String;
                case TypeCode.Boolean:
                    return DbType.Boolean;
                case TypeCode.Int16:
                    return DbType.Int16;
                case TypeCode.Int32:
                    return DbType.Int32;
                case TypeCode.Int64:
                    return DbType.Int64;
                case TypeCode.UInt16:
                    return DbType.UInt16;
                case TypeCode.UInt32:
                    return DbType.UInt32;
                case TypeCode.UInt64:
                    return DbType.UInt64;
                case TypeCode.DateTime:
                    return DbType.DateTime;
                case TypeCode.Decimal:
                    return DbType.Decimal;
                case TypeCode.Double:
                    return DbType.Double;
                case TypeCode.Byte:
                    return DbType.Byte;
                case TypeCode.SByte:
                    return DbType.SByte;
                case TypeCode.Char:
                    return DbType.Byte;
                case TypeCode.Single:
                    return DbType.Single;
                default:
                    if (type == typeof(byte[]))
                    {
                        return DbType.Binary;
                    }

                    return DbType.Object;
            }
        }
    }
}
