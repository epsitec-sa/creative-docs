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
    /// The <c>MultiSelectEventArgs</c> class represents a selection in the
    /// <see cref="ScrollListMultiSelect"/>, which is defined by two indexes.
    /// </summary>
    public class MultiSelectEventArgs : Support.EventArgs
    {
        public MultiSelectEventArgs()
        {
            this.BeginIndex = -1;
            this.EndIndex = -1;
        }

        public MultiSelectEventArgs(int beginIndex, int endIndex)
        {
            this.BeginIndex = beginIndex;
            this.EndIndex = endIndex;
        }

        public int BeginIndex { get; set; }

        public int EndIndex { get; set; }

        public int Count
        {
            get
            {
                if ((this.BeginIndex < 0) || (this.EndIndex < this.BeginIndex))
                {
                    return 0;
                }
                else
                {
                    return this.EndIndex - this.BeginIndex + 1;
                }
            }
        }
    }
}
