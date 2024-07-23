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

namespace Epsitec.Common.Document
{
    using XmlAttribute = System.Xml.Serialization.XmlAttributeAttribute;

    [System.Serializable]
    public struct Polar : Support.IXMLSerializable<Polar>
    {
        public Polar(double r, double a)
        {
            this.r = r;
            this.a = a;
        }

        [XmlAttribute]
        public double R
        {
            //	Distance à l'origine.
            get { return this.r; }
            set { this.r = value; }
        }

        [XmlAttribute]
        public double A
        {
            //	Angle en degrés.
            get { return this.a; }
            set { this.a = value; }
        }

        public bool IsZero
        {
            get { return this.r == 0 && this.a == 0; }
        }

        public static readonly Polar Zero;

        public override string ToString()
        {
            return System.String.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0};{1}\u00B0",
                this.r,
                this.a
            );
        }

        public override bool Equals(object obj)
        {
            return (obj is Polar) && (this == (Polar)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static Polar operator +(Polar a, Polar b)
        {
            return new Polar(a.r + b.r, a.a + b.a);
        }

        public static Polar operator -(Polar a, Polar b)
        {
            return new Polar(a.r - b.r, a.a - b.a);
        }

        public static Polar operator -(Polar a)
        {
            return new Polar(-a.r, a.a);
        }

        public static bool operator ==(Polar a, Polar b)
        {
            return (a.r == b.r) && (a.a == b.a);
        }

        public static bool operator !=(Polar a, Polar b)
        {
            return (a.r != b.r) || (a.a != b.a);
        }

        public bool HasEquivalentData(Support.IXMLWritable other)
        {
            Polar otherPolar = (Polar)other;
            return otherPolar == this;
        }

        public XElement ToXML()
        {
            return new XElement("Polar", new XAttribute("R", this.r), new XAttribute("A", this.a));
        }

        public static Polar FromXML(XElement xml)
        {
            return new Polar(xml);
        }

        private Polar(XElement xml)
        {
            this.r = (double)xml.Attribute("R");
            this.a = (double)xml.Attribute("A");
        }

        private double r;
        private double a;
    }
}
