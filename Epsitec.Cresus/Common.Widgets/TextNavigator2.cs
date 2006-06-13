//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextNavigator2 gère la navigation dans un texte en se basant sur
	/// des événements clavier/souris.
	/// </summary>
	public class TextNavigator2
	{
		public TextNavigator2()
		{
			this.disable_tab_key    = false;
			this.disable_return_key = false;
			this.disable_escape_key = false;
			
			this.allow_tabs   = true;
			this.allow_lines  = true;
			this.allow_frames = true;
			
			this.is_read_only = false;
		}
		
		
		public Text.TextNavigator				TextNavigator
		{
			get
			{
				return this.text_navigator;
			}
			set
			{
				if (this.text_navigator != value)
				{
					if (this.text_navigator != null)
					{
						this.text_navigator.ActiveStyleChanged -= new Support.EventHandler (this.HandleActiveStyleChanged);
						this.text_navigator.TabsChanged -= new Support.EventHandler (this.HandleTabsChanged);
						this.text_navigator.TextChanged -= new Support.EventHandler (this.HandleTextChanged);
						this.text_navigator.CursorMoved -= new Support.EventHandler (this.HandleCursorMoved);
					}
					
					this.text_navigator = value;
					
					if (this.text_navigator != null)
					{
						this.text_navigator.CursorMoved += new Support.EventHandler (this.HandleCursorMoved);
						this.text_navigator.TextChanged += new Support.EventHandler (this.HandleTextChanged);
						this.text_navigator.TabsChanged += new Support.EventHandler (this.HandleTabsChanged);
						this.text_navigator.ActiveStyleChanged += new Support.EventHandler (this.HandleActiveStyleChanged);
					}
				}
			}
		}
		
		
		public bool ProcessMessage(Message message, Drawing.Point pos, Text.ITextFrame frame)
		{
			switch (message.Type)
			{
				case MessageType.KeyDown:
					return this.ProcessKeyDown (message);
				
				case MessageType.KeyPress:
					return this.ProcessKeyPress (message);
				
				case MessageType.MouseDown:
					return this.ProcessMouseDown (message, pos, frame);
				
				case MessageType.MouseMove:
					return ( this.is_mouse_dragging && this.ProcessMouseDrag (pos, frame))
						|| (!this.is_mouse_dragging && this.ProcessMouseMove (pos, frame));
				
				case MessageType.MouseUp:
					return this.is_mouse_down && this.ProcessMouseUp (message, pos, frame);
			}
			
			return false;
		}
		
		
		#region Specialized Process Methods
		private bool ProcessKeyDown(Message message)
		{
			//	L'événement KeyDown doit être traité pour toutes les touches non
			//	alphabétiques (les curseurs, TAB, RETURN, etc.)
			
			bool processed = false;
			
			if (this.is_read_only == false)
			{
				switch (message.KeyCodeOnly)
				{
					case KeyCode.Return:	processed = this.ProcessReturnKey (message);	break;
					case KeyCode.Tab:		processed = this.ProcessTabKey (message);		break;
					case KeyCode.Back:		processed = this.ProcessBackKey (message);		break;
					case KeyCode.Delete:	processed = this.ProcessDeleteKey (message);	break;
					case KeyCode.Space:	    processed = this.ProcessSpaceKey (message);	    break;
				}
			}
			
			//	Navigation au clavier :
			
			switch (message.KeyCodeOnly)
			{
				case KeyCode.Escape:		processed = this.ProcessEscapeKey (message);		break;
				
				case KeyCode.Home:			processed = this.ProcessHomeKey (message);			break;
				case KeyCode.End:			processed = this.ProcessEndKey (message);			break;
				
				case KeyCode.ArrowLeft:		processed = this.ProcessLeftArrowKey (message);		break;
				case KeyCode.ArrowRight:	processed = this.ProcessRightArrowKey (message);	break;
				case KeyCode.ArrowUp:		processed = this.ProcessUpArrowKey (message);		break;
				case KeyCode.ArrowDown:		processed = this.ProcessDownArrowKey (message);		break;
				
				case KeyCode.PageUp:
				case KeyCode.PageDown:
					break;
			}
			
			
			message.Swallowed = processed;
			
			return processed;
		}
		
		
		private bool ProcessReturnKey(Message message)
		{
			if (this.disable_return_key)
			{
				if ((message.IsControlPressed == true) &&
					(message.IsAltPressed == false) &&
					(this.allow_lines))
				{
					if (message.IsShiftPressed)
					{
						return this.Insert (Text.Unicode.Code.LineSeparator);
					}
					else
					{
						return this.Insert (Text.Unicode.Code.ParagraphSeparator);
					}
				}
			}
			else
			{
				if ((message.IsAltPressed == false) &&
					(this.allow_lines))
				{
					if ((message.IsControlPressed) &&
						(this.allow_frames))
					{
						if (message.IsShiftPressed)										//	saut de colonne
						{
							return this.Insert (Text.Properties.BreakProperty.NewFrame);
						}
						else															//	saut de page
						{
							return this.Insert (Text.Properties.BreakProperty.NewPage);
						}
					}
					else
					{
						if (message.IsShiftPressed)										//	saut de ligne
						{
							return this.Insert (Text.Unicode.Code.LineSeparator);
						}
						else															//	saut de paragraphe
						{
							return this.Insert (Text.Unicode.Code.ParagraphSeparator);
						}
					}
				}
			}
			
			return false;
		}
		
		private bool ProcessTabKey(Message message)
		{
			if (this.disable_tab_key)
			{
				if ((message.IsControlPressed == true) &&
					(message.IsAltPressed == false) &&
					(this.allow_tabs))
				{
					return this.Insert (Text.Unicode.Code.HorizontalTab);
				}
			}
			else
			{
				if ((message.IsAltPressed == false) &&
					(this.allow_tabs))
				{
					return this.Insert (Text.Unicode.Code.HorizontalTab);
				}
			}
			
			return false;
		}
		
		private bool ProcessEscapeKey(Message message)
		{
			if (this.disable_escape_key == false)
			{
				this.EndSelection ();
			
				if (this.text_navigator.HasSelection)
				{
					this.text_navigator.ClearSelection ();
					return true;
				}
			}
			
			return false;
		}
		
		private bool ProcessBackKey(Message message)
		{
			this.EndSelection ();
			
			if (this.text_navigator.HasSelection)
			{
				this.text_navigator.Delete ();
				return true;
			}
			
			this.text_navigator.Delete (Text.TextNavigator.Direction.Backward);
			return true;
		}
		
		private bool ProcessDeleteKey(Message message)
		{
			this.EndSelection ();
			
			if (this.text_navigator.HasSelection)
			{
				this.text_navigator.Delete ();
				return true;
			}
			
			this.text_navigator.Delete (Text.TextNavigator.Direction.Forward);
			return true;
		}
		
		private bool ProcessSpaceKey(Message message)
		{
			if (message.IsControlPressed && message.IsShiftPressed)
			{
				this.Insert (Text.Unicode.Code.NoBreakSpace);
				return true;
			}
			return false;
		}
		
		private bool ProcessHomeKey(Message message)
		{
			this.PreProcessCursorMove (message.IsShiftPressed);
			this.ClearVerticalMoveCache ();
			this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, Text.TextNavigator.Direction.Backward);
			
			if (message.IsControlPressed)
			{
				this.text_navigator.MoveTo (Text.TextNavigator.Target.TextStart, 0);
			}
			else
			{
				this.text_navigator.MoveTo (Text.TextNavigator.Target.LineStart, 0);
			}
			
			return true;
		}
		
		private bool ProcessEndKey(Message message)
		{
			this.PreProcessCursorMove (message.IsShiftPressed);
			this.ClearVerticalMoveCache ();
			this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, Text.TextNavigator.Direction.Forward);
			
			if (message.IsControlPressed)
			{
				this.text_navigator.MoveTo (Text.TextNavigator.Target.TextEnd, 0);
			}
			else
			{
				this.text_navigator.MoveTo (Text.TextNavigator.Target.LineEnd, 0);
			}
			
			return true;
		}
		
		private bool ProcessLeftArrowKey(Message message)
		{
			this.PreProcessCursorMove (message.IsShiftPressed);
			this.ClearVerticalMoveCache ();
			
			if (this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, Text.TextNavigator.Direction.Backward))
			{
				if (message.IsControlPressed == false)
				{
					return true;
				}
			}
			
			if (message.IsControlPressed)
			{
				this.text_navigator.MoveTo (Text.TextNavigator.Target.WordStart, 1);
			}
			else
			{
				this.text_navigator.MoveTo (Text.TextNavigator.Target.CharacterPrevious, 1);
			}
			
			return true;
		}
		
		private bool ProcessRightArrowKey(Message message)
		{
			this.PreProcessCursorMove (message.IsShiftPressed);
			this.ClearVerticalMoveCache ();
			
			if (this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, Text.TextNavigator.Direction.Forward))
			{
				if (message.IsControlPressed == false)
				{
					return true;
				}
			}
			
			if (message.IsControlPressed)
			{
				this.text_navigator.MoveTo (Text.TextNavigator.Target.WordEnd, 1);
			}
			else
			{
				this.text_navigator.MoveTo (Text.TextNavigator.Target.CharacterNext, 1);
			}
			
			return true;
		}
		
		private bool ProcessUpArrowKey(Message message)
		{
			this.PreProcessCursorMove (message.IsShiftPressed);
			this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, Text.TextNavigator.Direction.Backward);
			
			if (message.IsControlPressed)
			{
				this.ClearVerticalMoveCache ();
				this.text_navigator.MoveTo (Text.TextNavigator.Target.ParagraphStart, 1);
			}
			else
			{
				this.text_navigator.VerticalMove (this.GetVerticalMoveCache (), -1);
			}
			
			return true;
		}
		
		private bool ProcessDownArrowKey(Message message)
		{
			this.PreProcessCursorMove (message.IsShiftPressed);
			this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, Text.TextNavigator.Direction.Forward);
			
			if (message.IsControlPressed)
			{
				this.ClearVerticalMoveCache ();
				this.text_navigator.MoveTo (Text.TextNavigator.Target.ParagraphEnd, 1);
			}
			else
			{
				this.text_navigator.VerticalMove (this.GetVerticalMoveCache (), 1);
			}
			
			return true;
		}
		
		
		private bool ProcessKeyPress(Message message)
		{
			Text.Unicode.Code code = (Text.Unicode.Code) message.KeyChar;
			
			if (code >= Text.Unicode.Code.Space)
			{
				this.Insert (code);
				return true;
			}
			
			return false;
		}
		
		
		private bool ProcessMouseDown(Message message, Drawing.Point pos, Text.ITextFrame frame)
		{
			this.ClearVerticalMoveCache ();
			
			this.is_mouse_down = true;
			
			if (message.ButtonDownCount == 1)
			{
				this.is_mouse_dragging = true;
			}
			
			this.EndSelection ();

			if (this.text_navigator.HasSelection)
			{
				this.text_navigator.ClearSelection ();
			}
			
			int p, d;
			
			double cx = pos.X;
			double cy = pos.Y;
			
			if (this.text_navigator.HitTest (frame, cx, cy, true, out p, out d))
			{
				this.initial_position = p;
				
				this.text_navigator.HitTest (frame, cx, cy, false, out p, out d);
				this.text_navigator.MoveTo (p, d);
				
				return true;
			}
			
			this.initial_position = -1;
			return false;
		}
		
		private bool ProcessMouseDrag(Drawing.Point pos, Text.ITextFrame frame)
		{
			int p, d;
			
			double cx = pos.X;
			double cy = pos.Y;
			
			if (this.text_navigator.HitTest (frame, cx, cy, true, out p, out d))
			{
				if ((this.is_mouse_selecting == false) &&
					(this.initial_position != p))
				{
					this.EndSelection ();
					this.text_navigator.ClearSelection ();
					
					this.is_mouse_selecting = true;
				}
				
				if (this.is_mouse_selecting)
				{
					if (this.text_navigator.IsSelectionActive)
					{
						//	Rien à faire, sélection déjà active.
					}
					else if (this.text_navigator.HasSelection)
					{
						this.text_navigator.ContinueSelection ();
					}
					else
					{
						this.text_navigator.StartSelection ();
					}
					
					this.text_navigator.MoveTo (p, d);
				}
				
				return true;
			}
			
			return true;
		}
		
		private bool ProcessMouseMove(Drawing.Point pos, Text.ITextFrame frame)
		{
			//	Rien à faire de spécial, la souris se déplace sans aucun bouton
			//	pressé.
			
			return true;
		}
		
		private bool ProcessMouseUp(Message message, Drawing.Point pos, Text.ITextFrame frame)
		{
			if (this.text_navigator.IsSelectionActive)
			{
//-				this.text_navigator.EndSelection ();
//-				this.is_mouse_selecting = false;
			}
			
			if (message.ButtonDownCount == 1)
			{
				//	TODO: valide position/fin de sélection
			}
			else if (message.ButtonDownCount == 2)
			{
				this.SelectWord ();
			}
			else if (message.ButtonDownCount == 3)
			{
				this.SelectLine ();
			}
			else if (message.ButtonDownCount > 3)
			{
				this.SelectAll ();
			}
			
//-			System.Diagnostics.Debug.Assert (this.is_mouse_selecting == false);
			
			this.is_mouse_down      = false;
			this.is_mouse_dragging  = false;
			this.is_mouse_selecting = false;
			
			return true;
		}
		#endregion
		
		public void MoveTo(Text.TextNavigator.Target target, int count, int direction, bool shift)
		{
			Text.TextNavigator.Direction dir = (Text.TextNavigator.Direction) System.Math.Sign (direction);
			
			this.PreProcessCursorMove (shift);
			this.ClearVerticalMoveCache ();
			this.ChangeSelectionModeBeforeMove (shift, dir);
			
			this.text_navigator.MoveTo (target, count);
		}
		
		
		public void SelectInsertedCharacter()
		{
			this.EndSelection ();
			this.text_navigator.ClearSelection ();
			
			this.text_navigator.MoveTo (Text.TextNavigator.Target.CharacterPrevious, 1);
			this.text_navigator.StartSelection ();
			this.text_navigator.MoveTo (Text.TextNavigator.Target.CharacterNext, 1);
			this.text_navigator.EndSelection ();
		}
		
		public void SelectWord()
		{
			this.EndSelection ();
			this.text_navigator.ClearSelection ();
			
			this.text_navigator.MoveTo (Text.TextNavigator.Target.WordStart, 0);
			this.text_navigator.StartSelection ();
			this.text_navigator.MoveTo (Text.TextNavigator.Target.WordEnd, 1);
		}
		
		public void SelectLine()
		{
			this.EndSelection ();
			this.text_navigator.ClearSelection ();
			
			this.text_navigator.MoveTo (Text.TextNavigator.Target.LineStart, 0);
			this.text_navigator.StartSelection ();
			this.text_navigator.MoveTo (Text.TextNavigator.Target.LineEnd, 1);
		}
		
		public void SelectAll()
		{
			this.EndSelection ();
			this.text_navigator.ClearSelection ();
			
			this.text_navigator.MoveTo (Text.TextNavigator.Target.TextStart, 0);
			this.text_navigator.StartSelection ();
			this.text_navigator.MoveTo (Text.TextNavigator.Target.TextEnd, 0);
		}
		
		
		public void ClearSelection()
		{
			this.EndSelection ();
			this.text_navigator.ClearSelection ();
		}
		
		
		public bool Insert(Text.Unicode.Code code, Text.Properties.OpenTypeProperty ot_property)
		{
			if (code > Text.Unicode.Code.Invalid)
			{
				//	TODO: gérer la génération d'un surrogate pair
				
				return false;
			}
			else
			{
				this.DeleteSelection ();
				this.text_navigator.Insert (code, ot_property);
			
				return true;
			}
		}
		
		public bool Insert(Text.Unicode.Code code)
		{
			if (code > Text.Unicode.Code.Invalid)
			{
				//	TODO: gérer la génération d'un surrogate pair
				
				return false;
			}
			else
			{
				this.DeleteSelection ();
				this.text_navigator.Insert (code);
				
				return true;
			}
		}
		
		public bool Insert(Text.Properties.TabProperty tab_property)
		{
			this.DeleteSelection ();
			this.text_navigator.Insert (Text.Unicode.Code.HorizontalTab, tab_property);
			
			return true;
		}
		
		public bool Insert(Text.Properties.BreakProperty break_property)
		{
			this.DeleteSelection ();
			this.text_navigator.Insert (Text.Unicode.Code.PageSeparator, break_property);
			
			return true;
		}
		
#if false	// MW: la logique d'insertion de tabuleurs est gérée directement dans TextNavigator
		public bool Insert(string text)
		{
			this.DeleteSelection ();
			
			string[] fragments = text.Split ('\t');
			
			this.text_navigator.Insert (fragments[0]);
			
			for (int i = 1; i < fragments.Length; i++)
			{
				string tag = this.text_navigator.FindInsertionTabTag ();
				
				if (tag == null)
				{
					this.text_navigator.Insert (" ");
				}
				else
				{
					this.text_navigator.Insert (Text.Unicode.Code.HorizontalTab, new Text.Properties.TabProperty (tag));
				}
				
				if (fragments[i].Length > 0)
				{
					this.text_navigator.Insert (fragments[i]);
				}
			}
			
			return true;
		}
#else
		public bool Insert(string text)
		{
			this.DeleteSelection ();
			this.text_navigator.InsertWithTabs (text);
			return true;
		}
#endif
		
		
		public void DeleteSelection()
		{
			this.EndSelection ();

			if (this.text_navigator.HasSelection)
			{
				this.text_navigator.Delete ();
			}
		}
		
		
		public void EndSelection()
		{
			if (this.text_navigator.IsSelectionActive)
			{
				this.text_navigator.EndSelection ();
			}
		}
		
		
		public bool Undo()
		{
			this.EndSelection ();
			
			if (this.text_navigator.OpletQueue.CanUndo)
			{
				this.text_navigator.Undo ();
				
				return true;
			}
			
			return false;
		}
		
		public bool Redo()
		{
			this.EndSelection ();
			
			if (this.text_navigator.OpletQueue.CanRedo)
			{
				this.text_navigator.Redo ();
				
				return true;
			}
			
			return false;
		}
		
		
		public void SetParagraphStyles(params Text.TextStyle[] styles)
		{
			this.EndSelection ();
			
			this.text_navigator.SetParagraphStyles (styles);
		}
		
		public void SetTextStyles(params Text.TextStyle[] styles)
		{
			this.EndSelection ();
			
			this.text_navigator.SetTextStyles (styles);
		}
		
		public void SetSymbolStyles(params Text.TextStyle[] styles)
		{
			this.EndSelection ();
			
			this.text_navigator.SetSymbolStyles (styles);
		}
		
		public void SetMetaProperties(Text.Properties.ApplyMode mode, params Text.TextStyle[] meta_properties)
		{
			this.EndSelection ();
			
			this.text_navigator.SetMetaProperties (mode, meta_properties);
		}
		
		public void SetParagraphProperties(Text.Properties.ApplyMode mode, params Text.Property[] properties)
		{
			this.EndSelection ();
			
			this.text_navigator.SetParagraphProperties (mode, properties);
		}
		
		public void SetTextProperties(Text.Properties.ApplyMode mode, params Text.Property[] properties)
		{
			this.EndSelection ();
			
			this.text_navigator.SetTextProperties (mode, properties);
		}
		
		
		public void RemoveTab(string tag)
		{
			this.EndSelection ();
			this.text_navigator.RemoveTab (tag);
		}
		
		
		public void NotifyTextChanged()
		{
			this.OnTextChanged ();
		}
		
		public void NotifyTabsChanged()
		{
			this.OnTabsChanged ();
		}
		
		public void NotifyCursorMoved()
		{
			this.OnCursorMoved ();
		}
		
		public void NotifyActiveStyleChanged()
		{
			this.OnActiveStyleChanged ();
		}
		
		
		private void PreProcessCursorMove (bool is_shift_pressed)
		{
			if (! is_shift_pressed)
			{
				this.text_navigator.ClearCurrentStylesAndProperties ();
			}
		}
		
		private void ClearVerticalMoveCache()
		{
			this.initial_x = double.NaN;
		}
		
		private double GetVerticalMoveCache()
		{
			//	Si on démarre un déplacement vertical avec les touches haut/bas,
			//	on désire se souvenir de la position [x] initale, de manière à
			//	pouvoir sauter des lignes plus courtes en maintenant un déplacement
			//	avec [x] constant.
			
			if (double.IsNaN (this.initial_x))
			{
				Text.ITextFrame frame;
				double cx, cy, ascender, descender, angle;
					
				this.text_navigator.GetCursorGeometry (out frame, out cx, out cy, out ascender, out descender, out angle);
				this.initial_x = cx;
			}
			
			return this.initial_x;
		}
		
		private bool ChangeSelectionModeBeforeMove(bool selection, Text.TextNavigator.Direction direction)
		{
			//	Commence ou termine une sélection (appelé par ex. lors d'un
			//	déplacement avec ou sans SHIFT pressé).
			
			//	En cas de désélection, positionne le curseur soit au début,
			//	soit à la fin de la zone sélectionnée, en fonction de la
			//	direction préférentielle.
			
			//	Retourne 'true' si une désélection a eu lieu.
			
			if (selection)
			{
				if (this.text_navigator.IsSelectionActive == false)
				{
					if (this.text_navigator.HasSelection)
					{
						this.text_navigator.ContinueSelection ();
					}
					else
					{
						this.text_navigator.StartSelection ();
					}
				}
			}
			else
			{
				this.EndSelection ();
				
				//	Si une sélection est active, il faut la désactiver en tenant
				//	compte de la direction de déplacement souhaitée :
				
				if (this.text_navigator.HasSelection)
				{
					this.text_navigator.ClearSelection (direction);
					return true;
				}
			}
			
			return false;
		}
		
		
		private void HandleTextChanged(object sender)
		{
			this.NotifyTextChanged ();
		}
		
		private void HandleTabsChanged(object sender)
		{
			this.NotifyTabsChanged ();
		}
		
		private void HandleCursorMoved(object sender)
		{
			this.NotifyCursorMoved ();
		}
		
		private void HandleActiveStyleChanged(object sender)
		{
			this.NotifyActiveStyleChanged ();
		}
		
		
		protected virtual void OnTextChanged()
		{
			this.ClearVerticalMoveCache ();
			
			if (this.TextChanged != null)
			{
				this.TextChanged (this);
			}
		}
		
		protected virtual void OnTabsChanged()
		{
			this.ClearVerticalMoveCache ();
			
			if (this.TabsChanged != null)
			{
				this.TabsChanged (this);
			}
		}
		
		protected virtual void OnCursorMoved()
		{
			if (this.CursorMoved != null)
			{
				this.CursorMoved (this);
			}
		}
		
		protected virtual void OnActiveStyleChanged()
		{
			if (this.ActiveStyleChanged != null)
			{
				this.ActiveStyleChanged (this);
			}
		}
		
		
		public event Support.EventHandler		TextChanged;
		public event Support.EventHandler		TabsChanged;
		public event Support.EventHandler		CursorMoved;
		public event Support.EventHandler		ActiveStyleChanged;
		
		private Text.TextNavigator				text_navigator;
		
		private int								initial_position;
		private double							initial_x = double.NaN;
		
		private bool							disable_tab_key;
		private bool							disable_return_key;
		private bool							disable_escape_key;
		
		private bool							allow_tabs;
		private bool							allow_lines;
		private bool							allow_frames;
		
		private bool							is_read_only;
		private bool							is_mouse_down;
		private bool							is_mouse_dragging;
		private bool							is_mouse_selecting;
	}
}
