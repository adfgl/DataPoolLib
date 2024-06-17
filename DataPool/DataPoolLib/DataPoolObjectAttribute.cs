namespace DataPoolLib
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DataPoolObjectAttribute : Attribute
    {
        public DataPoolObjectAttribute(string version)
        {
            Version = version;
        }

        public string Version { get; set; }
    }
}
