//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	using System.Collections;
	
	/// <summary>
	/// Implémentation d'un ResourceBundle basé sur un stockage interne de
	/// l'information sous forme XML DOM.
	/// </summary>
	public class ResourceBundle
	{
		public static ResourceBundle Create(string name, string prefix, ResourceLevel level, int recursion)
		{
			ResourceBundle bundle = new ResourceBundle (name);
			
			bundle.prefix = prefix;
			bundle.level  = level;
			bundle.depth  = recursion;
			
			return bundle;
		}
		
		public static ResourceBundle Create(string name)
		{
			return ResourceBundle.Create (name, null, ResourceLevel.Merged, 0);
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
		
		
		protected ResourceBundle(string name)
		{
			this.name = name;
			this.fields = new Field[0];
		}
		
		protected ResourceBundle(ResourceBundle parent, string name, System.Xml.XmlNode xmlroot) : this(name)
		{
			this.prefix = parent.prefix;
			this.level  = parent.level;
			this.depth  = parent.depth + 1;
			
			this.Compile (xmlroot);
			this.Merge ();
		}
		
		
		
		public string						Name
		{
			get { return this.name; }
		}
		
		public bool							IsEmpty
		{
			get { return this.CountFields == 0; }
		}
		
		public Field[]						Fields
		{
			get
			{
				return this.fields.Clone () as Field[];
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
		
		
		public Field						this[string name]
		{
			get
			{
				Field field = this.GetField (name);
				
				if (field != null)
				{
					return field;
				}
				
				return Field.Empty;
			}
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
		
		public bool Contains(string name)
		{
			return this.GetField (name) != null;
		}
		
		
		
		
		public void Compile(byte[] data)
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
				
				this.Compile (xmldoc.DocumentElement);
				this.Merge ();
			}
		}
		
		public void Compile(System.Xml.XmlNode xmlroot)
		{
			if (this.depth > ResourceBundle.MaxRecursion)
			{
				throw new ResourceException (string.Format ("Bundle is too complex, giving up."));
			}
			
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
						
						ResourceBundle bundle = this.ResolveRefBundle (node);
						
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
				throw new ResourceException (string.Format ("<ref target='{0}'/> does not reference a bundle.", ref_target));
			}
			if (ref_type != null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> specifies type='{1}'.", ref_target, ref_type));
			}
			
			ResourceBundle bundle = Resources.GetBundle (target_bundle, this.level, this.depth + 1) as ResourceBundle;
			
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
			
			ResourceBundle bundle = Resources.GetBundle (target_bundle, this.level, this.depth + 1) as ResourceBundle;
			
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
			public IEnumerator GetEnumerator()
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
				this.xml    = xml;
			}
			
			public Field(ResourceBundle parent, ResourceBundle bundle)
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
			
			public bool						IsEmpty
			{
				get { return this.parent == null; }
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
					
					throw new ResourceException (string.Format ("Cannot convert field '{0}' to string.", this.Name));
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
					
					throw new ResourceException (string.Format ("Cannot convert field '{0}' to binary.", this.Name));
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
					
					throw new ResourceException (string.Format ("Cannot convert field '{0}' to bundle.", this.Name));
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
					
					throw new ResourceException (string.Format ("Cannot convert field '{0}' to list.", this.Name));
				}
			}
			
			
			public static readonly Field	Empty = new Field ();
			
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
						
						default:
							throw new ResourceException (string.Format ("Unsupported tag <{0}> cannot be compiled.", this.xml.Name));
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
				
				this.data = new FieldList (list);
				this.type = ResourceFieldType.List;
			}
			
			
			protected ResourceBundle		parent;
			protected string				name;
			protected System.Xml.XmlNode	xml;
			protected object				data;
			protected ResourceFieldType		type = ResourceFieldType.None;
		}
		#endregion
		
		
		protected string					name;
		protected System.Xml.XmlNode		xmlroot;
		protected int						depth;
		protected int						compile_count;
		protected string					prefix;
		protected ResourceLevel				level;
		protected Field[]					fields;
		
		protected const int					MaxRecursion = 50;
	}
}
