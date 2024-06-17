namespace DataPoolLib
{
    public class DataPoolMetaData
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool Compressed { get; set; } = false;
        public EEncoding Encoding { get; set; } = EEncoding.Default;
    }
}
