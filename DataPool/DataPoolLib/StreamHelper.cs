using System.Text;

namespace DataPoolLib
{
    internal static class StreamHelper
    {
        readonly static Encoding STRING_ENCODING = Encoding.UTF8;

        const byte TRUE = 1;
        const byte FALSE = 0;

        public static string ReadString(Stream stream)
        {
            int length = stream.ReadByte();
            if (length == 0) return String.Empty;

            byte[] buffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                int b = stream.ReadByte();
                if (b == -1) throw new EndOfStreamException();
                buffer[i] = (byte)b;
            }
            return STRING_ENCODING.GetString(buffer);
        }

        public static void WriteString(Stream stream, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                stream.WriteByte(0);
                return;
            }

            byte[] buffer = STRING_ENCODING.GetBytes(value);
            stream.WriteByte((byte)buffer.Length);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteMetaData(Stream stream, DataPoolMetaData metaData)
        {
            WriteString(stream, metaData.Name);
            stream.WriteByte(metaData.MajorVersion);
            stream.WriteByte(metaData.MinorVersion);
            stream.WriteByte(metaData.Compressed ? TRUE : FALSE);
            stream.WriteByte((byte)metaData.Encoding);
        }

        public static DataPoolMetaData ReadMetaData(Stream stream)
        {
            DataPoolMetaData metaData = new DataPoolMetaData();
            metaData.Name = ReadString(stream);
            metaData.MajorVersion = (byte)stream.ReadByte();
            metaData.MinorVersion = (byte)stream.ReadByte();
            metaData.Compressed = stream.ReadByte() == TRUE;
            metaData.Encoding = (EEncoding)stream.ReadByte();
            return metaData;
        }
    }
}
