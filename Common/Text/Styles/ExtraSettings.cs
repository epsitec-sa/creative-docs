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
    /// Summary description for ExtraSettings.
    /// </summary>
    public sealed class ExtraSettings
        : AdditionalSettings,
            Common.Support.IXMLSerializable<ExtraSettings>
    {
        public ExtraSettings() { }

        public ExtraSettings(System.Collections.ICollection properties)
            : base(properties) { }

        public static bool CompareEqual(ExtraSettings a, ExtraSettings b)
        {
            //	Détermine si les deux réglages ont le même contenu. Utilise le
            //	plus d'indices possibles avant de passer à la comparaison.

            ////////////////////////////////////////////////////////////////////
            //	NB: contenu identique n'implique pas que le SettingsIndex est //
            //	identique !                                               //
            ////////////////////////////////////////////////////////////////////

            if (a == b)
            {
                return true;
            }
            if ((a == null) || (b == null))
            {
                return false;
            }
            if (a.GetType() != b.GetType())
            {
                return false;
            }
            if (a.GetContentsSignature() != b.GetContentsSignature())
            {
                return false;
            }

            //	Il y a de fortes chances que les deux objets aient le même
            //	contenu. Il faut donc opérer une comparaison des contenus.

            return a.CompareEqualContents(b);
        }

        public override void UpdateContentsSignature(IO.IChecksum checksum)
        {
            base.UpdateContentsSignature(checksum);
        }

        public override bool CompareEqualContents(object value)
        {
            return Styles.PropertyContainer.CompareEqualContents(
                this,
                value as Styles.ExtraSettings
            );
        }

        public override XElement ToXML()
        {
            return new XElement("ExtraSettings", base.IterXMLParts());
        }

        public static ExtraSettings FromXML(XElement xml)
        {
            return new ExtraSettings(xml);
        }

        private ExtraSettings(XElement xml)
            : base(xml) { }
    }
}
