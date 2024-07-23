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

namespace Epsitec.Common.Types
{
    /// <summary>
    /// La class StringDict implémente un dictionnaire de clefs/valeurs de
    /// type string.
    /// </summary>
    public class StringDict : Dictionary<string, string>, IStringDict
    {
        public StringDict() { }

        public StringDict(Types.IStringDict model)
            : this()
        {
            StringDict.Copy(model, this);
        }

        #region IStringDict Members

        public new string[] Keys
        {
            get
            {
                List<string> keys = new List<string>();
                keys.AddRange(base.Keys);
                return keys.ToArray();
            }
        }

        #endregion

        public static void Copy(Types.IStringDict model, Types.IStringDict target)
        {
            string[] keys = model.Keys;

            for (int i = 0; i < keys.Length; i++)
            {
                target.Add(keys[i], model[keys[i]]);
            }
        }
    }
}
