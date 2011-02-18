//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>TextFieldMultiEx</c> class implements a multiline text field with
	/// an optional vertical scroller and a accept/cancel function.
	/// </summary>
	public sealed class TextFieldMultiEx : AbstractTextField
	{
		public TextFieldMultiEx()
			: base (TextFieldStyle.Multiline)
		{
			this.TextLayout.BreakMode &= ~Drawing.TextBreakMode.SingleLine;
			this.TextLayout.BreakMode |=  Drawing.TextBreakMode.Hyphenate;

			this.scroller = new VScroller (this);

			this.ScrollerRightMargin = this.scroller.PreferredWidth;

			this.scroller.Enable = false;
			this.scroller.ValueChanged += this.HandleScrollerValueChanged;
			this.scroller.Anchor = AnchorStyles.Right | AnchorStyles.TopAndBottom;
			this.scroller.Margins = this.GetScrollerMargins ();

			this.acceptRejectBehavior = new Behaviors.AcceptRejectBehavior (this);
			this.acceptRejectBehavior.CreateButtons ();

			this.acceptRejectBehavior.RejectClicked += this.HandleAcceptRejectRejectClicked;
			this.acceptRejectBehavior.AcceptClicked += this.HandleAcceptRejectAcceptClicked;

			this.DefocusAction       = DefocusAction.None;
			this.ButtonShowCondition = ButtonShowCondition.WhenModified;
		}

		public TextFieldMultiEx(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public TextFieldMultiExPreferredLayout PreferredLayout
		{
			get
			{
				return this.preferredLayout;
			}
			set
			{
				if (this.preferredLayout != value)
				{
					this.preferredLayout = value;

					this.UpdateRightMargin ();
					this.UpdateButtonGeometry ();
				}
			}
		}

		public bool ScrollerVisibility
		{
			get
			{
				return this.scroller.Visibility;
			}
			set
			{
				if (this.scroller.Visibility != value)
				{
					this.scroller.Visibility = value;

					if (value)
					{
						this.ScrollerRightMargin = this.scroller.PreferredWidth;
					}
					else
					{
						this.ScrollerRightMargin = 0;
					}
				}
			}
		}


		protected override bool CanStartEdition
		{
			get
			{
				return true;
			}
		}


		public override bool AcceptEdition()
		{
			if ((this.IsValid) &&
				(this.IsEditing) &&
				(this.CheckAcceptEdition ()))
			{
				this.acceptRejectBehavior.InitialText = this.Text;
				this.OnTextDefined ();
				this.UpdateButtonVisibility ();

				return base.AcceptEdition ();
			}

			return false;
		}

		public override bool RejectEdition()
		{
			if ((this.IsEditing) &&
				(this.CheckRejectEdition ()))
			{
				bool ok = base.RejectEdition ();

				if (ok)
				{
					this.UpdateButtonVisibility ();
				}

				return ok;
			}
			else
			{
				return base.RejectEdition ();
			}
		}


		public override Drawing.Rectangle GetButtonBounds()
		{
			//	Retourne le rectangle à utiliser pour les boutons Accept/Reject.
			//	Ce rectangle est positionné en bas à droite du widget.
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = new Drawing.Rectangle ();

			if (this.preferredLayout == TextFieldMultiExPreferredLayout.PreserveTextWidth)
			{
				double height = System.Math.Min (this.ActualHeight, TextFieldMultiEx.SingleLineDefaultHeight);

				rect.Left   = this.ActualWidth - adorner.GeometryComboRightMargin - this.buttonsRightMargin;
				rect.Right  = this.ActualWidth - adorner.GeometryComboRightMargin;
				rect.Bottom = adorner.GeometryComboBottomMargin;
				rect.Top    = height - adorner.GeometryComboTopMargin;
			}

			if (this.preferredLayout == TextFieldMultiExPreferredLayout.PreserveScrollerHeight)
			{
				double offset = this.scrollerRightMargin;
				double height = System.Math.Min (this.ActualHeight, TextFieldMultiEx.SingleLineDefaultHeight);

				rect.Left   = this.ActualWidth - this.scrollerRightMargin - adorner.GeometryComboRightMargin - this.buttonsRightMargin;
				rect.Right  = this.ActualWidth - this.scrollerRightMargin - adorner.GeometryComboRightMargin;
				rect.Bottom = adorner.GeometryComboBottomMargin;
				rect.Top    = height - adorner.GeometryComboTopMargin;
			}

			return rect;
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.scroller.ValueChanged -= this.HandleScrollerValueChanged;
				this.scroller.Dispose ();

				this.acceptRejectBehavior.RejectClicked -= this.HandleAcceptRejectRejectClicked;
				this.acceptRejectBehavior.AcceptClicked -= this.HandleAcceptRejectAcceptClicked;
			}

			base.Dispose (disposing);
		}


		protected override bool ProcessMouseDown(Message message, Drawing.Point pos)
		{
			return base.ProcessMouseDown (message, pos);
		}

		protected override bool ProcessKeyDown(Message message, Drawing.Point pos)
		{
			return base.ProcessKeyDown (message, pos);
		}


		protected override bool AboutToLoseFocus(TabNavigationDir dir, TabNavigationMode mode)
		{
//			if (this.accept_reject_behavior.IsVisible)
			{
				switch (this.DefocusAction)
				{
					case DefocusAction.Modal:
						if (this.IsValid)
						{
							return this.CheckAcceptEdition ();
						}

						return false;
				}
			}

			if (this.CheckBeforeDefocus ())
			{
				return base.AboutToLoseFocus (dir, mode);
			}
			else
			{
				return false;
			}
		}

		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			return base.AboutToGetFocus (dir, mode, out focus);
		}


		protected override void UpdateButtonGeometry()
		{
			if (this.acceptRejectBehavior != null)
			{
				this.ButtonsRightMargin = this.acceptRejectBehavior.DefaultWidth;
				this.acceptRejectBehavior.UpdateButtonGeometry ();
			}

			base.UpdateButtonGeometry ();
		}

		protected override void UpdateButtonVisibility()
		{
			bool show = false;

			switch (this.ButtonShowCondition)
			{
				case ButtonShowCondition.Always:
					show = true;
					break;

				case ButtonShowCondition.Never:
					break;

				case ButtonShowCondition.WhenFocused:
					show = this.IsFocused;
					break;

				case ButtonShowCondition.WhenKeyboardFocused:
					show = this.KeyboardFocus;
					break;

				case ButtonShowCondition.WhenModified:
					show = this.HasEditedText;
					break;

				default:
					throw new System.NotImplementedException (string.Format ("ButtonShowCondition.{0} not implemented.", this.ButtonShowCondition));
			}

			this.SetButtonVisibility (show);
		}


		private void SetButtonVisibility(bool show)
		{
			if (this.acceptRejectBehavior == null)
			{
				return;
			}

			if (this.acceptRejectBehavior.IsVisible != show)
			{
				this.acceptRejectBehavior.SetVisible (show);

				Window window = this.Window;

				if (window != null)
				{
					window.ForceLayout ();
				}

				this.UpdateButtonGeometry ();
				this.UpdateButtonEnable ();
				this.UpdateTextLayout ();
				this.UpdateMouseCursor (this.MapRootToClient (Message.CurrentState.LastPosition));
			}
		}

		private void UpdateButtonEnable()
		{
			if (this.acceptRejectBehavior != null)
			{
				this.acceptRejectBehavior.SetAcceptEnabled (this.IsValid);
			}
		}


		protected override void OnTextDefined()
		{
			base.OnTextDefined ();

			this.acceptRejectBehavior.InitialText = this.Text;
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged ();

			//? System.Diagnostics.Debug.Assert (this.HasEditedText || this.Text == this.accept_reject_behavior.InitialText);

			this.UpdateButtonEnable ();
			this.UpdateButtonVisibility ();
		}

		protected override void OnKeyboardFocusChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnKeyboardFocusChanged (e);

			this.UpdateButtonVisibility ();
		}


		protected override void OnAdornerChanged()
		{
			this.scroller.Margins = this.GetScrollerMargins ();
			base.OnAdornerChanged();
		}

		protected override void CursorScrollText(Drawing.Rectangle cursor, bool force)
		{
			Drawing.Point end = this.GetTextEndPosition ();

			if (force)
			{
				double offset = cursor.Bottom;
				offset -= this.realSize.Height/2;
				offset  = System.Math.Max (offset, end.Y);
				offset += this.realSize.Height;
				offset  = System.Math.Min (offset, AbstractTextField.Infinity);
				this.scrollOffset.Y = offset-this.realSize.Height;
			}
			else
			{
				double ratioBottom = (cursor.Bottom-this.scrollOffset.Y)/this.realSize.Height;  // 0..1
				double ratioTop    = (cursor.Top   -this.scrollOffset.Y)/this.realSize.Height;  // 0..1
				double scrollZone  = this.ScrollZone * 0.5;

				double h = AbstractTextField.Infinity-end.Y;  // hauteur de tout le texte
				
				if (h <= this.realSize.Height || this.realSize.Height < 0)
				{
					this.scrollOffset.Y = AbstractTextField.Infinity-this.realSize.Height;
				}
				else
				{
					if (ratioBottom <= scrollZone)  // curseur trop bas ?
					{
						this.scrollOffset.Y -= (scrollZone-ratioBottom)*this.realSize.Height;
						double min = System.Math.Min (end.Y, AbstractTextField.Infinity-this.realSize.Height);
						this.scrollOffset.Y = System.Math.Max (this.scrollOffset.Y, min);
					}

					if (ratioTop >= 1.0-scrollZone)  // curseur trop haut ?
					{
						this.scrollOffset.Y += (ratioTop-(1.0-scrollZone))*this.realSize.Height;
						this.scrollOffset.Y = System.Math.Min (this.scrollOffset.Y, AbstractTextField.Infinity-this.realSize.Height);
					}
				}
			}

			this.scrollOffset.X = 0;
			this.UpdateScroller ();
		}

		protected override void ScrollVertical(double dist)
		{
			//	Décale le texte vers le haut (+) ou le bas (-), lorsque la
			//	souris dépasse pendant une sélection.
			this.scrollOffset.Y += dist;
			Drawing.Point end = this.GetTextEndPosition ();
			double min = System.Math.Min (end.Y, AbstractTextField.Infinity-this.realSize.Height);
			double max = AbstractTextField.Infinity-this.realSize.Height;
			this.scrollOffset.Y = System.Math.Max (this.scrollOffset.Y, min);
			this.scrollOffset.Y = System.Math.Min (this.scrollOffset.Y, max);
			this.Invalidate ();
			this.UpdateScroller ();

			Drawing.Point pos = this.LastMousePosition;
			pos.X -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos.Y -= AbstractTextField.TextMargin + AbstractTextField.FrameMargin;
			pos = this.Client.Bounds.Constrain (pos);
			pos += this.scrollOffset;
			this.TextNavigator.MouseMoveMessage (pos);
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			decimal value = this.scroller.Value;

			switch (message.MessageType)
			{
				case MessageType.KeyDown:
					if (message.IsControlPressed)
					{
						if (message.KeyCode == KeyCode.ArrowUp)
						{
							value = System.Math.Min (value+this.scroller.SmallChange*0.5m, this.scroller.Range);
							this.scroller.Value = value;
							message.Consumer = this;
							return;
						}
						if (message.KeyCode == KeyCode.ArrowDown)
						{
							value = System.Math.Max (value-this.scroller.SmallChange*0.5m, 0);
							this.scroller.Value = value;
							message.Consumer = this;
							return;
						}
					}
					break;

				case MessageType.MouseWheel:
					if (message.Wheel > 0)
					{
						value = System.Math.Min (value+this.scroller.SmallChange, this.scroller.Range);
					}
					else if (message.Wheel < 0)
					{
						value = System.Math.Max (value-this.scroller.SmallChange, 0);
					}

					this.scroller.Value = value;
					message.Consumer = this;
					return;
			}

			base.ProcessMessage (message, pos);
		}

		protected override Drawing.Size GetTextLayoutSize()
		{
			return new Drawing.Size(this.realSize.Width, AbstractTextField.Infinity);
		}

		private Drawing.Margins GetScrollerMargins()
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Margins padding = this.GetInternalPadding ();

			double bottom = adorner.GeometryScrollerBottomMargin;

			if (this.buttonsRightMargin > 0 && this.preferredLayout == TextFieldMultiExPreferredLayout.PreserveTextWidth)  // boutons Accept/Reject visibles ?
			{
				//	Si l'ascenseur doit cohabiter avec les boutons Accept/Reject, il ne doit pas aller
				//	jusqu'en bas, pour leur laisser de la place.
				bottom = TextFieldMultiEx.SingleLineDefaultHeight;
			}

			return new Drawing.Margins ()
			{
				Right  = adorner.GeometryScrollerRightMargin-padding.Right,
				Top    = adorner.GeometryScrollerTopMargin-padding.Top,
				Bottom = bottom-padding.Bottom,
			};
		}

		private Drawing.Point GetTextEndPosition()
		{
			TextLayout layout = this.GetPaintTextLayout ();
			return layout.FindTextEnd ();
		}

		private void UpdateScroller()
		{
			//	Met à jour l'asceuseur en fonction de this.scrollOffset.
			if (this.scroller == null)
			{
				return;
			}

			Drawing.Point end = this.GetTextEndPosition ();
			double h = AbstractTextField.Infinity-end.Y;  // hauteur de tout le texte
			if ((h <= this.realSize.Height) || 
				(this.realSize.Height < 0))
			{
				this.scroller.Enable            = false;
				this.scroller.MaxValue          = 0;
				this.scroller.VisibleRangeRatio = 0;
				this.scroller.Value             = 0;
			}
			else
			{
				this.scroller.Enable            = true;
				this.scroller.MaxValue          = (decimal) (h-this.realSize.Height);
				this.scroller.VisibleRangeRatio = (decimal) (this.realSize.Height/h);

				double offset = this.scrollOffset.Y+this.realSize.Height;
				decimal value = this.scroller.Range - (decimal) (AbstractTextField.Infinity-offset);

				if (value > this.scroller.Range)
				{
					value = this.scroller.Range;
				}
				else if (value < 0)
				{
					value = 0;
				}

				this.scroller.Value       = value;
				this.scroller.SmallChange = 20;
				this.scroller.LargeChange = (decimal) (this.realSize.Height/2.0);
			}
		}


		private double ScrollerRightMargin
		{
			//	Espace horizontal occupé par l'ascenseur.
			get
			{
				return this.scrollerRightMargin;
			}
			set
			{
				this.scrollerRightMargin = value;
				this.UpdateRightMargin ();
			}
		}

		private double ButtonsRightMargin
		{
			//	Espace horizontal occupé par les boutons Accept/Reject.
			get
			{
				return this.buttonsRightMargin;
			}
			set
			{
				this.buttonsRightMargin = value;
				this.UpdateRightMargin ();
			}
		}

		private void UpdateRightMargin()
		{
			//	Met à jour la marge droite du widget, qui dépend de l'espace occupé par l'ascenseur et de celui
			//	occupé par les boutons Accept/Reject.
			if (this.preferredLayout == TextFieldMultiExPreferredLayout.PreserveTextWidth)
			{
				this.margins.Right = System.Math.Max (this.scrollerRightMargin, this.buttonsRightMargin);
			}

			if (this.preferredLayout == TextFieldMultiExPreferredLayout.PreserveScrollerHeight)
			{
				this.margins.Right = this.scrollerRightMargin + this.buttonsRightMargin;
			}

			this.scroller.Margins = this.GetScrollerMargins ();
		}


		private void HandleScrollerValueChanged(object sender)
		{
			this.scrollOffset.Y = this.scroller.DoubleValue-this.scroller.DoubleRange+AbstractTextField.Infinity-this.realSize.Height;
			this.Invalidate();
		}

		private void HandleAcceptRejectAcceptClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.acceptRejectBehavior);
			this.AcceptEdition ();
		}

		private void HandleAcceptRejectRejectClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.acceptRejectBehavior);
			this.RejectEdition ();
		}


		public static double SingleLineDefaultHeight
		{
			//	Retourne la hauteur qu'aurait ce widget s'il était 'single line'.
			//	On a besoin de cela pour déterminer la hauteur des boutons Accept/Reject.
			get
			{
				return Widget.DefaultFontHeight + 2*(AbstractTextField.TextMargin+AbstractTextField.FrameMargin);
			}
		}


		private TextFieldMultiExPreferredLayout				preferredLayout;
		private readonly VScroller							scroller;
		private readonly Behaviors.AcceptRejectBehavior		acceptRejectBehavior;
		private double										scrollerRightMargin;
		private double										buttonsRightMargin;
	}
}
