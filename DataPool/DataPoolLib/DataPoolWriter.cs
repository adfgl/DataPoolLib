namespace DataPoolLib
{
    using System.Reflection;
    using static DataPoolConstants;

    public static class DataPoolWriter
    {
        public static void Write(BinaryWriter writer, object? obj, Type type)
        {
            if (type.IsValueType || type == typeof(string))
            {
                WriteValue(writer, obj, type);
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

            if (type.IsArray)
            {
                WriteArray(writer, (Array)obj);
                return;
            }

            PropertyInfo[] properties = PropertyLoader.GetOrderedProperties(type);
            foreach (PropertyInfo property in properties)
            {
                Write(writer, property.GetValue(obj), property.PropertyType);
            }
        }

        public static void WriteArray(BinaryWriter writer, Array arr)
        {
            Type elementType = arr.GetType().GetElementType()!;
            writer.Write((UInt16)arr.Length);
            foreach (object element in arr)
            {
                Write(writer, element, elementType);
            }
        }

        public static void WriteValue(BinaryWriter writer, object value, Type type)
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
                    writer.Write((int)value);
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
                    writer.Write((double)value);
                    break;
                case TypeCode.Decimal:
                    writer.Write((decimal)value);
                    break;
                case TypeCode.DateTime:
                    writer.Write(((DateTime)value).Ticks);
                    break;
                case TypeCode.String:
                    writer.Write((string)value);
                    break;
                default:
                    throw new NotSupportedException($"Type {type.FullName} is not supported.");
            }

        }
    }
}
