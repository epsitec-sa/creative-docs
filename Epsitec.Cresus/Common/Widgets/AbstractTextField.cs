//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets.Behaviors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextField implémente la ligne éditable, tout en permettant
	/// aussi de réaliser l'équivalent de la ComboBox Windows.
	/// </summary>
	public abstract partial class AbstractTextField : Widget, Types.IReadOnly
	{
		public AbstractTextField()
		{
			this.AutoEngage = false;
			this.AutoFocus  = true;
			this.AutoRepeat = true;
			this.AutoDoubleClick = true;

			this.InternalState |= WidgetInternalState.Focusable;
			this.InternalState |= WidgetInternalState.Engageable;

			this.MouseCursor = MouseCursor.AsIBeam;

			this.InitializeMargins ();
			this.CreateTextLayout ();

			this.navigator = new TextNavigator (this, base.TextLayout);
			
			this.navigator.AboutToChange  += this.HandleNavigatorAboutToChange;
			this.navigator.TextDeleted    += this.HandleNavigatorTextDeleted;
			this.navigator.TextInserted   += this.HandleNavigatorTextInserted;
			this.navigator.CursorScrolled += this.HandleNavigatorCursorScrolled;
			this.navigator.CursorChanged  += this.HandleNavigatorCursorChanged;
			this.navigator.StyleChanged   += this.HandleNavigatorStyleChanged;

			this.copyPasteBehavior = new Behaviors.CopyPasteBehavior (this);
//?			this.OnCursorChanged (true);

			this.CreateCommandController ();
			this.IsFocusedChanged += this.HandleIsFocusedChanged;
		}

		protected AbstractTextField(TextFieldStyle textFieldStyle)
			: this ()
		{
			this.textFieldStyle = textFieldStyle;
		}

		public AbstractTextField(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		#region Static Initializer

		static AbstractTextField()
		{
			Types.DependencyPropertyMetadata metadataAlign = Visual.ContentAlignmentProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataHeight = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			double height = Widget.DefaultFontHeight + 2*(AbstractTextField.TextMargin+AbstractTextField.FrameMargin);

			metadataAlign.DefineDefaultValue (Drawing.ContentAlignment.TopLeft);
			metadataHeight.DefineDefaultValue (height);

			Visual.ContentAlignmentProperty.OverrideMetadata (typeof (AbstractTextField), metadataAlign);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (AbstractTextField), metadataHeight);

			TextLayout.DefineLocalColor (".hint", new Drawing.RichColor (0.6));
		}

		#endregion

		public TextNavigator					TextNavigator
		{
			get
			{
				return this.navigator;
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
						//	dans la zone éditable; si la souris se trouve sur un bouton, on ne fait rien.

						this.UpdateMouseCursor (this.MapRootToClient (Message.CurrentState.LastPosition));
					}

					this.OnReadOnlyChanged ();
				}
			}
		}

		public bool								IsFormattedText
		{
			get;
			set;
		}

		public bool								IsMultilingualText
		{
			get;
			set;
		}

		public bool								AcceptsNullValue
		{
			get
			{
				return this.acceptsNullValue;
			}
			set
			{
				this.acceptsNullValue = value;
			}
		}

		public bool								IsModal
		{
			get
			{
				return this.isModal;
			}
			set
			{
				if (this.isModal != value)
				{
					this.isModal = value;

					Window window = this.Window;

					if ((window != null) &&
						(window.ModalWidget == this))
					{
						window.ModalWidget = null;
					}
				}
			}
		}

		public bool								IsPassword
		{
			get
			{
				return this.isPassword;
			}
			set
			{
				if (this.isPassword != value)
				{
					this.isPassword = value;
					this.Invalidate ();
				}
			}
		}

		public char								PasswordReplacementCharacter
		{
			get
			{
				return (char) this.GetValue (AbstractTextField.PasswordReplacementCharacterProperty);
			}
			set
			{
				if (value == TextLayout.CodeNull)
				{
					this.ClearValue (AbstractTextField.PasswordReplacementCharacterProperty);
				}
				else
				{
					this.SetValue (AbstractTextField.PasswordReplacementCharacterProperty, value);
				}
			}
		}

		public string							HintText
		{
			get
			{
				return (string) this.GetValue (AbstractTextField.HintTextProperty);
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (AbstractTextField.HintTextProperty);
				}
				else
				{
					this.SetValue (AbstractTextField.HintTextProperty, value);
				}
			}
		}

		public System.Func<string, string>		HintComparisonConverter
		{
			get;
			set;
		}

		public int								HintOffset
		{
			get
			{
				return (int) this.GetValue (AbstractTextField.HintOffsetProperty);
			}
		}

		public bool								IsTextEmpty
		{
			get
			{
				return this.navigator.IsEmpty;
			}
		}

		public bool								IsTextNull
		{
			get
			{
				return this.Text == ResourceBundle.Field.Null;
			}
		}

		public bool								HasEditedText
		{
			get
			{
				return this.hasEditedText;
			}
		}

		public bool								AutoSelectOnFocus
		{
			get
			{
				return this.autoSelectOnFocus;
			}
			set
			{
				this.autoSelectOnFocus = value;
			}
		}

		public bool								AutoEraseOnFocus
		{
			get
			{
				return this.autoEraseOnFocus;
			}
			set
			{
				this.autoEraseOnFocus = value;
			}
		}

		public bool								SwallowReturnOnAcceptEdition
		{
			get
			{
				return this.swallowReturn;
			}
			set
			{
				this.swallowReturn = value;
			}
		}

		public bool								SwallowEscapeOnRejectEdition
		{
			get
			{
				return this.swallowEscape;
			}
			set
			{
				this.swallowEscape = value;
			}
		}

		public Drawing.Rectangle				InnerTextBounds
		{
			get
			{
				return this.GetInnerTextBounds (this.Client.Bounds);
			}
		}

		protected Drawing.Point					LastMousePosition
		{
			get
			{
				return this.lastMousePos;
			}
		}

		public bool								IsEditing
		{
			get
			{
				return this.isEditing;
			}
			private set
			{
				if (this.isEditing != value)
				{
					this.isEditing = value;

					if (this.isModal)
					{
						Window window = this.Window;

						if (window != null)
						{
							if ((this.isEditing) &&
								(window.ModalWidget == null))
							{
								window.ModalWidget = this;
							}
							else if ((this.isEditing == false) &&
								/**/ (window.ModalWidget == this))
							{
								window.ModalWidget = null;
							}
						}
					}
				}
			}
		}

		private bool							IsCombo
		{
			get
			{
				return this.textFieldStyle == TextFieldStyle.Combo;
			}
		}


		public bool								Autocompletion
		{
			get;
			set;
		}

		public System.Func<string, string>		AutocompletionConverter
		{
			get;
			set;
		}

		public List<string>						AutocompletionList
		{
			get
			{
				if (this.autocompletionList == null)
				{
					this.autocompletionList = new List<string> ();
				}

				return this.autocompletionList;
			}
		}


		public int								MaxLength
		{
			get
			{
				return this.navigator.MaxLength;
			}
			set
			{
				this.navigator.MaxLength = value;
			}
		}

		public TextFieldStyle					TextFieldStyle
		{
			get
			{
				return this.textFieldStyle;
			}

			protected set
			{
				if (this.textFieldStyle != value)
				{
					switch (this.textFieldStyle)
					{
						case TextFieldStyle.Normal:
						case TextFieldStyle.Simple:
						case TextFieldStyle.Flat:
							switch (value)
							{
								case TextFieldStyle.Normal:
								case TextFieldStyle.Simple:
								case TextFieldStyle.Flat:
									this.textFieldStyle = value;
									this.Invalidate ();
									return;
							}
							break;
					}

					throw new System.InvalidOperationException (string.Format ("Cannot switch from {0} to {1}.", this.textFieldStyle, value));
				}
			}
		}

		public double							ScrollZone
		{
			//	Amplitude de la zone dans laquelle le curseur provoque un scroll.
			//	Avec 0.0, le texte ne scrolle que lorsque le curseur arrive aux extrémités.
			//	Avec 1.0, le texte scrolle tout le temps (curseur au milieu).
			get
			{
				return this.scrollZone;
			}

			set
			{
				value = System.Math.Max (value, 0.0);
				value = System.Math.Min (value, 1.0);
				if (this.scrollZone != value)
				{
					this.scrollZone = value;
					this.CursorScroll (true);
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
				return (DefocusAction) this.GetValue (AbstractTextField.DefocusActionProperty);
			}
			set
			{
				if (value == DefocusAction.None)
				{
					this.ClearValue (AbstractTextField.DefocusActionProperty);
				}
				else
				{
					this.SetValue (AbstractTextField.DefocusActionProperty, value);
				}
			}
		}

		public ButtonShowCondition				ButtonShowCondition
		{
			get
			{
				return (ButtonShowCondition) this.GetValue (AbstractTextField.ButtonShowConditionProperty);
			}
			set
			{
				this.SetValue (AbstractTextField.ButtonShowConditionProperty, value);
			}
		}


		public TextFieldDisplayMode				TextDisplayMode
		{
			get
			{
				return this.textFieldDisplayMode;
			}

			set
			{
				if (this.textFieldDisplayMode != value)
				{
					this.textFieldDisplayMode = value;
					this.UpdateButtonVisibility ();
					this.UpdateButtonGeometry ();
					this.Invalidate ();
				}
			}
		}

		public TextFieldDisplayMode				InitialTextDisplayMode
		{
			get
			{
				return this.initialTextDisplayMode;
			}

			set
			{
				this.initialTextDisplayMode = value;
			}
		}

		public string							InitialText
		{
			get
			{
				return this.initialText;
			}
			set
			{
				this.initialText = value;
			}
		}

		public override double					AutoEngageDelay
		{
			get
			{
				return base.AutoEngageDelay / 2;
			}
		}


		public void ProcessCut()
		{
			this.copyPasteBehavior.ProcessCopy ();
			this.copyPasteBehavior.ProcessDelete ();
		}

		public void ProcessCopy()
		{
			this.copyPasteBehavior.ProcessCopy ();
		}

		public void ProcessPaste()
		{
			this.copyPasteBehavior.ProcessPaste ();
		}


		public override Drawing.Point GetBaseLine(double width, double height, out double ascender, out double descender)
		{
			if (this.TextLayout != null)
			{
				//	Détermine la zone rectangulaire dans laquelle le texte est
				//	affiché et utilise celle-ci comme référence pour les calculs
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

		public override Drawing.Margins GetShapeMargins()
		{
			return Widgets.Adorners.Factory.Active.GeometryTextFieldShapeMargins;
		}


		protected override double GetBaseLineVerticalOffset()
		{
			return 1.0;
		}

		public override Drawing.Margins GetInternalPadding()
		{
			return this.GetInternalPadding (this.Client.Size);
		}

		private Drawing.Margins GetInternalPadding(Drawing.Size size)
		{
			Drawing.Margins padding = this.margins;

			if (this.textFieldStyle != TextFieldStyle.Flat)
			{
				double excess = System.Math.Max ((22-size.Height)/2, 0);
				double x = System.Math.Max (1, AbstractTextField.FrameMargin-excess);
				double y = System.Math.Max (0, AbstractTextField.FrameMargin-excess);
				double m = this.IsMultilingualText ? Adorners.AbstractAdorner.MultilingualLeftPadding : 0;
				padding = padding + new Drawing.Margins (x+m, x, y, y);
			}

			return padding;
		}

		private Drawing.Rectangle GetInnerTextBounds(Drawing.Rectangle rect)
		{
			rect.Deflate (this.GetInternalPadding (rect.Size));
			rect.Deflate (AbstractTextField.TextMargin, AbstractTextField.TextMargin);
			return rect;
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
			if (disposing)
			{
				this.IsFocusedChanged -= this.HandleIsFocusedChanged;
				if (TextField.blinking == this)
				{
					if (this.navigator != null)
					{
						this.navigator.AboutToChange -= this.HandleNavigatorAboutToChange;
						this.navigator.TextDeleted -= this.HandleNavigatorTextDeleted;
						this.navigator.TextInserted -= this.HandleNavigatorTextInserted;
						this.navigator.CursorScrolled -= this.HandleNavigatorCursorScrolled;
						this.navigator.CursorChanged -= this.HandleNavigatorCursorChanged;
						this.navigator.StyleChanged -= this.HandleNavigatorStyleChanged;
					}

					TextField.blinking = null;
				}
			}

			base.Dispose (disposing);
		}


		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			if (mode != TabNavigationMode.None)
			{
				this.SelectAll ();
			}

			return base.AboutToGetFocus (dir, mode, out focus);
		}

		protected override void ModifyTextLayout(string text)
		{
			if (text.Length > this.navigator.MaxLength)
			{
				text = text.Substring (0, this.navigator.MaxLength);
			}

			base.ModifyTextLayout (text);
		}

		protected override void DisposeTextLayout()
		{
			//	Ne fait rien, on veut s'assurer que le TextLayout associé avec le
			//	TextField n'est jamais détruit du vivant du TextField.
			this.navigator.TextLayout.Text = "";
		}


		public void ClearText()
		{
			if (this.navigator.TextLayout.Text.Length > 0)
			{
				this.navigator.TextLayout.Text = "";
				this.OnTextDeleted ();
			}
		}

		public void SelectAll()
		{
			//	Sélectione tous les caractères.
			this.Cursor = 0;
			this.SelectAll (false);
		}


		public bool StartEdition()
		{
			if ((this.IsEditing == false) &&
				(this.CanStartEdition) &&
				((this.DefocusAction != DefocusAction.None) || (this.IsCombo) || (this.ButtonShowCondition != ButtonShowCondition.Never)))
			{
				this.initialText              = this.Text;
				this.initialTextDisplayMode = this.TextDisplayMode;

				this.IsEditing = true;

				if (this.textFieldDisplayMode == TextFieldDisplayMode.InheritedValue)
				{
					this.textFieldDisplayMode = TextFieldDisplayMode.OverriddenValue;
				}

				this.OnEditionStarted ();

				return true;
			}

			return false;
		}

		protected virtual bool CanStartEdition
		{
			get
			{
				return false;
			}
		}

		public virtual bool AcceptEdition()
		{
			if ((this.IsEditing) &&
				(this.CheckAcceptEdition ()))
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
			if ((this.IsEditing) &&
				(this.CheckRejectEdition ()))
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
			this.navigator.SelectAll ();
			this.OnCursorChanged (silent);
		}

		protected bool CheckAcceptEdition()
		{
			CancelEventArgs e = new CancelEventArgs ();
			this.OnAcceptingEdition (e);
			return e.Cancel == false;
		}

		protected bool CheckRejectEdition()
		{
			CancelEventArgs e = new CancelEventArgs ();
			this.OnRejectingEdition (e);
			return e.Cancel == false;
		}

		protected bool CheckBeforeDefocus()
		{
			switch (this.DefocusAction)
			{
				case DefocusAction.AcceptEdition:
					return this.CheckAcceptEdition ();

				case DefocusAction.RejectEdition:
					return this.CheckRejectEdition ();

				case DefocusAction.Modal:
				case DefocusAction.AutoAcceptOrRejectEdition:
					if (this.IsValid)
					{
						return this.CheckAcceptEdition ();
					}
					else
					{
						return this.CheckRejectEdition ();
					}

				case DefocusAction.None:
					return true;

				default:
					throw new System.NotImplementedException (string.Format ("DefocusAction.{0} not implemented.", this.DefocusAction));
			}
		}


		public void SimulateEdited()
		{
			this.OnTextEdited ();
		}


		private Drawing.Point GetTextOriginLocation()
		{
			return this.InnerTextBounds.Location - this.scrollOffset + new Drawing.Point (0, this.GetBaseLineVerticalOffset ());
		}

		public bool GetCursorPosition(out Drawing.Point p1, out Drawing.Point p2)
		{
			return this.GetCursorPosition (out p1, out p2, this.GetTextOriginLocation ());
		}

		private bool GetCursorPosition(out Drawing.Point p1, out Drawing.Point p2, Drawing.Point offset)
		{
			TextLayout        layout  = this.GetPaintTextLayout ();
			TextLayoutContext context = this.navigator.Context;

			int hintOffset = this.HintOffset;
			context.OffsetCursor (hintOffset);

			try
			{
				if (layout.FindTextCursor (context, out p1, out p2))
				{
					p1 += offset;
					p2 += offset;

					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				context.OffsetCursor (-hintOffset);
			}
		}

		internal bool DetectIndex(Epsitec.Common.Drawing.Point pos, bool select, out int index, out bool after)
		{
			TextLayout layout = this.GetPaintTextLayout ();
			
			//	We use the text as it is displayed on screen, not as it is stored
			//	internally, since we must account for possible prefixes/suffixes
			//	related to the HintText property :
			
			bool ok = layout.DetectIndex (pos, select, out index, out after);
			int len = this.Text.Length;

			index -= this.HintOffset;

			//	The index must be constrained to the text typed in by the user :
			
			if (index < 0)
			{
				index = 0;
			}
			else if (index > len)
			{
				index = len;
			}

			return ok;
		}



		public virtual Drawing.Rectangle GetButtonBounds()
		{
			//	Retourne le rectangle à utiliser pour les boutons Accept/Reject.
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = new Drawing.Rectangle ();

			rect.Left   = this.ActualWidth - this.margins.Right - adorner.GeometryComboRightMargin;
			rect.Right  = this.ActualWidth - adorner.GeometryComboRightMargin;
			rect.Bottom = adorner.GeometryComboBottomMargin;
			rect.Top    = this.ActualHeight - adorner.GeometryComboTopMargin;

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
			return new Drawing.Size (AbstractTextField.Infinity, this.realSize.Height);
		}

		protected virtual void InitializeMargins()
		{
		}


		private static void HandleFlashTimer(object source)
		{
			//	Gère le temps écoulé pour faire clignoter un curseur.
			TextField.showCursor = !TextField.showCursor;

			if (TextField.blinking != null)
			{
				TextField.blinking.FlashCursor ();
			}
		}


		private void FlashCursor()
		{
			//	Fait clignoter le curseur.
			this.Invalidate ();
		}

		private void ResetCursor()
		{
			//	Allume le curseur au prochain affichage.
			if (this.IsFocused && TextField.flashTimer != null)
			{
				double delay = SystemInformation.CursorBlinkDelay;
				TextField.flashTimer.Delay = delay;
				TextField.flashTimer.AutoRepeat = delay;
				TextField.flashTimer.Start ();  // restart du timer
				TextField.showCursor = true;  // avec le curseur visible
			}
		}

		private void ProcessAutocompletionTextDeleted()
		{
		}

		private void ProcessAutocompletionTextInserted()
		{
			if (this.Autocompletion)
			{
				string completed = this.AutocompletionSearch (this.Text);

				if (!string.IsNullOrEmpty (completed) && this.Text != completed)
				{
					int length = this.Text.Length;

					this.Text = completed;
					this.CursorFrom = this.Text.Length;
					this.CursorTo = length;
				}
			}
		}

		private string AutocompletionSearch(string searching)
		{
			searching = this.AutocompletionConvert (searching);

			foreach (string t in this.autocompletionList)
			{
				string text = this.AutocompletionConvert (t);

				if (text.StartsWith (searching))
				{
					return t;
				}
			}

			return null;
		}

		private string AutocompletionConvert(string text)
		{
			if (this.AutocompletionConverter == null)
			{
				return text;
			}
			else
			{
				return this.AutocompletionConverter (text);
			}
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	Gestion d'un événement.
			if (!this.IsReadOnly && this.copyPasteBehavior.ProcessMessage (message, pos))
			{
				return;
			}

			Shortcut shortcut = Shortcut.FromMessage (message);

			if (shortcut != null)
			{
				if (this.ShortcutHandler (shortcut))
				{
					message.Handled   = true;
					message.Swallowed = true;

					return;
				}
			}

			this.lastMousePos = pos;
			pos = this.Client.Bounds.Constrain (pos);
			pos.X -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos.Y -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos += this.scrollOffset;

			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					if (this.ProcessMouseDown (message, pos))
					{
						message.Consumer = this;
					}
					break;

				case MessageType.MouseMove:
					if (this.mouseDown)
					{
						if (!message.IsRightButton)
						{
							this.EnableScroll (this.lastMousePos);
							this.navigator.ProcessMessage (message, pos);
						}
						message.Consumer = this;
					}
					else
					{
						if (this.UpdateMouseCursor (pos))
						{
							message.Consumer = this;
						}
					}
					break;

				case MessageType.MouseUp:
					if (this.mouseDown)
					{
						if (!message.IsRightButton)
						{
							this.SetEngaged (false);
							this.navigator.ProcessMessage (message, pos);
						}
						this.mouseDown = false;
						message.Consumer = this;
						if (message.IsRightButton)
						{
							this.ShowContextMenu (true);
						}
					}
					break;

				case MessageType.KeyDown:
					if (this.ProcessKeyDown (message, pos))
					{
						message.Consumer = this;
					}
					else
					{
						switch (message.KeyCode)
						{
							case KeyCode.Home:
							case KeyCode.End:
							case KeyCode.ArrowLeft:
							case KeyCode.ArrowRight:
								if (message.IsAltPressed == false)
								{
									message.Consumer = this;
								}
								break;

							case KeyCode.Escape:
								if (message.IsNoModifierPressed)
								{
									if (this.RejectEdition () && this.SwallowEscapeOnRejectEdition)
									{
										message.Consumer = this;
										message.Swallowed = true;
									}
								}
								break;

							case KeyCode.Return:
							case KeyCode.NumericEnter:
								if (message.IsNoModifierPressed)
								{
									if (this.AcceptEdition () && this.SwallowReturnOnAcceptEdition)
									{
										message.Consumer = this;
										message.Swallowed = true;
									}
								}
								break;

							case KeyCode.ContextualMenu:
								this.ShowContextMenu (false);
								message.Consumer = this;
								break;
						}
					}
					break;

				case MessageType.KeyPress:
					if (this.ProcessKeyPress (message, pos))
					{
						message.Consumer = this;
					}
					break;
			}
		}

		protected virtual bool ProcessMouseDown(Message message, Drawing.Point pos)
		{
			if (!message.IsRightButton)
			{
				this.navigator.ProcessMessage (message, pos);
			}

			if (this.AutoSelectOnFocus && !this.KeyboardFocus)
			{
				this.SelectAll ();
				message.Swallowed = true;
			}
			else
			{
				this.mouseDown = true;
			}

			if (this.IsReadOnly == false)
			{
				//	Un clic dans la ligne éditable doit mettre le focus sur celle-ci, quel que
				//	soit le type de gestion de focus actif (AutoFocus, etc.).

				this.Focus ();
				message.CancelFocus = true;
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
			//	Gestion d'une touche pressée avec KeyDown dans le texte.
			return this.navigator.ProcessMessage (message, pos);
		}

		protected virtual bool ProcessKeyPress(Message message, Drawing.Point pos)
		{
			//	Gestion d'une touche pressée avec KeyPress dans le texte.
			return this.navigator.ProcessMessage (message, pos);
		}

		#region ScrollDirection Enumeration

		[System.Flags]
		enum ScrollDirection : byte
		{
			None = 0,
			Left = 1,
			Right = 2,
			Down = 4,
			Up = 8
		}

		#endregion

		private void CreateCommandController()
		{
			var dispatcher = new CommandDispatcher ("TextField", CommandDispatcherLevel.Secondary, CommandDispatcherOptions.AutoForwardCommands);
			var context    = new CommandContext ();

			CommandDispatcher.SetDispatcher (this, dispatcher);
			CommandContext.SetContext (this, context);

			this.commandController = new CommandController (this);

			dispatcher.RegisterController (this.commandController);
		}

		private void EnableScroll(Drawing.Point pos)
		{
			ScrollDirection   dir = ScrollDirection.None;
			Drawing.Rectangle box = Drawing.Rectangle.Deflate (this.Client.Bounds, this.GetInternalPadding ());
			
			dir |= (pos.X <= box.Left)   ? ScrollDirection.Left  : ScrollDirection.None;
			dir |= (pos.X >= box.Right)  ? ScrollDirection.Right : ScrollDirection.None;
			dir |= (pos.Y <= box.Bottom) ? ScrollDirection.Down  : ScrollDirection.None;
			dir |= (pos.Y >= box.Top)    ? ScrollDirection.Up    : ScrollDirection.None;

			this.mouseScrollDirection = dir;
			
			if (dir != ScrollDirection.None)
			{
				this.SetEngaged (true);
			}
			else
			{
				this.SetEngaged (false);
			}
		}

		private void ShowContextMenu(bool mouseBased)
		{
			CommandContext context = CommandContext.GetContext (this);

			this.contextMenu = new VMenu ();
			this.contextMenu.Host = this;
			this.contextMenu.AutoDispose = true;
			this.contextMenu.Disposed += this.HandleContextMenuDisposed;

			System.Diagnostics.Debug.Assert (context == Helpers.VisualTree.GetCommandContext (this.contextMenu));
			System.Diagnostics.Debug.Assert (CommandDispatcherChain.BuildChain (this.contextMenu).IsEmpty == false);

			bool sel = (this.TextNavigator.CursorFrom != this.TextNavigator.CursorTo);
			bool readWrite = !this.navigator.IsReadOnly;

			context.SetLocalEnable (ApplicationCommands.Cut,        sel & readWrite);
			context.SetLocalEnable (ApplicationCommands.Copy,       sel);
			context.SetLocalEnable (ApplicationCommands.Delete,     sel & readWrite);
			context.SetLocalEnable (ApplicationCommands.Bold,       sel & readWrite);
			context.SetLocalEnable (ApplicationCommands.Italic,     sel & readWrite);
			context.SetLocalEnable (ApplicationCommands.Underlined, sel & readWrite);
			context.SetLocalEnable (ApplicationCommands.SelectAll, true);
			context.SetLocalEnable (ApplicationCommands.MultilingualEdition, this.IsMultilingualText & readWrite);
			

			this.contextMenu.Items.AddItem (ApplicationCommands.Cut);
			this.contextMenu.Items.AddItem (ApplicationCommands.Copy);
			this.contextMenu.Items.AddItem (ApplicationCommands.Paste);
			this.contextMenu.Items.AddItem (ApplicationCommands.Delete);
			this.contextMenu.Items.AddSeparator ();
			this.contextMenu.Items.AddItem (ApplicationCommands.Bold);
			this.contextMenu.Items.AddItem (ApplicationCommands.Italic);
			this.contextMenu.Items.AddItem (ApplicationCommands.Underlined);
			this.contextMenu.Items.AddSeparator ();
			this.contextMenu.Items.AddItem (ApplicationCommands.SelectAll);

			if (this.IsMultilingualText)
			{
				this.contextMenu.Items.AddSeparator ();
				this.contextMenu.Items.AddItem (ApplicationCommands.MultilingualEdition);
			}

			if (this.acceptsNullValue)
			{
				context.SetLocalEnable (Res.Commands.UseDefaultValue, readWrite);

				this.contextMenu.Items.AddSeparator ();
				this.contextMenu.Items.AddItem (Res.Commands.UseDefaultValue);
			}

			this.contextMenu.AdjustSize ();

			Drawing.Point mouse;

			if (mouseBased)
			{
				mouse = this.lastMousePos;
			}
			else
			{
				Drawing.Point p1, p2;

				if (this.GetCursorPosition (out p1, out p2))
				{
					mouse = p1;
				}
				else
				{
					mouse = this.Client.Bounds.Center;
				}
			}

			mouse = this.MapClientToScreen (mouse);

			ScreenInfo si = ScreenInfo.Find (mouse);
			Drawing.Rectangle wa = si.WorkingArea;
			if (mouse.Y-this.contextMenu.ActualHeight < wa.Bottom)
			{
				mouse.Y = wa.Bottom+this.contextMenu.ActualHeight;
			}

			this.contextMenu.ShowAsContextMenu (this, mouse);
		}


		private void HandleContextMenuDisposed(object sender)
		{
			this.contextMenu.Disposed -= this.HandleContextMenuDisposed;
			this.contextMenu = null;
		}

		private void HandleNavigatorAboutToChange(object sender)
		{
			this.StartEdition ();
		}

		private void HandleNavigatorTextDeleted(object sender)
		{
			this.OnTextEdited ();
			this.OnTextDeleted ();
		}

		private void HandleNavigatorTextInserted(object sender)
		{
			this.OnTextEdited ();
			this.OnTextInserted ();
		}

		private void HandleNavigatorCursorScrolled(object sender)
		{
			this.CursorScroll (false);
		}

		private void HandleNavigatorCursorChanged(object sender)
		{
			this.OnCursorChanged (false);
			this.commandController.NotifyStyleChanged ();
		}

		private void HandleNavigatorStyleChanged(object sender)
		{
			this.CursorScroll (true);
			this.commandController.NotifyStyleChanged ();
		}


		private void HandleFocused()
		{
			TextField.blinking = this;
			this.ResetCursor ();

			if (this.AutoSelectOnFocus)
			{
				CancelEventArgs cancelEvent = new CancelEventArgs ();
				this.OnAutoSelecting (cancelEvent);

				if (!cancelEvent.Cancel)
				{
					this.SelectAll ();
				}
			}

			if (this.AutoEraseOnFocus)
			{
				CancelEventArgs cancelEvent = new CancelEventArgs ();
				this.OnAutoErasing (cancelEvent);

				if (!cancelEvent.Cancel)
				{
					this.Text = "";
				}
			}

			this.Invalidate ();
		}

		private void HandleDefocused()
		{
			TextField.blinking = null;

			if (this.KeyboardFocus == false)
			{
				this.DefocusAndAcceptOrReject ();
			}

			this.Invalidate ();
		}

		public virtual void DefocusAndAcceptOrReject()
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
			base.OnStillEngaged ();

			double amplitude = 4;

			if ((this.mouseScrollDirection & ScrollDirection.Left) != 0)
			{
				this.ScrollHorizontal (-amplitude);
			}

			if ((this.mouseScrollDirection & ScrollDirection.Right) != 0)
			{
				this.ScrollHorizontal (amplitude);
			}

			if ((this.mouseScrollDirection & ScrollDirection.Down) != 0)
			{
				this.ScrollVertical (-amplitude);
			}

			if ((this.mouseScrollDirection & ScrollDirection.Up) != 0)
			{
				this.ScrollVertical (amplitude);
			}
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

			this.commandController.NotifyIsFocusedChanged (focused);
		}

		protected override void OnTextChanged()
		{
			this.UpdateButtonVisibility ();

			int from = this.CursorFrom;
			int to   = this.CursorTo;

			//	En réaffectant les positions de curseurs, on force implicitement une vérification sur
			//	les positions maximales tolérées (grâce à TextNavigator).
			this.navigator.SetCursors (from, to);

			//	Génère un événement pour dire que le texte a changé (tout changement).
			this.ResetCursor ();
			this.CursorScroll (false);
			this.Invalidate ();

			base.OnTextChanged ();
		}

		protected override void OnAdornerChanged()
		{
			base.OnAdornerChanged ();
			this.UpdateGeometry ();
		}

		protected override void OnCultureChanged()
		{
			base.OnCultureChanged ();
			this.SelectAll ();
		}

		protected override void OnTextDefined()
		{
			base.OnTextDefined ();
			this.hasEditedText = false;
		}

		protected override void OnKeyboardFocusChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnKeyboardFocusChanged (e);

			this.UpdateButtonVisibility ();
		}


		protected virtual void OnTextDeleted()
		{
			this.ProcessAutocompletionTextDeleted ();

//-			this.OnTextChanged ();

			var handler = this.GetUserEventHandler ("TextDeleted");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnTextInserted()
		{
			this.ProcessAutocompletionTextInserted ();

//-			this.OnTextChanged ();

			var handler = this.GetUserEventHandler ("TextInserted");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnTextEdited()
		{
			if (this.hasEditedText == false)
			{
				this.hasEditedText = true;

				this.UpdateButtonVisibility ();
			}

			var handler = this.GetUserEventHandler ("TextEdited");

			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnMultilingualEditionCalled()
		{
			var handler = this.GetUserEventHandler ("MultilingualEditionCalled");

			if (handler != null)
			{
				handler (this);
			}
		}


		protected virtual void OnCursorChanged(bool silent)
		{
			this.ResetCursor ();
			this.Invalidate ();

			System.Diagnostics.Debug.Assert (this.navigator != null);
			
			if (this.navigator == null)
				return;

			if ((this.navigator.Context.HasSelection) ||
				(this.lastHasSelection))
			{
				var handler = this.GetUserEventHandler ("SelectionChanged");
				
				if (handler != null)
				{
					handler (this);
				}
			}

			if ((!silent) &&
				(!this.navigator.Context.HasSelection))
			{
				var handler = this.GetUserEventHandler ("CursorChanged");
				if (handler != null)
				{
					handler (this);
				}
			}

			this.lastHasSelection = this.navigator.Context.HasSelection;
		}

		protected virtual void OnReadOnlyChanged()
		{
			var handler = this.GetUserEventHandler ("ReadOnlyChanged");
			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnAutoSelecting(CancelEventArgs e)
		{
			EventHandler<CancelEventArgs> handler = this.GetUserEventHandler<CancelEventArgs> ("AutoSelecting");
			if (handler != null)
			{
				handler (this, e);
			}
		}

		protected virtual void OnAutoErasing(CancelEventArgs e)
		{
			EventHandler<CancelEventArgs> handler = this.GetUserEventHandler<CancelEventArgs> ("AutoErasing");
			if (handler != null)
			{
				handler (this, e);
			}
		}


		protected virtual void OnEditionStarted()
		{
			this.RaiseUserEvent ("EditionStarted");
		}

		protected virtual void OnEditionAccepted()
		{
			//	OnEditionAccepted est appelé après que l'édition ait été validée et acceptée.
			this.RaiseUserEvent ("EditionAccepted");
		}

		protected virtual void OnEditionRejected()
		{
			this.RaiseUserEvent ("EditionRejected");
		}

		protected virtual void OnAcceptingEdition(CancelEventArgs e)
		{
			//	OnAcceptingEdition est appelé pendant la phase d'acceptation; l'événement passe une instance de CancelEventArgs
			//	qui permet à ceux qui écoutent l'événement de faire un e.Cancel=true pour annuler l'opération en cours (donc
			//	refuser l'acceptation).
			this.RaiseUserEvent ("AcceptingEdition", e);
		}

		protected virtual void OnRejectingEdition(CancelEventArgs e)
		{
			this.RaiseUserEvent ("RejectingEdition", e);
		}


		protected void CursorScroll(bool force)
		{
			//	Calcule le scrolling pour que le curseur soit visible.
			if (this.TextLayout == null)
				return;
			if (this.mouseDown)
				return;
			if (this.navigator == null)
				return;

			Drawing.Point p1, p2;
			if (this.GetCursorPosition (out p1, out p2, Drawing.Point.Zero))
			{
				Drawing.Rectangle cursor = new Drawing.Rectangle (p1, p2);
				this.CursorScrollText (cursor, force);
			}
		}

		protected virtual void CursorScrollText(Drawing.Rectangle cursor, bool force)
		{
			Drawing.Point end = this.TextLayout.FindTextEnd ();

			if (this.TextLayout.TotalLineCount == 1)
			{
				Drawing.Point linePos;

				double lineAscender;
				double lineDescender;
				double lineWidth;

				this.TextLayout.GetLineGeometry (0, out linePos, out lineAscender, out lineDescender, out lineWidth);

				if (lineWidth-this.scrollOffset.X < this.realSize.Width)
				{
					force = true;
				}
			}

			if (force)
			{
				double offset = cursor.Right;
				offset += this.realSize.Width/2;
				offset  = System.Math.Min (offset, end.X);
				offset -= this.realSize.Width;
				offset  = System.Math.Max (offset, 0);
				this.scrollOffset.X = offset;
			}
			else
			{
				double ratio = (cursor.Right-this.scrollOffset.X)/this.realSize.Width;  // 0..1
				double zone = this.scrollZone*0.5;

				if (ratio <= zone)  // curseur trop à gauche ?
				{
					this.scrollOffset.X -= (zone-ratio)*this.realSize.Width;
					this.scrollOffset.X = System.Math.Max (this.scrollOffset.X, 0.0);
				}

				if (ratio >= 1.0-zone)  // curseur trop à droite ?
				{
					this.scrollOffset.X += (ratio-(1.0-zone))*this.realSize.Width;
					double max = System.Math.Max (end.X-this.realSize.Width, 0.0);
					this.scrollOffset.X = System.Math.Min (this.scrollOffset.X, max);
				}
			}

			this.scrollOffset.Y = 0;
		}

		protected virtual void ScrollHorizontal(double dist)
		{
			//	Décale le texte vers la droite (+) ou la gauche (-), lorsque la
			//	souris dépasse pendant une sélection.
			if (this.textFieldStyle == TextFieldStyle.Multiline)
			{
				return;
			}

			this.scrollOffset.X += dist;
			Drawing.Point end = this.TextLayout.FindTextEnd ();
			double max = System.Math.Max (end.X-this.realSize.Width, 0.0);
			this.scrollOffset.X = System.Math.Max (this.scrollOffset.X, 0.0);
			this.scrollOffset.X = System.Math.Min (this.scrollOffset.X, max);
			this.Invalidate ();

			Drawing.Point pos = this.lastMousePos;
			pos = this.Client.Bounds.Constrain (pos);
			pos.X -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos.Y -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos += this.scrollOffset;
			this.navigator.MouseMoveMessage (pos);
		}

		protected virtual void ScrollVertical(double dist)
		{
			//	Décale le texte vers le haut (+) ou le bas (-), lorsque la
			//	souris dépasse pendant une sélection.
		}


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if (AbstractTextField.flashTimerStarted == false)
			{
				//	Il faut enregistrer le timer; on ne peut pas le faire avant que le
				//	premier TextField ne s'affiche, car sinon les WinForms semblent se
				//	mélanger les pinceaux :

				TextField.flashTimer = new Timer ();
				TextField.flashTimer.TimeElapsed += TextField.HandleFlashTimer;
				TextField.flashTimerStarted = true;

				this.ResetCursor ();
			}

			IAdorner adorner = Widgets.Adorners.Factory.Active;

			WidgetPaintState  state     = this.GetPaintState () & ~WidgetPaintState.Selected;
			Drawing.Point     pos       = this.GetTextOriginLocation ();
			Drawing.Rectangle rText     = this.InnerTextBounds;
			Drawing.Rectangle rInside   = this.Client.Bounds;

			rInside.Deflate (this.GetInternalPadding ());

			Drawing.Rectangle rSaveClip = graphics.SaveClippingRectangle ();
			Drawing.Rectangle rClip     = this.MapClientToRoot (rInside);
			Drawing.Rectangle rFill     = this.Client.Bounds;

			if (this.textFieldStyle == TextFieldStyle.Flat)
			{
				rFill.Deflate (1, 1);
			}

			this.PaintTextFieldBackground (graphics, adorner, state, rFill, pos);

			graphics.SetClippingRectangle (rClip);
			this.PaintTextFieldText (graphics, adorner, state, clipRect, rInside, pos);
			graphics.RestoreClippingRectangle (rSaveClip);
		}

		protected virtual void PaintTextFieldBackground(Drawing.Graphics graphics, IAdorner adorner, WidgetPaintState state, Drawing.Rectangle fill, Drawing.Point pos)
		{
			if (this.BackColor.IsTransparent)
			{
				//	Ne peint pas le fond de la ligne éditable si celle-ci a un fond
				//	explicitement défini comme "transparent".
			}
			else
			{
				//	Ne reproduit pas l'état sélectionné si on peint nous-même le fond
				//	de la ligne éditable.
				state &= ~WidgetPaintState.Selected;
				adorner.PaintTextFieldBackground (graphics, fill, state, this.textFieldStyle, this.textFieldDisplayMode, this.navigator.IsReadOnly&&!this.IsCombo, this.IsMultilingualText);
			}
		}

		protected virtual void PaintTextFieldText(Drawing.Graphics graphics, IAdorner adorner, WidgetPaintState state, Drawing.Rectangle clipRect, Drawing.Rectangle rInside, Drawing.Point pos)
		{
			TextLayout        layout  = this.GetPaintTextLayout ();
			TextLayoutContext context = new TextLayoutContext (this.navigator.Context);

			context.OffsetCursor (this.HintOffset);

			if ((this.KeyboardFocus && this.IsEnabled) || this.contextMenu != null)
			{
				bool visibleCursor = false;

				int from = System.Math.Min (context.CursorFrom, context.CursorTo);
				int to   = System.Math.Max (context.CursorFrom, context.CursorTo);

				if (this.IsCombo && this.navigator.IsReadOnly)
				{
					TextLayout.SelectedArea[] areas = new TextLayout.SelectedArea[1];
					areas[0] = new TextLayout.SelectedArea ();
					areas[0].Rect = rInside;
					areas[0].Rect.Deflate (1, 1);
					adorner.PaintTextSelectionBackground (graphics, areas, state, PaintTextStyle.TextField, this.textFieldDisplayMode);
					adorner.PaintGeneralTextLayout (graphics, clipRect, pos, layout, (state&~WidgetPaintState.Focused)|WidgetPaintState.Selected, PaintTextStyle.TextField, this.textFieldDisplayMode, this.BackColor);
					adorner.PaintFocusBox (graphics, areas[0].Rect);
				}
				else if (from == to)
				{
					adorner.PaintGeneralTextLayout (graphics, clipRect, pos, layout, state&~WidgetPaintState.Focused, PaintTextStyle.TextField, this.textFieldDisplayMode, this.BackColor);
					visibleCursor = TextField.showCursor && this.Window.IsFocused && !this.Window.IsSubmenuOpen;
				}
				else if (this.Window.IsFocused == false)
				{
					//	Il y a une sélection, mais la fenêtre n'a pas le focus; on ne peint
					//	donc pas la sélection...

					adorner.PaintGeneralTextLayout (graphics, clipRect, pos, layout, state&~WidgetPaintState.Focused, PaintTextStyle.TextField, this.textFieldDisplayMode, this.BackColor);
					visibleCursor = false;
				}
				else
				{
					//	Un morceau de texte a été sélectionné. Peint en plusieurs étapes :
					//	- Peint tout le texte normalement
					//	- Peint les rectangles de sélection
					//	- Peint tout le texte en mode sélectionné, avec clipping

					TextLayout.SelectedArea[] areas = layout.FindTextRange (pos, from, to);
					if (areas.Length == 0)
					{
						adorner.PaintGeneralTextLayout (graphics, clipRect, pos, layout, state&~WidgetPaintState.Focused, PaintTextStyle.TextField, this.textFieldDisplayMode, this.BackColor);
						visibleCursor = TextField.showCursor && this.Window.IsFocused && !this.Window.IsSubmenuOpen;
					}
					else
					{
						adorner.PaintGeneralTextLayout (graphics, clipRect, pos, layout, state&~(WidgetPaintState.Focused|WidgetPaintState.Selected), PaintTextStyle.TextField, this.textFieldDisplayMode, this.BackColor);

						for (int i=0; i<areas.Length; i++)
						{
							areas[i].Rect = graphics.Align (Drawing.Rectangle.Offset (areas[i].Rect, 0, -1));
						}
						WidgetPaintState st = state;
						if (this.contextMenu != null)
							st |= WidgetPaintState.Focused;
						adorner.PaintTextSelectionBackground (graphics, areas, st, PaintTextStyle.TextField, this.textFieldDisplayMode);

						Drawing.Rectangle[] rects = new Drawing.Rectangle[areas.Length];
						for (int i=0; i<areas.Length; i++)
						{
							rects[i] = this.MapClientToRoot (areas[i].Rect);
						}
						graphics.SetClippingRectangles (rects);

						adorner.PaintGeneralTextLayout (graphics, clipRect, pos, layout, (state&~WidgetPaintState.Focused)|WidgetPaintState.Selected, PaintTextStyle.TextField, this.textFieldDisplayMode, this.BackColor);
					}
				}

				if (!this.navigator.IsReadOnly && visibleCursor && this.KeyboardFocus)
				{
					//	Dessine le curseur, sauf si le menu contextuel est affiché :
					Drawing.Point p1, p2;
					if (this.GetCursorPosition (out p1, out p2, pos))
					{
						adorner.PaintTextCursor (graphics, p1, p2, true);
					}
				}
			}
			else
			{
				//	On n'a pas le focus...

				adorner.PaintGeneralTextLayout (graphics, clipRect, pos, layout, state&~WidgetPaintState.Focused, PaintTextStyle.TextField, this.textFieldDisplayMode, this.BackColor);
			}
		}

		protected virtual TextLayout GetTextLayout()
		{
			System.Diagnostics.Debug.Assert (this.TextLayout != null);

			return this.TextLayout;
		}

		/// <summary>
		/// Gets the text layout used for painting. This can be a synthetic text
		/// layout object (e.g. a masked password or a hint).
		/// </summary>
		/// <returns></returns>
		protected virtual TextLayout GetPaintTextLayout()
		{
			TextLayout original = this.TextLayout;

			this.ClearValue (AbstractTextField.HintOffsetProperty);

			if (this.isPassword)
			{
				return AbstractTextField.GetPaintTextLayoutForPassword (original, this.PasswordReplacementCharacter);
			}

			if (this.textFieldDisplayMode == TextFieldDisplayMode.InheritedValue)
			{
				return AbstractTextField.GetPaintTextLayoutItalic (original);
			}
			if (this.textFieldDisplayMode == TextFieldDisplayMode.OverriddenValue)
			{
				return AbstractTextField.GetPaintTextLayoutBold (original);
			}
			
			
			string hintText = this.HintText;

			if (hintText != null)
			{
				switch (this.textFieldDisplayMode)
				{
					case TextFieldDisplayMode.ActiveHint:
						return this.GetPaintTextLayoutForHint (original, hintText);
					
					case TextFieldDisplayMode.PassiveHint:
						return AbstractTextField.GetPaintTextLayoutReplacement (original, hintText);
					
					default:
						break;
				}
			}

			return original;
		}

		private TextLayout GetPaintTextLayoutForHint(TextLayout original, string hintText)
		{
			string hint = TextConverter.ConvertToSimpleText (hintText);
			string text = TextConverter.ConvertToSimpleText (original.Text);

			string compHint = hint;
			string compText = text;

			if (this.HintComparisonConverter != null)
			{
				compHint = this.HintComparisonConverter (compHint);
				compText = this.HintComparisonConverter (compText);
			}

			int pos = compHint.IndexOf (compText, System.StringComparison.Ordinal);

			const string fontBegin = @"<font color="".hint"">";
			const string fontEnd   = @"</font>";

			if (pos < 0)
			{
				return new TextLayout (original)
				{
					Text = string.Concat (fontBegin, TextConverter.ConvertToTaggedText (hint), fontEnd)
				};
			}
			else
			{
				this.SetValue (AbstractTextField.HintOffsetProperty, pos);
				
				string hintPrefix = TextConverter.ConvertToTaggedText (hint.Substring (0, pos));
				string hintSuffix = TextConverter.ConvertToTaggedText (hint.Substring (pos + text.Length));
				
				return new TextLayout (original)
				{
					Text = string.Concat (fontBegin, hintPrefix, fontEnd, TextConverter.ConvertToTaggedText (text), fontBegin, hintSuffix, fontEnd)
				};
			}
		}

		private static TextLayout GetPaintTextLayoutForPassword(TextLayout original, char passwordReplacementCharacter)
		{
			if (original.Text.Length == 0)
			{
				return original;
			}
			
			int textLength = TextConverter.GetSimpleTextLength (original.Text);
				
			return new TextLayout (original)
			{
				Text = new string (passwordReplacementCharacter, textLength)
			};
		}

		private static TextLayout GetPaintTextLayoutItalic(TextLayout original)
		{
			var text = TextConverter.ConvertToSimpleText (original.Text);

			return new TextLayout (original)
			{
				Text = string.Concat ("<i>", TextConverter.ConvertToTaggedText (text), "</i>")
			};
		}

		private static TextLayout GetPaintTextLayoutBold(TextLayout original)
		{
			var text = TextConverter.ConvertToSimpleText (original.Text);

			return new TextLayout (original)
			{
				Text = string.Concat ("<b>", TextConverter.ConvertToTaggedText (text), "</b>")
			};
		}

		private static TextLayout GetPaintTextLayoutReplacement(TextLayout original, string hintText)
		{
			var text = TextConverter.ConvertToSimpleText (hintText);

			return new TextLayout (original)
			{
				Text = TextConverter.ConvertToTaggedText (text)
			};
		}
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateGeometry ();
		}

		protected sealed override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);
			//			this.UpdateGeometry ();
		}

		protected virtual void UpdateGeometry()
		{
			this.UpdateButtonGeometry ();
			this.OnCursorChanged (true);
		}

		protected virtual void UpdateButtonGeometry()
		{
		}

		protected virtual void UpdateButtonVisibility()
		{
		}

		protected virtual bool UpdateMouseCursor(Drawing.Point pos)
		{
			if (this.Client.Bounds.Contains (pos))
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

		#region IReadOnly Members
		
		bool Types.IReadOnly.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}
		
		#endregion


		public event EventHandler TextEdited
		{
			add
			{
				this.AddUserEventHandler ("TextEdited", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("TextEdited", value);
			}
		}

		public event EventHandler TextInserted
		{
			add
			{
				this.AddUserEventHandler ("TextInserted", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("TextInserted", value);
			}
		}

		public event EventHandler TextDeleted
		{
			add
			{
				this.AddUserEventHandler ("TextDeleted", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("TextDeleted", value);
			}
		}

		public event EventHandler MultilingualEditionCalled
		{
			add
			{
				this.AddUserEventHandler ("MultilingualEditionCalled", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("MultilingualEditionCalled", value);
			}
		}

		public event EventHandler SelectionChanged
		{
			add
			{
				this.AddUserEventHandler ("SelectionChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("SelectionChanged", value);
			}
		}

		public event EventHandler CursorChanged
		{
			add
			{
				this.AddUserEventHandler ("CursorChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("CursorChanged", value);
			}
		}

		public event EventHandler ReadOnlyChanged
		{
			add
			{
				this.AddUserEventHandler ("ReadOnlyChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("ReadOnlyChanged", value);
			}
		}


		public event EventHandler<CancelEventArgs> AutoSelecting
		{
			add
			{
				this.AddUserEventHandler ("AutoSelecting", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("AutoSelecting", value);
			}
		}

		public event EventHandler<CancelEventArgs> AutoErasing
		{
			add
			{
				this.AddUserEventHandler ("AutoErasing", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("AutoErasing", value);
			}
		}


		public event EventHandler EditionStarted
		{
			add
			{
				this.AddUserEventHandler ("EditionStarted", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("EditionStarted", value);
			}
		}

		public event EventHandler EditionAccepted
		{
			add
			{
				this.AddUserEventHandler ("EditionAccepted", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("EditionAccepted", value);
			}
		}

		public event EventHandler EditionRejected
		{
			add
			{
				this.AddUserEventHandler ("EditionRejected", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("EditionRejected", value);
			}
		}

		public event EventHandler<CancelEventArgs> AcceptingEdition
		{
			add
			{
				this.AddUserEventHandler ("AcceptingEdition", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("AcceptingEdition", value);
			}
		}

		public event EventHandler<CancelEventArgs> RejectingEdition
		{
			add
			{
				this.AddUserEventHandler ("RejectingEdition", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("RejectingEdition", value);
			}
		}

		private static void NotifyButtonShowConditionChanged(DependencyObject obj, object oldValue, object newValue)
		{
			AbstractTextField textField = (AbstractTextField) obj;
			textField.UpdateButtonVisibility ();
		}


		public static readonly DependencyProperty PasswordReplacementCharacterProperty = DependencyProperty<AbstractTextField>.Register (x => x.PasswordReplacementCharacter, new DependencyPropertyMetadata ('*'));
		public static readonly DependencyProperty HintTextProperty                     = DependencyProperty<AbstractTextField>.Register (x => x.HintText, new Helpers.VisualPropertyMetadata (Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty HintOffsetProperty                   = DependencyProperty<AbstractTextField>.RegisterReadOnly (x => x.HintOffset, new DependencyPropertyMetadata (0));
		public static readonly DependencyProperty DefocusActionProperty                = DependencyProperty<AbstractTextField>.Register (x => x.DefocusAction, new DependencyPropertyMetadata (DefocusAction.None));
		public static readonly DependencyProperty ButtonShowConditionProperty          = DependencyProperty<AbstractTextField>.Register (x => x.ButtonShowCondition, new DependencyPropertyMetadata (ButtonShowCondition.Always, AbstractTextField.NotifyButtonShowConditionChanged));


		internal const double					TextMargin = 2;
		internal const double					FrameMargin = 2;
		internal const double					Infinity = 1000000;

		private bool							autoSelectOnFocus;
		private bool							autoEraseOnFocus;

		protected Drawing.Margins				margins;
		protected Drawing.Size					realSize;
		protected Drawing.Point					scrollOffset;

		private bool							mouseDown;
		private ScrollDirection					mouseScrollDirection;

		private Drawing.Point					lastMousePos;
		private TextFieldStyle					textFieldStyle;
		private TextFieldDisplayMode			textFieldDisplayMode;
		private double							scrollZone = 0.5;
		private string							initialText;
		private TextFieldDisplayMode			initialTextDisplayMode;
		private bool							isEditing;
		private bool							isModal;
		private bool							isPassword;
		private bool							acceptsNullValue;
		private bool							lastHasSelection;
		private bool							hasEditedText;
		private bool							swallowReturn;
		private bool							swallowEscape;

		private readonly TextNavigator			navigator;

		private readonly CopyPasteBehavior		copyPasteBehavior;
		private VMenu							contextMenu;

		private static Timer					flashTimer;
		private static bool						flashTimerStarted;
		private static bool						showCursor = true;

		private static AbstractTextField		blinking;

		private List<string>					autocompletionList;
		private CommandController				commandController;
	}
}
