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
