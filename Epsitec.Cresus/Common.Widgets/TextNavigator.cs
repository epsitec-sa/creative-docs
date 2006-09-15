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
			this.context = new TextLayout.Context(textLayout);
		}

		
		public TextLayout.Context				Context
		{
			get { return this.context; }
		}

		public TextLayout						TextLayout
		{
			get
			{
				return this.textLayout;
			}
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

		public bool								AllowTabInsertion
		{
			get { return this.allowTabInsertion; }
			set { this.allowTabInsertion = value; }
		}

		public bool								IsEmpty
		{
			get
			{
				return this.textLayout.Text.Length == 0;
			}
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
			//	Modifie les deux curseurs en même temps.
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
			//	Attribut typographique "ondulé" des caractères sélectionnés.
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
			try
			{
				this.textLayout.SuspendTextChangeNotifications ();
				switch (message.Type)
				{
					case MessageType.KeyDown:
						if (message.IsAltPressed)
						{
						}
						else if (this.ProcessKeyDown (message.KeyCode, message.IsShiftPressed, message.IsControlPressed))
						{
							message.Swallowed = true;
							return true;
						}
						break;

					case MessageType.KeyPress:
						if (message.IsAltPressed)
						{
						}
						else if (this.ProcessKeyPress (message.KeyChar))
						{
							return true;
						}
						break;

					case MessageType.MouseDown:
						if (message.ButtonDownCount == 1)
						{
							this.ProcessBeginPress (pos);
							this.mouseDrag = true;
						}
						this.mouseDown = true;
						break;

					case MessageType.MouseMove:
						if (this.mouseDrag)
						{
							this.ProcessMovePress (pos);
							return true;
						}
						break;

					case MessageType.MouseUp:
						if (this.mouseDown)
						{
							this.ProcessEndPress (pos, message.ButtonDownCount);
							this.mouseDown = false;
							this.mouseDrag = false;
							return true;
						}
						break;
				}

				return false;
			}
			finally
			{
				this.textLayout.ResumeTextChangeNotifications ();
			}
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

		protected bool ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isControlPressed)
		{
			//	Gestion d'une touche pressée avec KeyDown dans le texte.
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
						if ( this.isReadOnly || !this.allowTabInsertion )  return false;
						this.UndoMemorise(UndoType.Insert);
						this.textLayout.InsertCharacter(this.context, '\t');
						this.OnTextInserted(false);
						return true;

					case KeyCode.Home:
						if ( isControlPressed )
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
						if ( isControlPressed )
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
						//	TODO: gérer PageUp et PageDown...
						return true;
				}
			}

			switch ( key )
			{
				case KeyCode.Back:
					if ( this.isReadOnly )  return false;
					if ( isShiftPressed || isControlPressed )  return false;
					this.UndoMemorise(UndoType.Delete);
					this.textLayout.DeleteCharacter(this.context, -1);
					this.OnTextDeleted(false);
					this.OnCursorScrolled();
					this.OnCursorChanged(false);
					return true;
				
				case KeyCode.Delete:
					if ( this.isReadOnly )  return false;
					if ( isShiftPressed || isControlPressed )  return false;
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
					if ( this.textLayout.MoveCursor(this.context, -1, isShiftPressed, isControlPressed) )
					{
						this.OnCursorScrolled();
						this.OnCursorChanged(false);
						return true;
					}
					break;
				
				case KeyCode.ArrowRight:
					if ( this.textLayout.MoveCursor(this.context, 1, isShiftPressed, isControlPressed) )
					{
						this.OnCursorScrolled();
						this.OnCursorChanged(false);
						return true;
					}
					break;
			}
			
			return false;
		}

		protected bool ProcessKeyPress(int key)
		{
			//	Gestion d'une touche pressée avec KeyPress dans le texte.
			if ( this.isReadOnly )  return false;
			this.InitialMemorize();

			if ( key >= 32 )  // TODO: à vérifier ...
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

		protected bool ProcessBeginPress(Drawing.Point pos)
		{
			//	Appelé lorsque le bouton de la souris est pressé.
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

		protected void ProcessMovePress(Drawing.Point pos)
		{
			//	Appelé lorsque la souris est déplacée, bouton pressé.
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

		protected void ProcessEndPress(Drawing.Point pos, int downCount)
		{
			//	Appelé lorsque le bouton de la souris est relâché.
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

		protected void UndoMemorise(UndoType type)
		{
			//	Mémorise l'état actuel complet du texte, pour permettre l'annulation.
			this.OnAboutToChange ();
			
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
					return;  // situation initiale déjà mémorisée
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
			public TextOplet(TextNavigator navigator, UndoType type)
			{
				//	Effectue une copie du texte et du contexte.
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

			protected void Swap()
			{
				//	Permute le texte et le contexte contenus par l'hôte avec ceux
				//	contenus dans TextOplet.
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


		protected void InitialMemorize()
		{
			//	Mémorise l'état avant une opération quelconque sur le texte.
			this.iCursorFrom  = this.context.CursorFrom;
			this.iCursorTo    = this.context.CursorTo;
			this.iCursorAfter = this.context.CursorAfter;
			this.iTextLength  = this.textLayout.Text.Length;
		}

		protected void OnTextInserted(bool always)
		{
			//	Génère un événement pour dire que des caractères ont été insérés.
			if ( !always && this.iTextLength == this.textLayout.Text.Length )  return;

			if ( this.TextInserted != null )  // qq'un écoute ?
			{
				this.TextInserted(this);
			}
		}

		public event EventHandler TextInserted;

		//	Génère un événement pour dire que des caractères ont été détruits.
		protected void OnTextDeleted(bool always)
		{
			if ( !always && this.iTextLength == this.textLayout.Text.Length )  return;

			if ( this.TextDeleted != null )  // qq'un écoute ?
			{
				this.TextDeleted(this);
			}
		}


		protected void OnCursorChanged(bool always)
		{
			//	Génère un événement pour dire que le curseur a bougé.
			if ( !always                                       &&
				 this.iCursorFrom  == this.context.CursorFrom  &&
				 this.iCursorTo    == this.context.CursorTo    &&
				 this.iCursorAfter == this.context.CursorAfter )  return;

			if ( this.CursorChanged != null )  // qq'un écoute ?
			{
				this.CursorChanged(this);
			}
		}


		protected void OnCursorScrolled()
		{
			//	Génère un événement pour dire que le curseur a scrollé.
			if ( this.CursorScrolled != null )  // qq'un écoute ?
			{
				this.CursorScrolled(this);
			}
		}


		protected void OnStyleChanged()
		{
			//	Génère un événement pour dire que le style a changé.
			if ( this.StyleChanged != null )  // qq'un écoute ?
			{
				this.StyleChanged(this);
			}
		}

		
		protected void OnAboutToChange()
		{
			if (this.AboutToChange != null)
			{
				this.AboutToChange (this);
			}
		}
		
		public event EventHandler				AboutToChange;
		public event EventHandler				TextDeleted;
		public event EventHandler				CursorChanged;
		public event EventHandler				CursorScrolled;
		public event EventHandler				StyleChanged;


		protected TextLayout					textLayout;
		protected TextLayout.Context			context;
		protected Support.OpletQueue			undoQueue;
		protected bool							isReadOnly = false;
		protected bool							isNumeric = false;
		protected bool							allowTabInsertion = false;
		protected int							iCursorFrom;
		protected int							iCursorTo;
		protected bool							iCursorAfter;
		protected int							iTextLength;
		protected bool							mouseDown = false;
		protected bool							mouseDrag = false;
		protected bool							mouseSelZone = false;
	}
}
