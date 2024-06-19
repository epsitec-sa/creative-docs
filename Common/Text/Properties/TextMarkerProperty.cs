//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD
using System.Xml.Linq;

namespace Epsitec.Common.Text.Properties
{
    /// <summary>
    /// La classe TextMarkerProperty permet de régler les détails relatifs à
    /// la mise en évidence (au marquer "Stabylo") du texte.
    /// </summary>
    public class TextMarkerProperty
        : AbstractXlineProperty,
            Common.Support.IXMLSerializable<TextMarkerProperty>
    {
        public TextMarkerProperty() { }

        public TextMarkerProperty(
            double position,
            SizeUnits positionUnits,
            double thickness,
            SizeUnits thicknessUnits,
            string drawClass,
            string drawStyle
        )
            : base(position, positionUnits, thickness, thicknessUnits, drawClass, drawStyle) { }

        public static TextMarkerProperty DisableOverride
        {
            get
            {
                TextMarkerProperty property = new TextMarkerProperty();

                property.Disable();

                return property;
            }
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.TextMarker; }
        }

        public override Property EmptyClone()
        {
            return new TextMarkerProperty();
        }

        public override XElement ToXML()
        {
            return new XElement("TextMarkerProperty", base.IterXMLParts());
        }

        public static TextMarkerProperty FromXML(XElement xml)
        {
            return new TextMarkerProperty(xml);
        }

        private TextMarkerProperty(XElement xml)
            : base(xml) { }
    }
}
