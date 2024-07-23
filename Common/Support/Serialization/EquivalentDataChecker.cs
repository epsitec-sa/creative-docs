/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

﻿using System.Collections.Generic;
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

        public static bool HasEquivalentDataOrNull<T>(ICollection<T> first, ICollection<T> other)
            where T : IXMLWritable
        {
            if (first == null || other == null)
            {
                return first == null && other == null;
            }
            return first.HasEquivalentData(other);
        }
    }
}
