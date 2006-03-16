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

		public override void BeginStorageBundle()
		{
			this.xml.WriteStartElement ("s", "storage", this.nsStructure);
//			this.xml.WriteAttributeString ("xmlns", "s", null, this.nsStructure);
			this.xml.WriteAttributeString ("xmlns", "f", null, this.nsFields);
		}
		public override void EndStorageBundle()
		{
			this.xml.WriteEndElement ();
		}
		
		public override void WriteTypeDefinition(int id, string name)
		{
			this.xml.WriteStartElement ("type", this.nsStructure);
			this.xml.WriteAttributeString ("id", XmlSupport.IdToString (id));
			this.xml.WriteAttributeString ("name", this.nsStructure, name);
			this.xml.WriteEndElement ();
		}

		public override void WriteObjectDefinition(int id, int typeId)
		{
			this.xml.WriteStartElement ("object", this.nsStructure);
			this.xml.WriteAttributeString ("id", XmlSupport.IdToString (id));
			this.xml.WriteAttributeString ("type", this.nsStructure, XmlSupport.IdToString (typeId));
			this.xml.WriteEndElement ();
		}

		public override void BeginObject(int id, DependencyObject obj)
		{
			this.xml.WriteStartElement ("data", this.nsStructure);
			this.xml.WriteAttributeString ("obj", this.nsStructure, XmlSupport.IdToString (id));
		}
		public override void WriteObjectFieldReference(DependencyObject obj, string name, int id)
		{
			this.xml.WriteAttributeString ("f", name, this.nsFields, XmlSupport.IdToString (id));
		}
		public override void WriteObjectFieldReferenceList(DependencyObject obj, string name, IList<int> ids)
		{
			this.xml.WriteStartElement ("f", name, this.nsFields);
			
			foreach (int id in ids)
			{
				this.xml.WriteStartElement ("ref", this.nsStructure);
				this.xml.WriteAttributeString ("oid", this.nsStructure, XmlSupport.IdToString (id));
				this.xml.WriteEndElement ();
			}
			
			this.xml.WriteEndElement ();
		}
		public override void WriteObjectFieldValue(DependencyObject obj, string name, string value)
		{
			this.xml.WriteAttributeString ("f", name, this.nsFields, value);
		}
		public override void EndObject(int id, DependencyObject obj)
		{
			this.xml.WriteEndElement ();
		}

		private System.Xml.XmlWriter			xml;
		private string							nsStructure = "http://www.epsitec.ch/XNS/storage-structure-1";
		private string							nsFields	= "http://www.epsitec.ch/XNS/storage-fields-1";
	}
}
