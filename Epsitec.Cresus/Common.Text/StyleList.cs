//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe StyleList gère la liste des styles associés à un ou plusieurs
	/// textes.
	/// Note: "StyleList" se prononce comme "stylist" :-)
	/// </summary>
	public sealed class StyleList
	{
		public StyleList()
		{
			this.internal_styles = new Internal.StyleTable ();
			
			this.text_style_list = new System.Collections.ArrayList ();
			this.text_style_hash = new System.Collections.Hashtable ();
		}
		
		
		public long								Version
		{
			get
			{
				return this.version.Current;
			}
		}
		
		private StyleVersion					StyleVersion
		{
			get
			{
				return this.version;
			}
		}

		public StyleMap							StyleMap
		{
			get
			{
				if (this.style_map == null)
				{
					this.style_map = new StyleMap (this);
				}
				
				return this.style_map;
			}
		}
		
		
		public TextStyle						this[string name, TextStyleClass text_style_class]
		{
			get
			{
				return this.GetTextStyle (name, text_style_class);
			}
		}
		
		
		#region Internal Properties
		internal Internal.StyleTable			InternalStyleTable
		{
			get
			{
				return this.internal_styles;
			}
		}
		
		internal int							InternalStyleCount
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.text_style_list.Count == this.text_style_hash.Count);
				
				return this.text_style_list.Count;
			}
		}
		
		internal Styles.SimpleStyle				this[ulong code]
		{
			get
			{
				return this.internal_styles.GetStyle (code);
			}
		}
		#endregion
		
		public TextStyle NewTextStyle(string name, TextStyleClass text_style_class)
		{
			return this.NewTextStyle (name, text_style_class, new Property[0], null);
		}
		
		public TextStyle NewTextStyle(string name, TextStyleClass text_style_class, params Property[] properties)
		{
			return this.NewTextStyle (name, text_style_class, properties, null);
		}
		
		public TextStyle NewTextStyle(string name, TextStyleClass text_style_class, System.Collections.ICollection properties)
		{
			return this.NewTextStyle (name, text_style_class, properties, null);
		}
		
		public TextStyle NewTextStyle(string name, TextStyleClass text_style_class, System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			if (name == null)
			{
				name = this.GetUniqueName ();
			}
			
			TextStyle style = new TextStyle (name, text_style_class, properties, parent_styles);
			
			this.Attach (style);
			
			return style;
		}
		
		
		public TextStyle NewMetaProperty(string name, string meta_id, params Property[] properties)
		{
			return this.NewMetaProperty (name, meta_id, 0, properties, null);
		}
		
		public TextStyle NewMetaProperty(string name, string meta_id, int priority, params Property[] properties)
		{
			return this.NewMetaProperty (name, meta_id, priority, properties, null);
		}
		
		public TextStyle NewMetaProperty(string name, string meta_id, int priority, System.Collections.ICollection properties)
		{
			return this.NewMetaProperty (name, meta_id, priority, properties, null);
		}
		
		public TextStyle NewMetaProperty(string name, string meta_id, int priority, System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			if (name == null)
			{
				name = this.GetUniqueName ();
			}
			
			TextStyle style = new TextStyle (name, TextStyleClass.MetaProperty, properties, parent_styles);
			
			style.DefineMetaId (meta_id);
			style.DefinePriority (priority);
			
			this.Attach (style);
			
			return style;
		}
		
		
		public TextStyle CreateOrGetMetaProperty(string meta_id, params Property[] properties)
		{
			//	Crée ou réutilise une méta propriété déjà existante s'il en existe
			//	une qui soit 100% équivalente à celle demandée.
			
			TextStyle   temp = this.NewMetaProperty (null, meta_id, properties);
			TextStyle[] find = this.FindEqualTextStyles (temp);
			
			if (find.Length > 0)
			{
				System.Diagnostics.Debug.WriteLine ("Reusing meta property : " + find[0].ToString ());
				
				this.RecycleTextStyle (temp);
				
				return find[0];
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Created meta property : " + temp.ToString ());
				
				return temp;
			}
		}
		
		public TextStyle CreateOrGetMetaProperty(string meta_id, int priority, params Property[] properties)
		{
			//	Crée ou réutilise une méta propriété déjà existante s'il en existe
			//	une qui soit 100% équivalente à celle demandée.
			
			TextStyle   temp = this.NewMetaProperty (null, meta_id, priority, properties);
			TextStyle[] find = this.FindEqualTextStyles (temp);
			
			if (find.Length > 0)
			{
				System.Diagnostics.Debug.WriteLine ("Reusing meta property : " + find[0].ToString ());
				
				this.RecycleTextStyle (temp);
				
				return find[0];
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Created meta property : " + temp.ToString ());
				
				return temp;
			}
		}
		
		
		public void RedefineTextStyle(TextStyle style, System.Collections.ICollection properties)
		{
			//	Change les propriétés d'un style. Le style devient un style "plat"
			//	indépendant d'autres styles.
			
			System.Diagnostics.Debug.Assert (this.text_style_list.Contains (style));
			
			this.version.ChangeVersion ();
			
			style.Clear ();
			style.Initialise (properties);
		}
		
		public void RedefineTextStyle(TextStyle style, System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			//	Change les propriétés et les parents d'un style. Le style devient
			//	un style dérivé.
			
			System.Diagnostics.Debug.Assert (this.text_style_list.Contains (style));
			System.Diagnostics.Debug.Assert (parent_styles != null);
			System.Diagnostics.Debug.Assert (parent_styles.Count > 0);
			
			this.version.ChangeVersion ();
			
			style.Clear ();
			style.Initialise (properties, parent_styles);
		}
		
		
		public void RecycleTextStyle(TextStyle style)
		{
			this.Detach (style);
		}
		
		
		public void UpdateTextStyles()
		{
			//	Met à jour tous les styles, à la suite d'éventuels changements.
			//	Ne fait rien si aucune modification n'a eu lieu depuis le der-
			//	nier appel à UpdateTextStyles.
			
			long version = this.Version;
			int  changes = 0;
			
			if (version == this.version_of_last_update)
			{
				return;
			}
			
			this.version_of_last_update = version;
			
			foreach (TextStyle style in this.text_style_list)
			{
				if (style.Update (version))
				{
					changes++;
				}
			}
			
			//	TODO: en cas de changements, resynchronise TextStory.
		}
		
		
		public TextStyle GetTextStyle(string name, TextStyleClass text_style_class)
		{
			return this.GetTextStyle (StyleList.GetFullName (name, text_style_class));
		}
		
		internal TextStyle GetTextStyle(string full_name)
		{
			if (this.text_style_hash.Contains (full_name))
			{
				return this.text_style_hash[full_name] as TextStyle;
			}
			else
			{
				return null;
			}
		}
		
		
		public TextStyle[] FindEqualTextStyles(TextStyle style)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			int signature = style.GetContentsSignature ();
			
			foreach (TextStyle candidate in this.text_style_list)
			{
				if (candidate != style)
				{
					if (candidate.GetContentsSignature () == signature)
					{
						//	Candidat intéressant; la signature correspond déjà. Il faut
						//	procéder à une comparaison détaillée pour vérifier s'il est
						//	bien complètement identique au style recherché :
						
						if (candidate.CompareEqualContents (style))
						{
							list.Add (candidate);
						}
					}
				}
			}
			
			return (TextStyle[]) list.ToArray (typeof (TextStyle));
		}
		
		
		public void Serialize(System.Text.StringBuilder buffer)
		{
			buffer.Append (SerializerSupport.SerializeLong (this.unique_id));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (this.text_style_list.Count));
			buffer.Append ("/");
			
			this.internal_styles.Serialize (buffer);
			buffer.Append ("/");
			
			for (int i = 0; i < this.text_style_list.Count; i++)
			{
				TextStyle style = this.text_style_list[i] as TextStyle;
				
				style.Serialize (buffer);
				buffer.Append ("/");
			}
			
			this.StyleMap.Serialize (buffer);
		}
		
		public void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			this.internal_styles = new Internal.StyleTable ();
			this.text_style_list = new System.Collections.ArrayList ();
			this.text_style_hash = new System.Collections.Hashtable ();
			
			long unique   = SerializerSupport.DeserializeLong (args[offset++]);
			int  n_styles = SerializerSupport.DeserializeInt (args[offset++]);
			
			this.unique_id = unique;
			this.internal_styles.Deserialize (context, version, args, ref offset);
			
			for (int i = 0; i < n_styles; i++)
			{
				TextStyle style = new TextStyle ();
				
				style.Deserialize (context, version, args, ref offset);
				
				this.Attach (style);
			}
			
			foreach (TextStyle style in this.text_style_list)
			{
				style.DeserializeFixups (this);
			}
			
			this.StyleMap.Deserialize (context, version, args, ref offset);
		}
		
		
		public string GetUniqueName()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "#ID#{0}", this.GetUniqueId ());
		}
		
		public long GetUniqueId()
		{
			lock (this)
			{
				return this.unique_id++;
			}
		}
		
		
		#region Internal Methods
		internal Styles.SimpleStyle GetStyleFromIndex(int index)
		{
			return this.internal_styles.GetStyleFromIndex (index);
		}
		
		
		internal Property[] Flatten(ulong code)
		{
			Styles.SimpleStyle style = this[code];
			
			if (style != null)
			{
				return style.Flatten (code);
			}
			else
			{
				return new Property[0];
			}
		}
		
		
		internal static string GetFullName(string name, TextStyleClass text_style_class)
		{
			switch (text_style_class)
			{
				case TextStyleClass.Abstract:		return string.Concat ("A.", name);
				case TextStyleClass.Paragraph:		return string.Concat ("P.", name);
				case TextStyleClass.Text:			return string.Concat ("T.", name);
				case TextStyleClass.Character:		return string.Concat ("C.", name);
				case TextStyleClass.MetaProperty:	return string.Concat ("M.", name);
				
				default:
					throw new System.ArgumentException ();
			}
		}
		
		internal static void SplitFullName(string full_name, out string name, out TextStyleClass text_style_class)
		{
			if ((full_name.Length < 2) ||
				(full_name[1] != '.'))
			{
				throw new System.ArgumentException ();
			}
			
			char prefix = full_name[0];
			
			switch (prefix)
			{
				case 'A': text_style_class = TextStyleClass.Abstract;		break;
				case 'P': text_style_class = TextStyleClass.Paragraph;		break;
				case 'T': text_style_class = TextStyleClass.Text;			break;
				case 'C': text_style_class = TextStyleClass.Character;		break;
				case 'M': text_style_class = TextStyleClass.MetaProperty;	break;
				
				default:
					throw new System.ArgumentException ();
			}
			
			name = full_name.Substring (2);
		}
		#endregion
		
		private void Attach(TextStyle style)
		{
			string         name             = style.Name;
			TextStyleClass text_style_class = style.TextStyleClass;
			string         full_name        = StyleList.GetFullName (name, text_style_class);
			
			if (this.text_style_hash.Contains (full_name))
			{
				throw new System.ArgumentException (string.Format ("TextStyle named {0} ({1}) already exists", name, text_style_class), "style");
			}
			
			this.text_style_list.Add (style);
			this.text_style_hash[full_name] = style;
		}
		
		private void Detach(TextStyle style)
		{
			string         name             = style.Name;
			TextStyleClass text_style_class = style.TextStyleClass;
			string         full_name        = StyleList.GetFullName (name, text_style_class);
			
			if (this.text_style_hash.Contains (full_name))
			{
				this.text_style_list.Remove (style);
				this.text_style_hash.Remove (full_name);
			}
			else
			{
				throw new System.ArgumentException (string.Format ("TextStyle named {0} ({1}) does not exist", name, text_style_class), "style");
			}
		}
		
		
		private Internal.StyleTable				internal_styles;
		private System.Collections.ArrayList	text_style_list;
		private System.Collections.Hashtable	text_style_hash;
		private StyleMap						style_map;
		private long							unique_id;
		private StyleVersion					version = new StyleVersion ();
		private long							version_of_last_update;
	}
}
