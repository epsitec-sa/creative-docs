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


namespace Epsitec.Common.Text.Exchange
{
    /// <summary>
    /// La classe SpanStyle gère la création de la valeur de l'attribut style du tag html <span ...>
    /// Par exempe: "span style='font-size:8.0pt;font-family: ....'>"
    /// </summary>
    ///
    public class SpanStyle : System.IEquatable<SpanStyle>
    {
        public SpanStyle(string fontname, double fontsize, string fontcolor)
        {
            this.SetFontFamily(fontname);
            this.SetFontSize(fontsize);
            this.SetColor(fontcolor);
        }

        public void ClearFontSize()
        {
            this.fontSize = 0;
        }

        public void ClearFontFamily()
        {
            this.fontFamily = "";
        }

        public void ClearColor()
        {
            this.fontColor = "";
        }

        public void Clear()
        {
            this.ClearFontSize();
            this.ClearFontFamily();
            this.ClearColor();
        }

        public void SetFontSize(double size)
        {
            this.fontSize = size;
        }

        public void SetFontFamily(string family)
        {
            this.fontFamily = family;
        }

        public void SetColor(string color)
        {
            this.fontColor = color;
        }

        public void SetColor(int r, int g, int b) { }

        public override string ToString()
        {
            string result;

            result = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "font-size:{0:F1}pt;font-family:\"{1}\";color:{2}",
                this.fontSize,
                this.fontFamily,
                this.fontColor
            );

            return result;
        }

        #region IEquatable<SpanStyle> Members

        public bool Equals(SpanStyle other)
        {
            return this == other;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return this == (obj as SpanStyle);
        }

#if false
		public override bool Equals(object obj)
		{
			return this.Equals (obj as SpanStyle);
		}
#endif
        public override int GetHashCode()
        {
            return this.fontSize.GetHashCode()
                ^ this.fontFamily.GetHashCode()
                ^ this.fontColor.GetHashCode();
        }

        public static bool operator ==(SpanStyle s1, SpanStyle s2)
        {
            if (System.Object.ReferenceEquals(s1, s2))
            {
                return true;
            }
            if (System.Object.ReferenceEquals(s1, null) || System.Object.ReferenceEquals(s2, null))
            {
                return false;
            }

            return s1.fontSize == s2.fontSize
                && s1.fontFamily == s2.fontFamily
                && s1.fontColor == s2.fontColor;
        }

        public static bool operator !=(SpanStyle s1, SpanStyle s2)
        {
            return !(s1 == s2);
        }

        private double fontSize;
        private string fontFamily;
        private string fontColor;
    }
}
