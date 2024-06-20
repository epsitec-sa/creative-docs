//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD
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
