using System.Reflection;

namespace DataPoolLib
{
    public static class PropertyLoader
    {
        readonly static Dictionary<string, PropertyInfo[]> PROPERTY_CACHE = new Dictionary<string, PropertyInfo[]>();

        public static int CacheCount => PROPERTY_CACHE.Count;

        public static void ClearCache()
        {
            PROPERTY_CACHE.Clear();
        }

        public static PropertyInfo[] GetOrderedProperties(Type type)
        {
            if (type.GetCustomAttribute<DataPoolObjectAttribute>() == null && type.GetCustomAttribute<DataPoolPropertyAttribute>() == null)
            {
                throw new InvalidOperationException($"{type.FullName}: must have {nameof(DataPoolObjectAttribute)} or {nameof(DataPoolPropertyAttribute)}");
            }

            string key = type.Name;

            PropertyInfo[]? props;
            if (false == PROPERTY_CACHE.TryGetValue(key, out props))
            {
                props = LoadProperties(type);
                PROPERTY_CACHE[key] = props;
            }
            return props;
        }

        public static PropertyInfo[] LoadProperties(Type type)
        {
            PropertyInfo[] props = type.GetProperties();
            int numProps = props.Length;

            PropertyInfo[] attrProps = new PropertyInfo[numProps];
            int[] keys = new int[numProps];
            int count = 0;

            foreach (PropertyInfo p in props)
            {
                DataPoolPropertyAttribute? attr = p.GetCustomAttribute<DataPoolPropertyAttribute>();
                if (attr is null) continue;

                int key = attr.Key;

                int insertIndex = count;
                for (int i = 0; i < count; i++)
                {
                    if (keys[i] == key)
                    {
                        throw new InvalidOperationException($"{type.FullName}: Duplicate order value {key} found in property '{p.Name}'.");
                    }

                    if (keys[i] > key)
                    {
                        insertIndex = i;
                        break;
                    }
                }

                if (insertIndex < count)
                {
                    // shift values
                    for (int i = count - 1; i >= insertIndex; i--)
                    {
                        keys[i + 1] = keys[i];
                        attrProps[i + 1] = attrProps[i];
                    }
                }

                // add new values
                keys[insertIndex] = key;
                attrProps[insertIndex] = p;
                count++;
            }

            if (count == 0)
            {
                return Array.Empty<PropertyInfo>();
            }

            if (count == numProps) return attrProps;

            PropertyInfo[] sorted = new PropertyInfo[count];
            Array.Copy(attrProps, sorted, count);
            return sorted;
        }
    }
}
