//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Text
{
	using EventHandler		= Epsitec.Common.Support.EventHandler;
	using OpletEventHandler = Epsitec.Common.Support.OpletEventHandler;
	using OpletEventArgs	= Epsitec.Common.Support.OpletEventArgs;
	
	/// <summary>
	/// La classe TextNavigator permet de manipuler un TextStory en vue de son
	/// édition.
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
			this.story.TextChanged   += new EventHandler (this.HandleStoryTextChanged);
			
			this.story.TextContext.TabList.Changed += new EventHandler (this.HandleTabListChanged);
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
				
				TextStyle[] styles;
				
				styles = this.current_styles.Clone () as TextStyle[];
				styles = this.TextContext.RemoveDeadStyles (styles);
				styles = this.TextContext.AddDefaultTextStyleIfNeeded (styles);
				
				return styles;
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
		
		public TextStory						TextStory
		{
			get
			{
				return this.story;
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
			//	Donne la position du curseur, comprise en 0 et TextLength.
			get
			{
				return this.story.GetCursorPosition (this.ActiveCursor);
			}
		}
		
		public int								CursorDirection
		{
			//	Donne la "direction" du curseur, c'est-à-dire:
			//	-1 = on est arrivé en marche arrière
			//	0 = on est arrivé par un clic souris
			//	1 = on est arrivé en marche avant
			//	Par exemple, lorsque le curseur est à cheval entre des caractères normaux et
			//	gras, la direction détermine la typographie active.
			get
			{
				return this.story.GetCursorDirection (this.ActiveCursor);
			}
		}
		
		public bool								IsSelectionActive
		{
			//	Une sélection active est une sélection pour laquelle on a fait un StartSelection
			//	mais pas encore le EndSelection.
			get
			{
				return this.active_selection_cursor == null ? false : true;
			}
		}
		
		public bool								HasSelection
		{
			//	Indique s'il existe une sélection. Après un EndSelection, la sélection existe toujours.
			//	Après un ClearSelection, la sélection n'existe plus.
			get
			{
				return this.selection_cursors == null ? false : true;
			}
		}
		
		public bool								HasRealSelection
		{
			//	Comme HasSelection, mais en plus, la sélection doit comporter au moins un caractère.
			get
			{
				if (this.selection_cursors != null)
				{
					int[] pos = this.GetAdjustedSelectionCursorPositions ();
					
					for (int i = 0; i < pos.Length; i += 2)
					{
						if (pos[i+0] != pos[i+1])
						{
							return true;
						}
					}
				}
				
				return false;
			}
		}
		
		public int								SelectionCount
		{
			//	Nombre de sélections en cours. 0 indique qu'il n'y a aucune sélection.
			//	1 indique qu'il existe une sélection normale. Un nombre plus grand que 1
			//	correspond à une sélection multiple discontinue.
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
				else if ((this.selection_cursors != null) &&
					/**/ (this.selection_cursors.Count >= 2))
				{
#if true
					return this.cursor;
#else
					int n = this.selection_cursors.Count;
					
					return this.selection_cursors[n-1] as ICursor;
#endif
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
		
		#region Direction Enumeration
		public enum Direction
		{
			Backward	= -1,
			None		= 0,
			Forward		= 1,
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
			//	Insère un caractère spécial qui doit être lié à la propriété de
			//	manière inamovible.
			
			System.Diagnostics.Debug.Assert (property != null);
			System.Diagnostics.Debug.Assert (property.PropertyAffinity == Properties.PropertyAffinity.Symbol);
			
			//	La propriété passée en entrée est simplement ajoutée en fin de
			//	liste des propriétés associées au curseur, temporairement :
			
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

		public void InsertWithTabs(string text)
		{
			string[] fragments = text.Split ('\t');

			this.Insert (fragments[0]);

			for (int i = 1; i < fragments.Length; i++)
			{
				string tag = this.FindInsertionTabTag ();

				if (tag == null)
				{
					this.Insert (" ");
				}
				else
				{
					this.Insert (Text.Unicode.Code.HorizontalTab, new Text.Properties.TabProperty (tag));
				}

				if (fragments[i].Length > 0)
				{
					this.Insert (fragments[i]);
				}
			}
		}

		public void Insert(string text)
		{
			//	On n'a pas le droit d'insérer des tabulateurs avec cette méthode,
			//	car il faudrait connaître la position à atteindre :

			//  [MW] Est-ce qu'il y a besoin de tester ça en mode Release ??
			
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
			
			//	Supprime le contenu de la sélection (pour autant qu'il y en ait
			//	une qui soit définie).
			
			if (this.selection_cursors != null)
			{
				this.story.SuspendTextChanged ();
				
				Internal.TextTable text = this.story.TextTable;
				
				using (this.story.BeginAction ())
				{
					this.InternalInsertSelectionOplet ();
					
					for (int i = 0; i < this.selection_cursors.Count; i += 2)
					{
						//	Traite les tranches dans l'ordre, en les détruisant les
						//	unes après les autres.
						
						int[] selected_pos = this.GetAdjustedSelectionCursorPositions ();
						
						int p1 = selected_pos[i+0];
						int p2 = selected_pos[i+1];
						
						if (i+2 == this.selection_cursors.Count)
						{
							//	C'est la dernière tranche. Il faut positionner le curseur
							//	de travail au début de la zone et hériter des styles actifs
							//	à cet endroit :
							
							this.story.SetCursorPosition (this.cursor, p1, 0);
							this.UpdateCurrentStylesAndProperties ();
						}
						
						Cursors.TempCursor temp = new Cursors.TempCursor ();
						
						try
						{
							this.story.NewCursor (temp);
							this.story.SetCursorPosition (temp, p1);
							this.DeleteText (temp, p2-p1);
						}
						finally
						{
							this.story.RecycleCursor (temp);
						}
					}
					
					int pos = this.story.GetCursorPosition (this.cursor);
					int dir = this.story.GetCursorDirection (this.cursor);
					
					this.AdjustCursor (this.temp_cursor, Direction.Forward, ref pos, ref dir);
					this.story.SetCursorPosition (this.cursor, pos, dir);
					
					this.story.ValidateAction ();
				}
				
				this.InternalClearSelection ();
				this.UpdateSelectionMarkers ();
				
				this.story.ResumeTextChanged ();
			}
			
			this.NotifyTextChanged ();
		}
		
		public void Delete(Direction direction)
		{
			//	Détruit un caractère (un et un seul) en avant ou en arrière,
			//	ce qui correspond à l'action exécutée en cas de pression des
			//	touches Back et Delete.
			
			int move = (int) direction;
			
			System.Diagnostics.Debug.Assert (move != 0);
			System.Diagnostics.Debug.Assert (move == System.Math.Sign (move));
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			
			int p1 = this.story.GetCursorPosition (this.cursor);
			int p2;
			int d2;
			
			Cursors.TempCursor temp = new Cursors.TempCursor ();
			
			this.story.NewCursor (temp);
			this.story.SetCursorPosition (temp, p1);
			
			this.MoveCursor (temp, move, out p2, out d2);
			this.story.SetCursorPosition (temp, p2, d2);
			
			if ((direction == Direction.Backward) &&
				(Internal.Navigator.IsParagraphStart (this.story, temp, 0)) &&
				(this.GetParagraphManager (temp) != null))
			{
				//	L'utilisateur a pressé 'Back' dans un paragraphe, ce qui a
				//	déplacé le curseur en début de ligne. Si c'est un paragraphe
				//	"géré", il faut supprimer le paragraph manager (par ex. si
				//	on presse 'Back' immédiatement après une puce).
				
				this.story.RecycleCursor (temp);
				
				using (this.story.BeginAction ())
				{
					this.RemoveParagraphManager ();
					this.story.ValidateAction ();
				}
				
				return;
			}
			
			this.AdjustCursor (temp, direction, ref p2, ref d2);
			
			System.Diagnostics.Debug.Assert (p1 >= 0);
			System.Diagnostics.Debug.Assert (p1 <= this.story.TextLength);
			
			System.Diagnostics.Debug.Assert (p2 >= 0);
			System.Diagnostics.Debug.Assert (p2 <= this.story.TextLength);
			
			if (p2 < p1)
			{
				int pp = p1;
				p1 = p2;
				p2 = pp;
			}
		
			this.story.SetCursorPosition (temp, p2);
			
			if (Internal.Navigator.IsEndOfText (this.story, temp, -1))
			{
				//	La position du texte est telle que le curseur se trouve après
				//	la marque de fin de texte. Corrige sa position :
				
				p2 -= 1;
			}
			
			if (p2 > p1)
			{
				this.story.SuspendTextChanged ();
				this.story.SetCursorPosition (temp, p1);
				
				using (this.story.BeginAction ())
				{
					this.DeleteText (temp, p2-p1);
					
					int pos = this.story.GetCursorPosition (this.cursor);
					int dir = this.story.GetCursorDirection (this.cursor);
					
					this.AdjustCursor (this.temp_cursor, Direction.Forward, ref pos, ref dir);
					this.story.SetCursorPosition (this.cursor, pos, dir);
					
					this.story.ValidateAction ();
				}
				
				this.story.ResumeTextChanged ();
				this.NotifyTextChanged ();
			}
			
			this.story.RecycleCursor (temp);
		}
		
		public void MoveTo(int position, int direction)
		{
			this.AdjustCursor (this.temp_cursor, (Direction) System.Math.Sign (direction), ref position, ref direction);
			this.InternalSetCursor (position, direction);
		}
		
		public void MoveTo(Target target, int count)
		{
			System.Diagnostics.Debug.Assert (count >= 0);
			
			int old_pos = this.CursorPosition;
			int old_dir = this.CursorDirection;
			
			int new_pos;
			int new_dir;
			
			bool fix_dir = false;
			
			Direction direction = Direction.None;
			
			switch (target)
			{
				case Target.CharacterNext:
					this.MoveCursor (this.ActiveCursor, count, out new_pos, out new_dir);
					direction = Direction.Forward;
					fix_dir   = true;
					break;
				
				case Target.CharacterPrevious:
					this.MoveCursor (this.ActiveCursor, -count, out new_pos, out new_dir);
					direction = Direction.Backward;
					break;
				
				case Target.TextStart:
					new_pos   = 0;
					new_dir   = -1;
					direction = Direction.Backward;
					break;
				
				case Target.TextEnd:
					new_pos   = this.TextLength;
					new_dir   = 1;
					direction = Direction.Forward;
					break;
					
				case Target.ParagraphStart:
					this.MoveCursor (this.ActiveCursor, count, Direction.Backward, new MoveCallback (this.IsParagraphStart), out new_pos, out new_dir);
					direction = Direction.Backward;
					break;
				
				case Target.ParagraphEnd:
					this.MoveCursor (this.ActiveCursor, count, Direction.Forward, new MoveCallback (this.IsParagraphEnd), out new_pos, out new_dir);
					direction = Direction.Forward;
					break;
				
				case Target.LineStart:
					this.MoveCursor (this.ActiveCursor, count, Direction.Backward, new MoveCallback (this.IsLineStart), out new_pos, out new_dir);
					direction = Direction.Backward;
					break;
				
				case Target.LineEnd:
					this.MoveCursor (this.ActiveCursor, count, Direction.Forward, new MoveCallback (this.IsLineEnd), out new_pos, out new_dir);
					direction = Direction.Forward;
					break;
				
				case Target.WordStart:
					this.MoveCursor (this.ActiveCursor, count, Direction.Backward, new MoveCallback (this.IsWordStart), out new_pos, out new_dir);
					direction = Direction.Backward;
					break;
				
				case Target.WordEnd:
					this.MoveCursor (this.ActiveCursor, count, Direction.Forward, new MoveCallback (this.IsWordEnd), out new_pos, out new_dir);
					direction = Direction.Forward;
					fix_dir   = true;
					break;
					
				default:
					throw new System.NotSupportedException (string.Format ("Target {0} not supported", target));
			}
			
			if ((fix_dir) &&
				(new_dir == 1))
			{
				//	Si en marche avant, on arrive à la fin d'une ligne qui n'est pas
				//	une fin de paragraphe, alors il faut changer la direction, afin
				//	que le curseur apparaisse au début de la ligne suivante :
				
				this.story.SetCursorPosition (this.temp_cursor, new_pos);
				
				if ((Internal.Navigator.IsParagraphEnd (this.story, this.temp_cursor, 0) == false) &&
					(Internal.Navigator.IsLineEnd (this.story, this.fitter, this.temp_cursor, 0, 1)))
				{
					new_dir = -1;
				}
			}
			
			if ((old_pos != new_pos) ||
				(old_dir != new_dir))
			{
				this.AdjustCursor (this.temp_cursor, direction, ref new_pos, ref new_dir);
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
			//	Débute une sélection simple.
			
			//	A partir d'ici :
			//	- IsSelectionActive retourne true
			//	- HasSelection retourne true
			//	- HasRealSelection retourne false
			
			System.Diagnostics.Debug.Assert (! this.IsSelectionActive);
			System.Diagnostics.Debug.Assert (! this.HasSelection);
			
			this.StartDisjointSelection ();
			
			System.Diagnostics.Debug.Assert (this.selection_cursors.Count == 2);
		}
		
		public void StartDisjointSelection()
		{
			//	Débute une sélection disjointe. Cette méthode peut être appelée
			//	même lorsqu'une sélection est déjà définie.
			
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
		
		public void ContinueSelection()
		{
			//	Continue une sélection (terminée avec EndSelection). Il faudra
			//	donc de nouveau utiliser EndSelection.
			
			//	A partir d'ici :
			//	- IsSelectionActive retourne true
			//	- HasSelection retourne true
			//	- HasRealSelection retourne true ou false
			
			System.Diagnostics.Debug.Assert (! this.IsSelectionActive);
			System.Diagnostics.Debug.Assert (this.selection_cursors.Count > 1);
			
			this.selection_before = this.GetSelectionCursorPositions ();
			
			int n = this.selection_cursors.Count;
			
			Cursors.SelectionCursor c1 = this.selection_cursors[n-2] as Cursors.SelectionCursor;
			Cursors.SelectionCursor c2 = this.selection_cursors[n-1] as Cursors.SelectionCursor;
			
			System.Diagnostics.Debug.Assert (c1 != null);
			System.Diagnostics.Debug.Assert (c2 != null);
			
			this.active_selection_cursor = c2;
			this.NotifyCursorMoved ();
		}
		
		public void EndSelection()
		{
			//	Termine la sélection simple.

			//	A partir d'ici :
			//	- IsSelectionActive retourne false
			//	- HasSelection retourne true
			//	- HasRealSelection retourne true ou false
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive);
			
			this.active_selection_cursor = null;
			
			using (this.story.BeginAction ())
			{
				if ((this.selection_before != null) &&
					(this.selection_before.Length > 0))
				{
					this.InternalInsertDeselectionOplet (this.selection_before);
				}
				else
				{
					this.InternalInsertDeselectionOplet ();
				}
				
				this.selection_before = null;
				this.story.ValidateAction ();
			}
			
			this.NotifyCursorMoved ();
		}
		
		
		public void ClearSelection()
		{
			this.ClearSelection (Direction.None);
		}
		
		public void ClearSelection(Direction direction)
		{
			//	Supprime la sélection si elle existe. Le EndSelection doit avoir
			//	été fait.
			//	NB: Ceci ne détruit pas le texte sélectionné.
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			
			//	Désélectionne tout le texte.
			
			if (this.selection_cursors != null)
			{
				//	Prend note de la position des curseurs de sélection pour
				//	pouvoir restaurer la sélection en cas de UNDO :
				
				int[]   positions = this.GetSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				this.InternalClearSelection ();
				this.UpdateSelectionMarkers ();
				
				using (this.story.BeginAction ())
				{
					this.InternalInsertSelectionOplet (positions);
					
					//	Déplace le curseur de travail au début ou à la fin de la
					//	tranche sélectionnée, en fonction de la direction :
					
					if ((direction != Direction.None) &&
						(ranges.Length > 0))
					{
						int pos = (direction == Direction.Backward) ? ranges[0].Start : ranges[ranges.Length-1].End;
						this.story.SetCursorPosition (this.cursor, pos, (int) direction);
					}
					
					this.story.ValidateAction ();
				}
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
			
			int[] positions = this.GetAdjustedSelectionCursorPositions ();
			
			int p1 = positions[index*2+0];
			int p2 = positions[index*2+1];
			
			ulong[] buffer = new ulong[p2-p1];
			
			this.story.SetCursorPosition (this.temp_cursor, p1);
			this.story.ReadText (this.temp_cursor, p2-p1, buffer);
			
			return buffer;
		}
		
		public string[] GetSelectedTexts()
		{
			//	Retourne les textes sélectionnés, bruts, sans aucun formatage.
			//	S'il n'y en a pas, retourne un tableau vide.
			
			string[] texts;
			
			if (this.selection_cursors == null)
			{
				texts = new string[0];
			}
			else
			{
				texts = new string[this.SelectionCount];
				
				for (int i = 0; i < texts.Length; i++)
				{
					TextConverter.ConvertToString (this.GetRawSelection (i), out texts[i]);
				}
			}
			
			return texts;
		}
		
		public ulong[] GetSelectedLowLevelText(int index)
		{
			//	Retourne le texte sélectionné (au format interne) correspondant
			//	à la tranche sélectionnée 'index'.
			//	S'il n'y en a pas, retourne un tableau vide.
			
			if ((this.selection_cursors == null) ||
				(index >= this.SelectionCount))
			{
				return new ulong[0];
			}
			else
			{
				return this.GetRawSelection (index);
			}
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

			List<string> list = new List<string> ();
			
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

			return list.ToArray ();
		}
		
		public string FindInsertionTabTag()
		{
			//	Trouve le tag de tabulation à utiliser si on presse TAB à
			//	l'endroit actuel dans le texte.
			
			double x;
			
			this.GetCursorGeometry (out x);
			
			TabList  list = this.TextContext.TabList;
			string[] tags = this.GetAllTabTags ();
			
			double best_dx  = double.PositiveInfinity;
			string best_tag = null;
again:			
			for (int i = 0; i < tags.Length; i++)
			{
				double pos = list.GetTabPosition (new Properties.TabProperty (tags[i]));
				
				if (pos > x)
				{
					double dx = pos - x;
					
					if (dx < best_dx)
					{
						best_dx  = dx;
						best_tag = tags[i];
					}
				}
			}
			
			if ((best_tag == null) &&
				(x > 0))
			{
				//	Pas trouvé de tag après la position courante. Cherche encore
				//	à partir du début de la ligne !
				
				x = 0;
				goto again;
			}
			
			return best_tag;
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
//-					System.Diagnostics.Debug.WriteLine (string.Format ("Rename tab from {0} to {1}, {2} live, {3} defined", string.Join ("/", old_tags), new_tag, pos_1.Length, pos_2.Length));
					
					using (this.story.BeginAction ())
					{
						//	Remplace les propriétés TabProperty des divers TAB
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
					//	Trouvé le tabulateur dans au moins un des paragraphes
					//	concernés. Il y a donc bien quelque chose à faire :
					
					Text.Properties.TabsProperty tabs = new Text.Properties.TabsProperty (string.Concat ("-", tag));
					this.SetParagraphProperties (Text.Properties.ApplyMode.Combine, tabs);
					
					break;
				}
			}
		}
		
		public void RedefineTab(string tag, double position, Properties.SizeUnits units, double disposition, string docking_mark, TabPositionMode position_mode, string attribute)
		{
			this.story.SuspendTextChanged ();
			
			this.TextContext.TabList.RedefineTab (this.OpletQueue, this.TextStory, new Properties.TabProperty (tag), position, units, disposition, docking_mark, position_mode, attribute);
			
			int[] pos = this.FindTextTabPositions (tag);
			
			for (int i = 0; i < pos.Length; i++)
			{
				int start;
				int end;
				
				Internal.Navigator.GetParagraphPositions (this.story, pos[i], out start, out end);
				
				if (start < end)
				{
					this.story.NotifyTextChanged (start, end - start);
				}
				else
				{
					this.story.NotifyTextChanged (end, start - end);
				}
			}
			
			this.story.ResumeTextChanged ();
		}
		
		
		#region Private Tabulator Manipulation Methods
		private string[] GetParagraphTabTags()
		{
			//	Retourne les tabulateurs définis dans un paragraphe en se basant
			//	sur la propriété TabsProperty.
			
			List<Property> tabs_list = new List<Property> ();
			List<string> tags_list = new List<string> ();
			
			int length = 0;
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetAdjustedSelectionCursorPositions ();
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
			
			return tags_list.ToArray ();
		}
		
		private string[] GetTextTabTags()
		{
			//	Retourne la liste des tabulateurs en se basant sur les marques
			//	de tabulation elles-mêmes.

			List<string> tags_list = new List<string> ();
			
			int length = 0;
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetAdjustedSelectionCursorPositions ();
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

			return tags_list.ToArray ();
		}
		
		
		private Properties.TabsProperty GetTabsProperty(int pos)
		{
			this.story.SetCursorPosition (this.temp_cursor, pos);
			ulong code = this.story.ReadChar (this.temp_cursor);
			Properties.TabsProperty property;
			this.TextContext.GetTabs (code, out property);
			return property;
		}
		
		
		private void FindTextTabTags(ICollection<string> list, int pos)
		{
			//	Trouve les tags utilisés par des tabulateurs dans le texte
			//	du paragraphe :
			
			int start;
			int end;
			
			Internal.Navigator.GetParagraphPositions (this.story, pos, out start, out end);
			
			int length = end - start;
			
			if (length > 0)
			{
				ulong[] text = new ulong[length];
				
				this.story.ReadText (start, length, text);
				
				TextContext context = this.TextContext;
				
				for (int i = 0; i < length; i++)
				{
					if (Unicode.Bits.GetUnicodeCode (text[i]) == Unicode.Code.HorizontalTab)
					{
						//	Trouvé un tabulateur. Détermine le tag correspondant en
						//	analysant la propriété attachée :
						
						Properties.TabProperty property;
						Properties.AutoTextProperty auto_text;
						
						ulong code = text[i];
						
						context.GetTab (code, out property);
						context.GetAutoText (code, out auto_text);
						
						System.Diagnostics.Debug.Assert (property != null);
						System.Diagnostics.Debug.Assert (property.TabTag != null);
						
						if (auto_text == null)
						{
							//	Evite de lister les tabulateurs qui sont le résultat
							//	d'un texte automatique.
							
							if (list.Contains (property.TabTag) == false)
							{
								list.Add (property.TabTag);
							}
						}
					}
				}
			}
		}
		
		
		private int[] FindTextTabPositions(params string[] tags)
		{
			List<int> list = new List<int> ();
			
			int length = 0;
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetAdjustedSelectionCursorPositions ();
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

			return list.ToArray ();
		}
		
		private int[] FindTextTabsPositions(string[] tags)
		{
			List<int> list = new List<int> ();
			
			int length = 0;
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetAdjustedSelectionCursorPositions ();
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

			return list.ToArray ();
		}
		
		private void  FindTextTabPositions(ICollection<int> list, int pos, string[] tags)
		{
			//	Trouve la position des caractères TAB dand le texte du
			//	paragraphe qui correspondent à la marque de tabulation
			//	cherchée :
			
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
						//	Trouvé un tabulateur. Détermine le tag correspondant en
						//	analysant la propriété attachée :
						
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
		
		private void  FindTextTabsPositions(ICollection<int> list, int pos, string[] tags)
		{
			//	Trouve la position des paragraphes qui font référence au moyen
			//	d'un Properties.TabsProperty au tag spécifié :
			
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
			this.story.SuspendTextChanged ();
			this.InternalSetParagraphStyles (styles);
			this.story.ResumeTextChanged ();
		}
		
		public void SetTextStyles(params TextStyle[] styles)
		{
			this.story.SuspendTextChanged ();
			this.InternalSetTextStyles (styles);
			this.story.ResumeTextChanged ();
		}
		
		public void SetSymbolStyles(params TextStyle[] styles)
		{
			this.story.SuspendTextChanged ();
			this.InternalSetSymbolStyles (styles);
			this.story.ResumeTextChanged ();
		}

		public void SetMetaProperties(Properties.ApplyMode mode, params TextStyle[] styles)
		{
			this.story.SuspendTextChanged ();
			this.InternalSetMetaProperties (mode, styles);
			this.story.ResumeTextChanged ();
		}
		
		
		private void InternalSetParagraphStyles(TextStyle[] styles)
		{
			//	Change les styles du paragraphe attachés à la position courante (ou
			//	compris dans la sélection).
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			System.Diagnostics.Debug.Assert (styles != null);
			System.Diagnostics.Debug.Assert (styles.Length > 0);
			
			TextStyle[] paragraph_styles = TextStyle.FilterStyles (styles, TextStyleClass.Paragraph);
			
			System.Diagnostics.Debug.Assert (paragraph_styles.Length > 0);
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetAdjustedSelectionCursorPositions ();
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
							//	avoir modifié le texte (insertion de puces, par ex.), on doit
							//	redemander les positions :
							
							positions = this.GetAdjustedSelectionCursorPositions ();
						}
					}
					
					this.UpdateSelectionMarkers ();
					this.story.ValidateAction ();
				}
			}
			
			this.UpdateCurrentStylesAndPropertiesIfNeeded ();
			
			if (this.HasRealSelection == false)
			{
				this.SetParagraphStyles (this.story.GetCursorPosition (this.cursor), paragraph_styles);
			}

			List<TextStyle> new_styles = new List<TextStyle> ();
			
			new_styles.AddRange (paragraph_styles);
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Text));
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Symbol));
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.MetaProperty));
			
			this.current_styles = new_styles.ToArray ();
			
			this.RefreshAccumulatedStylesAndProperties ();
			this.NotifyTextChanged ();
		}
		
		private void InternalSetTextStyles(params TextStyle[] styles)
		{
			//	Change les styles du texte attachés à la position courante (ou
			//	compris dans la sélection).
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			System.Diagnostics.Debug.Assert (styles != null);
			System.Diagnostics.Debug.Assert (styles.Length > 0);
			
			TextStyle[] text_styles = TextStyle.FilterStyles (styles, TextStyleClass.Text);
			
			System.Diagnostics.Debug.Assert (text_styles.Length > 0);
			
			//	Si besoin, supprime le style de texte par défaut (il n'apporte rien
			//	dans ce contexte) :
			
			text_styles = story.TextContext.FilterDefaultTextStyle (text_styles);
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetAdjustedSelectionCursorPositions ();
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

				List<TextStyle> new_styles = new List<TextStyle> ();
				
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Paragraph));
				new_styles.AddRange (text_styles);
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Symbol));
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.MetaProperty));
				
				this.current_styles = new_styles.ToArray ();
				
				this.RefreshAccumulatedStylesAndProperties ();
			}
			
			this.NotifyTextChanged ();
		}
		
		private void InternalSetSymbolStyles(params TextStyle[] styles)
		{
			//	Change les styles des symboles attachés à la position courante (ou
			//	compris dans la sélection).
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			System.Diagnostics.Debug.Assert (styles != null);
			System.Diagnostics.Debug.Assert (styles.Length > 0);
			
			TextStyle[] character_styles = TextStyle.FilterStyles (styles, TextStyleClass.Symbol);
			
			System.Diagnostics.Debug.Assert (character_styles.Length > 0);
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetAdjustedSelectionCursorPositions ();
				Range[] ranges    = Range.CreateSortedRanges (positions);
				
				using (this.story.BeginAction ())
				{
					foreach (Range range in ranges)
					{
						int pos    = range.Start;
						int length = range.Length;
						
						this.SetSymbolStyles (pos, length, character_styles);
					}
					
					this.story.ValidateAction ();
				}
			}
			else
			{
				this.UpdateCurrentStylesAndPropertiesIfNeeded ();

				List<TextStyle> new_styles = new List<TextStyle> ();
				
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Paragraph));
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Text));
				new_styles.AddRange (character_styles);
				new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.MetaProperty));
				
				this.current_styles = new_styles.ToArray ();
				
				this.RefreshAccumulatedStylesAndProperties ();
			}
			
			this.NotifyTextChanged ();
		}
		
		private void InternalSetMetaProperties(Properties.ApplyMode mode, TextStyle[] styles)
		{
			//	Change les méta-propriétés attachées à la position courante (ou
			//	compris dans la sélection).
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			System.Diagnostics.Debug.Assert (styles != null);
			System.Diagnostics.Debug.Assert (styles.Length > 0);
			
			TextStyle[] meta_properties = TextStyle.FilterStyles (styles, TextStyleClass.MetaProperty);
			
			System.Diagnostics.Debug.Assert (meta_properties.Length == styles.Length);
			System.Diagnostics.Debug.Assert (meta_properties.Length > 0);
			
			bool is_uniform = false;
			
			if (mode == Properties.ApplyMode.ClearUniform)
			{
				is_uniform = true;
				mode       = Properties.ApplyMode.Clear;
			}
			else
			{
				foreach (TextStyle style in meta_properties)
				{
					if (style.RequiresUniformParagraph)
					{
						is_uniform = true;
						break;
					}
				}
			}
			
			if (this.HasSelection)
			{
				int[]   positions = this.GetAdjustedSelectionCursorPositions ();
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
								//	avoir modifié le texte (insertion de puces, par ex.), on doit
								//	redemander les positions :
								
								positions = this.GetAdjustedSelectionCursorPositions ();
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


			List<TextStyle> new_styles = new List<TextStyle> ();
			
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Paragraph));
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Text));
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Symbol));
			new_styles.AddRange (Internal.Navigator.Combine (TextStyle.FilterStyles (this.current_styles, TextStyleClass.MetaProperty), meta_properties, mode));
			
			this.current_styles = new_styles.ToArray ();
			
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
				int[]   positions = this.GetAdjustedSelectionCursorPositions ();
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

				List<Property> new_properties = new List<Property> ();
				
				new_properties.AddRange (Internal.Navigator.Combine (Property.Filter (this.current_properties, Properties.PropertyFilter.UniformOnly), paragraph_properties, mode));
				new_properties.AddRange (Property.Filter (this.current_properties, Properties.PropertyFilter.NonUniformOnly));
				
				this.current_properties = new_properties.ToArray ();
				
				this.RefreshFilterCurrentProperties ();
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
				int[]   positions = this.GetAdjustedSelectionCursorPositions ();
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

				List<Property> new_properties = new List<Property> ();
				
				new_properties.AddRange (Property.Filter (this.current_properties, Properties.PropertyFilter.UniformOnly));
				new_properties.AddRange (Internal.Navigator.Combine (Property.Filter (this.current_properties, Properties.PropertyFilter.NonUniformOnly), text_properties, mode));
				
				this.current_properties = new_properties.ToArray ();
				
				this.RefreshFilterCurrentProperties ();
				this.RefreshAccumulatedStylesAndProperties ();
			}
			
			this.NotifyTextChanged ();
		}
		
		
		public bool HitTest(ITextFrame frame, double cx, double cy, bool skip_invisible, out int position, out int direction)
		{
			position  = -1;
			direction = 0;
			
			if (frame != null)
			{
				if ((this.fitter.HitTestTextFrame (frame, cx, cy, skip_invisible, ref position, ref direction)) ||
					(position >= 0))
				{
					//	Vérifie encore si le curseur ne se trouve pas dans un
					//	fragment de texte automatique (texte auto. d'une liste
					//	par exemple). Si c'est le cas, il faut déplacer le
					//	curseur après le texte automatique.
					
					this.story.SetCursorPosition (this.temp_cursor, position, direction);
					
					if (this.GetParagraphManager (this.temp_cursor) != null)
					{
						if (this.SkipOverAutoText (ref position, Direction.Forward))
						{
							direction = 1;
						}
					}
					
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
					this.MoveCursor (temp, 0, Direction.Backward, new MoveCallback (this.IsRawLineStart), out new_pos, out new_dir);
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
					this.MoveCursor (temp, 0, Direction.Forward, new MoveCallback (this.IsLineEnd), out new_pos, out new_dir);
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
		
		
		public bool GetCursorGeometry(out double x)
		{
			ITextFrame frame;
			double y, ascender, descender, angle;
			
			return this.GetCursorGeometry (out frame, out x, out y, out ascender, out descender, out angle);
		}
		
		public bool GetCursorGeometry(out ITextFrame frame)
		{
			double x, y, ascender, descender, angle;
			
			return this.GetCursorGeometry (out frame, out x, out y, out ascender, out descender, out angle);
		}
		
		public bool GetCursorGeometry(out ITextFrame frame, out double cx, out double cy, out double ascender, out double descender, out double angle)
		{
			if (this.HasSelection)
			{
				int[] pos = this.GetAdjustedSelectionCursorPositions ();
				int   n   = pos.Length - 2;
				
				//	Si une sélection est active, on considère la position avant
				//	le dernier caractère sélectionné comme référence :
				
				if ((n >= 0) &&
					(pos[n+0] < pos[n+1]))
				{
					this.story.SetCursorPosition (this.temp_cursor, pos[n+1] - 1);
					
					return this.GetCursorGeometry (this.temp_cursor, out frame, out cx, out cy, out ascender, out descender, out angle);
				}
			}
			
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
					//	Cas particulier : le curseur se trouve tout seul en fin de pavé,
					//	sans aucun autre caractère dans la ligne.
					
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
				double        font_glue;
				
				this.story.TextContext.GetFont (properties, out ot_font);
				this.story.TextContext.GetFontSize (properties, out pt_size, out font_scale, out font_glue);
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
		
		
		public int GetRunLength(int max)
		{
			ulong last = 0;
			
			for (int offset = 0; offset < max; offset++)
			{
				ulong raw  = this.story.ReadChar (this.cursor, offset);
				ulong code = Internal.CharMarker.ExtractCoreAndSettings (raw);
				
				switch (Unicode.Bits.GetUnicodeCode (raw))
				{
					case Unicode.Code.Null:
					case Unicode.Code.EndOfText:
						return offset;
					case Unicode.Code.ParagraphSeparator:
						return (offset > 0) ? offset : 1;
				}
				
				if (offset == 0)
				{
					last = code;
				}
				else
				{
					if (last != code)
					{
						return offset;
					}
				}
			}
			
			return max;
		}
		
		public string ReadText(int length)
		{
			ulong[] buffer = new ulong[length];
			string  result;
			
			int read = this.story.ReadText (this.cursor, length, buffer);
			
			TextConverter.ConvertToString (buffer, read, out result);
			
			return result;
		}

		public int[] GetAdjustedSelectionCursorPositions()
		{
			int[] positions = this.GetSelectionCursorPositions ();

			for (int i = 0; i < positions.Length; i += 2)
			{
				int p1 = positions[i+0];
				int p2 = positions[i+1];

				if (p2 < p1)
				{
					p1 = positions[i+1];
					p2 = positions[i+0];
				}

				//	La position p1 dénote le début de la sélection et p2 sa fin.
				//	Procède à quelques ajustements pour tenir compte correctement
				//	des textes automatiques.

				if ((this.SkipOverAutoText (ref p2, Direction.Backward)) ||
					(this.IsAfterManagedParagraph (this.temp_cursor)) ||
					(p2+1 == this.TextLength))
				{
					this.SkipOverAutoText (ref p1, Direction.Backward);
				}

				positions[i+0] = p1;
				positions[i+1] = p2;
			}

			return positions;
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
//-			System.Diagnostics.Debug.WriteLine ("Executing UpdateCurrentStylesAndProperties");
			
			TextStyle[] styles;
			Property[]  properties;
			
			//	En marche arrière, on utilise le style du caractère courant, alors
			//	qu'en marche avant, on utilise le style du caractère précédent :
			
			int pos = this.story.GetCursorPosition (this.cursor);
			int dir = this.story.GetCursorDirection (this.cursor);
			
			if (this.HasSelection)
			{
				int[] sel = this.GetSelectionCursorPositions ();
				
				int sel_1 = sel[0];
				int sel_2 = sel[1];
				
				if (sel_1 < sel_2)
				{
					if (pos < sel_2)
					{
						dir = -1;
					}
					else if (pos > sel_1)
					{
						dir = 1;
					}
				}
				else
				{
					if (pos < sel_1)
					{
						dir = -1;
					}
					else if (pos > sel_2)
					{
						dir = 1;
					}
				}
			}
			
			int offset = ((pos > 0) && (dir > 0)) ? -1 : 0;
			
			if ((dir > -1) &&
				(this.IsAfterAutoText (this.cursor)))
			{
				//	Le caractère précédent appartient à un texte automatique. Il faut
				//	considérer que l'on vient de reculer, pas d'avancer :
				
				offset = 0;
				
//-				System.Diagnostics.Debug.WriteLine ("--> just after AutoText");
			}
			
			if ((pos > 0) &&
				(pos == this.TextLength))
			{
				offset = -1;
			}
			
			if ((offset != 0) &&
				(pos < this.TextLength) &&
				(Internal.Navigator.IsParagraphStart (this.story, this.cursor, 0)))
			{
				//	Au début d'un paragraphe, on prend toujours le style du premier caractère
				//	du paragraphe, quelle que soit la direction (on ne veut pas hériter du
				//	style du paragraphe précédent) :
				
				offset = 0;
			}
			
			ulong code = this.story.ReadChar (this.cursor, offset);
			
			if (code == 0)
			{
				if (this.TextContext.DefaultParagraphStyle != null)
				{
					styles     = new TextStyle[] { this.TextContext.DefaultParagraphStyle };
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
#if false				
				System.Diagnostics.Debug.WriteLine (string.Format ("[{0}:{1}:{2}:{3}] -> {4} + {5}",
					/**/										   Internal.CharMarker.GetLocalIndex (code),
					/**/										   Internal.CharMarker.GetExtraIndex (code),
					/**/										   Internal.CharMarker.GetCoreIndex (code),
					/**/										   Unicode.Bits.GetUnicodeCode (code),
					/**/										   debug_styles, debug_properties));
#endif		
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
			
			this.current_styles     = styles;
			this.current_properties = properties;
			
			this.RefreshFilterCurrentProperties ();
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
		
		
		private bool MoveCursor(ICursor cursor, int distance, out int new_pos, out int new_dir)
		{
			//	Déplace le curseur sur la distance indiquée. Saute les textes
			//	automatiques et ajuste la direction pour gérer correctement
			//	les fins de paragraphes.
			
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
			
			TextContext        context    = this.TextContext;
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
				
				//	Déplace le curseur dans la direction choisie, puis vérifie si
				//	l'on n'a pas atterri dans un fragment de texte marqué comme
				//	étant un texte automatique.
				
				System.Diagnostics.Debug.Assert (this.story.GetCursorPosition (this.temp_cursor) == pos);
				
				ulong code;
				
				//	En fonction de la direction de déplacement, il faut lire le
				//	caractère avant/après le déplacement :
				
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
				
				//	Gère le déplacement par-dessus des sections AutoText qui
				//	doivent être traitées comme indivisibles; idem pour les
				//	générateurs :
				
				Properties.AutoTextProperty  auto_text_property;
				Properties.GeneratorProperty generator_property;
				
				Property property_to_skip = null;
				
				pos += direction;
				
				if (context.GetAutoText (code, out auto_text_property))
				{
					property_to_skip = auto_text_property;
				}
				else if (context.GetGenerator (code, out generator_property))
				{
					property_to_skip = generator_property;
				}
				
				if (property_to_skip != null)
				{
					int skip = this.SkipOverProperty (this.temp_cursor, property_to_skip, direction);
					
					//	Un texte produit par un générateur (ou un texte automatique)
					//	compte comme un caractère unique pour la navigation.
					
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
					//	on prétend que l'on se trouve au début de la ligne suivante; si on
					//	est arrivé au début d'un paragraphe, considère qu'on est au début
					//	de la ligne :
					
					direction = -1;
				}
			}
			
			new_pos = pos;
			new_dir = direction;
			
			return moved > 0;
		}
		
		private bool MoveCursor(ICursor cursor, int count, Direction direction, MoveCallback callback, out int new_pos, out int new_dir)
		{
			//	Déplace le curseur en sautant le nombre d'éléments indiqué. Le
			//	callback permet de déterminer quand un élément est atteint.
			
			int moved   = 0;
			int old_pos = this.story.GetCursorPosition (cursor);
			int old_dir = this.story.GetCursorDirection (cursor);
			
			TextContext        context    = this.TextContext;
			Internal.TextTable text_table = this.story.TextTable;
			StyleList          style_list = context.StyleList;
			
			System.Diagnostics.Debug.Assert (count >= 0);
			System.Diagnostics.Debug.Assert ((direction == Direction.Backward) || (direction == Direction.Forward));
			
			this.story.SetCursorPosition (this.temp_cursor, old_pos);
			
			if (direction == Direction.Forward)
			{
				int dir = old_dir;
				int max = this.story.TextLength - old_pos;
				
				for (int i = 0; i < max; i++)
				{
					if (callback (i, (Direction) dir))
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
					if (callback (-i, (Direction) dir))
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
			new_dir = (int) direction;
			
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
		
		
		private void InsertText(string text)
		{
			this.story.SuspendTextChanged ();
			
			using (this.story.BeginAction ())
			{
				string[] args = text.Split ((char) Unicode.Code.ParagraphSeparator);
				int      last = args.Length-1;
				
				for (int i = 0; i < last; i++)
				{
					this.InternalInsertText (string.Concat (args[i], System.Char.ToString ((char) Unicode.Code.ParagraphSeparator)));
					
					if ((this.current_styles != null) &&
						(this.current_styles.Length > 0) &&
						(this.current_styles[0].NextStyle != null) &&
						(this.current_styles[0].NextStyle != this.current_styles[0]) &&
						(Internal.Navigator.IsParagraphEnd (this.story, this.cursor, 0)))
					{
						this.SetParagraphStyles (this.current_styles[0].NextStyle);
					}
				}
				
				this.InternalInsertText (args[last]);
				
				this.story.ValidateAction ();
			}
			
			this.story.ResumeTextChanged ();
		}
		
		private void DeleteText(ICursor cursor, int length)
		{
			//	Supprime le fragment de texte; il faut traiter spécialement la
			//	destruction des fins de paragraphes, car elle provoque le change-
			//	ment de style du paragraphe fragmentaire (le dernier morceau de
			//	paragraphe hérite du style du premier paragraphe).
			
			System.Diagnostics.Debug.Assert (this.temp_cursor != cursor);
			
			ulong[] text = new ulong[length];
			this.story.ReadText (cursor, length, text);
			
			System.Collections.Stack ranges = new System.Collections.Stack ();
			
			//	Vérifie si le texte contient une marque de fin de paragraphe :
			
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
					//	Vérifie si l'on détruit un paragraphe complet, à savoir
					//	si le départ de la sélection est sur une marque de début
					//	de paragraphe.
					
					count++;
					
					this.story.SetCursorPosition (this.temp_cursor, start);
					
					Range range;
					
					if (this.IsParagraphStart (0, Direction.Forward))
					{
						//	C'est un paragraphe complet qui est sélectionné. On le
						//	détruira sans autre forme de procès.
						
						range = new Range (start, pos+i - start + 1);
						
						IParagraphManager manager = this.GetParagraphManager (this.temp_cursor);
						
						if (manager != null)
						{
							//	TODO: on va supprimer un paragraphe appartenant à un
							//	paragraph manager particulier; il faut en tenir compte
							//	pour le undo/redo, la re-numérotation et les marques
							//	de début de séquence.
						}
					}
					else
					{
						//	Le paragraphe n'est pas sélectionné depuis le début; on va
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
				//	Il reste encore un fragment de début de paragraphe à détruire :
				
				if (! fix_up)
				{
					//	Le premier paragraphe est sélectionné dans son entier (ou
					//	il n'y a pas de premier paragraphe).
					//	Cela implique que si la fin de la sélection arrive au début
					//	d'un paragraphe contenant du texte automatique (sans inclure
					//	autre chose que tu texte automatique), il faut déplacer la
					//	fin après la fin du paragraphe précédent, mais avant le
					//	texte automatique du paragraphe en cours (sinon on supprime
					//	du texte automatique qu'il faut conserver).
				
					if (this.SkipOverAutoText (ref fence, Direction.Backward))
					{
						this.story.SetCursorPosition (this.temp_cursor, fence);
					
						System.Diagnostics.Debug.Assert (this.story.GetCursorPosition (this.temp_cursor) == fence);
						System.Diagnostics.Debug.Assert (this.IsParagraphStart (0, Direction.Backward));
						
						goto process_ranges;
					}
				}
				else
				{
					//	Nous allons devoir procéder à une fusion d'un fragment de
					//	paragraphe précédent avec ce paragraphe-ci. Si c'est un
					//	managed paragraph, on commence par le transformer en un
					//	paragraphe normal; ça simplifie ensuite la fusion.
					
					this.story.SetCursorPosition (this.temp_cursor, start);
					
					IParagraphManager manager = this.GetParagraphManager (this.temp_cursor);
					
					if (manager != null)
					{
						ICursor temp = new Cursors.TempCursor ();
						
						this.story.NewCursor (temp);
						this.story.SetCursorPosition (temp, fence);
						
						//	Modifie la paragraphe final pour en faire un paragraphe
						//	normal (supprime donc le texte automatique).
						
						//	ATTENTION: cet appel modifie la position du curseur temp_cursor !
						
						this.SetParagraphStyles (start, new TextStyle[] { this.TextContext.DefaultParagraphStyle });
						
						fence = this.story.GetCursorPosition (temp);
						this.story.RecycleCursor (temp);
					}
				}
				
				Range.Merge (ranges, new Range (start, fence - start));
			}
			
process_ranges:
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
				//	La destruction a combiné le début du premier paragraphe avec
				//	la fin du dernier paragraphe. Il faut donc appliquer les ré-
				//	glages associés au début, à la fin aussi :
				
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
			//	différente de ce qu'il avait avant. Il faut mettre à jour juste la
			//	partie "paragraphe" du style et des propriétés...
			
			Property[]  old_properties = this.current_properties;
			TextStyle[] old_styles     = this.current_styles;
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (old_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (old_properties) == false);
			
			//	Change l'état du curseur, comme s'il venait d'arriver où il est; on
			//	perd donc les réglages précédents, temporairement.
			
			this.UpdateCurrentStylesAndProperties ();

			List<TextStyle> new_styles     = new List<TextStyle> ();
			List<Property>  new_properties = new List<Property> ();
			
			new_styles.AddRange (TextStyle.FilterStyles (this.current_styles, TextStyleClass.Paragraph));
			new_styles.AddRange (TextStyle.FilterStyles (old_styles, TextStyleClass.Text));
			new_styles.AddRange (TextStyle.FilterStyles (old_styles, TextStyleClass.Symbol));
			new_styles.AddRange (TextStyle.FilterStyles (old_styles, TextStyleClass.MetaProperty));
			
			new_properties.AddRange (Property.Filter (this.current_properties, Properties.PropertyFilter.UniformOnly));
			new_properties.AddRange (Property.Filter (old_properties, Properties.PropertyFilter.NonUniformOnly));
			
			//	Regénère les styles et propriétés d'origine du curseur, pour ce qui
			//	concerne le texte, mais conserve les réglages du paragraphe en cours.
			
			this.current_styles     = new_styles.ToArray ();
			this.current_properties = new_properties.ToArray ();
			
			this.RefreshFilterCurrentProperties ();
			this.RefreshAccumulatedStylesAndProperties ();
			
			//	Si le paragraphe courant est "managed", il faut lui donner la
			//	possibilité d'être mis à jour si on a détruit une fin de para-
			//	graphe.
			
			if (count > 0)
			{
				Properties.ManagedParagraphProperty[] mpps = Properties.ManagedParagraphProperty.Filter (this.accumulated_properties);
				
				if (mpps.Length > 0)
				{
					Properties.ManagedParagraphProperty mpp = mpps[0];
					
					ParagraphManagerList list = this.story.TextContext.ParagraphManagerList;
					IParagraphManager manager = list[mpp.ManagerName];
					
					System.Diagnostics.Debug.Assert (manager != null, string.Format ("Cannot find ParagraphManager '{0}'", mpp.ManagerName));
					
					this.story.SetCursorPosition (this.temp_cursor, this.CursorPosition);
					this.story.MoveCursor (this.temp_cursor, Internal.Navigator.GetParagraphStartOffset (this.story, this.temp_cursor));
					
					manager.RefreshParagraph (this.story, this.temp_cursor, mpp);
				}
			}
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
				//	Insère une nouvelle zone sélectionnée. Si elle rejoint parfaitement
				//	la zone précédente, on allonge simplement celle-ci. Dans le cas
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
				//	Si plusieurs zones se recouvrent, les zones recouvertes sont supprimées
				//	de la liste.
				
				int pos = 0;
				
				for (int i = 0; i < ranges.Count; i++)
				{
					Range old_range = ranges[i] as Range;
					
					if ((new_range.Start <= old_range.End) &&
						(new_range.End >= old_range.Start))
					{
						//	Il y a un chevauchement. Fusionne les deux zones. Pour traiter
						//	correctement l'agrandissement, on préfère retirer l'ancienne
						//	zone, l'agrandir et la fusionner à son tour :
						
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
		
		protected bool IsParagraphStart(int offset, Direction direction)
		{
			return Internal.Navigator.IsParagraphStart (this.story, this.temp_cursor, offset);
		}
		
		protected bool IsParagraphEnd(int offset, Direction direction)
		{
			return Internal.Navigator.IsParagraphEnd (this.story, this.temp_cursor, offset);
		}
		
		protected bool IsWordStart(int offset, Direction direction)
		{
			//	Si nous sommes à la fin d'un paragraphe, nous considérons que
			//	c'est une frontière de mot :
			
			if (Internal.Navigator.IsParagraphEnd (this.story, this.temp_cursor, offset))
			{
				return true;
			}
			if ((this.IsAfterAutoText (this.temp_cursor, offset)) &&
				(this.IsAfterAutoText (this.temp_cursor, offset+1) == false))
			{
				return true;
			}
			if (this.IsAfterAutoText (this.temp_cursor, offset+1))
			{
				return false;
			}
			
			return Internal.Navigator.IsWordStart (this.story, this.temp_cursor, offset);
		}
		
		protected bool IsWordEnd(int offset, Direction direction)
		{
			//	Si nous sommes à la fin d'un paragraphe nous sommes déjà à
			//	une fin de mot :
			
			if (Internal.Navigator.IsParagraphEnd (this.story, this.temp_cursor, offset))
			{
				return true;
			}
			
			//	On détermine que la fin d'un mot est la même chose que le début
			//	du mot suivant, pour la navigation :
			
			return Internal.Navigator.IsWordStart (this.story, this.temp_cursor, offset);
		}
		
		protected bool IsLineStart(int offset, Direction direction)
		{
			if (this.IsRawLineStart (offset, direction))
			{
				return true;
			}
			if (this.IsAfterAutoText (this.temp_cursor, offset))
			{
				return true;
			}
			
			return false;
		}
		
		protected bool IsRawLineStart(int offset, Direction direction)
		{
			if (this.IsParagraphStart (offset, direction))
			{
				return true;
			}
			if (Internal.Navigator.IsLineStart (this.story, this.fitter, this.temp_cursor, offset, (int) direction))
			{
				return true;
			}
			if (Internal.Navigator.IsAfterLineBreak (this.story, this.temp_cursor, offset))
			{
				return true;
			}
			
			return false;
		}
		
		protected bool IsLineEnd(int offset, Direction direction)
		{
			if (this.IsParagraphEnd (offset, direction))
			{
				return true;
			}
			if (Internal.Navigator.IsLineEnd (this.story, this.fitter, this.temp_cursor, offset, (int) direction))
			{
				return true;
			}
			if (Internal.Navigator.IsAfterLineBreak (this.story, this.temp_cursor, offset+1))
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
					this.story.TextChanged   -= new EventHandler (this.HandleStoryTextChanged);

					this.story.TextContext.TabList.Changed -= new EventHandler (this.HandleTabListChanged);
					
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
			//	Pour modifier le style d'un paragraphe, il faut se placer au début
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
		
		private void SetSymbolStyles(int pos, int length, TextStyle[] styles)
		{
			this.story.SetCursorPosition (this.temp_cursor, pos);
			
			Internal.Navigator.SetSymbolStyles (this.story, this.temp_cursor, length, styles);
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
			
			Internal.Navigator.SetParagraphMetaProperties (this.story, this.temp_cursor, mode, meta_properties);
		}
		
		
		private void RefreshFilterCurrentProperties ()
		{
			System.Diagnostics.Debug.Assert (this.current_properties != null);

			List<Property> list = new List<Property> ();
			
			foreach (Property property in this.current_properties)
			{
				switch (property.WellKnownType)
				{
					case Properties.WellKnownType.AutoText:
					case Properties.WellKnownType.Generator:
						break;
					default:
						list.Add (property);
						break;
				}
			}
			
			if (list.Count < this.current_properties.Length)
			{
				this.current_properties = list.ToArray ();
			}
		}
		
		private void RefreshAccumulatedStylesAndProperties()
		{
			Styles.PropertyContainer.Accumulator current_accumulator = new Styles.PropertyContainer.Accumulator ();
			
			current_accumulator.SkipSymbolProperties = true;
			current_accumulator.Accumulate (this.story.FlattenStylesAndProperties (this.current_styles, this.current_properties));

			this.accumulated_properties = current_accumulator.AccumulatedProperties;
			
			//	Génère une "empreinte" des styles et propriétés actifs, ce qui va
			//	permettre de déterminer si les réglages ont changé depuis la dernière
			//	fois.
			
			System.Text.StringBuilder fingerprint = new System.Text.StringBuilder ();
			
			for (int i = 0; i < this.accumulated_properties.Length; i++)
			{
				//	Vérifie qu'aucune propriété AutoText ou Generator n'est venue
				//	se glisser dans la liste des propriétés accumulées :
				
				System.Diagnostics.Debug.Assert (this.accumulated_properties[i].WellKnownType != Properties.WellKnownType.AutoText);
				System.Diagnostics.Debug.Assert (this.accumulated_properties[i].WellKnownType != Properties.WellKnownType.Generator);
				
				fingerprint.Append (this.accumulated_properties[i].ToString ());
			}
			
			if (this.accumulated_properties_fingerprint != fingerprint.ToString ())
			{
				this.accumulated_properties_fingerprint = fingerprint.ToString ();
//-				System.Diagnostics.Debug.WriteLine (string.Format ("Property Fingerprint: {0}", this.accumulated_properties_fingerprint));
				
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
//-				System.Diagnostics.Debug.WriteLine (string.Format ("TabInfo Fingerprint: {0}", this.accumulated_tab_info_fingerprint));
				
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
		
		
		private void AdjustCursor(ICursor temp, Direction direction, ref int pos, ref int dir)
		{
			//	Ajuste la position du curseur pour éviter de placer celui-ci à
			//	des endroits "impossibles" (par ex. avant une puce).
			
			System.Diagnostics.Debug.Assert (temp.Attachment == CursorAttachment.Temporary);
			
			if (direction == Direction.None)
			{
				direction = Direction.Forward;
			}
			
			if (pos < 0)
			{
				pos = 0;
			}
			if (pos > this.story.TextLength)
			{
				pos = this.story.TextLength;
			}
			
			this.story.SetCursorPosition (temp, pos);
			
			//	Les "managed paragraphs" ont toute une logique intégrée qui peut
			//	déterminer si un curseur est positionné de manière correcte, ou
			//	non :
			
			IParagraphManager manager = this.GetParagraphManager (temp);
			
			if ((manager != null) &&
				(Internal.Navigator.IsParagraphStart (this.story, temp, 0)))
			{
				switch (direction)
				{
					case Direction.Forward:
						this.SkipOverAutoText (ref pos, Direction.Forward);
						break;
					
					case Direction.Backward:
						if (pos > 0)
						{
							pos--;
						}
						else
						{
							this.SkipOverAutoText (ref pos, Direction.Forward);
						}
						break;
				}
			}
		}
		
		private IParagraphManager GetParagraphManager(ICursor cursor)
		{
			return this.story.TextContext.GetParagraphManager (this.story.ReadChar (cursor));
		}
		
		private void RemoveParagraphManager()
		{
			//	Transforme le paragraphe courant (managed paragraph) en un para-
			//	graphe standard. Cela implique qu'il faut remettre le style par
			//	défaut.
			
			this.SetParagraphStyles (this.TextContext.DefaultParagraphStyle);
		}
		
		private Properties.ManagedParagraphProperty GetManagedParagraphProperty(ICursor cursor)
		{
			Properties.ManagedParagraphProperty mpp;
			ulong code = this.story.ReadChar (cursor);
			
			if (code != 0)
			{
				this.story.TextContext.GetManagedParagraph (code, out mpp);
			}
			else
			{
				mpp = null;
			}
			
			return mpp;
		}
		
		
		private void InternalInsertText(string text)
		{
			//	Insère un texte en utilisant les réglages (propriétés et styles)
			//	courants.
			
			System.Diagnostics.Debug.Assert (this.IsSelectionActive == false);
			System.Diagnostics.Debug.Assert (this.HasSelection == false);
			
			if (text.Length == 0)
			{
				return;
			}
			
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
			
			//	Si le texte inséré contient un saut de paragraphe et que le style
			//	en cours fait référence à un gestionnaire de paragraphe nécessitant
			//	l'ajout de texte automatique, il faut encore générer le texte auto.
			
			Properties.ManagedParagraphProperty[] mpps = Properties.ManagedParagraphProperty.Filter (this.accumulated_properties);
			
			if (mpps.Length > 0)
			{
				Properties.ManagedParagraphProperty mpp = mpps[0];
				
				for (int i = 0; i < styled_text.Length; i++)
				{
					if (Internal.Navigator.IsParagraphSeparator (styled_text[i]))
					{
						int start = pos + i + 1;
						
						if (start < this.story.TextLength)
						{
							//	Ne génère un changement de style de paragraphe que si
							//	le texte ne se termine pas par un paragraphe vide.
							
							starts.Push (start);
						}
					}
				}
				
				if (starts.Count > 0)
				{
					ParagraphManagerList list = this.story.TextContext.ParagraphManagerList;
					IParagraphManager manager = list[mpp.ManagerName];
					
					System.Diagnostics.Debug.Assert (manager != null, string.Format ("Cannot find ParagraphManager '{0}'", mpp.ManagerName));
					
					while (starts.Count > 0)
					{
						pos = (int) starts.Pop ();
						
						this.story.SetCursorPosition (this.temp_cursor, pos);
						
						manager.AttachToParagraph (this.story, this.temp_cursor, mpp);
					}
				}
			}
		}
		
		
		private void InternalSetCursor(int new_pos, int new_dir)
		{
			//	Positionne le curseur à l'endroit spécifié, en utilisant la
			//	direction d'approche indiquée.
			
			//	L'appelant doit avoir appelé AdjustCursor avant d'appeler la
			//	méthode InternalSetCursor afin de garantir que le curseur est
			//	positionné correctement par rapport à d'éventuels textes auto-
			//	matiques.
			
//-			System.Diagnostics.Debug.WriteLine (string.Format ("Pos: {0}, dir: {1}\n{2}", new_pos, new_dir, this.story.GetDebugAllStyledText ()));
			
			this.story.SetCursorPosition (this.temp_cursor, new_pos, new_dir);
#if false
			if (Internal.Navigator.IsParagraphStart (this.story, this.temp_cursor, 0))
			{
				//	Le curseur n'a pas le droit de se trouver en début de paragraphe
				//	si celui-ci commence par du texte automatique, car on n'a pas le
				//	droit d'insérer de texte avant celui-ci.
				
				if (this.SkipOverAutoText (ref new_pos, Direction.Forward))
				{
					new_dir = -1;
				}
			}
#endif
			if (Internal.Navigator.IsEndOfText (this.story, this.temp_cursor, -1))
			{
				//	Le curseur est au-delà de la fin du texte; il faut le ramener
				//	juste avant le caractère marqueur de la fin du texte :
				
				new_pos -= 1;
				new_dir  = Internal.Navigator.IsParagraphStart (this.story, this.temp_cursor, -1) ? -1 : 1;
			}
			else if (Internal.Navigator.IsEndOfText (this.story, this.temp_cursor, 0))
			{
				//	Le curseur est exactement sur la fin du texte; il faut déterminer
				//	la direction à utiliser :
				
				new_dir  = Internal.Navigator.IsParagraphStart (this.story, this.temp_cursor, 0) ? -1 : 1;
			}
			
			//	Déplace le curseur "officiel" une seule fois. Ceci permet d'éviter
			//	qu'un appel à MoveTo provoque plusieurs enregistrements dans l'oplet
			//	queue active :
			
			this.story.SetCursorPosition (this.ActiveCursor, new_pos, new_dir);
			
			//	Met encore à jour les marques de sélection ou les informations de
			//	format associées au curseur :
			
			if (this.IsSelectionActive)
			{
				this.UpdateSelectionMarkers ();
				this.UpdateCurrentStylesAndProperties ();
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
		
		private void InternalInsertDeselectionOplet(int[] positions)
		{
			this.story.Insert (new DefineSelectionOplet (this, positions));
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
			
			this.UpdateCurrentStylesAndProperties ();
		}
		
		
		protected bool IsAfterAutoText(ICursor cursor)
		{
			return this.IsAfterAutoText (cursor, 0);
		}
		
		protected bool IsAfterAutoText(ICursor cursor, int offset)
		{
			ulong code = this.story.ReadChar (cursor, offset-1);
			
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
		
		protected bool IsAfterManagedParagraph(ICursor cursor)
		{
			ulong code = this.story.ReadChar (cursor, -1);
			
			if (code == 0)
			{
				return false;
			}
			if (Internal.Navigator.IsParagraphSeparator (code))
			{
				Properties.ManagedParagraphProperty property;
				
				if (this.TextContext.GetManagedParagraph (code, out property))
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		protected int SkipOverProperty(ICursor cursor, Property property, int direction)
		{
			//	Saute la propriété, en marche avant ou en marche arrière. En cas
			//	de marche avant, on s'arrête après la tranche. En cas de marche
			//	arrière, on s'arrête juste au début de la tranche.
			//
			//	Retourne la distance à parcourir.
			
			if (direction < 0)
			{
				//	La distance au début de la tranche de texte va de 0 à -n.
				
				return Internal.Navigator.GetRunStartOffset (this.story, cursor, property);
			}
			else if (direction > 0)
			{
				//	La distance à la fin de la tranche de texte va de 1 à n.
				
				return Internal.Navigator.GetRunEndLength (this.story, cursor, property);
			}
			
			return 0;
		}
		
		protected bool SkipOverAutoText(ref int pos, Direction direction)
		{
			//	Saute le texte automatique et place le curseur temporaire avant/après
			//	celui-ci. Si aucun texte automatique n'existe, le curseur temporaire
			//	est tout de même positionné.
			
			bool hit = false;
			
			if (direction == Direction.Forward)
			{
				for (;;)
				{
					//	Important de faire le SetCursorPosition ici; c'est un effet
					//	de bord dont certaines méthodes dépendent !
					
					this.story.SetCursorPosition (this.temp_cursor, pos);
					
					ulong code = this.story.ReadChar (this.temp_cursor);
					
					if (code == 0)
					{
						break;
					}
					
					//	Gère le déplacement par-dessus la section AutoText, s'il y en a
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
			else if (direction == Direction.Backward)
			{
				for (;;)
				{
					//	Important de faire le SetCursorPosition ici; c'est un effet
					//	de bord dont certaines méthodes dépendent !
					
					this.story.SetCursorPosition (this.temp_cursor, pos);
					
					if (pos == 0)
					{
						break;
					}
					
					ulong code = this.story.ReadChar (this.temp_cursor, -1);
					
					//	Gère le déplacement par-dessus la section AutoText, s'il y en a
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
			else
			{
				//	Important de faire le SetCursorPosition ici; c'est un effet
				//	de bord dont certaines méthodes dépendent !
				
				this.story.SetCursorPosition (this.temp_cursor, pos);
			}
			
			return hit;
		}
		
		
		protected Cursors.SelectionCursor NewSelectionCursor()
		{
			//	Retourne un curseur utilisable pour une sélection. S'il existe
			//	encore des zombies, on les retourne à la vie plutôt que de
			//	créer de nouveaux curseurs.
			
			if (this.selection_cursors == null)
			{
				this.selection_cursors = new List<Cursors.SelectionCursor> ();
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
			//	Met à jour les marques de sélection dans le texte. On va opérer
			//	en deux passes; d'abord on les enlève toutes, ensuite on génère
			//	celles comprises entre deux marques de sélection.
			
			ulong marker = this.TextContext.Markers.Selected;
			
			this.story.ChangeAllMarkers (marker, false);
			
			int[] positions = this.GetAdjustedSelectionCursorPositions ();
			
			for (int i = 0; i < positions.Length; i += 2)
			{
				int p1 = positions[i+0];
				int p2 = positions[i+1];
				
				System.Diagnostics.Debug.Assert (p1 <= p2);
				
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
					ICursor cursor = this.selection_cursors[i];
					
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
				this.NotifyCursorMoved ();
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
			
			switch (e.Event)
			{
				case Common.Support.OpletEvent.RedoExecuted:
				case Common.Support.OpletEvent.UndoExecuted:
					this.OnOpletExecuted (e);
					break;
			}
		}
		
		private void HandleStoryTextChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.story == sender);
			
			this.UpdateCurrentStylesAndProperties ();
			this.NotifyTextChanged ();
		}
		
		private void HandleTabListChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.story.TextContext.TabList == sender);
			this.NotifyTabsChanged ();
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
		/// La classe ClearSelectionOplet permet de gérer l'annulation de la
		/// suppression d'une sélection.
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
		/// La classe DefineSelectionOplet permet de gérer l'annulation de la
		/// définition d'une sélection.
		/// </summary>
		internal class DefineSelectionOplet : Common.Support.AbstractOplet
		{
			public DefineSelectionOplet(TextNavigator navigator)
			{
				this.navigator = navigator;
			}
			
			public DefineSelectionOplet(TextNavigator navigator, int[] positions)
			{
				this.navigator = navigator;
				this.positions = positions;
			}
			
			
			public override Epsitec.Common.Support.IOplet Undo()
			{
				int[] old = this.positions;
				
				this.positions = this.navigator.GetSelectionCursorPositions ();
				
				if ((old == null) ||
					(old.Length == 0))
				{
					this.navigator.InternalClearSelection ();
				}
				else
				{
					this.navigator.InternalDefineSelection (old);
				}
				
				this.navigator.UpdateSelectionMarkers ();
				
				this.navigator.NotifyUndoExecuted (this);
				
				return this;
			}
			
			public override Epsitec.Common.Support.IOplet Redo()
			{
				int[] old = this.positions;
				
				this.positions = this.navigator.GetSelectionCursorPositions ();
				
				if ((old == null) ||
					(old.Length == 0))
				{
					this.navigator.InternalClearSelection ();
				}
				else
				{
					this.navigator.InternalDefineSelection (old);
				}
				
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
		
		protected delegate bool MoveCallback(int offset, Direction direction);
		
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
		private List<Cursors.SelectionCursor>	selection_cursors;
		private int[]							selection_before;
		
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
