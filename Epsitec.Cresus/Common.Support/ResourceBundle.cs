//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 21/01/2004

namespace Epsitec.Common.Support
{
	using System.Globalization;
	
	using ArrayList = System.Collections.ArrayList;
	using Hashtable = System.Collections.Hashtable;
	
	/// <summary>
	/// Impl�mentation d'un ResourceBundle bas� sur un stockage interne de
	/// l'information sous forme XML DOM.
	/// </summary>
	public class ResourceBundle
	{
		public static ResourceBundle Create()
		{
			return ResourceBundle.Create (null, null, ResourceLevel.Merged, null, 0);
		}
		
		public static ResourceBundle Create(ResourceLevel level)
		{
			return ResourceBundle.Create (null, null, level, null, 0);
		}
		
		public static ResourceBundle Create(string name)
		{
			return ResourceBundle.Create (name, null, ResourceLevel.Merged, null, 0);
		}
		
		public static ResourceBundle Create(string name, ResourceLevel level)
		{
			return ResourceBundle.Create (name, null, level, null, 0);
		}
		
		public static ResourceBundle Create(string name, string prefix, ResourceLevel level, int recursion)
		{
			return ResourceBundle.Create (name, prefix, level, null, recursion);
		}
		
		public static ResourceBundle Create(string name, string prefix, ResourceLevel level, CultureInfo culture)
		{
			return ResourceBundle.Create (name, prefix, level, culture, 0);
		}
		
		public static ResourceBundle Create(string name, string prefix, ResourceLevel level, CultureInfo culture, int recursion)
		{
			ResourceBundle bundle = new ResourceBundle (name);
			
			bundle.prefix  = prefix;
			bundle.level   = level;
			bundle.depth   = recursion;
			bundle.culture = culture;
			
			return bundle;
		}
		
		
		protected ResourceBundle(string name)
		{
			this.DefineName (name);
			
			this.fields = new Field[0];
		}
		
		protected ResourceBundle(ResourceBundle parent, string name, System.Xml.XmlNode xmlroot) : this(name)
		{
			this.prefix = parent.prefix;
			this.level  = parent.level;
			this.depth  = parent.depth + 1;
			
			this.Compile (xmlroot);
		}
		
		
		public string						Name
		{
			get
			{
				return this.name == null ? "" : this.name;
			}
		}
		
		public string						Type
		{
			get
			{
				return this.type == null ? "" : this.type;
			}
		}
		
		public string						About
		{
			get
			{
				return this.about;
			}
		}
		
		public ResourceLevel				ResourceLevel
		{
			get
			{
				return this.level;
			}
		}
		
		public CultureInfo					Culture
		{
			get
			{
				return this.culture;
			}
		}
		
		public string						PrefixedName
		{
			get
			{
				return this.prefix + ":" + this.name;
			}
		}
		
		public bool							IsEmpty
		{
			get
			{
				return this.CountFields == 0;
			}
		}
		
		public int							CountFields
		{
			get
			{
				return this.fields.Length;
			}
		}
		
		public string[]						FieldNames
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
		
		public const int					MaxRecursion = 50;
		
		public System.Xml.XmlDocument		XmlDocument
		{
			get
			{
				if (this.xmlroot == null)
				{
					System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
					doc.Load (new System.IO.StringReader ("<bundle></bundle>"));
					this.xmlroot = doc.DocumentElement;
				}
				
				return this.xmlroot.OwnerDocument;
			}
		}
		
		public bool							RefInclusionEnabled
		{
			get { return this.ref_inclusion; }
			set { this.ref_inclusion = value; }
		}
		
		public bool							AutoMergeEnabled
		{
			get { return this.auto_merge; }
			set { this.auto_merge = value; }
		}
		
		
		public Field						this[string name]
		{
			get
			{
				for (int i = 0; i < this.fields.Length; i++)
				{
					if (this.fields[i].Name == name)
					{
						return this.fields[i];
					}
				}
				
				return Field.Empty;
			}
		}
		
		public Field						this[int index]
		{
			get
			{
				return this.fields[index];
			}
		}
		
		
		public void DefineName(string name)
		{
			if (name != null)
			{
				if (! RegexFactory.ResourceName.IsMatch (name))
				{
					throw new ResourceException (string.Format ("Name '{0}' is not a valid bundle name.", name));
				}
			}
		
			this.name = name;
		}
		
		public void DefineType(string type)
		{
			this.type = type;
		}
		
		public void DefineAbout(string about)
		{
			this.about = about;
		}
		
		public bool Contains(string name)
		{
			return this[name].IsEmpty == false;
		}
		
		
		public void Insert(int index, Field field)
		{
			int len = this.fields.Length;
			Field[] temp = new Field[len + 1];
			
			for (int i = 0; i < index; i++)
			{
				temp[i] = this.fields[i];
			}
			
			temp[index] = field;
			
			for (int i = index; i < len; i++)
			{
				temp[i+1] = this.fields[i];
			}
			
			this.fields = temp;
			
			this.OnFieldsChanged ();
		}
		
		public void Remove(int index)
		{
			int len = this.fields.Length;
			Field[] temp = new Field[len - 1];
			
			for (int i = 0; i < index; i++)
			{
				temp[i] = this.fields[i];
			}
			
			for (int i = index+1; i < len; i++)
			{
				temp[i-1] = this.fields[i];
			}
			
			this.fields = temp;
			
			this.OnFieldsChanged ();
		}
		
		public int Add(Field field)
		{
			int index = this.fields.Length;
			
			Field[] temp = new Field[index + 1];
			
			this.fields.CopyTo (temp, 0);
			temp[index] = field;
			this.fields = temp;
			
			this.OnFieldsChanged ();
			
			return index;
		}
		
		public int AddRange(Field[] fields)
		{
			int index = this.fields.Length;
			
			Field[] temp = new Field[index + fields.Length];
			
			this.fields.CopyTo (temp, 0);
			fields.CopyTo (temp, index);
			this.fields = temp;
			
			this.OnFieldsChanged ();
			
			return index;
		}
		
		
		public void Merge()
		{
			ArrayList list = new ArrayList ();
			Hashtable hash = new Hashtable ();
			
			for (int i = 0; i < this.fields.Length; i++)
			{
				string name = this.fields[i].Name;
				
				if (name == null)
				{
					//	En principe, tous les champs doivent avoir un nom valide.
					
					if (this.ref_inclusion == false)
					{
						//	Cas particulier: si l'utilisateur a d�sactiv� l'inclusion des <ref>
						//	alors il se peut qu'un champ soit en fait un <ref> sans nom, auquel
						//	cas on doit le copier tel quel, sans faire de merge.
						
						list.Add (this.fields[i]);
					}
					else
					{
						throw new ResourceException (string.Format ("Field has no name. XML: {0}.", this.fields[i].Xml.OuterXml));
					}
				}
				else if (hash.Contains (name))
				{
					//	Le champ est d�j� connu: on remplace simplement l'ancienne occurrence
					//	dans la liste.
					
					int index = (int) hash[name];
					list[index] = this.fields[i];
				}
				else
				{
					//	Le champ n'est pas connu: on ajoute le champ en fin de liste et on prend
					//	note de son index, pour pouvoir y acc�der rapidement par la suite.
					
					hash[name] = list.Add (this.fields[i]);
				}
			}
			
			this.fields = new Field[list.Count];
			list.CopyTo (this.fields);
		}
		
		
		public void Compile(byte[] data)
		{
			//	La compilation des donn�es part du principe que le bundle XML est "well formed",
			//	c'est-�-dire qu'il comprend un seul bloc � la racine (<bundle>..</bundle>), et
			//	que son contenu est valide (l'en-t�te <?xml ...?> n'est pas requis).
			
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
				
				this.Compile (xmldoc.DocumentElement);
			}
		}
		
		public void Compile(System.Xml.XmlNode xmlroot)
		{
			if (this.depth > ResourceBundle.MaxRecursion)
			{
				throw new ResourceException (string.Format ("Bundle is too complex, giving up."));
			}
			
			if (xmlroot.Name != "bundle")
			{
				throw new ResourceException (string.Format ("Bundle does not start with <bundle> tag (<{0}> is an unsupported root).", xmlroot.Name));
			}
			
			string name_attr  = this.GetAttributeValue (xmlroot, "name");
			string type_attr  = this.GetAttributeValue (xmlroot, "type");
			string about_attr = this.GetAttributeValue (xmlroot, "about");
			
			if (name_attr != null)
			{
				if ((this.name == null) ||
					(this.name == ""))
				{
					this.name = name_attr;
				}
			}
			if (type_attr != null)
			{
				this.type = type_attr;
			}
			if (about_attr != null)
			{
				this.about = about_attr;
			}
			
			ArrayList list = new ArrayList ();
			list.AddRange (this.fields);
			
			this.CreateFieldList (xmlroot, list, true);
			
			this.fields  = new Field[list.Count];
			this.xmlroot = xmlroot;
			
			list.CopyTo (this.fields);
			
			this.compile_count++;
			
			if (this.auto_merge)
			{
				this.Merge ();
			}
		}
		
		
		public byte[] CreateXmlAsData()
		{
			byte[] data;
			
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream ())
			{
				System.Xml.XmlDocument xmldoc = this.CreateXmlDocument (false);
				xmldoc.Save (stream);
				data = stream.ToArray ();
			}
			
			return data;
		}
		
		public System.Xml.XmlDocument CreateXmlDocument(bool include_declaration)
		{
			System.Xml.XmlDocument xmldoc  = new System.Xml.XmlDocument ();
			System.Xml.XmlNode     xmlnode = this.CreateXmlNode (xmldoc);
			
			if (include_declaration)
			{
				xmldoc.AppendChild (xmldoc.CreateXmlDeclaration ("1.0", "utf-8", null));
			}
			
			xmldoc.AppendChild (xmlnode);
			
			return xmldoc;
		}
		
		public System.Xml.XmlNode CreateXmlNode(System.Xml.XmlDocument xmldoc)
		{
			System.Xml.XmlElement   bundle_node = xmldoc.CreateElement ("bundle");
			System.Xml.XmlAttribute name_attr   = xmldoc.CreateAttribute ("name");
			System.Xml.XmlAttribute type_attr   = xmldoc.CreateAttribute ("type");
			System.Xml.XmlAttribute about_attr  = xmldoc.CreateAttribute ("about");
			
			name_attr.Value  = this.name;
			type_attr.Value  = this.type;
			about_attr.Value = this.about;
			
			if (name_attr.Value != "")
			{
				bundle_node.Attributes.Append (name_attr);
			}
			if (type_attr.Value != "")
			{
				bundle_node.Attributes.Append (type_attr);
			}
			if (about_attr.Value != "")
			{
				bundle_node.Attributes.Append (about_attr);
			}
			
			for (int i = 0; i < this.fields.Length; i++)
			{
				Field  field  = this.fields[i];
				string source = field.Xml.OuterXml;
				
				System.Xml.XmlDocumentFragment fragment =  xmldoc.CreateDocumentFragment ();
				
				fragment.InnerXml = source;
				
				bundle_node.AppendChild (fragment);
			}
			
			return bundle_node;
		}
		
		
		public Field CreateEmptyField(ResourceFieldType type)
		{
			if (type != ResourceFieldType.Data)
			{
				throw new System.NotImplementedException (string.Format ("{0} support not implemented.", type));
			}
			
			System.Xml.XmlDocument  doc  = this.XmlDocument;
			System.Xml.XmlElement   elem = doc.CreateElement ("data");
			System.Xml.XmlAttribute attr = doc.CreateAttribute ("name");
			
			elem.Attributes.Append (attr);
			attr.Value = "?";
			
			Field field = new Field (this, elem);
			
			return field;
		}
		
		
		public static bool SplitTarget(string target, out string target_bundle, out string target_field)
		{
			int pos = target.IndexOf ("#");
			
			target_bundle = target;
			target_field  = null;
			
			if (pos >= 0)
			{
				target_bundle = target.Substring (0, pos);
				target_field  = target.Substring (pos+1);
				
				return true;
			}
			
			return false;
		}
		
		public static string ExtractName(string sort_name)
		{
			int pos = sort_name.IndexOf ('/');
			
			if (pos < 0)
			{
				throw new ResourceException (string.Format ("'{0}' is an invalid sort name", sort_name));
			}
			
			return sort_name.Substring (pos+1);
		}
		
		
		protected void CreateFieldList(System.Xml.XmlNode xmlroot, ArrayList list, bool unpack_bundle_ref)
		{
			foreach (System.Xml.XmlNode node in xmlroot.ChildNodes)
			{
				if (node.NodeType == System.Xml.XmlNodeType.Element)
				{
					if ((node.Name == "ref") && (this.ref_inclusion))
					{
						//	Cas particulier: on inclut des champs en provenance d'un bundle
						//	r�f�renc� par un tag <ref>.
						
						ResourceBundle bundle = this.ResolveRefBundle (node);
						
						if (unpack_bundle_ref)
						{
							list.AddRange (bundle.fields);
						}
						else
						{
							list.Add (new Field (this, bundle));
						}
					}
					else
					{
						//	Tous les autres tags sont stock�s sous la forme d'un "field" qui
						//	ne sera analys� que lorsque le besoin s'en fera sentir.
						
						list.Add (new Field (this, node));
					}
				}
			}
		}
		
		protected ResourceBundle ResolveRefBundle(System.Xml.XmlNode node)
		{
			string ref_target  = this.GetAttributeValue (node, "target");
			string ref_type    = this.GetAttributeValue (node, "type");
			string full_target = this.GetTargetSpecification (ref_target);
			
			string target_bundle;
			string target_field;
			
			ResourceBundle.SplitTarget (full_target, out target_bundle, out target_field);
			
			if (target_field != null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> does not reference a bundle. XML: {1}.", ref_target, node.OuterXml));
			}
			if (ref_type != null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> specifies type='{1}'. XML: {2}.", ref_target, ref_type, node.OuterXml));
			}
			
			ResourceBundle bundle = Resources.GetBundle (target_bundle, this.level, this.depth + 1) as ResourceBundle;
			
			if (bundle == null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> could not be resolved. Missing bundle. XML: {1}.", ref_target, node.OuterXml));
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
				throw new ResourceException (string.Format ("<ref target='{0}'/> does not reference a field. XML: {1}.", ref_target, node.OuterXml));
			}
			if (ref_type != null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> specifies type='{1}'. XML: {2}.", ref_target, ref_type, node.OuterXml));
			}
			
			ResourceBundle bundle = Resources.GetBundle (target_bundle, this.level, this.depth + 1) as ResourceBundle;
			
			if (bundle == null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> could not be resolved. Missing bundle. XML: {1}.", ref_target, node.OuterXml));
			}
			
			Field field = bundle[target_field];
			
			if (field.IsEmpty)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> could not be resolved. Missing field. XML: {1}.", ref_target, node.OuterXml));
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
					throw new ResourceException (string.Format ("Binary target '{0}' cannot be resolved. XML: {1}.", ref_target, node.OuterXml));
				}
				
				return data;
			}
			
			throw new ResourceException (string.Format ("Illegal reference to binary target '{0}'. XML: {1}.", ref_target, node.OuterXml));
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
		
		protected string GetAttributeValue(System.Xml.XmlNode node, string name)
		{
			System.Xml.XmlAttribute attr = node.Attributes[name];
			
			if (attr != null)
			{
				return attr.Value;
			}
			
			return null;
		}
		
		protected void SetAttributeValue(System.Xml.XmlNode node, string name, string value)
		{
			if (node == null)
			{
				throw new ResourceException ("Invalid node specified.");
			}
			
			if (node.Attributes[name] == null)
			{
				node.Attributes.Append (this.XmlDocument.CreateAttribute (name));
			}
			
			node.Attributes[name].Value = value;
			
			this.OnFieldsChanged ();
		}
		
		
		protected virtual void OnFieldsChanged()
		{
			if (this.FieldsChanged != null)
			{
				this.FieldsChanged (this);
			}
		}
		
		
		#region Class FieldList
		public class FieldList : System.Collections.IList
		{
			internal FieldList(ArrayList list)
			{
				this.list = list;
			}
			
			
			public Field					this[int index]
			{
				get { return this.list[index] as Field; }
			}
			
			
			#region IList Members
			public bool						IsReadOnly
			{
				get { return true; }
			}

			public bool						IsFixedSize
			{
				get { return true; }
			}
			
			
			object System.Collections.IList.this[int index]
			{
				get { return this.list[index]; }
				set { throw new ResourceException ("Fields in a list cannot be modified."); }
			}

			
			public int Add(object value)
			{
				throw new ResourceException ("Fields in a list cannot be added.");
			}

			public void Insert(int index, object value)
			{
				throw new ResourceException ("Fields in a list cannot be inserted.");
			}

			public void RemoveAt(int index)
			{
				throw new ResourceException ("Fields in a list cannot be removed.");
			}

			public void Remove(object value)
			{
				throw new ResourceException ("Fields in a list cannot be removed.");
			}

			public bool Contains(object value)
			{
				return this.list.Contains (value);
			}

			public void Clear()
			{
				throw new ResourceException ("Fields in a list cannot be removed.");
			}

			public int IndexOf(object value)
			{
				return this.list.IndexOf (value);
			}
			#endregion
			
			#region ICollection Members
			public bool						IsSynchronized
			{
				get { return this.list.IsSynchronized; }
			}

			public int						Count
			{
				get { return this.list.Count; }
			}

			public object					SyncRoot
			{
				get { return this.list.SyncRoot; }
			}
			
			
			public void CopyTo(System.Array array, int index)
			{
				this.list.CopyTo (array, index);
			}
			#endregion
			
			#region IEnumerable Members
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}
			#endregion
			
			protected ArrayList				list;
		}
		#endregion
		
		#region Class Field
		public class Field
		{
			protected Field()
			{
				this.parent = null;
			}
			
			
			public Field(ResourceBundle parent, System.Xml.XmlNode xml)
			{
				this.parent = parent;
				this.name   = parent.GetAttributeValue (xml, "name");
				this.about  = parent.GetAttributeValue (xml, "about");
				this.xml    = xml;
			}
			
			public Field(ResourceBundle parent, ResourceBundle bundle)
			{
				this.parent = parent;
				this.name   = bundle.Name;
				this.about  = bundle.About;
				this.data   = bundle;
				this.type   = ResourceFieldType.Bundle;
			}
			
			
			public string					Name
			{
				get { return this.name; }
//				set { this.name = value; }
			}
			
			public string					About
			{
				get { return this.about; }
//				set { this.about = value; }
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
			
			public bool						IsEmpty
			{
				get { return this.parent == null; }
			}
			
			public bool						IsRef
			{
				get
				{
					if (this.xml != null)
					{
						return this.xml.Name == "ref";
					}
					
					return false;
				}
			}
			
			public string					AsString
			{
				get
				{
					if (this.IsEmpty)
					{
						return null;
					}
					
					if (this.Type == ResourceFieldType.Data)
					{
						return this.Data as string;
					}
					
					throw new ResourceException (string.Format ("Cannot convert field '{0}' to string. XML: {1}.", this.Name, this.xml == null ? "-" : this.xml.OuterXml));
				}
			}
			
			public byte[]					AsBinary
			{
				get
				{
					if (this.IsEmpty)
					{
						return null;
					}
					
					if (this.Type == ResourceFieldType.Binary)
					{
						return this.Data as byte[];
					}
					
					throw new ResourceException (string.Format ("Cannot convert field '{0}' to binary. XML: {1}.", this.Name, this.xml == null ? "-" : this.xml.OuterXml));
				}
			}
			
			public ResourceBundle			AsBundle
			{
				get
				{
					if (this.IsEmpty)
					{
						return null;
					}
					
					if (this.Type == ResourceFieldType.Bundle)
					{
						return this.Data as ResourceBundle;
					}
					
					throw new ResourceException (string.Format ("Cannot convert field '{0}' to bundle. XML: {1}.", this.Name, this.xml == null ? "-" : this.xml.OuterXml));
				}
			}
			
			public FieldList				AsList
			{
				get
				{
					if (this.IsEmpty)
					{
						return null;
					}
					
					if (this.Type == ResourceFieldType.List)
					{
						return this.Data as FieldList;
					}
					
					throw new ResourceException (string.Format ("Cannot convert field '{0}' to list. XML: {1}.", this.Name, this.xml == null ? "-" : this.xml.OuterXml));
				}
			}
			
			
			public static readonly Field	Empty = new Field ();
			
			public void SetName(string name)
			{
				if (this.name != name)
				{
					this.name = name;
					
					if (this.xml != null)
					{
						this.parent.SetAttributeValue (this.xml, "name", name);
					}
				}
			}
			
			public void SetAbout(string about)
			{
				if (this.about != about)
				{
					this.about = about;
					
					if (this.xml != null)
					{
						this.parent.SetAttributeValue (this.xml, "about", about);
					}
				}
			}
			
			public void SetStringValue(string value)
			{
				if ((this.type != ResourceFieldType.Data) ||
					((string)this.data != value))
				{
					//	L'appelant doit s'assurer que la valeur pass�e en entr�e est valide d'un point
					//	de vue XML (balises �quilibr�es, etc.)
				
					this.type = ResourceFieldType.Data;
					this.data = value;
				
					if (value.IndexOf ('<') < 0)
					{
						//	Il n'y a aucune balise <>, donc on peut utiliser le texte tel quel :
					
						this.xml.InnerXml = value;
					}
					else
					{
						//	Il y a des balises, il faut donc signaler que la valeur est du XML et
						//	pas un texte simple :
					
						this.xml.InnerXml = "<xml>" + value + "</xml>";
					}
					
					this.parent.OnFieldsChanged ();
				}
			}
			
			
			protected void Compile()
			{
				if ((this.type == ResourceFieldType.None) &&
					(this.xml != null))
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
						
						case "ref":
							throw new ResourceException (string.Format ("Field contains unresolved <ref>, it cannot be compiled. XML: {0}.", this.xml.OuterXml));
						
						default:
							throw new ResourceException (string.Format ("Unsupported tag <{0}> cannot be compiled. XML: {1}.", this.xml.Name, this.xml.OuterXml));
					}
					
					System.Diagnostics.Debug.Assert (this.type != ResourceFieldType.None);
				}
			}
			
			protected void CompileBundle()
			{
				this.data = new ResourceBundle (this.parent, this.parent.Name + "#" + this.Name, this.xml);
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
					throw new ResourceException (string.Format ("<ref target='{0}'/> resolution is not <data> compatible. XML: {1}.", target, field.Xml.OuterXml));
				}
				
				string data = field.Data as string;
				buffer.Append (data);
			}
			
			protected void CompileList()
			{
				ArrayList list = new ArrayList ();
				
				this.parent.CreateFieldList (this.xml, list, false);
				
				//	Les champs stock�s dans la liste ont des noms qui sont du type 'nom[n]' o� 'nom' est
				//	le nom donn� � la liste, et 'n' l'index (� partir de 0..)
				
				for (int i = 0; i < list.Count; i++)
				{
					Field field = list[i] as Field;
					field.SetName (string.Format ("{0}[{1}]", this.Name, i));
				}
				
				this.data = new FieldList (list);
				this.type = ResourceFieldType.List;
			}
			
			
			protected ResourceBundle		parent;
			protected string				name;
			protected string				about;
			protected System.Xml.XmlNode	xml;
			protected object				data;
			protected ResourceFieldType		type = ResourceFieldType.None;
		}
		#endregion
		
		public event EventHandler			FieldsChanged;
		
		protected string					name;
		protected string					type;
		protected string					about;
		
		protected System.Xml.XmlNode		xmlroot;
		protected int						depth;
		protected int						compile_count;
		protected string					prefix;
		protected ResourceLevel				level;
		protected Field[]					fields;
		protected bool						ref_inclusion = true;
		protected bool						auto_merge    = true;
		protected CultureInfo				culture;
	}
}
