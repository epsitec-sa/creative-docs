//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Text;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>TextNavigator</c> class can be used to navigate through a
	/// <see cref="TextLayout"/> instance, including edition and mouse
	/// interaction.
	/// </summary>
	public sealed class TextNavigator
	{
		public TextNavigator(AbstractTextField textField, TextLayout textLayout)
		{
			this.textField  = textField;
			this.textLayout = textLayout;
			this.context    = new TextLayoutContext(textLayout);
		}

		
		public TextLayoutContext				Context
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
			get
			{
				return this.opletQueue;
			}
			set
			{
				this.opletQueue = value;
			}
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
				if (this.textLayout.IsSingleLine)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}
		
		public int								MaxLength
		{
			get { return this.context.MaxLength; }
			set { this.context.MaxLength = value; }
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
				this.UndoMemorize(UndoType.Insert);
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

		
		
		public bool								SelectionBold
		{
			get
			{
				return this.textLayout.IsSelectionBold(this.context);
			}

			set
			{
				if (this.textLayout.IsSelectionBold (this.context) != value)
				{
					this.UndoMemorize (UndoType.AutonomusStyle);
					this.textLayout.SetSelectionBold (this.context, value);
					this.OnStyleChanged ();
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
				if (this.textLayout.IsSelectionItalic (this.context) != value)
				{
					this.UndoMemorize (UndoType.AutonomusStyle);
					this.textLayout.SetSelectionItalic (this.context, value);
					this.OnStyleChanged ();
				}
			}
		}

		public bool								SelectionUnderline
		{
			get
			{
				return this.textLayout.IsSelectionUnderline(this.context);
			}

			set
			{
				if (this.textLayout.IsSelectionUnderline (this.context) != value)
				{
					this.UndoMemorize (UndoType.AutonomusStyle);
					this.textLayout.SetSelectionUnderline (this.context, value);
					this.OnStyleChanged ();
				}
			}
		}

		public bool								SelectionSubscript
		{
			get
			{
				return this.textLayout.IsSelectionSubscript (this.context);
			}

			set
			{
				if (this.textLayout.IsSelectionSubscript (this.context) != value)
				{
					this.UndoMemorize(UndoType.AutonomusStyle);
					this.textLayout.SetSelectionSubscript (this.context, value);
					this.OnStyleChanged();
				}
			}
		}

		public bool								SelectionSuperscript
		{
			get
			{
				return this.textLayout.IsSelectionSuperscript (this.context);
			}

			set
			{
				if (this.textLayout.IsSelectionSuperscript (this.context) != value)
				{
					this.UndoMemorize(UndoType.AutonomusStyle);
					this.textLayout.SetSelectionSuperscript (this.context, value);
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
				return this.textLayout.GetSelectionFontFace(this.context);
			}

			set
			{
				if ( this.textLayout.GetSelectionFontFace(this.context) != value )
				{
					this.UndoMemorize(UndoType.CascadableStyle);
					this.textLayout.SetSelectionFontFace(this.context, value);
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
					this.UndoMemorize(UndoType.CascadableStyle);
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
					this.UndoMemorize(UndoType.CascadableStyle);
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
					this.UndoMemorize(UndoType.CascadableStyle);
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
					this.UndoMemorize(UndoType.AutonomusStyle);
					this.textLayout.SetSelectionList(this.context, value);
					this.OnStyleChanged();
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
		
		public void DeleteSelection()
		{
			if ( this.textLayout.DeleteSelection(this.context) )
			{
				this.OnTextDeleted(true);
			}
		}

		public void ReplaceWithText(string text)
		{
			this.UndoMemorize(UndoType.Insert);
			this.textLayout.SelectAll (this.context);
			this.textLayout.ReplaceSelection (this.context, text);
			this.OnTextInserted(true);
			this.OnCursorScrolled();
			this.OnCursorChanged(true);
		}

		public void SelectAll()
		{
			this.textLayout.SelectAll (this.context);
		}

		public void TabUndoMemorize()
		{
			this.UndoMemorize(UndoType.Tab);
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
				
				switch (message.MessageType)
				{
					case MessageType.KeyDown:
						if (this.ProcessSpecialCharacters (message))
						{
							message.Swallowed = true;
							return true;
						}
						else if (message.IsAltPressed && !message.IsControlPressed)
						{
						}
						else if (this.ProcessKeyDown (message.KeyCode, message.IsShiftPressed, message.IsControlPressed))
						{
							message.Swallowed = true;
							return true;
						}
						break;

					case MessageType.KeyPress:
						if (message.IsAltPressed && !message.IsControlPressed)
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
							this.ProcessMouseDown (pos);
							this.mouseDrag = true;
						}
						this.mouseDown = true;
						break;

					case MessageType.MouseMove:
						if (this.mouseDrag)
						{
							this.ProcessMouseDrag (pos);
							return true;
						}
						break;

					case MessageType.MouseUp:
						if (this.mouseDown)
						{
							this.ProcessMouseUp (pos, message.ButtonDownCount);
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
//				this.initialInfo = null;
			}
		}

		public void MouseDownMessage(Drawing.Point pos)
		{
			this.ProcessMouseDown(pos);
			this.mouseDown = true;
		}

		public void MouseMoveMessage(Drawing.Point pos)
		{
			if ( this.mouseDrag )
			{
				this.ProcessMouseDrag(pos);
			}
		}

		
		private bool ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isControlPressed)
		{
			//	Gestion d'une touche pressée avec KeyDown dans le texte.
			this.InitialMemorize();

			if ( this.IsMultiLine )
			{
				switch ( key )
				{
					case KeyCode.Return:
						if ( this.isReadOnly )  return false;
						this.UndoMemorize(UndoType.Insert);
						this.textLayout.InsertCharacter(this.context, '\n');
						this.OnTextInserted(false);
						return true;

					case KeyCode.Tab:
						if ( this.isReadOnly || !this.allowTabInsertion )  return false;
						this.UndoMemorize(UndoType.Insert);
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
					this.UndoMemorize(UndoType.Delete);
					this.textLayout.DeleteCharacter(this.context, -1);
					this.OnTextDeleted(false);
					this.OnCursorScrolled();
					this.OnCursorChanged(false);
					return true;
				
				case KeyCode.Delete:
					if ( this.isReadOnly )  return false;
					if ( isShiftPressed || isControlPressed )  return false;
					this.UndoMemorize(UndoType.Delete);
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

		private bool ProcessSpecialCharacters(Message message)
		{
			string text = TextNavigator.MapKeyToSpecialCharacter (message);

			if (text != null)
			{
				bool replaced = this.textLayout.HasSelection (this.context);

				this.UndoMemorize (UndoType.Insert);
				this.textLayout.InsertCharacters (this.context, text);

				if (replaced)
				{
					this.OnTextDeleted (true);
					this.OnTextInserted (true);
				}
				else
				{
					this.OnTextInserted (false);
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool ProcessKeyPress(int key)
		{
			//	Gestion d'une touche pressée avec KeyPress dans le texte.
			if ( this.isReadOnly )  return false;
			this.InitialMemorize();

			if ( key >= 32 )  // TODO: à vérifier ...
			{
				bool replaced = this.textLayout.HasSelection(this.context);
				
				this.UndoMemorize(UndoType.Insert);
				this.textLayout.InsertCharacter (this.context, (char) key);
				
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

		private static string MapKeyToSpecialCharacter(Message message)
		{
			if ((message.IsControlPressed) ||
				(message.IsAltPressed))
			{
				System.Diagnostics.Debug.WriteLine (message.ToString ());
				
				if (message.IsControlPressed && message.IsAltPressed && !message.IsShiftPressed)
				{
					switch (message.KeyCode)
					{
						case KeyCode.NumericSubstract:
							return Unicode.ToString (Unicode.Code.EmDash);
					}
				}
				else if (message.IsControlPressed && !message.IsAltPressed && !message.IsShiftPressed)
				{
					switch (message.KeyCode)
					{
						case KeyCode.NumericSubstract:
							return Unicode.ToString (Unicode.Code.EnDash);

						case KeyCode.Dash:
							return Unicode.ToString (Unicode.Code.NonBreakingHyphen);
					}
				}
				else if (message.IsControlPressed && !message.IsAltPressed && message.IsShiftPressed)
				{
					switch (message.KeyCode)
					{
						case KeyCode.Space:
							return Unicode.ToString (Unicode.Code.NoBreakSpace);
					}
				}
			}
			
			return null;
		}

		private bool ProcessMouseDown(Drawing.Point pos)
		{
			//	Appelé lorsque le bouton de la souris est pressé.
			int index;
			bool after;
			this.mouseMoved = false;
			if (this.DetectIndex (pos, out index, out after))
			{
				if ((this.context.CursorFrom != index) ||
					(this.context.CursorTo != index))
				{
					this.context.CursorFrom  = index;
					this.context.CursorTo    = index;
					this.context.CursorAfter = after;
					this.textLayout.DefineCursorPosX (this.context);
					this.OnCursorChanged (true);
				}
				return true;
			}

			return false;
		}

		private bool DetectIndex(Epsitec.Common.Drawing.Point pos, out int index, out bool after)
		{
			if (this.textField == null)
			{
				return this.textLayout.DetectIndex (pos, this.mouseMoved, out index, out after);
			}
			else
			{
				return this.textField.DetectIndex (pos, this.mouseMoved, out index, out after);
			}
		}

		private void ProcessMouseDrag(Drawing.Point pos)
		{
			//	Appelé lorsque la souris est déplacée, bouton pressé.
			int index;
			bool after;
			this.mouseMoved = true;
			if (this.DetectIndex (pos, out index, out after))
			{
				if (this.context.CursorTo != index)
				{
					this.context.CursorTo    = index;
					this.context.CursorAfter = after;
					this.textLayout.DefineCursorPosX (this.context);
					this.OnCursorChanged (true);
				}
			}
			else
			{
				//	Not within the bounds of the text layout...
			}
		}

		private void ProcessMouseUp(Drawing.Point pos, int downCount)
		{
			//	Appelé lorsque le bouton de la souris est relâché.
			this.InitialMemorize ();

			if (this.IsNumeric)
			{
				if (downCount >= 2)
					downCount = 4;  // double clic -> sélectionne tout
			}

			if (downCount >= 4)  // quadruple clic ?
			{
				this.textLayout.SelectAll (this.context);
			}
			else if (downCount >= 3)  // triple clic ?
			{
				this.textLayout.SelectLine (this.context);
			}
			else if (downCount >= 2)  // double clic ?
			{
				this.textLayout.SelectWord (this.context);
			}
			else	// simple clic ?
			{
				int index;
				bool after;
				if (this.DetectIndex (pos, out index, out after))
				{
					if (this.context.CursorTo != index)
					{
						this.context.CursorTo    = index;
						this.context.CursorAfter = after;
						this.textLayout.DefineCursorPosX (this.context);
					}
				}
			}

			this.OnCursorChanged (false);
		}

		private void UndoMemorize(UndoType type)
		{
			//	Mémorise l'état actuel complet du texte, pour permettre l'annulation.
			this.OnAboutToChange ();

			if (this.opletQueue == null)
				return;

			Support.IOplet[] oplets = this.opletQueue.LastActionOplets;
			if (!this.opletQueue.CanRedo     &&
				 oplets.Length == 1          &&
				 !this.context.UndoSeparator)
			{
				TextOplet lastOplet = oplets[0] as TextOplet;
				if (type != UndoType.AutonomusStyle &&
					 type != UndoType.Tab            &&
					 lastOplet != null               &&
					 lastOplet.Navigator == this     &&
					 lastOplet.Type == type)
				{
					return;  // situation initiale déjà mémorisée
				}
			}

			var name = Res.Strings.TextNavigator.Action.Modify;
			
			switch (type)
			{
				case UndoType.Insert:
					name = Res.Strings.TextNavigator.Action.Insert;
					break;
				case UndoType.Delete:
					name = Res.Strings.TextNavigator.Action.Delete;
					break;
				case UndoType.AutonomusStyle:
				case UndoType.CascadableStyle:
					name = Res.Strings.TextNavigator.Action.Style;
					break;
				case UndoType.Tab:
					name = Res.Strings.TextNavigator.Action.Tab;
					break;
			}

			using (this.opletQueue.BeginAction (name.ToSimpleText ()))
			{
				TextOplet oplet = new TextOplet (this, type);
				this.opletQueue.Insert (oplet);
				this.opletQueue.ValidateAction ();
			}

			this.context.UndoSeparator = false;
		}

		#region UndoType Enumeration

		private enum UndoType
		{
			Insert,
			Delete,
			CascadableStyle,	// plusieurs modifs -> un seul undo global
			AutonomusStyle,		// plusieurs modifs -> autant de undo que de modifs
			Tab,
		}

		#endregion

		#region TextOplet Class

		private sealed class TextOplet : Support.AbstractOplet
		{
			public TextOplet(TextNavigator navigator, UndoType type)
			{
				//	Effectue une copie du texte et du contexte.
				this.host = navigator;
				this.type = type;
				this.textCopy = string.Copy(this.host.textLayout.InternalText);
				this.contextCopy = new TextLayoutContext (this.host.context);
				this.tabs = this.host.textLayout.Style.GetTabs ();
			}

			public TextNavigator Navigator
			{
				get { return this.host; }
			}

			public UndoType Type
			{
				get { return this.type; }
			}

			private void Swap()
			{
				//	Permute le texte et le contexte contenus par l'hôte avec ceux
				//	contenus dans TextOplet.
				string undoText = string.Copy(this.textCopy);
				string redoText = string.Copy(this.host.textLayout.InternalText);
				this.host.textLayout.Text = undoText;
				this.textCopy = redoText;

				TextLayoutContext undoContext = new TextLayoutContext (this.contextCopy);
				TextLayoutContext redoContext = new TextLayoutContext (this.host.context);
				undoContext.CopyTo(this.host.context);
				redoContext.CopyTo(this.contextCopy);

				Drawing.TextStyle.Tab[] temp = this.host.textLayout.Style.GetTabs ();
				this.host.textLayout.Style.SetTabs(this.tabs);
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

			private TextNavigator				host;
			private UndoType					type;
			private string						textCopy;
			private TextLayoutContext			contextCopy;
			private Drawing.TextStyle.Tab[]		tabs;
		}

		#endregion

		#region CursorInfo Class

		/// <summary>
		/// The <c>CursorInfo</c> class is used to store information about the
		/// initial cursor position outside of the <see cref="TextNavigator"/>
		/// instance body (this is just temporary information, discarded as
		/// soon as event processing is done).
		/// </summary>
		private class CursorInfo
		{
			public CursorInfo()
			{
			}

			public int CursorFrom
			{
				get;
				set;
			}

			public int CursorTo
			{
				get;
				set;
			}

			public bool CursorAfter
			{
				get;
				set;
			}

			public int TextLength
			{
				get;
				set;
			}
		}

		#endregion


		private void InitialMemorize()
		{
			//	Mémorise l'état avant une opération quelconque sur le texte.

			this.initialInfo = new CursorInfo ()
			{
				CursorFrom  = this.context.CursorFrom,
				CursorTo    = this.context.CursorTo,
				CursorAfter = this.context.CursorAfter,
				TextLength  = this.textLayout.Text.Length
			};
		}

		private void OnTextInserted(bool always)
		{
			//	Génère un événement pour dire que des caractères ont été insérés.
			if (!always && this.initialInfo.TextLength == this.textLayout.Text.Length)
			{
				return;
			}

			if (this.TextInserted != null)  // qq'un écoute ?
			{
				this.textLayout.NotifyTextChangeEvent (this.TextInserted, this);
			}
		}

		private void OnTextDeleted(bool always)
		{
			//	Génère un événement pour dire que des caractères ont été détruits.
			if (!always && this.initialInfo.TextLength == this.textLayout.Text.Length)
			{
				return;
			}

			if (this.TextDeleted != null)  // qq'un écoute ?
			{
				this.textLayout.NotifyTextChangeEvent (this.TextDeleted, this);
			}
		}

		private void OnCursorChanged(bool always)
		{
			//	Génère un événement pour dire que le curseur a bougé.
			if (!always                                       &&
				 this.initialInfo.CursorFrom  == this.context.CursorFrom  &&
				 this.initialInfo.CursorTo    == this.context.CursorTo    &&
				 this.initialInfo.CursorAfter == this.context.CursorAfter)
			{
				return;
			}

			if (this.CursorChanged != null)  // qq'un écoute ?
			{
				this.textLayout.NotifyTextChangeEvent (this.CursorChanged, this);
			}
		}

		private void OnCursorScrolled()
		{
			//	Génère un événement pour dire que le curseur a scrollé.
			if (this.CursorScrolled != null)  // qq'un écoute ?
			{
				this.textLayout.NotifyTextChangeEvent (this.CursorScrolled, this);
			}
		}

		private void OnStyleChanged()
		{
			//	Génère un événement pour dire que le style a changé.
			if (this.StyleChanged != null)  // qq'un écoute ?
			{
				this.textLayout.NotifyTextChangeEvent (this.StyleChanged, this);
			}
		}

		private void OnAboutToChange()
		{
			if (this.AboutToChange != null)
			{
				this.AboutToChange (this);
			}
		}


		public event EventHandler				AboutToChange;
		public event EventHandler				TextInserted;
		public event EventHandler				TextDeleted;
		public event EventHandler				CursorChanged;
		public event EventHandler				CursorScrolled;
		public event EventHandler				StyleChanged;


		private readonly AbstractTextField		textField;
		private readonly TextLayout				textLayout;
		private readonly TextLayoutContext		context;
		private Support.OpletQueue				opletQueue;
		private bool							isReadOnly;
		private bool							isNumeric;
		private bool							allowTabInsertion;
		private CursorInfo						initialInfo;
		
		private bool							mouseDown;
		private bool							mouseDrag;
		private bool							mouseMoved;
	}
}
