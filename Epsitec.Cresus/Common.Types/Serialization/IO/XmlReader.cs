//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
						(this.xml.NamespaceURI == Xml.StructureNamespace))
					{
						this.hasEmptyStorageElement = this.xml.IsEmptyElement;
						
						rootId = Context.ParseId (this.xml.GetAttribute ("root", Xml.StructureNamespace));
						
						externalCount = Context.ParseNum (this.xml.GetAttribute ("n_ext", Xml.StructureNamespace));
						typeCount     = Context.ParseNum (this.xml.GetAttribute ("n_typ", Xml.StructureNamespace));
						objectCount   = Context.ParseNum (this.xml.GetAttribute ("n_obj", Xml.StructureNamespace));
						
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
					(this.xml.NamespaceURI != Xml.StructureNamespace))
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
						(this.xml.NamespaceURI == Xml.StructureNamespace))
					{
						return this.xml.GetAttribute ("name", Xml.StructureNamespace);
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
						(this.xml.NamespaceURI == Xml.StructureNamespace))
					{
						string elementId = this.xml.GetAttribute ("id");
						string expectedId = Context.IdToString (id);

						if (elementId != expectedId)
						{
							throw new System.FormatException (string.Format ("Element <type> id={0}; expected id={1}", elementId, expectedId));
						}

						return this.xml.GetAttribute ("name", Xml.StructureNamespace);
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
						(this.xml.NamespaceURI == Xml.StructureNamespace))
					{
						string elementId = this.xml.GetAttribute ("id");
						string expectedId = Context.IdToString (id);

						if (elementId != expectedId)
						{
							throw new System.FormatException (string.Format ("Element <object> id={0}; expected id={1}", elementId, expectedId));
						}

						return Context.ParseId (this.xml.GetAttribute ("type", Xml.StructureNamespace));
					}
					else
					{
						throw new System.FormatException (string.Format ("Element <{0}> not expected here; expected <object>", this.xml.Name));
					}
				}
			}

			throw new System.FormatException ("Unexpected end of XML; expected <object>");
		}

		public override void BeginObject(int id, DependencyObject obj)
		{
			while (this.xml.Read ())
			{
				if (this.xml.NodeType == System.Xml.XmlNodeType.Element)
				{
					if ((this.xml.LocalName == "data") &&
						(this.xml.NamespaceURI == Xml.StructureNamespace))
					{
						string elementId = this.xml.GetAttribute ("id");
						string expectedId = Context.IdToString (id);

						if (elementId != expectedId)
						{
							throw new System.FormatException (string.Format ("Element <data> id={0}; expected id={1}", elementId, expectedId));
						}

						if (this.xml.MoveToFirstAttribute ())
						{
							return;
						}
						else
						{
							throw new System.FormatException ("Element <data> has no attributes");
						}
					}
					else
					{
						throw new System.FormatException (string.Format ("Element <{0}> not expected here; expected <data>", this.xml.Name));
					}
				}
			}

			throw new System.FormatException ("Unexpected end of XML; expected <object>");
		}
		public override bool ReadObjectFieldValue(DependencyObject obj, out string field, out string value)
		{
			while (this.xml.MoveToNextAttribute ())
			{
				if (this.xml.NamespaceURI == Xml.FieldsNamespace)
				{
					field = this.xml.LocalName;
					value = this.xml.Value;
					
					return true;
				}
			}

			field = null;
			value = null;

			return false;
		}
		public override void EndObject(int id, DependencyObject obj)
		{
			this.xml.MoveToElement ();
		}
		
		private System.Xml.XmlReader			xml;
		private bool hasEmptyStorageElement;
	}
}
