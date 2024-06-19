namespace DataPoolLib
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DataPoolPropertyAttribute : Attribute
    {
        public DataPoolPropertyAttribute(int key, bool allowDowngrade = true)
        {
            Key = key;
            AllowDowngrade = allowDowngrade;
        }

        /// <summary>
        /// Property key (index).
        /// </summary>
        public int Key { get; }

        public bool AllowDowngrade { get; }
    }
}
