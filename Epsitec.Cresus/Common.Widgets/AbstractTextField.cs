namespace Epsitec.Common.Widgets
{
	using BundleAttribute  = Epsitec.Common.Support.BundleAttribute;
	
	public enum TextFieldStyle
	{
		Flat,							// pas de cadre, ni de relief, fond blanc
		Normal,							// ligne éditable normale
		Multi,							// ligne éditable Multi
		Combo,							// ligne éditable Combo
		UpDown,							// ligne éditable UpDown
		Simple,							// cadre tout simple
		Static,							// comme Flat mais fond transparent, sélectionnable, pas éditable...
	}
	
	
	/// <summary>
	/// La classe TextField implémente la ligne éditable, tout en permettant
	/// aussi de réaliser l'équivalent de la ComboBox Windows.
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractTextField : Widget
	{
		static AbstractTextField()
		{
			TextField.flashTimer.TimeElapsed += new Support.EventHandler(TextField.HandleFlashTimer);
		}
		
		
		public AbstractTextField()
		{
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

		
		// Mode pour la ligne éditable.
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

		// Retourne la hauteur standard d'une ligne éditable.
		public override double DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight + 2*(AbstractTextField.TextMargin+AbstractTextField.FrameMargin);
			}
		}

		public override Drawing.Point BaseLine
		{
			get
			{
				if (this.TextLayout != null)
				{
					Drawing.Point pos   = this.TextLayout.GetLineOrigin (0);
					Drawing.Point shift = this.InnerTextBounds.Location;
					
					double y_from_top = this.TextLayout.LayoutSize.Height - pos.Y;
					double y_from_bot = this.realSize.Height - y_from_top + shift.Y + 1;
					
					return new Drawing.Point(shift.X, y_from_bot);
				}
				
				return base.BaseLine;
			}
		}
		
		public override Drawing.Rectangle InnerBounds
		{
			get
			{
				Drawing.Rectangle rect = this.Client.Bounds;
				
				rect.Deflate(this.margins);
				
				if ( this.textStyle != TextFieldStyle.Flat )
				{
					rect.Deflate(AbstractTextField.FrameMargin, AbstractTextField.FrameMargin);
				}
				
				return rect;
			}
		}
		
		public virtual Drawing.Rectangle InnerTextBounds
		{
			get
			{
				Drawing.Rectangle rect = this.InnerBounds;
				rect.Deflate(AbstractTextField.TextMargin, AbstractTextField.TextMargin);
				
				return rect;
			}
		}
		
		// Texte édité.
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
			// Ne fait rien, on veut s'assurer que le TextLayout associé avec le
			// TextField n'est jamais détruit du vivant du TextField.
			this.TextLayout.Text = "";
		}
		
		
		internal override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			if (mode != TabNavigationMode.Passive)
			{
				this.SelectAll ();
			}
			
			return base.AboutToGetFocus (dir, mode, out focus);
		}


		// Nombre max de caractères dans la ligne éditée.
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

		// Position du curseur d'édition.
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

		// Sélectione tous les caractéres.
		public void SelectAll()
		{
			this.SelectAll(false);
		}
		
		public virtual void SelectAll(bool silent)
		{
			this.cursorFrom = 0;
			this.cursorTo   = this.Text.Length;
			this.OnCursorChanged(silent);
		}

		protected override void UpdateTextLayout()
		{
			if ( this.TextLayout != null )
			{
				this.realSize = this.InnerTextBounds.Size;
				this.TextLayout.Alignment  = this.Alignment;
				this.TextLayout.LayoutSize = this.GetTextLayoutSize();
				
				if ( this.TextLayout.Text != null )
				{
					this.CursorScroll();
				}
			}
		}
		
		protected virtual Drawing.Size GetTextLayoutSize()
		{
			return new Drawing.Size(AbstractTextField.Infinity, this.realSize.Height);
		}

		// Retourne l'alignement par défaut d'un bouton.
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.TopLeft;
			}
		}


		// Gère le temps écoulé pour faire clignoter un curseur.
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



		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			pos.X -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos.Y -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos += this.scrollOffset;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.mouseDown = true;
					this.BeginPress(pos);
					message.Consumer = this;
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						this.MovePress(pos);
						message.Consumer = this;
					}
					break;
				
				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.EndPress(pos);
						this.mouseDown = false;
						message.Consumer = this;
					}
					break;
				
				case MessageType.KeyDown:
					if (this.ProcessKeyDown(message.KeyCode, message.IsShiftPressed, message.IsCtrlPressed))
					{
						message.Consumer = this;
					}
					break;
				
				case MessageType.KeyPress:
					if (this.ProcessKeyPress(message.KeyChar))
					{
						message.Consumer = this;
					}
					break;
			}
		}

		// Appelé lorsque le bouton de la souris est pressé.
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

		// Appelé lorsque la souris est déplacée, bouton pressé.
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

		// Appelé lorsque le bouton de la souris est relâché.
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

		// Gestion d'une touche pressée avec KeyDown dans le texte.
		protected virtual bool ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isCtrlPressed)
		{
			switch (key)
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
				
				default:
					return false;
			}
			
			return true;
		}

		// Gestion d'une touche pressée avec KeyPress dans le texte.
		protected virtual bool ProcessKeyPress(int key)
		{
			if (key >= 32)  // TODO: à vérifier ...
			{
				this.InsertCharacter ((char)key);
				return true;
			}
			
			return false;
		}

		// Insère un caractère.
		protected bool InsertCharacter(char character)
		{
			return this.InsertString(TextLayout.ConvertToTaggedText (character));
		}

		// Insère une chaîne correspondant à un caractère ou un tag (jamais plus).
		protected bool InsertString(string ins)
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			if ( this.isReadOnly )  return false;
			
			int cursorFrom = this.TextLayout.FindOffsetFromIndex(this.cursorFrom);
			int cursorTo   = this.TextLayout.FindOffsetFromIndex(this.cursorTo);
			
			int from = System.Math.Min(cursorFrom, cursorTo);
			int to   = System.Math.Max(cursorFrom, cursorTo);
			
			string text = this.Text;
			
			if ( from < to )
			{
				text = text.Remove(from, to-from);
				from = this.TextLayout.FindIndexFromOffset(from);
				this.cursorTo   = from;
				this.cursorFrom = from;
			}
			
			if ( this.Text.Length+ins.Length > this.maxChar )
			{
				this.Text = text;
				this.OnTextDeleted ();
				this.OnCursorChanged ();
				return false;
			}
			
			int cursor = this.TextLayout.FindOffsetFromIndex(this.cursorTo);
			text = text.Insert(cursor, ins);
			this.cursorTo ++;
			this.cursorFrom = this.cursorTo;
			this.Text = text;
			this.OnTextInserted();
			this.OnCursorChanged();
			return true;
		}

		// Supprime le caractère à gauche ou à droite du curseur.
		protected bool DeleteCharacter(int dir)
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			if ( this.isReadOnly )  return false;
			if ( this.DeleteSelection() )  return false;

			int cursor = this.TextLayout.FindOffsetFromIndex(this.cursorTo);

			if ( dir < 0 )  // à gauche du curseur ?
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
			else	// à droite du curseur ?
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
			// Supprime les caractères sélectionnés dans le texte.
			
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

		// Indique si un caractère est un séparateur pour les déplacements
		// avec Ctrl+flèche.
		protected bool IsWordSeparator(char character)
		{
			character = System.Char.ToUpper(character);
			if ( character == '_' )  return false;
			if ( character >= 'A' && character <= 'Z' )  return false;
			if ( character >= '0' && character <= '9' )  return false;
			return true;
		}

		// Déplace le curseur au début ou à la fin d'une ligne.
		protected bool MoveExtremity(int move, bool isShiftPressed, bool isCtrlPressed)
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			if ( isCtrlPressed )  // début/fin du texte ?
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

		// Déplace le curseur par lignes.
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

		// Déplace le curseur.
		protected bool MoveCursor(int move, bool isShiftPressed, bool isCtrlPressed)
		{
			int cursor = this.cursorTo;
			string simple = TextLayout.ConvertToSimpleText(this.Text);

			if ( isCtrlPressed )  // déplacement par mots ?
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
			else	// déplacement par caractères ?
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
			this.SelectAll(true);
			base.OnDefocused();
		}

		protected override void OnTextChanged()
		{
			// Génère un événement pour dire que le texte a changé (tout changement).
			
			this.ResetCursor();
			this.CursorScroll ();
			this.Invalidate ();
			
			base.OnTextChanged ();
		}

		protected virtual void OnTextInserted()
		{
			// Génère un événement pour dire que le texte a changé (ajout).
			
			if ( this.TextInserted != null )  // qq'un écoute ?
			{
				this.TextInserted(this);
			}
		}

		protected virtual void OnTextDeleted()
		{
			// Génère un événement pour dire que le texte a changé (suppression).
			
			if ( this.TextDeleted != null )  // qq'un écoute ?
			{
				this.TextDeleted(this);
			}
		}
		
		protected void OnCursorChanged()
		{
			this.OnCursorChanged (false);
		}
		
		protected virtual void OnCursorChanged(bool silent)
		{
			// Ne génère rien pour l'instant...
			
			if (silent == false)
			{
				this.CursorScroll();
			}
			
			this.ResetCursor();
			this.Invalidate();
		}


		// Calcule le scrolling pour que le curseur soit visible.
		protected void CursorScroll()
		{
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			if ( this.mouseDown )  return;

			this.scrollOffset = new Drawing.Point();

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
			//	Dessine le texte en cours d'édition :
			
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			WidgetState       state     = this.PaintState;
			Drawing.Point     pos       = this.InnerTextBounds.Location - this.scrollOffset + new Drawing.Point(0, 1);
			Drawing.Rectangle rText     = this.InnerTextBounds;
			Drawing.Rectangle rInside   = this.InnerBounds;
			Drawing.Rectangle rSaveClip = graphics.SaveClippingRectangle();
			Drawing.Rectangle rClip     = rInside;
			Drawing.Rectangle rFill     = this.Client.Bounds;
			
			if ( this.textStyle == TextFieldStyle.Flat )
			{
				rFill.Deflate(1, 1);
			}
			
			adorner.PaintTextFieldBackground(graphics, rFill, state, this.textStyle, this.isReadOnly);
			
//			graphics.AddFilledRectangle (rText);
//			graphics.RenderSolid (Drawing.Color.FromARGB (0.6, 1, 0, 0));
			
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
					//	Un morceau de texte a été sélectionné. Peint en plusieurs étapes :
					//
					//	- Peint tout le texte normalement
					//	- Peint les rectangles de sélection
					//	- Peint tout le texte en mode sélectionné, avec clipping
					
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
				
				if ( !this.isReadOnly )
				{
					//	Dessine le curseur :
					
					Drawing.Rectangle cursor = this.TextLayout.FindTextCursor(this.cursorTo, out this.cursorLine);
					this.cursorPosX = (cursor.Left + cursor.Right)/2;
					double x = cursor.Left;
					double y = cursor.Bottom;
					graphics.Align(ref x, ref y);
					cursor.Left = x;
					cursor.Right = x+1;
					cursor.Offset(pos);
					adorner.PaintTextCursor(graphics, cursor, visibleCursor);
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
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Inflate(adorner.GeometryTextFieldShapeBounds);
			return rect;
		}

		
		public event Support.EventHandler TextInserted;
		public event Support.EventHandler TextDeleted;
		
		
		internal static readonly double			TextMargin = 2;
		internal static readonly double			FrameMargin = 2;
		internal static readonly double			Infinity = 1000000;
		
		protected bool							isReadOnly = false;
		protected bool							isCombo = false;
		protected Drawing.Margins				margins = new Drawing.Margins();
		protected Drawing.Size					realSize;
		protected Drawing.Point					scrollOffset = new Drawing.Point();
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
