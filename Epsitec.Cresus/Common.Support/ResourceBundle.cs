//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	using System.Globalization;
	
	using ArrayList = System.Collections.ArrayList;
	using Hashtable = System.Collections.Hashtable;
	
	/// <summary>
	/// Implémentation d'un ResourceBundle basé sur un stockage interne de
	/// l'information sous forme XML DOM.
	/// </summary>
	public class ResourceBundle : Types.DependencyObject, System.ICloneable
	{
		public static ResourceBundle Create(ResourceManager resource_manager)
		{
			return ResourceBundle.Create (resource_manager, null, null, ResourceLevel.Merged, null, 0);
		}
		
		public static ResourceBundle Create(ResourceManager resource_manager, ResourceLevel level)
		{
			return ResourceBundle.Create (resource_manager, null, null, level, null, 0);
		}
		
		public static ResourceBundle Create(ResourceManager resource_manager, string name)
		{
			return ResourceBundle.Create (resource_manager, null, name, ResourceLevel.Merged, null, 0);
		}
		
		public static ResourceBundle Create(ResourceManager resource_manager, string name, ResourceLevel level)
		{
			return ResourceBundle.Create (resource_manager, null, name, level, null, 0);
		}
		
		public static ResourceBundle Create(ResourceManager resource_manager, string prefix, string name, ResourceLevel level)
		{
			return ResourceBundle.Create (resource_manager, prefix, name, level, null, 0);
		}
		
		public static ResourceBundle Create(ResourceManager resource_manager, string prefix, string name, ResourceLevel level, int recursion)
		{
			return ResourceBundle.Create (resource_manager, prefix, name, level, null, recursion);
		}
		
		public static ResourceBundle Create(ResourceManager resource_manager, string prefix, string name, ResourceLevel level, CultureInfo culture)
		{
			return ResourceBundle.Create (resource_manager, prefix, name, level, culture, 0);
		}
		
		public static ResourceBundle Create(ResourceManager resource_manager, string prefix, string name, ResourceLevel level, CultureInfo culture, int recursion)
		{
			ResourceBundle bundle = new ResourceBundle (resource_manager, name);
			
			bundle.DefinePrefix (prefix);
			bundle.DefineCulture (culture);
			bundle.level   = level;
			bundle.depth   = recursion;
			
			return bundle;
		}
		
		
		protected ResourceBundle(ResourceManager resource_manager)
		{
			this.manager = resource_manager;
			this.fields  = new Field[0];
		}
		
		protected ResourceBundle(ResourceManager resource_manager, string name) : this (resource_manager)
		{
			this.DefineName (name);
		}
		
		protected ResourceBundle(ResourceManager resource_manager, ResourceBundle parent, string name, System.Xml.XmlNode xmlroot) : this (resource_manager, name)
		{
			this.DefinePrefix (parent.prefix);
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

		public int							Rank
		{
			get
			{
				return this.rank;
			}
		}
		
		public ResourceLevel				ResourceLevel
		{
			get
			{
				return this.level;
			}
		}
		
		public ResourceManager				ResourceManager
		{
			get
			{
				return this.manager;
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
				return this.manager.MakeFullName (this.prefix, this.name);
			}
		}
		
		public bool							IsEmpty
		{
			get
			{
				return this.FieldCount == 0;
			}
		}
		
		public int							FieldCount
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
		
		public IEnumerable<Field>			Fields
		{
			get
			{
				foreach (Field field in this.fields)
				{
					yield return field;
				}
			}
		}
		
		public void DefineName(string name)
		{
			if (name != null)
			{
				if (! RegexFactory.ResourceBundleName.IsMatch (name))
				{
					throw new ResourceException (string.Format ("Name '{0}' is not a valid bundle name.", name));
				}
			}
			
			this.name = name;
		}
		
		public void DefineType(string type)
		{
			if (type != null)
			{
				if (! RegexFactory.ResourceBundleName.IsMatch (type))
				{
					throw new ResourceException (string.Format ("Type '{0}' is not a valid type name.", type));
				}
			}
			
			this.type = type;
		}
		
		public void DefineAbout(string about)
		{
			this.about = about;
		}

		public void DefineRank(int rank)
		{
			this.rank = rank;
		}
		
		public void DefineManager(ResourceManager resource_manager)
		{
			this.manager = resource_manager;
		}
		
		public void DefinePrefix(string prefix)
		{
			if (prefix != null)
			{
				switch (prefix)
				{
					case "file":
					case "base":
						break;
					default:
						throw new System.NotImplementedException (string.Format ("Support for prefix {0} not implemented.", prefix));
				}
				
				if (! RegexFactory.AlphaName.IsMatch (prefix))
				{
					throw new ResourceException (string.Format ("Prefix '{0}' is not a valid prefix name.", prefix));
				}
			}
			
			this.prefix = prefix;
		}
		
		public void DefineCulture(CultureInfo culture)
		{
			this.culture = culture;
		}
		
		public bool Contains(string name)
		{
			return this[name].IsEmpty == false;
		}


		public int IndexOf(Field field)
		{
			for (int i=0; i<this.fields.Length; i++)
			{
				if (this.fields[i] == field)
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(Field field)
		{
			this.Insert (this.fields.Length, field);
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
		
		public void Remove(string name)
		{
			for (int i = 0; i < this.fields.Length; i++)
			{
				if (this.fields[i].Name == name)
				{
					this.Remove (i);
					return;
				}
			}
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
						//	Cas particulier: si l'utilisateur a désactivé l'inclusion des <ref>
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
		
		
		public static bool CheckBundleHeader(byte[] data)
		{
			if ((data == null) ||
				(data.Length < 16))
			{
				return false;
			}
			
			System.Text.Decoder decoder = System.Text.Encoding.UTF8.GetDecoder ();
			
			int byte_length = 16;
			int char_length = decoder.GetCharCount (data, 0, byte_length);
				
			char[] chars = new char[char_length];
				
			decoder.GetChars (data, 0, byte_length, chars, 0);
				
			//	Le header peut commencer par un "byte order mark" Unicode; il faut le sauter
			//	car il n'est pas signifiant pour le fichier XML considéré :
			
			int    start  = chars[0] == 0xfeff ? 1 : 0;
			string header = new string (chars, start, chars.Length - start);
			
			if ((header.StartsWith ("<?xml")) ||
				(header.StartsWith ("<!--")) ||
				(header.StartsWith ("<bundle")))
			{
				return true;
			}
			
			return false;
		}
		
		
		public void Compile(byte[] data)
		{
			if (data == null)
			{
				return;
			}
			
			//	La compilation des données part du principe que le bundle XML est "well formed",
			//	c'est-à-dire qu'il comprend un seul bloc à la racine (<bundle>..</bundle>), et
			//	que son contenu est valide (l'en-tête <?xml ...?> n'est pas requis).
			
			if (ResourceBundle.CheckBundleHeader (data))
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
			else
			{
				throw new ResourceException ("Cannot compile garbage; invalid data provided.");
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
			string culture_attr = this.GetAttributeValue (xmlroot, "culture");
			string rank_attr  = this.GetAttributeValue (xmlroot, "rank");
			
			if ((name_attr != null) && (name_attr != ""))
			{
				if ((this.name == null) ||
					(this.name == ""))
				{
					this.name = name_attr;
				}
			}
			if ((type_attr != null) && (type_attr != ""))
			{
				this.type = type_attr;
			}
			if ((about_attr != null) && (about_attr != ""))
			{
				this.about = about_attr;
			}
			if ((culture_attr != null) && (culture_attr != ""))
			{
				this.culture = new CultureInfo (culture_attr);
			}
			
			if (string.IsNullOrEmpty (rank_attr))
			{
				this.rank = -1;
			}
			else
			{
				this.rank = System.Int32.Parse (rank_attr, System.Globalization.CultureInfo.InvariantCulture);
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
				System.Xml.XmlDocument xmldoc = this.CreateXmlDocument (true);
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
			System.Xml.XmlElement   bundle_node  = xmldoc.CreateElement ("bundle");
			System.Xml.XmlAttribute name_attr    = xmldoc.CreateAttribute ("name");
			System.Xml.XmlAttribute type_attr    = xmldoc.CreateAttribute ("type");
			System.Xml.XmlAttribute about_attr   = xmldoc.CreateAttribute ("about");
			System.Xml.XmlAttribute culture_attr = xmldoc.CreateAttribute ("culture");
			System.Xml.XmlAttribute rank_attr    = xmldoc.CreateAttribute ("rank");
			
			name_attr.Value    = this.name;
			type_attr.Value    = this.type;
			about_attr.Value   = this.about;
			culture_attr.Value = this.culture.TwoLetterISOLanguageName;
			rank_attr.Value    = this.rank < 0 ? "" : this.rank.ToString (System.Globalization.CultureInfo.InvariantCulture);
			
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
			if (culture_attr.Value != "")
			{
				bundle_node.Attributes.Append (culture_attr);
			}
			if (rank_attr.Value != "")
			{
				bundle_node.Attributes.Append (rank_attr);
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
		
		
		public Field CreateField(ResourceFieldType type)
		{
			return this.CreateField (type, null);
		}
		
		public Field CreateField(ResourceFieldType type, object more)
		{
			switch (type)
			{
				case ResourceFieldType.Data:
					return this.CreateFieldAsData ();
				
				case ResourceFieldType.List:
					return this.CreateFieldAsList (more as System.Collections.ICollection);
				
				case ResourceFieldType.Bundle:
					return this.CreateFieldAsBundle (more as ResourceBundle);
			}
			
			throw new System.NotImplementedException (string.Format ("{0} support not implemented.", type));
		}
		
		protected Field CreateFieldAsData()
		{
			System.Xml.XmlDocument  doc  = this.XmlDocument;
			System.Xml.XmlElement   elem = doc.CreateElement ("data");
			System.Xml.XmlAttribute attr = doc.CreateAttribute ("name");
			
			elem.Attributes.Append (attr);
			attr.Value = "";
			
			Field field = new Field (this, elem);
			
			return field;
		}
		
		protected Field CreateFieldAsList(System.Collections.ICollection collection)
		{
			if (collection == null)
			{
				throw new System.ArgumentNullException ("collection", "List field needs a valid collection.");
			}
			
			ResourceBundle[] bundles = new ResourceBundle[collection.Count];
			collection.CopyTo (bundles, 0);
			
			System.Xml.XmlDocument  doc  = this.XmlDocument;
			System.Xml.XmlElement   elem = doc.CreateElement ("list");
			System.Xml.XmlAttribute attr = doc.CreateAttribute ("name");
			
			elem.Attributes.Append (attr);
			
			for (int i = 0; i < bundles.Length; i++)
			{
				elem.AppendChild (bundles[i].CreateXmlNode (doc));
			}
			
			Field field = new Field (this, elem);
			
			return field;
		}
		
		protected Field CreateFieldAsBundle(ResourceBundle bundle)
		{
			if (bundle == null)
			{
				throw new System.ArgumentNullException ("bundle", "Bundle field needs a valid bundle.");
			}
			
			Field field = new Field (this, bundle.CreateXmlNode (this.XmlDocument));
			
			return field;
		}
		
		
		public static bool SplitTarget(string target, out string target_bundle, out string target_field)
		{
			ResourceManager.ResolveDruidReference (ref target);
			
			int pos = target.IndexOf ('#');
			
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
		
		public static string MakeTarget(string target_bundle, string target_field)
		{
			if ((target_bundle == null) ||
				(target_bundle.IndexOf ('#') != -1) ||
				(target_field == null) ||
				(target_field.IndexOf ('#') != -1))
			{
				throw new ResourceException ("Invalid target specified.");
			}
			
			return string.Concat (target_bundle, "#", target_field);
		}
		
		public static string ExtractSortName(string sort_name)
		{
			int pos = sort_name.IndexOf ('/');
			
			if (pos < 0)
			{
				throw new ResourceException (string.Format ("'{0}' is an invalid sort name", sort_name));
			}
			
			return sort_name.Substring (pos+1);
		}
		
		public static string MakeSortName(string name, int rank, int num_digits)
		{
			string rank_text = rank.ToString (System.Globalization.CultureInfo.InvariantCulture);
			
			if (rank_text.Length > num_digits)
			{
				throw new ResourceException (string.Format ("Cannot create a sort name using '{0}/{1}', maximum {2} digits not respected.", name, rank_text, num_digits));
			}

			string sort_name = string.Concat (rank_text, "/", name);
			
			return sort_name.PadLeft (name.Length + num_digits + 1, '0');
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
						//	référencé par un tag <ref>.
						
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
				throw new ResourceException (string.Format ("<ref target='{0}'/> does not reference a bundle. XML: {1}.", ref_target, node.OuterXml));
			}
			if (ref_type != null)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> specifies type='{1}'. XML: {2}.", ref_target, ref_type, node.OuterXml));
			}
			
			ResourceBundle bundle = this.manager.GetBundle (target_bundle, this.level, this.depth + 1) as ResourceBundle;
			
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
			
			ResourceBundle bundle = this.manager.GetBundle (target_bundle, this.level, this.depth + 1) as ResourceBundle;
			
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
				byte[] data = this.manager.GetBinaryData (target_bundle, level);
				
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
				
				target = this.manager.MakeFullName (this.prefix, target);
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
				if (value == "")
				{
					return;
				}
				
				node.Attributes.Append (this.XmlDocument.CreateAttribute (name));
				
				this.OnFieldsChanged ();
				
				return;
			}
			
			if (value == "")
			{
				node.Attributes.Remove (node.Attributes[name]);
			}
			else
			{
				if (node.Attributes[name].Value == value)
				{
					return;
				}
				
				node.Attributes[name].Value = value;
			}
			
			this.OnFieldsChanged ();
		}
		
		
		protected virtual void OnFieldsChanged()
		{
			if (this.FieldsChanged != null)
			{
				this.FieldsChanged (this);
			}
		}
		
		
		public ResourceBundle Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ()) as ResourceBundle;
		}


		protected virtual object CloneNewObject()
		{
			return new ResourceBundle (this.manager);
		}
		
		protected virtual object CloneCopyToNewObject(object o)
		{
			ResourceBundle that = o as ResourceBundle;
			
			that.name    = this.name;
			that.type    = this.type;
			that.about   = this.about;
			that.prefix  = this.prefix;
			that.level   = this.level;
			that.culture = this.culture;
			that.rank    = this.rank;

			Types.DependencyObject.CopyAttachedProperties (this, that);
			
			that.Compile (this.CreateXmlAsData ());
			
			return that;
		}
		
		
		#region ICloneable Members
		object System.ICloneable.Clone()
		{
			return this.Clone ();
		}
		#endregion
		
		#region FieldList Class
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
		
		#region Field Class
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

				string mod = parent.GetAttributeValue (xml, "mod");

				if ((mod != null) &&
					(mod.Length > 0))
				{
					this.modification_id = int.Parse (mod, System.Globalization.CultureInfo.InvariantCulture);
				}
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
			}
			
			public string					About
			{
				get { return this.about; }
			}

			public int						ModificationId
			{
				get
				{
					return this.modification_id;
				}
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
				get
				{
					return this.xml;
				}
			}
			
			public bool						IsEmpty
			{
				get
				{
					return this.parent == null;
				}
			}
			
			public bool						IsValid
			{
				get
				{
					if (this.IsEmpty)
					{
						return false;
					}
					
					if ((this.name == null) ||
						(this.name.Length == 0) ||
						(this.name == "?"))
					{
						return false;
					}
					
					return true;
				}
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
			
			public bool						IsDataRef
			{
				get
				{
					if ((this.xml != null) &&
						(this.xml.Name == "data") &&
						(this.xml.FirstChild != null))
					{
						return this.xml.FirstChild.Name == "ref";
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
				if (this.IsEmpty)
				{
					throw new ResourceException ("An empty field cannot be modified.");
				}
				
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
				if (this.IsEmpty)
				{
					throw new ResourceException ("An empty field cannot be modified.");
				}
				
				if (this.about != about)
				{
					this.about = about;
					
					if (this.xml != null)
					{
						this.parent.SetAttributeValue (this.xml, "about", about);
					}
				}
			}

			public void SetModificationId(int id)
			{
				if (this.IsEmpty)
				{
					throw new ResourceException ("An empty field cannot be modified.");
				}

				if (this.modification_id != id)
				{
					this.modification_id = id;

					if (this.xml != null)
					{
						this.parent.SetAttributeValue (this.xml, "mod", id.ToString (System.Globalization.CultureInfo.InvariantCulture));
					}
				}
			}

			public void SetStringValue(string value)
			{
				if (this.IsEmpty)
				{
					throw new ResourceException ("An empty field cannot be modified.");
				}
				
				if ((this.type != ResourceFieldType.Data) ||
					((string)this.data != value))
				{
					this.type = ResourceFieldType.Data;
					this.data = value;
					
					this.xml.InnerText = value;
					
					this.parent.OnFieldsChanged ();
				}
			}
			
			public void SetXmlValue(string value)
			{
				if (this.IsEmpty)
				{
					throw new ResourceException ("An empty field cannot be modified.");
				}
				
				string xml = string.Concat ("<xml>", value, "</xml>");
				
				if ((this.type != ResourceFieldType.Data) ||
					((string)this.data != value))
				{
					this.type = ResourceFieldType.Data;
					this.data = value;
					
					this.xml.InnerXml = xml;
					
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
				this.data = new ResourceBundle (this.parent.ResourceManager, this.parent, null, this.xml);
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
				
				//	Les champs stockés dans la liste ont des noms qui sont du type 'nom[n]' où 'nom' est
				//	le nom donné à la liste, et 'n' l'index (à partir de 0..)
				
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
			protected int					modification_id;
			protected System.Xml.XmlNode	xml;
			protected object				data;
			protected ResourceFieldType		type = ResourceFieldType.None;
		}
		#endregion

		public event EventHandler FieldsChanged;
		
		protected string					name;
		protected string					type;
		protected string					about;
		
		protected ResourceManager			manager;
		protected System.Xml.XmlNode		xmlroot;
		protected int						depth;
		protected int						compile_count;
		protected string					prefix;
		protected ResourceLevel				level;
		protected Field[]					fields;
		protected bool						ref_inclusion = true;
		protected bool						auto_merge    = true;
		protected CultureInfo				culture;
		protected int						rank = -1;
	}
}
