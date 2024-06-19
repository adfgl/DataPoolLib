namespace DataPoolLib
{
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using static DataPoolConstants;

    public static class DataPoolWriter
    {
        static void WriteObjectVersion(BinaryWriter writer, byte minorVersion, byte majorVersion)
        {
            writer.Write(minorVersion);
            writer.Write(majorVersion);
        }

        public static void Write(BinaryWriter writer, object? obj, Type type, bool allowDowngrade, bool skipVersion = false)
        {
            if (type.IsValueType)
            {
                WriteValue(writer, obj!, type, allowDowngrade);
                return;
            }

            if (type == typeof(string))
            {
                WriteString(writer, obj);
                return;
            }

            if (type.IsArray)
            {
                WriteArray(writer, obj, allowDowngrade);
                return;
            }

            if (obj is null)
            {
                writer.Write(NULL);
                return;
            }
            writer.Write(NOT_NULL);

            DataPoolProperties properties = PropertyLoader.GetOrderedProperties(type);
            if (false == skipVersion)
            {
                WriteObjectVersion(writer, properties.MajorVersion, properties.MajorVersion);
            }
            foreach (DataPoolProperty property in properties.Properties)
            {
                PropertyInfo info = property.Info;
                Write(writer, info.GetValue(obj), info.PropertyType, allowDowngrade && property.AllowDowngrade);
            }
        }

        public static void WriteArray(BinaryWriter writer, object? array, bool allowDowngrade)
        {
            if (array is null)
            {
                writer.Write(NULL);
                return;
            }

            writer.Write(NOT_NULL);
            Array arr = (Array)array;
            writer.Write((UInt16)arr.Length);

            Type elementType = arr.GetType().GetElementType()!;

            if (elementType.IsValueType && false == allowDowngrade)
            {
                int byteLength = arr.Length * Marshal.SizeOf(elementType);
                byte[] buffer = new byte[byteLength];
                Buffer.BlockCopy(arr, 0, buffer, 0, byteLength);
                writer.Write(buffer);
            }
            else
            {
                DataPoolObjectAttribute? objAttribute = elementType.GetCustomAttribute<DataPoolObjectAttribute>();
                if (objAttribute is not null)
                {
                    WriteObjectVersion(writer, objAttribute.MajorVersion, objAttribute.MajorVersion);
                }
                foreach (object element in arr)
                {
                    Write(writer, element, elementType, allowDowngrade, true);
                }
            }
        }

        public static void WriteString(BinaryWriter writer, object? value)
        {
            if (value is null)
            {
                writer.Write(NULL);
                return;
            }

            writer.Write(NOT_NULL);
            byte[] bytes = Encoding.UTF8.GetBytes((string)value);
            int len = bytes.Length;
            writer.Write((UInt16)len);  
            if (len > 0)
            {
                byte[] buffer = new byte[len];
                Buffer.BlockCopy(bytes, 0, buffer, 0, len);
                writer.Write(buffer);
            }
        }

        public static void WriteDowngraded(BinaryWriter writer, int value)
        {
            if (value == 0) // byte
            {
                writer.Write((byte)ENumber.Zero);
                return;
            }

            if (value > 0)
            {
                if (value <= byte.MaxValue) // byte
                {
                    writer.Write((byte)ENumber.Byte);
                    writer.Write((byte)value);
                }
                else if (value <= ushort.MaxValue) // short
                {
                    writer.Write((byte)ENumber.Short);
                    writer.Write((ushort)value);
                }
                else // int
                {
                    writer.Write((byte)ENumber.Int);
                    writer.Write(value);
                }
            }
            else
            {
                if (value >= sbyte.MinValue) // sbyte
                {
                    writer.Write((byte)ENumber.SByte);
                    writer.Write((sbyte)value);
                }
                else if (value >= short.MinValue) // short
                {
                    writer.Write((byte)ENumber.Short);
                    writer.Write((short)value);
                }
                else // int
                {
                    writer.Write((byte)ENumber.Int);
                    writer.Write(value);
                }
            }
        }

        public static void WriteDowngraded(BinaryWriter writer, float value)
        {
            // 1 byte
            if (value == 0) 
            {
                writer.Write((byte)ENumber.Zero);
                return;
            }

            // 4 bytes
            if (value != MathF.Floor(value) 
                || 
                value > ushort.MaxValue 
                ||
                value < short.MinValue)
            {
                writer.Write((byte)ENumber.Float);
                writer.Write((float)value);
            }
            else
            {
                if (value > 0)
                {
                    // 1 byte
                    if (value <= byte.MaxValue) 
                    {
                        writer.Write((byte)ENumber.Byte);
                        writer.Write((byte)value);
                    }

                    // 2 bytes
                    else if (value <= ushort.MaxValue)
                    {
                        writer.Write((byte)ENumber.UShort);
                        writer.Write((ushort)value);
                    }
                }
                else
                {
                    // 1 byte
                    if (value >= sbyte.MinValue)
                    {
                        writer.Write((byte)ENumber.SByte);
                        writer.Write((sbyte)value);
                    }

                    // 2 bytes
                    else if (value >= short.MinValue)
                    {
                        writer.Write((byte)ENumber.Short);
                        writer.Write((short)value);
                    }
                }
            }
        }

        public static void WriteDowngraded(BinaryWriter writer, double value)
        {
            // 1 byte
            if (value == 0)
            {
                writer.Write((byte)ENumber.Zero);
                return;
            }

            if (value != Math.Floor(value)) // has fraction
            {
                // 4 bytes
                if (value <= float.MaxValue || value >= float.MinValue)
                {
                    writer.Write((byte)ENumber.Float);
                    writer.Write((float)value);
                    return;
                }

                // 8 bytes
                else
                {
                    writer.Write((byte)ENumber.Double);
                    writer.Write(value);
                }
            }
            else
            {
                if (value > 0)
                {
                    // 1 byte
                    if (value <= byte.MaxValue)
                    {
                        writer.Write((byte)ENumber.Byte);
                        writer.Write((byte)value);
                    }

                    // 2 bytes
                    else if (value <= ushort.MaxValue) 
                    {
                        writer.Write((byte)ENumber.UShort);
                        writer.Write((ushort)value);
                    }

                    // 4 bytes
                    else if (value <= int.MaxValue)
                    {
                        writer.Write((byte)ENumber.Int);
                        writer.Write((int)value);
                    }

                    // 8 bytes
                    else
                    {
                        writer.Write((byte)ENumber.Double);
                        writer.Write(value);
                    }
                }
                else
                {
                    // 1 byte
                    if (value >= sbyte.MinValue)
                    {
                        writer.Write((byte)ENumber.SByte);
                        writer.Write((sbyte)value);
                    }

                    // 2 bytes
                    else if (value >= short.MinValue)
                    {
                        writer.Write((byte)ENumber.Short);
                        writer.Write((short)value);
                    }

                    // 4 bytes
                    else if (value >= int.MaxValue)
                    {
                        writer.Write((byte)ENumber.Int);
                        writer.Write((int)value);
                    }

                    // 8 bytes
                    else
                    {
                        writer.Write((byte)ENumber.Double);
                        writer.Write(value);
                    }
                }
            }
        }

        public static void WriteValue(BinaryWriter writer, object value, Type type, bool allowDowngrade)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    writer.Write((bool)value);
                    break;
                case TypeCode.Char:
                    writer.Write((char)value);
                    break;
                case TypeCode.SByte:
                    writer.Write((sbyte)value);
                    break;
                case TypeCode.Byte:
                    writer.Write((byte)value);
                    break;
                case TypeCode.Int16:
                    writer.Write((short)value);
                    break;
                case TypeCode.UInt16:
                    writer.Write((ushort)value);
                    break;

                case TypeCode.Int32:
                    if (allowDowngrade)
                    {
                        WriteDowngraded(writer, (int)value);
                    }
                    else
                    {
                        writer.Write((int)value);
                    }
                    break;

                case TypeCode.UInt32:
                    writer.Write((uint)value);
                    break;

                case TypeCode.Int64:
                    writer.Write((long)value);
                    break;

                case TypeCode.UInt64:
                    writer.Write((ulong)value);
                    break;

                case TypeCode.Single:
                    if (allowDowngrade)
                    {
                        WriteDowngraded(writer, (float)value);
                    }
                    else
                    {
                        writer.Write((float)value);
                    }
                    break;

                case TypeCode.Double:
                    if (allowDowngrade)
                    {
                        WriteDowngraded(writer, (double)value);
                    }
                    else
                    {
                        writer.Write((double)value);
                    }
                    break;

                case TypeCode.Decimal:
                    writer.Write((decimal)value);
                    break;
                case TypeCode.DateTime:
                    writer.Write(((DateTime)value).Ticks);
                    break;
                default:
                    throw new NotSupportedException($"Type {type.FullName} is not supported.");
            }
        }
    }
}
