namespace DataPoolLib
{
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.Marshalling;
    using System.Text;
    using static DataPoolConstants;

    public static class DataPoolWriter
    {
        public static void Write(BinaryWriter writer, object? obj, Type type, bool allowDowngrade)
        {
            if (type.IsValueType)
            {
                WriteValue(writer, obj!, type, allowDowngrade);
                return;
            }

            if (obj is null)
            {
                writer.Write(NULL);
                return;
            }
            else
            {
                writer.Write(NOT_NULL);
            }

            if (type == typeof(string))
            {
                WriteString(writer, Encoding.UTF8.GetBytes((string)obj), 100);
                return;
            }

            if (type.IsArray)
            {
                WriteArray(writer, (Array)obj, allowDowngrade);
                return;
            }

            DataPoolProperty[] properties = PropertyLoader.GetOrderedProperties(type);
            foreach (DataPoolProperty property in properties)
            {
                PropertyInfo info = property.Info;
                Write(writer, info.GetValue(obj), info.PropertyType, allowDowngrade);
            }
        }

        public static void WriteArray(BinaryWriter writer, Array arr, bool allowDowngrade)
        {
            Type elementType = arr.GetType().GetElementType()!;
            writer.Write((UInt16)arr.Length);

            if (elementType.IsValueType && false == allowDowngrade)
            {
                int elementSize = Marshal.SizeOf(elementType);
                int byteLength = arr.Length * elementSize;
                byte[] buffer = new byte[byteLength];
                Buffer.BlockCopy(arr, 0, buffer, 0, byteLength);
                writer.Write(buffer);
            }
            else
            {
                foreach (object element in arr)
                {
                    Write(writer, element, elementType, allowDowngrade);
                }
            }
        }

        public static void WriteString(BinaryWriter writer, byte[] value, int maxLength)
        {
            int len = Math.Min(value.Length, maxLength);
            writer.Write((UInt16)len);

            if (len > 0)
            {
                byte[] buffer = new byte[len];
                Buffer.BlockCopy(value, 0, buffer, 0, len);
                writer.Write(buffer);
            }
        }

        public static void WriteDowngraded(BinaryWriter writer, int value)
        {
            if (value == 0) // byte
            {
                writer.Write((byte)0);
            }

            if (value > 0)
            {
                if (value <= byte.MaxValue) // byte
                {
                    writer.Write((byte)2);
                    writer.Write((byte)value);
                }
                else if (value <= ushort.MaxValue) // short
                {
                    writer.Write((byte)3);
                    writer.Write((ushort)value);
                }
                else // int
                {
                    writer.Write((byte)1);
                    writer.Write(value);
                }
            }
            else
            {
                if (value >= sbyte.MinValue) // sbyte
                {
                    writer.Write((byte)4);
                    writer.Write((sbyte)value);
                }
                else if (value >= short.MinValue) // short
                {
                    writer.Write((byte)5);
                    writer.Write((short)value);
                }
                else // int
                {
                    writer.Write((byte)1);
                    writer.Write(value);
                }
            }
        }

        public static void WriteDowngraded(BinaryWriter writer, double value)
        {
            if (value == 0) // byte
            {
                writer.Write((byte)0);
            }

            bool hasFraction = value != Math.Floor(value);
            if (hasFraction)
            {
                if (value <= float.MaxValue) // float
                {
                    writer.Write((byte)1);
                    writer.Write((float)value);
                }
                else // double
                {
                    writer.Write((byte)2);
                    writer.Write(value);
                }
            }
            else
            {
                if (value < 0)
                {
                    if (value <= sbyte.MinValue)
                    {
                        writer.Write((byte)4);
                        writer.Write((byte)value);
                    }
                    else if (value <= short.MinValue)
                    {
                        writer.Write((byte)5);
                        writer.Write((byte)value);
                    }
                    else // int
                    {
                        writer.Write((byte)3);
                        writer.Write((int)value);
                    }
                }
                else
                {
                    if (value <= byte.MaxValue) // byte
                    {
                        writer.Write((byte)6);
                        writer.Write((byte)value);
                    }
                    else if (value <= ushort.MaxValue) // short
                    {
                        writer.Write((byte)7);
                        writer.Write((ushort)value);
                    }
                    else // int
                    {
                        writer.Write((byte)3);
                        writer.Write((int)value);
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
                    writer.Write((float)value);
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
