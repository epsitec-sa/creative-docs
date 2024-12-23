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
    /// La propriété KeepProperty détermine comment un paragraphe se comporte
    /// par rapport aux autres (paragraphes liés) et comment les veuves et les
    /// orphelines sont gérées.
    /// </summary>
    public class KeepProperty : Property, Common.Support.IXMLSerializable<KeepProperty>
    {
        public KeepProperty() { }

        public KeepProperty(
            int startLines,
            int endLines,
            ParagraphStartMode mode,
            ThreeState withPrevParagraph,
            ThreeState withNextParagraph
        )
        {
            this.startLines = startLines;
            this.endLines = endLines;

            this.paragraphStartMode = mode;
            this.withPrevParagraph = withPrevParagraph;
            this.withNextParagraph = withNextParagraph;
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.Keep; }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.CoreSetting; }
        }

        public override bool RequiresUniformParagraph
        {
            get { return true; }
        }

        public int StartLines
        {
            get { return this.startLines; }
        }

        public int EndLines
        {
            get { return this.endLines; }
        }

        public ParagraphStartMode ParagraphStartMode
        {
            get { return this.paragraphStartMode; }
        }

        public ThreeState KeepWithNextParagraph
        {
            get { return this.withNextParagraph; }
        }

        public ThreeState KeepWithPreviousParagraph
        {
            get { return this.withPrevParagraph; }
        }

        public override Property EmptyClone()
        {
            return new KeepProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeInt(this.startLines),
                /**/SerializerSupport.SerializeInt(this.endLines),
                /**/SerializerSupport.SerializeEnum(this.paragraphStartMode),
                /**/SerializerSupport.SerializeThreeState(this.withNextParagraph),
                /**/SerializerSupport.SerializeThreeState(this.withPrevParagraph)
            );
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            KeepProperty other = (KeepProperty)otherWritable;
            return this.startLines == other.startLines
                && this.endLines == other.endLines
                && this.paragraphStartMode == other.paragraphStartMode
                && this.withNextParagraph == other.withNextParagraph
                && this.withPrevParagraph == other.withPrevParagraph;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "KeepProperty",
                new XAttribute("StartLines", this.startLines),
                new XAttribute("EndLines", this.endLines),
                new XAttribute("ParagraphStartMode", this.paragraphStartMode),
                new XAttribute("WithNextParagraph", this.withNextParagraph),
                new XAttribute("WithPrevParagraph", this.withPrevParagraph)
            );
        }

        public static KeepProperty FromXML(XElement xml)
        {
            return new KeepProperty(xml);
        }

        private KeepProperty(XElement xml)
        {
            this.startLines = (int)xml.Attribute("StartLines");
            this.endLines = (int)xml.Attribute("EndLines");
            System.Enum.TryParse(
                xml.Attribute("ParagraphStartMode").Value,
                out this.paragraphStartMode
            );
            System.Enum.TryParse(
                xml.Attribute("WithNextParagraph").Value,
                out this.withNextParagraph
            );
            System.Enum.TryParse(
                xml.Attribute("WithPrevParagraph").Value,
                out this.withPrevParagraph
            );
        }

        public override void DeserializeFromText(
            TextContext context,
            string text,
            int pos,
            int length
        )
        {
            string[] args = SerializerSupport.Split(text, pos, length);

            Debug.Assert.IsTrue(args.Length == 5);

            int startLines = SerializerSupport.DeserializeInt(args[0]);
            int endLines = SerializerSupport.DeserializeInt(args[1]);

            ParagraphStartMode paragraphStartMode = (ParagraphStartMode)
                SerializerSupport.DeserializeEnum(typeof(ParagraphStartMode), args[2]);
            ThreeState withNextParagraph = SerializerSupport.DeserializeThreeState(args[3]);
            ThreeState withPrevParagraph = SerializerSupport.DeserializeThreeState(args[4]);

            this.startLines = startLines;
            this.endLines = endLines;
            this.paragraphStartMode = paragraphStartMode;
            this.withNextParagraph = withNextParagraph;
            this.withPrevParagraph = withPrevParagraph;
        }

        public override Property GetCombination(Property property)
        {
            Debug.Assert.IsTrue(property is Properties.KeepProperty);

            KeepProperty a = this;
            KeepProperty b = property as KeepProperty;
            KeepProperty c = new KeepProperty();

            c.startLines = b.startLines == 0 ? a.startLines : b.startLines;
            c.endLines = b.endLines == 0 ? a.endLines : b.endLines;

            c.paragraphStartMode =
                b.paragraphStartMode == ParagraphStartMode.Undefined
                    ? a.paragraphStartMode
                    : b.paragraphStartMode;
            c.withNextParagraph =
                b.withNextParagraph == ThreeState.Undefined
                    ? a.withNextParagraph
                    : b.withNextParagraph;
            c.withPrevParagraph =
                b.withPrevParagraph == ThreeState.Undefined
                    ? a.withPrevParagraph
                    : b.withPrevParagraph;

            return c;
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue(this.startLines);
            checksum.UpdateValue(this.endLines);
            checksum.UpdateValue((int)this.paragraphStartMode);
            checksum.UpdateValue((int)this.withNextParagraph);
            checksum.UpdateValue((int)this.withPrevParagraph);
        }

        public override bool CompareEqualContents(object value)
        {
            return KeepProperty.CompareEqualContents(this, value as KeepProperty);
        }

        private static bool CompareEqualContents(KeepProperty a, KeepProperty b)
        {
            return a.startLines == b.startLines
                && a.endLines == b.endLines
                && a.paragraphStartMode == b.paragraphStartMode
                && a.withNextParagraph == b.withNextParagraph
                && a.withPrevParagraph == b.withPrevParagraph;
        }

        private int startLines;
        private int endLines;

        private ParagraphStartMode paragraphStartMode;
        private ThreeState withNextParagraph;
        private ThreeState withPrevParagraph;
    }
}
