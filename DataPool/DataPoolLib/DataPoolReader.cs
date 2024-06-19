namespace DataPoolLib
{
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using static DataPoolConstants;

    public static class DataPoolReader
    {
        static void ReadAndValidateVersion(BinaryReader reader, Type type, byte majorExpected, byte minorExpected)
        {
            byte majorActual = reader.ReadByte();
            byte minorActual = reader.ReadByte();
            if (majorExpected != majorActual || minorExpected < minorActual)
            {
                throw new InvalidOperationException($"Type {type.FullName} version mismatch. Expected {majorExpected}.{minorExpected}, but got {majorActual}.{minorActual}");
            }
        }

        public static object? Read(BinaryReader reader, Type type, bool allowDowngrade, bool skipVersion = false)
        {
            // value types
            if (type.IsValueType)
            {
                return ReadValue(reader, type, allowDowngrade);
            }

            // reference types
            if (type == typeof(string))
            {
                return ReadString(reader);
            }

            if (type.IsArray)
            {
                return ReadArray(reader, type.GetElementType()!, allowDowngrade);
            }

            if (NULL == reader.ReadByte()) return null;
            DataPoolProperties properties = PropertyLoader.GetOrderedProperties(type);
            if (false == skipVersion)
            {
                ReadAndValidateVersion(reader, type, properties.MajorVersion, properties.MinorVersion);
            }

            object obj = Activator.CreateInstance(type)!;
            foreach (DataPoolProperty property in properties.Properties)
            {
                PropertyInfo info = property.Info;
                info.SetValue(obj, Read(reader, info.PropertyType, allowDowngrade && property.AllowDowngrade));
            }
            return obj;
        }

        public static Array? ReadArray(BinaryReader reader, Type elementType, bool allowDowngrade)
        {
            if (NULL == reader.ReadByte()) return null;

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
                DataPoolObjectAttribute? objAttribute = elementType.GetCustomAttribute<DataPoolObjectAttribute>();
                if (objAttribute is not null)
                {
                    ReadAndValidateVersion(reader, elementType, objAttribute.MajorVersion, objAttribute.MinorVersion);
                }

                for (int i = 0; i < length; i++)
                {
                    array.SetValue(Read(reader, elementType, allowDowngrade, true), i);
                }
            }
            return array;
        }

        public static string? ReadString(BinaryReader reader)
        {
            if (NULL == reader.ReadByte()) return null;

            int length = reader.ReadUInt16();
            if (length == 0) return string.Empty;

            byte[] bytes = reader.ReadBytes(sizeof(byte) * length);
            string str = Encoding.UTF8.GetString(bytes);
            return str;
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
                    if (allowDowngrade)
                    {
                        value = ReadDowngradedFloat(reader);
                    }
                    else
                    {
                        value = reader.ReadSingle();
                    }
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

        public static float ReadDowngradedFloat(BinaryReader reader)
        {
            ENumber flag = (ENumber)reader.ReadByte();
            switch (flag)
            {
                case ENumber.Zero:
                    return 0;
                case ENumber.Float:
                    return reader.ReadSingle();

                case ENumber.Byte:
                    return reader.ReadByte();
                case ENumber.SByte:
                    return reader.ReadSByte();
                case ENumber.Short:
                    return reader.ReadInt16();
                case ENumber.UShort:
                    return reader.ReadUInt16();
         
                default:
                    throw new NotImplementedException("unknown flag: " + flag);
            }
        }

        public static double ReadDowngradedDouble(BinaryReader reader)
        {
            ENumber flag = (ENumber)reader.ReadByte();
            switch (flag)
            {
                case ENumber.Zero:
                    return 0;
                case ENumber.Byte:
                    return reader.ReadByte();
                case ENumber.SByte:
                    return reader.ReadSByte();
                case ENumber.Short:
                    return reader.ReadInt16();
                case ENumber.UShort:
                    return reader.ReadUInt16();
                case ENumber.Float:
                    return reader.ReadSingle();
                case ENumber.Double:
                    return reader.ReadDouble();
                case ENumber.Int:
                    return reader.ReadInt32();
                default:
                    throw new NotImplementedException("unknown flag: " + flag);
            }
        }

        public static int ReadDowngradedInteger(BinaryReader reader)
        {
            ENumber flag = (ENumber)reader.ReadByte();
            switch (flag)
            {
                case ENumber.Zero:
                    return 0;
                case ENumber.Byte:
                    return reader.ReadByte();
                case ENumber.SByte:
                    return reader.ReadSByte();
                case ENumber.Short:
                    return reader.ReadInt16();
                case ENumber.UShort:
                    return reader.ReadUInt16();
                case ENumber.Int:
                    return reader.ReadInt32();
                default:
                    throw new NotImplementedException("unknown flag: " + flag);
            }
        }
    }
}
