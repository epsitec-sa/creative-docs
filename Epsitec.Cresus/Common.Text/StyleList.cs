//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	using EventHandler = Common.Support.EventHandler;
	
	/// <summary>
	/// La classe StyleList gère la liste des styles associés à un ou plusieurs
	/// textes.
	/// Note: "StyleList" se prononce comme "stylist" :-)
	/// </summary>
	public sealed class StyleList
	{
		public StyleList(TextContext context)
		{
			this.context           = context;
			this.internal_settings = new Internal.SettingsTable ();
			
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
		
		public TextContext						TextContext
		{
			get
			{
				return this.context;
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
		internal Internal.SettingsTable			InternalSettingsTable
		{
			get
			{
				return this.internal_settings;
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
		
		internal Styles.CoreSettings			this[ulong code]
		{
			get
			{
				return this.internal_settings.GetCore (code);
			}
		}
		#endregion
		
		public TextStyle NewTextStyle(Common.Support.OpletQueue queue, string name, TextStyleClass text_style_class)
		{
			return this.NewTextStyle (queue, name, text_style_class, new Property[0], null);
		}
		
		public TextStyle NewTextStyle(Common.Support.OpletQueue queue, string name, TextStyleClass text_style_class, params Property[] properties)
		{
			return this.NewTextStyle (queue, name, text_style_class, properties, null);
		}
		
		public TextStyle NewTextStyle(Common.Support.OpletQueue queue, string name, TextStyleClass text_style_class, System.Collections.ICollection properties)
		{
			return this.NewTextStyle (queue, name, text_style_class, properties, null);
		}
		
		public TextStyle NewTextStyle(Common.Support.OpletQueue queue, string name, TextStyleClass text_style_class, System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			if (name == null)
			{
				name = this.GenerateUniqueName ();
			}
			
			TextStyle style = new TextStyle (name, text_style_class, properties, parent_styles);
			
			this.Attach (style);
			this.UpdateTabListUserCount (style, 1);
			this.UpdateGeneratorUserCount (style, 1);
			
			if (queue != null)
			{
				System.Diagnostics.Debug.Assert (queue.IsActionDefinitionInProgress);
				TextStory.InsertOplet (queue, new NewOplet (this, style));
			}
			
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
				name = this.GenerateUniqueName ();
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
				this.RecycleTextStyle (temp);
				
				return find[0];
			}
			else
			{
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
				this.RecycleTextStyle (temp);
				
				return find[0];
			}
			else
			{
				return temp;
			}
		}
		
		
		public void RedefineTextStyle(Common.Support.OpletQueue queue, TextStyle style, System.Collections.ICollection properties)
		{
			//	Change les propriétés d'un style. Le style devient un style "plat"
			//	indépendant d'autres styles.
			
			this.RedefineTextStyle (queue, style, properties, null);
		}
		
		public void RedefineTextStyle(Common.Support.OpletQueue queue, TextStyle style, System.Collections.ICollection properties, System.Collections.ICollection parent_styles)
		{
			//	Change les propriétés et les parents d'un style. Le style devient
			//	un style dérivé.
			
			System.Diagnostics.Debug.Assert (this.text_style_list.Contains (style));
			
			if (queue != null)
			{
				System.Diagnostics.Debug.Assert (queue.IsActionDefinitionInProgress);
				TextStory.InsertOplet (queue, new RedefineOplet (this, style));
			}
			
			this.PreRedefine (style);
			style.Initialize (properties, parent_styles);
			this.PostRedefine (style);
		}
		
		
		public void DeleteTextStyle(Common.Support.OpletQueue queue, TextStyle style)
		{
			//	Supprime un style : le style est vidé, ses parents remplacés par le style
			//	par défaut correspondant à la classe du style et le style marqué "deleted".
			
			System.Diagnostics.Debug.Assert (style != null);
			System.Diagnostics.Debug.Assert (style.IsDeleted == false);
			
			TextStyle default_style = this.GetDefaultTextStyle (style);
			
			if (queue != null)
			{
				System.Diagnostics.Debug.Assert (queue.IsActionDefinitionInProgress);
			}
			
			TextStory.InsertOplet (queue, new DeleteOplet (this, style));
			
			style.IsDeleted = true;
			this.RedefineTextStyle (queue, style, new Property[0], new TextStyle[1] { default_style } );
		}
		
		public void SetNextStyle(Common.Support.OpletQueue queue, TextStyle style, TextStyle next_style)
		{
			if (style.NextStyle == next_style)
			{
				return;
			}
			
			if (queue != null)
			{
				System.Diagnostics.Debug.Assert (queue.IsActionDefinitionInProgress);
				TextStory.InsertOplet (queue, new SetNextStyleOplet (this, style));
			}
			
			if (next_style == null)
			{
				style.DefineNextStyle (next_style);
			}
			else
			{
				System.Diagnostics.Debug.Assert (style.TextStyleClass == next_style.TextStyleClass);
				style.DefineNextStyle (next_style);
			}
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
		
		
		public bool IsDefaultParagraphTextStyle(TextStyle style)
		{
			return (this.context.DefaultParagraphStyle == style);
		}
		
		public bool IsDefaultTextTextStyle(TextStyle style)
		{
			return (this.context.DefaultTextStyle == style);
		}
		
		public bool IsDeletedTextStyle(TextStyle style)
		{
			if (style != null)
			{
				return style.IsDeleted;
			}
			
			return false;
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
		
		internal TextStyle GetDefaultTextStyle(TextStyle style)
		{
			TextStyle default_style = null;
			
			switch (style.TextStyleClass)
			{
				case TextStyleClass.Text:		default_style = this.TextContext.DefaultTextStyle;		break;
				case TextStyleClass.Paragraph:	default_style = this.TextContext.DefaultParagraphStyle;	break;
					
				default:
					throw new System.NotSupportedException (string.Format ("Cannot delete style {0} of class {1}", style.Name, style.TextStyleClass));
			}
			
			System.Diagnostics.Debug.Assert (default_style != null);
			
			return default_style;
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
			int n = this.text_style_list.Count;
			
			for (int i = 0; i < this.text_style_list.Count; i++)
			{
				TextStyle style = this.text_style_list[i] as TextStyle;
				
				if (style.IsDeleted)
				{
					n--;
				}
			}
			
			buffer.Append (SerializerSupport.SerializeLong (this.unique_id));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (n));
			buffer.Append ("/");
			
			this.internal_settings.Serialize (buffer);
			buffer.Append ("/");
			
			for (int i = 0; i < this.text_style_list.Count; i++)
			{
				TextStyle style = this.text_style_list[i] as TextStyle;
				
				if (style.IsDeleted == false)
				{
					style.Serialize (buffer);
					buffer.Append ("/");
				}
			}
			
			this.StyleMap.Serialize (buffer);
		}
		
		public void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			this.internal_settings = new Internal.SettingsTable ();
			this.text_style_list   = new System.Collections.ArrayList ();
			this.text_style_hash   = new System.Collections.Hashtable ();
			
			long unique   = SerializerSupport.DeserializeLong (args[offset++]);
			int  n_styles = SerializerSupport.DeserializeInt (args[offset++]);
			
			this.unique_id = unique;
			this.internal_settings.Deserialize (context, version, args, ref offset);
			
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
		
		
		public string GenerateUniqueName()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "#ID#{0}", this.GenerateUniqueId ());
		}
		
		public long GenerateUniqueId()
		{
			lock (this.unique_id_lock)
			{
				return this.unique_id++;
			}
		}
		
		
		public static bool IsAutomaticName(string value)
		{
			//	Retourne true si un nom a été généré avec GenerateUniqueName
			//	(c'est un nom "automatique" par opposition avec un nom qui est
			//	défini à la main).
			
			if ((value != null) &&
				(value.Length > 4) &&
				(value.StartsWith ("#ID#")))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		#region Internal Methods
		internal Styles.CoreSettings GetCoreFromIndex(int index)
		{
			return this.internal_settings.GetCoreFromIndex (index);
		}
		
		internal void NotifyStyleMapChanged()
		{
			this.OnStyleMapChanged ();
		}
		
		
		internal Property[] Flatten(ulong code)
		{
			Styles.CoreSettings core_settings = this[code];
			
			if (core_settings != null)
			{
				return core_settings.Flatten (code);
			}
			else
			{
				return new Property[0];
			}
		}
		
		
		internal void UpdateTabListUserCount()
		{
			foreach (TextStyle style in this.text_style_list)
			{
				this.UpdateTabListUserCount (style, 1);
			}
		}
		
		internal void UpdateGeneratorUserCount()
		{
			foreach (TextStyle style in this.text_style_list)
			{
				this.UpdateGeneratorUserCount (style, 1);
			}
		}
		
		internal void UpdateTabListUserCount(TextStyle style, int sign)
		{
			Properties.TabsProperty tabs = style[Properties.WellKnownType.Tabs] as Properties.TabsProperty;
			Properties.ManagedParagraphProperty mpp = style[Properties.WellKnownType.ManagedParagraph] as Properties.ManagedParagraphProperty;
			
			TabList list = this.context.TabList;
			
			//	Tient compte des tabulateurs définis directement dans le style :
			
			if (tabs != null)
			{
				foreach (string tag in tabs.TabTags)
				{
					if (sign > 0)
					{
						list.IncrementTabUserCount (tag);
					}
					else if (sign < 0)
					{
						list.DecrementTabUserCount (tag);
					}
				}
			}
			
			//	Tient compte des tabulateurs utilisés par des managed paragraphs :
			
			if (mpp != null)
			{
				string   manager_name = mpp.ManagerName;
				string[] parameters   = mpp.ManagerParameters;
				
				switch (manager_name)
				{
					case "ItemList":
						ParagraphManagers.ItemListManager.Parameters p = new Epsitec.Common.Text.ParagraphManagers.ItemListManager.Parameters (this.context, parameters);
						
						if (p.TabItem != null)
						{
							if (sign > 0)
							{
								list.IncrementTabUserCount (p.TabItem.TabTag);
							}
							else if (sign < 0)
							{
								list.DecrementTabUserCount (p.TabItem.TabTag);
							}
						}
						
						if (p.TabBody != null)
						{
							if (sign > 0)
							{
								list.IncrementTabUserCount (p.TabBody.TabTag);
							}
							else if (sign < 0)
							{
								list.DecrementTabUserCount (p.TabBody.TabTag);
							}
						}
						
						break;
				}
			}
		}
		
		internal void UpdateGeneratorUserCount(TextStyle style, int sign)
		{
			Properties.ManagedParagraphProperty mpp = style[Properties.WellKnownType.ManagedParagraph] as Properties.ManagedParagraphProperty;
			
			GeneratorList list = this.context.GeneratorList;
			
			//	Tient compte des générateurs utilisés par des managed paragraphs :
			
			if (mpp != null)
			{
				string   manager_name = mpp.ManagerName;
				string[] parameters   = mpp.ManagerParameters;
				
				switch (manager_name)
				{
					case "ItemList":
						ParagraphManagers.ItemListManager.Parameters p = new ParagraphManagers.ItemListManager.Parameters (this.context, parameters);
						
						if (p.Generator != null)
						{
							if (sign > 0)
							{
								list.IncrementUserCount (p.Generator.Name);
							}
							else if (sign < 0)
							{
								list.DecrementUserCount (p.Generator.Name);
							}
						}
						
						break;
				}
			}
		}
		
		internal static string GetFullName(TextStyle text_style)
		{
			return StyleList.GetFullName (text_style.Name, text_style.TextStyleClass);
		}
		
		internal static string GetFullName(string name, TextStyleClass text_style_class)
		{
			switch (text_style_class)
			{
				case TextStyleClass.Abstract:		return string.Concat ("A.", name);
				case TextStyleClass.Paragraph:		return string.Concat ("P.", name);
				case TextStyleClass.Text:			return string.Concat ("T.", name);
				case TextStyleClass.Symbol:			return string.Concat ("S.", name);
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
				case 'S': text_style_class = TextStyleClass.Symbol;			break;
				case 'M': text_style_class = TextStyleClass.MetaProperty;	break;
				
				default:
					throw new System.ArgumentException ();
			}
			
			name = full_name.Substring (2);
		}
		#endregion
		
		private void PreRedefine(TextStyle style)
		{
			this.version.ChangeVersion ();
			
			TextStyle next = style.NextStyle;
			
			this.UpdateTabListUserCount (style, -1);
			this.UpdateGeneratorUserCount (style, -1);
			
			style.Clear ();
			style.DefineNextStyle (next);
		}
		
		private void PostRedefine(TextStyle style)
		{
			style.DefineIsFlagged (true);
			
			this.UpdateTabListUserCount (style, 1);
			this.UpdateGeneratorUserCount (style, 1);
			
			this.OnStyleRedefined ();
		}
		
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
					
					story.SuspendTextChanged ();
					story.NewCursor (cursor);
					story.DisableOpletQueue ();
					
					int pos = 0;
					
					this.is_patcher_on_break     = true;
					this.pending_manager_patches = new System.Collections.Stack ();
					
					try
					{
						//	Passe en revue tout le texte et substitue les infos de
						//	formatage là où les styles ont changé :
						
						while (length > 0)
						{
							int count = System.Math.Min (length, buffer.Length);
							
							story.SetCursorPosition (cursor, pos);
							story.ReadText (cursor, count, buffer);
							
							if (this.UpdateTextBuffer (story, patcher, buffer, pos, count))
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
							
							pos    += count;
							length -= count;
						}
						
						//	S'il y a des "managed paragraphs" dans le texte qui a été
						//	mis à jour, il faut encore les passer en revue pour ajuster
						//	les textes automatiques.
						//
						//	On commence par la fin, ainsi les positions intermédiaires
						//	ne seront pas altérées par l'insertion ou la suppression de
						//	textes automatiques.
						
						while (this.pending_manager_patches.Count > 0)
						{
							ManagedParagraphPropertyInfo mppi = this.pending_manager_patches.Pop () as ManagedParagraphPropertyInfo;
							
							story.SetCursorPosition (cursor, mppi.Position);
							
							Properties.ManagedParagraphProperty[] mpp_old = (mppi.Mpp1 == null) ? new Properties.ManagedParagraphProperty[0] : new Properties.ManagedParagraphProperty[1] { mppi.Mpp1 };
							Properties.ManagedParagraphProperty[] mpp_new = (mppi.Mpp2 == null) ? new Properties.ManagedParagraphProperty[0] : new Properties.ManagedParagraphProperty[1] { mppi.Mpp2 };
							
							Internal.Navigator.HandleManagedParagraphPropertiesChange (story, cursor, 0, mpp_old, mpp_new);
						}
					}
					finally
					{
						story.EnableOpletQueue ();
						story.RecycleCursor (cursor);
						story.ResumeTextChanged ();
					}
					
					this.is_patcher_on_break     = false;
					this.pending_manager_patches = null;
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
		
		private bool UpdateTextBuffer(TextStory story, Patcher patcher, ulong[] buffer, int abs_pos, int length)
		{
			bool update = false;
			
			if (length == 0)
			{
				return update;
			}
			
			ulong last = Internal.CharMarker.ExtractCoreAndSettings (buffer[0]);
			int   num  = 1;
			
			for (int i = 1; i < length; i++)
			{
				ulong code = Internal.CharMarker.ExtractCoreAndSettings (buffer[i]);
				
				if (code != last)
				{
					update |= this.UpdateTextRun (story, patcher, buffer, last, abs_pos, i-num, num);
					
					num  = 1;
					last = code;
				}
				else
				{
					num += 1;
				}
			}
			
			update |= this.UpdateTextRun (story, patcher, buffer, last, abs_pos, length-num, num);
			
			return update;
		}
		
		private bool UpdateTextRun(TextStory story, Patcher patcher, ulong[] buffer, ulong code, int abs_pos, int pos, int length)
		{
			//	La tranche définie par 'pos' et 'length' est définie avec les
			//	mêmes propriétés. On peut donc procéder au même remplacement
			//	partout :
			
			ulong replacement;
			ManagedParagraphPropertyInfo mppi = patcher.FindManagedParagraphPropertyInfo (code);
			
			if (patcher.FindReplacement (code, out replacement) == false)
			{
				TextStyle[] styles;
				Property[]  properties;
				
				this.context.GetStylesAndProperties (code, out styles, out properties);
				
				bool is_flagged = false;
				
				//	Regarde si l'un des styles appliqués au "run" actuel a été marqué
				//	comme modifié. Ce n'est que dans ce cas qu'une substitution sera
				//	nécessaire :
				
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
				
				if (replacement != code)
				{
					//	Si les propriétés sont remplacées, regarde encore s'il y a un
					//	changement de "manged paragraph" qui s'en suit :
					
					Properties.ManagedParagraphProperty mpp1;
					Properties.ManagedParagraphProperty mpp2;
					
					this.context.GetManagedParagraph (code, out mpp1);
					this.context.GetManagedParagraph (replacement, out mpp2);
					
					if (Properties.ManagedParagraphProperty.CompareEqualContents (mpp1, mpp2) == false)
					{
						mppi = new ManagedParagraphPropertyInfo (mpp1, mpp2);
						
						patcher.DefineManagedParagraphPropertyInfo (code, mppi);
					}
				}
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
					if (this.is_patcher_on_break)
					{
						//	Au début de chaque paragraphe, vérifie si nous avons un
						//	changement de "managed paragraph". Si c'est le cas, il
						//	faut prendre note de la position pour pouvoir y revenir
						//	plus tard pour faire les attach/detach nécessaires sur
						//	les IParagraphManager :
						
						if (mppi != null)
						{
							mppi = new ManagedParagraphPropertyInfo (mppi, abs_pos+i);
							
							this.pending_manager_patches.Push (mppi);
						}
					}
					
					buffer[i] ^= replacement;
					
					//	Regarde encore si l'on se trouve à une fin de paragraphe;
					//	c'est utile pour la suite :
					
					this.is_patcher_on_break = Internal.Navigator.IsParagraphSeparator (Unicode.Bits.GetUnicodeCode (buffer[i]));
				}
				
				return true;
			}
			else
			{
				//	Aucune modification n'a été nécessaire. Vérifie encore si le
				//	"run" se termine par une fin de paragraphe :
				
				this.is_patcher_on_break = Internal.Navigator.IsParagraphSeparator (Unicode.Bits.GetUnicodeCode (buffer[end-1]));
				
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
			
			this.OnStyleAdded ();
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
				this.mppi = new System.Collections.Hashtable ();
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
			
			public ManagedParagraphPropertyInfo FindManagedParagraphPropertyInfo(ulong code)
			{
				return this.mppi[code] as ManagedParagraphPropertyInfo;
			}
			
			public void DefineReplacement(ulong code, ulong replacement)
			{
				this.hash[code] = replacement;
			}
			
			public void DefineManagedParagraphPropertyInfo(ulong code, ManagedParagraphPropertyInfo mppi)
			{
				this.mppi[code] = mppi;
			}
			
			
			System.Collections.Hashtable		hash;
			System.Collections.Hashtable		mppi;
		}
		#endregion
		
		#region ManagedParagraphPropertyInfo Class
		private class ManagedParagraphPropertyInfo
		{
			//	Représente la transition d'une propriété Managed Paragraph à une
			//	autre; utilisé par la méthode de patch lors de mise à jour des
			//	propriétés associées à un style dans TextStory.
			
			public ManagedParagraphPropertyInfo(Properties.ManagedParagraphProperty mpp1, Properties.ManagedParagraphProperty mpp2)
			{
				this.mpp1 = mpp1;
				this.mpp2 = mpp2;
				this.pos  = -1;
			}
			
			public ManagedParagraphPropertyInfo(ManagedParagraphPropertyInfo mppi, int pos)
			{
				this.mpp1 = mppi.mpp1;
				this.mpp2 = mppi.mpp2;
				this.pos  = pos;
			}
			
			public Properties.ManagedParagraphProperty Mpp1
			{
				get
				{
					return this.mpp1;
				}
			}
			
			public Properties.ManagedParagraphProperty Mpp2
			{
				get
				{
					return this.mpp2;
				}
			}
			
			
			public int							Position
			{
				get
				{
					return this.pos;
				}
			}
			
			
			Properties.ManagedParagraphProperty	mpp1;
			Properties.ManagedParagraphProperty mpp2;
			private int							pos;
		}
		#endregion
		
		#region RedefineOplet Class
		public class RedefineOplet : Common.Support.AbstractOplet
		{
			public RedefineOplet(StyleList stylist, TextStyle style)
			{
				this.stylist = stylist;
				this.style   = style;
				this.state   = this.style.SaveState (this.stylist);
			}
			
			
			public override Common.Support.IOplet Undo()
			{
				string new_state = this.style.SaveState (this.stylist);
				string old_state = this.state;
				
				this.state = new_state;
				
				this.stylist.PreRedefine (this.style);
				this.style.RestoreState (this.stylist, old_state);
				this.stylist.PostRedefine (this.style);
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				return this.Undo ();
			}
			
			public override void Dispose()
			{
				base.Dispose ();
			}
			
			
			public bool MergeWith(RedefineOplet other)
			{
				if ((this.style   == other.style) &&
					(this.stylist == other.stylist))
				{
					return true;
				}
				
				return false;
			}
						
			
			private StyleList					stylist;
			private TextStyle					style;
			private string						state;
		}
		#endregion
		
		#region SetNextStyleOplet Class
		private class SetNextStyleOplet : Common.Support.AbstractOplet
		{
			public SetNextStyleOplet(StyleList stylist, TextStyle style)
			{
				this.stylist = stylist;
				this.style   = style;
				this.next    = this.style.NextStyle;
			}
			

			public override Common.Support.IOplet Undo()
			{
				TextStyle new_next = this.next;
				TextStyle old_next = this.style.NextStyle;
				
				this.style.DefineNextStyle (new_next);
				
				this.next = old_next;
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				return this.Undo ();
			}
			
			
			private StyleList					stylist;
			private TextStyle					style;
			private TextStyle					next;
		}
		#endregion
		
		#region DeleteOplet Class
		private class DeleteOplet : Common.Support.AbstractOplet
		{
			public DeleteOplet(StyleList stylist, TextStyle style)
			{
				this.stylist = stylist;
				this.style   = style;
			}
			

			public override Common.Support.IOplet Undo()
			{
				this.style.IsDeleted = false;
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				this.style.IsDeleted = true;
				
				return this;
			}
			
			
			private StyleList					stylist;
			private TextStyle					style;
		}
		#endregion
		
		#region NewOplet Class
		private class NewOplet : Common.Support.AbstractOplet
		{
			public NewOplet(StyleList stylist, TextStyle style)
			{
				this.stylist = stylist;
				this.style   = style;
			}
			

			public override Common.Support.IOplet Undo()
			{
				this.state = this.style.SaveState (this.stylist);
				
				TextStyle default_style = this.stylist.GetDefaultTextStyle (this.style);
				
				this.style.IsDeleted = true;
				this.stylist.RedefineTextStyle (null, this.style, new Property[0], new TextStyle[1] { default_style } );
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				this.stylist.PreRedefine (this.style);
				this.style.RestoreState (this.stylist, this.state);
				this.stylist.PostRedefine (this.style);
				
				this.style.IsDeleted = false;
				
				return this;
			}
			
			
			private StyleList					stylist;
			private TextStyle					style;
			private string						state;
		}
		#endregion
		
		private void OnStyleRedefined()
		{
			if (this.StyleRedefined != null)
			{
				this.StyleRedefined (this);
			}
		}
		
		private void OnStyleAdded()
		{
			if (this.StyleAdded != null)
			{
				this.StyleAdded (this);
			}
		}
		
		private void OnStyleRemoved()
		{
			if (this.StyleRemoved != null)
			{
				this.StyleRemoved (this);
			}
		}
		
		private void OnStyleMapChanged()
		{
			if (this.StyleMapChanged != null)
			{
				this.StyleMapChanged (this);
			}
		}
		
		
		public event EventHandler				StyleRedefined;
		public event EventHandler				StyleAdded;
		public event EventHandler				StyleRemoved;
		public event EventHandler				StyleMapChanged;
		
		private TextContext						context;
		private Internal.SettingsTable			internal_settings;
		private System.Collections.ArrayList	text_style_list;
		private System.Collections.Hashtable	text_style_hash;
		private StyleMap						style_map;
		private long							unique_id;
		private object							unique_id_lock = new object ();
		private StyleVersion					version = new StyleVersion ();
		private long							version_of_last_update;
		
		private bool							is_patcher_on_break;
		private System.Collections.Stack		pending_manager_patches;
	}
}
