//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	using EventHandler		= Epsitec.Common.Support.EventHandler;
	using OpletEventHandler = Epsitec.Common.Support.OpletEventHandler;
	using OpletEventArgs	= Epsitec.Common.Support.OpletEventArgs;
	
	/// <summary>
	/// La classe TextNavigator permet de manipuler un TextStory en vue de son
	/// �dition.
	/// </summary>
	public class TextNavigator : System.IDisposable
	{
		public TextNavigator(TextStory story) : this (new TextFitter (story))
		{
		}
		
		public TextNavigator(TextFitter fitter)
		{
			this.story  = fitter.TextStory;
			this.fitter = fitter;
			this.cursor = new Cursors.SimpleCursor ();
			
			this.temp_cursor = new Cursors.TempCursor ();
			
			this.story.NewCursor (this.cursor);
			this.story.NewCursor (this.temp_cursor);
			
			this.story.OpletExecuted += new OpletEventHandler (this.HandleStoryOpletExecuted);
		}
		
		
		public int								TextLength
		{
			get
			{
				return this.story.TextLength;
			}
		}
		
		public TextContext						TextContext
		{
			get
			{
				return this.story.TextContext;
			}
		}
		
		public TextStyle[]						TextStyles
		{
			get
			{
				this.UpdateCurrentStylesAndPropertiesIfNeeded ();
				return this.current_styles.Clone () as TextStyle[];
			}
		}
		
		public Property[]						TextProperties
		{
			get
			{
				this.UpdateCurrentStylesAndPropertiesIfNeeded ();
				return this.current_properties.Clone () as Property[];
			}
		}
		
		
		public Property[]						AccumulatedTextProperties
		{
			get
			{
				this.UpdateCurrentStylesAndPropertiesIfNeeded ();
				return this.accumulated_properties.Clone () as Property[];
			}
		}
		
		
		public int								CursorPosition
		{
			get
			{
				return this.story.GetCursorPosition (this.ActiveCursor);
			}
		}
		
		public int								CursorDirection
		{
			get
			{
				return this.story.GetCursorDirection (this.ActiveCursor);
			}
		}
		
		public bool								IsSelectionActive
		{
			get
			{
				return this.active_selection_cursor == null ? false : true;
			}
		}
		
		public bool								HasSelection
		{
			get
			{
				return this.selection_cursors == null ? false : true;
			}
		}
		
		public int								SelectionCount
		{
			get
			{
				if (this.selection_cursors == null)
				{
					return 0;
				}
				else
				{
					return this.selection_cursors.Count / 2;
				}
			}
		}
		
		
		public Common.Support.OpletQueue		OpletQueue
		{
			get
			{
				return this.story.OpletQueue;
			}
		}
		
		
		public ICursor							ActiveCursor
		{
			get
			{
				if (this.active_selection_cursor != null)
				{
					return this.active_selection_cursor;
				}
				else
				{
					return this.cursor;
				}
			}
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		public void Insert(Unicode.Code code)
		{
			if (code == Unicode.Code.HorizontalTab)
			{
				throw new System.InvalidOperationException ("Cannot insert tab without tag");
			}
			
			this.InsertText (new string ((char) code, 1));
			this.NotifyTextChanged ();
		}
		
		public void Insert(Unicode.Code code, Property property)
		{
			//	Ins�re un caract�re sp�cial qui doit �tre li� � la propri�t� de
			//	mani�re inamovible.
			
			System.Diagnostics.Debug.Assert (property != null);
			System.Diagnostics.Debug.Assert (property.PropertyAffinity == Properties.PropertyAffinity.Symbol);
			
			//	La propri�t� pass�e en entr�e est simplement ajout�e en fin de
			//	liste des propri�t�s associ�es au curseur, temporairement :
			
			this.UpdateCurrentStylesAndPropertiesIfNeeded ();
			
			Property[] old_properties = this.current_properties;
			Property[] new_properties = new Property[old_properties.Length+1];
			
			old_properties.CopyTo (new_properties, 0);
			new_properties[old_properties.Length] = property;
			
			try
			{
				this.current_properties = new_properties;
				this.InsertText (new string ((char) code, 1));
			}
			finally
			{
				this.current_properties = old_properties;
			}
			
			this.NotifyTextChanged ();
		}
		
		public void Insert(string text)
		{
			//	On n'a pas le droit d'ins�rer des tabulateurs avec cette m�thode,
			//	car il faudrait conna�tre la position � atteindre :
			
			foreach (char c in text)
			{
				if (c == '\t')
				{
					throw new System.InvalidOperationException ("Cannot insert tab without tag");
				}
			}
			
			this.InsertText (text);
			this.NotifyTextChanged ();
		}
		
		public void Delete()
		{
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			
			//	Supprime le contenu de la s�lection (pour autant qu'il y en ait
			//	une qui soit d�finie).
			
			if (this.selection_cursors != null)
			{
				Internal.TextTable text = this.story.TextTable;
				
				using (this.story.BeginAction ())
				{
					this.InternalInsertSelectionOplet ();
					
					for (int i = 0; i < this.selection_cursors.Count; i += 2)
					{
						//	Traite les tranches dans l'ordre, en les d�truisant les
						//	unes apr�s les autres.
						
						ICursor c1 = this.selection_cursors[i+0] as ICursor;
						ICursor c2 = this.selection_cursors[i+1] as ICursor;
						
						int p1 = text.GetCursorPosition (c1.CursorId);
						int p2 = text.GetCursorPosition (c2.CursorId);
						
						if (p1 > p2)
						{
							ICursor cc = c1;
							int     pp = p1;
							
							p1 = p2;	c1 = c2;
							p2 = pp;	c2 = cc;
						}
						
						if (i+2 == this.selection_cursors.Count)
						{
							//	C'est la derni�re tranche. Il faut positionner le curseur
							//	de travail au d�but de la zone et h�riter des styles actifs
							//	� cet endroit :
							
							this.story.SetCursorPosition (this.cursor, p1, 0);
							this.UpdateCurrentStylesAndProperties ();
						}
						
						this.DeleteText (c1, p2-p1);
					}
					
					this.story.ValidateAction ();
				}
				
				this.InternalClearSelection ();
				this.UpdateSelectionMarkers ();
			}
			
			this.NotifyTextChanged ();
		}
		
		public void Delete(int move)
		{
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			
			int p1 = this.story.GetCursorPosition (this.cursor);
			int p2 = p1 + move;
			int d2;
			
			Cursors.TempCursor temp = new Cursors.TempCursor ();
			
			this.story.NewCursor (temp);
			this.story.SetCursorPosition (temp, p1);
			
			this.MoveCursor (temp, move, out p2, out d2);
			
			if (Internal.Navigator.IsParagraphStart (this.story, temp, p2 - p1))
			{
				//	Le curseur n'a pas le droit de se trouver en d�but de paragraphe
				//	si celui-ci commence par du texte automatique, car on n'a pas le
				//	droit d'ins�rer de texte avant celui-ci.
				
				this.SkipOverAutoText (ref p2, 1);
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Delete from {0} to {1}", p1, p2));
			
			this.story.RecycleCursor (temp);
			
			if (p1 < 0)
			{
				p1 = 0;
			}
			if (p1 > this.story.TextLength)
			{
				p1 = this.story.TextLength;
			}
			if (p2 < 0)
			{
				p2 = 0;
			}
			if (p2 > this.story.TextLength)
			{
				p2 = this.story.TextLength;
			}
			
			if (p2 < p1)
			{
				int pp = p1;
				p1 = p2;
				p2 = pp;
			}
			
			if (p1 == p2)
			{
				return;
			}
			
			this.story.NewCursor (temp);
			
			try
			{
				this.story.SetCursorPosition (temp, p2);
				
				if (Internal.Navigator.IsEndOfText (this.story, temp, -1))
				{
					//	La position du texte est telle que le curseur se trouve apr�s
					//	la marque de fin de texte. Corrige sa position :
					
					p2 -= 1;
				}
				
				if (p2 > p1)
				{
					this.story.SetCursorPosition (temp, p1);
					this.DeleteText (temp, p2-p1);
				}
				
				if (Internal.Navigator.IsParagraphStart (this.story, this.ActiveCursor, 0))
				{
					//	Le curseur n'a pas le droit de se trouver en d�but de paragraphe
					//	si celui-ci commence par du texte automatique, car on n'a pas le
					//	droit d'ins�rer de texte avant celui-ci.
				
					int pos = this.story.GetCursorPosition (this.ActiveCursor);
					
					if (this.SkipOverAutoText (ref pos, 1))
					{
						this.story.SetCursorPosition (this.ActiveCursor, pos);
					}
				}
			}
			finally
			{
				this.story.RecycleCursor (temp);
			}
			
			this.NotifyTextChanged ();
		}
		
		public void MoveTo(int position, int direction)
		{
			this.InternalSetCursor (position, direction);
		}
		
		public void MoveTo(Target target, int count)
		{
			System.Diagnostics.Debug.Assert (count >= 0);
			
			int old_pos = this.CursorPosition;
			int old_dir = this.CursorDirection;
			
			int new_pos;
			int new_dir;
			
			switch (target)
			{
				case Target.CharacterNext:
					this.MoveCursor (this.ActiveCursor, count, out new_pos, out new_dir);
					break;
				
				case Target.CharacterPrevious:
					this.MoveCursor (this.ActiveCursor, -count, out new_pos, out new_dir);
					break;
				
				case Target.TextStart:
					new_pos = 0;
					new_dir = -1;
					break;
				
				case Target.TextEnd:
					new_pos = this.TextLength;
					new_dir = 1;
					break;
					
				case Target.ParagraphStart:
					this.MoveCursor (this.ActiveCursor, count, -1, new MoveCallback (this.IsParagraphStart), out new_pos, out new_dir);
					break;
				
				case Target.ParagraphEnd:
					this.MoveCursor (this.ActiveCursor, count, 1, new MoveCallback (this.IsParagraphEnd), out new_pos, out new_dir);
					break;
				
				case Target.LineStart:
					this.MoveCursor (this.ActiveCursor, count, -1, new MoveCallback (this.IsLineStart), out new_pos, out new_dir);
					break;
				
				case Target.LineEnd:
					this.MoveCursor (this.ActiveCursor, count, 1, new MoveCallback (this.IsLineEnd), out new_pos, out new_dir);
					break;
				
				case Target.WordStart:
					this.MoveCursor (this.ActiveCursor, count, -1, new MoveCallback (this.IsWordStart), out new_pos, out new_dir);
					break;
				
				case Target.WordEnd:
					this.MoveCursor (this.ActiveCursor, count, 1, new MoveCallback (this.IsWordEnd), out new_pos, out new_dir);
					
					//	Si en marche avant, on arrive � la fin d'une ligne qui n'est pas
					//	une fin de paragraphe, alors il faut changer la direction, afin
					//	que le curseur apparaisse au d�but de la igne suivante :
					
					if ((Internal.Navigator.IsParagraphEnd (this.story, this.temp_cursor, new_pos - old_pos) == false) &&
						(Internal.Navigator.IsLineEnd (this.story, this.fitter, this.temp_cursor, new_pos - old_pos, 1)))
					{
						System.Diagnostics.Debug.WriteLine ("Swap direction (2)");
						new_dir = -1;
					}
					
					break;
					
				default:
					throw new System.NotSupportedException (string.Format ("Target {0} not supported", target));
			}
			
			if ((old_pos != new_pos) ||
				(old_dir != new_dir))
			{
				this.InternalSetCursor (new_pos, new_dir);
			}
		}
		
		
		public void Undo()
		{
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			
			this.OpletQueue.UndoAction ();
		}
		
		public void Redo()
		{
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			
			this.OpletQueue.RedoAction ();
		}
		
		
		public void StartSelection()
		{
			System.Diagnostics.Debug.Assert (! this.IsSelectionActive);
			
			Cursors.SelectionCursor c1 = this.NewSelectionCursor ();
			Cursors.SelectionCursor c2 = this.NewSelectionCursor ();
			
			this.selection_cursors.Add (c1);
			this.selection_cursors.Add (c2);
			
			int position  = this.story.GetCursorPosition (this.cursor);
			int direction = this.story.GetCursorDirection (this.cursor);
			
			this.story.SetCursorPosition (c1, position, direction);
			this.story.SetCursorPosition (c2, position, direction);
			
			this.active_selection_cursor = c2;
			this.NotifyCursorMoved ();
		}
		
		public void EndSelection()
		{
			System.Diagnostics.Debug.Assert (this.IsSelectionActive);
			
			this.active_selection_cursor = null;
			
			using (this.story.BeginAction ())
			{
				this.InternalInsertDeselectionOplet ();
				this.story.ValidateAction ();
			}
			
			this.NotifyCursorMoved ();
		}
		
		public void ClearSelection()
		{
			this.ClearSelection (0);
		}
		
		public void ClearSelection(int direction)
		{
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			
			//	D�s�lectionne tout le texte.
			
			if (this.selection_cursors != null)
			{
				//	Prend note de la position des curseurs de s�lection pour
				//	pouvoir restaurer la s�lection en cas de UNDO :
				
				using (this.story.BeginAction ())
				{
					int[]   positions = this.GetSelectionCursorPositions ();
					Range[] ranges    = Range.CreateSortedRanges (positions);
					
					this.InternalInsertSelectionOplet (positions);
					
					//	D�place le curseur de travail au d�but ou � la fin de la
					//	tranche s�lectionn�e, en fonction de la direction :
					
					if ((direction != 0) &&
						(ranges.Length > 0))
					{
						int pos = (direction < 0) ? ranges[0].Start : ranges[ranges.Length-1].End;
						this.story.SetCursorPosition (this.cursor, pos, direction);
					}
					
					this.story.ValidateAction ();
				}
				
				this.InternalClearSelection ();
				this.UpdateSelectionMarkers ();
			}
			
			System.Diagnostics.Debug.Assert (this.HasSelection == false);
			this.NotifyCursorMoved ();
		}
		
		
		public ulong[] GetRawSelection(int index)
		{
			if ((index >= this.SelectionCount) ||
				(index < 0))
			{
				throw new System.ArgumentOutOfRangeException ("index", index, "Index out of range");
			}
			
			int[] positions = this.GetSelectionCursorPositions ();
			
			int p1 = positions[index*2+0];
			int p2 = positions[index*2+1];
			
			ICursor c1 = this.selection_cursors[index*2+0] as ICursor;
			ICursor c2 = this.selection_cursors[index*2+1] as ICursor;
			
			if (p1 > p2)
			{
				int     pp = p1; p1 = p2; p2 = pp;
				ICursor cc = c1; c1 = c2; c2 = cc;
			}
			
			ulong[] buffer = new ulong[p2-p1];
			
			this.story.ReadText (c1, p2-p1, buffer);
			
			return buffer;
		}
		
		public string[] GetSelectedTexts()
		{
			//	Retourne les textes s�lectionn�s, bruts, sans aucun formatage.
			//	S'il n'y en a pas, retourne un tableau vide.
			
			string[] texts;
			
			if (this.selection_cursors == null)
			{
				texts = new string[0];
			}
			else
			{
				int[] positions = this.GetSelectionCursorPositions ();
				
				texts = new string[positions.Length / 2];
				
				for (int i = 0; i < positions.Length; i += 2)
				{
					int p1 = positions[i+0];
					int p2 = positions[i+1];
					
					ICursor c1 = this.selection_cursors[i+0] as ICursor;
					ICursor c2 = this.selection_cursors[i+1] as ICursor;
					
					if (p1 > p2)
					{
						int     pp = p1; p1 = p2; p2 = pp;
						ICursor cc = c1; c1 = c2; c2 = cc;
					}
					
					string  text;
					ulong[] buffer = new ulong[p2-p1];
					
					this.story.ReadText (c1, p2-p1, buffer);
					
					TextConverter.ConvertToString (buffer, out text);
					
					texts[i/2] = text;
				}
			}
			
			return texts;
		}
		
		
		public TabInfo[] GetTabInfos()
		{
			string[] tags_1 = this.GetParagraphTabTags ();
			string[] tags_2 = this.GetTextTabTags ();
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			foreach (string tag in tags_1)
			{
				System.Diagnostics.Debug.Assert (hash.Contains (tag) == false);
				
				TabInfo info = new TabInfo (tag, TabStatus.Definition);
				
				hash[tag] = info;
			}
			
			foreach (string tag in tags_2)
			{
				if (hash.Contains (tag))
				{
					hash[tag] = new TabInfo (tag, TabStatus.Live);
				}
				else
				{
					hash[tag] = new TabInfo (tag, TabStatus.Zombie);
				}
			}
			
			string[]  tags  = new string[hash.Count];
			TabInfo[] infos = new TabInfo[hash.Count];
			
			hash.Keys.CopyTo (tags, 0);
			hash.Values.CopyTo (infos, 0);
			
			System.Array.Sort (tags, infos);
			
			return infos;
		}
		
		
		public string[] GetAllTabTags()
		{
			string[] tags_1 = this.GetParagraphTabTags ();
			string[] tags_2 = this.GetTextTabTags ();
			
			if (tags_2.Length == 0)
			{
				return tags_1;
			}
			if (tags_1.Length == 0)
			{
				return tags_2;
			}
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (string tag in tags_1)
			{
				list.Add (tag);
			}
			foreach (string tag in tags_2)
			{
				if (list.Contains (tag) == false)
				{
					list.Add (tag);
				}
			}
			
			list.Sort ();
			
			return (string[]) list.ToArray (typeof (string));
		}
		
		
		public bool RenameTab(string old_tag, string new_tag)
		{
			return this.RenameTabs (new string[] { old_tag }, new_tag);
		}
		
		public bool RenameTabs(string[] old_tags, string new_tag)
		{
			System.Diagnostics.Debug.Assert (old_tags != null);
			System.Diagnostics.Debug.Assert (old_tags.Length > 0);
			
			if ((old_tags.Length > 1) ||
				(old_tags[0] != new_tag))
			{
				int[] pos_1 = this.FindTextTabPositions (old_tags);
				int[] pos_2 = this.FindTextTabsPositions (old_tags);
				
				if ((pos_1.Length > 0) ||
					(pos_2.Length > 0))
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Rename tab from {0} to {1}, {2} live, {3} defined", string.Join ("/", old_tags), new_tag, pos_1.Length, pos_2.Length));
					
					using (this.story.BeginAction ())
					{
						//	Remplace les propri�t�s TabProperty des divers TAB
						//	du texte par une nouvelle avec le nouveau nom :
						
						Property[] tab_rename  = new Property[1];
						Property[] tabs_change = new Property[1];
						
						string[] tabs = new string[old_tags.Length + 1];
						
						for (int i = 0; i < old_tags.Length; i++)
						{
							tabs[i] = string.Concat ("-", old_tags[i]);
						}
						
						tabs[old_tags.Length] = new_tag;
						
						tab_rename[0]  = this.TextContext.TabList[new_tag];
						tabs_change[0] = new Properties.TabsProperty (tabs);
						
						for (int i = 0; i < pos_1.Length; i++)
						{
							this.SetTextProperties (pos_1[i], 1, Properties.ApplyMode.Set, tab_rename);
							this.SetParagraphProperties (pos_1[i], Properties.ApplyMode.Combine, tabs_change);
						}
						
						for (int i = 0; i < pos_2.Length; i++)
						{
							this.SetParagraphProperties (pos_2[i], Properties.ApplyMode.Combine, tabs_change);
						}
						
						this.story.ValidateAction ();
					}
					
					this.UpdateCurrentStylesAndProperties ();
					this.NotifyTextChanged ();
				}
			}
			
			return false;
		}
		
		public void RemoveTab(string tag)
		{
			string[] tags = this.GetParagraphTabTags ();
			
			for (int i = 0; i < tags.Length; i++)
			{
				if (tags[i] == tag)
				{
					//	Trouv� le tabulateur dans au moins un des paragraphes
					//	concern�s. Il y a donc bien quelque chose � faire :
					
					Text.Properties.TabsProperty tabs = new Text.Properties.TabsProperty (string.Concat ("-", tag));
					this.SetParagraphProperties (Text.Properties.ApplyMode.Combine, tabs);
					
					break;
				}
			}
		}
		
		
		#region Private Tabulator Manipulation Methods
		private string[] GetParagraphTabTags()
		{
			System.Collections.ArrayList tabs_list = new System.Collections.ArrayList ();
			System.Collections.ArrayList tags_list = new System.Collections.ArrayList ();
			
			int length = 0;
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				foreach (Range range in ranges)
				{
					int start = range.Start;
					int end   = range.End;
					int pos   = start;
					
					length += end - start;
					
					while (pos < end)
					{
						Property property = this.GetTabsProperty (pos);
						
						if (property != null)
						{
							tabs_list.Add (property);
						}
						
						pos = this.FindNextParagraphStart (pos);
					}
				}
			}
			
			if (length == 0)
			{
				foreach (Property property in this.accumulated_properties)
				{
					if (property.WellKnownType == Properties.WellKnownType.Tabs)
					{
						tabs_list.Add (property);
						break;
					}
				}
			}
			
			foreach (Properties.TabsProperty tabs_property in tabs_list)
			{
				foreach (string tag in tabs_property.TabTags)
				{
					if (tags_list.Contains (tag) == false)
					{
						tags_list.Add (tag);
					}
				}
			}
			
			tags_list.Sort ();
			
			return (string[]) tags_list.ToArray (typeof (string));
		}
		
		private string[] GetTextTabTags()
		{
			System.Collections.ArrayList tags_list = new System.Collections.ArrayList ();
			
			int length = 0;
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				foreach (Range range in ranges)
				{
					int start = range.Start;
					int end   = range.End;
					int pos   = start;
					
					length += end - start;
					
					while (pos < end)
					{
						this.FindTextTabTags (tags_list, pos);
						
						pos = this.FindNextParagraphStart (pos);
					}
				}
			}
			
			if (length == 0)
			{
				this.FindTextTabTags (tags_list, this.CursorPosition);
			}
			
			tags_list.Sort ();
			
			return (string[]) tags_list.ToArray (typeof (string));
		}
		
		
		private Properties.TabsProperty GetTabsProperty(int pos)
		{
			this.story.SetCursorPosition (this.temp_cursor, pos);
			ulong code = this.story.ReadChar (this.temp_cursor);
			Properties.TabsProperty property;
			this.TextContext.GetTabs (code, out property);
			return property;
		}
		
		
		private void FindTextTabTags(System.Collections.ArrayList list, int pos)
		{
			//	Trouve les tags utilis�s par des tabulateurs dans le texte
			//	du paragraphe :
			
			int start;
			int end;
			
			Internal.Navigator.GetParagraphPositions (this.story, pos, out start, out end);
			
			int length = end - start;
			
			if (length > 0)
			{
				ulong[] text = new ulong[length];
				this.story.ReadText (start, length, text);
				
				for (int i = 0; i < length; i++)
				{
					if (Unicode.Bits.GetUnicodeCode (text[i]) == Unicode.Code.HorizontalTab)
					{
						//	Trouv� un tabulateur. D�termine le tag correspondant en
						//	analysant la propri�t� attach�e :
						
						Properties.TabProperty property;
						
						this.TextContext.GetTab (text[i], out property);
						
						System.Diagnostics.Debug.Assert (property != null);
						System.Diagnostics.Debug.Assert (property.TabTag != null);
						
						if (list.Contains (property.TabTag) == false)
						{
							list.Add (property.TabTag);
						}
					}
				}
			}
		}
		
		
		private int[] FindTextTabPositions(string[] tags)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			int length = 0;
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				foreach (Range range in ranges)
				{
					int start = range.Start;
					int end   = range.End;
					int pos   = start;
					
					length += end - start;
					
					while (pos < end)
					{
						this.FindTextTabPositions (list, pos, tags);
						
						pos = this.FindNextParagraphStart (pos);
					}
				}
			}
			
			if (length == 0)
			{
				this.FindTextTabPositions (list, this.CursorPosition, tags);
			}
			
			return (int[]) list.ToArray (typeof (int));
		}
		
		private int[] FindTextTabsPositions(string[] tags)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			int length = 0;
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				foreach (Range range in ranges)
				{
					int start = range.Start;
					int end   = range.End;
					int pos   = start;
					
					length += end - start;
					
					while (pos < end)
					{
						this.FindTextTabsPositions (list, pos, tags);
						
						pos = this.FindNextParagraphStart (pos);
					}
				}
			}
			
			if (length == 0)
			{
				this.FindTextTabsPositions (list, this.CursorPosition, tags);
			}
			
			return (int[]) list.ToArray (typeof (int));
		}
		
		private void  FindTextTabPositions(System.Collections.ArrayList list, int pos, string[] tags)
		{
			//	Trouve la position des caract�res TAB dand le texte du
			//	paragraphe qui correspondent � la marque de tabulation
			//	cherch�e :
			
			int start;
			int end;
			
			Internal.Navigator.GetParagraphPositions (this.story, pos, out start, out end);
			
			int length = end - start;
			
			if (length > 0)
			{
				ulong[] text = new ulong[length];
				this.story.ReadText (start, length, text);
				
				for (int i = 0; i < length; i++)
				{
					if (Unicode.Bits.GetUnicodeCode (text[i]) == Unicode.Code.HorizontalTab)
					{
						//	Trouv� un tabulateur. D�termine le tag correspondant en
						//	analysant la propri�t� attach�e :
						
						Properties.TabProperty property;
						
						this.TextContext.GetTab (text[i], out property);
						
						System.Diagnostics.Debug.Assert (property != null);
						System.Diagnostics.Debug.Assert (property.TabTag != null);
						
						for (int j = 0; j < tags.Length; j++)
						{
							if (property.TabTag == tags[j])
							{
								list.Add (start + i);
								break;
							}
						}
					}
				}
			}
		}
		
		private void  FindTextTabsPositions(System.Collections.ArrayList list, int pos, string[] tags)
		{
			//	Trouve la position des paragraphes qui font r�f�rence au moyen
			//	d'un Properties.TabsProperty au tag sp�cifi� :
			
			Properties.TabsProperty property = this.GetTabsProperty (pos);
			
			if (property != null)
			{
				for (int i = 0; i < tags.Length; i++)
				{
					if (property.ContainsTabTag (tags[i]))
					{
						list.Add (pos);
						break;
					}
				}
			}
		}
		#endregion
		
		public void SetParagraphStyles(params TextStyle[] styles)
		{
			//	Change les styles du paragraphe attach�s � la position courante (ou
			//	compris dans la s�lection).
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			System.Diagnostics.Debug.Assert (styles != null);
			System.Diagnostics.Debug.Assert (styles.Length > 0);
			
			TextStyle[] paragraph_styles = TextStyle.FilterStyles (styles, TextStyleClass.Paragraph);
			
			System.Diagnostics.Debug.Assert (paragraph_styles.Length > 0);
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				using (this.story.BeginAction ())
				{
					foreach (Range range in ranges)
					{
						int start = range.Start;
						int end   = range.End;
						int pos   = start;
						
						while (pos < positions[range.EndIndex])
						{
							this.SetParagraphStyles (pos, paragraph_styles);
							pos = this.FindNextParagraphStart (pos);
							
							//	Comme l'application d'un style de paragraphe avec manager peut
							//	avoir modifi� le texte (insertion de puces, par ex.), on doit
							//	redemander les positions :
							
							positions = this.GetSelectionCursorPositions ();
						}
					}
					
					this.story.ValidateAction ();
				}
			}
			
			this.UpdateCurrentStylesAndPropertiesIfNeeded ();
			
			this.SetParagraphStyles (this.story.GetCursorPosition (this.cursor), paragraph_styles);
			
			System.Collections.ArrayList new_styles = new System.Collections.ArrayList ();
			
			new_styles.AddRange (paragraph_styles);
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Text));
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Character));
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.MetaProperty));
			
			this.current_styles = new_styles.ToArray (typeof (TextStyle)) as TextStyle[];
			
			this.RefreshAccumulatedStylesAndProperties ();
			this.NotifyTextChanged ();
		}
		
		public void SetTextStyles(params TextStyle[] styles)
		{
			//	Change les styles du texte attach�s � la position courante (ou
			//	compris dans la s�lection).
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			System.Diagnostics.Debug.Assert (styles != null);
			System.Diagnostics.Debug.Assert (styles.Length > 0);
			
			TextStyle[] text_styles = TextStyle.FilterStyles (styles, TextStyleClass.Text);
			
			System.Diagnostics.Debug.Assert (text_styles.Length > 0);
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				using (this.story.BeginAction ())
				{
					foreach (Range range in ranges)
					{
						int pos    = range.Start;
						int length = range.Length;
				
						this.SetTextStyles (pos, length, text_styles);
					}
					
					this.story.ValidateAction ();
				}
			}
//			else
			{
				this.UpdateCurrentStylesAndPropertiesIfNeeded ();
				
				System.Collections.ArrayList new_styles = new System.Collections.ArrayList ();
				
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Paragraph));
				new_styles.AddRange (text_styles);
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Character));
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.MetaProperty));
				
				this.current_styles = new_styles.ToArray (typeof (TextStyle)) as TextStyle[];
				
				this.RefreshAccumulatedStylesAndProperties ();
			}
			
			this.NotifyTextChanged ();
		}
		
		public void SetCharacterStyles(params TextStyle[] styles)
		{
			//	Change les styles des caract�res attach�s � la position courante (ou
			//	compris dans la s�lection).
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			System.Diagnostics.Debug.Assert (styles != null);
			System.Diagnostics.Debug.Assert (styles.Length > 0);
			
			TextStyle[] character_styles = TextStyle.FilterStyles (styles, TextStyleClass.Character);
			
			System.Diagnostics.Debug.Assert (character_styles.Length > 0);
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				using (this.story.BeginAction ())
				{
					foreach (Range range in ranges)
					{
						int pos    = range.Start;
						int length = range.Length;
						
						this.SetCharacterStyles (pos, length, character_styles);
					}
					
					this.story.ValidateAction ();
				}
			}
			else
			{
				this.UpdateCurrentStylesAndPropertiesIfNeeded ();
				
				System.Collections.ArrayList new_styles = new System.Collections.ArrayList ();
				
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Paragraph));
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Text));
				new_styles.AddRange (character_styles);
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.MetaProperty));
				
				this.current_styles = new_styles.ToArray (typeof (TextStyle)) as TextStyle[];
				
				this.RefreshAccumulatedStylesAndProperties ();
			}
			
			this.NotifyTextChanged ();
		}
		
		public void SetMetaProperties(Properties.ApplyMode mode, params TextStyle[] styles)
		{
			//	Change les m�ta-propri�t�s attach�es � la position courante (ou
			//	compris dans la s�lection).
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			System.Diagnostics.Debug.Assert (styles != null);
			System.Diagnostics.Debug.Assert (styles.Length > 0);
			
			TextStyle[] meta_properties = TextStyle.FilterStyles (styles, TextStyleClass.MetaProperty);
			
			System.Diagnostics.Debug.Assert (meta_properties.Length == styles.Length);
			System.Diagnostics.Debug.Assert (meta_properties.Length > 0);
			
			bool is_uniform = false;
			
			foreach (TextStyle style in meta_properties)
			{
				if (style.RequiresUniformParagraph)
				{
					is_uniform = true;
					break;
				}
			}
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				using (this.story.BeginAction ())
				{
					foreach (Range range in ranges)
					{
						int start = range.Start;
						
						if (is_uniform)
						{
							int pos = start;
							int end = range.End;
							
							while (pos < positions[range.EndIndex])
							{
								this.SetParagraphMetaProperties (pos, mode, meta_properties);
								pos = this.FindNextParagraphStart (pos);
								
								//	Comme l'application d'un style de paragraphe avec manager peut
								//	avoir modifi� le texte (insertion de puces, par ex.), on doit
								//	redemander les positions :
								
								positions = this.GetSelectionCursorPositions ();
							}
						}
						else
						{
							this.SetMetaProperties (start, range.Length, mode, meta_properties);
						}
					}
					
					this.story.ValidateAction ();
				}
			}
			
			this.UpdateCurrentStylesAndPropertiesIfNeeded ();
			
			if (is_uniform)
			{
				int pos = this.story.GetCursorPosition (this.cursor);
			
				this.SetParagraphMetaProperties (pos, mode, meta_properties);
			}
			
			
			System.Collections.ArrayList new_styles = new System.Collections.ArrayList ();
			
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Paragraph));
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Text));
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Character));
			new_styles.AddRange (Internal.Navigator.Combine (TextStyle.FilterStyles (this.current_styles, TextStyleClass.MetaProperty), meta_properties, mode));
			
			this.current_styles = new_styles.ToArray (typeof (TextStyle)) as TextStyle[];
			
			this.RefreshAccumulatedStylesAndProperties ();
			this.NotifyTextChanged ();
		}
		
		public void SetParagraphProperties(Properties.ApplyMode mode, params Property[] properties)
		{
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			
			if (properties == null)
			{
				properties = new Property[0];
			}
			
			Property[] paragraph_properties = Property.Filter (properties, Properties.PropertyFilter.UniformOnly);
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				using (this.story.BeginAction ())
				{
					foreach (Range range in ranges)
					{
						int start = range.Start;
						int end   = range.End;
						int pos   = start;
						
						while (pos < end)
						{
							this.SetParagraphProperties (pos, mode, paragraph_properties);
							pos = this.FindNextParagraphStart (pos);
						}
					}
					
					this.story.ValidateAction ();
				}
			}
//			else
			{
				this.UpdateCurrentStylesAndPropertiesIfNeeded ();
				
				Internal.Navigator.SetParagraphProperties (this.story, this.cursor, mode, paragraph_properties);
				
				System.Collections.ArrayList new_properties = new System.Collections.ArrayList ();
				
				new_properties.AddRange (Internal.Navigator.Combine (Property.Filter (this.current_properties, Properties.PropertyFilter.UniformOnly), paragraph_properties, mode));
				new_properties.AddRange (Property.Filter (this.current_properties, Properties.PropertyFilter.NonUniformOnly));
				
				this.current_properties = new_properties.ToArray (typeof (Property)) as Property[];
				
				this.RefreshAccumulatedStylesAndProperties ();
			}
			
			this.NotifyTextChanged ();
		}
		
		public void SetTextProperties(Properties.ApplyMode mode, params Property[] properties)
		{
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			
			if (properties == null)
			{
				properties = new Property[0];
			}
			
			Property[] text_properties = Property.Filter (properties, Properties.PropertyFilter.NonUniformOnly);
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				using (this.story.BeginAction ())
				{
					foreach (Range range in ranges)
					{
						int pos    = range.Start;
						int length = range.Length;
					
						this.SetTextProperties (pos, length, mode, text_properties);
					}
					
					this.story.ValidateAction ();
				}
			}
//			else
			{
				this.UpdateCurrentStylesAndPropertiesIfNeeded ();
				
				System.Collections.ArrayList new_properties = new System.Collections.ArrayList ();
				
				new_properties.AddRange (Property.Filter (this.current_properties, Properties.PropertyFilter.UniformOnly));
				new_properties.AddRange (Internal.Navigator.Combine (Property.Filter (this.current_properties, Properties.PropertyFilter.NonUniformOnly), text_properties, mode));
				
				this.current_properties = new_properties.ToArray (typeof (Property)) as Property[];
				
				this.RefreshAccumulatedStylesAndProperties ();
			}
			
			this.NotifyTextChanged ();
		}
		
		
		public bool HitTest(ITextFrame frame, double cx, double cy, bool skip_invisible, out int position, out int direction)
		{
			position  = 0;
			direction = 0;
			
			if (frame != null)
			{
				if (this.fitter.HitTestTextFrame (frame, cx, cy, skip_invisible, ref position, ref direction))
				{
					return true;
				}
				
				if (direction != 0)
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		public void VerticalMove(double x, int direction)
		{
			if (direction == 0)
			{
				return;
			}
			
			ICursor temp = new Cursors.TempCursorDir ();
			this.story.NewCursor (temp);
			
			try
			{
				int old_pos = this.CursorPosition;
				int old_dir = this.CursorDirection;
				
				this.story.SetCursorPosition (temp, old_pos, old_dir);
				
				int new_pos;
				int new_dir;
				
				ITextFrame frame;
				double cx, cy, ascender, descender, angle;
				
				if (direction < 0)
				{
					this.MoveCursor (temp, 0, -1, new MoveCallback (this.IsLineStart), out new_pos, out new_dir);
					this.story.SetCursorPosition (temp, new_pos, new_dir);
					this.MoveCursor (temp, -1, out new_pos, out new_dir);
					this.story.SetCursorPosition (temp, new_pos, new_dir);
					
					if ((this.GetCursorGeometry (temp, out frame, out cx, out cy, out ascender, out descender, out angle)) &&
						(this.HitTest (frame, x, cy, false, out new_pos, out new_dir)))
					{
						this.MoveTo (new_pos, new_dir);
					}
				}
				else
				{
					this.MoveCursor (temp, 0, 1, new MoveCallback (this.IsLineEnd), out new_pos, out new_dir);
					this.story.SetCursorPosition (temp, new_pos, new_dir);
					this.MoveCursor (temp, 1, out new_pos, out new_dir);
					this.story.SetCursorPosition (temp, new_pos, new_dir);
					
					if ((this.GetCursorGeometry (temp, out frame, out cx, out cy, out ascender, out descender, out angle)) &&
						(this.HitTest (frame, x, cy, false, out new_pos, out new_dir)))
					{
						this.MoveTo (new_pos, new_dir);
					}
				}
			}
			finally
			{
				this.story.RecycleCursor (temp);
			}
			
			this.ClearCurrentStylesAndProperties ();
			this.NotifyCursorMoved ();
		}
		
		public bool GetCursorGeometry(out ITextFrame frame, out double cx, out double cy, out double ascender, out double descender, out double angle)
		{
			return this.GetCursorGeometry (this.ActiveCursor, out frame, out cx, out cy, out ascender, out descender, out angle);
		}
		
		public bool GetCursorGeometry(ICursor cursor, out ITextFrame frame, out double cx, out double cy, out double ascender, out double descender, out double angle)
		{
			this.UpdateCurrentStylesAndPropertiesIfNeeded ();
			
			int para_line;
			int line_char;
			
			if (this.fitter.GetCursorGeometry (cursor, out frame, out cx, out cy, out para_line, out line_char))
			{
				Property[] properties = this.accumulated_properties;
				
				if ((this.CursorPosition == this.story.TextLength) &&
					(para_line == 0) &&
					(line_char == 0))
				{
					//	Cas particulier : le curseur se trouve tout seul en fin de pav�,
					//	sans aucun autre caract�re dans la ligne.
					
					Properties.MarginsProperty margins = null;
					
					for (int i = 0; i < properties.Length; i++)
					{
						if (properties[i] is Properties.MarginsProperty)
						{
							margins = properties[i] as Properties.MarginsProperty;
							break;
						}
					}
					
					double ox;
					double oy;
					double width;
					double next_y;
					
					frame.MapFromView (ref cx, ref cy);
					frame.ConstrainLineBox (cy, 0, 0, 0, 0, false, out ox, out oy, out width, out next_y);
					
					double mx1 = margins.LeftMarginFirstLine;
					double mx2 = margins.RightMarginFirstLine;
					double disposition = margins.Disposition;
					
					width -= mx1;
					width -= mx2;
					
					cx += mx1;
					cx += width * disposition;
					
					frame.MapToView (ref cx, ref cy);
				}
				
				OpenType.Font ot_font;
				double        pt_size;
				double        pt_offset;
				double        font_scale;
				
				this.story.TextContext.GetFont (properties, out ot_font);
				this.story.TextContext.GetFontSize (properties, out pt_size, out font_scale);
				this.story.TextContext.GetFontBaselineOffset (pt_size, properties, out pt_offset);
				
				ascender  = ot_font.GetAscender (pt_size) * font_scale;
				descender = ot_font.GetDescender (pt_size) * font_scale;
				angle     = ot_font.GetCaretAngle ();
				
				if (pt_offset != 0)
				{
					frame.MapFromView (ref cx, ref cy);
					cy += pt_offset;
					frame.MapToView (ref cx, ref cy);
				}
				
				return true;
			}
			else
			{
				cx = 0;
				cy = 0;
				
				ascender  = 0;
				descender = 0;
				angle     = 0;
				
				return false;
			}
		}
		
		
		public void ClearCurrentStylesAndProperties()
		{
			this.current_styles     = null;
			this.current_properties = null;
		}
		
		public void UpdateCurrentStylesAndPropertiesIfNeeded()
		{
			if ((this.current_styles == null) ||
				(this.current_properties == null))
			{
				this.UpdateCurrentStylesAndProperties ();
			}
		}
		
		public void UpdateCurrentStylesAndProperties()
		{
			System.Diagnostics.Debug.WriteLine ("Executing UpdateCurrentStylesAndProperties");
			
			TextStyle[] styles;
			Property[]  properties;
			
			//	En marche arri�re, on utilise le style du caract�re courant, alors
			//	qu'en marche avant, on utilise le style du caract�re pr�c�dent :
			
			int pos    = this.story.GetCursorPosition (this.cursor);
			int dir    = this.story.GetCursorDirection (this.cursor);
			int offset = ((pos > 0) && (dir > 0)) ? -1 : 0;
			
			if ((dir > -1) &&
				(this.IsAfterAutoText (this.cursor)))
			{
				//	Le caract�re pr�c�dent appartient � un texte automatique. Il faut
				//	consid�rer que l'on vient de reculer, pas d'avancer :
				
				offset = 0;
				
				System.Diagnostics.Debug.WriteLine ("--> just after AutoText");
			}
			
			if ((pos > 0) &&
				(pos == this.TextLength))
			{
				offset = -1;
			}
			
			if ((offset != 0) &&
				(pos < this.TextLength) &&
				(Internal.Navigator.IsParagraphStart (this.story, this.temp_cursor, 0)))
			{
				//	Au d�but d'un paragraphe, on prend toujours le style du premier caract�re
				//	du paragraphe, quelle que soit la direction (on ne veut pas h�riter du
				//	style du paragraphe pr�c�dent) :
				
				offset = 0;
			}
			
			ulong code = this.story.ReadChar (this.cursor, offset);
			
			if (code == 0)
			{
				if (this.TextContext.DefaultStyle != null)
				{
					styles     = new TextStyle[] { this.TextContext.DefaultStyle };
					properties = new Property[0];
				}
				else
				{
					styles     = new TextStyle[0];
					properties = new Property[0];
				}
			}
			else
			{
				this.TextContext.GetStylesAndProperties (code, out styles, out properties);
				
				System.Text.StringBuilder debug_styles = new System.Text.StringBuilder ();
				System.Text.StringBuilder debug_properties = new System.Text.StringBuilder ();
				
				foreach (TextStyle s in styles)
				{
					if (debug_styles.Length > 0)
					{
						debug_styles.Append ("/");
					}
					
					if (s.MetaId != null)
					{
						debug_styles.Append (s.MetaId);
						debug_styles.Append ("-");
						debug_styles.Append (s.Priority);
					}
					else
					{
						debug_styles.Append (s.Name);
					}
				}
				
				foreach (Property p in properties)
				{
					if (debug_properties.Length > 0)
					{
						debug_properties.Append ("/");
					}
					
					debug_properties.Append (p.WellKnownType);
				}
				
				if (debug_properties.Length == 0)
				{
					debug_properties.Append ("[none]");
				}
				
				System.Diagnostics.Debug.WriteLine (string.Format ("[{0}:{1}:{2}:{3}] -> {4} + {5}",
					/**/										   Internal.CharMarker.GetLocalIndex (code),
					/**/										   Internal.CharMarker.GetExtraIndex (code),
					/**/										   Internal.CharMarker.GetStyleIndex (code),
					/**/										   Unicode.Bits.GetUnicodeCode (code),
					/**/										   debug_styles, debug_properties));
				
				int n = 0;
				
				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i].PropertyAffinity != Properties.PropertyAffinity.Symbol)
					{
						n++;
					}
				}
				
				if (n != properties.Length)
				{
					Property[] old_properties = properties;
					Property[] new_properties = new Property[n];
					
					int j = 0;
					
					for (int i = 0; i < old_properties.Length; i++)
					{
						if (old_properties[i].PropertyAffinity != Properties.PropertyAffinity.Symbol)
						{
							new_properties[j] = old_properties[i];
							j++;
						}
					}
					
					properties = new_properties;
					
					System.Diagnostics.Debug.Assert (new_properties.Length == n);
					System.Diagnostics.Debug.Assert (new_properties.Length < old_properties.Length);
				}
			}
			
			this.current_styles      = styles;
			this.current_properties  = properties;
			
			this.RefreshAccumulatedStylesAndProperties ();
		}
		
		
		public void SuspendNotifications()
		{
			this.suspend_notifications++;
		}
		
		public void ResumeNotifications()
		{
			System.Diagnostics.Debug.Assert (this.suspend_notifications > 0);
			
			this.suspend_notifications--;
			
			if (this.suspend_notifications == 0)
			{
				if (this.notify_text_changed)
				{
					this.OnTextChanged ();
					this.notify_text_changed = false;
				}
				if (this.notify_tabs_changed)
				{
					this.OnTabsChanged ();
					this.notify_tabs_changed = false;
				}
				if (this.notify_style_changed)
				{
					this.OnActiveStyleChanged ();
					this.notify_style_changed = false;
				}
				if (this.notify_cursor_moved)
				{
					this.OnCursorMoved ();
					this.notify_cursor_moved = false;
				}
			}
		}
		
		
		public void ExternalNotifyTextChanged()
		{
			this.NotifyTextChanged ();
		}
		
		
		protected bool MoveCursor(ICursor cursor, int distance, out int new_pos, out int new_dir)
		{
			int count;
			int direction;
			int moved = 0;
			
			if (distance > 0)
			{
				count     = distance;
				direction = 1;
			}
			else
			{
				count     = -distance;
				direction = -1;
			}
			
			TextContext            context    = this.TextContext;
			Internal.TextTable text_table = this.story.TextTable;
			StyleList          style_list = context.StyleList;
			
			int pos = this.story.GetCursorPosition (cursor);
			
			this.story.SetCursorPosition (this.temp_cursor, pos);
			
			while (moved < count)
			{
				if ((direction > 0) &&
					(pos == this.story.TextLength))
				{
					break;
				}
				if ((direction < 0) &&
					(pos == 0))
				{
					break;
				}
				
				//	D�place le curseur dans la direction choisie, puis v�rifie si
				//	l'on n'a pas atterri dans un fragment de texte marqu� comme
				//	�tant un texte automatique ou un texte produit par un g�n�rateur.
				
				System.Diagnostics.Debug.Assert (this.story.GetCursorPosition (this.temp_cursor) == pos);
				
				ulong code;
				
				if (direction > 0)
				{
					code = this.story.ReadChar (this.temp_cursor);
					this.story.MoveCursor (this.temp_cursor, direction);
				}
				else
				{
					this.story.MoveCursor (this.temp_cursor, direction);
					code = this.story.ReadChar (this.temp_cursor);
				}
				
				if (code == 0)
				{
					System.Diagnostics.Debug.Assert (pos+1 == this.story.TextLength);
					System.Diagnostics.Debug.Assert (direction == 1);
					
					moved += 1;
					pos   += 1;
					
					break;
				}
				
				//	G�re le d�placement par-dessus des sections AutoText/Generator
				//	qui n�cessitent des traitements particuliers :
				
				Properties.AutoTextProperty  auto_text_property;
				Properties.GeneratorProperty generator_property;
				
				pos += direction;
				
				if (context.GetAutoText (code, out auto_text_property))
				{
					int skip = this.SkipOverProperty (this.temp_cursor, auto_text_property, direction);
					
					//	Un texte automatique compte comme z�ro caract�re dans nos
					//	d�placements.
					
					this.story.MoveCursor (this.temp_cursor, skip);
					
					pos   += skip;
					moved += 0;
				}
				else if (context.GetGenerator (code, out generator_property))
				{
					int skip = this.SkipOverProperty (this.temp_cursor, generator_property, direction);
					
					//	Un texte produit par un g�n�rateur compte comme un caract�re
					//	unique.
					
					this.story.MoveCursor (this.temp_cursor, skip);
					
					pos   += skip;
					moved += 1;
				}
				else
				{
					moved += 1;
				}
			}
			
			if ((moved > 0) &&
				(direction > 0))
			{
				if ((Internal.Navigator.IsLineEnd (this.story, this.fitter, this.temp_cursor, 0, direction) && ! Internal.Navigator.IsParagraphEnd (this.story, this.temp_cursor, 0)) ||
					(Internal.Navigator.IsParagraphStart (this.story, this.temp_cursor, 0)))
				{
					//	Si nous avons atteint la fin d'une ligne de texte en marche avant,
					//	on pr�tend que l'on se trouve au d�but de la ligne suivante; si on
					//	est arriv� au d�but d'un paragraphe, consid�re qu'on est au d�but
					//	de la ligne :
					
					direction = -1;
				}
			}
			
			new_pos = pos;
			new_dir = direction;
			
			return moved > 0;
		}
		
		protected bool MoveCursor(ICursor cursor, int count, int direction, MoveCallback callback, out int new_pos, out int new_dir)
		{
			int moved   = 0;
			int old_pos = this.story.GetCursorPosition (cursor);
			int old_dir = this.story.GetCursorDirection (cursor);
			
			TextContext            context    = this.TextContext;
			Internal.TextTable text_table = this.story.TextTable;
			StyleList          style_list = context.StyleList;
			
			System.Diagnostics.Debug.Assert (count >= 0);
			System.Diagnostics.Debug.Assert ((direction == -1) || (direction == 1));
			
			this.story.SetCursorPosition (this.temp_cursor, old_pos);
			
			if (direction > 0)
			{
				int dir = old_dir;
				int max = this.story.TextLength - old_pos;
				
				for (int i = 0; i < max; i++)
				{
					if (callback (i, dir))
					{
						if (count-- == 0)
						{
							break;
						}
					}
					else if ((i == 0) && (count > 0))
					{
						count--;
					}
					
					moved++;
					dir = 1;
				}
			}
			else
			{
				int dir = old_dir;
				int max = old_pos;
				
				for (int i = 0; i < max; i++)
				{
					if (callback (-i, dir))
					{
						if (count-- == 0)
						{
							break;
						}
					}
					else if ((i == 0) && (count > 0))
					{
						count--;
					}
					
					moved--;
					dir = -1;
				}
			}
			
			new_pos = old_pos + moved;
			new_dir = direction;
			
			if ((new_pos != old_pos) ||
				(new_dir != old_dir))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		protected void InsertText(string text)
		{
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			
			ulong[] styled_text;
			
			this.UpdateCurrentStylesAndPropertiesIfNeeded ();
			
			System.Collections.Stack starts = new System.Collections.Stack ();
			
			int pos = this.story.GetCursorPosition (this.cursor);
			
			if ((Internal.Navigator.IsEndOfText (this.story, this.cursor, 0)) &&
				(Internal.Navigator.IsParagraphStart (this.story, this.cursor, 0)))
			{
				starts.Push (pos);
			}
			
			this.story.ConvertToStyledText (text, this.current_styles, this.current_properties, out styled_text);
			this.story.InsertText (this.cursor, styled_text);
			
			//	Si le texte ins�r� contient un saut de paragraphe et que le style
			//	en cours fait r�f�rence � un gestionnaire de paragraphe n�cessitant
			//	l'ajout de texte automatique, il faut encore g�n�rer le texte auto.
			
			Property[] properties = this.accumulated_properties;
			
#if DEBUG
			for (int i = 0; i < properties.Length; i++)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0} : {1} -- {2}", i, properties[i].GetType ().Name, properties[i].ToString ()));
			}
#endif
			
			Properties.ManagedParagraphProperty mpp = null;
			
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i] is Properties.ManagedParagraphProperty)
				{
					mpp = properties[i] as Properties.ManagedParagraphProperty;
					break;
				}
			}
			
			if (mpp != null)
			{
				System.Diagnostics.Debug.WriteLine ("Found managed paragraph property.");
				
				for (int i = 0; i < styled_text.Length; i++)
				{
					if (Internal.Navigator.IsParagraphSeparator (styled_text[i]))
					{
						//	Ne g�n�re pas un changement de style de paragraphe si le texte se
						//	termine par un paragraphe vide.
						
						int start = pos + i + 1;
						
						if (start < this.story.TextLength)
						{
							starts.Push (pos + i + 1);
						}
					}
				}
				
				if (starts.Count > 0)
				{
					System.Diagnostics.Debug.WriteLine ("Handle insertion of new managed paragraphs.");
					ParagraphManagerList list = story.TextContext.ParagraphManagerList;
					
					while (starts.Count > 0)
					{
						pos = (int) starts.Pop ();
						
						System.Diagnostics.Debug.WriteLine ("--> start at " + pos.ToString ());
						
						this.story.SetCursorPosition (this.temp_cursor, pos);
						
						list[mpp.ManagerName].AttachToParagraph (this.story, this.temp_cursor, mpp);
					}
				}
			}
		}
		
		protected void DeleteText(ICursor cursor, int length)
		{
			//	Supprime le fragment de texte; il faut traiter sp�cialement la
			//	destruction des fins de paragraphes, car elle provoque le change-
			//	ment de style du paragraphe fragmentaire (le dernier morceau de
			//	paragraphe h�rite du style du premier paragraphe).
			
			System.Diagnostics.Debug.Assert (this.temp_cursor != cursor);
			
			ulong[] text = new ulong[length];
			this.story.ReadText (cursor, length, text);
			
			System.Collections.Stack ranges = new System.Collections.Stack ();
			
			//	V�rifie si le texte contient une marque de fin de paragraphe :
			
			int  count  = 0;
			int  pos    = this.story.GetCursorPosition (cursor);
			int  start  = pos;
			int  end    = pos + length;
			int  fence  = end;
			bool fix_up = false;
			
			for (int i = 0; i < length; i++)
			{
				System.Diagnostics.Debug.Assert (start <= pos+i);
				
				if (Internal.Navigator.IsParagraphSeparator (text[i]))
				{
					//	V�rifie si l'on d�truit un paragraphe complet (avec un
					//	�ventuel texte automatique au d�but)
					
					count++;
					
					if (this.SkipOverAutoText (ref start, -1))
					{
						//	Le d�but de la tranche � d�truire ne contenait pas le
						//	texte automatique. Nous devons �tendre la s�lection pour
						//	englober le texte automatique.
						
						this.story.SetCursorPosition (this.temp_cursor, start);
						
						System.Diagnostics.Debug.Assert (count == 1);
						System.Diagnostics.Debug.Assert (this.story.GetCursorPosition (this.temp_cursor) == start);
						System.Diagnostics.Debug.Assert (this.IsParagraphStart (0, -1));
					}
					else
					{
						//	Il n'y avait aucun texte automatique. Mais comme effet
						//	de bord, SkipOverAutoText a positionn�e le curseur
						//	temporaire :
						
						System.Diagnostics.Debug.Assert (this.story.GetCursorPosition (this.temp_cursor) == start);
					}
					
					Range range;
					
					if (this.IsParagraphStart (0, 1))
					{
						//	C'est un paragraphe complet qui est s�lectionn�. On le
						//	d�truira sans autre forme de proc�s.
						
						range = new Range (start, pos+i - start + 1);
					}
					else
					{
						//	Le paragraphe n'est pas s�lectionn� depuis le d�but; on va
						//	devoir appliquer notre style au reste du paragraphe suivant
						//	au moment du "fix up" plus loin :
						
						System.Diagnostics.Debug.Assert (count == 1);
						
						range  = new Range (start, pos+i - start + 1);
						fix_up = true;
					}
					
					Range.Merge (ranges, range);
					
					start = pos + i + 1;
				}
			}
			
			if (start < end)
			{
				//	Il reste encore un fragment de d�but de paragraphe � d�truire :
				
				Range.Merge (ranges, new Range (start, end - start));
			}
			
			if (! fix_up)
			{
				//	Le premier paragraphe est s�lectionn� dans son entier. Cela
				//	implique que si la fin de la s�lection arrive au d�but d'un
				//	paragraphe contenant du texte automatique, il faut d�placer
				//	la fin apr�s la fin du paragraphe pr�c�dent, mais avant le
				//	texte automatique du paragraphe en cours (sinon on supprime
				//	du texte automatique qu'il faut conserver).
				
				if (this.SkipOverAutoText (ref fence, -1))
				{
					this.story.SetCursorPosition (this.temp_cursor, fence);
					
					System.Diagnostics.Debug.Assert (this.story.GetCursorPosition (this.temp_cursor) == fence);
					System.Diagnostics.Debug.Assert (this.IsParagraphStart (0, -1));
				}
			}
			
			while (ranges.Count > 0)
			{
				Range range = ranges.Pop () as Range;
				
				if (range.End > fence)
				{
					range.End = fence;
				}
				
				this.story.SetCursorPosition (this.temp_cursor, range.Start);
				this.story.DeleteText (this.temp_cursor, range.Length);
			}
			
			if (fix_up)
			{
				//	La destruction a combin� le d�but du premier paragraphe avec
				//	la fin du dernier paragraphe. Il faut donc appliquer les r�-
				//	glages associ�s au d�but � la fin aussi :
				
				TextStyle[] styles;
				Property[]  props;
				
				Internal.Navigator.GetParagraphStyles (this.story, this.temp_cursor, -1, out styles);
				Internal.Navigator.GetParagraphProperties (this.story, this.temp_cursor, -1, out props);
				
				if (styles == null) styles = new TextStyle[0];
				if (props == null)  props  = new Property[0];
				
				Internal.Navigator.SetParagraphStyles (this.story, this.temp_cursor, styles);
				Internal.Navigator.SetParagraphProperties (this.story, this.temp_cursor, Properties.ApplyMode.Overwrite, props);
			}
			
			
			//	Le curseur pourrait maintenant avoir une mise en page de paragraphe
			//	diff�rente de ce qu'il avait avant. Il faut mettre � jour juste la
			//	partie "paragraphe" du style et des propri�t�s...
			
			Property[]  old_properties = this.current_properties;
			TextStyle[] old_styles     = this.current_styles;
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (old_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (old_properties) == false);
			
			//	Change l'�tat du curseur, comme s'il venait d'arriver o� il est; on
			//	perd donc les r�glages pr�c�dents, temporairement.
			
			this.UpdateCurrentStylesAndProperties ();
			
			System.Collections.ArrayList new_styles     = new System.Collections.ArrayList ();
			System.Collections.ArrayList new_properties = new System.Collections.ArrayList ();
			
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Paragraph));
			new_styles.AddRange (TextStyle.FilterStyles (old_styles, TextStyleClass.Text));
			new_styles.AddRange (TextStyle.FilterStyles (old_styles, TextStyleClass.Character));
			new_styles.AddRange (TextStyle.FilterStyles (old_styles, TextStyleClass.MetaProperty));
			
			new_properties.AddRange (Property.Filter (this.current_properties, Properties.PropertyFilter.UniformOnly));
			new_properties.AddRange (Property.Filter (old_properties, Properties.PropertyFilter.NonUniformOnly));
			
			//	Reg�n�re les styles et propri�t�s d'origine du curseur, pour ce qui
			//	concerne le texte, mais conserve les r�glages du paragraphe en cours.
			
			this.current_styles     = new_styles.ToArray (typeof (TextStyle)) as TextStyle[];
			this.current_properties = new_properties.ToArray (typeof (Property)) as Property[];
			
			this.RefreshAccumulatedStylesAndProperties ();
		}
		
		
		#region Range Class
		private class Range
		{
			public Range(int start, int length) : this (start, length, -1, -1)
			{
			}
			
			public Range(int start, int length, int i_start, int i_end)
			{
				this.start   = start;
				this.length  = length;
				this.i_start = i_start;
				this.i_end   = i_end;
			}
			
			
			public int							Start
			{
				get
				{
					return this.start;
				}
				set
				{
					if (this.start != value)
					{
						int end = this.End;
						
						this.start  = value;
						this.length = end - value;
					}
				}
			}
			
			public int							End
			{
				get
				{
					return this.start + this.length;
				}
				set
				{
					this.length = value - this.start;
				}
			}
			
			public int							Length
			{
				get
				{
					return this.length;
				}
			}
			
			public int							StartIndex
			{
				get
				{
					return this.i_start;
				}
			}
			
			public int							EndIndex
			{
				get
				{
					return this.i_end;
				}
			}
			
			
			public static void Merge(System.Collections.Stack ranges, Range new_range)
			{
				//	Ins�re une nouvelle zone s�lectionn�e. Si elle rejoint parfaitement
				//	la zone pr�c�dente, on allonge simplement celle-ci. Dans le cas
				//	contraire, ajoute une zone dans la pile.
				
				if (ranges.Count > 0)
				{
					Range old_range = ranges.Peek () as Range;
					
					if (old_range.End == new_range.Start)
					{
						old_range.End   = new_range.End;
						old_range.i_end = new_range.i_end;
					}
					else
					{
						ranges.Push (new_range);
					}
				}
				else
				{
					ranges.Push (new_range);
				}
			}
			
			public static void Merge(System.Collections.ArrayList ranges, Range new_range)
			{
				//	Fusionne une nouvelle zone dans la liste des zones existantes. S'il
				//	y a recouvrement avec une zone existante, celle-ci sera agrandie.
				//	Si plusieurs zones se recouvrent, les zones recouvertes sont supprim�es
				//	de la liste.
				
				int pos = 0;
				
				for (int i = 0; i < ranges.Count; i++)
				{
					Range old_range = ranges[i] as Range;
					
					if ((new_range.Start <= old_range.End) &&
						(new_range.End >= old_range.Start))
					{
						//	Il y a un chevauchement. Fusionne les deux zones. Pour traiter
						//	correctement l'agrandissement, on pr�f�re retirer l'ancienne
						//	zone, l'agrandir et la fusionner � son tour :
						
						ranges.RemoveAt (i);
						
						if (old_range.Start > new_range.Start)
						{
							old_range.Start   = new_range.Start;
							old_range.i_start = new_range.i_start;
						}
						
						if (old_range.End < new_range.End)
						{
							old_range.End    = new_range.End;
							old_range.i_end  = new_range.i_end;
						}
						
						Range.Merge (ranges, old_range);
						
						return;
					}
					if (new_range.Start > old_range.End)
					{
						pos = i+1;
					}
				}
				
				ranges.Insert (pos, new_range);
			}
			
			
			public static Range[] CreateSortedRanges(int[] positions)
			{
				System.Diagnostics.Debug.Assert ((positions.Length % 2) == 0);
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				for (int i = 0; i < positions.Length; i += 2)
				{
					int p1 = positions[i+0];
					int p2 = positions[i+1];
					
					if (p1 < p2)
					{
						Range.Merge (list, new Range (p1, p2-p1, i+0, i+1));
					}
					else if (p1 > p2)
					{
						Range.Merge (list, new Range (p2, p1-p2, i+1, i+0));
					}
				}
				
				Range[] ranges = new Range[list.Count];
				list.CopyTo (ranges);
				return ranges;
			}
			
			
			private int							start;
			private int							length;
			private int							i_start;
			private int							i_end;
		}
		#endregion
		
		protected bool IsParagraphStart(int offset, int direction)
		{
			return Internal.Navigator.IsParagraphStart (this.story, this.temp_cursor, offset);
		}
		
		protected bool IsParagraphEnd(int offset, int direction)
		{
			return Internal.Navigator.IsParagraphEnd (this.story, this.temp_cursor, offset);
		}
		
		protected bool IsWordStart(int offset, int direction)
		{
			//	Si nous sommes � la fin d'un paragraphe, nous consid�rons que
			//	c'est une fronti�re de mot :
			
			if (Internal.Navigator.IsParagraphEnd (this.story, this.temp_cursor, offset))
			{
				return true;
			}
			
			return Internal.Navigator.IsWordStart (this.story, this.temp_cursor, offset);
		}
		
		protected bool IsWordEnd(int offset, int direction)
		{
			//	Si nous sommes � la fin d'un paragraphe nous sommes d�j� �
			//	une fin de mot :
			
			if (Internal.Navigator.IsParagraphEnd (this.story, this.temp_cursor, offset))
			{
				return true;
			}
			
			//	On d�termine que la fin d'un mot est la m�me chose que le d�but
			//	du mot suivant, pour la navigation :
			
			return Internal.Navigator.IsWordStart (this.story, this.temp_cursor, offset);
		}
		
		protected bool IsLineStart(int offset, int direction)
		{
			if (this.IsParagraphStart (offset, direction))
			{
				return true;
			}
			
			if (Internal.Navigator.IsLineStart (this.story, this.fitter, this.temp_cursor, offset, direction))
			{
				return true;
			}
			
			return false;
		}
		
		protected bool IsLineEnd(int offset, int direction)
		{
			if (this.IsParagraphEnd (offset, direction))
			{
				return true;
			}
			
			if (Internal.Navigator.IsLineEnd (this.story, this.fitter, this.temp_cursor, offset, direction))
			{
				return true;
			}
			
			return false;
		}
		
		
		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.story != null)
				{
					this.InternalClearSelection ();
					this.UpdateSelectionMarkers ();
					
					this.story.OpletExecuted -= new OpletEventHandler (this.HandleStoryOpletExecuted);
					this.story.RecycleCursor (this.cursor);
					this.story.RecycleCursor (this.temp_cursor);
					
					this.story  = null;
					this.cursor = null;
					this.temp_cursor = null;
				}
			}
		}
		
		
		private void SetParagraphStyles(int pos, TextStyle[] styles)
		{
			//	Pour modifier le style d'un paragraphe, il faut se placer au d�but
			//	du paragraphe :
			
			this.story.SetCursorPosition (this.temp_cursor, pos);
			
			int start = Internal.Navigator.GetParagraphStartOffset (this.story, this.temp_cursor);
			
			this.story.SetCursorPosition (this.temp_cursor, pos + start);
			
			Internal.Navigator.SetParagraphStyles (this.story, this.temp_cursor, styles);
		}
		
		private void SetParagraphProperties(int pos, Properties.ApplyMode mode, Property[] properties)
		{
			this.story.SetCursorPosition (this.temp_cursor, pos);
			
			int start = Internal.Navigator.GetParagraphStartOffset (this.story, this.temp_cursor);
			
			this.story.SetCursorPosition (this.temp_cursor, pos + start);
			
			Internal.Navigator.SetParagraphProperties (this.story, this.temp_cursor, mode, properties);
		}
		
		private void SetTextStyles(int pos, int length, TextStyle[] styles)
		{
			this.story.SetCursorPosition (this.temp_cursor, pos);
			
			Internal.Navigator.SetTextStyles (this.story, this.temp_cursor, length, styles);
		}
		
		private void SetCharacterStyles(int pos, int length, TextStyle[] styles)
		{
			this.story.SetCursorPosition (this.temp_cursor, pos);
			
			Internal.Navigator.SetCharacterStyles (this.story, this.temp_cursor, length, styles);
		}
		
		private void SetTextProperties(int pos, int length, Properties.ApplyMode mode, Property[] properties)
		{
			this.story.SetCursorPosition (this.temp_cursor, pos);
			
			Internal.Navigator.SetTextProperties (this.story, this.temp_cursor, length, mode, properties);
		}
		
		private void SetMetaProperties(int pos, int length, Properties.ApplyMode mode, TextStyle[] meta_properties)
		{
			this.story.SetCursorPosition (this.temp_cursor, pos);
			
			Internal.Navigator.SetMetaProperties (this.story, this.temp_cursor, length, mode, meta_properties);
		}
		
		private void SetParagraphMetaProperties(int pos, Properties.ApplyMode mode, TextStyle[] meta_properties)
		{
			this.story.SetCursorPosition (this.temp_cursor, pos);
			
			int start = Internal.Navigator.GetParagraphStartOffset (this.story, this.temp_cursor);
			
			this.story.SetCursorPosition (this.temp_cursor, pos + start);
			
			int length = Internal.Navigator.GetParagraphEndLength (this.story, this.temp_cursor);
			
			Internal.Navigator.SetMetaProperties (this.story, this.temp_cursor, length, mode, meta_properties);
		}
		
		
		private void RefreshAccumulatedStylesAndProperties()
		{
			Styles.PropertyContainer.Accumulator current_accumulator = new Styles.PropertyContainer.Accumulator ();
			
			current_accumulator.SkipSymbolProperties = true;
			current_accumulator.Accumulate (this.story.FlattenStylesAndProperties (this.current_styles, this.current_properties));
			
			this.accumulated_properties = current_accumulator.AccumulatedProperties;
			
			//	G�n�re une "empreinte" des styles et propri�t�s actifs, ce qui va
			//	permettre de d�terminer si les r�glages ont chang� depuis la derni�re
			//	fois.
			
			System.Text.StringBuilder fingerprint = new System.Text.StringBuilder ();
			
			for (int i = 0; i < this.accumulated_properties.Length; i++)
			{
				System.Diagnostics.Debug.Assert (this.accumulated_properties[i].WellKnownType != Properties.WellKnownType.AutoText);
				
				fingerprint.Append (this.accumulated_properties[i].ToString ());
			}
			
			if (this.accumulated_properties_fingerprint != fingerprint.ToString ())
			{
				this.accumulated_properties_fingerprint = fingerprint.ToString ();
				System.Diagnostics.Debug.WriteLine (string.Format ("Property Fingerprint: {0}", this.accumulated_properties_fingerprint));
				
				this.NotifyActiveStyleChanged ();
			}
		}
		
		private void RefreshTabInfosFingerprint()
		{
			System.Text.StringBuilder fingerprint = new System.Text.StringBuilder ();
			TabInfo[] infos = this.GetTabInfos ();
			
			foreach (TabInfo info in infos)
			{
				fingerprint.Append (info.Tag);
				fingerprint.Append (info.Status);
			}
			
			if (this.accumulated_tab_info_fingerprint != fingerprint.ToString ())
			{
				this.accumulated_tab_info_fingerprint = fingerprint.ToString ();
				System.Diagnostics.Debug.WriteLine (string.Format ("TabInfo Fingerprint: {0}", this.accumulated_tab_info_fingerprint));
				
				this.NotifyTabsChanged ();
			}
		}
		
		private int FindNextParagraphStart(int pos)
		{
			this.story.SetCursorPosition (this.temp_cursor, pos);
			
			int max = this.story.TextLength;
			
			for (int offset = 0; pos + offset < max; offset++)
			{
				if (Internal.Navigator.IsParagraphEnd (this.story, this.temp_cursor, offset))
				{
					return pos + offset + 1;
				}
			}
			
			return max;
		}
		
		
		private void InternalSetCursor(int new_pos, int new_dir)
		{
			this.story.SetCursorPosition (this.temp_cursor, new_pos, new_dir);

			if (Internal.Navigator.IsParagraphStart (this.story, this.temp_cursor, 0))
			{
				//	Le curseur n'a pas le droit de se trouver en d�but de paragraphe
				//	si celui-ci commence par du texte automatique, car on n'a pas le
				//	droit d'ins�rer de texte avant celui-ci.
				
				if (this.SkipOverAutoText (ref new_pos, 1))
				{
					new_dir = -1;
				}
			}
			
			if (Internal.Navigator.IsEndOfText (this.story, this.temp_cursor, -1))
			{
				//	Le curseur est au-del� de la fin du texte; il faut le ramener
				//	juste avant le caract�re marqueur de la fin du texte :
				
				new_pos -= 1;
				new_dir  = Internal.Navigator.IsParagraphStart (this.story, this.temp_cursor, -1) ? -1 : 1;
			}
			else if (Internal.Navigator.IsEndOfText (this.story, this.temp_cursor, 0))
			{
				//	Le curseur est exactement sur la fin du texte; il faut d�terminer
				//	la direction � utiliser :
				
				new_dir  = Internal.Navigator.IsParagraphStart (this.story, this.temp_cursor, 0) ? -1 : 1;
			}
			
			//	D�place le curseur "officiel" une seule fois. Ceci permet d'�viter
			//	qu'un appel � MoveTo provoque plusieurs enregistrements dans l'oplet
			//	queue active :
			
			this.story.SetCursorPosition (this.ActiveCursor, new_pos, new_dir);
			
			//	Met encore � jour les marques de s�lection ou les informations de
			//	format associ�es au curseur :
			
			if (this.IsSelectionActive)
			{
				this.UpdateSelectionMarkers ();
			}
			else
			{
				this.UpdateCurrentStylesAndProperties ();
			}
			
			this.NotifyCursorMoved ();
		}
		
		private void InternalInsertSelectionOplet()
		{
			int[] positions = this.GetSelectionCursorPositions ();
			this.InternalInsertSelectionOplet (positions);
		}
		
		private void InternalInsertSelectionOplet(int[] positions)
		{
			this.story.Insert (new ClearSelectionOplet (this, positions));
		}
		
		private void InternalInsertDeselectionOplet()
		{
			this.story.Insert (new DefineSelectionOplet (this));
		}
		
		private void InternalClearSelection()
		{
			if (this.selection_cursors != null)
			{
				foreach (Cursors.SelectionCursor cursor in this.selection_cursors)
				{
					this.RecycleSelectionCursor (cursor);
				}
				
				this.selection_cursors.Clear ();
				this.selection_cursors = null;
				
				this.active_selection_cursor = null;
			}
			
			System.Diagnostics.Debug.Assert (this.HasSelection == false);
		}
		
		private void InternalDefineSelection(int[] positions)
		{
			this.InternalClearSelection ();
			
			System.Diagnostics.Debug.Assert ((positions.Length % 2) == 0);
			
			for (int i = 0; i < positions.Length; i += 2)
			{
				Cursors.SelectionCursor c1 = this.NewSelectionCursor ();
				Cursors.SelectionCursor c2 = this.NewSelectionCursor ();
				
				this.selection_cursors.Add (c1);
				this.selection_cursors.Add (c2);
				
				this.story.SetCursorPosition (c1, positions[i+0]);
				this.story.SetCursorPosition (c2, positions[i+1]);
			}
			
			System.Diagnostics.Debug.Assert (this.HasSelection);
		}
		
		
		protected bool IsAfterAutoText(ICursor cursor)
		{
			ulong code = this.story.ReadChar (cursor, -1);
			
			if (code == 0)
			{
				return false;
			}
			
			Properties.AutoTextProperty  property;
			
			if (this.TextContext.GetAutoText (code, out property))
			{
				return true;
			}
			
			return false;
		}
		
		
		protected int SkipOverProperty(ICursor cursor, Property property, int direction)
		{
			//	Saute la propri�t�, en marche avant ou en marche arri�re. En cas
			//	de marche avant, on s'arr�te apr�s la tranche. En cas de marche
			//	arri�re, on s'arr�te juste au d�but de la tranche.
			//
			//	Retourne la distance � parcourir.
			
			if (direction < 0)
			{
				//	La distance au d�but de la tranche de texte va de 0 � -n.
				
				return Internal.Navigator.GetRunStartOffset (this.story, cursor, property);
			}
			else if (direction > 0)
			{
				//	La distance � la fin de la tranche de texte va de 1 � n.
				
				return Internal.Navigator.GetRunEndLength (this.story, cursor, property);
			}
			
			return 0;
		}
		
		protected bool SkipOverAutoText(ref int pos, int direction)
		{
			bool hit = false;
			
			if (direction > 0)
			{
				for (;;)
				{
					this.story.SetCursorPosition (this.temp_cursor, pos);
					
					ulong code = this.story.ReadChar (this.temp_cursor);
					
					if (code == 0)
					{
						break;
					}
					
					//	G�re le d�placement par-dessus la section AutoText, s'il y en a
					//	une :
					
					Properties.AutoTextProperty  property;
					
					if (! this.TextContext.GetAutoText (code, out property))
					{
						break;
					}
					
					System.Diagnostics.Debug.Assert (property != null);
					System.Diagnostics.Debug.Assert (property.Tag != null);
					
					pos += this.SkipOverProperty (this.temp_cursor, property, 1);
					hit  = true;
				}
			}
			else if (direction < 0)
			{
				while (pos > 0)
				{
					this.story.SetCursorPosition (this.temp_cursor, pos);
					
					ulong code = this.story.ReadChar (this.temp_cursor, -1);
					
					//	G�re le d�placement par-dessus la section AutoText, s'il y en a
					//	une :
					
					Properties.AutoTextProperty  property;
					
					if (! this.TextContext.GetAutoText (code, out property))
					{
						break;
					}
					
					System.Diagnostics.Debug.Assert (property != null);
					System.Diagnostics.Debug.Assert (property.Tag != null);
					
					pos += this.SkipOverProperty (this.temp_cursor, property, -1);
					hit  = true;
				}
			}
			
			return hit;
		}
		
		
		protected Cursors.SelectionCursor NewSelectionCursor()
		{
			//	Retourne un curseur utilisable pour une s�lection. S'il existe
			//	encore des zombies, on les retourne � la vie plut�t que de
			//	cr�er de nouveaux curseurs.
			
			if (this.selection_cursors == null)
			{
				this.selection_cursors = new System.Collections.ArrayList ();
			}
			
			Cursors.SelectionCursor cursor = new Cursors.SelectionCursor ();
			
			this.story.NewCursor (cursor);
			
			return cursor;
		}
		
		protected void RecycleSelectionCursor(Cursors.SelectionCursor cursor)
		{
			this.story.RecycleCursor (cursor);
		}
		
		
		protected void UpdateSelectionMarkers()
		{
			//	Met � jour les marques de s�lection dans le texte. On va op�rer
			//	en deux passes; d'abord on les enl�ve toutes, ensuite on g�n�re
			//	celles comprises entre deux marques de s�lection.
			
			ulong marker = this.TextContext.Markers.Selected;
			
			this.story.ChangeAllMarkers (marker, false);
			
			int[] positions = this.GetSelectionCursorPositions ();
			
			for (int i = 0; i < positions.Length; i += 2)
			{
				int p1 = positions[i+0];
				int p2 = positions[i+1];
				
				if (p1 > p2)
				{
					int pp = p1; p1 = p2; p2 = pp;
				}
					
				this.story.ChangeMarkers (p1, p2-p1, marker, true);
			}
		}
		
		
		private int[] GetSelectionCursorPositions()
		{
			int[] positions;
			
			if (this.selection_cursors == null)
			{
				positions = new int[0];
			}
			else
			{
				positions = new int[this.selection_cursors.Count];
				
				for (int i = 0; i < this.selection_cursors.Count; i++)
				{
					ICursor cursor = this.selection_cursors[i] as ICursor;
					
					positions[i] = this.story.GetCursorPosition (cursor);
				}
			}
			
			System.Diagnostics.Debug.Assert ((positions.Length % 2) == 0);
			
			return positions;
		}

		
		protected virtual void OnCursorMoved()
		{
			if (this.CursorMoved != null)
			{
				this.CursorMoved (this);
			}
		}
		
		protected virtual void OnOpletExecuted(Common.Support.OpletEventArgs e)
		{
			if ((e.Oplet is TextStory.CursorMoveOplet) ||
				(e.Oplet is TextNavigator.DefineSelectionOplet) ||
				(e.Oplet is TextNavigator.ClearSelectionOplet) ||
				(e.Oplet is TextStory.TextChangeOplet) ||
				(e.Oplet is TextStory.TextInsertOplet) ||
				(e.Oplet is TextStory.TextDeleteOplet))
			{
				this.UpdateCurrentStylesAndProperties ();
			}
			
			if (this.OpletExecuted != null)
			{
				this.OpletExecuted (this, e);
			}
			
			this.NotifyTextChanged ();
		}
		
		protected virtual void OnTextChanged()
		{
			if (this.TextChanged != null)
			{
				this.TextChanged (this);
			}
		}
		
		protected virtual void OnTabsChanged()
		{
			if (this.TabsChanged != null)
			{
				this.TabsChanged (this);
			}
		}
		
		protected virtual void OnActiveStyleChanged()
		{
			if (this.ActiveStyleChanged != null)
			{
				this.ActiveStyleChanged (this);
			}
		}
		
		
		private void HandleStoryOpletExecuted(object sender, OpletEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.story == sender);
			
			this.OnOpletExecuted (e);
		}
		
		
		private void NotifyUndoExecuted(Common.Support.AbstractOplet oplet)
		{
			this.OnOpletExecuted (new OpletEventArgs (oplet, Common.Support.OpletEvent.UndoExecuted));
		}
		
		private void NotifyRedoExecuted(Common.Support.AbstractOplet oplet)
		{
			this.OnOpletExecuted (new OpletEventArgs (oplet, Common.Support.OpletEvent.RedoExecuted));
		}
		
		private void NotifyTextChanged()
		{
			if (this.suspend_notifications == 0)
			{
				this.OnTextChanged ();
			}
			else
			{
				this.notify_text_changed = true;
			}
			
			this.RefreshTabInfosFingerprint ();
		}
		
		private void NotifyTabsChanged()
		{
			if (this.suspend_notifications == 0)
			{
				this.OnTabsChanged ();
			}
			else
			{
				this.notify_tabs_changed = true;
			}
		}
		
		private void NotifyCursorMoved()
		{
			if (this.suspend_notifications == 0)
			{
				this.OnCursorMoved ();
			}
			else
			{
				this.notify_cursor_moved = true;
			}
		}
		
		private void NotifyActiveStyleChanged()
		{
			if (this.suspend_notifications == 0)
			{
				this.OnActiveStyleChanged ();
			}
			else
			{
				this.notify_style_changed = true;
			}
		}
		
		
		#region ClearSelectionOplet Class
		/// <summary>
		/// La classe ClearSelectionOplet permet de g�rer l'annulation de la
		/// suppression d'une s�lection.
		/// </summary>
		protected class ClearSelectionOplet : Common.Support.AbstractOplet
		{
			public ClearSelectionOplet(TextNavigator navigator, int[] positions)
			{
				this.navigator = navigator;
				this.positions = positions;
			}
			
			
			public override Epsitec.Common.Support.IOplet Undo()
			{
				this.navigator.InternalDefineSelection (this.positions);
				this.navigator.UpdateSelectionMarkers ();
				
				this.navigator.NotifyUndoExecuted (this);
				
				return this;
			}
			
			public override Epsitec.Common.Support.IOplet Redo()
			{
				this.navigator.InternalClearSelection ();
				this.navigator.UpdateSelectionMarkers ();
				
				this.navigator.NotifyRedoExecuted (this);
				
				return this;
			}
			
			public override void Dispose()
			{
				base.Dispose ();
			}
			
			
			private TextNavigator				navigator;
			private int[]						positions;
		}
		#endregion
		
		#region DefineSelectionOplet Class
		/// <summary>
		/// La classe DefineSelectionOplet permet de g�rer l'annulation de la
		/// d�finition d'une s�lection.
		/// </summary>
		protected class DefineSelectionOplet : Common.Support.AbstractOplet
		{
			public DefineSelectionOplet(TextNavigator navigator)
			{
				this.navigator = navigator;
			}
			
			
			public override Epsitec.Common.Support.IOplet Undo()
			{
				this.positions = this.navigator.GetSelectionCursorPositions ();
				
				this.navigator.InternalClearSelection ();
				this.navigator.UpdateSelectionMarkers ();
				
				this.navigator.NotifyUndoExecuted (this);
				
				return this;
			}
			
			public override Epsitec.Common.Support.IOplet Redo()
			{
				this.navigator.InternalDefineSelection (this.positions);
				this.navigator.UpdateSelectionMarkers ();
				
				this.navigator.NotifyRedoExecuted (this);
				
				return this;
			}
			
			public override void Dispose()
			{
				base.Dispose ();
			}
			
			
			private TextNavigator				navigator;
			private int[]						positions;
		}
		#endregion
		
		#region Target Enumeration
		public enum Target
		{
			None,
			
			CharacterNext,
			CharacterPrevious,
			
			TextStart,
			TextEnd,
			
			ParagraphStart,
			ParagraphEnd,
			
			LineStart,
			LineEnd,
			
			WordStart,
			WordEnd,
		}
		#endregion
		
		#region TabInfo Class
		public class TabInfo
		{
			public TabInfo(string tag, TabStatus status)
			{
				this.tag    = tag;
				this.status = status;
			}
			
			
			public string						Tag
			{
				get
				{
					return this.tag;
				}
			}
			
			public TabStatus					Status
			{
				get
				{
					return this.status;
				}
			}
			
			public TabClass						Class
			{
				get
				{
					return TabList.GetTabClass (this.tag);
				}
			}
			
			
			private string						tag;
			private TabStatus					status;
		}
		#endregion
		
		protected delegate bool MoveCallback(int offset, int direction);
		
		public event OpletEventHandler			OpletExecuted;
		public event EventHandler				TextChanged;
		public event EventHandler				TabsChanged;
		public event EventHandler				CursorMoved;
		public event EventHandler				ActiveStyleChanged;
		
		private TextStory						story;
		private TextFitter						fitter;
		private Cursors.SimpleCursor			cursor;
		private Cursors.TempCursor				temp_cursor;
		private Cursors.SelectionCursor			active_selection_cursor;
		private System.Collections.ArrayList	selection_cursors;
		
		private TextStyle[]						current_styles;
		private Property[]						current_properties;
		private Property[]						accumulated_properties;
		private string							accumulated_properties_fingerprint;
		private string							accumulated_tab_info_fingerprint;
		
		private int								suspend_notifications;
		private bool							notify_text_changed;
		private bool							notify_tabs_changed;
		private bool							notify_cursor_moved;
		private bool							notify_style_changed;
	}
}
