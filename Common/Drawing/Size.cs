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
    [System.ComponentModel.TypeConverter(typeof(Size.Converter))]
    public struct Size : Support.IXMLSerializable<Size>
    {
        public Size(double width, double height)
        {
            this.width = width;
            this.height = height;
        }

        public Size(System.Drawing.SizeF size)
        {
            this.width = size.Width;
            this.height = size.Height;
        }

        public Size(System.Drawing.Size size)
        {
            this.width = size.Width;
            this.height = size.Height;
        }

        [XmlAttribute]
        public double Width
        {
            get { return this.width; }
            set { this.width = value; }
        }

        [XmlAttribute]
        public double Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        public bool IsEmpty
        {
            get { return (this.width <= 0) && (this.height <= 0); }
        }

        public static readonly Size Empty;
        public static readonly Size MaxValue = new Drawing.Size(2000000000, 2000000000);
        public static readonly Size Zero = new Drawing.Size(0, 0);
        public static readonly Size NegativeInfinity = new Drawing.Size(
            double.NegativeInfinity,
            double.NegativeInfinity
        );
        public static readonly Size PositiveInfinity = new Drawing.Size(
            double.PositiveInfinity,
            double.PositiveInfinity
        );

        public Point ToPoint()
        {
            return new Point(this.width, this.height);
        }

        public override string ToString()
        {
            string arg1 = this.width.IsSafeNaN()
                ? "*"
                : this.width.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string arg2 = this.height.IsSafeNaN()
                ? "*"
                : this.height.ToString(System.Globalization.CultureInfo.InvariantCulture);

            if ((arg1 == "*") && (arg2 == "*"))
            {
                return "*";
            }
            else
            {
                return string.Concat(arg1, ";", arg2);
            }
        }

        public static bool Equal(Size a, Size b, double δ)
        {
            return Math.Equal(a.Width, b.Width, δ) && Math.Equal(a.Height, b.Height, δ);
        }

        public override bool Equals(object obj)
        {
            return (obj is Size) && (this == (Size)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Size Parse(string value)
        {
            if (value == null)
            {
                return Size.Empty;
            }
            if (value == "*")
            {
                return new Size(double.NaN, double.NaN);
            }

            string[] args = value.Split(';', ':');

            if (args.Length != 2)
            {
                throw new System.ArgumentException(
                    string.Format("Invalid size specification ({0}).", value)
                );
            }

            string argX = args[0].Trim();
            string argY = args[1].Trim();

            double x =
                argX == "*"
                    ? double.NaN
                    : System.Double.Parse(argX, System.Globalization.CultureInfo.InvariantCulture);
            double y =
                argY == "*"
                    ? double.NaN
                    : System.Double.Parse(argY, System.Globalization.CultureInfo.InvariantCulture);

            return new Size(x, y);
        }

        public static Size Parse(string value, Size defaultValue)
        {
            if (value == "*")
            {
                return defaultValue;
            }

            string[] args = value.Split(new char[] { ';', ':' });

            if (args.Length != 2)
            {
                throw new System.ArgumentException(
                    string.Format("Invalid size specification ({0}).", value)
                );
            }

            string argX = args[0].Trim();
            string argY = args[1].Trim();

            double x =
                (argX == "*")
                    ? defaultValue.Width
                    : System.Double.Parse(argX, System.Globalization.CultureInfo.InvariantCulture);
            double y =
                (argY == "*")
                    ? defaultValue.Height
                    : System.Double.Parse(argY, System.Globalization.CultureInfo.InvariantCulture);

            return new Size(x, y);
        }

        public static Size operator +(Size a, Size b)
        {
            return new Size(a.Width + b.Width, a.Height + b.Height);
        }

        public static Size operator -(Size a, Size b)
        {
            return new Size(a.Width - b.Width, a.Height - b.Height);
        }

        public static Size operator *(Size a, double value)
        {
            return new Size(a.width * value, a.height * value);
        }

        public static Size operator /(Size a, double value)
        {
            return new Size(a.width / value, a.height / value);
        }

        public static bool operator ==(Size a, Size b)
        {
            return ((a.width.IsSafeNaN() && b.width.IsSafeNaN()) || (a.width == b.width))
                && ((a.height.IsSafeNaN() && b.height.IsSafeNaN()) || (a.height == b.height));
        }

        public static bool operator !=(Size a, Size b)
        {
            return ((!a.width.IsSafeNaN() || !b.width.IsSafeNaN()) && (a.width != b.width))
                || ((!a.height.IsSafeNaN() || !b.height.IsSafeNaN()) && (a.height != b.height));
        }

        public static Size operator +(Size a, Margins b)
        {
            return new Size(a.Width + b.Width, a.Height + b.Height);
        }

        public static Size operator -(Size a, Margins b)
        {
            return new Size(
                System.Math.Max(0, a.Width - b.Width),
                System.Math.Max(0, a.Height - b.Height)
            );
        }

        public bool HasEquivalentData(Support.IXMLWritable other)
        {
            return this == (Size)other;
        }

        public XElement ToXML()
        {
            return new XElement(
                "Size",
                new XAttribute("width", this.Width),
                new XAttribute("height", this.Height)
            );
        }

        public static Size FromXML(XElement xml)
        {
            return new Size(xml);
        }

        private Size(XElement xml)
        {
            XElement root = xml.Element("Size");
            this.width = (double)root.Attribute("width");
            this.height = (double)root.Attribute("height");
        }

        #region Converter Class
        public class Converter : Types.AbstractStringConverter
        {
            public override object ParseString(
                string value,
                System.Globalization.CultureInfo culture
            )
            {
                return Size.Parse(value);
            }

            public override string ToString(object value, System.Globalization.CultureInfo culture)
            {
                Size size = (Size)value;
                return size.ToString();
            }
        }
        #endregion


        private double width;
        private double height;
    }
}
