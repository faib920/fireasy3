// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Fireasy.Common
{
    /// <summary>
    /// 提供一种可自由转换的类型。
    /// </summary>
    public struct FreeValue : IEquatable<FreeValue>, ICloneable, IConvertible
    {
        /// <summary>
        /// 空值。
        /// </summary>
        public readonly static FreeValue Empty = default;

        #region 存储
        /// <summary>
        /// 获取存储数据的实际类型。
        /// </summary>
        private StorageType _storageType;

        private char? _char;

        private bool? _boolean;

        private byte? _byte;

        private sbyte? _sbyte;

        private byte[] _byteArray;

        private DateTime? _dateTime;

        private decimal? _decimal;

        private double? _double;

        private Guid? _guid;

        private short? _int16;

        private int? _int32;

        private long? _int64;

        private ushort? _uint16;

        private uint? _uint32;

        private ulong? _uint64;

        private float? _single;

        private string? _string;

        private Enum? _enum;

        private object? _object;
        #endregion

        #region char
        /// <summary>
        /// 将 <see cref="Char"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(char value)
        {
            return new FreeValue { _storageType = StorageType.Char, _char = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="_char"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator char(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return '\0';
            }

            return value._storageType switch
            {
                StorageType.Byte => (char)(value._byte ?? 0),
                StorageType.SByte => (char)(value._sbyte ?? 0),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(bool).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{Char}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(char? value)
        {
            return new FreeValue { _storageType = StorageType.Char, _char = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Char}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator char?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => (char?)value._byte,
                StorageType.SByte => (char?)value._sbyte,
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(char?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region bool
        /// <summary>
        /// 将 <see cref="Boolean"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(bool value)
        {
            return new FreeValue { _storageType = StorageType.Boolean, _boolean = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Boolean"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator bool(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return false;
            }

            return value._storageType switch
            {
                StorageType.Byte => value._byte > 0,
                StorageType.SByte => value._sbyte > 0,
                StorageType.Boolean => (bool)(value._boolean ?? false),
                StorageType.Int16 => value._int16 > 0,
                StorageType.Int32 => value._int32 > 0,
                StorageType.Int64 => value._int64 > 0,
                StorageType.UInt16 => value._uint16 > 0,
                StorageType.UInt32 => value._uint32 > 0,
                StorageType.UInt64 => value._uint64 > 0,
                StorageType.Single => value._single > 0,
                StorageType.Decimal => value._decimal > 0,
                StorageType.Double => value._double > 0,
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(bool).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{Boolean}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(bool? value)
        {
            return new FreeValue { _storageType = StorageType.Boolean, _boolean = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Boolean}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator bool?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => value._byte > 0,
                StorageType.SByte => value._sbyte > 0,
                StorageType.Boolean => value._boolean,
                StorageType.Int16 => value._int16 > 0,
                StorageType.Int32 => value._int32 > 0,
                StorageType.Int64 => value._int64 > 0,
                StorageType.UInt16 => value._uint16 > 0,
                StorageType.UInt32 => value._uint32 > 0,
                StorageType.UInt64 => value._uint64 > 0,
                StorageType.Single => value._single > 0,
                StorageType.Decimal => value._decimal > 0,
                StorageType.Double => value._double > 0,
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(bool?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region byte
        /// <summary>
        /// 将 <see cref="Byte"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(byte value)
        {
            return new FreeValue { _storageType = StorageType.Byte, _byte = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Byte"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator byte(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0;
            }

            return value._storageType switch
            {
                StorageType.Byte => (byte)(value._byte ?? 0),
                StorageType.SByte => (byte)(value._sbyte ?? 0),
                StorageType.Boolean => (byte)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => (byte)(value._int16 ?? 0),
                StorageType.Int32 => (byte)(value._int32 ?? 0),
                StorageType.Int64 => (byte)(value._int64 ?? 0),
                StorageType.UInt16 => (byte)(value._uint16 ?? 0),
                StorageType.UInt32 => (byte)(value._uint32 ?? 0),
                StorageType.UInt64 => (byte)(value._uint64 ?? 0),
                StorageType.Single => (byte)(value._single ?? 0),
                StorageType.Decimal => (byte)(value._decimal ?? 0),
                StorageType.Double => (byte)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? (byte)0 : ((IConvertible)value._enum).ToByte(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(byte).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{Byte}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(byte? value)
        {
            return new FreeValue { _storageType = StorageType.Byte, _byte = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Byte}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator byte?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => (byte?)value._byte,
                StorageType.SByte => (byte?)value._sbyte,
                StorageType.Boolean => (byte?)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => (byte?)value._int16,
                StorageType.Int32 => (byte?)value._int32,
                StorageType.Int64 => (byte?)value._int64,
                StorageType.UInt16 => (byte?)value._uint16,
                StorageType.UInt32 => (byte?)value._uint32,
                StorageType.UInt64 => (byte?)value._uint64,
                StorageType.Single => (byte?)value._single,
                StorageType.Decimal => (byte?)value._decimal,
                StorageType.Double => (byte?)value._double,
                StorageType.Enum => value._enum == null ? (byte?)null : ((IConvertible)value._enum).ToByte(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(byte?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region sbyte
        /// <summary>
        /// 将 <see cref="SByte"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(sbyte value)
        {
            return new FreeValue { _storageType = StorageType.SByte, _sbyte = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="SByte"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator sbyte(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0;
            }

            return value._storageType switch
            {
                StorageType.Byte => (sbyte)(value._byte ?? 0),
                StorageType.SByte => (sbyte)(value._sbyte ?? 0),
                StorageType.Boolean => (sbyte)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => (sbyte)(value._int16 ?? 0),
                StorageType.Int32 => (sbyte)(value._int32 ?? 0),
                StorageType.Int64 => (sbyte)(value._int64 ?? 0),
                StorageType.UInt16 => (sbyte)(value._uint16 ?? 0),
                StorageType.UInt32 => (sbyte)(value._uint32 ?? 0),
                StorageType.UInt64 => (sbyte)(value._uint64 ?? 0),
                StorageType.Single => (sbyte)(value._single ?? 0),
                StorageType.Decimal => (sbyte)(value._decimal ?? 0),
                StorageType.Double => (sbyte)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? (sbyte)0 : ((IConvertible)value._enum).ToSByte(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(sbyte).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{SByte}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(sbyte? value)
        {
            return new FreeValue { _storageType = StorageType.SByte, _sbyte = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{SByte}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator sbyte?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => (sbyte?)value._byte,
                StorageType.SByte => value._sbyte,
                StorageType.Boolean => (sbyte?)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => (sbyte?)value._int16,
                StorageType.Int32 => (sbyte?)value._int32,
                StorageType.Int64 => (sbyte?)value._int64,
                StorageType.UInt16 => (sbyte?)value._uint16,
                StorageType.UInt32 => (sbyte?)value._uint32,
                StorageType.UInt64 => (sbyte?)value._uint64,
                StorageType.Single => (sbyte?)value._single,
                StorageType.Decimal => (sbyte?)value._decimal,
                StorageType.Double => (sbyte?)value._double,
                StorageType.Enum => value._enum == null ? (sbyte?)null : ((IConvertible)value._enum).ToSByte(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(sbyte).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region bytes
        /// <summary>
        /// 将 <see cref="Byte"/> 数组类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(byte[] value)
        {
            return new FreeValue { _storageType = StorageType.ByteArray, _byteArray = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Byte"/> 数组类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator byte[](FreeValue value)
        {
            if (Equals(value, null))
            {
                return new byte[0];
            }
            return value._object != null ? (byte[])value._object : value._byteArray == null ? new byte[0] : value._byteArray;
        }
        #endregion

        #region DateTime
        /// <summary>
        /// 将 <see cref="DateTime"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(DateTime value)
        {
            return new FreeValue { _storageType = StorageType.DateTime, _dateTime = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="DateTime"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator DateTime(FreeValue value)
        {
            return value == Empty ? DateTime.MinValue : value._dateTime!.Value;
        }

        /// <summary>
        /// 将 <see cref="Nullable{DateTime}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(DateTime? value)
        {
            return new FreeValue { _storageType = StorageType.DateTime, _dateTime = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{DateTime}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator DateTime?(FreeValue value)
        {
            return value == Empty ? null : value._dateTime;
        }
        #endregion

        #region Guid
        /// <summary>
        /// 将 <see cref="Guid"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(Guid value)
        {
            return new FreeValue { _storageType = StorageType.Guid, _guid = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Guid"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Guid(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return Guid.Empty;
            }

            if (value._object != null)
            {
                return (Guid)value._object;
            }
            if (value._guid != null)
            {
                return value._guid.Value;
            }
            if (value._string != null)
            {
                return new Guid(value._string);
            }
            if (value._byteArray != null)
            {
                return new Guid(value._byteArray);
            }

            throw new InvalidCastException($"FreeValue 不支持类型 {typeof(Guid).FullName} 的转换操作。");
        }

        /// <summary>
        /// 将 <see cref="Nullable{Guid}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(Guid? value)
        {
            return new FreeValue { _storageType = StorageType.Guid, _guid = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Guid}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Guid?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            if (value._object != null)
            {
                return (Guid)value._object;
            }
            if (value._guid != null)
            {
                return value._guid.Value;
            }
            if (value._string != null)
            {
                return new Guid(value._string);
            }
            if (value._byteArray != null)
            {
                return new Guid(value._byteArray);
            }

            throw new InvalidCastException($"FreeValue 不支持类型 {typeof(Guid).FullName} 的转换操作。");
        }
        #endregion

        #region short
        /// <summary>
        /// 将 <see cref="Int16"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(short value)
        {
            return new FreeValue { _storageType = StorageType.Int16, _int16 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Int16"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator short(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0;
            }

            return value._storageType switch
            {
                StorageType.Byte => (short)(value._byte ?? 0),
                StorageType.SByte => (short)(value._sbyte ?? 0),
                StorageType.Boolean => (short)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => (short)(value._int16 ?? 0),
                StorageType.Int32 => (short)(value._int32 ?? 0),
                StorageType.Int64 => (short)(value._int64 ?? 0),
                StorageType.UInt16 => (short)(value._uint16 ?? 0),
                StorageType.UInt32 => (short)(value._uint32 ?? 0),
                StorageType.UInt64 => (short)(value._uint64 ?? 0),
                StorageType.Single => (short)(value._single ?? 0),
                StorageType.Decimal => (short)(value._decimal ?? 0),
                StorageType.Double => (short)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? (short)0 : ((IConvertible)value._enum).ToInt16(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(short).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{Int16}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(short? value)
        {
            return new FreeValue { _storageType = StorageType.Int16, _int16 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Int16}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator short?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => value._byte,
                StorageType.SByte => value._sbyte,
                StorageType.Boolean => (short?)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => value._int16,
                StorageType.Int32 => (short?)value._int32,
                StorageType.Int64 => (short?)value._int64,
                StorageType.UInt16 => (short?)value._uint16,
                StorageType.UInt32 => (short?)value._uint32,
                StorageType.UInt64 => (short?)value._uint64,
                StorageType.Single => (short?)value._single,
                StorageType.Decimal => (short?)value._decimal,
                StorageType.Double => (short?)value._double,
                StorageType.Enum => value._enum == null ? (short?)null : ((IConvertible)value._enum).ToInt16(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(short?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region ushort
        /// <summary>
        /// 将 <see cref="UInt16"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(ushort value)
        {
            return new FreeValue { _storageType = StorageType.UInt16, _uint16 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="UInt16"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator ushort(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0;
            }

            return value._storageType switch
            {
                StorageType.Byte => (ushort)(value._byte ?? 0),
                StorageType.SByte => (ushort)(value._sbyte ?? 0),
                StorageType.Boolean => (ushort)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => (ushort)(value._int16 ?? 0),
                StorageType.Int32 => (ushort)(value._int32 ?? 0),
                StorageType.Int64 => (ushort)(value._int64 ?? 0),
                StorageType.UInt16 => (ushort)(value._uint16 ?? 0),
                StorageType.UInt32 => (ushort)(value._uint32 ?? 0),
                StorageType.UInt64 => (ushort)(value._uint64 ?? 0),
                StorageType.Single => (ushort)(value._single ?? 0),
                StorageType.Decimal => (ushort)(value._decimal ?? 0),
                StorageType.Double => (ushort)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? (ushort)0 : ((IConvertible)value._enum).ToUInt16(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(ushort).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{UInt16}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(ushort? value)
        {
            return new FreeValue { _storageType = StorageType.UInt16, _uint16 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{UInt16}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator ushort?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => value._byte,
                StorageType.SByte => (ushort?)value._sbyte,
                StorageType.Boolean => value._boolean == null ? (ushort?)null : (ushort)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => (ushort?)value._int16,
                StorageType.Int32 => (ushort?)value._int32,
                StorageType.Int64 => (ushort?)value._int64,
                StorageType.UInt16 => value._uint16,
                StorageType.UInt32 => (ushort?)value._uint32,
                StorageType.UInt64 => (ushort?)value._uint64,
                StorageType.Single => (ushort?)value._single,
                StorageType.Decimal => (ushort?)value._decimal,
                StorageType.Double => (ushort?)value._double,
                StorageType.Enum => value._enum == null ? (ushort?)null : ((IConvertible)value._enum).ToUInt16(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(ushort?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region int
        /// <summary>
        /// 将 <see cref="Int32"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(int value)
        {
            return new FreeValue { _storageType = StorageType.Int32, _int32 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Int32"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator int(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0;
            }

            return value._storageType switch
            {
                StorageType.Byte => (int)(value._byte ?? 0),
                StorageType.SByte => (int)(value._sbyte ?? 0),
                StorageType.Boolean => value._boolean == true ? 1 : 0,
                StorageType.Int16 => (int)(value._int16 ?? 0),
                StorageType.Int32 => (int)(value._int32 ?? 0),
                StorageType.Int64 => (int)(value._int64 ?? 0),
                StorageType.UInt16 => (int)(value._uint16 ?? 0),
                StorageType.UInt32 => (int)(value._uint32 ?? 0),
                StorageType.UInt64 => (int)(value._uint64 ?? 0),
                StorageType.Single => (int)(value._single ?? 0),
                StorageType.Decimal => (int)(value._decimal ?? 0),
                StorageType.Double => (int)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? 0 : ((IConvertible)value._enum).ToInt32(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(int).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{Int32}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(int? value)
        {
            return new FreeValue { _storageType = StorageType.Int32, _int32 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Int32}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator int?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => value._byte,
                StorageType.SByte => value._sbyte,
                StorageType.Boolean => value._boolean == null ? (int?)null : (value._boolean == true ? 1 : 0),
                StorageType.Int16 => value._int16,
                StorageType.Int32 => value._int32,
                StorageType.Int64 => (int?)value._int64,
                StorageType.UInt16 => value._uint16,
                StorageType.UInt32 => (int?)value._uint32,
                StorageType.UInt64 => (int?)value._uint64,
                StorageType.Single => (int?)value._single,
                StorageType.Decimal => (int?)value._decimal,
                StorageType.Double => (int?)value._double,
                StorageType.Enum => value._enum == null ? (int?)null : ((IConvertible)value._enum).ToInt32(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(int?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region uint
        /// <summary>
        /// 将 <see cref="UInt32"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(uint value)
        {
            return new FreeValue { _storageType = StorageType.UInt32, _uint32 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="UInt32"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator uint(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0;
            }

            return value._storageType switch
            {
                StorageType.Byte => (uint)(value._byte ?? 0),
                StorageType.SByte => (uint)(value._sbyte ?? 0),
                StorageType.Boolean => (uint)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => (uint)(value._int16 ?? 0),
                StorageType.Int32 => (uint)(value._int32 ?? 0),
                StorageType.Int64 => (uint)(value._int64 ?? 0),
                StorageType.UInt16 => (uint)(value._uint16 ?? 0),
                StorageType.UInt32 => (uint)(value._uint32 ?? 0),
                StorageType.UInt64 => (uint)(value._uint64 ?? 0),
                StorageType.Single => (uint)(value._single ?? 0),
                StorageType.Decimal => (uint)(value._decimal ?? 0),
                StorageType.Double => (uint)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? 0 : ((IConvertible)value._enum).ToUInt32(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(uint).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{UInt32}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(uint? value)
        {
            return new FreeValue { _storageType = StorageType.UInt32, _uint32 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{UInt32}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator uint?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => value._byte,
                StorageType.SByte => (uint?)value._sbyte,
                StorageType.Boolean => value._boolean == null ? (uint?)null : (uint)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => (uint?)value._int16,
                StorageType.Int32 => (uint?)value._int32,
                StorageType.Int64 => (uint?)value._int64,
                StorageType.UInt16 => value._uint16,
                StorageType.UInt32 => value._uint32,
                StorageType.UInt64 => (uint?)value._uint64,
                StorageType.Single => (uint?)value._single,
                StorageType.Decimal => (uint?)value._decimal,
                StorageType.Double => (uint?)value._double,
                StorageType.Enum => value._enum == null ? (uint?)null : ((IConvertible)value._enum).ToUInt32(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(uint?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region long
        /// <summary>
        /// 将 <see cref="Int64"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(long value)
        {
            return new FreeValue { _storageType = StorageType.Int64, _int64 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Int64"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator long(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0L;
            }

            return value._storageType switch
            {
                StorageType.Byte => (long)(value._byte ?? 0),
                StorageType.SByte => (long)(value._sbyte ?? 0),
                StorageType.Boolean => value._boolean == true ? 1L : 0L,
                StorageType.Int16 => (long)(value._int16 ?? 0),
                StorageType.Int32 => (long)(value._int32 ?? 0),
                StorageType.Int64 => (long)(value._int64 ?? 0),
                StorageType.UInt16 => (long)(value._uint16 ?? 0),
                StorageType.UInt32 => (long)(value._uint32 ?? 0),
                StorageType.UInt64 => (long)(value._uint64 ?? 0),
                StorageType.Single => (long)(value._single ?? 0),
                StorageType.Decimal => (long)(value._decimal ?? 0),
                StorageType.Double => (long)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? 0 : ((IConvertible)value._enum).ToInt64(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(long).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{Int64}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(long? value)
        {
            return new FreeValue { _storageType = StorageType.Int64, _int64 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Int64}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator long?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => value._byte,
                StorageType.SByte => value._sbyte,
                StorageType.Boolean => value._boolean == null ? (long?)null : (value._boolean == true ? 1L : 0L),
                StorageType.Int16 => value._int16,
                StorageType.Int32 => value._int32,
                StorageType.Int64 => value._int64,
                StorageType.UInt16 => value._uint16,
                StorageType.UInt32 => value._uint32,
                StorageType.UInt64 => (long?)value._uint64,
                StorageType.Single => (long?)value._single,
                StorageType.Decimal => (long?)value._decimal,
                StorageType.Double => (long?)value._double,
                StorageType.Enum => value._enum == null ? (long?)null : ((IConvertible)value._enum).ToInt64(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(long?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region ulong
        /// <summary>
        /// 将 <see cref="UInt64"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(ulong value)
        {
            return new FreeValue { _storageType = StorageType.UInt64, _uint64 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="UInt64"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator ulong(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0L;
            }

            return value._storageType switch
            {
                StorageType.Byte => (ulong)(value._byte ?? 0),
                StorageType.SByte => (ulong)(value._sbyte ?? 0),
                StorageType.Boolean => (ulong)(value._boolean == true ? 1L : 0L),
                StorageType.Int16 => (ulong)(value._int16 ?? 0),
                StorageType.Int32 => (ulong)(value._int32 ?? 0),
                StorageType.Int64 => (ulong)(value._int64 ?? 0),
                StorageType.UInt16 => (ulong)(value._uint16 ?? 0),
                StorageType.UInt32 => (ulong)(value._uint32 ?? 0),
                StorageType.UInt64 => (ulong)(value._uint64 ?? 0),
                StorageType.Single => (ulong)(value._single ?? 0),
                StorageType.Decimal => (ulong)(value._decimal ?? 0),
                StorageType.Double => (ulong)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? 0 : ((IConvertible)value._enum).ToUInt64(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(ulong).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{UInt64}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(ulong? value)
        {
            return new FreeValue { _storageType = StorageType.UInt64, _uint64 = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Int64}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator ulong?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => value._byte,
                StorageType.SByte => (ulong?)value._sbyte,
                StorageType.Boolean => value._boolean == null ? (ulong?)null : (ulong)(value._boolean == true ? 1 : 0),
                StorageType.Int16 => (ulong?)value._int16,
                StorageType.Int32 => (ulong?)value._int32,
                StorageType.Int64 => (ulong?)value._int64,
                StorageType.UInt16 => value._uint16,
                StorageType.UInt32 => value._uint32,
                StorageType.UInt64 => value._uint64,
                StorageType.Single => (ulong?)value._single,
                StorageType.Decimal => (ulong?)value._decimal,
                StorageType.Double => (ulong?)value._double,
                StorageType.Enum => value._enum == null ? (ulong?)null : ((IConvertible)value._enum).ToUInt64(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(ulong?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region decimal
        /// <summary>
        /// 将 <see cref="Decimal"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(decimal value)
        {
            return new FreeValue { _storageType = StorageType.Decimal, _decimal = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Decimal"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator decimal(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0m;
            }

            return value._storageType switch
            {
                StorageType.Byte => (decimal)(value._byte ?? 0),
                StorageType.SByte => (decimal)(value._sbyte ?? 0),
                StorageType.Boolean => value._boolean == true ? 1 : 0,
                StorageType.Int16 => (decimal)(value._int16 ?? 0),
                StorageType.Int32 => (decimal)(value._int32 ?? 0),
                StorageType.Int64 => (decimal)(value._int64 ?? 0),
                StorageType.UInt16 => (decimal)(value._uint16 ?? 0),
                StorageType.UInt32 => (decimal)(value._uint32 ?? 0),
                StorageType.UInt64 => (decimal)(value._uint64 ?? 0),
                StorageType.Single => (decimal)(value._single ?? 0),
                StorageType.Decimal => (decimal)(value._decimal ?? 0),
                StorageType.Double => (decimal)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? 0m : ((IConvertible)value._enum).ToDecimal(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(decimal).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{Decimal}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(decimal? value)
        {
            return new FreeValue { _storageType = StorageType.Decimal, _decimal = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Decimal}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator decimal?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => value._byte,
                StorageType.SByte => value._sbyte,
                StorageType.Boolean => value._boolean == null ? (decimal?)null : (value._boolean == true ? 1m : 0m),
                StorageType.Int16 => value._int16,
                StorageType.Int32 => value._int32,
                StorageType.Int64 => value._int64,
                StorageType.UInt16 => value._uint16,
                StorageType.UInt32 => value._uint32,
                StorageType.UInt64 => value._uint64,
                StorageType.Single => (decimal?)value._single,
                StorageType.Decimal => value._decimal,
                StorageType.Double => (decimal?)value._double,
                StorageType.Enum => value._enum == null ? (decimal?)null : ((IConvertible)value._enum).ToDecimal(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(decimal?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region float
        /// <summary>
        /// 将 <see cref="Single"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(float value)
        {
            return new FreeValue { _storageType = StorageType.Single, _single = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Single"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator float(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0;
            }

            return value._storageType switch
            {
                StorageType.Byte => (float)(value._byte ?? 0),
                StorageType.SByte => (float)(value._sbyte ?? 0),
                StorageType.Boolean => value._boolean == true ? 1 : 0,
                StorageType.Int16 => (float)(value._int16 ?? 0),
                StorageType.Int32 => (float)(value._int32 ?? 0),
                StorageType.Int64 => (float)(value._int64 ?? 0),
                StorageType.UInt16 => (float)(value._uint16 ?? 0),
                StorageType.UInt32 => (float)(value._uint32 ?? 0),
                StorageType.UInt64 => (float)(value._uint64 ?? 0),
                StorageType.Single => (float)(value._single ?? 0),
                StorageType.Decimal => (float)(value._decimal ?? 0),
                StorageType.Double => (float)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? 0f : ((IConvertible)value._enum).ToSingle(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(float).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{Single}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(float? value)
        {
            return new FreeValue { _storageType = StorageType.Single, _single = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Single}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator float?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => (float?)value._byte,
                StorageType.SByte => (float?)value._sbyte,
                StorageType.Boolean => value._boolean == null ? (float?)null : (value._boolean == true ? 1f : 0f),
                StorageType.Int16 => (float?)value._int16,
                StorageType.Int32 => (float?)value._int32,
                StorageType.Int64 => (float?)value._int64,
                StorageType.UInt16 => (float?)value._uint16,
                StorageType.UInt32 => (float?)value._uint32,
                StorageType.UInt64 => (float?)value._uint64,
                StorageType.Single => (float?)value._single,
                StorageType.Decimal => (float?)value._decimal,
                StorageType.Double => (float?)value._double,
                StorageType.Enum => value._enum == null ? (float?)null : ((IConvertible)value._enum).ToSingle(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(float?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region double
        /// <summary>
        /// 将 <see cref="Double"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(double value)
        {
            return new FreeValue { _storageType = StorageType.Double, _double = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Double"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator double(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return 0d;
            }

            return value._storageType switch
            {
                StorageType.Byte => (double)(value._byte ?? 0),
                StorageType.SByte => (double)(value._sbyte ?? 0),
                StorageType.Boolean => value._boolean == true ? 1 : 0,
                StorageType.Int16 => (double)(value._int16 ?? 0),
                StorageType.Int32 => (double)(value._int32 ?? 0),
                StorageType.Int64 => (double)(value._int64 ?? 0),
                StorageType.UInt16 => (double)(value._uint16 ?? 0),
                StorageType.UInt32 => (double)(value._uint32 ?? 0),
                StorageType.UInt64 => (double)(value._uint64 ?? 0),
                StorageType.Single => (double)(value._single ?? 0),
                StorageType.Decimal => (double)(value._decimal ?? 0),
                StorageType.Double => (double)(value._double ?? 0),
                StorageType.Enum => value._enum == null ? 0 : ((IConvertible)value._enum).ToDouble(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(double).FullName} 的转换操作。"),
            };
        }

        /// <summary>
        /// 将 <see cref="Nullable{Double}"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(double? value)
        {
            return new FreeValue { _storageType = StorageType.Double, _double = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Nullable{Double}"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator double?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Byte => value._byte,
                StorageType.SByte => value._sbyte,
                StorageType.Boolean => value._boolean == null ? (double?)null : (value._boolean == true ? 1d : 0d),
                StorageType.Int16 => value._int16,
                StorageType.Int32 => value._int32,
                StorageType.Int64 => value._int64,
                StorageType.UInt16 => value._uint16,
                StorageType.UInt32 => value._uint32,
                StorageType.UInt64 => value._uint64,
                StorageType.Single => value._single,
                StorageType.Decimal => (double?)value._decimal,
                StorageType.Double => value._double,
                StorageType.Enum => value._enum == null ? (double?)null : ((IConvertible)value._enum).ToDouble(null),
                _ => throw new InvalidCastException($"FreeValue 不支持类型 {typeof(double?).FullName} 的转换操作。"),
            };
        }
        #endregion

        #region string
        /// <summary>
        /// 将 <see cref="String"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(string? value)
        {
            return new FreeValue { _storageType = StorageType.String, _string = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="String"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator string?(FreeValue value)
        {
            return value == Empty ? null : value._string;
        }
        #endregion

        #region enum
        /// <summary>
        /// 将 <see cref="Enum"/> 类型隐式转换为 <see cref="FreeValue"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FreeValue(Enum value)
        {
            return new FreeValue { _storageType = StorageType.Enum, _enum = value };
        }

        /// <summary>
        /// 将 <see cref="FreeValue"/> 类型显示转换为 <see cref="Enum"/> 类型。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator Enum?(FreeValue value)
        {
            if (IsEmpty(value))
            {
                return null;
            }

            return value._storageType switch
            {
                StorageType.Enum => value._enum,
                StorageType.Object => value._object as Enum,
                _ => throw new InvalidCastException($"FreeValue 不支持类型 Enum 的转换操作。"),
            };
        }
        #endregion

        #region ==和!=
        /// <summary>
        /// 两个 <see cref="FreeValue"/> 是否相等。
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator ==(FreeValue v1, FreeValue v2)
        {
            if (v1._storageType != v2._storageType)
            {
                return false;
            }

            return v1._storageType switch
            {
                StorageType.Boolean => v1._boolean == v2._boolean,
                StorageType.Byte => v1._byte == v2._byte,
                StorageType.SByte => v1._sbyte == v2._sbyte,
                StorageType.Char => v1._char == v2._char,
                StorageType.DateTime => v1._dateTime == v2._dateTime,
                StorageType.Decimal => v1._decimal == v2._decimal,
                StorageType.Double => v1._double == v2._double,
                StorageType.Enum => v1._enum == v2._enum,
                StorageType.Guid => v1._guid == v2._guid,
                StorageType.Int16 => v1._int16 == v2._int16,
                StorageType.Int32 => v1._int32 == v2._int32,
                StorageType.Int64 => v1._int64 == v2._int64,
                StorageType.UInt16 => v1._uint16 == v2._uint16,
                StorageType.UInt32 => v1._uint32 == v2._uint32,
                StorageType.UInt64 => v1._uint64 == v2._uint64,
                StorageType.Single => v1._single == v2._single,
                StorageType.String => v1._string == v2._string,
                StorageType.ByteArray => ByteEqueals(v1._byteArray, v2._byteArray),
                _ => v1._object == v2._object,
            };
        }

        private static bool ByteEqueals(byte[] b1, byte[] b2)
        {
            if (b1 == null && b2 == null)
            {
                return true;
            }

            if (b1 == null && b2 != null)
            {
                return false;
            }

            if (b1 != null && b2 == null)
            {
                return false;
            }

            if (b1?.Length != b2?.Length)
            {
                return false;
            }

            for (int i = 0, n = b1.Length; i < n; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 两个 <see cref="FreeValue"/> 是否不相等。
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator !=(FreeValue v1, FreeValue v2)
        {
            return !(v1 == v2);
        }

        #region int equals
        /// <summary>
        /// <see cref="FreeValue"/> 是否和 int 相等。
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool operator ==(FreeValue v1, int v2)
        {
            if (v1._storageType != StorageType.Int32)
            {
                return false;
            }

            return v1._int32 == v2;
        }

        public static bool operator !=(FreeValue v1, int v2)
        {
            if (v1._storageType != StorageType.Int32)
            {
                return true;
            }

            return v1._int32 != v2;
        }

        public static bool operator ==(int v1, FreeValue v2)
        {
            if (v2._storageType != StorageType.Int32)
            {
                return false;
            }

            return v2._int32 == v1;
        }

        public static bool operator !=(int v1, FreeValue v2)
        {
            if (v2._storageType != StorageType.Int32)
            {
                return true;
            }

            return v2._int32 != v1;
        }
        #endregion

        #region uint equals
        public static bool operator ==(FreeValue v1, uint v2)
        {
            if (v1._storageType != StorageType.UInt32)
            {
                return false;
            }

            return v1._uint32 == v2;
        }

        public static bool operator !=(FreeValue v1, uint v2)
        {
            if (v1._storageType != StorageType.UInt32)
            {
                return true;
            }

            return v1._uint32 != v2;
        }

        public static bool operator ==(uint v1, FreeValue v2)
        {
            if (v2._storageType != StorageType.UInt32)
            {
                return false;
            }

            return v2._uint32 == v1;
        }

        public static bool operator !=(uint v1, FreeValue v2)
        {
            if (v2._storageType != StorageType.UInt32)
            {
                return true;
            }

            return v2._uint32 != v1;
        }
        #endregion

        #region long equals
        public static bool operator ==(FreeValue v1, long v2)
        {
            if (v1._storageType != StorageType.Int64)
            {
                return false;
            }

            return v1._int64 == v2;
        }

        public static bool operator !=(FreeValue v1, long v2)
        {
            if (v1._storageType != StorageType.Int64)
            {
                return true;
            }

            return v1._int64 != v2;
        }

        public static bool operator ==(long v1, FreeValue v2)
        {
            if (v2._storageType != StorageType.Int64)
            {
                return false;
            }

            return v2._int64 == v1;
        }

        public static bool operator !=(long v1, FreeValue v2)
        {
            if (v2._storageType != StorageType.Int64)
            {
                return true;
            }

            return v2._int64 != v1;
        }
        #endregion

        #region ulong equals
        public static bool operator ==(FreeValue v1, ulong v2)
        {
            if (v1._storageType != StorageType.UInt64)
            {
                return false;
            }

            return v1._uint64 == v2;
        }

        public static bool operator !=(FreeValue v1, ulong v2)
        {
            if (v1._storageType != StorageType.UInt64)
            {
                return true;
            }

            return v1._uint64 != v2;
        }

        public static bool operator ==(ulong v1, FreeValue v2)
        {
            if (v2._storageType != StorageType.UInt64)
            {
                return false;
            }

            return v2._uint64 == v1;
        }

        public static bool operator !=(ulong v1, FreeValue v2)
        {
            if (v2._storageType != StorageType.UInt64)
            {
                return true;
            }

            return v2._uint64 != v1;
        }
        #endregion

        #region string equals
        public static bool operator ==(FreeValue v1, string v2)
        {
            if (v1._storageType != StorageType.String)
            {
                return false;
            }

            return v1._string == v2;
        }

        public static bool operator !=(FreeValue v1, string v2)
        {
            if (v1._storageType != StorageType.String)
            {
                return true;
            }

            return v1._string != v2;
        }

        public static bool operator ==(string v1, FreeValue v2)
        {
            if (v2._storageType != StorageType.String)
            {
                return false;
            }

            return v2._string == v1;
        }

        public static bool operator !=(string v1, FreeValue v2)
        {
            if (v2._storageType != StorageType.String)
            {
                return true;
            }

            return v2._string != v1;
        }
        #endregion
        #endregion

        /// <summary>
        /// 判断是否为空。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmpty(FreeValue value)
        {
            return value == Empty || value._storageType == StorageType.Empty;
        }

        #region ICloneable
        /// <summary>
        /// 克隆该对象副本。
        /// </summary>
        /// <returns></returns>
        public FreeValue Clone()
        {
            switch (_storageType)
            {
                case StorageType.Boolean: return _boolean;
                case StorageType.Byte: return _byte;
                case StorageType.SByte: return _sbyte;
                case StorageType.Char: return _char;
                case StorageType.DateTime: return _dateTime;
                case StorageType.Decimal: return _decimal;
                case StorageType.Double: return _double;
                case StorageType.Enum: return _enum;
                case StorageType.Guid: return _guid;
                case StorageType.Int16: return _int16;
                case StorageType.Int32: return _int32;
                case StorageType.Int64: return _int64;
                case StorageType.UInt16: return _uint16;
                case StorageType.UInt32: return _uint32;
                case StorageType.UInt64: return _uint64;
                case StorageType.Single: return _single;
                case StorageType.String: return string.IsNullOrEmpty(_string) ? string.Empty : string.Copy(_string);
                default:
                    if (_object == null || _object is not ICloneable cloneable)
                    {
                        return Empty;
                    }

                    return new FreeValue { _storageType = StorageType.Object, _object = cloneable == null ? _object : cloneable.Clone() };
            }
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region IEquatable
        bool IEquatable<FreeValue>.Equals(FreeValue other)
        {
            return this == other;
        }
        #endregion

        #region IConvertible
        TypeCode IConvertible.GetTypeCode()
        {
            return _storageType switch
            {
                StorageType.Byte => TypeCode.Byte,
                StorageType.SByte => TypeCode.SByte,
                StorageType.Boolean => TypeCode.Boolean,
                StorageType.Int16 => TypeCode.Int16,
                StorageType.Int32 => TypeCode.Int32,
                StorageType.Int64 => TypeCode.Int64,
                StorageType.UInt16 => TypeCode.UInt16,
                StorageType.UInt32 => TypeCode.UInt32,
                StorageType.UInt64 => TypeCode.UInt64,
                StorageType.Single => TypeCode.Single,
                StorageType.Double => TypeCode.Double,
                StorageType.Decimal => TypeCode.Decimal,
                StorageType.String => TypeCode.String,
                StorageType.DateTime => TypeCode.DateTime,
                _ => TypeCode.Object,
            };
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return (bool)this;
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return (byte)this;
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return (char)this;
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return (DateTime)this;
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return (decimal)this;
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return (double)this;
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return (short)this;
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return (int)this;
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return (long)this;
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return (sbyte)this;
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return (float)this;
        }

        string? IConvertible.ToString(IFormatProvider provider)
        {
            return (string?)this;
        }

        object? IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == typeof(byte))
            {
                return (byte)this;
            }
            if (conversionType == typeof(sbyte))
            {
                return (sbyte)this;
            }
            if (conversionType == typeof(char))
            {
                return (char)this;
            }
            if (conversionType == typeof(short))
            {
                return (short)this;
            }
            if (conversionType == typeof(ushort))
            {
                return (ushort)this;
            }
            if (conversionType == typeof(int))
            {
                return (int)this;
            }
            if (conversionType == typeof(uint))
            {
                return (uint)this;
            }
            if (conversionType == typeof(long))
            {
                return (long)this;
            }
            if (conversionType == typeof(ulong))
            {
                return (ulong)this;
            }
            if (conversionType == typeof(float))
            {
                return (float)this;
            }
            if (conversionType == typeof(decimal))
            {
                return (decimal)this;
            }
            if (conversionType == typeof(double))
            {
                return (double)this;
            }
            if (conversionType == typeof(string))
            {
                return (string?)this;
            }
            if (conversionType == typeof(DateTime))
            {
                return (DateTime)this;
            }
            if (conversionType == typeof(Guid))
            {
                return (Guid)this;
            }
            if (conversionType.IsEnum)
            {
                return (Enum?)this;
            }

            return _object;
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return (ushort)this;
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return (uint)this;
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return (ulong)this;
        }
        #endregion

        /// <summary>
        /// 存储数据的类别。
        /// </summary>
        private enum StorageType
        {
            /// <summary>
            /// 空的，没有存储数据。
            /// </summary>
            Empty,
            /// <summary>
            /// System.Char 类型的数据。
            /// </summary>
            Char,
            /// <summary>
            /// System.Enum 类型的数据。
            /// </summary>
            Enum,
            /// <summary>
            /// System.Boolean 类型的数据。
            /// </summary>
            Boolean,
            /// <summary>
            /// System.Byte 类型的数据。
            /// </summary>
            Byte,
            /// <summary>
            /// System.SByte 类型的数据。
            /// </summary>
            SByte,
            /// <summary>
            /// System.Byte[] 类型的数据。
            /// </summary>
            ByteArray,
            /// <summary>
            /// System.DateTime 类型的数据。
            /// </summary>
            DateTime,
            /// <summary>
            /// System.Decimal 类型的数据。
            /// </summary>
            Decimal,
            /// <summary>
            /// System.Double 类型的数据。
            /// </summary>
            Double,
            /// <summary>
            /// System.Guid 类型的数据。
            /// </summary>
            Guid,
            /// <summary>
            /// System.Int16 类型的数据。
            /// </summary>
            Int16,
            /// <summary>
            /// System.Int32 类型的数据。
            /// </summary>
            Int32,
            /// <summary>
            /// System.Int64 类型的数据。
            /// </summary>
            Int64,
            /// <summary>
            /// System.UInt16 类型的数据。
            /// </summary>
            UInt16,
            /// <summary>
            /// System.UInt32 类型的数据。
            /// </summary>
            UInt32,
            /// <summary>
            /// System.UInt64 类型的数据。
            /// </summary>
            UInt64,
            /// <summary>
            /// System.Single 类型的数据。
            /// </summary>
            Single,
            /// <summary>
            /// System.String 类型的数据。
            /// </summary>
            String,
            /// <summary>
            /// System.Object 类型的数据。
            /// </summary>
            Object
        }
    }
}
