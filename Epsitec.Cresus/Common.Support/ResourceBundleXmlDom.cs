//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	using System.Collections;
	
	/// <summary>
	/// Implémentation d'un ResourceBundle basé sur un stockage interne de
	/// l'information sous forme XML DOM.
	/// </summary>
	public class ResourceBundleXmlDom : ResourceBundle
	{
		public ResourceBundleXmlDom(string name) : base(name)
		{
			this.fields = new Field[0];
		}
		
		public ResourceBundleXmlDom(ResourceBundleXmlDom parent, string name, System.Xml.XmlNode xmlroot) : this(name)
		{
			this.Compile (xmlroot, parent.prefix, parent.level, parent.depth + 1);
			this.Merge ();
		}
		
		public Field[]						Fields
		{
			get
			{
				return this.fields.Clone () as Field[];
			}
		}
		
		public override int					CountFields
		{
			get
			{
				return this.fields.Length;
			}
		}
		
		public override string[]			FieldNames
		{
			get
			{
				string[] names = new string[this.fields.Length];
				
				for (int i = 0; i < this.fields.Length; i++)
				{
					names[i] = this.fields[i].Name;
				}
				
				return names;
			}
		}
		
		public override object				this[string name]
		{
			get
			{
				Field field = this.GetField (name);
				
				if (field != null)
				{
					return field.Data;
				}
				
				return null;
			}
		}
		
		public override ResourceFieldType GetFieldType(string name)
		{
			Field field = this.GetField (name);
			
			if (field != null)
			{
				return field.Type;
			}
			
			return ResourceFieldType.None;
		}
		
		public Field GetField(string name)
		{
			for (int i = 0; i < this.fields.Length; i++)
			{
				if (this.fields[i].Name == name)
				{
					return this.fields[i];
				}
			}
			
			return null;
		}
		
		protected string GetAttributeValue(System.Xml.XmlNode node, string name)
		{
			System.Xml.XmlAttribute attr = node.Attributes[name];
			
			if (attr != null)
			{
				return attr.Value;
			}
			
			return null;
		}
		
		public override bool Contains(string name)
		{
			return this.GetField (name) != null;
		}
		
		public override System.Collections.IList GetFieldBundleList(string name)
		{
			ArrayList list = this[name] as ArrayList;
			ArrayList copy = (list == null) ? null : new ArrayList ();
			
			if (list != null)
			{
				foreach (Field field in list)
				{
					copy.Add (field.Data as ResourceBundle);
				}
			}
			
			return copy;
		}
		
		public override ResourceBundle GetFieldBundleListItem(string name, int index)
		{
			ArrayList list = this[name] as ArrayList;
			
			if (list != null)
			{
				Field field = list[index] as Field;
				return field.Data as ResourceBundle;
			}
			
			return null;
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
				
				this.Compile (xmldoc.DocumentElement, default_prefix, level, recursion);
				this.Merge ();
				
				this.xmldoc = xmldoc;
			}
		}
		
		public void Compile(System.Xml.XmlNode xmlroot, string default_prefix, ResourceLevel level, int recursion)
		{
			if (recursion > ResourceBundle.MaxRecursion)
			{
				throw new ResourceException (string.Format ("Bundle is too complex, giving up."));
			}
			
			this.depth   = recursion;
			this.prefix  = default_prefix;
			this.level   = level;
			
			ArrayList list = new ArrayList ();
			list.AddRange (this.fields);
			
			this.CreateFieldList (xmlroot, list, true);
			
			this.fields  = new Field[list.Count];
			this.xmlroot = xmlroot;
			
			list.CopyTo (this.fields);
			
			this.compile_count++;
		}
		
		protected void Merge()
		{
			ArrayList list = new ArrayList ();
			Hashtable hash = new Hashtable ();
			
			for (int i = 0; i < this.fields.Length; i++)
			{
				string name = this.fields[i].Name;
				
				if (hash.Contains (name))
				{
					//	Le champ est déjà connu: on remplace simplement l'ancienne occurrence
					//	dans la liste.
					
					int index = (int) hash[name];
					list[index] = this.fields[i];
				}
				else
				{
					//	Le champ n'est pas connu: on ajoute le champ en fin de liste et on prend
					//	note de son index, pour pouvoir y accéder rapidement par la suite.
					
					hash[name] = list.Add (this.fields[i]);
				}
			}
			
			this.fields = new Field[list.Count];
			list.CopyTo (this.fields);
		}
		
		protected void CreateFieldList(System.Xml.XmlNode xmlroot, ArrayList list, bool unpack_bundle_ref)
		{
			foreach (System.Xml.XmlNode node in xmlroot.ChildNodes)
			{
				if (node.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (node.Name == "ref")
					{
						//	Cas particulier: on inclut des champs en provenance d'un bundle
						//	référencé par un tag <ref>.
						
						ResourceBundleXmlDom bundle = this.ResolveRefBundle (node);
						
						if (unpack_bundle_ref)
						{
							Field[] fields = bundle.Fields;
							list.AddRange (fields);
						}
						else
						{
							list.Add (new Field (this, bundle));
						}
					}
					else
					{
						//	Tous les autres tags sont stockés sous la forme d'un "field" qui
						//	ne sera analysé que lorsque le besoin s'en fera sentir.
						
						list.Add (new Field (this, node));
					}
				}
			}
		}
		
		protected ResourceBundleXmlDom ResolveRefBundle(System.Xml.XmlNode node)
		{
			string ref_target  = this.GetAttributeValue (node, "target");
			string ref_type    = this.GetAttributeValue (node, "type");
			string full_target = this.GetTargetSpecification (ref_target);
			
			string target_bundle;
			string target_field;
			
			ResourceBundle.SplitTarget (full_target, out target_bundle, out target_field);
			
			if (target_field != null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> does not reference a bundle.", ref_target));
			}
			if (ref_type != null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> specifies type='{1}'.", ref_target, ref_type));
			}
			
			ResourceBundleXmlDom bundle = Resources.GetBundle (target_bundle, this.level, this.depth + 1) as ResourceBundleXmlDom;
			
			if (bundle == null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> could not be resolved. Missing bundle.", ref_target));
			}
			
			return bundle;
		}
		
		protected Field ResolveRefField(System.Xml.XmlNode node)
		{
			string ref_target  = this.GetAttributeValue (node, "target");
			string ref_type    = this.GetAttributeValue (node, "type");
			string full_target = this.GetTargetSpecification (ref_target);
			
			string target_bundle;
			string target_field;
			
			ResourceBundle.SplitTarget (full_target, out target_bundle, out target_field);
			
			if (target_field == null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> does not reference a field.", ref_target));
			}
			if (ref_type != null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> specifies type='{1}'.", ref_target, ref_type));
			}
			
			ResourceBundleXmlDom bundle = Resources.GetBundle (target_bundle, this.level, this.depth + 1) as ResourceBundleXmlDom;
			
			if (bundle == null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> could not be resolved. Missing bundle.", ref_target));
			}
			
			Field field = bundle.GetField (target_field);
			
			if (field == null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> could not be resolved. Missing field.", ref_target));
			}
			
			return field;
		}
		
		protected byte[] ResolveRefBinary(System.Xml.XmlNode node)
		{
			string ref_target  = this.GetAttributeValue (node, "target");
			string full_target = this.GetTargetSpecification (ref_target);
			
			string target_bundle;
			string target_field;
			
			ResourceBundle.SplitTarget (full_target, out target_bundle, out target_field);
			
			if ((target_bundle != null) &&
				(target_field  == null))
			{
				byte[] data = Resources.GetBinaryData (target_bundle, level);
				
				if (data == null)
				{
					throw new ResourceException (string.Format ("Binary target '{0}' cannot be resolved.", ref_target));
				}
				
				return data;
			}
			
			throw new ResourceException (string.Format ("Illegal reference to binary target '{0}'.", ref_target));
		}		
		
		protected string GetTargetSpecification(string target)
		{
			if (target == null)
			{
				throw new ResourceException (string.Format ("Reference has no target."));
			}
			
			if (Resources.ExtractPrefix (target) == null)
			{
				if (this.prefix == null)
				{
					throw new ResourceException (string.Format ("No default prefix specified, target '{0}' cannot be resolved.", target));
				}
				
				target = this.prefix + ":" + target;
			}
			
			return target;
		}
		
		
		public class Field
		{
			public Field(ResourceBundleXmlDom parent, System.Xml.XmlNode xml)
			{
				this.parent = parent;
				this.name   = parent.GetAttributeValue (xml, "name");
				this.xml    = xml;
			}
			
			public Field(ResourceBundleXmlDom parent, ResourceBundleXmlDom bundle)
			{
				this.parent = parent;
				this.name   = bundle.Name;
				this.data   = bundle;
				this.type   = ResourceFieldType.Bundle;
			}
			
			
			public string					Name
			{
				get { return this.name; }
				set { this.name = value; }
			}
			
			public ResourceFieldType		Type
			{
				get
				{
					this.Compile ();
					return this.type;
				}
			}
			
			public object					Data
			{
				get
				{
					this.Compile ();
					return this.data;
				}
			}
			
			public System.Xml.XmlNode		Xml
			{
				get { return this.xml; }
			}
			
			
			protected void Compile()
			{
				if (this.type == ResourceFieldType.None)
				{
					switch (this.xml.Name)
					{
						case "bundle":
							this.CompileBundle ();
							break;
						
						case "data":
							this.CompileData ();
							break;

						case "binary":
							this.CompileBinary ();
							break;
						
						case "list":
							this.CompileList ();
							break;
						
						default:
							throw new ResourceException (string.Format ("Unsupported tag <{0}> cannot be compiled.", this.xml.Name));
					}
					
					System.Diagnostics.Debug.Assert (this.type != ResourceFieldType.None);
				}
			}
			
			protected void CompileBundle()
			{
				this.data = new ResourceBundleXmlDom (this.parent, this.parent.Name + "#" + this.Name, this.xml);
				this.type = ResourceFieldType.Bundle;
			}
			
			protected void CompileData()
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				foreach (System.Xml.XmlNode node in this.xml.ChildNodes)
				{
					switch (node.NodeType)
					{
						case System.Xml.XmlNodeType.CDATA:
						case System.Xml.XmlNodeType.Text:
							buffer.Append (node.Value);
							break;
						
						case System.Xml.XmlNodeType.Element:
							this.CompileDataElement (node, buffer);
							break;
					}
				}
				
				this.type = ResourceFieldType.Data;
				this.data = buffer.ToString ();
			}
			
			protected void CompileBinary()
			{
				byte[] data = this.parent.ResolveRefBinary (this.xml);
				
				this.type = ResourceFieldType.Binary;
				this.data = data;
			}
			
			protected void CompileDataElement(System.Xml.XmlNode node, System.Text.StringBuilder buffer)
			{
				switch (node.Name)
				{
					case "xml":
						buffer.Append (node.InnerXml);
						break;
					case "ref":
						this.CompileDataReference(node, buffer);
						break;
				}
			}
			
			protected void CompileDataReference(System.Xml.XmlNode node, System.Text.StringBuilder buffer)
			{
				Field field = this.parent.ResolveRefField (node);
				
				if (field.Type != ResourceFieldType.Data)
				{
					string target = this.parent.GetAttributeValue (node, "target");
					throw new ResourceException (string.Format ("<ref target='{0}'/> resolution is not <data> compatible.", target));
				}
				
				string data = field.Data as string;
				buffer.Append (data);
			}
			
			protected void CompileList()
			{
				ArrayList list = new ArrayList ();
				
				this.parent.CreateFieldList (this.xml, list, false);
				
				//	Les champs stockés dans la liste ont des noms qui sont du type 'nom[n]' où 'nom' est
				//	le nom donné à la liste, et 'n' l'index (à partir de 0..)
				
				for (int i = 0; i < list.Count; i++)
				{
					Field field = list[i] as Field;
					field.Name  = string.Format ("{0}[{1}]", this.Name, i);
				}
				
				this.data = list;
				this.type = ResourceFieldType.List;
			}
			
			
			protected ResourceBundleXmlDom	parent;
			protected string				name;
			protected System.Xml.XmlNode	xml;
			protected object				data = null;
			protected ResourceFieldType		type = ResourceFieldType.None;
		}
		
		
		
		protected System.Xml.XmlDocument	xmldoc;
		protected System.Xml.XmlNode		xmlroot;
		protected int						depth;
		protected int						compile_count;
		protected string					prefix;
		protected ResourceLevel				level;
		protected Field[]					fields;
	}
}
