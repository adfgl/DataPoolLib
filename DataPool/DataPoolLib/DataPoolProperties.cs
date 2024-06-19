namespace DataPoolLib
{
    public class DataPoolProperties
    {
        public DataPoolProperties(byte majorVersion, byte minorVersion)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
        }

        public byte MajorVersion { get; }
        public byte MinorVersion { get; }

        public DataPoolProperty[] Properties { get; set; }
    }
}
