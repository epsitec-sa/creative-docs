//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization.IO
{
	public class XmlWriter : AbstractWriter
	{
		public XmlWriter(System.Xml.XmlWriter xml)
		{
			this.xml = xml;
		}
		
		public override void WriteTypeDefinition(int id, string name)
		{
			this.xml.WriteStartElement ("type");
			this.xml.WriteAttributeString ("id", id.ToString (System.Globalization.CultureInfo.InvariantCulture));
			this.xml.WriteAttributeString ("name", name);
			this.xml.WriteEndElement ();
		}

		public override void WriteObjectDefinition(int id, int typeId)
		{
			this.xml.WriteStartElement ("object");
			this.xml.WriteAttributeString ("id", id.ToString (System.Globalization.CultureInfo.InvariantCulture));
			this.xml.WriteAttributeString ("typeId", typeId.ToString (System.Globalization.CultureInfo.InvariantCulture));
			this.xml.WriteEndElement ();
		}

		public override void WriteObjectData(int id, DependencyObject obj)
		{
			
		}

		private System.Xml.XmlWriter xml;
	}
}
