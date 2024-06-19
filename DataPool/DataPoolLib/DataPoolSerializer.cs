using System.Reflection;
using System.Text;

namespace DataPoolLib
{
    public static class DataPoolSerializer
    {
        public static byte[] Serialize<T>(T obj, bool compress = false, EEncoding encoding = EEncoding.UTF8)
        {
            Type type = typeof(T);
            DataPoolObjectAttribute? attr = type.GetCustomAttribute<DataPoolObjectAttribute>();
            if (attr is null)
            {
                throw new InvalidOperationException($"Serializable object must have {nameof(DataPoolObjectAttribute)}");
            }

            DataPoolMetaData metaData = new DataPoolMetaData()
            {
                Name = type.Name,
                Version = attr.Version,
                Compressed = compress,
                Encoding = encoding,
            };

            using (MemoryStream stream = new MemoryStream())
            {
                StreamHelper.WriteMetaData(stream, metaData);

                using (BinaryWriter writer = new BinaryWriter(stream, GetEncoding(encoding), true))
                {
                    DataPoolWriter.Write(writer, obj, type, compress);
                }

                return stream.ToArray();
            }
        }

        public static T? Deserialize<T>(byte[] data) where T : new()
        {
            Type type = typeof(T);
            DataPoolObjectAttribute? attr = type.GetCustomAttribute<DataPoolObjectAttribute>();
            if (attr is null)
            {
                throw new InvalidOperationException($"Serializable object must have {nameof(DataPoolObjectAttribute)}");
            }

            using (MemoryStream stream = new MemoryStream(data))
            {
                DataPoolMetaData metaData = StreamHelper.ReadMetaData(stream);

                if (type.Name != metaData.Name)
                {
                    throw new InvalidOperationException($"Type {type.Name} does not match serialized type {metaData.Name}");
                }

                if (attr.Version != metaData.Version)
                {
                    throw new InvalidOperationException($"Version {attr.Version} does not match serialized version {metaData.Version}");
                }

                object? obj = new T();
                using (BinaryReader reader = new BinaryReader(stream, GetEncoding(metaData.Encoding), true))
                {
                    obj = DataPoolReader.Read(reader, type, metaData.Compressed);
                }
                return (T)obj;
            }
        }

        static Encoding GetEncoding(EEncoding encoding)
        {
            switch (encoding)
            {
                case EEncoding.Default:
                    return Encoding.Default;
                case EEncoding.UTF8:
                    return Encoding.UTF8;
                case EEncoding.ASCII:
                    return Encoding.ASCII;
                case EEncoding.UTF32:
                    return Encoding.UTF32;
                default:
                    throw new NotSupportedException($"Encoding {encoding} is not supported");
            }
        }
    }
}
