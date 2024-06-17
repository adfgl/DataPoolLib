namespace DataPoolLib
{
    using static DataPoolConstants;

    public static class DataPoolReader
    {
        public static object? Read(BinaryReader reader, Type type)
        {
            // value types
            if (type.IsValueType || type == typeof(string))
            {
                return ReadValue(reader, type);
            }

            // reference types
            if (type.IsArray)
            {

            }

            if (type.IsGenericType)
            {

            }

            return null;
        }

        public static Array? ReadArray(BinaryReader reader, Type elementType)
        {
            if (reader.ReadByte() == NULL) return null;

            int length = reader.ReadUInt16();
            Array array = Array.CreateInstance(elementType, length);
            for (int i = 0; i < length; i++)
            {
                array.SetValue(Read(reader, elementType), i);
            }
            return array;
        }

        public static object ReadValue(BinaryReader reader, Type type)
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
                    value = reader.ReadInt32();
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
                    value = reader.ReadDouble();
                    break;
                case TypeCode.Decimal:
                    value = reader.ReadDecimal();
                    break;
                case TypeCode.DateTime:
                    value = new DateTime(reader.ReadInt64());
                    break;
                case TypeCode.String:
                    value = reader.ReadString();
                    break;
                default:
                    throw new NotSupportedException($"Type {type.FullName} is not supported.");
            }
            return value;
        }
    }
}
