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


namespace Epsitec.Common.Widgets.Helpers
{
    public sealed class HypertextInfo : System.ICloneable, System.IComparable
    {
        internal HypertextInfo(TextLayout layout, Drawing.Rectangle bounds, int index)
        {
            this.layout = layout;
            this.bounds = bounds;
            this.index = index;
        }

        #region ICloneable Members
        public object Clone()
        {
            return new HypertextInfo(this.layout, this.bounds, this.index);
        }
        #endregion

        #region IComparable Members
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            HypertextInfo that = obj as HypertextInfo;

            if ((that == null) || (that.layout != this.layout))
            {
                throw new System.ArgumentException("Invalid argument");
            }

            return this.index.CompareTo(that.index);
        }
        #endregion

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return this.index;
        }

        public Drawing.Rectangle Bounds
        {
            get { return this.bounds; }
        }

        public string Anchor
        {
            get { return this.layout.FindAnchor(this.index); }
        }

        private TextLayout layout;
        private Drawing.Rectangle bounds;
        private int index;
    }
}
