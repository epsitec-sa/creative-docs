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

        public override XElement ToXML()
        {
            return new XElement("OverlineProperty", base.IterXMLParts());
        }

        public static OverlineProperty FromXML(XElement xml)
        {
            return new OverlineProperty(xml);
        }

        private OverlineProperty(XElement xml)
            : base(xml) { }
    }
}
