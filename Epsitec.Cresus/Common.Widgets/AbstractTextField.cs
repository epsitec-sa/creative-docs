namespace Epsitec.Common.Widgets
{
	using BundleAttribute  = Epsitec.Common.Support.BundleAttribute;
	
	public enum TextFieldStyle
	{
		Flat,							// pas de cadre, ni de relief, fond blanc
		Normal,							// ligne �ditable normale
		Multi,							// ligne �ditable Multi
		Combo,							// ligne �ditable Combo
		UpDown,							// ligne �ditable UpDown
		Simple,							// cadre tout simple
		Static,							// comme Flat mais fond transparent, s�lectionnable, pas �ditable...
	}
	
	
	/// <summary>
	/// La classe TextField impl�mente la ligne �ditable, tout en permettant
	/// aussi de r�aliser l'�quivalent de la ComboBox Windows.
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractTextField : Widget
	{
		static AbstractTextField()
		{
			TextField.flashTimer.TimeElapsed += new EventHandler(TextField.HandleFlashTimer);
		}
		
		
		public AbstractTextField()
		{
			this.DockMargins = new Drawing.Margins(AbstractTextField.FrameMargin, AbstractTextField.FrameMargin, AbstractTextField.FrameMargin, AbstractTextField.FrameMargin);

			this.InternalState |= InternalState.AutoFocus;
			this.InternalState |= InternalState.AutoEngage;
			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.Engageable;
			this.textStyle = TextFieldStyle.Normal;

			this.ResetCursor();
			this.MouseCursor = MouseCursor.AsIBeam;

			this.CreateTextLayout();
		}
		
		public AbstractTextField(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( TextField.blinking == this )
				{
					TextField.blinking = null;
				}
			}
			
			base.Dispose(disposing);
		}

		
		// Mode pour la ligne �ditable.
		[ Bundle ("ro") ] public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}

			set
			{
				if ( this.isReadOnly != value )
				{
					this.isReadOnly = value;
					this.MouseCursor = this.isReadOnly ? MouseCursor.AsArrow : MouseCursor.AsIBeam;
				}
			}
		}

		// Retourne la hauteur standard d'une ligne �ditable.
		public override double DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight + 2*AbstractTextField.Margin;
			}
		}

		public override Drawing.Point BaseLine
		{
			get
			{
				if (this.TextLayout != null)
				{
					Drawing.Point pos   = this.TextLayout.GetLineOrigin (0);
					Drawing.Point shift = this.InnerTextOffset;
					
					double y_from_top = this.TextLayout.LayoutSize.Height - pos.Y;
					double y_from_bot = this.realSize.Height - y_from_top + shift.Y;
					
					return new Drawing.Point (shift.X, y_from_bot);
				}
				
				return base.BaseLine;
			}
		}
		
		public virtual Drawing.Point InnerTextOffset
		{
			get
			{
				Drawing.Point     pos  = new Drawing.Point(AbstractTextField.Margin, AbstractTextField.Margin);
				Drawing.Rectangle rect = this.InnerBounds;
				
				if ( rect.Height < 18 )	//	TODO: remplacer cette constante par qqch de plus ad�quat...
				{
					pos.Y += 18-rect.Height;  // remonte le texte si la hauteur est tr�s petite
				}
				
				return pos;
			}
		}
		
		public override Drawing.Rectangle InnerBounds
		{
			get
			{
				Drawing.Rectangle rect = this.Client.Bounds;
				
				rect.Deflate(this.margins);
				rect.Deflate(AbstractTextField.FrameMargin, AbstractTextField.FrameMargin);
				
				return rect;
			}
		}
		
		// Texte �dit�.
		protected override void ModifyTextLayout(string text)
		{
			if ( text.Length > this.maxChar )
			{
				text = text.Substring(0, this.maxChar);
			}
			
			base.ModifyTextLayout (text);
		}
		
		protected override void DisposeTextLayout()
		{
			// Ne fait rien, on veut s'assurer que le TextLayout associ� avec le
			// TextField n'est jamais d�truit du vivant du TextField.
			this.TextLayout.Text = "";
		}
		

		// Nombre max de caract�res dans la ligne �dit�e.
		public int MaxChar
		{
			get { return this.maxChar; }
			set { this.maxChar = value; }
		}

		public virtual Drawing.Margins Margins
		{
			get { return this.margins; }
			set { this.margins = value; }
		}
		
		public virtual double LeftMargin
		{
			get { return this.margins.Left; }
			set { this.margins.Left = value; }
		}
		
		public virtual double RightMargin
		{
			get { return this.margins.Right; }
			set { this.margins.Right = value; }
		}
		
		public virtual double BottomMargin
		{
			get { return this.margins.Bottom; }
			set { this.margins.Bottom = value; }
		}
		
		public virtual double TopMargin
		{
			get { return this.margins.Top; }
			set { this.margins.Top = value; }
		}
		
		
		public TextFieldStyle TextFieldStyle
		{
			get
			{
				return this.textStyle;
			}

			set
			{
				if ( this.textStyle != value )
				{
					this.textStyle = value;
					this.Invalidate();
				}
			}
		}

		// Position du curseur d'�dition.
		public int Cursor
		{
			get
			{
				return this.cursorTo;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.Text.Length);

				if ( value != this.cursorFrom && value != this.cursorTo )
				{
					this.cursorFrom = value;
					this.cursorTo   = value;
					this.OnCursorChanged();
				}
			}
		}
		
		public int CursorFrom
		{
			get
			{
				return this.cursorFrom;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.Text.Length);

				if ( value != this.cursorFrom )
				{
					this.cursorFrom = value;
					this.OnCursorChanged();
				}
			}
		}
		
		public int CursorTo
		{
			get
			{
				return this.cursorTo;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.Text.Length);

				if ( value != this.cursorTo )
				{
					this.cursorTo = value;
					this.CursorScroll();
					this.Invalidate();
				}
			}
		}

		// S�lectione tous les caract�res.
		public void SelectAll()
		{
			this.cursorFrom = 0;
			this.cursorTo   = this.Text.Length;
			this.OnCursorChanged();
		}

		protected override void UpdateTextLayout()
		{
			if ( this.TextLayout != null )
			{
				double dx = this.Client.Width - AbstractTextField.Margin*2 - this.margins.Width;
				double dy = this.Client.Height - AbstractTextField.Margin*2 - this.margins.Height;
				this.realSize = new Drawing.Size(dx, dy);
				this.TextLayout.Alignment = this.Alignment;
				this.TextLayout.LayoutSize = new Drawing.Size(AbstractTextField.Infinity, dy);

				if ( this.TextLayout.Text != null )
				{
					this.CursorScroll();
				}
			}
		}

		// Retourne l'alignement par d�faut d'un bouton.
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.TopLeft;
			}
		}


		// G�re le temps �coul� pour faire clignoter un curseur.
		protected static void HandleFlashTimer(object source)
		{
			TextField.showCursor = !TextField.showCursor;
			
			if ( TextField.blinking != null )
			{
				TextField.blinking.FlashCursor();
			}
		}
		
		
		// Fait clignoter le curseur.
		protected void FlashCursor()
		{
			this.Invalidate();
		}

		// Allume le curseur au prochain affichage.
		protected void ResetCursor()
		{
			if ( this.IsFocused && this.Window.IsFocused )
			{
				double delay = SystemInformation.CursorBlinkDelay;
				TextField.flashTimer.Delay = delay;
				TextField.flashTimer.AutoRepeat = delay;
				TextField.flashTimer.Start();  // restart du timer
				TextField.showCursor = true;  // avec le curseur visible
			}
		}



		// Gestion d'un �v�nement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			pos.X -= AbstractTextField.Margin;
			pos.Y -= AbstractTextField.Margin;
			pos += this.scrollOffset;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.mouseDown = true;
					this.BeginPress(pos);
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						this.MovePress(pos);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.EndPress(pos);
						this.mouseDown = false;
					}
					break;

				case MessageType.KeyDown:
					this.ProcessKeyDown(message.KeyCode, message.IsShiftPressed, message.IsCtrlPressed);
					break;

				case MessageType.KeyPress:
					this.ProcessKeyPress(message.KeyChar);
					break;
			}
			
			message.Consumer = this;
		}

		// Appel� lorsque le bouton de la souris est press�.
		protected void BeginPress(Drawing.Point pos)
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			int detect = this.TextLayout.DetectIndex(pos);
			if ( detect != -1 )
			{
				this.cursorFrom = detect;
				this.cursorTo   = detect;
				this.Invalidate();
			}
		}

		// Appel� lorsque la souris est d�plac�e, bouton press�.
		protected void MovePress(Drawing.Point pos)
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			int detect = this.TextLayout.DetectIndex(pos);
			if ( detect != -1 )
			{
				this.cursorTo = detect;
				this.Invalidate();
			}
		}

		// Appel� lorsque le bouton de la souris est rel�ch�.
		protected void EndPress(Drawing.Point pos)
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			int detect = this.TextLayout.DetectIndex(pos);
			if ( detect != -1 )
			{
				this.cursorTo = detect;
				this.Invalidate();
			}
		}

		// Gestion d'une touche press�e avec KeyDown dans le texte.
		protected virtual void ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isCtrlPressed)
		{
			switch ( key )
			{
				case KeyCode.Back:
					this.DeleteCharacter(-1);
					break;

				case KeyCode.Delete:
					this.DeleteCharacter(1);
					break;

				case KeyCode.Escape:
					break;

				case KeyCode.Home:
					this.MoveCursor(-1000000, isShiftPressed, false);  // recule beaucoup
					break;

				case KeyCode.End:
					this.MoveCursor(1000000, isShiftPressed, false);  // avance beaucoup
					break;

				case KeyCode.PageUp:
					this.MoveCursor(-1000000, isShiftPressed, false);  // recule beaucoup
					break;

				case KeyCode.PageDown:
					this.MoveCursor(1000000, isShiftPressed, false);  // avance beaucoup
					break;

				case KeyCode.ArrowLeft:
					this.MoveCursor(-1, isShiftPressed, isCtrlPressed);
					break;

				case KeyCode.ArrowRight:
					this.MoveCursor(1, isShiftPressed, isCtrlPressed);
					break;

				case KeyCode.ArrowUp:
					this.MoveCursor(-1, isShiftPressed, isCtrlPressed);
					break;

				case KeyCode.ArrowDown:
					this.MoveCursor(1, isShiftPressed, isCtrlPressed);
					break;
			}
		}

		// Gestion d'une touche press�e avec KeyPress dans le texte.
		protected void ProcessKeyPress(int key)
		{
			if ( key >= 32 )  // TODO: � v�rifier ...
			{
				this.InsertCharacter((char)key);
			}
		}

		// Ins�re un caract�re.
		protected bool InsertCharacter(char character)
		{
			return this.InsertString(TextLayout.ConvertToTaggedText (character));
		}

		// Ins�re une cha�ne correspondant � un caract�re ou un tag (jamais plus).
		protected bool InsertString(string ins)
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			if ( this.isReadOnly )  return false;
			
			// L'appel de DeleteSelection g�n�re un �v�nement TextChanged avec un texte dans
			// un �tat interm�diaire. Tant pis...
			
			this.DeleteSelection();

			if ( this.Text.Length+ins.Length > this.maxChar )  return false;

			int cursor = this.TextLayout.FindOffsetFromIndex(this.cursorTo);
			string text = this.Text;
			text = text.Insert(cursor, ins);
			this.cursorTo ++;
			this.cursorFrom = this.cursorTo;
			this.Text = text;
			this.OnTextInserted();
			this.OnCursorChanged();
			return true;
		}

		// Supprime le caract�re � gauche ou � droite du curseur.
		protected bool DeleteCharacter(int dir)
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			if ( this.isReadOnly )  return false;
			if ( this.DeleteSelection() )  return false;

			int cursor = this.TextLayout.FindOffsetFromIndex(this.cursorTo);

			if ( dir < 0 )  // � gauche du curseur ?
			{
				if ( cursor <= 0 )  return false;

				string text = this.Text;
				int len = this.TextLayout.RecedeTag(cursor);
				text = text.Remove(cursor-len, len);
				this.cursorTo --;
				this.cursorFrom = this.cursorTo;
				this.Text = text;
				this.OnTextDeleted();
				this.OnCursorChanged();
			}
			else	// � droite du curseur ?
			{
				if ( cursor >= this.Text.Length )  return false;

				string text = this.Text;
				int len = this.TextLayout.AdvanceTag(cursor);
				text = text.Remove(cursor, len);
				this.Text = text;
				this.OnTextDeleted();
			}

			return true;
		}
		
		public bool DeleteSelection()
		{
			// Supprime les caract�res s�lectionn�s dans le texte.
			
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			if ( this.isReadOnly )  return false;
			
			int cursorFrom = this.TextLayout.FindOffsetFromIndex(this.cursorFrom);
			int cursorTo   = this.TextLayout.FindOffsetFromIndex(this.cursorTo);
			
			int from = System.Math.Min(cursorFrom, cursorTo);
			int to   = System.Math.Max(cursorFrom, cursorTo);
			
			if ( from == to )  return false;
			
			string text = this.Text;
			text = text.Remove(from, to-from);
			from = this.TextLayout.FindIndexFromOffset(from);
			this.cursorTo   = from;
			this.cursorFrom = from;
			this.Text = text;
			this.OnTextDeleted();
			this.OnCursorChanged();
			
			return true;
		}

		// Indique si un caract�re est un s�parateur pour les d�placements
		// avec Ctrl+fl�che.
		protected bool IsWordSeparator(char character)
		{
			character = System.Char.ToUpper(character);
			if ( character == '_' )  return false;
			if ( character >= 'A' && character <= 'Z' )  return false;
			if ( character >= '0' && character <= '9' )  return false;
			return true;
		}

		// D�place le curseur au d�but ou � la fin d'une ligne.
		protected bool MoveExtremity(int move, bool isShiftPressed, bool isCtrlPressed)
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			if ( isCtrlPressed )  // d�but/fin du texte ?
			{
				return this.MoveCursor(move*1000000, isShiftPressed, false);
			}

			double posx;
			if ( move < 0 )  posx = 0;
			else             posx = this.TextLayout.LayoutSize.Width;
			int cursor = this.TextLayout.DetectIndex(posx, this.cursorLine);
			if ( cursor == -1 )  return false;
			this.cursorTo = cursor;
			
			if ( !isShiftPressed )
			{
				this.cursorFrom = cursor;
			}
			
			this.OnCursorChanged();
			return true;
		}

		// D�place le curseur par lignes.
		protected bool MoveLine(int move, bool isShiftPressed, bool isCtrlPressed)
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			int cursor = this.TextLayout.DetectIndex(this.cursorPosX, this.cursorLine+move);
			if ( cursor == -1 )  return false;
			this.cursorTo = cursor;
			
			if ( !isShiftPressed )
			{
				this.cursorFrom = cursor;
			}
			
			this.OnCursorChanged();
			return true;
		}

		// D�place le curseur.
		protected bool MoveCursor(int move, bool isShiftPressed, bool isCtrlPressed)
		{
			int cursor = this.cursorTo;
			string simple = TextLayout.ConvertToSimpleText(this.Text);

			if ( isCtrlPressed )  // d�placement par mots ?
			{
				if ( move < 0 )
				{
					while ( cursor > 0 )
					{
						if ( !this.IsWordSeparator(simple[cursor-1]) )  break;
						cursor --;
					}
					while ( cursor > 0 )
					{
						if ( this.IsWordSeparator(simple[cursor-1]) )  break;
						cursor --;
					}
				}
				else
				{
					while ( cursor < simple.Length )
					{
						if ( this.IsWordSeparator(simple[cursor]) )  break;
						cursor ++;
					}
					while ( cursor < simple.Length )
					{
						if ( !this.IsWordSeparator(simple[cursor]) )  break;
						cursor ++;
					}
				}
			}
			else	// d�placement par caract�res ?
			{
				cursor += move;
			}

			cursor = System.Math.Max(cursor, 0);
			cursor = System.Math.Min(cursor, simple.Length);
			if ( cursor == this.cursorTo && cursor == this.cursorFrom )  return false;
			this.cursorTo = cursor;
			if ( !isShiftPressed )
			{
				this.cursorFrom = cursor;
			}
			
			this.OnCursorChanged();
			return true;
		}


		protected override void OnFocused()
		{
			base.OnFocused();
			TextField.blinking = this;
			this.ResetCursor();
		}

		protected override void OnDefocused()
		{
			TextField.blinking = null;
			base.OnDefocused();
		}

		protected override void OnTextChanged()
		{
			// G�n�re un �v�nement pour dire que le texte a chang� (tout changement).
			
			this.ResetCursor();
			this.CursorScroll ();
			this.Invalidate ();
			
			base.OnTextChanged ();
		}

		protected virtual void OnTextInserted()
		{
			// G�n�re un �v�nement pour dire que le texte a chang� (ajout).
			
			if ( this.TextInserted != null )  // qq'un �coute ?
			{
				this.TextInserted(this);
			}
		}

		protected virtual void OnTextDeleted()
		{
			// G�n�re un �v�nement pour dire que le texte a chang� (suppression).
			
			if ( this.TextDeleted != null )  // qq'un �coute ?
			{
				this.TextDeleted(this);
			}
		}
		
		protected virtual void OnCursorChanged()
		{
			// Ne g�n�re rien pour l'instant...
			
			this.CursorScroll();
			this.ResetCursor();
			this.Invalidate();
		}


		// Calcule le scrolling pour que le curseur soit visible.
		protected void CursorScroll()
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			if ( this.mouseDown )  return;

			this.scrollOffset = new Drawing.Point(0, 0);

			Drawing.Rectangle cursor = this.TextLayout.FindTextCursor(this.cursorTo, out this.cursorLine);
			this.cursorPosX = (cursor.Left+cursor.Right)/2;

			Drawing.Point end = this.TextLayout.FindTextEnd();
			
			this.CursorScrollTextEnd(end, cursor);
		}
		
		protected virtual void CursorScrollTextEnd(Drawing.Point end, Drawing.Rectangle cursor)
		{
			double offset = cursor.Right;
			offset += this.realSize.Width/2;
			offset  = System.Math.Min (offset, end.X);
			offset -= this.realSize.Width;
			offset  = System.Math.Max (offset, 0);
			this.scrollOffset.X = offset;
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			// Dessine le texte en cours d'�dition.
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			WidgetState   state = this.PaintState;
			Drawing.Point pos   = this.InnerTextOffset - this.scrollOffset;
			
			adorner.PaintTextFieldBackground(graphics, this.Client.Bounds, state, this.textStyle, this.isReadOnly);
			
			Drawing.Rectangle rInside = this.InnerBounds;
			Drawing.Rectangle rSaveClip = graphics.SaveClippingRectangle();
			Drawing.Rectangle rClip = rInside;
			
			rClip = this.MapClientToRoot(rClip);
			graphics.SetClippingRectangle(rClip);
			
			if ( this.IsFocused )
			{
				bool visibleCursor = false;
				
				int from = System.Math.Min(this.cursorFrom, this.cursorTo);
				int to   = System.Math.Max(this.cursorFrom, this.cursorTo);
				
				if ( this.isCombo && this.isReadOnly )
				{
					Drawing.Rectangle[] rects = new Drawing.Rectangle[1];
					rects[0] = rInside;
					rects[0].Deflate(1, 1);
					adorner.PaintTextSelectionBackground(graphics, rects);
					adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, (state&~WidgetState.Focused)|WidgetState.Selected, PaintTextStyle.TextField, this.BackColor);
					adorner.PaintFocusBox(graphics, rects[0]);
				}
				else if ( from == to )
				{
					adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, state&~WidgetState.Focused, PaintTextStyle.TextField, this.BackColor);
					visibleCursor = TextField.showCursor && this.Window.IsFocused;
				}
				else
				{
					adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, state&~(WidgetState.Focused|WidgetState.Selected), PaintTextStyle.TextField, this.BackColor);

					Drawing.Rectangle[] rects = this.TextLayout.FindTextRange(pos, from, to);
					for ( int i=0 ; i<rects.Length ; i++ )
					{
						graphics.Align(ref rects[i]);
					}
					adorner.PaintTextSelectionBackground(graphics, rects);
					
					for ( int i=0 ; i<rects.Length ; i++ )
					{
						rects[i] = this.MapClientToRoot(rects[i]);
					}
					graphics.SetClippingRectangles(rects);

					adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, (state&~WidgetState.Focused)|WidgetState.Selected, PaintTextStyle.TextField, this.BackColor);
				}


				// Dessine le curseur.
				if ( !this.isReadOnly )
				{
					Drawing.Rectangle rCursor = this.TextLayout.FindTextCursor(this.cursorTo, out this.cursorLine);
					this.cursorPosX = (rCursor.Left+rCursor.Right)/2;
					double x = rCursor.Left;
					double y = rCursor.Bottom;
					graphics.Align(ref x, ref y);
					rCursor.Left = x;
					rCursor.Right = x+1;
					rCursor.Offset(pos);
					adorner.PaintTextCursor(graphics, rCursor, visibleCursor);
				}
			}
			else
			{
				adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, state&~WidgetState.Focused, PaintTextStyle.TextField, this.BackColor);
			}

			graphics.RestoreClippingRectangle(rSaveClip);
		}


		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			rect.Inflate(adorner.GeometryTextFieldShapeBounds);
			return rect;
		}

		
		public event EventHandler TextInserted;
		public event EventHandler TextDeleted;
		
		
		internal static readonly double			Margin = 4;
		internal static readonly double			FrameMargin = 2;
		internal static readonly double			Infinity = 1000000;
		
		protected bool							isReadOnly = false;
		protected bool							isCombo = false;
		protected Drawing.Margins				margins = new Drawing.Margins();
		protected Drawing.Size					realSize;
		protected Drawing.Point					scrollOffset = new Drawing.Point(0, 0);
		protected TextFieldStyle				textStyle;
		protected int							cursorFrom = 0;
		protected int							cursorTo = 0;
		protected int							cursorLine;
		protected double						cursorPosX;
		protected int							maxChar = 1000;
		protected bool							mouseDown = false;
		
		protected static Timer					flashTimer = new Timer();
		protected static bool					showCursor = true;
		
		protected static AbstractTextField		blinking;
	}
}
