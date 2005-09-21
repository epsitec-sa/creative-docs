//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			//	Provisoire...
			
			get
			{
				return this.text_navigator;
			}
			set
			{
				this.text_navigator = value;
			}
		}
		
		public Text.ITextFrame					TextFrame
		{
			get
			{
				return this.text_frame;
			}
			set
			{
				this.text_frame = value;
			}
		}
		
		
		public bool ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.Type)
			{
				case MessageType.KeyDown:
					return this.ProcessKeyDown (message);
				
				case MessageType.KeyPress:
					return this.ProcessKeyPress (message);
				
				case MessageType.MouseDown:
					return this.ProcessMouseDown (message, pos);
				
				case MessageType.MouseMove:
					return ( this.is_mouse_dragging && this.ProcessMouseDrag (pos))
						|| (!this.is_mouse_dragging && this.ProcessMouseMove (pos));
				
				case MessageType.MouseUp:
					return this.is_mouse_down && this.ProcessMouseUp (message, pos);
			}
			
			return false;
		}
		
		
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
				if ((message.IsCtrlPressed == true) &&
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
					if ((message.IsCtrlPressed) &&
						(this.allow_frames))
					{
						if (message.IsShiftPressed)
						{
							//	TODO: insérer un saut de frame
						}
						else
						{
							return this.Insert (Text.Unicode.Code.PageSeparator);
						}
					}
					else
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
			}
			
			return false;
		}
		
		private bool ProcessTabKey(Message message)
		{
			if (this.disable_tab_key)
			{
				if ((message.IsCtrlPressed == true) &&
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
			if (this.text_navigator.IsSelectionActive)
			{
				this.text_navigator.EndSelection ();
			}
			
			if (this.text_navigator.HasSelection)
			{
				this.text_navigator.Delete ();
				this.NotifyTextChanged ();
				return true;
			}
			
			this.text_navigator.Delete (-1);
			this.NotifyTextChanged ();
			return true;
		}
		
		private bool ProcessDeleteKey(Message message)
		{
			if (this.text_navigator.IsSelectionActive)
			{
				this.text_navigator.EndSelection ();
			}
			
			if (this.text_navigator.HasSelection)
			{
				this.text_navigator.Delete ();
				this.NotifyTextChanged ();
				return true;
			}
			
			this.text_navigator.Delete (1);
			this.NotifyTextChanged ();
			return true;
		}
		
		private bool ProcessHomeKey(Message message)
		{
			this.ClearVerticalMoveCache ();
			this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, -1);
			
			if (message.IsCtrlPressed)
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
			this.ClearVerticalMoveCache ();
			this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, 1);
			
			if (message.IsCtrlPressed)
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
			this.ClearVerticalMoveCache ();
			
			if (this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, -1))
			{
				if (message.IsCtrlPressed == false)
				{
					return true;
				}
			}
			
			if (message.IsCtrlPressed)
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
			this.ClearVerticalMoveCache ();
			
			if (this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, 1))
			{
				if (message.IsCtrlPressed == false)
				{
					return true;
				}
			}
			
			if (message.IsCtrlPressed)
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
			this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, -1);
			
			if (message.IsCtrlPressed)
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
			this.ChangeSelectionModeBeforeMove (message.IsShiftPressed, 1);
			
			if (message.IsCtrlPressed)
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
		
		private bool ChangeSelectionModeBeforeMove(bool selection, int direction)
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
					this.text_navigator.StartSelection ();
				}
			}
			else
			{
				if (this.text_navigator.IsSelectionActive)
				{
					this.text_navigator.EndSelection ();
				}
				
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
		
		
		private bool ProcessMouseDown(Message message, Drawing.Point pos)
		{
			this.ClearVerticalMoveCache ();
			
			this.is_mouse_down = true;
			
			if (message.ButtonDownCount == 1)
			{
				this.is_mouse_dragging = true;
			}
			
			if (this.text_navigator.IsSelectionActive)
			{
				this.text_navigator.EndSelection ();
			}
			if (this.text_navigator.HasSelection)
			{
				this.text_navigator.ClearSelection ();
			}
			
			int p, d;
			
			double cx = pos.X;
			double cy = pos.Y;
			
			if (this.text_navigator.HitTest (this.text_frame, cx, cy, true, out p, out d))
			{
				this.initial_position = p;
				
				this.text_navigator.HitTest (this.text_frame, cx, cy, false, out p, out d);
				this.text_navigator.MoveTo (p, d);
				
				return true;
			}
			
			this.initial_position = -1;
			return false;
		}
		
		private bool ProcessMouseDrag(Drawing.Point pos)
		{
			int p, d;
			
			double cx = pos.X;
			double cy = pos.Y;
			
			if (this.text_navigator.HitTest (this.text_frame, cx, cy, true, out p, out d))
			{
				if ((this.is_mouse_selecting == false) &&
					(this.initial_position != p))
				{
					this.is_mouse_selecting = true;
					this.text_navigator.StartSelection ();
				}
				
				if (this.is_mouse_selecting)
				{
					this.text_navigator.MoveTo (p, d);
				}
				
				return true;
			}
			
			return true;
		}
		
		private bool ProcessMouseMove(Drawing.Point pos)
		{
			//	Rien à faire de spécial, la souris se déplace sans aucun bouton
			//	pressé.
			
			return true;
		}
		
		private bool ProcessMouseUp(Message message, Drawing.Point pos)
		{
			if (this.text_navigator.IsSelectionActive)
			{
				this.text_navigator.EndSelection ();
				this.is_mouse_selecting = false;
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
			
			System.Diagnostics.Debug.Assert (this.is_mouse_selecting == false);
			
			this.is_mouse_down      = false;
			this.is_mouse_dragging  = false;
			this.is_mouse_selecting = false;
			
			return true;
		}
		
		
		public void SelectWord()
		{
			this.text_navigator.MoveTo (Text.TextNavigator.Target.WordStart, 0);
			this.text_navigator.StartSelection ();
			this.text_navigator.MoveTo (Text.TextNavigator.Target.WordEnd, 1);
			this.text_navigator.EndSelection ();
		}
		
		public void SelectLine()
		{
			this.text_navigator.MoveTo (Text.TextNavigator.Target.LineStart, 0);
			this.text_navigator.StartSelection ();
			this.text_navigator.MoveTo (Text.TextNavigator.Target.LineEnd, 1);
			this.text_navigator.EndSelection ();
		}
		
		public void SelectAll()
		{
			this.text_navigator.MoveTo (Text.TextNavigator.Target.TextStart, 0);
			this.text_navigator.StartSelection ();
			this.text_navigator.MoveTo (Text.TextNavigator.Target.TextEnd, 0);
			this.text_navigator.EndSelection ();
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
				return this.Insert (new string ((char) code, 1));
			}
		}
		
		public bool Insert(string text)
		{
			if (this.text_navigator.IsSelectionActive)
			{
				this.text_navigator.EndSelection ();
			}
			if (this.text_navigator.HasSelection)
			{
				this.text_navigator.Delete ();
			}
			
			this.text_navigator.Insert (text);
			this.NotifyTextChanged ();
			
			return true;
		}
		
		
		
		public void NotifyTextChanged()
		{
			this.ClearVerticalMoveCache ();
			this.OnTextChanged ();
		}
		
		
		protected virtual void OnTextChanged()
		{
			if (this.TextChanged != null)
			{
				this.TextChanged (this);
			}
		}
		
		
		public event Support.EventHandler		TextChanged;
		
		private Text.TextNavigator				text_navigator;
		private Text.ITextFrame					text_frame;
		
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
