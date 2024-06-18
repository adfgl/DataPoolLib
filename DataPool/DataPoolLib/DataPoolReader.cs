namespace DataPoolLib
{
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using static DataPoolConstants;

    public static class DataPoolReader
    {
        public static object? Read(BinaryReader reader, Type type, bool allowDowngrade)
        {
            // value types
            if (type.IsValueType)
            {
                return ReadValue(reader, type, allowDowngrade);
            }

            // reference types
            if (NULL == reader.ReadByte())
            {
                return null;
            }

            if (type == typeof(string))
            {
                return ReadString(reader);
            }

            if (type.IsArray)
            {
                return ReadArray(reader, type.GetElementType()!, allowDowngrade);
            }

            DataPoolProperty[] properties = PropertyLoader.GetOrderedProperties(type);
            object obj = Activator.CreateInstance(type)!;
            foreach (DataPoolProperty property in properties)
            {
                PropertyInfo info = property.Info;
                info.SetValue(obj, Read(reader, info.PropertyType, allowDowngrade));
            }
            return obj;
        }

        public static Array ReadArray(BinaryReader reader, Type elementType, bool allowDowngrade)
        {
            int length = reader.ReadUInt16();
            Array array = Array.CreateInstance(elementType, length);

            if (elementType.IsValueType && false == allowDowngrade)
            {
                int elementSize = Marshal.SizeOf(elementType);
                int byteLength = length * elementSize;

                byte[] buffer = reader.ReadBytes(byteLength);
                Buffer.BlockCopy(buffer, 0, array, 0, byteLength);
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    array.SetValue(Read(reader, elementType, allowDowngrade), i);
                }
            }
            return array;
        }

        public static string ReadString(BinaryReader reader)
        {
            int length = reader.ReadUInt16();
            if (length == 0) return string.Empty;

            byte[] bytes = reader.ReadBytes(sizeof(byte) * length);
            return Encoding.UTF8.GetString(bytes);
        }

        public static object ReadValue(BinaryReader reader, Type type, bool allowDowngrade)
        {
            object value;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    value = reader.ReadBoolean();
                    break;
                case TypeCode.Char:
                    value = reader.ReadChar();
                    break;
                case TypeCode.SByte:
                    value = reader.ReadSByte();
                    break;
                case TypeCode.Byte:
                    value = reader.ReadByte();
                    break;
                case TypeCode.Int16:
                    value = reader.ReadInt16();
                    break;
                case TypeCode.UInt16:
                    value = reader.ReadUInt16();
                    break;
                case TypeCode.Int32:
                    if (allowDowngrade)
                    {
                        value = ReadDowngradedInteger(reader);
                    }
                    else
                    {
                        value = reader.ReadInt32();
                    }
                    break;
                case TypeCode.UInt32:
                    value = reader.ReadUInt32();
                    break;
                case TypeCode.Int64:
                    value = reader.ReadInt64();
                    break;
                case TypeCode.UInt64:
                    value = reader.ReadUInt64();
                    break;
                case TypeCode.Single:
                    value = reader.ReadSingle();
                    break;
                case TypeCode.Double:
                    if (allowDowngrade)
                    {
                        value = ReadDowngradedDouble(reader);
                    }
                    else
                    {
                        value = reader.ReadDouble();
                    }
                    break;
                case TypeCode.Decimal:
                    value = reader.ReadDecimal();
                    break;
                case TypeCode.DateTime:
                    value = new DateTime(reader.ReadInt64());
                    break;
                default:
                    throw new NotSupportedException($"Type {type.FullName} is not supported.");
            }
            return value;
        }

        public static double ReadDowngradedDouble(BinaryReader reader)
        {
            byte flag = reader.ReadByte();
            switch (flag)
            {
                case 0:
                    return 0;

                case 1:
                    return reader.ReadSingle();

                case 2:
                    return reader.ReadDouble();

                case 3:
                    return reader.ReadInt32();

                case 4:
                    return reader.ReadSByte();

                case 5:
                    return reader.ReadInt16();

                case 6:
                    return reader.ReadByte();

                case 7:
                    return reader.ReadUInt16();

                default:
                    throw new NotImplementedException("unknown flag: " + flag);
            }
        }

        public static int ReadDowngradedInteger(BinaryReader reader)
        {
            byte flag = reader.ReadByte();
            switch (flag)
            {
                case 0:
                    return 0;

                case 1:
                    return reader.ReadInt32();

                case 2:
                    return reader.ReadByte();

                case 3:
                    return reader.ReadUInt16();

                case 4:
                    return reader.ReadSByte();

                case 5:
                    return reader.ReadInt16();

                default:
                    throw new NotImplementedException("unknown flag: " + flag);
            }
        }
    }
}
