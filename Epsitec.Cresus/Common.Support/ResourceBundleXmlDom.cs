//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	/// <summary>
	/// Implémentation d'un ResourceBundle basé sur un stockage interne de
	/// l'information sous forme XML DOM.
	/// </summary>
	public class ResourceBundleXmlDom : ResourceBundle
	{
		public ResourceBundleXmlDom(string name) : base(name)
		{
		}
		
		public ResourceBundleXmlDom(string name, System.Xml.XmlNode xmlroot) : this(name)
		{
			this.xmldoc  = null;
			this.xmlroot = xmlroot;
		}
		
		
		public override int					CountFields
		{
			get
			{
				if (this.xmlroot == null)
				{
					return 0;
				}
				
				return this.xmlroot.ChildNodes.Count;
			}
		}
		
		public override string[]			FieldNames
		{
			get
			{
				int      count = this.CountFields;
				string[] names = new string[count];
				
				for (int i = 0; i < count; i++)
				{
					names[i] = ResourceBundleXmlDom.GetNodeNameAttrValue (this.xmlroot.ChildNodes[i]);
				}
				
				return names;
			}
		}
		
		public override object				this[string name]
		{
			get
			{
				if (this.xmlroot != null)
				{
					for (int i = 0; i < this.xmlroot.ChildNodes.Count; i++)
					{
						System.Xml.XmlNode      node = this.xmlroot.ChildNodes[i];
						string             node_name = ResourceBundleXmlDom.GetNodeNameAttrValue (node);
						
						if (node_name == name)
						{
							return CreateObjectFromNode (node, node_name);
						}
					}
				}
				
				return null;
			}
		}
		
		protected static string GetNodeNameAttrValue(System.Xml.XmlNode node)
		{
			System.Xml.XmlAttribute attr = node.Attributes["name"];
			
			if (attr != null)
			{
				return attr.Value;
			}
			
			return null;
		}
		
		protected object CreateObjectFromNode(System.Xml.XmlNode node, string node_name)
		{
			string node_tag = node.Name;
			
			if (node_tag == "field")
			{
				return node.InnerText;
			}
			if (node_tag == "bundle")
			{
				return new ResourceBundleXmlDom (this.name + "#" + node_name, node);
			}
			
			return null;
		}
		
		public override bool Contains(string field)
		{
			return false;
		}

		
		
		public override void Compile(byte[] data, string default_prefix, ResourceLevel level, int recursion)
		{
			//	La compilation des données part du principe que le bundle XML est "well formed",
			//	c'est-à-dire qu'il comprend un seul bloc à la racine (<bundle>..</bundle>), et
			//	que son contenu est valide (l'en-tête <?xml ...?> n'est pas requis).
			
			if (data != null)
			{
				System.IO.MemoryStream stream = new System.IO.MemoryStream (data, false);
				System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument ();
				
				xmldoc.PreserveWhitespace = false;
				
				try
				{
					xmldoc.Load (stream);
				}
				finally
				{
					stream.Close ();
				}
				
				this.xmldoc  = xmldoc;
				this.xmlroot = xmldoc.DocumentElement;
			}
		}
		
		
		protected System.Xml.XmlDocument	xmldoc;
		protected System.Xml.XmlNode		xmlroot;
	}
}
