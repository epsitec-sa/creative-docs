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
    /// The <c>WindowPlacement</c> structure describes the geometry of a window,
    /// including its state (full screen and/or minimized).
    /// </summary>
    [System.ComponentModel.TypeConverter(typeof(WindowPlacement.Converter))]
    public struct WindowPlacement : System.IEquatable<WindowPlacement>
    {
        public WindowPlacement(
            Drawing.Rectangle bounds,
            bool isFullScreen,
            bool isMinimized,
            bool isHidden
        )
        {
            this.bounds = bounds;
            this.isFullScreen = isFullScreen;
            this.isMinimized = isMinimized;
            this.isHidden = isHidden;
        }

        public Drawing.Rectangle Bounds
        {
            get { return this.bounds; }
        }

        public bool IsFullScreen
        {
            get { return this.isFullScreen; }
        }

        public bool IsMinimized
        {
            get { return this.isMinimized; }
        }

        public bool IsHidden
        {
            get { return this.isHidden; }
        }

        #region IEquatable<WindowPlacement> Members

        public bool Equals(WindowPlacement other)
        {
            return this.bounds == other.bounds
                && this.isFullScreen == other.isFullScreen
                && this.isMinimized == other.isMinimized
                && this.isHidden == other.isHidden;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is WindowPlacement)
            {
                return this.Equals((WindowPlacement)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.bounds.GetHashCode();
        }

        public override string ToString()
        {
            return string.Concat(
                this.bounds.ToString(),
                ";",
                this.isFullScreen ? "F" : "",
                this.isMinimized ? "M" : "",
                this.isHidden ? "H" : ""
            );
        }

        public static WindowPlacement Parse(string value)
        {
            int pos = value.LastIndexOf(';');

            if (pos < 0)
            {
                throw new System.FormatException();
            }

            string rect = value.Substring(0, pos);
            string attr = value.Substring(pos + 1);

            return new WindowPlacement(
                Drawing.Rectangle.Parse(rect),
                attr.Contains("F"),
                attr.Contains("M"),
                attr.Contains("H")
            );
        }

        #region Converter Class

        public class Converter : Types.AbstractStringConverter
        {
            public override object ParseString(
                string value,
                System.Globalization.CultureInfo culture
            )
            {
                return WindowPlacement.Parse(value);
            }

            public override string ToString(object value, System.Globalization.CultureInfo culture)
            {
                WindowPlacement placement = (WindowPlacement)value;
                return placement.ToString();
            }
        }

        #endregion

        private readonly Drawing.Rectangle bounds;
        private readonly bool isFullScreen;
        private readonly bool isMinimized;
        private readonly bool isHidden;
    }
}
