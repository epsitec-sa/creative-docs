//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.disableTabKey    = false;
			this.disableReturnKey = false;
			this.disableEscapeKey = false;
			
			this.allowTabs   = true;
			this.allowLines  = true;
			this.allowFrames = true;
			
			this.isReadOnly = false;
		}
		
		
		public Text.TextNavigator				TextNavigator
		{
			get
			{
				return this.textNavigator;
			}
			set
			{
				if (this.textNavigator != value)
				{
					if (this.textNavigator != null)
					{
						this.textNavigator.ActiveStyleChanged -= this.HandleActiveStyleChanged;
						this.textNavigator.TabsChanged -= this.HandleTabsChanged;
						this.textNavigator.TextChanged -= this.HandleTextChanged;
						this.textNavigator.CursorMoved -= this.HandleCursorMoved;
					}
					
					this.textNavigator = value;
					
					if (this.textNavigator != null)
					{
						this.textNavigator.CursorMoved += this.HandleCursorMoved;
						this.textNavigator.TextChanged += this.HandleTextChanged;
						this.textNavigator.TabsChanged += this.HandleTabsChanged;
						this.textNavigator.ActiveStyleChanged += this.HandleActiveStyleChanged;
					}
				}
			}
		}
		
		
		public bool ProcessMessage(Message message, Drawing.Point pos, Text.ITextFrame frame)
		{
			switch (message.MessageType)
			{
				case MessageType.KeyDown:
					return this.ProcessKeyDown (message);
				
				case MessageType.KeyPress:
					return this.ProcessKeyPress (message);
				
				case MessageType.MouseDown:
					return this.ProcessMouseDown (message, pos, frame);
				
				case MessageType.MouseMove:
					return ( this.isMouseDragging && this.ProcessMouseDrag (pos, frame))
						|| (!this.isMouseDragging && this.ProcessMouseMove (pos, frame));
				
				case MessageType.MouseUp:
					return this.isMouseDown && this.ProcessMouseUp (message, pos, frame);
			}
			
			return false;
		}
		
		
		#region Specialized Process Methods
		private bool ProcessKeyDown(Message message)
		{
			//	L'événement KeyDown doit être traité pour toutes les touches non
			//	alphabétiques (les curseurs, TAB, RETURN, etc.)
			
			bool processed = false;
			
			if (this.isReadOnly == false)
			{
				switch (message.KeyCodeOnly)
				{
					case KeyCode.NumericEnter:
					case KeyCode.Return:
						processed = this.ProcessReturnKey (message);
						break;
					
					case KeyCode.Tab:
						processed = this.ProcessTabKey (message);
						break;
					
					case KeyCode.Back:
						processed = this.ProcessBackKey (message);
						break;
					
					case KeyCode.Delete:
						processed = this.ProcessDeleteKey (message);
						break;
					
					case KeyCode.Space:
						processed = this.ProcessSpaceKey (message);
						break;
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
			if (this.disableReturnKey)
			{
				if ((message.IsControlPressed == true) &&
					(message.IsAltPressed == false) &&
					(this.allowLines))
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
					(this.allowLines))
				{
					if ((message.IsControlPressed) &&
						(this.allowFrames))
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
			if (this.disableTabKey)
			{
				if ((message.IsControlPressed == true) &&
					(message.IsAltPressed == false) &&
					(this.allowTabs))
				{
					return this.Insert (Text.Unicode.Code.HorizontalTab);
				}
			}
			else
			{
				if ((message.IsAltPressed == false) &&
					(this.allowTabs))
				{
					return this.Insert (Text.Unicode.Code.HorizontalTab);
				}
			}
			
			return false;
		}
		
		private bool ProcessEscapeKey(Message message)
		{
			if (this.disableEscapeKey == false)
			{
				this.EndSelection ();
			
				if (this.textNavigator.HasSelection)
				{
					this.textNavigator.ClearSelection ();
					return true;
				}
			}
			
			return false;
		}
		
		private bool ProcessBackKey(Message message)
		{
			this.EndSelection ();
			
			if (this.textNavigator.HasSelection)
			{
				this.textNavigator.Delete ();
				return true;
			}
			
			this.textNavigator.Delete (Text.TextNavigator.Direction.Backward);
			return true;
		}
		
		private bool ProcessDeleteKey(Message message)
		{
			this.EndSelection ();
			
			if (this.textNavigator.HasSelection)
			{
				this.textNavigator.Delete ();
				return true;
			}
			
			this.textNavigator.Delete (Text.TextNavigator.Direction.Forward);
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
				this.textNavigator.MoveTo (Text.TextNavigator.Target.TextStart, 0);
			}
			else
			{
				this.textNavigator.MoveTo (Text.TextNavigator.Target.LineStart, 0);
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
				this.textNavigator.MoveTo (Text.TextNavigator.Target.TextEnd, 0);
			}
			else
			{
				this.textNavigator.MoveTo (Text.TextNavigator.Target.LineEnd, 0);
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
				this.textNavigator.MoveTo (Text.TextNavigator.Target.WordStart, 1);
			}
			else
			{
				this.textNavigator.MoveTo (Text.TextNavigator.Target.CharacterPrevious, 1);
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
				this.textNavigator.MoveTo (Text.TextNavigator.Target.WordEnd, 1);
			}
			else
			{
				this.textNavigator.MoveTo (Text.TextNavigator.Target.CharacterNext, 1);
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
				this.textNavigator.MoveTo (Text.TextNavigator.Target.ParagraphStart, 1);
			}
			else
			{
				this.textNavigator.VerticalMove (this.GetVerticalMoveCache (), -1);
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
				this.textNavigator.MoveTo (Text.TextNavigator.Target.ParagraphEnd, 1);
			}
			else
			{
				this.textNavigator.VerticalMove (this.GetVerticalMoveCache (), 1);
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
			else if ((int)code == 31)	//	CTRL + dash
			{
				this.Insert (Text.Unicode.Code.SoftHyphen);
			}
			
			return false;
		}
		
		
		private bool ProcessMouseDown(Message message, Drawing.Point pos, Text.ITextFrame frame)
		{
			this.ClearVerticalMoveCache ();
			
			this.isMouseDown = true;
			
			if (message.ButtonDownCount == 1)
			{
				this.isMouseDragging = true;
			}
			
			this.EndSelection ();

			if (this.textNavigator.HasSelection)
			{
				this.textNavigator.ClearSelection ();
			}
			
			int p, d;
			
			double cx = pos.X;
			double cy = pos.Y;
			
			if (this.textNavigator.HitTest (frame, cx, cy, true, out p, out d))
			{
				this.initialPosition = p;
				
				this.textNavigator.HitTest (frame, cx, cy, false, out p, out d);
				this.textNavigator.MoveTo (p, d);
				
				return true;
			}
			
			this.initialPosition = -1;
			return false;
		}
		
		private bool ProcessMouseDrag(Drawing.Point pos, Text.ITextFrame frame)
		{
			int p, d;
			
			double cx = pos.X;
			double cy = pos.Y;
			
			if (this.textNavigator.HitTest (frame, cx, cy, true, out p, out d))
			{
				if ((this.isMouseSelecting == false) &&
					(this.initialPosition != p))
				{
					this.EndSelection ();
					this.textNavigator.ClearSelection ();
					
					this.isMouseSelecting = true;
				}
				
				if (this.isMouseSelecting)
				{
					if (this.textNavigator.IsSelectionActive)
					{
						//	Rien à faire, sélection déjà active.
					}
					else if (this.textNavigator.HasSelection)
					{
						this.textNavigator.ContinueSelection ();
					}
					else
					{
						this.textNavigator.StartSelection ();
					}
					
					this.textNavigator.MoveTo (p, d);
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
			if (this.textNavigator.IsSelectionActive)
			{
//-				this.textNavigator.EndSelection ();
//-				this.isMouseSelecting = false;
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
			
//-			System.Diagnostics.Debug.Assert (this.isMouseSelecting == false);
			
			this.isMouseDown      = false;
			this.isMouseDragging  = false;
			this.isMouseSelecting = false;
			
			return true;
		}
		#endregion
		
		public void MoveTo(Text.TextNavigator.Target target, int count, int direction, bool shift)
		{
			Text.TextNavigator.Direction dir = (Text.TextNavigator.Direction) System.Math.Sign (direction);
			
			this.PreProcessCursorMove (shift);
			this.ClearVerticalMoveCache ();
			this.ChangeSelectionModeBeforeMove (shift, dir);
			
			this.textNavigator.MoveTo (target, count);
		}
		
		
		public void SelectInsertedCharacter()
		{
			this.EndSelection ();
			this.textNavigator.ClearSelection ();
			
			this.textNavigator.MoveTo (Text.TextNavigator.Target.CharacterPrevious, 1);
			this.textNavigator.StartSelection ();
			this.textNavigator.MoveTo (Text.TextNavigator.Target.CharacterNext, 1);
			this.textNavigator.EndSelection ();
		}
		
		public void SelectWord()
		{
			this.EndSelection ();
			this.textNavigator.ClearSelection ();
			
			this.textNavigator.MoveTo (Text.TextNavigator.Target.WordStart, 0);
			this.textNavigator.StartSelection ();
			this.textNavigator.MoveTo (Text.TextNavigator.Target.WordEnd, 1);
		}
		
		public void SelectLine()
		{
			this.EndSelection ();
			this.textNavigator.ClearSelection ();
			
			this.textNavigator.MoveTo (Text.TextNavigator.Target.LineStart, 0);
			this.textNavigator.StartSelection ();
			this.textNavigator.MoveTo (Text.TextNavigator.Target.LineEnd, 1);
		}
		
		public void SelectAll()
		{
			this.EndSelection ();
			this.textNavigator.ClearSelection ();
			
			this.textNavigator.MoveTo (Text.TextNavigator.Target.TextStart, 0);
			this.textNavigator.StartSelection ();
			this.textNavigator.MoveTo (Text.TextNavigator.Target.TextEnd, 0);
		}
		
		
		public void ClearSelection()
		{
			this.EndSelection ();
			this.textNavigator.ClearSelection ();
		}
		
		
		public bool Insert(Text.Unicode.Code code, Text.Properties.OpenTypeProperty otProperty)
		{
			if (code > Text.Unicode.Code.Invalid)
			{
				//	TODO: gérer la génération d'un surrogate pair
				
				return false;
			}
			else
			{
				this.DeleteSelection ();
				this.textNavigator.Insert (code, otProperty);
			
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

				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
				watch.Start ();
				this.textNavigator.Insert (code);
				watch.Stop ();
				System.Diagnostics.Trace.WriteLine (string.Format ("{0} ms", watch.ElapsedMilliseconds));
				
				return true;
			}
		}
		
		public bool Insert(Text.Properties.TabProperty tabProperty)
		{
			this.DeleteSelection ();
			this.textNavigator.Insert (Text.Unicode.Code.HorizontalTab, tabProperty);
			
			return true;
		}
		
		public bool Insert(Text.Properties.BreakProperty breakProperty)
		{
			this.DeleteSelection ();
			this.textNavigator.Insert (Text.Unicode.Code.PageSeparator, breakProperty);
			
			return true;
		}
		
#if false	// MW: la logique d'insertion de tabuleurs est gérée directement dans TextNavigator
		public bool Insert(string text)
		{
			this.DeleteSelection ();
			
			string[] fragments = text.Split ('\t');
			
			this.textNavigator.Insert (fragments[0]);
			
			for (int i = 1; i < fragments.Length; i++)
			{
				string tag = this.textNavigator.FindInsertionTabTag ();
				
				if (tag == null)
				{
					this.textNavigator.Insert (" ");
				}
				else
				{
					this.textNavigator.Insert (Text.Unicode.Code.HorizontalTab, new Text.Properties.TabProperty (tag));
				}
				
				if (fragments[i].Length > 0)
				{
					this.textNavigator.Insert (fragments[i]);
				}
			}
			
			return true;
		}
#else
		public bool Insert(string text)
		{
			this.DeleteSelection ();
			this.textNavigator.InsertWithTabs (text);
			return true;
		}
#endif
		
		
		public void DeleteSelection()
		{
			this.EndSelection ();

			if (this.textNavigator.HasSelection)
			{
				this.textNavigator.Delete ();
			}
		}
		
		
		public void EndSelection()
		{
			if (this.textNavigator.IsSelectionActive)
			{
				this.textNavigator.EndSelection ();
			}
		}
		
		
		public bool Undo()
		{
			this.EndSelection ();
			
			if (this.textNavigator.OpletQueue.CanUndo)
			{
				this.textNavigator.Undo ();
				
				return true;
			}
			
			return false;
		}
		
		public bool Redo()
		{
			this.EndSelection ();
			
			if (this.textNavigator.OpletQueue.CanRedo)
			{
				this.textNavigator.Redo ();
				
				return true;
			}
			
			return false;
		}
		
		
		public void SetParagraphStyles(params Text.TextStyle[] styles)
		{
			this.EndSelection ();
			
			this.textNavigator.SetParagraphStyles (styles);
		}
		
		public void SetTextStyles(params Text.TextStyle[] styles)
		{
			this.EndSelection ();
			
			this.textNavigator.SetTextStyles (styles);
		}
		
		public void SetSymbolStyles(params Text.TextStyle[] styles)
		{
			this.EndSelection ();
			
			this.textNavigator.SetSymbolStyles (styles);
		}
		
		public void SetMetaProperties(Text.Properties.ApplyMode mode, params Text.TextStyle[] metaProperties)
		{
			this.EndSelection ();
			
			this.textNavigator.SetMetaProperties (mode, metaProperties);
		}
		
		public void SetParagraphProperties(Text.Properties.ApplyMode mode, params Text.Property[] properties)
		{
			this.EndSelection ();
			
			this.textNavigator.SetParagraphProperties (mode, properties);
		}
		
		public void SetTextProperties(Text.Properties.ApplyMode mode, params Text.Property[] properties)
		{
			this.EndSelection ();
			
			this.textNavigator.SetTextProperties (mode, properties);
		}
		
		
		public void RemoveTab(string tag)
		{
			this.EndSelection ();
			this.textNavigator.RemoveTab (tag);
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
		
		
		private void PreProcessCursorMove (bool isShiftPressed)
		{
			if (! isShiftPressed)
			{
				this.textNavigator.ClearCurrentStylesAndProperties ();
			}
		}
		
		private void ClearVerticalMoveCache()
		{
			this.initialX = double.NaN;
		}
		
		private double GetVerticalMoveCache()
		{
			//	Si on démarre un déplacement vertical avec les touches haut/bas,
			//	on désire se souvenir de la position [x] initale, de manière à
			//	pouvoir sauter des lignes plus courtes en maintenant un déplacement
			//	avec [x] constant.
			
			if (double.IsNaN (this.initialX))
			{
				Text.ITextFrame frame;
				double cx, cy, ascender, descender, angle;
					
				this.textNavigator.GetCursorGeometry (out frame, out cx, out cy, out ascender, out descender, out angle);
				this.initialX = cx;
			}
			
			return this.initialX;
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
				if (this.textNavigator.IsSelectionActive == false)
				{
					if (this.textNavigator.HasSelection)
					{
						this.textNavigator.ContinueSelection ();
					}
					else
					{
						this.textNavigator.StartSelection ();
					}
				}
			}
			else
			{
				this.EndSelection ();
				
				//	Si une sélection est active, il faut la désactiver en tenant
				//	compte de la direction de déplacement souhaitée :
				
				if (this.textNavigator.HasSelection)
				{
					this.textNavigator.ClearSelection (direction);
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
		
		private Text.TextNavigator				textNavigator;
		
		private int								initialPosition;
		private double							initialX = double.NaN;
		
		private bool							disableTabKey;
		private bool							disableReturnKey;
		private bool							disableEscapeKey;
		
		private bool							allowTabs;
		private bool							allowLines;
		private bool							allowFrames;
		
		private bool							isReadOnly;
		private bool							isMouseDown;
		private bool							isMouseDragging;
		private bool							isMouseSelecting;
	}
}
