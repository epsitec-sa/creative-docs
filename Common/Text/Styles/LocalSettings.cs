//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD
using System.Xml.Linq;

namespace Epsitec.Common.Text.Styles
{
    /// <summary>
    /// Summary description for LocalSettings.
    /// </summary>
    public sealed class LocalSettings
        : AdditionalSettings,
            Common.Support.IXMLSerializable<LocalSettings>
    {
        public LocalSettings() { }

        public LocalSettings(System.Collections.ICollection properties)
            : base(properties) { }

        public static bool CompareEqual(LocalSettings a, LocalSettings b)
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
                value as Styles.LocalSettings
            );
        }

        public override XElement ToXML()
        {
            return new XElement("LocalSettings", base.IterXMLParts());
        }

        public static LocalSettings FromXML(XElement xml)
        {
            return new LocalSettings(xml);
        }

        private LocalSettings(XElement xml)
            : base(xml) { }
    }
}
