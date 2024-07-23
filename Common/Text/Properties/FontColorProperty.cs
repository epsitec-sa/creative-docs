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
    /// La classe FontColorProperty définit la couleur à appliquer au corps
    /// du texte.
    /// </summary>
    public class FontColorProperty : Property, Common.Support.IXMLSerializable<FontColorProperty>
    {
        public FontColorProperty() { }

        public FontColorProperty(string textColor)
        {
            this.textColor = textColor;
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.FontColor; }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.ExtraSetting; }
        }

        public string TextColor
        {
            get { return this.textColor; }
        }

        public override Property EmptyClone()
        {
            return new FontColorProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeString(this.textColor)
            );
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            FontColorProperty other = (FontColorProperty)otherWritable;
            return this.textColor == other.textColor;
        }

        public override XElement ToXML()
        {
            return new XElement("FontColorProperty", new XAttribute("TextColor", this.textColor));
        }

        public static FontColorProperty FromXML(XElement xml)
        {
            return new FontColorProperty(xml);
        }

        private FontColorProperty(XElement xml)
        {
            this.textColor = xml.Attribute("TextColor").Value;
        }

        public override void DeserializeFromText(
            TextContext context,
            string text,
            int pos,
            int length
        )
        {
            string[] args = SerializerSupport.Split(text, pos, length);

            Debug.Assert.IsTrue(args.Length == 1);

            string textColor = SerializerSupport.DeserializeString(args[0]);

            this.textColor = textColor;
        }

        public override Property GetCombination(Property property)
        {
            Debug.Assert.IsTrue(property is Properties.FontColorProperty);

            FontColorProperty a = this;
            FontColorProperty b = property as FontColorProperty;
            FontColorProperty c = new FontColorProperty();

            c.textColor = (b.textColor == null) ? a.textColor : b.textColor;

            return c;
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue(this.textColor);
        }

        public override bool CompareEqualContents(object value)
        {
            return FontColorProperty.CompareEqualContents(this, value as FontColorProperty);
        }

        private static bool CompareEqualContents(FontColorProperty a, FontColorProperty b)
        {
            return a.textColor == b.textColor;
        }

        private string textColor;
    }
}
