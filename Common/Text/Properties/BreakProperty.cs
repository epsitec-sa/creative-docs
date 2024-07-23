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
    /// La classe BreakProperty décrit un saut vertical (de page, section, etc.)
    /// </summary>
    public class BreakProperty : Property, Common.Support.IXMLSerializable<BreakProperty>
    {
        public BreakProperty()
            : this(ParagraphStartMode.Anywhere) { }

        public BreakProperty(ParagraphStartMode startMode)
        {
            this.startMode = startMode;
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.Break; }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.LocalSetting; }
        }

        public override PropertyAffinity PropertyAffinity
        {
            get { return PropertyAffinity.Symbol; }
        }

        public override CombinationMode CombinationMode
        {
            get { return CombinationMode.Invalid; }
        }

        public ParagraphStartMode ParagraphStartMode
        {
            get { return this.startMode; }
        }

        public static BreakProperty NewFrame
        {
            get { return new BreakProperty(ParagraphStartMode.NewFrame); }
        }

        public static BreakProperty NewPage
        {
            get { return new BreakProperty(ParagraphStartMode.NewPage); }
        }

        public override Property EmptyClone()
        {
            return new BreakProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeInt((int)this.startMode)
            );
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            BreakProperty other = (BreakProperty)otherWritable;
            return this.startMode == other.startMode;
        }

        public override XElement ToXML()
        {
            return new XElement("BreakProperty", new XAttribute("StartMode", this.startMode));
        }

        public static BreakProperty FromXML(XElement xml)
        {
            return new BreakProperty(xml);
        }

        private BreakProperty(XElement xml)
        {
            System.Enum.TryParse(xml.Attribute("StartMode").Value, out this.startMode);
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

            int startMode = SerializerSupport.DeserializeInt(args[0]);

            this.startMode = (ParagraphStartMode)startMode;
        }

        public override Property GetCombination(Property property)
        {
            throw new System.InvalidOperationException();
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue((int)this.startMode);
        }

        public override bool CompareEqualContents(object value)
        {
            return BreakProperty.CompareEqualContents(this, value as BreakProperty);
        }

        private static bool CompareEqualContents(BreakProperty a, BreakProperty b)
        {
            return a.startMode == b.startMode;
        }

        private ParagraphStartMode startMode;
    }
}
