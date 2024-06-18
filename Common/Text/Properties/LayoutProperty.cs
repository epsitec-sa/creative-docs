//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD
using System.Xml.Linq;

namespace Epsitec.Common.Text.Properties
{
    /// <summary>
    /// La classe LayoutProperty décrit quel moteur de layout utiliser pour
    /// un fragment de texte.
    /// </summary>
    public class LayoutProperty : Property, Common.Support.IXMLSerializable<LayoutProperty>
    {
        public LayoutProperty() { }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.Layout; }
        }

        public override PropertyType PropertyType
        {
            get { return PropertyType.CoreSetting; }
        }

        public override CombinationMode CombinationMode
        {
            get { return CombinationMode.Invalid; }
        }

        public string EngineName
        {
            get { return this.engineName; }
        }

        public override Property EmptyClone()
        {
            return new LayoutProperty();
        }

        public override void SerializeToText(System.Text.StringBuilder buffer)
        {
            SerializerSupport.Join(buffer, SerializerSupport.SerializeString(this.engineName));
        }

        public override bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            LayoutProperty other = (LayoutProperty)otherWritable;
            return this.engineName == other.engineName;
        }

        public override XElement ToXML()
        {
            return new XElement("LayoutProperty", new XAttribute("EngineName", this.engineName));
        }

        public static LayoutProperty FromXML(XElement xml)
        {
            return new LayoutProperty(xml);
        }

        private LayoutProperty(XElement xml)
        {
            this.engineName = xml.Attribute("EngineName").Value;
        }

        public override void DeserializeFromText(
            TextContext context,
            string text,
            int pos,
            int length
        )
        {
            this.engineName = SerializerSupport.DeserializeString(text.Substring(pos, length));
        }

        public override Property GetCombination(Property property)
        {
            throw new System.InvalidOperationException("Cannot combine layouts.");
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            checksum.UpdateValue(this.engineName);
        }

        public override bool CompareEqualContents(object value)
        {
            return LayoutProperty.CompareEqualContents(this, value as LayoutProperty);
        }

        private static bool CompareEqualContents(LayoutProperty a, LayoutProperty b)
        {
            if (a.engineName == b.engineName)
            {
                return true;
            }

            return false;
        }

        private string engineName;
    }
}
