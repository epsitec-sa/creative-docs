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


namespace Epsitec.Common.Drawing
{
    [System.Serializable]
    /// <summary>
    /// The <c>ImageFilter</c> structure defines the image filtering setting.
    /// </summary>
    public struct ImageFilter : System.IEquatable<ImageFilter>
    {
        public ImageFilter(ImageFilteringMode mode)
        {
            this.mode = mode;
            this.radius = 1.0;
        }

        public ImageFilter(ImageFilteringMode mode, double radius)
        {
            this.mode = mode;
            this.radius = radius;
        }

        public ImageFilteringMode Mode
        {
            get { return this.mode; }
        }

        /// <summary>
        /// Gets the radius for the <c>Sinc</c>, <c>Lanczos</c> and <c>Blackman</c> filtering
        /// modes.
        /// </summary>
        /// <value>The radius.</value>
        public double Radius
        {
            //	Le rayon est effectif pour les modes Sinc (c'est la fonction "sin(x)/x"), Lanczos et Blackman.
            get { return this.radius; }
        }

        public bool Active
        {
            get { return this.mode != ImageFilteringMode.None; }
        }

        public override bool Equals(object obj)
        {
            if (obj is ImageFilter)
            {
                return this == (ImageFilter)obj;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.mode.GetHashCode() ^ this.radius.GetHashCode();
        }

        public override string ToString()
        {
            return string.Concat(
                this.mode.ToString(),
                ":",
                this.radius.ToString(System.Globalization.CultureInfo.InvariantCulture)
            );
        }

        public static ImageFilter Parse(string text)
        {
            string[] args = text.Split(':');

            if (args.Length != 2)
            {
                throw new System.FormatException("Invalid format");
            }

            var mode = Epsitec.Common.Types.InvariantConverter.ToEnum<ImageFilteringMode>(args[0]);
            var radius = Epsitec.Common.Types.InvariantConverter.ToDouble(args[1]);

            return new ImageFilter(mode, radius);
        }

        #region IEquatable<ImageFilter> Members

        public bool Equals(ImageFilter other)
        {
            return this == other;
        }

        #endregion

        public static bool operator ==(ImageFilter a, ImageFilter b)
        {
            return (a.mode == b.mode) && (a.radius == b.radius);
        }

        public static bool operator !=(ImageFilter a, ImageFilter b)
        {
            return (a.mode != b.mode) || (a.radius != b.radius);
        }

        private ImageFilteringMode mode;
        private double radius;
    }
}
