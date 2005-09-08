using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextNavigator permet de naviguer dans un TextLayout,
	/// d'ins�rer/supprimer des caract�res, etc.
	/// </summary>
	public class TextNavigator
	{
		public TextNavigator(TextLayout textLayout)
		{
			this.textLayout = textLayout;
			this.context = new TextLayout.Context(textLayout);
		}

		
		public TextLayout.Context				Context
		{
			get { return this.context; }
		}

		public Support.OpletQueue				OpletQueue
		{
			get { return this.undoQueue; }
			set { this.undoQueue = value; }
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
				int cursorFrom = System.Math.Min(this.context.CursorFrom, this.context.CursorTo);
				int cursorTo   = System.Math.Max(this.context.CursorFrom, this.context.CursorTo);
				
				int from = this.textLayout.FindOffsetFromIndex(cursorFrom, false);
				int to   = this.textLayout.FindOffsetFromIndex(cursorTo, true);
				
				string text = this.textLayout.Text;
				return TextLayout.SubstringComplete(text, from, to);
			}

			set
			{
				this.UndoMemorise(UndoType.Insert);
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
			// Modifie les deux curseurs en m�me temps.
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

		public void ValidateCursors()
		{
			int max = this.textLayout.MaxTextIndex;

			if ( this.context.CursorFrom > max )
			{
				this.context.CursorFrom = max;
			}

			if ( this.context.CursorTo > max )
			{
				this.context.CursorTo = max;
			}
		}
		
		
		public bool								SelectionBold
		{
			get
			{
				return this.textLayout.IsSelectionBold(this.context);
			}

			set
			{
				if ( this.textLayout.IsSelectionBold(this.context) != value )
				{
					this.UndoMemorise(UndoType.AutonomusStyle);
					this.textLayout.SetSelectionBold(this.context, value);
					this.OnStyleChanged();
				}
			}
		}

		public bool								SelectionItalic
		{
			get
			{
				return this.textLayout.IsSelectionItalic(this.context);
			}

			set
			{
				if ( this.textLayout.IsSelectionItalic(this.context) != value )
				{
					this.UndoMemorise(UndoType.AutonomusStyle);
					this.textLayout.SetSelectionItalic(this.context, value);
					this.OnStyleChanged();
				}
			}
		}

		public bool								SelectionUnderlined
		{
			get
			{
				return this.textLayout.IsSelectionUnderlined(this.context);
			}

			set
			{
				if ( this.textLayout.IsSelectionUnderlined(this.context) != value )
				{
					this.UndoMemorise(UndoType.AutonomusStyle);
					this.textLayout.SetSelectionUnderlined(this.context, value);
					this.OnStyleChanged();
				}
			}
		}

		public bool								SelectionWaved
		{
			// Attribut typographique "ondul�" des caract�res s�lectionn�s.
			get
			{
				return this.textLayout.IsSelectionWaved(this.context);
			}
		}
		
		public string							SelectionFontName
		{
			get
			{
				return this.textLayout.GetSelectionFontName(this.context);
			}

			set
			{
				if ( this.textLayout.GetSelectionFontName(this.context) != value )
				{
					this.UndoMemorise(UndoType.CascadableStyle);
					this.textLayout.SetSelectionFontName(this.context, value);
					this.OnStyleChanged();
				}
			}
		}

		public double							SelectionFontScale
		{
			get
			{
				return this.textLayout.GetSelectionFontScale(this.context);
			}

			set
			{
				if ( this.textLayout.GetSelectionFontScale(this.context) != value )
				{
					this.UndoMemorise(UndoType.CascadableStyle);
					this.textLayout.SetSelectionFontScale(this.context, value);
					this.OnStyleChanged();
				}
			}
		}

		public Drawing.RichColor				SelectionFontRichColor
		{
			get
			{
				return this.textLayout.GetSelectionFontRichColor(this.context);
			}

			set
			{
				if ( this.textLayout.GetSelectionFontRichColor(this.context) != value )
				{
					this.UndoMemorise(UndoType.CascadableStyle);
					this.textLayout.SetSelectionFontRichColor(this.context, value);
					this.OnStyleChanged();
				}
			}
		}

		public Drawing.Color					SelectionFontColor
		{
			get
			{
				return this.textLayout.GetSelectionFontColor(this.context);
			}

			set
			{
				if ( this.textLayout.GetSelectionFontColor(this.context) != value )
				{
					this.UndoMemorise(UndoType.CascadableStyle);
					this.textLayout.SetSelectionFontColor(this.context, value);
					this.OnStyleChanged();
				}
			}
		}

		public Drawing.TextListType				SelectionList
		{
			get
			{
				return this.textLayout.GetSelectionList(this.context);
			}

			set
			{
				if ( this.textLayout.GetSelectionList(this.context) != value )
				{
					this.UndoMemorise(UndoType.AutonomusStyle);
					this.textLayout.SetSelectionList(this.context, value);
					this.OnStyleChanged();
				}
			}
		}


		public void DeleteSelection()
		{
			if ( this.textLayout.DeleteSelection(this.context) )
			{
				this.OnTextDeleted(true);
			}
		}


		public void TabUndoMemorise()
		{
			this.UndoMemorise(UndoType.Tab);
		}

		public int TabInsert(Drawing.TextStyle.Tab tab)
		{
			int rank = this.textLayout.Style.TabInsert(tab);
			this.OnStyleChanged();
			return rank;
		}

		public int TabCount
		{
			get
			{
				return this.textLayout.Style.TabCount;
			}
		}

		public void TabRemoveAt(int rank)
		{
			this.textLayout.Style.TabRemoveAt(rank);
			this.OnStyleChanged();
		}

		public Drawing.TextStyle.Tab GetTab(int rank)
		{
			return this.textLayout.Style.GetTab(rank);
		}

		public void SetTabPosition(int rank, double pos)
		{
			this.textLayout.SetTabPosition(rank, pos);
			this.OnStyleChanged();
		}


		public bool ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.KeyDown:
					if ( this.ProcessKeyDown(message.KeyCode, message.IsShiftPressed, message.IsCtrlPressed) )
					{
						//?System.Diagnostics.Debug.WriteLine(this.textLayout.Text);
						message.Swallowed = true;
						return true;
					}
					break;
				
				case MessageType.KeyPress:
					if ( this.ProcessKeyPress(message.KeyChar) )
					{
						//?System.Diagnostics.Debug.WriteLine(this.textLayout.Text);
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

		// Gestion d'une touche press�e avec KeyDown dans le texte.
		protected bool ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isCtrlPressed)
		{
			this.InitialMemorize();

			if ( this.IsMultiLine )
			{
				switch ( key )
				{
					case KeyCode.Return:
						if ( this.isReadOnly )  return false;
						this.UndoMemorise(UndoType.Insert);
						this.textLayout.InsertCharacter(this.context, '\n');
						this.OnTextInserted(false);
						return true;

					case KeyCode.Tab:
						if ( this.isReadOnly )  return false;
						this.UndoMemorise(UndoType.Insert);
						this.textLayout.InsertCharacter(this.context, '\t');
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
						// TODO: g�rer PageUp et PageDown...
						return true;
				}
			}

			switch ( key )
			{
				case KeyCode.Back:
					if ( this.isReadOnly )  return false;
					if ( isShiftPressed || isCtrlPressed )  return false;
					this.UndoMemorise(UndoType.Delete);
					this.textLayout.DeleteCharacter(this.context, -1);
					this.OnTextDeleted(false);
					this.OnCursorScrolled();
					this.OnCursorChanged(false);
					return true;
				
				case KeyCode.Delete:
					if ( this.isReadOnly )  return false;
					if ( isShiftPressed || isCtrlPressed )  return false;
					this.UndoMemorise(UndoType.Delete);
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

		// Gestion d'une touche press�e avec KeyPress dans le texte.
		protected bool ProcessKeyPress(int key)
		{
			if ( this.isReadOnly )  return false;
			this.InitialMemorize();

			if ( key >= 32 )  // TODO: � v�rifier ...
			{
				bool replaced = this.textLayout.HasSelection(this.context);
				
				this.UndoMemorise(UndoType.Insert);
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

		// Appel� lorsque le bouton de la souris est press�.
		protected bool ProcessBeginPress(Drawing.Point pos)
		{
			int index;
			bool after;
			this.mouseSelZone = false;
			if ( this.textLayout.DetectIndex(pos, false, out index, out after) )
			{
				if ( this.context.CursorFrom  != index ||
					 this.context.CursorTo    != index )
				{
					this.context.CursorFrom  = index;
					this.context.CursorTo    = index;
					this.context.CursorAfter = after;
					this.textLayout.DefineCursorPosX(this.context);
					this.OnCursorChanged(true);
				}
				return true;
			}
			
			return false;
		}

		// Appel� lorsque la souris est d�plac�e, bouton press�.
		protected void ProcessMovePress(Drawing.Point pos)
		{
			int index;
			bool after;
			if ( this.textLayout.DetectIndex(pos, true, out index, out after) )
			{
				this.mouseSelZone = true;
				if ( this.context.CursorTo != index )
				{
					this.context.CursorTo    = index;
					this.context.CursorAfter = after;
					this.textLayout.DefineCursorPosX(this.context);
					this.OnCursorChanged(true);
				}
			}
		}

		// Appel� lorsque le bouton de la souris est rel�ch�.
		protected void ProcessEndPress(Drawing.Point pos, int downCount)
		{
			this.InitialMemorize();

			if ( this.IsNumeric )
			{
				if ( downCount >= 2 )  downCount = 4;  // double clic -> s�lectionne tout
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
				if ( this.textLayout.DetectIndex(pos, this.mouseSelZone, out index, out after) )
				{
					if ( this.context.CursorTo != index )
					{
						this.context.CursorTo    = index;
						this.context.CursorAfter = after;
						this.textLayout.DefineCursorPosX(this.context);
					}
				}
			}

			this.OnCursorChanged(false);
		}


		protected enum UndoType
		{
			Insert,
			Delete,
			CascadableStyle,	// plusieurs modifs -> un seul undo global
			AutonomusStyle,		// plusieurs modifs -> autant de undo que de modifs
			Tab,
		}

		// M�morise l'�tat actuel complet du texte, pour permettre l'annulation.
		protected void UndoMemorise(UndoType type)
		{
			if ( this.undoQueue == null )  return;

			Support.IOplet[] oplets = this.undoQueue.LastActionOplets;
			if ( !this.undoQueue.CanRedo     &&
				 oplets.Length == 1          &&
				 !this.context.UndoSeparator )
			{
				TextOplet lastOplet = oplets[0] as TextOplet;
				if ( type != UndoType.AutonomusStyle &&
					 type != UndoType.Tab            &&
					 lastOplet != null               &&
					 lastOplet.Navigator == this     &&
					 lastOplet.Type == type          )
				{
					return;  // situation initiale d�j� m�moris�e
				}
			}

			string name = Res.Strings.TextNavigator.Action.Modify;
			switch ( type )
			{
				case UndoType.Insert:           name = Res.Strings.TextNavigator.Action.Insert;  break;
				case UndoType.Delete:           name = Res.Strings.TextNavigator.Action.Delete;  break;
				case UndoType.AutonomusStyle:
				case UndoType.CascadableStyle:  name = Res.Strings.TextNavigator.Action.Style;   break;
				case UndoType.Tab:              name = Res.Strings.TextNavigator.Action.Tab;     break;
			}

			using ( this.undoQueue.BeginAction(name) )
			{
				TextOplet oplet = new TextOplet(this, type);
				this.undoQueue.Insert(oplet);
				this.undoQueue.ValidateAction();
			}

			this.context.UndoSeparator = false;
		}

		protected class TextOplet : Support.AbstractOplet
		{
			// Effectue une copie du texte et du contexte.
			public TextOplet(TextNavigator navigator, UndoType type)
			{
				this.host = navigator;
				this.type = type;
				this.textCopy = string.Copy(this.host.textLayout.InternalText);
				this.contextCopy = TextLayout.Context.Copy(this.host.context);
				this.host.textLayout.Style.TabCopyTo(out this.tabs);
			}

			public TextNavigator Navigator
			{
				get { return this.host; }
			}

			public UndoType Type
			{
				get { return this.type; }
			}

			// Permute le texte et le contexte contenus par l'h�te avec ceux
			// contenus dans TextOplet.
			protected void Swap()
			{
				string undoText = string.Copy(this.textCopy);
				string redoText = string.Copy(this.host.textLayout.InternalText);
				this.host.textLayout.Text = undoText;
				this.textCopy = redoText;

				TextLayout.Context undoContext = TextLayout.Context.Copy(this.contextCopy);
				TextLayout.Context redoContext = TextLayout.Context.Copy(this.host.context);
				undoContext.CopyTo(this.host.context);
				redoContext.CopyTo(this.contextCopy);

				Drawing.TextStyle.Tab[] temp;
				this.host.textLayout.Style.TabCopyTo(out temp);
				this.host.textLayout.Style.TabCopyFrom(this.tabs);
				this.tabs = temp;

				this.host.OnCursorChanged(true);
			}

			public override Support.IOplet Undo()
			{
				this.Swap();  // permutation

				if ( this.type == UndoType.Insert )
				{
					this.host.OnTextDeleted(true);
				}
				else if ( this.type == UndoType.Delete )
				{
					this.host.OnTextInserted(true);
				}
				else
				{
					this.host.OnStyleChanged();
				}

				return this;
			}

			public override Support.IOplet Redo()
			{
				this.Swap();  // permutation

				if ( this.type == UndoType.Insert )
				{
					this.host.OnTextInserted(true);
				}
				else if ( this.type == UndoType.Delete )
				{
					this.host.OnTextDeleted(true);
				}
				else
				{
					this.host.OnStyleChanged();
				}

				return this;
			}

			protected TextNavigator				host;
			protected UndoType					type;
			protected string					textCopy;
			protected TextLayout.Context		contextCopy;
			protected Drawing.TextStyle.Tab[]	tabs;
		}


		// M�morise l'�tat avant une op�ration quelconque sur le texte.
		protected void InitialMemorize()
		{
			this.iCursorFrom  = this.context.CursorFrom;
			this.iCursorTo    = this.context.CursorTo;
			this.iCursorAfter = this.context.CursorAfter;
			this.iTextLength  = this.textLayout.Text.Length;
		}

		// G�n�re un �v�nement pour dire que des caract�res ont �t� ins�r�s.
		protected void OnTextInserted(bool always)
		{
			if ( !always && this.iTextLength == this.textLayout.Text.Length )  return;

			if ( this.TextInserted != null )  // qq'un �coute ?
			{
				this.TextInserted(this);
			}
		}

		public event EventHandler TextInserted;

		// G�n�re un �v�nement pour dire que des caract�res ont �t� d�truits.
		protected void OnTextDeleted(bool always)
		{
			if ( !always && this.iTextLength == this.textLayout.Text.Length )  return;

			if ( this.TextDeleted != null )  // qq'un �coute ?
			{
				this.TextDeleted(this);
			}
		}

		public event EventHandler TextDeleted;

		// G�n�re un �v�nement pour dire que le curseur a boug�.
		protected void OnCursorChanged(bool always)
		{
			if ( !always                                       &&
				 this.iCursorFrom  == this.context.CursorFrom  &&
				 this.iCursorTo    == this.context.CursorTo    &&
				 this.iCursorAfter == this.context.CursorAfter )  return;

			if ( this.CursorChanged != null )  // qq'un �coute ?
			{
				this.CursorChanged(this);
			}
		}

		public event EventHandler CursorChanged;

		// G�n�re un �v�nement pour dire que le curseur a scroll�.
		protected void OnCursorScrolled()
		{
			if ( this.CursorScrolled != null )  // qq'un �coute ?
			{
				this.CursorScrolled(this);
			}
		}

		public event EventHandler CursorScrolled;

		// G�n�re un �v�nement pour dire que le style a chang�.
		protected void OnStyleChanged()
		{
			if ( this.StyleChanged != null )  // qq'un �coute ?
			{
				this.StyleChanged(this);
			}
		}

		public event EventHandler StyleChanged;


		protected TextLayout					textLayout;
		protected TextLayout.Context			context;
		protected Support.OpletQueue			undoQueue;
		protected bool							isReadOnly = false;
		protected bool							isNumeric = false;
		protected int							iCursorFrom;
		protected int							iCursorTo;
		protected bool							iCursorAfter;
		protected int							iTextLength;
		protected bool							mouseDown = false;
		protected bool							mouseDrag = false;
		protected bool							mouseSelZone = false;
	}
}
