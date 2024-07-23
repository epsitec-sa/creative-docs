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

﻿using System.Collections;
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
