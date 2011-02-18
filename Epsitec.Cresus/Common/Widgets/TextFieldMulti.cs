//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>TextFieldMulti</c> class implements a multiline text field with
	/// an optional vertical scroller.
	/// </summary>
	public sealed class TextFieldMulti : AbstractTextField
	{
		public TextFieldMulti()
			: base (TextFieldStyle.Multiline)
		{
			this.TextLayout.BreakMode &= ~Drawing.TextBreakMode.SingleLine;
			this.TextLayout.BreakMode |=  Drawing.TextBreakMode.Hyphenate;

			this.scroller = new VScroller (this);

			this.margins.Right = this.scroller.PreferredWidth;

			this.scroller.Enable = false;
			this.scroller.ValueChanged += this.HandleScrollerValueChanged;
			this.scroller.Anchor = AnchorStyles.Right | AnchorStyles.TopAndBottom;
			this.scroller.Margins = this.GetScrollerMargins ();
		}

		public TextFieldMulti(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
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
						this.margins.Right = this.scroller.PreferredWidth;
					}
					else
					{
						this.margins.Right = 0;
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.scroller.ValueChanged -= this.HandleScrollerValueChanged;
				this.scroller.Dispose ();
			}

			base.Dispose (disposing);
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

			return new Drawing.Margins ()
			{
				Right  = adorner.GeometryScrollerRightMargin-padding.Right,
				Top    = adorner.GeometryScrollerTopMargin-padding.Top,
				Bottom = adorner.GeometryScrollerBottomMargin-padding.Bottom
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

		private void HandleScrollerValueChanged(object sender)
		{
			this.scrollOffset.Y = this.scroller.DoubleValue-this.scroller.DoubleRange+AbstractTextField.Infinity-this.realSize.Height;
			this.Invalidate();
		}
		
		
		private readonly VScroller				scroller;
	}
}
