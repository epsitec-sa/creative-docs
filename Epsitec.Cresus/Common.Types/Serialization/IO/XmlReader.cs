//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization.IO
{
	public class XmlReader : AbstractReader
	{
		public XmlReader(System.Xml.XmlReader xml)
		{
			this.xml = xml;
		}

		public override void BeginStorageBundle(out int rootId, out int externalCount, out int typeCount, out int objectCount)
		{
			while (this.xml.Read ())
			{
				if (this.xml.NodeType == System.Xml.XmlNodeType.Element)
				{
					if ((this.xml.LocalName == "storage") &&
						(this.xml.NamespaceURI == this.nsStructure))
					{
						this.hasEmptyStorageElement = this.xml.IsEmptyElement;
						
						rootId = Context.ParseId (this.xml.GetAttribute ("root", this.nsStructure));
						
						externalCount = Context.ParseNum (this.xml.GetAttribute ("n_ext", this.nsStructure));
						typeCount     = Context.ParseNum (this.xml.GetAttribute ("n_typ", this.nsStructure));
						objectCount   = Context.ParseNum (this.xml.GetAttribute ("n_obj", this.nsStructure));
						
						return;
					}
					else
					{
						throw new System.FormatException (string.Format ("Element <{0}> not expected here; expected <storage>", this.xml.Name));
					}
				}
			}
			
			throw new System.FormatException ("Unexpected end of XML");
		}
		public override void EndStorageBundle()
		{
			if (this.hasEmptyStorageElement)
			{
				return;
			}
			
			while (this.xml.Read ())
			{
				if (this.xml.NodeType == System.Xml.XmlNodeType.Whitespace)
				{
					continue;
				}
				
				if ((this.xml.NodeType != System.Xml.XmlNodeType.EndElement) ||
					(this.xml.LocalName != "storage") ||
					(this.xml.NamespaceURI != this.nsStructure))
				{
					this.xml.Skip ();
				}
				else
				{
					break;
				}
			}
			
		}

		public override string ReadExternalReference()
		{
			while (this.xml.Read ())
			{
				if (this.xml.NodeType == System.Xml.XmlNodeType.Element)
				{
					if ((this.xml.LocalName == "external") &&
						(this.xml.NamespaceURI == this.nsStructure))
					{
						return this.xml.GetAttribute ("name", this.nsStructure);
					}
					else
					{
						throw new System.FormatException (string.Format ("Element <{0}> not expected here; expected <external>", this.xml.Name));
					}
				}
			}

			throw new System.FormatException ("Unexpected end of XML; expected <external>");
		}
		public override string ReadTypeDefinition(int id)
		{
			while (this.xml.Read ())
			{
				if (this.xml.NodeType == System.Xml.XmlNodeType.Element)
				{
					if ((this.xml.LocalName == "type") &&
						(this.xml.NamespaceURI == this.nsStructure))
					{
						string elementId = this.xml.GetAttribute ("id");
						string expectedId = Context.IdToString (id);

						if (elementId != expectedId)
						{
							throw new System.FormatException (string.Format ("Element <type> id={0}; expected id={1}", elementId, expectedId));
						}

						return this.xml.GetAttribute ("name", this.nsStructure);
					}
					else
					{
						throw new System.FormatException (string.Format ("Element <{0}> not expected here; expected <type>", this.xml.Name));
					}
				}
			}

			throw new System.FormatException ("Unexpected end of XML; expected <type>");
		}
		public override int ReadObjectDefinition(int id)
		{
			while (this.xml.Read ())
			{
				if (this.xml.NodeType == System.Xml.XmlNodeType.Element)
				{
					if ((this.xml.LocalName == "object") &&
						(this.xml.NamespaceURI == this.nsStructure))
					{
						string elementId = this.xml.GetAttribute ("id");
						string expectedId = Context.IdToString (id);

						if (elementId != expectedId)
						{
							throw new System.FormatException (string.Format ("Element <object> id={0}; expected id={1}", elementId, expectedId));
						}

						return Context.ParseId (this.xml.GetAttribute ("type", this.nsStructure));
					}
					else
					{
						throw new System.FormatException (string.Format ("Element <{0}> not expected here; expected <object>", this.xml.Name));
					}
				}
			}

			throw new System.FormatException ("Unexpected end of XML; expected <object>");
		}
		
		private System.Xml.XmlReader			xml;
		private string							nsStructure = "http://www.epsitec.ch/XNS/storage-structure-1";
		private string							nsFields	= "http://www.epsitec.ch/XNS/storage-fields-1";
		private bool hasEmptyStorageElement;
	}
}
