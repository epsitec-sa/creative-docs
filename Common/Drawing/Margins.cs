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

using System.Xml.Linq;

namespace Epsitec.Common.Drawing
{
    using XmlAttribute = System.Xml.Serialization.XmlAttributeAttribute;

    [System.Serializable]
    [System.ComponentModel.TypeConverter(typeof(Margins.Converter))]
    public struct Margins : Support.IXMLSerializable<Margins>
    {
        public Margins(double left, double right, double top, double bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public Margins(double margin)
        {
            this.left = margin;
            this.right = margin;
            this.top = margin;
            this.bottom = margin;
        }

        [XmlAttribute]
        public double Left
        {
            get { return this.left; }
            set { this.left = value; }
        }

        [XmlAttribute]
        public double Right
        {
            get { return this.right; }
            set { this.right = value; }
        }

        [XmlAttribute]
        public double Top
        {
            get { return this.top; }
            set { this.top = value; }
        }

        [XmlAttribute]
        public double Bottom
        {
            get { return this.bottom; }
            set { this.bottom = value; }
        }

        public double Width
        {
            get { return this.left + this.right; }
        }

        public double Height
        {
            get { return this.top + this.bottom; }
        }

        public Size Size
        {
            get { return new Size(this.Width, this.Height); }
        }

        public void ClipNegative()
        {
            if (this.left < 0)
            {
                this.left = 0;
            }
            if (this.right < 0)
            {
                this.right = 0;
            }
            if (this.top < 0)
            {
                this.top = 0;
            }
            if (this.bottom < 0)
            {
                this.bottom = 0;
            }
        }

        public static readonly Margins Zero = new Margins(0, 0, 0, 0);

        public static readonly Margins NaN = new Margins(
            double.NaN,
            double.NaN,
            double.NaN,
            double.NaN
        );

        public override string ToString()
        {
            return System.String.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0};{1};{2};{3}",
                this.left,
                this.right,
                this.top,
                this.bottom
            );
        }

        public static bool Equal(Margins a, Margins b, double δ)
        {
            return Math.Equal(a.Left, b.Left, δ)
                && Math.Equal(a.Right, b.Right, δ)
                && Math.Equal(a.Top, b.Top, δ)
                && Math.Equal(a.Bottom, b.Bottom, δ);
        }

        public override bool Equals(object obj)
        {
            return (obj is Margins) && (this == (Margins)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Margins Parse(string value)
        {
            string[] args = value.Split(new char[] { ';', ':' });

            if (args.Length != 4)
            {
                throw new System.ArgumentException(
                    string.Format("Invalid margins specification ({0})", value)
                );
            }

            string argX1 = args[0].Trim();
            string argX2 = args[1].Trim();
            string argY1 = args[2].Trim();
            string argY2 = args[3].Trim();

            double x1 = System.Double.Parse(
                argX1,
                System.Globalization.CultureInfo.InvariantCulture
            );
            double x2 = System.Double.Parse(
                argX2,
                System.Globalization.CultureInfo.InvariantCulture
            );
            double y1 = System.Double.Parse(
                argY1,
                System.Globalization.CultureInfo.InvariantCulture
            );
            double y2 = System.Double.Parse(
                argY2,
                System.Globalization.CultureInfo.InvariantCulture
            );

            return new Margins(x1, x2, y1, y2);
        }

        public static Margins Parse(string value, Margins defaultValue)
        {
            string[] args = value.Split(new char[] { ';', ':' });

            if (args.Length != 4)
            {
                throw new System.ArgumentException(
                    string.Format("Invalid margins specification ({0})", value)
                );
            }

            string argX1 = args[0].Trim();
            string argX2 = args[1].Trim();
            string argY1 = args[2].Trim();
            string argY2 = args[3].Trim();

            if (argX1 != "*")
                defaultValue.Left = System.Double.Parse(
                    argX1,
                    System.Globalization.CultureInfo.InvariantCulture
                );
            if (argX2 != "*")
                defaultValue.Right = System.Double.Parse(
                    argX2,
                    System.Globalization.CultureInfo.InvariantCulture
                );
            if (argY1 != "*")
                defaultValue.Top = System.Double.Parse(
                    argY1,
                    System.Globalization.CultureInfo.InvariantCulture
                );
            if (argY2 != "*")
                defaultValue.Bottom = System.Double.Parse(
                    argY2,
                    System.Globalization.CultureInfo.InvariantCulture
                );

            return defaultValue;
        }

        public static bool operator ==(Margins a, Margins b)
        {
            return (a.left == b.left)
                && (a.right == b.right)
                && (a.top == b.top)
                && (a.bottom == b.bottom);
        }

        public static bool operator !=(Margins a, Margins b)
        {
            return (a.left != b.left)
                || (a.right != b.right)
                || (a.top != b.top)
                || (a.bottom != b.bottom);
        }

        public static Margins operator -(Margins a)
        {
            return new Margins(-a.left, -a.right, -a.top, -a.bottom);
        }

        public static Margins operator +(Margins a, Margins b)
        {
            return new Margins(
                a.left + b.left,
                a.right + b.right,
                a.top + b.top,
                a.bottom + b.bottom
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
                return Margins.Parse(value);
            }

            public override string ToString(object value, System.Globalization.CultureInfo culture)
            {
                Margins margins = (Margins)value;
                return string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "{0};{1};{2};{3}",
                    margins.Left,
                    margins.Right,
                    margins.Top,
                    margins.Bottom
                );
            }

            public static string ToString(
                object value,
                bool suppressLeft,
                bool suppressRight,
                bool suppressTop,
                bool suppressBottom
            )
            {
                Margins margins = (Margins)value;

                string arg1 = suppressLeft
                    ? "*"
                    : margins.Left.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string arg2 = suppressRight
                    ? "*"
                    : margins.Right.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string arg3 = suppressTop
                    ? "*"
                    : margins.Top.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string arg4 = suppressBottom
                    ? "*"
                    : margins.Bottom.ToString(System.Globalization.CultureInfo.InvariantCulture);

                return string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "{0};{1};{2};{3}",
                    arg1,
                    arg2,
                    arg3,
                    arg4
                );
            }
        }
        #endregion

        public bool HasEquivalentData(Support.IXMLWritable other)
        {
            Margins otherMargins = (Margins)other;
            return this.left == otherMargins.left
                && this.right == otherMargins.right
                && this.top == otherMargins.top
                && this.bottom == otherMargins.bottom;
        }

        public XElement ToXML()
        {
            return new XElement(
                "Margins",
                new XAttribute("Left", this.left),
                new XAttribute("Right", this.right),
                new XAttribute("Top", this.top),
                new XAttribute("Bottom", this.bottom)
            );
        }

        public static Margins FromXML(XElement xml)
        {
            return new Margins(xml);
        }

        private Margins(XElement xml)
        {
            this.left = (double)xml.Attribute("Left");
            this.right = (double)xml.Attribute("Right");
            this.top = (double)xml.Attribute("Top");
            this.bottom = (double)xml.Attribute("Bottom");
        }

        private double left;
        private double right;
        private double top;
        private double bottom;
    }
}
