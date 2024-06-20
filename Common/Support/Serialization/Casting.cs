using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Serialization
{
    public class Casting
    {
        public static Dictionary<K, V> HashtableToDictionary<K, V>(Hashtable table)
        {
            // idea taken from https://stackoverflow.com/questions/6455822/convert-hashtable-to-dictionary-in-c-sharp
            if (table.Count == 0)
            {
                return new Dictionary<K, V>();
            }
            return table
                .Cast<DictionaryEntry>()
                .ToDictionary(kvp => (K)kvp.Key, kvp => (V)kvp.Value);
        }
    }
}
