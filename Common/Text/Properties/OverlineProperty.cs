//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD
using System.Xml.Linq;

namespace Epsitec.Common.Text.Properties
{
    /// <summary>
    /// La classe OverlineProperty permet de régler les détails relatifs au
    /// surlignement du texte.
    /// </summary>
    public class OverlineProperty
        : AbstractXlineProperty,
            Common.Support.IXMLSerializable<OverlineProperty>
    {
        public OverlineProperty() { }

        public OverlineProperty(
            double position,
            SizeUnits positionUnits,
            double thickness,
            SizeUnits thicknessUnits,
            string drawClass,
            string drawStyle
        )
            : base(position, positionUnits, thickness, thicknessUnits, drawClass, drawStyle) { }

        public static OverlineProperty DisableOverride
        {
            get
            {
                OverlineProperty property = new OverlineProperty();

                property.Disable();

                return property;
            }
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.Overline; }
        }

        public override Property EmptyClone()
        {
            return new OverlineProperty();
        }

        public static OverlineProperty FromXML(XElement xml)
        {
            return new OverlineProperty(xml);
        }

        private OverlineProperty(XElement xml)
            : base(xml) { }
    }
}
