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


namespace Epsitec.Common.Types
{
    /// <summary>
    /// La classe Copier permet de réaliser des copies de données simples.
    /// </summary>
    public static class Copier
    {
        public static object Copy(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is string)
            {
                return obj;
            }
            if (obj is System.ValueType)
            {
                return obj;
            }

            //	TODO: compléter...

            throw new System.NotSupportedException(
                string.Format("Cannot copy type {0}, not supported", obj.GetType().Name)
            );
        }

        public static T[] CopyArray<T>(T[] array)
        {
            if (array == null)
            {
                return null;
            }
            else
            {
                T[] copy = new T[array.Length];
                array.CopyTo(copy, 0);
                return copy;
            }
        }

        public static T[] CopyArray<T>(System.Collections.Generic.ICollection<T> collection)
        {
            T[] copy = new T[collection.Count];
            collection.CopyTo(copy, 0);
            return copy;
        }
    }
}
