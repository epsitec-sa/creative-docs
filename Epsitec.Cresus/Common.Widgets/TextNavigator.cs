using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextNavigator permet de naviguer dans un TextLayout,
	/// d'insérer/supprimer des caractères, etc.
	/// </summary>
	public class TextNavigator
	{
		public TextNavigator(TextLayout textLayout)
		{
			this.textLayout = textLayout;
			this.context = new TextLayout.Context();
		}

		
		public TextLayout.Context				Context
		{
			get { return this.context; }
		}

		public bool								IsReadOnly
		{
			get { return this.isReadOnly; }
			set { this.isReadOnly = value; }
		}

		public bool								IsNumeric
		{
			get { return this.isNumeric; }
			set { this.isNumeric = value; }
		}

		public bool								IsMultiLine
		{
			get
			{
				if ( (this.textLayout.BreakMode & Drawing.TextBreakMode.SingleLine) == 0 )
				{
					return true;
				}
				
				return false;
			}
		}
		
		public int								MaxChar
		{
			get { return this.context.MaxChar; }
			set { this.context.MaxChar = value; }
		}

		public string							Selection
		{
			get
			{
				int cursorFrom = this.textLayout.FindOffsetFromIndex(this.context.CursorFrom);
				int cursorTo   = this.textLayout.FindOffsetFromIndex(this.context.CursorTo);
				
				int from = System.Math.Min(cursorFrom, cursorTo);
				int to   = System.Math.Max(cursorFrom, cursorTo);
				
				string text = this.textLayout.Text;
				return text.Substring(from, to-from);
			}

			set
			{
				this.textLayout.ReplaceSelection(this.context, value);
				this.OnTextInserted(true);
				this.OnCursorScrolled();
				this.OnCursorChanged(true);
			}
		}

		public int								Cursor
		{
			get
			{
				return this.context.CursorTo;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.textLayout.MaxTextIndex);

				if ( value != this.context.CursorFrom || value != this.context.CursorTo )
				{
					this.context.CursorFrom = value;
					this.context.CursorTo   = value;
					this.OnCursorScrolled();
					this.OnCursorChanged(true);
				}
			}
		}
		
		public int								CursorFrom
		{
			get
			{
				return this.context.CursorFrom;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.textLayout.MaxTextIndex);

				if ( value != this.context.CursorFrom )
				{
					this.context.CursorFrom = value;
					this.OnCursorScrolled();
					this.OnCursorChanged(true);
				}
			}
		}
		
		public int								CursorTo
		{
			get
			{
				return this.context.CursorTo;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.textLayout.MaxTextIndex);

				if ( value != this.context.CursorTo )
				{
					this.context.CursorTo = value;
					this.OnCursorScrolled();
					this.OnCursorChanged(true);
				}
			}
		}

		public bool								CursorAfter
		{
			get
			{
				return this.context.CursorAfter;
			}

			set
			{
				if ( value != this.context.CursorAfter )
				{
					this.context.CursorAfter = value;
					this.OnCursorScrolled();
					this.OnCursorChanged(true);
				}
			}
		}

		
		public void SetCursors(int from, int to)
		{
			this.SetCursors(from, to, this.CursorAfter);
		}
		
		public void SetCursors(int from, int to, bool after)
		{
			// Modifie les deux curseurs en même temps.
			
			int len = this.textLayout.MaxTextIndex;
			
			from = System.Math.Max(from, 0);
			from = System.Math.Min(from, len);
			
			to = System.Math.Max(to, 0);
			to = System.Math.Min(to, len);
			
			if ( from != this.context.CursorFrom || to != this.context.CursorTo || after != this.context.CursorAfter )
			{
				this.context.CursorFrom  = from;
				this.context.CursorTo    = to;
				this.context.CursorAfter = after;
				this.OnCursorScrolled();
				this.OnCursorChanged(true);
			}
		}
		
		
		public bool ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.KeyDown:
					if ( this.ProcessKeyDown(message.KeyCode, message.IsShiftPressed, message.IsCtrlPressed) )
					{
						message.Swallowed = true;
						return true;
					}
					break;
				
				case MessageType.KeyPress:
					if ( this.ProcessKeyPress(message.KeyChar) )
					{
						return true;
					}
					break;

				case MessageType.MouseDown:
					if ( message.ButtonDownCount == 1 )
					{
						this.ProcessBeginPress(pos);
						this.mouseDrag = true;
					}
					this.mouseDown = true;
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDrag )
					{
						this.ProcessMovePress(pos);
						return true;
					}
					break;
				
				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.ProcessEndPress(pos, message.ButtonDownCount);
						this.mouseDown = false;
						this.mouseDrag = false;
						return true;
					}
					break;
			}

			return false;
		}

		public void MouseDownMessage(Drawing.Point pos)
		{
			this.ProcessBeginPress(pos);
			this.mouseDown = true;
		}

		public void MouseMoveMessage(Drawing.Point pos)
		{
			if ( this.mouseDrag )
			{
				this.ProcessMovePress(pos);
			}
		}

		// Gestion d'une touche pressée avec KeyDown dans le texte.
		protected bool ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isCtrlPressed)
		{
			this.InitialMemorize();

			if ( this.IsMultiLine )
			{
				switch ( key )
				{
					case KeyCode.Return:
						this.textLayout.InsertCharacter(this.context, '\n');
						this.OnTextInserted(false);
						return true;

					case KeyCode.Home:
						if ( isCtrlPressed )
						{
							this.textLayout.MoveCursor(this.context, -1000000, isShiftPressed, false);
						}
						else
						{
							this.textLayout.MoveExtremity(this.context, -1, isShiftPressed);
						}
						this.OnCursorScrolled();
						this.OnCursorChanged(false);
						return true;

					case KeyCode.End:
						if ( isCtrlPressed )
						{
							this.textLayout.MoveCursor(this.context, 1000000, isShiftPressed, false);
						}
						else
						{
							this.textLayout.MoveExtremity(this.context, 1, isShiftPressed);
						}
						this.OnCursorScrolled();
						this.OnCursorChanged(false);
						return true;

					case KeyCode.ArrowUp:
						if ( this.textLayout.MoveLine(this.context, -1, isShiftPressed) )
						{
							this.OnCursorScrolled();
							this.OnCursorChanged(false);
							return true;
						}
						break;

					case KeyCode.ArrowDown:
						if ( this.textLayout.MoveLine(this.context, 1, isShiftPressed) )
						{
							this.OnCursorScrolled();
							this.OnCursorChanged(false);
							return true;
						}
						break;
				
					case KeyCode.PageUp:
					case KeyCode.PageDown:
						// TODO: gérer PageUp et PageDown...
						return true;
				}
			}

			switch ( key )
			{
				case KeyCode.Back:
					if ( this.isReadOnly )  return false;
					if ( isShiftPressed || isCtrlPressed )  return false;
					this.textLayout.DeleteCharacter(this.context, -1);
					this.OnTextDeleted(false);
					this.OnCursorScrolled();
					this.OnCursorChanged(false);
					return true;
				
				case KeyCode.Delete:
					if ( this.isReadOnly )  return false;
					if ( isShiftPressed || isCtrlPressed )  return false;
					this.textLayout.DeleteCharacter(this.context, 1);
					this.OnTextDeleted(false);
					this.OnCursorScrolled();
					this.OnCursorChanged(false);
					return true;
				
				case KeyCode.Home:
					if ( this.textLayout.MoveCursor(this.context, -1000000, isShiftPressed, false) )  // recule beaucoup
					{
						this.OnCursorScrolled();
						this.OnCursorChanged(false);
						return true;
					}
					break;
				
				case KeyCode.End:
					if ( this.textLayout.MoveCursor(this.context, 1000000, isShiftPressed, false) )  // avance beaucoup
					{
						this.OnCursorScrolled();
						this.OnCursorChanged(false);
						return true;
					}
					break;
				
				case KeyCode.ArrowLeft:
					if ( this.textLayout.MoveCursor(this.context, -1, isShiftPressed, isCtrlPressed) )
					{
						this.OnCursorScrolled();
						this.OnCursorChanged(false);
						return true;
					}
					break;
				
				case KeyCode.ArrowRight:
					if ( this.textLayout.MoveCursor(this.context, 1, isShiftPressed, isCtrlPressed) )
					{
						this.OnCursorScrolled();
						this.OnCursorChanged(false);
						return true;
					}
					break;
			}
			
			return false;
		}

		// Gestion d'une touche pressée avec KeyPress dans le texte.
		protected bool ProcessKeyPress(int key)
		{
			if ( this.isReadOnly )  return false;
			this.InitialMemorize();

			if ( key >= 32 )  // TODO: à vérifier ...
			{
				bool replaced = this.textLayout.HasSelection(this.context);
				
				this.textLayout.InsertCharacter(this.context, (char)key);
				
				if ( replaced )
				{
					this.OnTextDeleted(true);
					this.OnTextInserted(true);
				}
				else
				{
					this.OnTextInserted(false);
				}
				
				return true;
			}
			
			return false;
		}

		// Appelé lorsque le bouton de la souris est pressé.
		protected bool ProcessBeginPress(Drawing.Point pos)
		{
			int index;
			bool after;
			if ( this.textLayout.DetectIndex(pos, out index, out after) )
			{
				this.context.CursorFrom  = index;
				this.context.CursorTo    = index;
				this.context.CursorAfter = after;
				this.textLayout.DefineCursorPosX(this.context);
				this.OnCursorChanged(true);
				return true;
			}
			
			return false;
		}

		// Appelé lorsque la souris est déplacée, bouton pressé.
		protected void ProcessMovePress(Drawing.Point pos)
		{
			int index;
			bool after;
			if ( this.textLayout.DetectIndex(pos, out index, out after) )
			{
				this.context.CursorTo    = index;
				this.context.CursorAfter = after;
				this.textLayout.DefineCursorPosX(this.context);
				this.OnCursorChanged(true);
			}
		}

		// Appelé lorsque le bouton de la souris est relâché.
		protected void ProcessEndPress(Drawing.Point pos, int downCount)
		{
			this.InitialMemorize();

			if ( this.IsNumeric )
			{
				if ( downCount >= 2 )  downCount = 4;  // double clic -> sélectionne tout
			}

			if ( downCount >= 4 )  // quadruple clic ?
			{
				this.textLayout.SelectAll(this.context);
			}
			else if ( downCount >= 3 )  // triple clic ?
			{
				this.textLayout.SelectLine(this.context);
			}
			else if ( downCount >= 2 )  // double clic ?
			{
				this.textLayout.SelectWord(this.context);
			}
			else	// simple clic ?
			{
				int index;
				bool after;
				if ( this.textLayout.DetectIndex(pos, out index, out after) )
				{
					this.context.CursorTo    = index;
					this.context.CursorAfter = after;
					this.textLayout.DefineCursorPosX(this.context);
				}
			}

			this.OnCursorChanged(false);
		}


		// Mémorise l'état avant une opération quelconque sur le texte.
		protected void InitialMemorize()
		{
			this.iCursorFrom  = this.context.CursorFrom;
			this.iCursorTo    = this.context.CursorTo;
			this.iCursorAfter = this.context.CursorAfter;
			this.iTextLength  = this.textLayout.Text.Length;
		}

		// Génère un événement pour dire que des caractères ont été insérés.
		protected void OnTextInserted(bool always)
		{
			if ( !always && this.iTextLength == this.textLayout.Text.Length )  return;

			if ( this.TextInserted != null )  // qq'un écoute ?
			{
				this.TextInserted(this);
			}
		}

		public event EventHandler TextInserted;

		// Génère un événement pour dire que des caractères ont été détruits.
		protected void OnTextDeleted(bool always)
		{
			if ( !always && this.iTextLength == this.textLayout.Text.Length )  return;

			if ( this.TextDeleted != null )  // qq'un écoute ?
			{
				this.TextDeleted(this);
			}
		}

		public event EventHandler TextDeleted;

		// Génère un événement pour dire que le curseur a bougé.
		protected void OnCursorChanged(bool always)
		{
			if ( !always                                       &&
				 this.iCursorFrom  == this.context.CursorFrom  &&
				 this.iCursorTo    == this.context.CursorTo    &&
				 this.iCursorAfter == this.context.CursorAfter )  return;

			if ( this.CursorChanged != null )  // qq'un écoute ?
			{
				this.CursorChanged(this);
			}
		}

		public event EventHandler CursorChanged;

		// Génère un événement pour dire que le curseur a scrollé.
		protected void OnCursorScrolled()
		{
			if ( this.CursorScrolled != null )  // qq'un écoute ?
			{
				this.CursorScrolled(this);
			}
		}

		public event EventHandler CursorScrolled;


		protected TextLayout					textLayout;
		protected TextLayout.Context			context;
		protected bool							isReadOnly = false;
		protected bool							isNumeric = false;
		protected int							iCursorFrom;
		protected int							iCursorTo;
		protected bool							iCursorAfter;
		protected int							iTextLength;
		protected bool							mouseDown = false;
		protected bool							mouseDrag = false;
	}
}
