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
		
		public override void BeginStorageBundle(out int rootId)
		{
			while (this.xml.Read ())
			{
				if (this.xml.NodeType == System.Xml.XmlNodeType.Element)
				{
					if ((this.xml.LocalName == "storage") &&
						(this.xml.NamespaceURI == this.nsStructure))
					{
						this.hasEmptyStorageElement = this.xml.IsEmptyElement;
						
						string id = this.xml.GetAttribute ("s:root");
						rootId = Context.ParseId (id);
						
						return;
					}
					else
					{
						throw new System.FormatException (string.Format ("Element '{0}' not expected here", this.xml.Name));
					}
				}
			}
			
			rootId = 0;
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
		
		private System.Xml.XmlReader			xml;
		private string							nsStructure = "http://www.epsitec.ch/XNS/storage-structure-1";
		private string							nsFields	= "http://www.epsitec.ch/XNS/storage-fields-1";
		private bool hasEmptyStorageElement;
	}
}
