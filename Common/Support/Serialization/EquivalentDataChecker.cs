using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Serialization
{
    public static class EquivalentDataChecker
    {
        public static bool HasEquivalentData<T>(this T[] first, T[] other)
            where T : IXMLWritable
        {
            if (first == null && other == null)
            {
                return true;
            }
            if (first.Count() != other.Count())
            {
                return false;
            }

            return first.Zip(other).All(pair => pair.First.HasEquivalentData(pair.Second));
        }

        public static bool HasEquivalentData<T>(this ICollection<T> first, ICollection<T> other)
            where T : IXMLWritable
        {
            if (first.Count != other.Count)
            {
                return false;
            }

            foreach (var (fst, snd) in first.Zip(other))
            {
                if (!fst.HasEquivalentData(snd))
                {
                    fst.HasEquivalentData(snd);
                    return false;
                }
            }
            return true;
        }
    }
}
