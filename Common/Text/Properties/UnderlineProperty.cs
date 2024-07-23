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
    /// La classe UnderlineProperty permet de régler les détails relatifs au
    /// soulignement du texte.
    /// </summary>
    public class UnderlineProperty
        : AbstractXlineProperty,
            Common.Support.IXMLSerializable<UnderlineProperty>
    {
        public UnderlineProperty() { }

        public UnderlineProperty(
            double position,
            SizeUnits positionUnits,
            double thickness,
            SizeUnits thicknessUnits,
            string drawClass,
            string drawStyle
        )
            : base(position, positionUnits, thickness, thicknessUnits, drawClass, drawStyle) { }

        public static UnderlineProperty DisableOverride
        {
            get
            {
                UnderlineProperty property = new UnderlineProperty();

                property.Disable();

                return property;
            }
        }

        public override WellKnownType WellKnownType
        {
            get { return WellKnownType.Underline; }
        }

        public override Property EmptyClone()
        {
            return new UnderlineProperty();
        }

        public override XElement ToXML()
        {
            return new XElement("UnderlineProperty", base.IterXMLParts());
        }

        public static UnderlineProperty FromXML(XElement xml)
        {
            return new UnderlineProperty(xml);
        }

        private UnderlineProperty(XElement xml)
            : base(xml) { }
    }
}
