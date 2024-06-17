namespace DataPoolLib
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DataPoolPropertyAttribute : Attribute
    {
        public DataPoolPropertyAttribute(int key)
        {
            Key = key;
        }

        /// <summary>
        /// Property key (index).
        /// </summary>
        public int Key { get; }
    }
}
