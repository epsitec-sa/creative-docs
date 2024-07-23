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

namespace Epsitec.Common.Text.Styles
{
    /// <summary>
    /// La classe CoreSettings permet de décrire des propriétés fondamentales telles
    /// que défition de fonte et de paragraphe, plus quelques autres détails.
    /// </summary>
    public sealed class CoreSettings : BaseSettings, Common.Support.IXMLSerializable<CoreSettings>
    {
        public CoreSettings() { }

        public CoreSettings(System.Collections.ICollection properties)
            : base(properties) { }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            base.UpdateContentsSignature(checksum);
        }

        public override bool CompareEqualContents(object value)
        {
            return Styles.PropertyContainer.CompareEqualContents(
                this,
                value as Styles.CoreSettings
            );
        }

        public override XElement ToXML()
        {
            return new XElement("CoreSettings", base.IterXMLParts());
        }

        public static CoreSettings FromXML(XElement xml)
        {
            return new CoreSettings(xml);
        }

        private CoreSettings(XElement xml)
            : base(xml) { }
    }
}
