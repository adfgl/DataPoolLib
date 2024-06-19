namespace DataPoolLib
{
    public class DataPoolMetaData
    {
        public string Name { get; set; } = string.Empty;
        public byte MajorVersion { get; set; } = 0;
        public byte MinorVersion { get; set; } = 0;
        public bool Compressed { get; set; } = false;
        public EEncoding Encoding { get; set; } = EEncoding.Default;
    }
}
