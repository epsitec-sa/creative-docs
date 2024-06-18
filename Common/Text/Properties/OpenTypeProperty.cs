//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD
using System.Xml.Linq;

namespace Epsitec.Common.Text.Properties
{
    /// <summary>
    /// La classe OpenTypeProperty donne accès aux glyphes supplémentaires d'une
    /// fonte OpenType (variantes, etc.)
    /// </summary>
    public class OpenTypeProperty : Property, Common.Support.IXMLSerializable<OpenTypeProperty>
    {
        public OpenTypeProperty() { }

        public OpenTypeProperty(string fontFace, string fontStyle, int glyphIndex)
        {
            this.fontFace = fontFace;
            this.fontStyle = fontStyle;
            this.glyphIndex = glyphIndex;
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.OpenType; }
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

        public override bool RequiresSpecialCodeProcessing
        {
            get { return true; }
        }

        public string FontFace
        {
            get { return this.fontFace; }
        }

        public string FontStyle
        {
            get { return this.fontStyle; }
        }

        public int GlyphIndex
        {
            get { return this.glyphIndex; }
        }

        public override int GetGlyphForSpecialCode(ulong code)
        {
            return this.glyphIndex;
        }

        public override OpenType.Font GetFontForSpecialCode(TextContext context, ulong code)
        {
            return TextContext.GetFont(this.fontFace, this.fontStyle);
        }

        public override Property EmptyClone()
        {
            return new OpenTypeProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(
                buffer,
                /**/SerializerSupport.SerializeString(this.fontFace),
                /**/SerializerSupport.SerializeString(this.fontStyle),
                /**/SerializerSupport.SerializeInt(this.glyphIndex)
            );
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            OpenTypeProperty other = (OpenTypeProperty)otherWritable;
            return this.fontFace == other.fontFace
                && this.fontStyle == other.fontStyle
                && this.glyphIndex == other.glyphIndex;
        }

        public override XElement ToXML()
        {
            return new XElement(
                "OpenTypeProperty",
                new XAttribute("FontFace", this.fontFace),
                new XAttribute("FontStyle", this.fontStyle),
                new XAttribute("GlyphIndex", this.glyphIndex)
            );
        }

        public static OpenTypeProperty FromXML(XElement xml)
        {
            return new OpenTypeProperty(xml);
        }

        private OpenTypeProperty(XElement xml)
        {
            this.fontFace = xml.Attribute("FontFace").Value;
            this.fontStyle = xml.Attribute("FontStyle").Value;
            this.glyphIndex = (int)xml.Attribute("GlyphIndex");
        }

        public override void DeserializeFromText(
            TextContext context,
            string text,
            int pos,
            int length
        )
        {
            string[] args = SerializerSupport.Split(text, pos, length);

            Debug.Assert.IsTrue(args.Length == 3);

            string fontFace = SerializerSupport.DeserializeString(args[0]);
            string fontStyle = SerializerSupport.DeserializeString(args[1]);
            int glyphIndex = SerializerSupport.DeserializeInt(args[2]);

            this.fontFace = fontFace;
            this.fontStyle = fontStyle;
            this.glyphIndex = glyphIndex;
        }

        public override Property GetCombination(Property property)
        {
            throw new System.InvalidOperationException("Cannot combine OpenType properties.");
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue(this.fontFace);
            checksum.UpdateValue(this.fontStyle);
            checksum.UpdateValue(this.glyphIndex);
        }

        public override bool CompareEqualContents(object value)
        {
            return OpenTypeProperty.CompareEqualContents(this, value as OpenTypeProperty);
        }

        private static bool CompareEqualContents(OpenTypeProperty a, OpenTypeProperty b)
        {
            return a.fontFace == b.fontFace
                && a.fontStyle == b.fontStyle
                && a.glyphIndex == b.glyphIndex;
        }

        private string fontFace;
        private string fontStyle;
        private int glyphIndex;
    }
}
