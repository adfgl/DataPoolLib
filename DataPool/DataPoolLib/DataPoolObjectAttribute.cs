namespace DataPoolLib
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DataPoolObjectAttribute : Attribute
    {
        public DataPoolObjectAttribute(string version, bool allowDowngrade = false)
        {
            Version = version;
            AllowDowngrade = allowDowngrade;
        }

        public string Version { get; }
        public bool AllowDowngrade { get; }
    }
}
