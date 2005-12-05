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
		public StyleList(TextContext context)
		{
			this.context         = context;
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
			style.DefineIsFlagged (true);
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
			style.DefineIsFlagged (true);
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
			
			if (changes > 0)
			{
				this.UpdateTextStories ();
			}
			
			foreach (TextStyle style in this.text_style_list)
			{
				style.DefineIsFlagged (false);
			}
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
		
		private bool UpdateTextStories()
		{
			//	Met à jour tous les textes attaché au même TextContext que ce
			//	styliste, en regénérant toutes les propriétés pour les styles
			//	qui ont changé (qui retournent TextStyle.IsFlagged == true).
			
			Patcher patcher = new Patcher ();
			ulong[] buffer  = new ulong[1000];
			
			System.Collections.ArrayList dirty_stories = new System.Collections.ArrayList ();
			
			foreach (TextStory story in this.context.GetTextStories ())
			{
				int  length = story.TextLength;
				bool update = false;
				
				if (length > 0)
				{
					Cursors.TempCursor cursor = new Cursors.TempCursor ();
					
					story.NewCursor (cursor);
					
					try
					{
						while (length > 0)
						{
							int count = System.Math.Min (length, buffer.Length);
							
							story.ReadText (cursor, count, buffer);
							
							if (this.UpdateTextBuffer (story, patcher, buffer, count))
							{
								if (count == buffer.Length)
								{
									story.WriteText (cursor, buffer);
								}
								else
								{
									ulong[] copy = new ulong[count];
									System.Array.Copy (buffer, 0, copy, 0, count);
									story.WriteText (cursor, copy);
								}
								update = true;
							}
							
							story.MoveCursor (cursor, count);
							
							length -= count;
						}
					}
					finally
					{
						story.RecycleCursor (cursor);
					}
				}
				
				if (update)
				{
					dirty_stories.Add (story);
				}
			}
			
			//	Signale encore que les diverses instances de TextStory ont été
			//	modifiées :
			
			foreach (TextStory story in dirty_stories)
			{
				story.NotifyTextChanged ();
			}
			
			return dirty_stories.Count > 0;
		}
		
		private bool UpdateTextBuffer(TextStory story, Patcher patcher, ulong[] buffer, int length)
		{
			bool update = false;
			
			if (length == 0)
			{
				return update;
			}
			
			ulong last = Internal.CharMarker.ExtractStyleAndSettings (buffer[0]);
			int   num  = 1;
			
			for (int i = 1; i < length; i++)
			{
				ulong code = Internal.CharMarker.ExtractStyleAndSettings (buffer[i]);
				
				if (code != last)
				{
					update |= this.UpdateTextRun (story, patcher, buffer, last, i-num, num);
					
					num  = 1;
					last = code;
				}
				else
				{
					num += 1;
				}
			}
			
			update |= this.UpdateTextRun (story, patcher, buffer, last, length-num, num);
			
			return update;
		}
		
		private bool UpdateTextRun(TextStory story, Patcher patcher, ulong[] buffer, ulong code, int pos, int length)
		{
			//	La tranche définie par 'pos' et 'length' est définie avec les
			//	mêmes propriétés. On peut donc procéder au même remplacement
			//	partout :
			
			ulong replacement;
			
			if (patcher.FindReplacement (code, out replacement) == false)
			{
				TextStyle[] styles;
				Property[]  properties;
				
				this.context.GetStylesAndProperties (code, out styles, out properties);
				
				bool is_flagged = false;
				
				for (int i = 0; i < styles.Length; i++)
				{
					if (styles[i].IsFlagged)
					{
						is_flagged = true;
						break;
					}
				}
				
				if (is_flagged)
				{
					//	Trouve le remplacement adéquat pour les styles et propriétés
					//	définies pour l'ancien 'code' :
					
					story.ConvertToStyledText (story.FlattenStylesAndProperties (styles, properties), out replacement);
				}
				else
				{
					//	Aucun des styles composant ce code n'a été modifié. Il n'est
					//	donc pas nécessaire de remplacer quoi que ce soit :
					
					replacement = code;
				}
				
				patcher.DefineReplacement (code, replacement);
			}
			
			int end = pos + length;
			
			//	Le remplacement se fait par une série d'opérations XOR, ce qui
			//	évite de devoir masquer et combiner les bits :
			//
			//	nouveau-car = ancien-car XOR ancien-code XOR nouveau-code
			//
			//	Note: si ancien-code == nouveau-code, le XOR des deux donne zéro
			//	et cela implique qu'il n'y aura aucune modification.
			
			replacement ^= code;
			
			if (replacement != 0)
			{
				for (int i = pos; i < end; i++)
				{
					buffer[i] ^= replacement;
				}
				
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
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
		
		
		#region Patcher Class
		private class Patcher
		{
			public Patcher()
			{
				this.hash = new System.Collections.Hashtable ();
			}
			
			
			public bool FindReplacement(ulong code, out ulong replacement)
			{
				object data = this.hash[code];
				
				if (data == null)
				{
					replacement = 0;
					return false;
				}
				else
				{
					replacement = (ulong) data;
					return true;
				}
			}
			
			public void DefineReplacement(ulong code, ulong replacement)
			{
				this.hash[code] = replacement;
			}
			
			
			System.Collections.Hashtable		hash;
		}
		#endregion
		
		private TextContext						context;
		private Internal.StyleTable				internal_styles;
		private System.Collections.ArrayList	text_style_list;
		private System.Collections.Hashtable	text_style_hash;
		private StyleMap						style_map;
		private long							unique_id;
		private StyleVersion					version = new StyleVersion ();
		private long							version_of_last_update;
	}
}
