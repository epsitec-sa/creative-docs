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


namespace Epsitec.Common.Text
{
    /// <summary>
    /// La classe StyleVersion abrite un compteur qui permet d'associer des
    /// numéros de version aux styles, tabulateurs, etc. afin de détecter les
    /// modifications.
    /// </summary>
    public sealed class StyleVersion
    {
        public StyleVersion() { }

        public long Current
        {
            get { return this.current; }
        }

        public System.DateTime LastModificationTime
        {
            get { return this.time; }
        }

        public long ChangeVersion()
        {
            //	Il faut appeler cette méthode chaque fois qu'un style est
            //	modifié.

            long value = this.current++;
            this.time = System.DateTime.Now;
            return value;
        }

        private long current = 1;
        private System.DateTime time = System.DateTime.Now;
    }
}
