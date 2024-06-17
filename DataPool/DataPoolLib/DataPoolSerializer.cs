using System.Reflection;

namespace DataPoolLib
{
    public static class DataPoolSerializer
    {
        const byte NULL = 0;
        const byte NOT_NULL = 1;

        readonly static Dictionary<string, PropertyInfo[]> PROPERTY_CACHE = new Dictionary<string, PropertyInfo[]>();

      

    
    }
}
