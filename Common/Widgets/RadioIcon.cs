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

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe RadioIcon est un IconButton pour une collection dont un seul
    /// bouton sera sélectionné.
    /// </summary>
    public class RadioIcon : IconButton
    {
        public RadioIcon()
        {
            this.AutoToggle = true;
            this.AutoRadio = true;
            this.ButtonStyle = ButtonStyle.ActivableIcon;
        }

        public RadioIcon(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public int EnumValue
        {
            get { return this.enumValue; }
            set { this.enumValue = value; }
        }

        public bool EndOfLine
        {
            get { return this.endOfLine; }
            set { this.endOfLine = value; }
        }

        public int Column
        {
            get { return this.column; }
            set { this.column = value; }
        }

        public int Row
        {
            get { return this.row; }
            set { this.row = value; }
        }

        protected int enumValue;
        protected bool endOfLine;
        protected int rank = -1;
        protected int column = -1;
        protected int row = -1;
    }
}
