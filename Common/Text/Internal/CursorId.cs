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


namespace Epsitec.Common.Text.Internal
{
    /// <summary>
    /// La structure CursorId encapsule un identificateur lié à une structure
    /// de type Internal.Cursor.
    /// </summary>
    internal struct CursorId
    {
        public CursorId(int id)
        {
            this.value = id;
        }

        public bool IsValid
        {
            //	Un curseur est déclaré comme étant valide s'il a un identificateur
            //	strictement positif.

            //	CursorId = 0 n'est donc pas valide.

            get { return this.value > 0; }
        }

        public static implicit operator int(CursorId id)
        {
            return id.value;
        }

        public static implicit operator CursorId(int id)
        {
            return new CursorId(id);
        }

        private int value;
    }
}
