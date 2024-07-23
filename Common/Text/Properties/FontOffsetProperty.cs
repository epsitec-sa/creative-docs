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

namespace Epsitec.Common.Text.Properties
{
    /// <summary>
    /// La classe FontOffsetProperty décrit un décalage vertical de la ligne
    /// de base d'un caractère.
    /// </summary>
    public class FontOffsetProperty : Property, Common.Support.IXMLSerializable<FontOffsetProperty>
    {
        public FontOffsetProperty()
            : this(0, SizeUnits.Points) { }

        public FontOffsetProperty(double offset, SizeUnits units)
        {
            this.offset = offset;
            this.units = units;
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.FontOffset; }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.LocalSetting; }
        }

        public double Offset
        {
            get { return this.offset; }
        }

        public SizeUnits Units
        {
            get { return this.units; }
        }

        public double GetOffsetInPoints(double fontSizeInPoints)
        {
            if (UnitsTools.IsAbsoluteSize(this.units))
            {
                return UnitsTools.ConvertToPoints(this.offset, this.units);
            }
            if (UnitsTools.IsRelativeSize(this.units))
            {
                return UnitsTools.ConvertToPoints(this.offset, this.units) + fontSizeInPoints;
            }
            if (UnitsTools.IsScale(this.units))
            {
                return UnitsTools.ConvertToScale(this.offset, this.units) * fontSizeInPoints;
            }

            throw new System.InvalidOperationException();
        }

        public override Property EmptyClone()
        {
            return new FontOffsetProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeDouble(this.offset),
                /**/SerializerSupport.SerializeSizeUnits(this.units)
            );
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            FontOffsetProperty other = (FontOffsetProperty)otherWritable;
            return this.offset == other.offset && this.units == other.units;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "FontOffsetProperty",
                new XAttribute("Offset", this.offset),
                new XAttribute("Units", this.units)
            );
        }

        public static FontOffsetProperty FromXML(XElement xml)
        {
            return new FontOffsetProperty(xml);
        }

        private FontOffsetProperty(XElement xml)
        {
            this.offset = (double)xml.Attribute("Offset");
            System.Enum.TryParse(xml.Attribute("Units").Value, out this.units);
        }

        public override void DeserializeFromText(
            TextContext context,
            string text,
            int pos,
            int length
        )
        {
            string[] args = SerializerSupport.Split(text, pos, length);

            Debug.Assert.IsTrue(args.Length == 2);

            double offset = SerializerSupport.DeserializeDouble(args[0]);
            SizeUnits units = SerializerSupport.DeserializeSizeUnits(args[1]);

            this.offset = offset;
            this.units = units;
        }

        public override Property GetCombination(Property property)
        {
            Debug.Assert.IsTrue(property is Properties.FontOffsetProperty);

            FontOffsetProperty a = this;
            FontOffsetProperty b = property as FontOffsetProperty;
            FontOffsetProperty c = new FontOffsetProperty();

            UnitsTools.Combine(a.offset, a.units, b.offset, b.units, out c.offset, out c.units);

            return c;
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue(this.offset);
            checksum.UpdateValue((int)this.units);
        }

        public override bool CompareEqualContents(object value)
        {
            return FontOffsetProperty.CompareEqualContents(this, value as FontOffsetProperty);
        }

        private static bool CompareEqualContents(FontOffsetProperty a, FontOffsetProperty b)
        {
            return NumberSupport.Equal(a.offset, b.offset) && a.units == b.units;
        }

        private double offset;
        private SizeUnits units;
    }
}
