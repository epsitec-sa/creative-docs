//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.internalSettings  = new Internal.SettingsTable ();
			
			this.textStyleList = new System.Collections.ArrayList ();
			this.textStyleHash = new System.Collections.Hashtable ();
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
				if (this.styleMap == null)
				{
					this.styleMap = new StyleMap (this);
				}
				
				return this.styleMap;
			}
		}
		
		public TextContext						TextContext
		{
			get
			{
				return this.context;
			}
		}
		
		
		public TextStyle						this[string name, TextStyleClass textStyleClass]
		{
			get
			{
				return this.GetTextStyle (name, textStyleClass);
			}
		}
		
		
		#region Internal Properties
		internal Internal.SettingsTable			InternalSettingsTable
		{
			get
			{
				return this.internalSettings;
			}
		}
		
		internal int							InternalStyleCount
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.textStyleList.Count == this.textStyleHash.Count);
				
				return this.textStyleList.Count;
			}
		}
		
		internal Styles.CoreSettings			this[ulong code]
		{
			get
			{
				return this.internalSettings.GetCore (code);
			}
		}
		#endregion
		
		public TextStyle NewTextStyle(Common.Support.OpletQueue queue, string name, TextStyleClass textStyleClass)
		{
			return this.NewTextStyle (queue, name, textStyleClass, new Property[0], null);
		}
		
		public TextStyle NewTextStyle(Common.Support.OpletQueue queue, string name, TextStyleClass textStyleClass, params Property[] properties)
		{
			return this.NewTextStyle (queue, name, textStyleClass, properties, null);
		}
		
		public TextStyle NewTextStyle(Common.Support.OpletQueue queue, string name, TextStyleClass textStyleClass, System.Collections.ICollection properties)
		{
			return this.NewTextStyle (queue, name, textStyleClass, properties, null);
		}
		
		public TextStyle NewTextStyle(Common.Support.OpletQueue queue, string name, TextStyleClass textStyleClass, System.Collections.ICollection properties, System.Collections.ICollection parentStyles)
		{
			if (name == null)
			{
				name = this.GenerateUniqueName ();
			}
			
			TextStyle style = new TextStyle (name, textStyleClass, properties, parentStyles);
			
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
		
		
		public TextStyle NewMetaProperty(string name, string metaId, params Property[] properties)
		{
			return this.NewMetaProperty (name, metaId, 0, properties, null);
		}
		
		public TextStyle NewMetaProperty(string name, string metaId, int priority, params Property[] properties)
		{
			return this.NewMetaProperty (name, metaId, priority, properties, null);
		}
		
		public TextStyle NewMetaProperty(string name, string metaId, int priority, System.Collections.ICollection properties)
		{
			return this.NewMetaProperty (name, metaId, priority, properties, null);
		}
		
		public TextStyle NewMetaProperty(string name, string metaId, int priority, System.Collections.ICollection properties, System.Collections.ICollection parentStyles)
		{
			if (name == null)
			{
				name = this.GenerateUniqueName ();
			}
			
			TextStyle style = new TextStyle (name, TextStyleClass.MetaProperty, properties, parentStyles);
			
			style.DefineMetaId (metaId);
			style.DefinePriority (priority);
			
			this.Attach (style);
			
			return style;
		}
		
		
		public TextStyle CreateOrGetMetaProperty(string metaId, params Property[] properties)
		{
			//	Crée ou réutilise une méta propriété déjà existante s'il en existe
			//	une qui soit 100% équivalente à celle demandée.
			
			TextStyle   temp = this.NewMetaProperty (null, metaId, properties);
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
		
		public TextStyle CreateOrGetMetaProperty(string metaId, int priority, params Property[] properties)
		{
			//	Crée ou réutilise une méta propriété déjà existante s'il en existe
			//	une qui soit 100% équivalente à celle demandée.
			
			TextStyle   temp = this.NewMetaProperty (null, metaId, priority, properties);
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
		
		public void RedefineTextStyle(Common.Support.OpletQueue queue, TextStyle style, System.Collections.ICollection properties, System.Collections.ICollection parentStyles)
		{
			//	Change les propriétés et les parents d'un style. Le style devient
			//	un style dérivé.
			
			System.Diagnostics.Debug.Assert (this.textStyleList.Contains (style));
			
			if (queue != null)
			{
				System.Diagnostics.Debug.Assert (queue.IsActionDefinitionInProgress);
				TextStory.InsertOplet (queue, new RedefineOplet (this, style));
			}
			
			this.PreRedefine (style);
			style.Initialize (properties, parentStyles);
			this.PostRedefine (style);
		}
		
		
		public void DeleteTextStyle(Common.Support.OpletQueue queue, TextStyle style)
		{
			//	Supprime un style : le style est vidé, ses parents remplacés par le style
			//	par défaut correspondant à la classe du style et le style marqué "deleted".
			
			System.Diagnostics.Debug.Assert (style != null);
			System.Diagnostics.Debug.Assert (style.IsDeleted == false);
			
			TextStyle defaultStyle = this.GetDefaultTextStyle (style);
			
			if (queue != null)
			{
				System.Diagnostics.Debug.Assert (queue.IsActionDefinitionInProgress);
			}
			
			TextStory.InsertOplet (queue, new DeleteOplet (this, style));
			
			style.IsDeleted = true;
			this.RedefineTextStyle (queue, style, new Property[0], new TextStyle[1] { defaultStyle } );
		}
		
		public void SetNextStyle(Common.Support.OpletQueue queue, TextStyle style, TextStyle nextStyle)
		{
			if (style.NextStyle == nextStyle)
			{
				return;
			}
			
			if (queue != null)
			{
				System.Diagnostics.Debug.Assert (queue.IsActionDefinitionInProgress);
				TextStory.InsertOplet (queue, new SetNextStyleOplet (this, style));
			}
			
			if (nextStyle == null)
			{
				style.DefineNextStyle (nextStyle);
			}
			else
			{
				System.Diagnostics.Debug.Assert (style.TextStyleClass == nextStyle.TextStyleClass);
				style.DefineNextStyle (nextStyle);
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
			
			if (version == this.versionOfLastUpdate)
			{
				return;
			}
			
			this.versionOfLastUpdate = version;
			
			foreach (TextStyle style in this.textStyleList)
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
			
			foreach (TextStyle style in this.textStyleList)
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
		
		
		public TextStyle GetTextStyle(string name, TextStyleClass textStyleClass)
		{
			return this.GetTextStyle (StyleList.GetFullName (name, textStyleClass));
		}
		
		
		internal TextStyle GetTextStyle(string fullName)
		{
			if (this.textStyleHash.Contains (fullName))
			{
				return this.textStyleHash[fullName] as TextStyle;
			}
			else
			{
				return null;
			}
		}
		
		internal TextStyle GetDefaultTextStyle(TextStyle style)
		{
			TextStyle defaultStyle = null;
			
			switch (style.TextStyleClass)
			{
				case TextStyleClass.Text:		defaultStyle = this.TextContext.DefaultTextStyle;		break;
				case TextStyleClass.Paragraph:	defaultStyle = this.TextContext.DefaultParagraphStyle;	break;
					
				default:
					throw new System.NotSupportedException (string.Format ("Cannot delete style {0} of class {1}", style.Name, style.TextStyleClass));
			}
			
			System.Diagnostics.Debug.Assert (defaultStyle != null);
			
			return defaultStyle;
		}
		
		
		public TextStyle[] FindEqualTextStyles(TextStyle style)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			int signature = style.GetContentsSignature ();
			
			foreach (TextStyle candidate in this.textStyleList)
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
			int n = this.textStyleList.Count;
			
			for (int i = 0; i < this.textStyleList.Count; i++)
			{
				TextStyle style = this.textStyleList[i] as TextStyle;
				
				if (style.IsDeleted)
				{
					n--;
				}
			}
			
			buffer.Append (SerializerSupport.SerializeLong (this.uniqueId));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (n));
			buffer.Append ("/");
			
			this.internalSettings.Serialize (buffer);
			buffer.Append ("/");
			
			for (int i = 0; i < this.textStyleList.Count; i++)
			{
				TextStyle style = this.textStyleList[i] as TextStyle;
				
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
			this.internalSettings = new Internal.SettingsTable ();
			this.textStyleList   = new System.Collections.ArrayList ();
			this.textStyleHash   = new System.Collections.Hashtable ();
			
			long unique   = SerializerSupport.DeserializeLong (args[offset++]);
			int  nStyles = SerializerSupport.DeserializeInt (args[offset++]);
			
			this.uniqueId = unique;
			this.internalSettings.Deserialize (context, version, args, ref offset);
			
			for (int i = 0; i < nStyles; i++)
			{
				TextStyle style = new TextStyle ();
				
				style.Deserialize (context, version, args, ref offset);
				
				this.Attach (style);
			}
			
			foreach (TextStyle style in this.textStyleList)
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
			lock (this.uniqueIdLock)
			{
				return this.uniqueId++;
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
			return this.internalSettings.GetCoreFromIndex (index);
		}
		
		internal void NotifyStyleMapChanged()
		{
			this.OnStyleMapChanged ();
		}
		
		
		internal Property[] Flatten(ulong code)
		{
			Styles.CoreSettings coreSettings = this[code];
			
			if (coreSettings != null)
			{
				return coreSettings.Flatten (code);
			}
			else
			{
				return new Property[0];
			}
		}
		
		
		internal void UpdateTabListUserCount()
		{
			foreach (TextStyle style in this.textStyleList)
			{
				this.UpdateTabListUserCount (style, 1);
			}
		}
		
		internal void UpdateGeneratorUserCount()
		{
			foreach (TextStyle style in this.textStyleList)
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
				string   managerName = mpp.ManagerName;
				string[] parameters   = mpp.ManagerParameters;
				
				switch (managerName)
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
				string   managerName = mpp.ManagerName;
				string[] parameters   = mpp.ManagerParameters;
				
				switch (managerName)
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
		
		internal static string GetFullName(TextStyle textStyle)
		{
			return StyleList.GetFullName (textStyle.Name, textStyle.TextStyleClass);
		}
		
		internal static string GetFullName(string name, TextStyleClass textStyleClass)
		{
			switch (textStyleClass)
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
		
		internal static void SplitFullName(string fullName, out string name, out TextStyleClass textStyleClass)
		{
			if ((fullName.Length < 2) ||
				(fullName[1] != '.'))
			{
				throw new System.ArgumentException ();
			}
			
			char prefix = fullName[0];
			
			switch (prefix)
			{
				case 'A': textStyleClass = TextStyleClass.Abstract;		break;
				case 'P': textStyleClass = TextStyleClass.Paragraph;		break;
				case 'T': textStyleClass = TextStyleClass.Text;			break;
				case 'S': textStyleClass = TextStyleClass.Symbol;			break;
				case 'M': textStyleClass = TextStyleClass.MetaProperty;	break;
				
				default:
					throw new System.ArgumentException ();
			}
			
			name = fullName.Substring (2);
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
			
			System.Collections.ArrayList dirtyStories = new System.Collections.ArrayList ();
			
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
					
					this.isPatcherOnBreak     = true;
					this.pendingManagerPatches = new System.Collections.Stack ();
					
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
						
						while (this.pendingManagerPatches.Count > 0)
						{
							ManagedParagraphPropertyInfo mppi = this.pendingManagerPatches.Pop () as ManagedParagraphPropertyInfo;
							
							story.SetCursorPosition (cursor, mppi.Position);
							
							Properties.ManagedParagraphProperty[] mppOld = (mppi.Mpp1 == null) ? new Properties.ManagedParagraphProperty[0] : new Properties.ManagedParagraphProperty[1] { mppi.Mpp1 };
							Properties.ManagedParagraphProperty[] mppNew = (mppi.Mpp2 == null) ? new Properties.ManagedParagraphProperty[0] : new Properties.ManagedParagraphProperty[1] { mppi.Mpp2 };
							
							Internal.Navigator.HandleManagedParagraphPropertiesChange (story, cursor, 0, mppOld, mppNew);
						}
					}
					finally
					{
						story.EnableOpletQueue ();
						story.RecycleCursor (cursor);
						story.ResumeTextChanged ();
					}
					
					this.isPatcherOnBreak     = false;
					this.pendingManagerPatches = null;
				}
				
				if (update)
				{
					dirtyStories.Add (story);
				}
			}
			
			//	Signale encore que les diverses instances de TextStory ont été
			//	modifiées :
			
			foreach (TextStory story in dirtyStories)
			{
				story.NotifyTextChanged ();
			}
			
			return dirtyStories.Count > 0;
		}
		
		private bool UpdateTextBuffer(TextStory story, Patcher patcher, ulong[] buffer, int absPos, int length)
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
					update |= this.UpdateTextRun (story, patcher, buffer, last, absPos, i-num, num);
					
					num  = 1;
					last = code;
				}
				else
				{
					num += 1;
				}
			}
			
			update |= this.UpdateTextRun (story, patcher, buffer, last, absPos, length-num, num);
			
			return update;
		}
		
		private bool UpdateTextRun(TextStory story, Patcher patcher, ulong[] buffer, ulong code, int absPos, int pos, int length)
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
				
				bool isFlagged = false;
				
				//	Regarde si l'un des styles appliqués au "run" actuel a été marqué
				//	comme modifié. Ce n'est que dans ce cas qu'une substitution sera
				//	nécessaire :
				
				for (int i = 0; i < styles.Length; i++)
				{
					if (styles[i].IsFlagged)
					{
						isFlagged = true;
						break;
					}
				}
				
				if (isFlagged)
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
					if (this.isPatcherOnBreak)
					{
						//	Au début de chaque paragraphe, vérifie si nous avons un
						//	changement de "managed paragraph". Si c'est le cas, il
						//	faut prendre note de la position pour pouvoir y revenir
						//	plus tard pour faire les attach/detach nécessaires sur
						//	les IParagraphManager :
						
						if (mppi != null)
						{
							mppi = new ManagedParagraphPropertyInfo (mppi, absPos+i);
							
							this.pendingManagerPatches.Push (mppi);
						}
					}
					
					buffer[i] ^= replacement;
					
					//	Regarde encore si l'on se trouve à une fin de paragraphe;
					//	c'est utile pour la suite :
					
					this.isPatcherOnBreak = Internal.Navigator.IsParagraphSeparator (Unicode.Bits.GetUnicodeCode (buffer[i]));
				}
				
				return true;
			}
			else
			{
				//	Aucune modification n'a été nécessaire. Vérifie encore si le
				//	"run" se termine par une fin de paragraphe :
				
				this.isPatcherOnBreak = Internal.Navigator.IsParagraphSeparator (Unicode.Bits.GetUnicodeCode (buffer[end-1]));
				
				return false;
			}
		}
		
		
		private void Attach(TextStyle style)
		{
			string         name             = style.Name;
			TextStyleClass textStyleClass   = style.TextStyleClass;
			string         fullName         = StyleList.GetFullName (name, textStyleClass);
			
			if (this.textStyleHash.Contains (fullName))
			{
				throw new System.ArgumentException (string.Format ("TextStyle named {0} ({1}) already exists", name, textStyleClass), "style");
			}
			
			this.textStyleList.Add (style);
			this.textStyleHash[fullName] = style;
			
			this.OnStyleAdded ();
		}
		
		private void Detach(TextStyle style)
		{
			string         name             = style.Name;
			TextStyleClass textStyleClass   = style.TextStyleClass;
			string         fullName         = StyleList.GetFullName (name, textStyleClass);
			
			if (this.textStyleHash.Contains (fullName))
			{
				this.textStyleList.Remove (style);
				this.textStyleHash.Remove (fullName);
			}
			else
			{
				throw new System.ArgumentException (string.Format ("TextStyle named {0} ({1}) does not exist", name, textStyleClass), "style");
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
				string newState = this.style.SaveState (this.stylist);
				string oldState = this.state;
				
				this.state = newState;
				
				this.stylist.PreRedefine (this.style);
				this.style.RestoreState (this.stylist, oldState);
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
				TextStyle newNext = this.next;
				TextStyle oldNext = this.style.NextStyle;
				
				this.style.DefineNextStyle (newNext);
				
				this.next = oldNext;
				
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
				
				TextStyle defaultStyle = this.stylist.GetDefaultTextStyle (this.style);
				
				this.style.IsDeleted = true;
				this.stylist.RedefineTextStyle (null, this.style, new Property[0], new TextStyle[1] { defaultStyle } );
				
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
		private Internal.SettingsTable			internalSettings;
		private System.Collections.ArrayList	textStyleList;
		private System.Collections.Hashtable	textStyleHash;
		private StyleMap						styleMap;
		private long							uniqueId;
		private object							uniqueIdLock = new object ();
		private StyleVersion					version = new StyleVersion ();
		private long							versionOfLastUpdate;
		
		private bool							isPatcherOnBreak;
		private System.Collections.Stack		pendingManagerPatches;
	}
}
