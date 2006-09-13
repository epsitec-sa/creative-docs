using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
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
	
	public enum TextDisplayMode
	{
		Default,						// valeur standard
		Defined,						// valeur d�finie
		Proposal,						// proposition en italique
	}


	/// <summary>
	/// La classe TextField impl�mente la ligne �ditable, tout en permettant
	/// aussi de r�aliser l'�quivalent de la ComboBox Windows.
	/// </summary>
	public abstract class AbstractTextField : Widget, Types.IReadOnly
	{
		public AbstractTextField()
		{
			this.AutoEngage = false;
			this.AutoFocus  = true;
			this.AutoRepeat = true;
			this.AutoDoubleClick = true;
			
			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.Engageable;
			
//@			this.InternalState &= ~ InternalState.AutoResolveResRef;
			
			this.ResetCursor();
			this.MouseCursor = MouseCursor.AsIBeam;

			this.InitializeMargins ();
			this.CreateTextLayout ();

			this.navigator = new TextNavigator (base.TextLayout);
			this.navigator.AboutToChange += new EventHandler (this.HandleNavigatorAboutToChange);
			this.navigator.TextDeleted += new EventHandler(this.HandleNavigatorTextDeleted);
			this.navigator.TextInserted += new EventHandler(this.HandleNavigatorTextInserted);
			this.navigator.CursorScrolled += new EventHandler(this.HandleNavigatorCursorScrolled);
			this.navigator.CursorChanged += new EventHandler(this.HandleNavigatorCursorChanged);
			this.navigator.StyleChanged += new EventHandler(this.HandleNavigatorStyleChanged);
			this.textFieldStyle = TextFieldStyle.Normal;
			
			this.copyPasteBehavior = new Behaviors.CopyPasteBehavior(this);
			this.OnCursorChanged(true);

			CommandDispatcher dispatcher = new CommandDispatcher ("TextField", CommandDispatcherLevel.Secondary);
			dispatcher.RegisterController (new Dispatcher (this));

			CommandDispatcher.SetDispatcher (this, dispatcher);
			
			this.IsFocusedChanged += this.HandleIsFocusedChanged;
		}
		
		public AbstractTextField(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		

		public TextNavigator					TextNavigator
		{
			get { return this.navigator; }
		}

		public virtual bool						IsCombo
		{
			get
			{
				return false;
			}
		}
		
		public bool								IsReadOnly
		{
			get
			{
				return this.navigator.IsReadOnly;
			}
			set
			{
				if (this.navigator.IsReadOnly != value)
				{
					this.navigator.IsReadOnly = value;


					if ((Message.CurrentState.LastWindow == this.Window) &&
					    (this.IsEntered))
					{
						//	Ne changeons l'aspect de la souris que si actuellement le curseur se trouve
						//	dans la zone �ditable; si la souris se trouve sur un bouton, on ne fait rien.

						this.UpdateMouseCursor (this.MapRootToClient (Message.CurrentState.LastPosition));
					}

					this.OnReadOnlyChanged ();
				}
			}
		}

		public bool								IsModal
		{
			get
			{
				return this.is_modal;
			}
			set
			{
				if (this.is_modal != value)
				{
					this.is_modal = value;

					Window window = this.Window;

					if ((window != null) &&
						(window.ModalWidget == this))
					{
						window.ModalWidget = null;
					}
				}
			}
		}

		public bool								IsTextEmpty
		{
			get
			{
				return this.navigator.IsEmpty;
			}
		}
		
		public bool								HasEditedText
		{
			get
			{
				return this.has_edited_text;
			}
		}
		
		public bool								AutoSelectOnFocus
		{
			get { return this.autoSelectOnFocus; }
			set { this.autoSelectOnFocus = value; }
		}
		
		public bool								AutoEraseOnFocus
		{
			get { return this.autoEraseOnFocus; }
			set { this.autoEraseOnFocus = value; }
		}
		
		public bool								SwallowReturn
		{
			get
			{
				return this.swallow_return;
			}
			set
			{
				this.swallow_return = value;
			}
		}
		
		public bool								SwallowEscape
		{
			get
			{
				return this.swallow_escape;
			}
			set
			{
				this.swallow_escape = value;
			}
		}

		protected bool							IsEditing
		{
			get
			{
				return this.is_editing;
			}
			set
			{
				if (this.is_editing != value)
				{
					this.is_editing = value;

					if (this.is_modal)
					{
						Window window = this.Window;

						if (window != null)
						{
							if ((this.is_editing) &&
								(window.ModalWidget == null))
							{
								window.ModalWidget = this;
							}
							else if ((this.is_editing == false) &&
								/**/ (window.ModalWidget == this))
							{
								window.ModalWidget = null;
							}
						}
					}
				}
			}
		}
		
		static AbstractTextField()
		{
			Helpers.VisualPropertyMetadata metadataAlign = new Helpers.VisualPropertyMetadata (Drawing.ContentAlignment.TopLeft, Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Helpers.VisualPropertyMetadata metadataHeight = new Helpers.VisualPropertyMetadata (Widget.DefaultFontHeight + 2*(AbstractTextField.TextMargin+AbstractTextField.FrameMargin), Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			
			Visual.ContentAlignmentProperty.OverrideMetadata (typeof (AbstractTextField), metadataAlign);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (AbstractTextField), metadataHeight);
		}
		
		public void ProcessCut()
		{
			this.copyPasteBehavior.ProcessCopy();
			this.copyPasteBehavior.ProcessDelete();
		}

		public void ProcessCopy()
		{
			this.copyPasteBehavior.ProcessCopy();
		}

		public void ProcessPaste()
		{
			this.copyPasteBehavior.ProcessPaste();
		}

#if false
		public override Drawing.Point GetBaseLine()
		{
			if ( this.TextLayout != null )
			{
				Drawing.Point pos   = this.TextLayout.GetLineOrigin(0);
				Drawing.Point shift = this.InnerTextBounds.Location;

				double yFromTop = this.TextLayout.LayoutSize.Height - pos.Y;
				double yFromBot = this.realSize.Height - yFromTop + shift.Y + this.GetBaseLineVerticalOffset ();

				Drawing.Point point = new Drawing.Point (shift.X, yFromBot);

				if (this.Parent == null)
				{
					return point;
				}
				else
				{
					return this.MapClientToParent (point) - this.ActualLocation;
				}
			}

			return base.GetBaseLine ();
		}
#endif

		public override Drawing.Point GetBaseLine(double width, double height, out double ascender, out double descender)
		{
			if (this.TextLayout != null)
			{
				//	D�termine la zone rectangulaire dans laquelle le texte est
				//	affich� et utilise celle-ci comme r�f�rence pour les calculs
				//	de hauteur :
				
				Drawing.Rectangle bounds = this.GetInnerTextBounds (new Drawing.Rectangle (0, 0, width, height));
				
				Drawing.Point origin = base.GetBaseLine (width, bounds.Height, out ascender, out descender);
				Drawing.Point point  = bounds.Location + origin;

				double hAbove = height - bounds.Top;
				double hBelow = bounds.Bottom;

				ascender  += hAbove;
				descender -= hBelow;
				
				return point;
			}

			return base.GetBaseLine (width, height, out ascender, out descender);
		}

		protected override double GetBaseLineVerticalOffset()
		{
			return 1.0;
		}

		public override Drawing.Margins GetInternalPadding()
		{
			return this.GetInternalPadding (this.Client.Size);
		}
		
		public Drawing.Rectangle				InnerTextBounds
		{
			get
			{
				return this.GetInnerTextBounds (this.Client.Bounds);
			}
		}

		private Drawing.Margins GetInternalPadding(Drawing.Size size)
		{
			Drawing.Margins padding = this.margins;

			if (this.textFieldStyle != TextFieldStyle.Flat)
			{
				double excess = System.Math.Max ((22-size.Height)/2, 0);
				double x = System.Math.Max (1, AbstractTextField.FrameMargin-excess);
				double y = System.Math.Max (0, AbstractTextField.FrameMargin-excess);
				padding = padding + new Drawing.Margins (x, x, y, y);
			}

			return padding;
		}

		private Drawing.Rectangle GetInnerTextBounds(Drawing.Rectangle rect)
		{
			rect.Deflate (this.GetInternalPadding (rect.Size));
			rect.Deflate (AbstractTextField.TextMargin, AbstractTextField.TextMargin);
			return rect;
		}
		
#if false	//#fix
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
#endif

		
		public int								MaxChar
		{
			get { return this.navigator.MaxChar; }
			set { this.navigator.MaxChar = value; }
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
			//	Amplitude de la zone dans laquelle le curseur provoque un scroll.
			//	Avec 0.0, le texte ne scrolle que lorsque le curseur arrive aux extr�mit�s.
			//	Avec 1.0, le texte scrolle tout le temps (curseur au milieu).
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


		public DefocusAction					DefocusAction
		{
			get
			{
				return this.defocus_action;
			}
			set
			{
				this.defocus_action = value;
			}
		}

		public ShowCondition					ButtonShowCondition
		{
			get
			{
				return this.button_show_condition;
			}
			set
			{
				if (this.button_show_condition != value)
				{
					this.button_show_condition = value;
					this.UpdateButtonVisibility ();
				}
			}
		}
		

		public TextDisplayMode					TextDisplayMode
		{
			get
			{
				return this.textDisplayMode;
			}

			set
			{
				if ( this.textDisplayMode != value )
				{
					this.textDisplayMode = value;
					this.Invalidate();
				}
			}
		}

		public TextDisplayMode					InitialTextDisplayMode
		{
			get
			{
				return this.initial_text_display_mode;
			}

			set
			{
				this.initial_text_display_mode = value;
			}
		}

		public string							InitialText
		{
			get
			{
				return this.initial_text;
			}
			set
			{
				this.initial_text = value;
			}
		}

		public void DefineOpletQueue(OpletQueue queue)
		{
			CommandDispatcher dispatcher = CommandDispatcher.GetDispatcher (this);

			System.Diagnostics.Debug.Assert (dispatcher != null);
			System.Diagnostics.Debug.Assert (this.navigator != null);
			
			this.navigator.OpletQueue = queue;
			dispatcher.OpletQueue = queue;
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.IsFocusedChanged -= this.HandleIsFocusedChanged;
				if ( TextField.blinking == this )
				{
					if ( this.navigator != null )
					{
						this.navigator.AboutToChange -= new EventHandler(this.HandleNavigatorAboutToChange);
						this.navigator.TextDeleted -= new EventHandler(this.HandleNavigatorTextDeleted);
						this.navigator.TextInserted -= new EventHandler(this.HandleNavigatorTextInserted);
						this.navigator.CursorScrolled -= new EventHandler(this.HandleNavigatorCursorScrolled);
						this.navigator.CursorChanged -= new EventHandler(this.HandleNavigatorCursorChanged);
						this.navigator.StyleChanged -= new EventHandler(this.HandleNavigatorStyleChanged);
					}
					
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
			//	Ne fait rien, on veut s'assurer que le TextLayout associ� avec le
			//	TextField n'est jamais d�truit du vivant du TextField.
			this.navigator.TextLayout.Text = "";
		}

		
		public void ClearText()
		{
			if (this.navigator.TextLayout.Text.Length > 0)
			{
				this.navigator.TextLayout.Text = "";
				this.OnTextDeleted();
			}
		}
		
		public void SelectAll()
		{
			//	S�lectione tous les caract�res.
			this.Cursor = 0;
			this.SelectAll(false);
		}

		
		public virtual bool StartEdition()
		{
			if ((this.IsEditing == false) &&
				((this.DefocusAction != DefocusAction.None) || (this.IsCombo) || (this.ButtonShowCondition != ShowCondition.Never)))
			{
				this.initial_text              = this.Text;
				this.initial_text_display_mode = this.TextDisplayMode;
				
				this.IsEditing = true;
				
				if (this.textDisplayMode == TextDisplayMode.Proposal)
				{
					this.textDisplayMode = TextDisplayMode.Defined;
				}
				
				this.OnEditionStarted ();
				
				return true;
			}
			
			return false;
		}
		
		public virtual bool AcceptEdition()
		{
			if (this.IsEditing)
			{
				this.IsEditing = false;
				
				this.OnEditionAccepted ();
				
				this.SelectAll ();
				
				return true;
			}
			
			return false;
		}
		
		public virtual bool RejectEdition()
		{
			if (this.IsEditing)
			{
				this.IsEditing = false;
				
				this.OnEditionRejected ();
				
				this.Text            = this.InitialText;
				this.TextDisplayMode = this.InitialTextDisplayMode;
				
				this.SelectAll ();
				
				return true;
			}
			
			return false;
		}
		
		
		protected virtual void SelectAll(bool silent)
		{
			this.navigator.TextLayout.SelectAll(this.navigator.Context);
			this.OnCursorChanged(silent);
		}

		public void SimulateEdited()
		{
			this.OnTextEdited();
		}
		
		
		public bool GetCursorPosition(out Drawing.Point p1, out Drawing.Point p2)
		{
			Drawing.Point pos = this.InnerTextBounds.Location - this.scrollOffset + new Drawing.Point(0, 1);
			
			if ( this.TextLayout.FindTextCursor(this.navigator.Context, out p1, out p2) )
			{
				p1 += pos;
				p2 += pos;
				
				return true;
			}
			
			return false;
		}
		

		
		public Drawing.Rectangle GetButtonBounds()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = new Drawing.Rectangle();
			
			rect.Left   = this.ActualWidth-this.margins.Right-adorner.GeometryComboRightMargin;
			rect.Right  = this.ActualWidth-adorner.GeometryComboRightMargin;
			rect.Bottom = adorner.GeometryComboBottomMargin;
			rect.Top    = this.ActualHeight-adorner.GeometryComboTopMargin;
			
			return rect;
		}

		protected override void UpdateTextLayout()
		{
			if (this.TextLayout != null)
			{
				this.realSize = this.InnerTextBounds.Size;
				this.TextLayout.Alignment  = this.ContentAlignment;
				this.TextLayout.LayoutSize = this.GetTextLayoutSize ();

				if (this.TextLayout.Text != null)
				{
					this.CursorScroll (true);
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("UpdateTextLayout Failed."));
			}
		}
		
		protected override Drawing.Size GetTextLayoutSize()
		{
			return new Drawing.Size(AbstractTextField.Infinity, this.realSize.Height);
		}

		protected virtual void InitializeMargins()
		{
		}

		
		protected static void HandleFlashTimer(object source)
		{
			//	G�re le temps �coul� pour faire clignoter un curseur.
			TextField.showCursor = !TextField.showCursor;
			
			if ( TextField.blinking != null )
			{
				TextField.blinking.FlashCursor();
			}
		}
		
		
		protected void FlashCursor()
		{
			//	Fait clignoter le curseur.
			this.Invalidate();
		}

		protected void ResetCursor()
		{
			//	Allume le curseur au prochain affichage.
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
			//	Gestion d'un �v�nement.
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
						if ( !message.IsRightButton )
						{
							this.EnableScroll(this.lastMousePos);
							this.navigator.ProcessMessage(message, pos);
						}
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
						if ( !message.IsRightButton )
						{
							this.SetEngaged(false);
							this.navigator.ProcessMessage(message, pos);
						}
						this.mouseDown = false;
						message.Consumer = this;
						if ( message.IsRightButton )
						{
							this.ShowContextMenu(true);
						}
					}
					break;
				
				case MessageType.KeyDown:
					if ( this.ProcessKeyDown(message, pos) )
					{
						message.Consumer = this;
					}
					else
					{
						switch ( message.KeyCode )
						{
							case KeyCode.Home:
							case KeyCode.End:
							case KeyCode.ArrowLeft:
							case KeyCode.ArrowRight:
								message.Consumer = this;
								break;
							
							case KeyCode.Escape:
								if (this.RejectEdition () && this.SwallowEscape)
								{
									message.Consumer = this;
									message.Swallowed = true;
								}
								break;
							
							case KeyCode.Return:
								if (this.AcceptEdition () && this.SwallowReturn)
								{
									message.Consumer = this;
									message.Swallowed = true;
								}
								break;
								
							case KeyCode.ContextualMenu:
								this.ShowContextMenu(false);
								message.Consumer = this;
								break;
						}
					}
					break;
				
				case MessageType.KeyPress:
					if ( this.ProcessKeyPress(message, pos) )
					{
						message.Consumer = this;
					}
					break;
			}
		}
		
		protected virtual bool ProcessMouseDown(Message message, Drawing.Point pos)
		{
			if ( !message.IsRightButton )
			{
				this.navigator.ProcessMessage(message, pos);
			}
			
			if ( this.AutoSelectOnFocus && !this.KeyboardFocus )
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
				//	Un clic dans la ligne �ditable doit mettre le focus sur celle-ci, quel que
				//	soit le type de gestion de focus actif (AutoFocus, etc.).
				
				this.Focus();
			}

			if (this.IsModal)
			{
				if (this.Client.Bounds.Contains (pos) == false)
				{
					this.DefocusAndAcceptOrReject ();
				}
			}
			
			return true;
		}
		
		protected virtual bool ProcessKeyDown(Message message, Drawing.Point pos)
		{
			//	Gestion d'une touche press�e avec KeyDown dans le texte.
			return this.navigator.ProcessMessage(message, pos);
		}
		
		protected virtual bool ProcessKeyPress(Message message, Drawing.Point pos)
		{
			//	Gestion d'une touche press�e avec KeyPress dans le texte.
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


		#region ContextMenu
		protected void ShowContextMenu(bool mouseBased)
		{
			this.contextMenu = new VMenu();
			this.contextMenu.Host = this;
			this.contextMenu.Accepted += new EventHandler(this.HandleContextMenuAccepted);
			this.contextMenu.Rejected += new EventHandler(this.HandleContextMenuRejected);

			MenuItem mi;
			bool sel = (this.TextNavigator.CursorFrom != this.TextNavigator.CursorTo);

			mi = new MenuItem();
			mi.CommandObject = ApplicationCommands.Cut;
			mi.Name = "Cut";
			mi.Text = Res.Strings.AbstractTextField.Menu.Cut;
			mi.Enable = sel;
			this.contextMenu.Items.Add(mi);

			mi = new MenuItem();
			mi.CommandObject = ApplicationCommands.Copy;
			mi.Name = "Copy";
			mi.Text = Res.Strings.AbstractTextField.Menu.Copy;
			mi.Enable = sel;
			this.contextMenu.Items.Add(mi);

			mi = new MenuItem();
			mi.CommandObject = ApplicationCommands.Paste;
			mi.Name = "Paste";
			mi.Text = Res.Strings.AbstractTextField.Menu.Paste;
			this.contextMenu.Items.Add(mi);

			mi = new MenuItem();
			mi.CommandObject = ApplicationCommands.Delete;
			mi.Name = "Delete";
			mi.Text = Res.Strings.AbstractTextField.Menu.Delete;
			mi.Enable = sel;
			this.contextMenu.Items.Add(mi);

			this.contextMenu.Items.Add(new MenuSeparator());

			mi = new MenuItem();
			mi.CommandObject = ApplicationCommands.SelectAll;
			mi.Name = "SelectAll";
			mi.Text = Res.Strings.AbstractTextField.Menu.SelectAll;
			this.contextMenu.Items.Add(mi);

			this.contextMenu.AdjustSize();
			
			Drawing.Point mouse;
			
			if (mouseBased)
			{
				mouse = this.lastMousePos;
			}
			else
			{
				Drawing.Point p1, p2;
				
				if (this.TextLayout.FindTextCursor(this.navigator.Context, out p1, out p2))
				{
					mouse = p1 + this.InnerTextBounds.Location - this.scrollOffset;
				}
				else
				{
					mouse = this.Client.Bounds.Center;
				}
			}
			
			mouse = this.MapClientToScreen(mouse);
			
			ScreenInfo si = ScreenInfo.Find(mouse);
			Drawing.Rectangle wa = si.WorkingArea;
			if ( mouse.Y-this.contextMenu.ActualHeight < wa.Bottom )
			{
				mouse.Y = wa.Bottom+this.contextMenu.ActualHeight;
			}

			this.contextMenu.ShowAsContextMenu(this, mouse);
		}
		
		protected void DisposeContextMenu()
		{
			if (this.contextMenu != null)
			{
				this.contextMenu.Accepted -= new EventHandler(this.HandleContextMenuAccepted);
				this.contextMenu.Rejected -= new EventHandler(this.HandleContextMenuRejected);
				this.contextMenu.Dispose ();
				this.contextMenu = null;
			}
		}

		private void HandleContextMenuAccepted(object sender)
		{
			this.DisposeContextMenu ();
			this.Invalidate ();
		}
		
		private void HandleContextMenuRejected(object sender)
		{
			this.DisposeContextMenu ();
			this.Invalidate ();
		}
		#endregion

		
		private void HandleNavigatorAboutToChange(object sender)
		{
			this.StartEdition ();
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

		private void HandleNavigatorStyleChanged(object sender)
		{
			this.CursorScroll(true);
		}

		
		protected virtual void HandleFocused()
		{
			System.Diagnostics.Debug.WriteLine ("AbstractTextField focused");
			TextField.blinking = this;
			this.ResetCursor();
			
			if ( this.AutoSelectOnFocus )
			{
				CancelEventArgs cancelEvent = new CancelEventArgs();
				this.OnAutoSelecting(cancelEvent);
				
				if ( !cancelEvent.Cancel )
				{
					this.SelectAll();
				}
			}
			
			if ( this.AutoEraseOnFocus )
			{
				CancelEventArgs cancelEvent = new CancelEventArgs();
				this.OnAutoErasing(cancelEvent);
				
				if ( !cancelEvent.Cancel )
				{
					this.Text = "";
				}
			}
			
			this.Invalidate ();
		}

		protected virtual void HandleDefocused()
		{
			System.Diagnostics.Debug.WriteLine ("AbstractTextField de-focused (KeyboardFocus="+this.KeyboardFocus+")");
			
			TextField.blinking = null;
			
			if (this.KeyboardFocus == false)
			{
				this.DefocusAndAcceptOrReject ();
			}
			
			this.Invalidate ();
		}

		private void DefocusAndAcceptOrReject()
		{
			switch (this.DefocusAction)
			{
				case DefocusAction.AcceptEdition:
					this.AcceptEdition ();
					break;

				case DefocusAction.RejectEdition:
					this.RejectEdition ();
					break;

				case DefocusAction.Modal:
				case DefocusAction.AutoAcceptOrRejectEdition:
					if (this.IsValid)
					{
						this.AcceptEdition ();
					}
					else
					{
						this.RejectEdition ();
					}
					break;

				case DefocusAction.None:
					break;

				default:
					throw new System.NotImplementedException (string.Format ("DefocusAction.{0} not implemented.", this.DefocusAction));
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

		private void HandleIsFocusedChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			System.Diagnostics.Debug.Assert (Helpers.VisualTree.GetOpletQueue (this) == this.navigator.OpletQueue);
			
			bool focused = (bool) e.NewValue;
			
			if (focused)
			{
				this.HandleFocused ();
			}
			else
			{
				this.HandleDefocused ();
			}
		}

		protected override void OnTextChanged()
		{
			this.UpdateButtonVisibility ();
			
			int from = this.CursorFrom;
			int to   = this.CursorTo;
			
			//	En r�affectant les positions de curseurs, on force implicitement une v�rification sur
			//	les positions maximales tol�r�es (gr�ce � TextNavigator).
			this.navigator.SetCursors(from, to);
			
			//	G�n�re un �v�nement pour dire que le texte a chang� (tout changement).
			this.ResetCursor();
			this.CursorScroll(false);
			this.Invalidate();
			
			base.OnTextChanged();
		}
		
		protected override void OnAdornerChanged()
		{
			base.OnAdornerChanged();
			this.UpdateGeometry();
		}
		
		protected override void OnCultureChanged()
		{
			base.OnCultureChanged ();
			this.SelectAll ();
		}

		protected override void OnTextDefined()
		{
			base.OnTextDefined ();
			this.has_edited_text = false;
		}

		protected override void OnKeyboardFocusChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnKeyboardFocusChanged (e);
			
			this.UpdateButtonVisibility ();
		}

		
		protected virtual void OnTextDeleted()
		{
			this.OnTextChanged();

			EventHandler handler = (EventHandler) this.GetUserEventHandler("TextDeleted");
			if (handler != null)
			{
				handler(this);
			}
		}

		protected virtual void OnTextInserted()
		{
			this.OnTextChanged();

			EventHandler handler = (EventHandler) this.GetUserEventHandler("TextInserted");
			if (handler != null)
			{
				handler(this);
			}
		}

		protected virtual void OnTextEdited()
		{
			if (this.has_edited_text == false)
			{
				System.Diagnostics.Debug.WriteLine ("Text edited. has_edited_text = true");
				this.has_edited_text = true;
				
				this.UpdateButtonVisibility ();
			}

			EventHandler handler = (EventHandler) this.GetUserEventHandler("TextEdited");
			if (handler != null)
			{
				handler(this);
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
				EventHandler handler = (EventHandler) this.GetUserEventHandler("SelectionChanged");
				if (handler != null)
				{
					handler(this);
				}
			}

			if ( !silent && this.navigator.Context.CursorFrom == this.navigator.Context.CursorTo )
			{
				EventHandler handler = (EventHandler) this.GetUserEventHandler("CursorChanged");
				if (handler != null)
				{
					handler(this);
				}
			}

			this.lastCursorFrom = this.navigator.Context.CursorFrom;
			this.lastCursorTo   = this.navigator.Context.CursorTo;
		}
		
		protected virtual void OnReadOnlyChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ReadOnlyChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		protected virtual void OnAutoSelecting(CancelEventArgs e)
		{
			EventHandler<CancelEventArgs> handler = (EventHandler<CancelEventArgs>) this.GetUserEventHandler("AutoSelecting");
			if (handler != null)
			{
				handler(this, e);
			}
		}
		
		protected virtual void OnAutoErasing(CancelEventArgs e)
		{
			EventHandler<CancelEventArgs> handler = (EventHandler<CancelEventArgs>) this.GetUserEventHandler("AutoErasing");
			if (handler != null)
			{
				handler(this, e);
			}
		}
		
		
		protected virtual void OnEditionStarted()
		{
			System.Diagnostics.Debug.WriteLine ("Started Edition");

			EventHandler handler = (EventHandler) this.GetUserEventHandler("EditionStarted");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnEditionAccepted()
		{
			System.Diagnostics.Debug.WriteLine ("Accepted Edition");

			EventHandler handler = (EventHandler) this.GetUserEventHandler("EditionAccepted");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		protected virtual void OnEditionRejected()
		{
			System.Diagnostics.Debug.WriteLine ("Rejected Edition");

			EventHandler handler = (EventHandler) this.GetUserEventHandler("EditionRejected");
			if (handler != null)
			{
				handler(this);
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

				if ( ratio <= zone )  // curseur trop � gauche ?
				{
					this.scrollOffset.X -= (zone-ratio)*this.realSize.Width;
					this.scrollOffset.X = System.Math.Max(this.scrollOffset.X, 0.0);
				}

				if ( ratio >= 1.0-zone )  // curseur trop � droite ?
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
			//	D�cale le texte vers la droite (+) ou la gauche (-), lorsque la
			//	souris d�passe pendant une s�lection.
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
			//	D�cale le texte vers le haut (+) ou le bas (-), lorsque la
			//	souris d�passe pendant une s�lection.
		}

		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if ( AbstractTextField.flashTimerStarted == false )
			{
				//	Il faut enregistrer le timer; on ne peut pas le faire avant que le
				//	premier TextField ne s'affiche, car sinon les WinForms semblent se
				//	m�langer les pinceaux :
				TextField.flashTimer = new Timer();
				TextField.flashTimer.TimeElapsed += new EventHandler(TextField.HandleFlashTimer);
				TextField.flashTimerStarted = true;
				
				this.ResetCursor();
			}
			
			TextLayout textLayout = this.TextLayout;
			
			//	Dessine le texte en cours d'�dition :
			System.Diagnostics.Debug.Assert(textLayout != null);
			
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			WidgetPaintState  state     = this.PaintState;
			Drawing.Point     pos       = this.InnerTextBounds.Location - this.scrollOffset + new Drawing.Point(0, this.GetBaseLineVerticalOffset ());
			Drawing.Rectangle rText     = this.InnerTextBounds;
			Drawing.Rectangle rInside   = this.Client.Bounds;
			rInside.Deflate(this.GetInternalPadding());
			Drawing.Rectangle rSaveClip = graphics.SaveClippingRectangle();
			Drawing.Rectangle rClip     = rInside;
			Drawing.Rectangle rFill     = this.Client.Bounds;
			
			if ( this.textFieldStyle == TextFieldStyle.Flat )
			{
				rFill.Deflate(1, 1);
			}
			
			if ( this.BackColor.IsTransparent )
			{
				//	Ne peint pas le fond de la ligne �ditable si celle-ci a un fond
				//	explicitement d�fini comme "transparent".
			}
			else
			{
				//	Ne reproduit pas l'�tat s�lectionn� si on peint nous-m�me le fond
				//	de la ligne �ditable.
				state &= ~WidgetPaintState.Selected;
				adorner.PaintTextFieldBackground(graphics, rFill, state, this.textFieldStyle, this.textDisplayMode, this.navigator.IsReadOnly&&!this.IsCombo);
			}
			
//			graphics.AddFilledRectangle(rText);
//			graphics.RenderSolid(Drawing.Color.FromAlphaRgb(0.6, 1, 0, 0));
			
			rClip = this.MapClientToRoot(rClip);
			graphics.SetClippingRectangle(rClip);
			
			if ( (this.KeyboardFocus && this.IsEnabled) || this.contextMenu != null )
			{
				bool visibleCursor = false;
				
				int from = System.Math.Min(this.navigator.Context.CursorFrom, this.navigator.Context.CursorTo);
				int to   = System.Math.Max(this.navigator.Context.CursorFrom, this.navigator.Context.CursorTo);
				
				if ( this.IsCombo && this.navigator.IsReadOnly )
				{
					TextLayout.SelectedArea[] areas = new TextLayout.SelectedArea[1];
					areas[0] = new TextLayout.SelectedArea();
					areas[0].Rect = rInside;
					areas[0].Rect.Deflate(1, 1);
					adorner.PaintTextSelectionBackground(graphics, areas, state, PaintTextStyle.TextField, this.textDisplayMode);
					adorner.PaintGeneralTextLayout(graphics, clipRect, pos, textLayout, (state&~WidgetPaintState.Focused)|WidgetPaintState.Selected, PaintTextStyle.TextField, this.textDisplayMode, this.BackColor);
					adorner.PaintFocusBox(graphics, areas[0].Rect);
				}
				else if ( from == to )
				{
					adorner.PaintGeneralTextLayout(graphics, clipRect, pos, textLayout, state&~WidgetPaintState.Focused, PaintTextStyle.TextField, this.textDisplayMode, this.BackColor);
					visibleCursor = TextField.showCursor && this.Window.IsFocused && !this.Window.IsSubmenuOpen;
				}
				else if (this.Window.IsFocused == false)
				{
					//	Il y a une s�lection, mais la fen�tre n'a pas le focus; on ne peint
					//	donc pas la s�lection...
					
					adorner.PaintGeneralTextLayout (graphics, clipRect, pos, textLayout, state&~WidgetPaintState.Focused, PaintTextStyle.TextField, this.textDisplayMode, this.BackColor);
					visibleCursor = false;
				}
				else
				{
					//	Un morceau de texte a �t� s�lectionn�. Peint en plusieurs �tapes :
					//	- Peint tout le texte normalement
					//	- Peint les rectangles de s�lection
					//	- Peint tout le texte en mode s�lectionn�, avec clipping
					
					TextLayout.SelectedArea[] areas = textLayout.FindTextRange(pos, from, to);
					if ( areas.Length == 0 )
					{
						adorner.PaintGeneralTextLayout(graphics, clipRect, pos, textLayout, state&~WidgetPaintState.Focused, PaintTextStyle.TextField, this.textDisplayMode, this.BackColor);
						visibleCursor = TextField.showCursor && this.Window.IsFocused && !this.Window.IsSubmenuOpen;
					}
					else
					{
						adorner.PaintGeneralTextLayout(graphics, clipRect, pos, textLayout, state&~(WidgetPaintState.Focused|WidgetPaintState.Selected), PaintTextStyle.TextField, this.textDisplayMode, this.BackColor);
					
						for ( int i=0 ; i<areas.Length ; i++ )
						{
							areas[i].Rect.Offset(0, -1);
							graphics.Align(ref areas[i].Rect);
						}
						WidgetPaintState st = state;
						if ( this.contextMenu != null )  st |= WidgetPaintState.Focused;
						adorner.PaintTextSelectionBackground(graphics, areas, st, PaintTextStyle.TextField, this.textDisplayMode);
					
						Drawing.Rectangle[] rects = new Drawing.Rectangle[areas.Length];
						for ( int i=0 ; i<areas.Length ; i++ )
						{
							rects[i] = this.MapClientToRoot(areas[i].Rect);
						}
						graphics.SetClippingRectangles(rects);

						adorner.PaintGeneralTextLayout(graphics, clipRect, pos, textLayout, (state&~WidgetPaintState.Focused)|WidgetPaintState.Selected, PaintTextStyle.TextField, this.textDisplayMode, this.BackColor);
					}
				}
				
				if ( !this.navigator.IsReadOnly && visibleCursor && this.KeyboardFocus )
				{
					//	Dessine le curseur, sauf si le menu contextuel est affich� :
					Drawing.Point p1, p2;
					if ( textLayout.FindTextCursor(this.navigator.Context, out p1, out p2) )
					{
						p1 += pos;
						p2 += pos;
						adorner.PaintTextCursor(graphics, p1, p2, true);
					}
				}
			}
			else
			{
				//	On n'a pas le focus...

				adorner.PaintGeneralTextLayout(graphics, clipRect, pos, textLayout, state&~WidgetPaintState.Focused, PaintTextStyle.TextField, this.textDisplayMode, this.BackColor);
			}

			graphics.RestoreClippingRectangle(rSaveClip);
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateGeometry ();
		}

		protected sealed override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
//			this.UpdateGeometry ();
		}
		
		protected virtual void UpdateGeometry()
		{
			this.UpdateButtonGeometry();
			this.OnCursorChanged(true);
		}
		
		protected virtual void UpdateButtonGeometry()
		{
		}
		
		protected virtual void UpdateButtonVisibility()
		{
		}
		
		protected virtual bool UpdateMouseCursor(Drawing.Point pos)
		{
			if (this.Client.Bounds.Contains(pos))
			{
				if ((pos.X >= this.margins.Left) &&
					(pos.X <= this.Client.Size.Width - this.margins.Right) &&
					(pos.Y >= this.margins.Bottom) &&
					(pos.Y <= this.Client.Size.Height - this.margins.Top) &&
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
		
		public override Drawing.Margins GetShapeMargins()
		{
			return Widgets.Adorners.Factory.Active.GeometryTextFieldShapeMargins;
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
		
		private class Dispatcher
		{
			public Dispatcher(AbstractTextField host)
			{
				this.host = host;
			}
			
			
			[Command ("Copy")]		public void CommandCopy(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				string value = this.host.Selection;
				
				if (value == "")
				{
					value = this.host.Text;
				}
				
				Clipboard.WriteData data = new Clipboard.WriteData ();
				
				data.WriteTextLayout (value);
				data.WriteHtmlFragment (value);
				Clipboard.SetData (data);
				
				e.Executed = true;
			}
			
			[Command ("Cut")]		public void CommandCut(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				string value = this.host.Selection;
				
				if (value == "")
				{
					value = this.host.Text;
					this.host.SelectAll ();
				}
				
				Clipboard.WriteData data = new Clipboard.WriteData ();
				
				data.WriteTextLayout (value);
				data.WriteHtmlFragment (value);
				Clipboard.SetData (data);
				
				this.host.TextNavigator.DeleteSelection ();
				this.host.SimulateEdited ();
				
				e.Executed = true;
			}
			
			[Command ("Delete")]	public void CommandDelete(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				string value = this.host.Selection;
				
				if (value == "")
				{
					this.host.SelectAll ();
				}
				
				this.host.TextNavigator.DeleteSelection ();
				this.host.SimulateEdited ();
				
				e.Executed = true;
			}
			
			[Command ("SelectAll")]	public void CommandSelectAll(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				this.host.SelectAll ();
				
				e.Executed = true;
			}
			
			[Command ("Paste")]		public void CommandPaste(CommandDispatcher dispatcher, CommandEventArgs e)
			{
				Clipboard.ReadData data = Clipboard.GetData ();
				
				string text_layout = data.ReadTextLayout ();
				string html        = null;
				
				if (text_layout != null)
				{
					html = text_layout;
				}
				else
				{
					html = data.ReadHtmlFragment ();
					
					if (html != null)
					{
						html = Clipboard.ConvertHtmlToSimpleXml (html);
					}
					else
					{
						html = TextLayout.ConvertToTaggedText (data.ReadText ());
					}
				}
				
				if ((html != null) &&
					(html.Length > 0))
				{
					if (this.host.TextFieldStyle != TextFieldStyle.Multi)
					{
						html = html.Replace ("<br/>", " ");
					}
					
					this.host.Selection = html;
					this.host.SimulateEdited ();
					
					e.Executed = true;
				}
			}
			
			
			private AbstractTextField			host;
		}
		

		public event EventHandler				TextEdited
		{
			add
			{
				this.AddUserEventHandler("TextEdited", value);
			}
			remove
			{
				this.RemoveUserEventHandler("TextEdited", value);
			}
		}

		public event EventHandler				TextInserted
		{
			add
			{
				this.AddUserEventHandler("TextInserted", value);
			}
			remove
			{
				this.RemoveUserEventHandler("TextInserted", value);
			}
		}

		public event EventHandler				TextDeleted
		{
			add
			{
				this.AddUserEventHandler("TextDeleted", value);
			}
			remove
			{
				this.RemoveUserEventHandler("TextDeleted", value);
			}
		}

		public event EventHandler				SelectionChanged
		{
			add
			{
				this.AddUserEventHandler("SelectionChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("SelectionChanged", value);
			}
		}

		public event EventHandler				CursorChanged
		{
			add
			{
				this.AddUserEventHandler("CursorChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("CursorChanged", value);
			}
		}

		public event EventHandler				ReadOnlyChanged
		{
			add
			{
				this.AddUserEventHandler("ReadOnlyChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ReadOnlyChanged", value);
			}
		}

		
		public event EventHandler<CancelEventArgs> AutoSelecting
		{
			add
			{
				this.AddUserEventHandler("AutoSelecting", value);
			}
			remove
			{
				this.RemoveUserEventHandler("AutoSelecting", value);
			}
		}

		public event EventHandler<CancelEventArgs> AutoErasing
		{
			add
			{
				this.AddUserEventHandler("AutoErasing", value);
			}
			remove
			{
				this.RemoveUserEventHandler("AutoErasing", value);
			}
		}

		
		public event EventHandler				EditionStarted
		{
			add
			{
				this.AddUserEventHandler("EditionStarted", value);
			}
			remove
			{
				this.RemoveUserEventHandler("EditionStarted", value);
			}
		}

		public event EventHandler				EditionAccepted
		{
			add
			{
				this.AddUserEventHandler("EditionAccepted", value);
			}
			remove
			{
				this.RemoveUserEventHandler("EditionAccepted", value);
			}
		}

		public event EventHandler				EditionRejected
		{
			add
			{
				this.AddUserEventHandler("EditionRejected", value);
			}
			remove
			{
				this.RemoveUserEventHandler("EditionRejected", value);
			}
		}
		
		
		internal static readonly double			TextMargin = 2;
		internal static readonly double			FrameMargin = 2;
		internal static readonly double			Infinity = 1000000;
		
		private bool							autoSelectOnFocus;
		private bool							autoEraseOnFocus;
		protected Drawing.Margins				margins;
		protected Drawing.Size					realSize;
		protected Drawing.Point					scrollOffset;
		protected bool							mouseDown = false;
		protected bool							scrollLeft = false;
		protected bool							scrollRight = false;
		protected bool							scrollBottom = false;
		protected bool							scrollTop = false;
		protected Drawing.Point					lastMousePos;
		protected TextFieldStyle				textFieldStyle = TextFieldStyle.Normal;
		protected double						scrollZone = 0.5;
		protected TextDisplayMode				textDisplayMode = TextDisplayMode.Default;
		private DefocusAction					defocus_action;
		private ShowCondition					button_show_condition;
		protected string initial_text;
		protected TextDisplayMode				initial_text_display_mode;
		private bool							is_editing;
		private bool							is_modal;
		protected bool							has_edited_text;
		protected bool							swallow_return;
		protected bool							swallow_escape;
		
		private TextNavigator					navigator;
		private int								lastCursorFrom = -1;
		private int								lastCursorTo = -1;
		
		private Behaviors.CopyPasteBehavior		copyPasteBehavior;
		
		private static Timer					flashTimer;
		private static bool						flashTimerStarted = false;
		private static bool						showCursor = true;
		
		protected static AbstractTextField		blinking;
		protected VMenu							contextMenu;
	}
}
