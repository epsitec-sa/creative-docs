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
	public abstract class AbstractTextField : Widget, Types.IReadOnly
	{
		public AbstractTextField()
		{
			this.AutoEngage = false;
			
			this.InternalState |= InternalState.AutoFocus;
			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.Engageable;
			this.InternalState |= InternalState.AutoDoubleClick;
			this.InternalState |= InternalState.AutoRepeatEngaged;
			
			this.InternalState &= ~ InternalState.AutoResolveResRef;
			
			this.ResetCursor();
			this.MouseCursor = MouseCursor.AsIBeam;
			
			this.CreateTextLayout();
			this.TextLayout.BreakMode |= Drawing.TextBreakMode.SingleLine;
			
			this.navigator = new TextNavigator(this.TextLayout);
			this.navigator.TextDeleted += new Epsitec.Common.Support.EventHandler(this.HandleNavigatorTextDeleted);
			this.navigator.TextInserted += new Epsitec.Common.Support.EventHandler(this.HandleNavigatorTextInserted);
			this.navigator.CursorScrolled += new Epsitec.Common.Support.EventHandler(this.HandleNavigatorCursorScrolled);
			this.navigator.CursorChanged += new Epsitec.Common.Support.EventHandler(this.HandleNavigatorCursorChanged);
			this.textFieldStyle = TextFieldStyle.Normal;
			
			this.copyPasteBehavior = new Helpers.CopyPasteBehavior(this);
			this.OnCursorChanged(true);
		}
		
		public AbstractTextField(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		

		protected TextNavigator					TextNavigator
		{
			get { return this.navigator; }
		}

		
		[Bundle]	public bool					IsReadOnly
		{
			get
			{
				return this.navigator.IsReadOnly;
			}

			set
			{
				if ( this.navigator.IsReadOnly != value )
				{
					this.navigator.IsReadOnly = value;
					
					
					if ( Message.State.LastWindow == this.Window &&
					     this.IsEntered )
					{
						// Ne changeons l'aspect de la souris que si actuellement le curseur se trouve
						// dans la zone éditable; si la souris se trouve sur un bouton, on ne fait rien.
						
						this.UpdateMouseCursor(this.MapRootToClient(Message.State.LastPosition));
					}
					
					this.OnReadOnlyChanged();
				}
			}
		}

		[Bundle]	public bool					AutoSelectOnFocus
		{
			get { return this.autoSelectOnFocus; }
			set { this.autoSelectOnFocus = value; }
		}
		
		[Bundle]	public bool					AutoEraseOnFocus
		{
			get { return this.autoEraseOnFocus; }
			set { this.autoEraseOnFocus = value; }
		}
		
		
		public override double					DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight + 2*(AbstractTextField.TextMargin+AbstractTextField.FrameMargin);
			}
		}

		public override Drawing.Point			BaseLine
		{
			get
			{
				if ( this.TextLayout != null )
				{
					Drawing.Point pos   = this.TextLayout.GetLineOrigin(0);
					Drawing.Point shift = this.InnerTextBounds.Location;

					double yFromTop = this.TextLayout.LayoutSize.Height - pos.Y;
					double yFromBot = this.realSize.Height - yFromTop + shift.Y + 1;

					return this.MapClientToParent(new Drawing.Point(shift.X, yFromBot)) - this.Location;
				}

				return base.BaseLine;
			}
		}
		
		public override Drawing.Rectangle		InnerBounds
		{
			get
			{
				Drawing.Rectangle rect = this.Client.Bounds;
				
				rect.Deflate(this.margins);
				
				if ( this.navigator != null && this.textFieldStyle != TextFieldStyle.Flat )
				{
					if ( this.Client.Height < 18 )
					{
						if ( this.Client.Height >= 15 )
						{
							rect.Deflate(AbstractTextField.FrameMargin/2, AbstractTextField.FrameMargin/2);
						}
					}
					else
					{
						rect.Deflate(AbstractTextField.FrameMargin, AbstractTextField.FrameMargin);
					}
				}
				
				return rect;
			}
		}
		
		public virtual Drawing.Rectangle		InnerTextBounds
		{
			get
			{
				Drawing.Rectangle rect = this.InnerBounds;
				
				if ( this.Client.Height < 18 && this.textFieldStyle != TextFieldStyle.Flat )
				{
					if ( this.Client.Height >= 17 )
					{
						rect.Deflate(AbstractTextField.TextMargin/2, AbstractTextField.TextMargin/2);
					}
				}
				else
				{
					rect.Deflate(AbstractTextField.TextMargin, AbstractTextField.TextMargin);
				}
				
				return rect;
			}
		}
		
		public override Drawing.Size			MinSize
		{
			get
			{
				Drawing.Size min = base.MinSize;
				
				double width  = 20;
				double height = this.DefaultHeight;
				
				return new Drawing.Size (System.Math.Max (min.Width, width), System.Math.Max (min.Height, height));
			}
			set
			{
				base.MinSize = value;
			}
		}

		
		public int								MaxChar
		{
			get { return this.navigator.MaxChar; }
			set { this.navigator.MaxChar = value; }
		}

		public virtual Drawing.Margins			Margins
		{
			get { return this.margins; }
			set { this.margins = value; }
		}
		
		public virtual double					LeftMargin
		{
			get { return this.margins.Left; }
			set { this.margins.Left = value; }
		}
		
		public virtual double					RightMargin
		{
			get { return this.margins.Right; }
			set { this.margins.Right = value; }
		}
		
		public virtual double					BottomMargin
		{
			get { return this.margins.Bottom; }
			set { this.margins.Bottom = value; }
		}
		
		public virtual double					TopMargin
		{
			get { return this.margins.Top; }
			set { this.margins.Top = value; }
		}
		
		public TextFieldStyle					TextFieldStyle
		{
			get
			{
				return this.textFieldStyle;
			}

			set
			{
				if ( this.textFieldStyle != value )
				{
					if ( this.textFieldStyle == TextFieldStyle.Normal ||
						 this.textFieldStyle == TextFieldStyle.Simple ||
						 this.textFieldStyle == TextFieldStyle.Static ||
						 this.textFieldStyle == TextFieldStyle.Flat   )
					{
						if ( value == TextFieldStyle.Normal ||
							 value == TextFieldStyle.Simple ||
							 value == TextFieldStyle.Static ||
							 value == TextFieldStyle.Flat   )
						{
							this.textFieldStyle = value;
							this.Invalidate();
							return;
						}
					}
					
					throw new System.InvalidOperationException(string.Format("Cannot switch from {0} to {1}.", this.textFieldStyle, value));
				}
			}
		}

		public double							ScrollZone
		{
			// Amplitude de la zone dans laquelle le curseur provoque un scroll.
			// Avec 0.0, le texte ne scrolle que lorsque le curseur arrive aux extrémités.
			// Avec 1.0, le texte scrolle tout le temps (curseur au milieu).
			get
			{
				return this.scrollZone;
			}

			set
			{
				value = System.Math.Max(value, 0.0);
				value = System.Math.Min(value, 1.0);
				if ( this.scrollZone != value )
				{
					this.scrollZone = value;
					this.CursorScroll(true);
				}
			}
		}

		
		public string							Selection
		{
			get
			{
				return this.navigator.Selection;
			}

			set
			{
				this.navigator.Selection = value;
			}
		}
		
		
		public int								Cursor
		{
			get
			{
				return this.navigator.Cursor;
			}

			set
			{
				this.navigator.Cursor = value;
			}
		}
		
		public int								CursorFrom
		{
			get
			{
				return this.navigator.CursorFrom;
			}

			set
			{
				this.navigator.CursorFrom = value;
			}
		}
		
		public int								CursorTo
		{
			get
			{
				return this.navigator.CursorTo;
			}

			set
			{
				this.navigator.CursorTo = value;
			}
		}

		public bool								CursorAfter
		{
			get
			{
				return this.navigator.CursorAfter;
			}

			set
			{
				this.navigator.CursorAfter = value;
			}
		}


		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( TextField.blinking == this )
				{
					this.navigator.TextDeleted -= new Epsitec.Common.Support.EventHandler(this.HandleNavigatorTextDeleted);
					this.navigator.TextInserted -= new Epsitec.Common.Support.EventHandler(this.HandleNavigatorTextInserted);
					this.navigator.CursorScrolled -= new Epsitec.Common.Support.EventHandler(this.HandleNavigatorCursorScrolled);
					this.navigator.CursorChanged -= new Epsitec.Common.Support.EventHandler(this.HandleNavigatorCursorChanged);
					TextField.blinking = null;
				}
			}
			
			base.Dispose(disposing);
		}

		
		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("About to get focus on '{0}'.", this.Text));
			
			if ( mode != TabNavigationMode.Passive )
			{
				this.SelectAll();
			}
			
			return base.AboutToGetFocus(dir, mode, out focus);
		}

		protected override void ModifyTextLayout(string text)
		{
			if ( text.Length > this.navigator.MaxChar )
			{
				text = text.Substring(0, this.navigator.MaxChar);
			}
			
			base.ModifyTextLayout(text);
		}

		protected override void DisposeTextLayout()
		{
			// Ne fait rien, on veut s'assurer que le TextLayout associé avec le
			// TextField n'est jamais détruit du vivant du TextField.
			this.TextLayout.Text = "";
		}

		
		public void SelectAll()
		{
			// Sélectione tous les caractères.
			this.Cursor = 0;
			this.SelectAll(false);
		}

		protected void SelectAll(bool silent)
		{
			this.TextLayout.SelectAll(this.navigator.Context);
			this.OnCursorChanged(silent);
		}

		public void DeleteSelection()
		{
			if ( this.TextLayout.DeleteSelection(this.navigator.Context) )
			{
				this.OnTextChanged();
			}
		}

		public void SimulateEdited()
		{
			this.OnTextEdited();
		}
		
		
		public bool								SelectionBold
		{
			// Attribut typographique "gras" des caractères sélectionnés.
			get
			{
				return this.TextLayout.IsSelectionBold(this.navigator.Context);
			}

			set
			{
				this.TextLayout.SetSelectionBold(this.navigator.Context, value);
				this.OnTextChanged();
			}
		}

		public bool								SelectionItalic
		{
			// Attribut typographique "italique" des caractères sélectionnés.
			get
			{
				return this.TextLayout.IsSelectionItalic(this.navigator.Context);
			}

			set
			{
				this.TextLayout.SetSelectionItalic(this.navigator.Context, value);
				this.OnTextChanged();
			}
		}

		public bool								SelectionUnderlined
		{
			// Attribut typographique "souligné" des caractères sélectionnés.
			get
			{
				return this.TextLayout.IsSelectionUnderlined(this.navigator.Context);
			}

			set
			{
				this.TextLayout.SetSelectionUnderlined(this.navigator.Context, value);
				this.OnTextChanged();
			}
		}

		public string							SelectionFontName
		{
			// Nom de la police des caractères sélectionnés.
			get
			{
				return this.TextLayout.GetSelectionFontName(this.navigator.Context);
			}

			set
			{
				this.TextLayout.SetSelectionFontName(this.navigator.Context, value);
				this.OnTextChanged();
			}
		}

		public double							SelectionFontSize
		{
			// Taille de la police des caractères sélectionnés.
			get
			{
				return this.TextLayout.GetSelectionFontSize(this.navigator.Context);
			}

			set
			{
				this.TextLayout.SetSelectionFontSize(this.navigator.Context, value);
				this.OnTextChanged();
			}
		}

		public Drawing.Color					SelectionFontColor
		{
			// Couleur de la police des caractères sélectionnés.
			get
			{
				return this.TextLayout.GetSelectionFontColor(this.navigator.Context);
			}

			set
			{
				this.TextLayout.SetSelectionFontColor(this.navigator.Context, value);
				this.OnTextChanged();
			}
		}

		
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.TopLeft;
			}
		}

		
		public Drawing.Rectangle GetButtonBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = this.Bounds.Width-this.margins.Right-adorner.GeometryComboRightMargin;
			rect.Right  = this.Bounds.Width-adorner.GeometryComboRightMargin;
			rect.Bottom = adorner.GeometryComboBottomMargin;
			rect.Top    = this.Bounds.Height-adorner.GeometryComboTopMargin;
			return rect;
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
					this.CursorScroll(true);
				}
			}
		}
		
		protected virtual Drawing.Size GetTextLayoutSize()
		{
			return new Drawing.Size(AbstractTextField.Infinity, this.realSize.Height);
		}

		
		protected static void HandleFlashTimer(object source)
		{
			// Gère le temps écoulé pour faire clignoter un curseur.
			TextField.showCursor = !TextField.showCursor;
			
			if ( TextField.blinking != null )
			{
				TextField.blinking.FlashCursor();
			}
		}
		
		
		protected void FlashCursor()
		{
			// Fait clignoter le curseur.
			this.Invalidate();
		}

		protected void ResetCursor()
		{
			// Allume le curseur au prochain affichage.
			if ( this.IsFocused && TextField.flashTimer != null )
			{
				double delay = SystemInformation.CursorBlinkDelay;
				TextField.flashTimer.Delay = delay;
				TextField.flashTimer.AutoRepeat = delay;
				TextField.flashTimer.Start();  // restart du timer
				TextField.showCursor = true;  // avec le curseur visible
			}
		}

		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			// Gestion d'un événement.
			if ( this.copyPasteBehavior.ProcessMessage(message, pos) )
			{
				return;
			}
			
			Shortcut shortcut = Shortcut.FromMessage(message);
			
			if ( shortcut != null )
			{
				if ( this.ShortcutHandler(shortcut) )
				{
					message.Handled   = true;
					message.Swallowed = true;
					
					return;
				}
			}
			
			this.lastMousePos = pos;
			pos = this.Client.Bounds.Constrain(pos);
			pos.X -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos.Y -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos += this.scrollOffset;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					if ( this.ProcessMouseDown(message, pos) )
					{
						message.Consumer = this;
					}
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						this.EnableScroll(this.lastMousePos);
						this.navigator.ProcessMessage(message, pos);
						message.Consumer = this;
					}
					else
					{
						if (this.UpdateMouseCursor(pos))
						{
							message.Consumer = this;
						}
					}
					break;
				
				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						this.SetEngaged(false);
						this.navigator.ProcessMessage(message, pos);
						this.mouseDown = false;
						message.Consumer = this;
					}
					break;
				
				case MessageType.KeyDown:
					if ( this.ProcessKeyDown(message, pos) )
					{
						message.Consumer = this;
					}
					break;
				
				case MessageType.KeyPress:
					if ( this.navigator.ProcessMessage(message, pos) )
					{
						message.Consumer = this;
					}
					break;
			}
		}
		
		protected virtual bool ProcessMouseDown(Message message, Drawing.Point pos)
		{
			this.navigator.ProcessMessage(message, pos);
			
			if ( this.AutoSelectOnFocus && !this.IsFocusedFlagSet )
			{
				this.SelectAll();
				message.Swallowed = true;
			}
			else
			{
				this.mouseDown = true;
			}
			
			if ( this.IsReadOnly == false )
			{
				// Un clic dans la ligne éditable doit mettre le focus sur celle-ci, quel que
				// soit le type de gestion de focus actif (AutoFocus, etc.).
				
				this.Focus();
			}
			
			return true;
		}
		
		protected virtual bool ProcessKeyDown(Message message, Drawing.Point pos)
		{
			// Gestion d'une touche pressée avec KeyDown dans le texte.
			return this.navigator.ProcessMessage(message, pos);
		}

		protected void EnableScroll(Drawing.Point pos)
		{
			this.scrollLeft   = ( pos.X <= this.Client.Bounds.Left   );
			this.scrollRight  = ( pos.X >= this.Client.Bounds.Right  );
			this.scrollBottom = ( pos.Y <= this.Client.Bounds.Bottom );
			this.scrollTop    = ( pos.Y >= this.Client.Bounds.Top    );

			if ( this.scrollLeft || this.scrollRight || this.scrollBottom || this.scrollTop )
			{
				this.SetEngaged(true);
			}
			else
			{
				this.SetEngaged(false);
			}
		}

		protected override void OnStillEngaged()
		{
			base.OnStillEngaged();

			double amplitude = 4;
			if ( this.scrollLeft   )  this.ScrollHorizontal(-amplitude);
			if ( this.scrollRight  )  this.ScrollHorizontal(amplitude);
			if ( this.scrollBottom )  this.ScrollVertical(-amplitude);
			if ( this.scrollTop    )  this.ScrollVertical(amplitude);
		}

		
		private void HandleNavigatorTextDeleted(object sender)
		{
			this.OnTextEdited();
			this.OnTextDeleted();
		}

		private void HandleNavigatorTextInserted(object sender)
		{
			this.OnTextEdited();
			this.OnTextInserted();
		}

		private void HandleNavigatorCursorScrolled(object sender)
		{
			this.CursorScroll(false);
		}

		private void HandleNavigatorCursorChanged(object sender)
		{
			this.OnCursorChanged(false);
		}

		
		protected override void OnFocused()
		{
			TextField.blinking = this;
			this.ResetCursor();
			
			if ( this.AutoSelectOnFocus )
			{
				Support.CancelEventArgs cancelEvent = new Support.CancelEventArgs();
				this.OnAutoSelecting(cancelEvent);
				
				if ( !cancelEvent.Cancel )
				{
					this.SelectAll();
				}
			}
			
			if ( this.AutoEraseOnFocus )
			{
				Support.CancelEventArgs cancelEvent = new Support.CancelEventArgs();
				this.OnAutoErasing(cancelEvent);
				
				if ( !cancelEvent.Cancel )
				{
					this.Text = "";
				}
			}
			
			base.OnFocused();
		}

		protected override void OnDefocused()
		{
			TextField.blinking = null;
			base.OnDefocused();
		}

		protected override void OnTextChanged()
		{
			int from = this.CursorFrom;
			int to   = this.CursorTo;
			
			// En réaffectant les positions de curseurs, on force implicitement une vérification sur
			// les positions maximales tolérées (grâce à TextNavigator).
			this.navigator.SetCursors(from, to);
			
			// Génère un événement pour dire que le texte a changé (tout changement).
			this.ResetCursor();
			this.CursorScroll(false);
			this.Invalidate();
			
			base.OnTextChanged();
		}
		
		protected override void OnAdornerChanged()
		{
			base.OnAdornerChanged();
			this.UpdateClientGeometry();
		}
		
		protected override void OnCultureChanged()
		{
			base.OnCultureChanged ();
			this.SelectAll ();
		}

		
		protected virtual void OnTextDeleted()
		{
			this.OnTextChanged();

			if ( this.TextDeleted != null )  // qq'un écoute ?
			{
				this.TextDeleted(this);
			}
		}

		protected virtual void OnTextInserted()
		{
			this.OnTextChanged();

			if ( this.TextInserted != null )  // qq'un écoute ?
			{
				this.TextInserted(this);
			}
		}

		protected virtual void OnTextEdited()
		{
			if ( this.TextEdited != null )
			{
				this.TextEdited(this);
			}
		}

		
		protected virtual void OnCursorChanged(bool silent)
		{
			this.ResetCursor();
			this.Invalidate();

			if ( this.navigator == null )  return;

			if ( this.navigator.Context.CursorFrom != this.navigator.Context.CursorTo ||
				 this.lastCursorFrom != this.lastCursorTo )
			{
				if ( this.SelectionChanged != null )  // qq'un écoute ?
				{
					this.SelectionChanged(this);
				}
			}

			if ( !silent && this.navigator.Context.CursorFrom == this.navigator.Context.CursorTo )
			{
				if ( this.CursorChanged != null )  // qq'un écoute ?
				{
					this.CursorChanged(this);
				}
			}

			this.lastCursorFrom = this.navigator.Context.CursorFrom;
			this.lastCursorTo   = this.navigator.Context.CursorTo;
		}
		
		protected virtual void OnReadOnlyChanged()
		{
			if (this.ReadOnlyChanged != null)
			{
				this.ReadOnlyChanged (this);
			}
		}

		protected virtual void OnAutoSelecting(Support.CancelEventArgs e)
		{
			if ( this.AutoSelecting != null )
			{
				this.AutoSelecting(this, e);
			}
		}
		
		protected virtual void OnAutoErasing(Support.CancelEventArgs e)
		{
			if ( this.AutoErasing != null )
			{
				this.AutoErasing(this, e);
			}
		}
		
		
		protected void CursorScroll(bool force)
		{
			//	Calcule le scrolling pour que le curseur soit visible.
			if ( this.TextLayout == null )  return;
			if ( this.mouseDown )  return;
			if ( this.navigator == null ) return;

			Drawing.Point p1, p2;
			if ( this.TextLayout.FindTextCursor(this.navigator.Context, out p1, out p2) )
			{
				Drawing.Rectangle cursor = new Drawing.Rectangle(p1, p2);
				this.CursorScrollText(cursor, force);
			}
		}
		
		protected virtual void CursorScrollText(Drawing.Rectangle cursor, bool force)
		{
			Drawing.Point end = this.TextLayout.FindTextEnd();
			
			if ( this.TextLayout.TotalLineCount == 1 )
			{
				Drawing.Point linePos;
				
				double lineAscender;
				double lineDescender;
				double lineWidth;
				
				this.TextLayout.GetLineGeometry(0, out linePos, out lineAscender, out lineDescender, out lineWidth);
				
				if ( lineWidth-this.scrollOffset.X < this.realSize.Width )
				{
					force = true;
				}
			}
			
			if ( force )
			{
				double offset = cursor.Right;
				offset += this.realSize.Width/2;
				offset  = System.Math.Min(offset, end.X);
				offset -= this.realSize.Width;
				offset  = System.Math.Max(offset, 0);
				this.scrollOffset.X = offset;
			}
			else
			{
				double ratio = (cursor.Right-this.scrollOffset.X)/this.realSize.Width;  // 0..1
				double zone = this.scrollZone*0.5;

				if ( ratio <= zone )  // curseur trop à gauche ?
				{
					this.scrollOffset.X -= (zone-ratio)*this.realSize.Width;
					this.scrollOffset.X = System.Math.Max(this.scrollOffset.X, 0.0);
				}

				if ( ratio >= 1.0-zone )  // curseur trop à droite ?
				{
					this.scrollOffset.X += (ratio-(1.0-zone))*this.realSize.Width;
					double max = System.Math.Max(end.X-this.realSize.Width, 0.0);
					this.scrollOffset.X = System.Math.Min(this.scrollOffset.X, max);
				}
			}

			this.scrollOffset.Y = 0;
		}

		protected virtual void ScrollHorizontal(double dist)
		{
			// Décale le texte vers la droite (+) ou la gauche (-), lorsque la
			// souris dépasse pendant une sélection.
			if ( this.textFieldStyle == TextFieldStyle.Multi )  return;

			this.scrollOffset.X += dist;
			Drawing.Point end = this.TextLayout.FindTextEnd();
			double max = System.Math.Max(end.X-this.realSize.Width, 0.0);
			this.scrollOffset.X = System.Math.Max(this.scrollOffset.X, 0.0);
			this.scrollOffset.X = System.Math.Min(this.scrollOffset.X, max);
			this.Invalidate();

			Drawing.Point pos = this.lastMousePos;
			pos = this.Client.Bounds.Constrain(pos);
			pos.X -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos.Y -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos += this.scrollOffset;
			this.navigator.MouseMoveMessage(pos);
		}

		protected virtual void ScrollVertical(double dist)
		{
			// Décale le texte vers le haut (+) ou le bas (-), lorsque la
			// souris dépasse pendant une sélection.
		}

		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if ( AbstractTextField.flashTimerStarted == false )
			{
				// Il faut enregistrer le timer; on ne peut pas le faire avant que le
				// premier TextField ne s'affiche, car sinon les WinForms semblent se
				// mélanger les pinceaux :
				TextField.flashTimer = new Timer();
				TextField.flashTimer.TimeElapsed += new Support.EventHandler(TextField.HandleFlashTimer);
				TextField.flashTimerStarted = true;
				
				this.ResetCursor();
			}
			
			// Dessine le texte en cours d'édition :
			System.Diagnostics.Debug.Assert(this.TextLayout != null);
			
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			WidgetState       state     = this.PaintState;
			Drawing.Point     pos       = this.InnerTextBounds.Location - this.scrollOffset + new Drawing.Point(0, 1);
			Drawing.Rectangle rText     = this.InnerTextBounds;
			Drawing.Rectangle rInside   = this.InnerBounds;
			Drawing.Rectangle rSaveClip = graphics.SaveClippingRectangle();
			Drawing.Rectangle rClip     = rInside;
			Drawing.Rectangle rFill     = this.Client.Bounds;
			
			if ( this.textFieldStyle == TextFieldStyle.Flat )
			{
				rFill.Deflate(1, 1);
			}
			
			if ( this.BackColor.IsTransparent )
			{
				// Ne peint pas le fond de la ligne éditable si celle-ci a un fond
				// explicitement défini comme "transparent".
			}
			else
			{
				// Ne reproduit pas l'état sélectionné si on peint nous-même le fond
				// de la ligne éditable.
				state &= ~WidgetState.Selected;
				adorner.PaintTextFieldBackground(graphics, rFill, state, this.textFieldStyle, this.navigator.IsReadOnly);
			}
			
//			graphics.AddFilledRectangle(rText);
//			graphics.RenderSolid(Drawing.Color.FromARGB(0.6, 1, 0, 0));
			
			rClip = this.MapClientToRoot(rClip);
			graphics.SetClippingRectangle(rClip);
			
			if ( this.IsFocused )
			{
				bool visibleCursor = false;
				
				int from = System.Math.Min(this.navigator.Context.CursorFrom, this.navigator.Context.CursorTo);
				int to   = System.Math.Max(this.navigator.Context.CursorFrom, this.navigator.Context.CursorTo);
				
				if ( this.isCombo && this.navigator.IsReadOnly )
				{
					TextLayout.SelectedArea[] areas = new TextLayout.SelectedArea[1];
					areas[0] = new TextLayout.SelectedArea();
					areas[0].Rect = rInside;
					areas[0].Rect.Deflate(1, 1);
					adorner.PaintTextSelectionBackground(graphics, areas, state);
					adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, (state&~WidgetState.Focused)|WidgetState.Selected, PaintTextStyle.TextField, this.BackColor);
					adorner.PaintFocusBox(graphics, areas[0].Rect);
				}
				else if ( from == to )
				{
					adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, state&~WidgetState.Focused, PaintTextStyle.TextField, this.BackColor);
					visibleCursor = TextField.showCursor && this.Window.IsFocused;
				}
				else
				{
					// Un morceau de texte a été sélectionné. Peint en plusieurs étapes :
					// - Peint tout le texte normalement
					// - Peint les rectangles de sélection
					// - Peint tout le texte en mode sélectionné, avec clipping
					
					TextLayout.SelectedArea[] areas = this.TextLayout.FindTextRange(pos, from, to);
					if ( areas.Length == 0 )
					{
						adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, state&~WidgetState.Focused, PaintTextStyle.TextField, this.BackColor);
						visibleCursor = TextField.showCursor && this.Window.IsFocused;
					}
					else
					{
						adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, state&~(WidgetState.Focused|WidgetState.Selected), PaintTextStyle.TextField, this.BackColor);
					
						for ( int i=0 ; i<areas.Length ; i++ )
						{
							areas[i].Rect.Offset(0, -1);
							graphics.Align(ref areas[i].Rect);
						}
						adorner.PaintTextSelectionBackground(graphics, areas, state);
					
						Drawing.Rectangle[] rects = new Drawing.Rectangle[areas.Length];
						for ( int i=0 ; i<areas.Length ; i++ )
						{
							rects[i] = this.MapClientToRoot(areas[i].Rect);
						}
						graphics.SetClippingRectangles(rects);

						adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, (state&~WidgetState.Focused)|WidgetState.Selected, PaintTextStyle.TextField, this.BackColor);
					}
				}
				
				if ( !this.navigator.IsReadOnly && visibleCursor )
				{
					// Dessine le curseur :
					Drawing.Point p1, p2;
					if ( this.TextLayout.FindTextCursor(this.navigator.Context, out p1, out p2) )
					{
						p1 += pos;
						p2 += pos;
						adorner.PaintTextCursor(graphics, p1, p2, true);
					}
				}
			}
			else
			{
				adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, state&~WidgetState.Focused, PaintTextStyle.TextField, this.BackColor);
			}

			graphics.RestoreClippingRectangle(rSaveClip);
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.UpdateButtonGeometry();
			this.OnCursorChanged(true);
		}
		
		protected virtual void UpdateButtonGeometry()
		{
		}
		
		protected virtual bool UpdateMouseCursor(Drawing.Point pos)
		{
			if (this.Client.Bounds.Contains(pos))
			{
				if ((pos.X >= this.margins.Left) &&
					(pos.X <= this.Client.Width - this.margins.Right) &&
					(this.navigator.IsReadOnly == false))
				{
					this.MouseCursor = MouseCursor.AsIBeam;
				}
				else
				{
					this.MouseCursor = MouseCursor.AsArrow;
				}
				
				return true;
			}
			
			return false;
		}
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Inflate(adorner.GeometryTextFieldShapeBounds);
			return rect;
		}

		
		#region IReadOnly Members
		bool									Types.IReadOnly.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}
		#endregion
		
		public event Support.EventHandler		TextEdited;
		public event Support.EventHandler		TextInserted;
		public event Support.EventHandler		TextDeleted;
		public event Support.EventHandler		SelectionChanged;
		public event Support.EventHandler		CursorChanged;
		public event Support.EventHandler		ReadOnlyChanged;
		public event Support.CancelEventHandler	AutoSelecting;
		public event Support.CancelEventHandler	AutoErasing;
		
		
		internal static readonly double			TextMargin = 2;
		internal static readonly double			FrameMargin = 2;
		internal static readonly double			Infinity = 1000000;
		
		protected bool							isCombo = false;
		private bool							autoSelectOnFocus;
		private bool							autoEraseOnFocus;
		protected Drawing.Margins				margins = new Drawing.Margins();
		protected Drawing.Size					realSize;
		protected Drawing.Point					scrollOffset = new Drawing.Point();
		protected bool							mouseDown = false;
		protected bool							scrollLeft = false;
		protected bool							scrollRight = false;
		protected bool							scrollBottom = false;
		protected bool							scrollTop = false;
		protected Drawing.Point					lastMousePos;
		protected TextFieldStyle				textFieldStyle = TextFieldStyle.Normal;
		protected double						scrollZone = 0.5;
		
		private TextNavigator					navigator;
		private int								lastCursorFrom = -1;
		private int								lastCursorTo = -1;
		
		private Helpers.CopyPasteBehavior		copyPasteBehavior;
		
		private static Timer					flashTimer;
		private static bool						flashTimerStarted = false;
		private static bool						showCursor = true;
		
		protected static AbstractTextField		blinking;
	}
}
