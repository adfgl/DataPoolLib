using System.Reflection;

namespace DataPoolLib
{
    public static class PropertyLoader
    {
        readonly static Dictionary<string, DataPoolProperty[]> PROPERTY_CACHE = new Dictionary<string, DataPoolProperty[]>();

        public static int CacheCount => PROPERTY_CACHE.Count;

        public static void ClearCache()
        {
            PROPERTY_CACHE.Clear();
        }

        public static void PreloadProperties(params Type[] types)
        {
            foreach (Type type in types)
            {
                if (PROPERTY_CACHE.ContainsKey(type.Name))
                {
                    throw new InvalidOperationException($"{type.FullName}: already loaded. Make sure you do not have duplicate names in your classes.");
                }
                GetOrderedProperties(type);
            }
        }

        public static DataPoolProperty[] GetOrderedProperties(Type type)
        {
            if (type.GetCustomAttribute<DataPoolObjectAttribute>() == null && type.GetCustomAttribute<DataPoolPropertyAttribute>() == null)
            {
                throw new InvalidOperationException($"{type.FullName}: must have {nameof(DataPoolObjectAttribute)} or {nameof(DataPoolPropertyAttribute)}");
            }

            string key = type.Name;

            DataPoolProperty[]? props;
            if (false == PROPERTY_CACHE.TryGetValue(key, out props))
            {
                props = LoadProperties(type);
                PROPERTY_CACHE[key] = props;
            }
            return props;
        }

        public static DataPoolProperty[] LoadProperties(Type type)
        {
            PropertyInfo[] props = type.GetProperties();
            int numProps = props.Length;

            DataPoolProperty[] attrProps = new DataPoolProperty[numProps];
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
                attrProps[insertIndex] = new DataPoolProperty(p)
                {
                    AllowDowngrade = attr.AllowDowngrade,
                };
                count++;
            }

            if (count == 0)
            {
                return Array.Empty<DataPoolProperty>();
            }

            if (count == numProps) return attrProps;

            DataPoolProperty[] sorted = new DataPoolProperty[count];
            Array.Copy(attrProps, sorted, count);
            return sorted;
        }
    }
}
