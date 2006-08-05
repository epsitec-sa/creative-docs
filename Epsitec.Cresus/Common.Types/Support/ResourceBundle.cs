//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using System.Globalization;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// Implémentation d'un ResourceBundle basé sur un stockage interne de
	/// l'information sous forme XML DOM.
	/// </summary>
	public sealed class ResourceBundle : Types.DependencyObject, System.ICloneable
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
			return ResourceBundle.Create (resource_manager, prefix, new ResourceModuleInfo (), name, level, culture, recursion);
		}

		public static ResourceBundle Create(ResourceManager resource_manager, string prefix, ResourceModuleInfo module, string name, ResourceLevel level, CultureInfo culture, int recursion)
		{
			ResourceBundle bundle = new ResourceBundle (resource_manager, name);

			bundle.DefinePrefix (prefix);
			bundle.DefineModule (module);
			bundle.DefineCulture (culture);
			bundle.level = level;
			bundle.depth = recursion;

			return bundle;
		}
		
		
		private ResourceBundle(ResourceManager resource_manager)
		{
			this.manager = resource_manager;
			this.fields  = new Field[0];
		}

		private ResourceBundle(ResourceManager resource_manager, string name) : this (resource_manager)
		{
			this.DefineName (name);
		}

		private ResourceBundle(ResourceManager resource_manager, ResourceBundle parent, string name, System.Xml.XmlNode xmlroot) : this (resource_manager, name)
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

		public string						Caption
		{
			get
			{
				return this.caption == null ? "" : this.caption;
			}
		}
		
		public Druid						Druid
		{
			get
			{
				return this.druid;
			}
		}

		public ResourceModuleInfo			Module
		{
			get
			{
				return this.module;
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

		public string						Prefix
		{
			get
			{
				return Resources.JoinFullPrefix (this.prefix, this.module.ToString ());
			}
		}
		
		public string						PrefixedName
		{
			get
			{
				return this.manager.NormalizeFullId (this.Prefix, this.name);
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
			get { return this.refInclusion; }
			set { this.refInclusion = value; }
		}
		
		public bool							AutoMergeEnabled
		{
			get { return this.autoMerge; }
			set { this.autoMerge = value; }
		}
		
		
		public Field						this[string name]
		{
			get
			{
				return this[this.IndexOf (name)];
			}
		}

		public Field						this[Druid druid]
		{
			get
			{
				return this[this.IndexOf (druid)];
			}
		}

		public Field						this[int index]
		{
			get
			{
				if (index < 0)
				{
					return Field.Empty;
				}
				else
				{
					return this.fields[index];
				}
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
				if ((! RegexFactory.ResourceBundleName.IsMatch (name)) &&
					(! Druid.IsValidBundleId (name)))
				{
					throw new ResourceException (string.Format ("Name '{0}' is not a valid bundle name.", name));
				}
			}
			
			this.name = name;
			this.druid = Druid.IsValidBundleId (name) ? Druid.Parse (name) : Druid.Empty;
		}

		public void DefineCaption(string caption)
		{
			if (caption != null)
			{
				if (! RegexFactory.ResourceBundleName.IsMatch (caption))
				{
					throw new ResourceException (string.Format ("Caption '{0}' is not a valid bundle caption.", caption));
				}
			}
			
			this.caption = caption;
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

		#region Internal Define... Methods

		internal void DefineManager(ResourceManager resource_manager)
		{
			this.manager = resource_manager;
		}

		internal void DefinePrefix(string prefix)
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

				if (!RegexFactory.AlphaName.IsMatch (prefix))
				{
					throw new ResourceException (string.Format ("Prefix '{0}' is not a valid prefix name.", prefix));
				}
			}

			this.prefix = prefix;
		}

		internal void DefineModule(ResourceModuleInfo module)
		{
			this.module = module;
		}

		internal void DefineCulture(CultureInfo culture)
		{
			this.culture = culture;
		}

		internal void DefineRecursion(int recursion)
		{
			this.depth = recursion;
		}

		#endregion

		public bool Contains(string name)
		{
			return this.IndexOf (name) < 0 ? false : true;
		}

		public int IndexOf(string name)
		{
			if (string.IsNullOrEmpty (name))
			{
				return -1;
			}

			if (name[0] == Resources.FieldIdPrefix)
			{
				return this.IndexOf (Druid.Parse (name));
			}
			else
			{
				for (int i = 0; i < this.fields.Length; i++)
				{
					if (this.fields[i].Name == name)
					{
						return i;
					}
				}
			}

			return -1;
		}

		public int IndexOf(Druid druid)
		{
			DruidType type = druid.Type;

			switch (type)
			{
				case DruidType.ModuleRelative:
					break;
				case DruidType.Full:
					if (this.module.Id != druid.Module)
					{
						return -1;
					}
					break;
				default:
					return -1;
			}
			long id    = druid.ToFieldId ();

			for (int i = 0; i < this.fields.Length; i++)
			{
				if (this.fields[i].Id == id)
				{
					return i;
				}
			}
			
			return -1;
		}

		public int IndexOf(Field field)
		{
			for (int i = 0; i < this.fields.Length; i++)
			{
				if (this.fields[i] == field)
				{
					return i;
				}
			}
			
			return -1;
		}

		public void Insert(int index, Field field)
		{
			if ((index < 0) ||
				(index > this.fields.Length))
			{
				throw new System.IndexOutOfRangeException (string.Format ("ResourceBundle field index {0} is out of range", index));
			}

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
			if ((index < 0) ||
				(index > this.fields.Length))
			{
				throw new System.IndexOutOfRangeException (string.Format ("ResourceBundle field index {0} is out of range", index));
			}
			
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
			int index = this.IndexOf (name);
			
			if (index >= 0)
			{
				this.Remove (index);
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
		
		
		public void Merge(ResourceLevel dataLevel)
		{
			List<Field> list = new List<Field> ();
			Dictionary<string, int> hash = new Dictionary<string, int> ();
			
			for (int i = 0; i < this.fields.Length; i++)
			{
				Field field = this.fields[i];

				if (field.DataLevel == ResourceLevel.None)
				{
					field.SetDataLevel (dataLevel);
				}
				
				string name = field.Name;
				long   id   = field.Id;

				if (id >= 0)
				{
					name = Druid.FromFieldId (id).ToFieldName ();
				}

				int index;
				
				if (name == null)
				{
					//	En principe, tous les champs doivent avoir un nom valide.
					
					if (this.refInclusion == false)
					{
						//	Cas particulier: si l'utilisateur a désactivé l'inclusion des <ref>
						//	alors il se peut qu'un champ soit en fait un <ref> sans nom, auquel
						//	cas on doit le copier tel quel, sans faire de merge.

						list.Add (field);
					}
					else
					{
						throw new ResourceException (string.Format ("Field has no name. XML: {0}.", field.Xml.OuterXml));
					}
				}
				else if (hash.TryGetValue (name, out index))
				{
					//	Le champ est déjà connu: on remplace simplement l'ancienne occurrence
					//	dans la liste.

					list[index] = field;
				}
				else
				{
					//	Le champ n'est pas connu: on ajoute le champ en fin de liste et on prend
					//	note de son index, pour pouvoir y accéder rapidement par la suite.

					hash[name] = list.Count;
					list.Add (field);
				}
			}

			this.fields = list.ToArray ();
		}

		public ResourceBundle Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ()) as ResourceBundle;
		}


		public void Compile(byte[] data)
		{
			this.Compile (data, this.ResourceLevel);
		}

		public void Compile(byte[] data, ResourceLevel dataLevel)
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
				
				this.Compile (xmldoc.DocumentElement, dataLevel);
			}
			else
			{
				throw new ResourceException ("Cannot compile garbage; invalid data provided.");
			}
		}

		public void Compile(System.Xml.XmlNode xmlroot)
		{
			this.Compile (xmlroot, this.ResourceLevel);
		}
		
		public void Compile(System.Xml.XmlNode xmlroot, ResourceLevel dataLevel)
		{
			if (this.depth > Resources.MaxRecursion)
			{
				throw new ResourceException (string.Format ("Bundle is too complex, giving up."));
			}
			
			if (xmlroot.Name != "bundle")
			{
				throw new ResourceException (string.Format ("Bundle does not start with <bundle> tag (<{0}> is an unsupported root).", xmlroot.Name));
			}
			
			string name_attr    = this.GetAttributeValue (xmlroot, "name");
			string caption_attr = this.GetAttributeValue (xmlroot, "caption");
			string type_attr    = this.GetAttributeValue (xmlroot, "type");
			string about_attr   = this.GetAttributeValue (xmlroot, "about");
			string culture_attr = this.GetAttributeValue (xmlroot, "culture");
			string rank_attr    = this.GetAttributeValue (xmlroot, "rank");
			
			if (!string.IsNullOrEmpty (name_attr))
			{
				if (string.IsNullOrEmpty (this.name))
				{
					this.DefineName (name_attr);
				}
			}
			if (!string.IsNullOrEmpty (caption_attr))
			{
				if (string.IsNullOrEmpty (this.caption))
				{
					this.DefineCaption (caption_attr);
				}
			}
			if (!string.IsNullOrEmpty (type_attr))
			{
				this.type = type_attr;
			}
			if (!string.IsNullOrEmpty (about_attr))
			{
				this.about = about_attr;
			}
			if (!string.IsNullOrEmpty (culture_attr))
			{
				this.culture = Resources.FindCultureInfo (culture_attr);
			}
			
			if (string.IsNullOrEmpty (rank_attr))
			{
				this.rank = -1;
			}
			else
			{
				this.rank = System.Int32.Parse (rank_attr, System.Globalization.CultureInfo.InvariantCulture);
			}

			List<Field> list = new List<Field> ();
			list.AddRange (this.fields);
			
			this.CreateFieldList (xmlroot, list, true);

			this.fields  = list.ToArray ();
			this.xmlroot = xmlroot;
			
			if (this.autoMerge)
			{
				this.Merge (dataLevel);
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
		
		public System.Xml.XmlDocument CreateXmlDocument(bool includeDeclaration)
		{
			System.Xml.XmlDocument xmldoc  = new System.Xml.XmlDocument ();
			System.Xml.XmlNode     xmlnode = this.CreateXmlNode (xmldoc);
			
			if (includeDeclaration)
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
			System.Xml.XmlAttribute caption_attr = xmldoc.CreateAttribute ("caption");
			System.Xml.XmlAttribute type_attr    = xmldoc.CreateAttribute ("type");
			System.Xml.XmlAttribute about_attr   = xmldoc.CreateAttribute ("about");
			System.Xml.XmlAttribute culture_attr = xmldoc.CreateAttribute ("culture");
			System.Xml.XmlAttribute rank_attr    = xmldoc.CreateAttribute ("rank");
			
			name_attr.Value    = this.name;
			caption_attr.Value = this.caption;
			type_attr.Value    = this.type;
			about_attr.Value   = this.about;
			culture_attr.Value = this.culture.TwoLetterISOLanguageName;
			rank_attr.Value    = this.rank < 0 ? "" : this.rank.ToString (System.Globalization.CultureInfo.InvariantCulture);
			
			if (name_attr.Value != "")
			{
				bundle_node.Attributes.Append (name_attr);
			}
			if (caption_attr.Value != "")
			{
				bundle_node.Attributes.Append (caption_attr);
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

			int start  = chars[0] == 0xfeff ? 1 : 0;
			string header = new string (chars, start, chars.Length - start);

			if ((header.StartsWith ("<?xml")) ||
				(header.StartsWith ("<!--")) ||
				(header.StartsWith ("<bundle")))
			{
				return true;
			}

			return false;
		}
		
		#region Private Methods

		private Field CreateFieldAsData()
		{
			System.Xml.XmlDocument doc  = this.XmlDocument;
			System.Xml.XmlElement elem = doc.CreateElement ("data");
			System.Xml.XmlAttribute attr = doc.CreateAttribute ("name");

			elem.Attributes.Append (attr);
			attr.Value = "";

			Field field = new Field (this, elem);

			return field;
		}

		private Field CreateFieldAsList(System.Collections.ICollection collection)
		{
			if (collection == null)
			{
				throw new System.ArgumentNullException ("collection", "List field needs a valid collection.");
			}

			ResourceBundle[] bundles = new ResourceBundle[collection.Count];
			collection.CopyTo (bundles, 0);

			System.Xml.XmlDocument doc  = this.XmlDocument;
			System.Xml.XmlElement elem = doc.CreateElement ("list");
			System.Xml.XmlAttribute attr = doc.CreateAttribute ("name");

			elem.Attributes.Append (attr);

			for (int i = 0; i < bundles.Length; i++)
			{
				elem.AppendChild (bundles[i].CreateXmlNode (doc));
			}

			Field field = new Field (this, elem);

			return field;
		}

		private Field CreateFieldAsBundle(ResourceBundle bundle)
		{
			if (bundle == null)
			{
				throw new System.ArgumentNullException ("bundle", "Bundle field needs a valid bundle.");
			}

			Field field = new Field (this, bundle.CreateXmlNode (this.XmlDocument));

			return field;
		}

		private void CreateFieldList(System.Xml.XmlNode xmlroot, List<Field> list, bool unpack_bundle_ref)
		{
			foreach (System.Xml.XmlNode node in xmlroot.ChildNodes)
			{
				if (node.NodeType == System.Xml.XmlNodeType.Element)
				{
					if ((node.Name == "ref") && (this.refInclusion))
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

		private ResourceBundle ResolveRefBundle(System.Xml.XmlNode node)
		{
			string ref_target  = this.GetAttributeValue (node, "target");
			string ref_type    = this.GetAttributeValue (node, "type");
			string full_target = this.GetTargetSpecification (ref_target);
			
			string target_bundle;
			string target_field;
			
			Resources.SplitFieldId (full_target, out target_bundle, out target_field);
			
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

		private Field ResolveRefField(System.Xml.XmlNode node)
		{
			string ref_target  = this.GetAttributeValue (node, "target");
			string ref_type    = this.GetAttributeValue (node, "type");
			string full_target = this.GetTargetSpecification (ref_target);
			
			string target_bundle;
			string target_field;

			Resources.SplitFieldId (full_target, out target_bundle, out target_field);
			
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
			if (bundle.depth > Resources.MaxRecursion)
			{
				throw new ResourceException (string.Format ("Bundle is too complex, giving up."));
			}
			
			Field field = bundle[target_field];
			
			if (field.IsEmpty)
			{
				throw new ResourceException (string.Format ("<ref target='{0}'/> could not be resolved. Missing field. XML: {1}.", ref_target, node.OuterXml));
			}
			
			return field;
		}

		private byte[] ResolveRefBinary(System.Xml.XmlNode node)
		{
			string ref_target  = this.GetAttributeValue (node, "target");
			string full_target = this.GetTargetSpecification (ref_target);
			
			string target_bundle;
			string target_field;

			Resources.SplitFieldId (full_target, out target_bundle, out target_field);
			
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

		private string GetTargetSpecification(string target)
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
				
				target = this.manager.NormalizeFullId (this.prefix, target);
			}
			
			return target;
		}

		private string GetAttributeValue(System.Xml.XmlNode node, string name)
		{
			System.Xml.XmlAttribute attr = node.Attributes[name];
			
			if (attr != null)
			{
				return attr.Value;
			}
			
			return null;
		}

		private void SetAttributeValue(System.Xml.XmlNode node, string name, string value)
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
				node.Attributes[name].Value = value;
				
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

		private void OnFieldsChanged()
		{
			if (this.FieldsChanged != null)
			{
				this.FieldsChanged (this);
			}
		}
		
		private object CloneNewObject()
		{
			return new ResourceBundle (this.manager);
		}

		private object CloneCopyToNewObject(object o)
		{
			ResourceBundle that = o as ResourceBundle;
			
			that.name    = this.name;
			that.druid   = this.druid;
			that.caption = this.caption;
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

		#endregion

		#region ICloneable Members
		object System.ICloneable.Clone()
		{
			return this.Clone ();
		}
		#endregion
		
		#region FieldList Class
		public class FieldList : System.Collections.IList
		{
			internal FieldList(List<Field> list)
			{
				this.list = list;
			}
			
			
			public Field					this[int index]
			{
				get
				{
					return this.list[index];
				}
			}

			
			public bool Contains(Field value)
			{
				return this.list.Contains (value);
			}

	
			#region IList Members
			public bool						IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public bool						IsFixedSize
			{
				get
				{
					return true;
				}
			}
			
			
			object System.Collections.IList.this[int index]
			{
				get
				{
					return this.list[index];
				}
				set
				{
					throw new ResourceException ("Fields in a list cannot be modified.");
				}
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

			bool System.Collections.IList.Contains(object value)
			{
				return this.Contains (value as Field);
			}

			public void Clear()
			{
				throw new ResourceException ("Fields in a list cannot be removed.");
			}

			public int IndexOf(object value)
			{
				return this.list.IndexOf (value as Field);
			}
			#endregion
			
			#region ICollection Members
			public bool						IsSynchronized
			{
				get
				{
					return false;
				}
			}

			public int						Count
			{
				get
				{
					return this.list.Count;
				}
			}

			public object					SyncRoot
			{
				get
				{
					return this.list;
				}
			}
			
			
			public void CopyTo(System.Array array, int index)
			{
				System.Array.Copy (this.list.ToArray (), 0, array, index, this.list.Count);
			}
			#endregion
			
			#region IEnumerable Members
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}
			#endregion

			private List<Field>				list;
		}
		#endregion
		
		#region Field Class
		public class Field
		{
			private Field()
			{
				this.parent = null;
			}
			
			
			public Field(ResourceBundle parent, System.Xml.XmlNode xml)
			{
				this.parent = parent;
				this.name   = parent.GetAttributeValue (xml, "name");
				this.about  = parent.GetAttributeValue (xml, "about");
				this.xml    = xml;

				string id = parent.GetAttributeValue (xml, "id");
				string mod = parent.GetAttributeValue (xml, "mod");

				if ((string.IsNullOrEmpty (id)) ||
					(Druid.IsValidModuleString (id) == false))
				{
					this.id = -1;
				}
				else
				{
					this.id = Druid.Parse (id).ToFieldId ();
				}


				if (!string.IsNullOrEmpty (mod))
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
				get
				{
					return this.name;
				}
			}
			
			internal long					Id
			{
				get
				{
					return this.id;
				}
			}

			public Druid					Druid
			{
				get
				{
					return Druid.FromFieldId (this.id);
				}
			}
			
			public string					About
			{
				get
				{
					return this.about;
				}
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

			public ResourceLevel			DataLevel
			{
				get
				{
					return this.level;
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

			public void SetDruid(Druid druid)
			{
				if (this.IsEmpty)
				{
					throw new ResourceException ("An empty field cannot be modified.");
				}

				long id = druid.ToFieldId ();

				if (this.id != id)
				{
					this.id = id;

					if (this.xml != null)
					{
						if (this.id < 0)
						{
							this.parent.SetAttributeValue (this.xml, "id", "");
						}
						else
						{
							string value = Druid.ToModuleString (this.id);
							this.parent.SetAttributeValue (this.xml, "id", value);
						}
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

			internal void SetDataLevel(ResourceLevel level)
			{
				this.level = level;
			}

			private void Compile()
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

			private void CompileBundle()
			{
				this.data = new ResourceBundle (this.parent.ResourceManager, this.parent, null, this.xml);
				this.type = ResourceFieldType.Bundle;
			}

			private void CompileData()
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

			private void CompileBinary()
			{
				byte[] data = this.parent.ResolveRefBinary (this.xml);
				
				this.type = ResourceFieldType.Binary;
				this.data = data;
			}

			private void CompileDataElement(System.Xml.XmlNode node, System.Text.StringBuilder buffer)
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

			private void CompileDataReference(System.Xml.XmlNode node, System.Text.StringBuilder buffer)
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

			private void CompileList()
			{
				List<Field> list = new List<Field> ();
				
				this.parent.CreateFieldList (this.xml, list, false);
				
				//	Les champs stockés dans la liste ont des noms qui sont du type 'nom[n]' où 'nom' est
				//	le nom donné à la liste, et 'n' l'index (à partir de 0..)
				
				for (int i = 0; i < list.Count; i++)
				{
					Field field = list[i];
					field.SetName (string.Format ("{0}.{1}", this.Name, i));
				}
				
				this.data = new FieldList (list);
				this.type = ResourceFieldType.List;
			}


			private ResourceBundle parent;
			private string name;
			private long id;
			private string about;
			private int modification_id;
			private System.Xml.XmlNode xml;
			private object data;
			private ResourceFieldType type;
			private ResourceLevel level;
		}
		#endregion

		public event EventHandler		FieldsChanged;
		
		private string					prefix;
		private ResourceModuleInfo		module;
		private string					name;
		private Druid					druid;
		private string					caption;
		private string					type;
		private string					about;
		private ResourceLevel			level;
		private CultureInfo				culture;
		private ResourceManager			manager;
		private int						rank = -1;
		
		private System.Xml.XmlNode		xmlroot;
		private int						depth;
		
		private Field[]					fields;
		private bool					refInclusion = true;
		private bool					autoMerge    = true;
	}
}
