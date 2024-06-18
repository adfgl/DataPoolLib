using System.Reflection;

namespace DataPoolLib
{
    public class DataPoolProperty
    {
        public DataPoolProperty(PropertyInfo info)
        {
            Info = info;
        }

        public PropertyInfo Info { get; }
        public bool AllowDowngrade { get; set; }
    }
}
