using System.Reflection;

namespace DataPoolLib
{
    public static class PropertyLoader
    {
        readonly static Dictionary<Type, DataPoolProperties> PROPERTY_CACHE = new Dictionary<Type, DataPoolProperties>();

        public static int CacheCount => PROPERTY_CACHE.Count;

        public static void ClearCache()
        {
            PROPERTY_CACHE.Clear();
        }

        public static void PreloadProperties(params Type[] types)
        {
            foreach (Type type in types)
            {
                if (PROPERTY_CACHE.ContainsKey(type))
                {
                    throw new InvalidOperationException($"{type.FullName}: already loaded. Make sure you do not have duplicate names in your classes.");
                }
                GetOrderedProperties(type);
            }
        }

        public static DataPoolProperties GetOrderedProperties(Type type)
        {
            DataPoolProperties? props;
            if (false == PROPERTY_CACHE.TryGetValue(type, out props))
            {
                props = LoadProperties(type);
                PROPERTY_CACHE[type] = props;
            }
            return props;
        }

        public static DataPoolProperties LoadProperties(Type type)
        {
            DataPoolObjectAttribute? obj = type.GetCustomAttribute<DataPoolObjectAttribute>();
            if (obj == null)
            {
                throw new InvalidOperationException($"{type.FullName}: must have {nameof(DataPoolObjectAttribute)}.");
            }

            DataPoolProperties properties = new DataPoolProperties(obj.MajorVersion, obj.MinorVersion);

            PropertyInfo[] props = type.GetProperties();
            int numProps = props.Length;

            DataPoolProperty[] attrProps = new DataPoolProperty[numProps];
            int[] keys = new int[numProps];
            int count = 0;

            foreach (PropertyInfo p in props)
            {
                if (p.CanWrite == false) continue;

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
                properties.Properties = Array.Empty<DataPoolProperty>();
                return properties;
            }

            if (count == numProps)
            {
                properties.Properties = attrProps;
                return properties;
            }

            DataPoolProperty[] sorted = new DataPoolProperty[count];
            Array.Copy(attrProps, sorted, count);

            properties.Properties = sorted;
            return properties;
        }
    }
}
