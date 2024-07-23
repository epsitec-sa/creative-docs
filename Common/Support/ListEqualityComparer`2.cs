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


using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
    public class ListEqualityComparer<T1, T2> : IEqualityComparer<T1>
        where T1 : IList<T2>
        where T2 : class
    {
        #region IEqualityComparer<IList<string>> Members

        public bool Equals(T1 x, T1 y)
        {
            if (x.Count != y.Count)
            {
                return false;
            }

            int n = x.Count;

            for (int i = 0; i < n; i++)
            {
                if (x[i] == y[i])
                {
                    continue;
                }

                if (x[i] == null)
                {
                    return false;
                }

                if (x[i].Equals(y[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(T1 obj)
        {
            return obj.Aggregate(0, (n, x) => (x == null ? 0 : x.GetHashCode() * 37) ^ n);
        }

        #endregion
    }
}
