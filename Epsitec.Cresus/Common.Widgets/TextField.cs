namespace Epsitec.Common.Widgets
{
	public enum TextFieldStyle
	{
		Flat,							// pas de cadre, ni de relief, fond blanc
		Normal,							// ligne éditable normale
		Simple,							// cadre tout simple
		Static,							// comme Flat mais fond transparent, sélectionnable, pas éditable...
	}
	
	public enum TextFieldType
	{
		SingleLine,						// ligne simple, scrollable horizontalement
		MultiLine,						// ligne multiple, scrollable verticalement
		UpDown,							// valeur numérique avec boutons +/-
		Combo,							// combo box
	}
	
	/// <summary>
	/// La classe TextField implémente la ligne éditable, tout en permettant
	/// aussi de réaliser l'équivalent de la ComboBox Windows.
	/// </summary>
	public class TextField : Widget
	{
		static TextField()
		{
			TextField.flashTimer.Tick += new System.EventHandler(TextField.HandleFlashTimer);
		}
		
		// Crée une ligne éditable d'un type quelconque.
		public TextField(TextFieldType type)
		{
			this.type = type;

			this.internalState |= InternalState.AutoFocus;
			this.internalState |= InternalState.AutoEngage;
			this.internalState |= InternalState.Focusable;
			this.internalState |= InternalState.Engageable;
			this.textStyle = TextFieldStyle.Normal;

			this.ResetCursor();
			this.MouseCursor = MouseCursor.AsIBeam;

			switch ( type )
			{
				case TextFieldType.SingleLine:
					break;

				case TextFieldType.MultiLine:
					this.scroller = new Scroller();
					this.scroller.SetEnabled(false);
					this.scroller.Moved += new EventHandler(this.HandleScroller);
					this.Children.Add(this.scroller);
					break;

				case TextFieldType.UpDown:
					this.arrowUp = new ArrowButton();
					this.arrowDown = new ArrowButton();
					this.arrowUp.Direction = Direction.Up;
					this.arrowDown.Direction = Direction.Down;
					this.arrowUp.ButtonStyle = ButtonStyle.Scroller;
					this.arrowDown.ButtonStyle = ButtonStyle.Scroller;
					this.arrowUp.Engaged += new EventHandler(this.HandleButton);
					this.arrowDown.Engaged += new EventHandler(this.HandleButton);
					this.arrowUp.StillEngaged += new EventHandler(this.HandleButton);
					this.arrowDown.StillEngaged += new EventHandler(this.HandleButton);
					this.arrowUp.AutoRepeatEngaged = true;
					this.arrowDown.AutoRepeatEngaged = true;
					this.Children.Add(this.arrowUp);
					this.Children.Add(this.arrowDown);
					break;

				case TextFieldType.Combo:
					this.arrowDown = new ArrowButton();
					this.arrowDown.Direction = Direction.Down;
					this.arrowDown.ButtonStyle = ButtonStyle.Scroller;
					this.arrowDown.Pressed += new MessageEventHandler(this.HandleCombo);
					this.Children.Add(this.arrowDown);
					break;
			}
			
			this.CreateTextLayout();
		}
		

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				System.Diagnostics.Debug.WriteLine("Dispose TextField " + this.Text);
				
				TextField.Blinking -= new EventHandler(this.FlashCursor);
				
				switch ( this.type )
				{
					case TextFieldType.MultiLine:
						this.scroller.Moved -= new EventHandler(this.HandleScroller);
						break;

					case TextFieldType.UpDown:
						this.arrowUp.Engaged -= new EventHandler(this.HandleButton);
						this.arrowDown.Engaged -= new EventHandler(this.HandleButton);
						this.arrowUp.StillEngaged -= new EventHandler(this.HandleButton);
						this.arrowDown.StillEngaged -= new EventHandler(this.HandleButton);
						break;

					case TextFieldType.Combo:
						this.arrowDown.Pressed -= new MessageEventHandler(this.HandleCombo);
						break;
				}
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard d'une ligne éditable.
		public override double DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight+6;
			}
		}

		// Texte édité.
		public override string Text
		{
			get
			{
				return base.Text;
			}

			set
			{
				if ( value.Length > this.maxChar )
				{
					value = value.Substring(0, this.maxChar);
				}
				
				System.Diagnostics.Debug.Assert(this.textLayout != null);
				
				if ( base.Text != value )
				{
					base.Text = value;
					this.Invalidate();
					this.CursorScroll();
				}
			}
		}
		
		protected override void DisposeTextLayout()
		{
			// Ne fait rien, on veut s'assurer que le TextLayout associé avec le
			// TextField n'est jamais détruit du vivant du TextField.
			this.textLayout.Text = "";
		}


		// Nombre max de caractères dans la ligne éditée.
		public int MaxChar
		{
			get
			{
				return this.maxChar;
			}

			set
			{
				this.maxChar = value;
			}
		}


		public virtual double LeftMargin
		{
			get
			{
				return this.leftMargin;
			}
		}
		
		public virtual double RightMargin
		{
			get
			{
				return this.rightMargin;
			}
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
					this.CursorScroll();
					this.Invalidate();
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
					this.CursorScroll();
					this.Invalidate();
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
			this.cursorFrom = 0;
			this.cursorTo = this.Text.Length;
			this.CursorScroll();
			this.Invalidate();
		}

		// Valeur numérique éditée.
		public double Value
		{
			get
			{
				string text = this.Text;
				double number = 0;
				try
				{
					number = System.Convert.ToDouble(text);
				}
				catch
				{
					number = 0;
				}
				return number;
			}

			set
			{
				string text = System.Convert.ToString(value);
				this.Text = text;
				this.cursorFrom = 0;
				this.cursorTo   = text.Length;
				this.CursorScroll();
				this.Invalidate();
			}
		}

		// Valeur numérique minimale possible.
		public double MinRange
		{
			get
			{
				return this.minRange;
			}

			set
			{
				this.minRange = value;
			}
		}
		
		// Valeur numérique maximale possible.
		public double MaxRange
		{
			get
			{
				return this.maxRange;
			}

			set
			{
				this.maxRange = value;
			}
		}
		
		// Pas pour les boutons up/down.
		public double Step
		{
			get
			{
				return this.step;
			}

			set
			{
				this.step = value;
			}
		}
		

		// Vide toute la liste de la ComboBox.
		public void ComboReset()
		{
			this.comboList.Clear();
		}

		// Ajoute un texte à la fin de la liste de la ComboBox.
		public void ComboAddText(string text)
		{
			this.comboList.Add(text);
		}

		// Donne un texte de la liste de la ComboBox.
		public string ComboGetText(int index)
		{
			if ( index < 0 || index >= this.comboList.Count )  return "";
			return (string)this.comboList[index];
		}

		
		// Met à jour la géométrie des boutons de l'ascenseur.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.type == TextFieldType.MultiLine )
			{
				Drawing.Rectangle rect = this.Bounds;
				this.rightMargin = Scroller.StandardWidth;
				double m = TextField.margin-1;
				rect.Inflate(-m, -m);

				if ( this.scroller != null )
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(m+rect.Width-this.rightMargin, m, this.rightMargin, rect.Height);
					this.scroller.Bounds = aRect;
				}
			}

			if ( this.type == TextFieldType.UpDown )
			{
#if false
				Drawing.Rectangle rect = this.Bounds;
				//this.rightMargin = System.Math.Floor(rect.Height/2+1);
				this.rightMargin = System.Math.Floor(rect.Height*0.6);
				double m = TextField.margin-1;
				rect.Inflate(-m, -m);

				if ( this.arrowUp != null )
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(m+rect.Width-this.rightMargin, m+rect.Height/2, this.rightMargin, rect.Height/2);
					this.arrowUp.Bounds = aRect;
				}
				if ( this.arrowDown != null )
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(m+rect.Width-this.rightMargin, m, this.rightMargin, rect.Height/2);
					this.arrowDown.Bounds = aRect;
				}
#else
				Drawing.Rectangle rect = this.Bounds;
				//this.rightMargin = System.Math.Floor(rect.Height/2+1);
				this.rightMargin = System.Math.Floor(rect.Height*0.6);

				if ( this.arrowUp != null )
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(rect.Width-this.rightMargin, rect.Height/2, this.rightMargin, rect.Height/2);
					this.arrowUp.Bounds = aRect;
				}
				if ( this.arrowDown != null )
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(rect.Width-this.rightMargin, 0, this.rightMargin, rect.Height/2);
					this.arrowDown.Bounds = aRect;
				}
#endif
			}

			if ( this.type == TextFieldType.Combo )
			{
				Drawing.Rectangle rect = this.Bounds;
				double m = TextField.margin-1;
				this.rightMargin = rect.Height-m*2;
				if ( this.rightMargin > rect.Width/2 )  this.rightMargin = rect.Width/2;
				rect.Inflate(-m, -m);

				if ( this.arrowDown != null )
				{
					Drawing.Rectangle aRect = new Drawing.Rectangle(m+rect.Width-this.rightMargin, m, this.rightMargin, rect.Height);
					this.arrowDown.Bounds = aRect;
				}
			}
		}

		protected override void UpdateLayoutSize()
		{
			if ( this.textLayout != null )
			{
				double dx = this.Client.Width - TextField.margin*2 - this.rightMargin - this.leftMargin;
				double dy = this.Client.Height - TextField.margin*2;
				this.realSize = new Drawing.Size(dx, dy);
				if ( this.type == TextFieldType.MultiLine )
				{
					dy = TextField.infinity;  // hauteur infinie
				}
				else
				{
					dx = TextField.infinity;  // largeur infinie
				}
				this.textLayout.Alignment = this.Alignment;
				this.textLayout.LayoutSize = new Drawing.Size(dx, dy);

				if ( this.textLayout.Text != null )
				{
					this.CursorScroll();
				}
			}
		}

		// Retourne l'alignement par défaut d'un bouton.
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.TopLeft;
			}
		}

#if false
		public override Drawing.Rectangle GetPaintBounds()
		{
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.client_info.width, this.client_info.height);
			if ( this.scroller != null )
			{
				rect.Bottom -= this.scroller.Height;
			}
			return rect;
		}
#endif

		// Gère le temps écoulé pour faire clignoter un curseur.
		protected static void HandleFlashTimer(object source, System.EventArgs e)
		{
			TextField.showCursor = !TextField.showCursor;
			
			if ( TextField.Blinking != null )
			{
				System.Diagnostics.Debug.WriteLine("Blinking");
				TextField.Blinking(null);
			}
		}
		
		protected override void OnFocused()
		{
			base.OnFocused();
			TextField.Blinking += new EventHandler(this.FlashCursor);
			this.ResetCursor();
		}

		protected override void OnDefocused()
		{
			TextField.Blinking -= new EventHandler(this.FlashCursor);
			base.OnDefocused();
		}

		
		// Fait clignoter le curseur.
		protected void FlashCursor(object sender)
		{
			this.Invalidate();
		}

		// Allume le curseur au prochain affichage.
		protected void ResetCursor()
		{
			if ( this.IsFocused && this.WindowFrame.Focused )
			{
				TextField.flashTimer.Interval = SystemInformation.CursorBlinkDelay;
				TextField.flashTimer.Stop();
				TextField.flashTimer.Start();  // restart du timer
				TextField.showCursor = true;  // avec le curseur visible
			}
		}


		// Gestion d'un événement lorsque l'ascenseur est déplacé.
		private void HandleScroller(object sender)
		{
			if ( this.type != TextFieldType.MultiLine )  return;

			this.scrollOffset.Y = this.scroller.Position-this.scroller.Range+TextField.infinity-this.realSize.Height;
			this.Invalidate();
		}

		// Gestion d'un événement lorsqu'un bouton est pressé.
		private void HandleButton(object sender)
		{
			ArrowButton button = sender as ArrowButton;

			string text = this.Text;
			double number;
			try
			{
				number = System.Convert.ToDouble(text);
			}
			catch
			{
				return;
			}

			if ( button == this.arrowUp )
			{
				number += this.step;
			}
			else if ( button == this.arrowDown )
			{
				number -= this.step;
			}
			number = System.Math.Max(number, this.minRange);
			number = System.Math.Min(number, this.maxRange);

			text = System.Convert.ToString(number);
			this.Text = text;
			this.cursorFrom = 0;
			this.cursorTo   = text.Length;
			this.Invalidate();
		}

		// Gestion d'un événement lorsqu'un bouton est pressé.
		private void HandleCombo(object sender, MessageEventArgs e)
		{
			this.scrollList = new ScrollList();
			this.scrollList.ScrollListStyle = ScrollListStyle.Simple;
			this.scrollList.ComboMode = true;
			Drawing.Point pos = new Drawing.Point(0, 0);
			this.scrollList.Location = pos;
			this.scrollList.Size = new Drawing.Size(this.Width, 200);

			int sel = -1;
			int i = 0;
			foreach ( string text in this.comboList )
			{
				this.scrollList.AddText(text);
				if ( text == this.Text )
				{
					sel = i;
				}
				i ++;
			}

#if true
			pos = this.MapClientToRoot(new Drawing.Point(0, 0));
			pos = this.WindowFrame.MapWindowToScreen(pos);
			ScreenInfo si = ScreenInfo.Find(pos);
			Drawing.Rectangle wa = si.WorkingArea;
			double hMax = pos.Y-wa.Bottom;
#else
			pos = this.MapClientToRoot(new Drawing.Point(0, 0));
			pos = this.WindowFrame.MapWindowToScreen(pos);
			double hMax = pos.Y-20;
#endif
			this.scrollList.AdjustToContent(ScrollListAdjust.MoveUp, 40, hMax);

			this.scrollList.Select = sel;
			this.scrollList.ShowSelect(ScrollListShow.Middle);
			this.scrollList.SelectChanged += new EventHandler(this.HandleScrollListSelectChanged);

			this.comboWindow = new WindowFrame();
			this.comboWindow.MakeFramelessWindow();
			pos = this.MapClientToRoot(new Drawing.Point(0, -this.scrollList.Height));
			pos = this.WindowFrame.MapWindowToScreen(pos);
			this.comboWindow.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.scrollList.Width, this.scrollList.Height);
			this.comboWindow.Show();
			this.comboWindow.WindowDeactivated += new System.EventHandler(this.HandleComboWindowDeactivated);
			this.comboWindow.Root.Children.Add(this.scrollList);

			this.SetFocused(false);
			this.scrollList.SetFocused(true);
		}

		// Gestion d'un événement lorsque la scroll-liste est sélectionnée.
		private void HandleScrollListSelectChanged(object sender)
		{
			int sel = this.scrollList.Select;
			if ( sel == -1 )  return;
			this.Text = this.scrollList.GetText(sel);
			this.OnTextChanged();
			this.OnTextInserted();
			this.SelectAll();
			this.SetFocused(true);

			this.scrollList.SelectChanged -= new EventHandler(this.HandleScrollListSelectChanged);
			this.comboWindow.WindowDeactivated -= new System.EventHandler(this.HandleComboWindowDeactivated);
			this.scrollList.Dispose();
			this.scrollList = null;
			this.comboWindow.Dispose();
			this.comboWindow = null;
		}

		// Appelé lorsque la fenêtre de la liste combo est désactivée.
		private void HandleComboWindowDeactivated(object sender, System.EventArgs e)
		{
			if ( this.scrollList == null )  return;
			this.scrollList.SelectChanged -= new EventHandler(this.HandleScrollListSelectChanged);
			this.comboWindow.WindowDeactivated -= new System.EventHandler(this.HandleComboWindowDeactivated);
			this.scrollList.Dispose();
			this.scrollList = null;
			this.comboWindow.Dispose();
			this.comboWindow = null;
		}


		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			pos.X -= TextField.margin;
			pos.Y -= TextField.margin;
			pos += this.scrollOffset;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.mouseDown = true;
					BeginPress(pos);
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						MovePress(pos);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						EndPress(pos);
						this.mouseDown = false;
					}
					break;

				case MessageType.KeyDown:
					//System.Diagnostics.Debug.WriteLine("KeyDown "+message.KeyChar+" "+message.KeyCode);
					ProcessKeyDown(message.KeyCode, message.IsShiftPressed, message.IsCtrlPressed);
					break;

				case MessageType.KeyPress:
					//System.Diagnostics.Debug.WriteLine("KeyPress "+message.KeyChar+" "+message.KeyCode);
					ProcessKeyPress(message.KeyChar);
					break;
			}
			
			message.Consumer = this;
		}

		// Appelé lorsque le bouton de la souris est pressé.
		protected void BeginPress(Drawing.Point pos)
		{
			System.Diagnostics.Debug.Assert(this.textLayout != null);
			
			int detect = this.textLayout.DetectIndex(pos);
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
			System.Diagnostics.Debug.Assert(this.textLayout != null);
			
			int detect = this.textLayout.DetectIndex(pos);
			if ( detect != -1 )
			{
				this.cursorTo = detect;
				this.Invalidate();
			}
		}

		// Appelé lorsque le bouton de la souris est relâché.
		protected void EndPress(Drawing.Point pos)
		{
			System.Diagnostics.Debug.Assert(this.textLayout != null);
			
			int detect = this.textLayout.DetectIndex(pos);
			if ( detect != -1 )
			{
				this.cursorTo = detect;
				this.Invalidate();
			}
		}

		// Gestion d'une touche pressée avec KeyDown dans le texte.
		protected void ProcessKeyDown(int key, bool isShiftPressed, bool isCtrlPressed)
		{
			switch ( key )
			{
				case (int)System.Windows.Forms.Keys.Enter:
					if ( this.type == TextFieldType.MultiLine )
					{
						this.InsertCharacter('\n');
					}
					break;

				case (int)System.Windows.Forms.Keys.Back:
					this.DeleteCharacter(-1);
					break;

				case (int)System.Windows.Forms.Keys.Delete:
					this.DeleteCharacter(1);
					break;

				case (int)System.Windows.Forms.Keys.Escape:
					break;

				case (int)System.Windows.Forms.Keys.Home:
					if ( this.type == TextFieldType.MultiLine )
					{
						this.MoveExtremity(-1, isShiftPressed, isCtrlPressed);
					}
					else
					{
						this.MoveCursor(-1000000, isShiftPressed, false);  // recule beaucoup
					}
					break;

				case (int)System.Windows.Forms.Keys.End:
					if ( this.type == TextFieldType.MultiLine )
					{
						this.MoveExtremity(1, isShiftPressed, isCtrlPressed);
					}
					else
					{
						this.MoveCursor(1000000, isShiftPressed, false);  // avance beaucoup
					}
					break;

				case (int)System.Windows.Forms.Keys.PageUp:
					this.MoveCursor(-1000000, isShiftPressed, false);  // recule beaucoup
					break;

				case (int)System.Windows.Forms.Keys.PageDown:
					this.MoveCursor(1000000, isShiftPressed, false);  // avance beaucoup
					break;

				case (int)System.Windows.Forms.Keys.Left:
					this.MoveCursor(-1, isShiftPressed, isCtrlPressed);
					break;

				case (int)System.Windows.Forms.Keys.Right:
					this.MoveCursor(1, isShiftPressed, isCtrlPressed);
					break;

				case (int)System.Windows.Forms.Keys.Up:
					if ( this.type == TextFieldType.Combo )
					{
						ComboExcavation(-1);
					}
					else
					{
						if ( this.type == TextFieldType.MultiLine )
						{
							this.MoveLine(-1, isShiftPressed, isCtrlPressed);
						}
						else
						{
							this.MoveCursor(-1, isShiftPressed, isCtrlPressed);
						}
					}
					break;

				case (int)System.Windows.Forms.Keys.Down:
					if ( this.type == TextFieldType.Combo )
					{
						ComboExcavation(1);
					}
					else
					{
						if ( this.type == TextFieldType.MultiLine )
						{
							this.MoveLine(1, isShiftPressed, isCtrlPressed);
						}
						else
						{
							this.MoveCursor(1, isShiftPressed, isCtrlPressed);
						}
					}
					break;
			}
		}

		// Gestion d'une touche pressée avec KeyPress dans le texte.
		protected void ProcessKeyPress(int key)
		{
			if ( key >= 32 )  // TODO: à vérifier ...
			{
				this.InsertCharacter((char)key);
			}
		}

		// Insère un caractère.
		protected bool InsertCharacter(char character)
		{
			return InsertString(TextLayout.ConvertToTaggedText (character));
		}

		// Insère une chaîne correspondant à un caractère ou un tag (jamais plus).
		protected bool InsertString(string ins)
		{
			System.Diagnostics.Debug.Assert(this.textLayout != null);
			this.DeleteSelectedCharacter(false);

			if ( this.Text.Length+ins.Length > this.maxChar )  return false;

			int cursor = this.textLayout.FindOffsetFromIndex(this.cursorTo);
			string text = this.Text;
			text = text.Insert(cursor, ins);
			this.Text = text;
			this.cursorTo ++;
			this.cursorFrom = this.cursorTo;
			this.Invalidate();
			this.ResetCursor();
			this.CursorScroll();
			this.OnTextChanged();
			this.OnTextInserted();
			return true;
		}

		// Supprime le caractère à gauche ou à droite du curseur.
		protected bool DeleteCharacter(int dir)
		{
			System.Diagnostics.Debug.Assert(this.textLayout != null);
			if ( this.DeleteSelectedCharacter(true) )  return false;

			int cursor = this.textLayout.FindOffsetFromIndex(this.cursorTo);

			if ( dir < 0 )  // à gauche du curseur ?
			{
				if ( cursor <= 0 )  return false;

				string text = this.Text;
				int len = this.textLayout.RecedeTag(cursor);
				text = text.Remove(cursor-len, len);
				this.Text = text;
				this.cursorTo --;
				this.cursorFrom = this.cursorTo;
				this.Invalidate();
				this.ResetCursor();
				this.CursorScroll();
				this.OnTextChanged();
				this.OnTextDeleted();
			}
			else	// à droite du curseur ?
			{
				if ( cursor >= this.Text.Length )  return false;

				string text = this.Text;
				int len = this.textLayout.AdvanceTag(cursor);
				text = text.Remove(cursor, len);
				this.Text = text;
				this.Invalidate();
				this.ResetCursor();
				this.CursorScroll();
				this.OnTextChanged();
				this.OnTextDeleted();
			}

			return true;
		}

		// Supprime les caractères sélectionnés dans le texte.
		protected bool DeleteSelectedCharacter(bool signalTextChanged)
		{
			System.Diagnostics.Debug.Assert(this.textLayout != null);
			
			int cursorFrom = this.textLayout.FindOffsetFromIndex(this.cursorFrom);
			int cursorTo   = this.textLayout.FindOffsetFromIndex(this.cursorTo);

			int from = System.Math.Min(cursorFrom, cursorTo);
			int to   = System.Math.Max(cursorFrom, cursorTo);

			if ( from == to )  return false;

			string text = this.Text;
			text = text.Remove(from, to-from);
			this.Text = text;
			from = this.textLayout.FindIndexFromOffset(from);
			this.cursorTo   = from;
			this.cursorFrom = from;
			this.Invalidate();
			this.ResetCursor();
			this.CursorScroll();
			if ( signalTextChanged )  this.OnTextChanged();
			this.OnTextDeleted();
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
			System.Diagnostics.Debug.Assert(this.textLayout != null);
			
			if ( isCtrlPressed )  // début/fin du texte ?
			{
				return MoveCursor(move*1000000, isShiftPressed, false);
			}

			double posx;
			if ( move < 0 )  posx = 0;
			else             posx = this.textLayout.LayoutSize.Width;
			int cursor = this.textLayout.DetectIndex(posx, this.cursorLine);
			if ( cursor == -1 )  return false;
			this.cursorTo = cursor;
			if ( !isShiftPressed )
			{
				this.cursorFrom = cursor;
			}
			this.CursorScroll();
			this.Invalidate();
			this.ResetCursor();
			return true;
		}

		// Déplace le curseur par lignes.
		protected bool MoveLine(int move, bool isShiftPressed, bool isCtrlPressed)
		{
			System.Diagnostics.Debug.Assert(this.textLayout != null);
			
			int cursor = this.textLayout.DetectIndex(this.cursorPosX, this.cursorLine+move);
			if ( cursor == -1 )  return false;
			this.cursorTo = cursor;
			if ( !isShiftPressed )
			{
				this.cursorFrom = cursor;
			}
			this.CursorScroll();
			this.Invalidate();
			this.ResetCursor();
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
			this.CursorScroll();
			this.Invalidate();
			this.ResetCursor();
			return true;
		}

		// Cherche le nom suivant ou précédent dans la comboList, même si elle
		// n'est pas "déroulée".
		protected void ComboExcavation(int dir)
		{
			int		sel;
			bool	exact;

			if ( !ComboSearch(out sel, out exact) )
			{
				sel = 0;
			}
			else
			{
				if ( exact)  sel += dir;
			}
			sel = System.Math.Max(sel, 0);
			sel = System.Math.Min(sel, this.comboList.Count-1);
			this.Text = (string)this.comboList[sel];
			this.SelectAll();
			this.SetFocused(true);
		}

		// Cherche à quelle ligne (dans comboList) correspond le mieux la ligne éditée.
		protected bool ComboSearch(out int rank, out bool exact)
		{
			string edit = this.Text.ToUpper();
			rank = 0;
			exact = false;
			foreach ( string text in this.comboList )
			{
				string maj = text.ToUpper();
				if ( maj == edit )
				{
					exact = true;
					return true;
				}

				if ( maj.StartsWith(edit) )
				{
					exact = false;
					return true;
				}

				rank ++;
			}
			return false;
		}


		// Génère un événement pour dire que le texte a changé (ajout ou suppression).
		protected virtual void OnTextChanged()
		{
			if ( this.TextChanged != null )  // qq'un écoute ?
			{
				this.TextChanged(this);
			}
		}

		// Génère un événement pour dire que le texte a changé (ajout).
		protected virtual void OnTextInserted()
		{
			if ( this.TextInserted != null )  // qq'un écoute ?
			{
				this.TextInserted(this);
			}
		}

		// Génère un événement pour dire que le texte a changé (suppression).
		protected virtual void OnTextDeleted()
		{
			if ( this.TextDeleted != null )  // qq'un écoute ?
			{
				this.TextDeleted(this);
			}
		}


		// Calcule le scrolling pour que le curseur soit visible.
		protected void CursorScroll()
		{
			System.Diagnostics.Debug.Assert(this.textLayout != null);
			
			if ( this.mouseDown )  return;

			this.scrollOffset = new Drawing.Point(0, 0);

			Drawing.Rectangle rCursor = this.textLayout.FindTextCursor(this.cursorTo, out this.cursorLine);
			this.cursorPosX = (rCursor.Left+rCursor.Right)/2;

			Drawing.Point pEnd = this.textLayout.FindTextEnd();

			if ( this.type == TextFieldType.MultiLine )  // scroll vertical ?
			{
				double offset = rCursor.Bottom;
				offset -= this.realSize.Height/2;
				if ( offset < pEnd.Y )  offset = pEnd.Y;
				offset += this.realSize.Height;
				if ( offset > TextField.infinity )  offset = TextField.infinity;
				this.scrollOffset.Y = offset-this.realSize.Height;

				if ( this.scroller != null )
				{
					double h = TextField.infinity-pEnd.Y;  // hauteur de tout le texte
					if ( h <= this.realSize.Height )
					{
						this.scroller.SetEnabled(false);
						this.scroller.Range = 1;
						this.scroller.Display = 1;
						this.scroller.Position = 0;
					}
					else
					{
						this.scroller.SetEnabled(true);
						this.scroller.Range = h-this.realSize.Height;
						this.scroller.Display = this.realSize.Height/h * this.scroller.Range;
						this.scroller.Position = this.scroller.Range - (TextField.infinity-offset);
						this.scroller.ButtonStep = 20;
						this.scroller.PageStep = this.realSize.Height/2;
					}
				}
			}
			else	// scroll horizontal ?
			{
				double offset = rCursor.Right;
				offset += this.realSize.Width/2;
				if ( offset > pEnd.X )  offset = pEnd.X;
				offset -= this.realSize.Width;
				if ( offset < 0 )  offset = 0;
				this.scrollOffset.X = offset;
			}
		}

		// Dessine le texte en cours d'édition.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			System.Diagnostics.Debug.Assert(this.textLayout != null);
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			Drawing.Point     pos   = new Drawing.Point(TextField.margin, TextField.margin);
			pos -= this.scrollOffset;
			
			adorner.PaintTextFieldBackground(graphics, rect, state, dir, this.textStyle);
			
			Drawing.Rectangle rSaveClip = graphics.SaveClippingRectangle ();
			Drawing.Rectangle rClip = new Drawing.Rectangle();
			rClip = rect;
			rClip.Inflate(-2, -2);
			rClip = this.MapClientToRoot (rClip);
			graphics.SetClippingRectangle(rClip);

			if ( (state&WidgetState.Focused) == 0 )
			{
				pos.Y += 1;
				adorner.PaintGeneralTextLayout(graphics, pos, this.textLayout, state&~WidgetState.Focused, dir);
			}
			else
			{
				bool visibleCursor = false;
				
				int from = System.Math.Min(this.cursorFrom, this.cursorTo);
				int to   = System.Math.Max(this.cursorFrom, this.cursorTo);
				
				if ( from == to )
				{
					pos.Y += 1;
					adorner.PaintGeneralTextLayout(graphics, pos, this.textLayout, state&~WidgetState.Focused, dir);
					visibleCursor = TextField.showCursor && this.WindowFrame.Focused;
				}
				else
				{
					Drawing.Rectangle[] rects = this.textLayout.FindTextRange(from, to);
					adorner.PaintTextSelectionBackground(graphics, pos, rects);
					pos.Y += 1;
					adorner.PaintGeneralTextLayout(graphics, pos, this.textLayout, state&~WidgetState.Focused, dir);
				}

				// Dessine le curseur.
				Drawing.Rectangle rCursor = this.textLayout.FindTextCursor(this.cursorTo, out this.cursorLine);
				this.cursorPosX = (rCursor.Left+rCursor.Right)/2;
				double x = rCursor.Left;
				double y = rCursor.Bottom;
				graphics.Align(ref x, ref y);
				rCursor.Offset(x-rCursor.Left+0.5, 0);
				
				adorner.PaintTextCursor(graphics, pos, rCursor, visibleCursor);
			}

			graphics.RestoreClippingRectangle(rSaveClip);
		}


		public event EventHandler TextChanged;
		public event EventHandler TextInserted;
		public event EventHandler TextDeleted;
		
		
		protected TextFieldType					type = TextFieldType.SingleLine;
		protected static readonly double		margin = 3;
		protected static readonly double		infinity = 1000000;
		protected double						leftMargin = 0;
		protected double						rightMargin = 0;
		protected Drawing.Size					realSize;
		protected Drawing.Point					scrollOffset = new Drawing.Point(0, 0);
		protected TextFieldStyle				textStyle;
		protected int							cursorFrom = 0;
		protected int							cursorTo = 0;
		protected int							cursorLine;
		protected double						cursorPosX;
		protected int							maxChar = 1000;
		protected bool							mouseDown = false;
		protected double						minRange = 0;
		protected double						maxRange = 100;
		protected double						step = 1;
		protected Scroller						scroller;
		protected ArrowButton					arrowUp;
		protected ArrowButton					arrowDown;
		protected WindowFrame					comboWindow;
		protected ScrollList					scrollList;
		protected System.Collections.ArrayList	comboList = new System.Collections.ArrayList();
		
		protected static System.Windows.Forms.Timer	flashTimer = new System.Windows.Forms.Timer();
		protected static bool					showCursor = true;
		
		protected static event EventHandler		Blinking;
	}
}
