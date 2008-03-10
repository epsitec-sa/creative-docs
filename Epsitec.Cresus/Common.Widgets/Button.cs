using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Button))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class Button repr�sente un bouton standard.
	/// </summary>
	public class Button : AbstractButton
	{
		public Button()
		{
		}
		
		public Button(string text)
		{
			this.Text = text;
		}
		
		public Button(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		static Button()
		{
			Types.DependencyPropertyMetadata metadata = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();
			
			double height = Widget.DefaultFontHeight+10;
			
			metadata.DefineDefaultValue (height);
			
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (Button), metadata);
		}
		
		public virtual ButtonStyle		ButtonStyle
		{
			get
			{
				return (ButtonStyle) this.GetValue (Button.ButtonStyleProperty);
			}

			set
			{
				this.SetValue (Button.ButtonStyleProperty, value);
			}
		}

		public double					InnerZoom
		{
			get
			{
				return this.innerZoom;
			}
			set
			{
				if (this.innerZoom != value)
				{
					if (value < 0.01)
					{
						value = 1.0;
					}
					
					this.innerZoom = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public override Drawing.Margins GetShapeMargins()
		{
			if (this.ButtonStyle == ButtonStyle.ActivableIcon)
			{
				if ((this.PaintState&WidgetPaintState.ThreeState) == 0)
				{
					return Widgets.Adorners.Factory.Active.GeometryToolShapeMargins;
				}
				else
				{
					return Widgets.Adorners.Factory.Active.GeometryThreeStateShapeMargins;
				}
			}
			else
			{
				return base.GetShapeMargins ();
			}
		}

		protected override void DefineIconFromCaption(string icon)
		{
			base.DefineIconFromCaption (icon);

			if ((string.IsNullOrEmpty (this.Text)) &&
				(!string.IsNullOrEmpty (icon)))
			{
				this.Text = string.Concat (@"<img src=""", icon, @"""/>");
			}
		}
		
		protected override bool ProcessShortcut(Shortcut shortcut)
		{
			IFeel feel = Feel.Factory.Active;

			if ((this.ButtonStyle == ButtonStyle.DefaultAccept) || 
				(this.ButtonStyle == ButtonStyle.DefaultAcceptAndCancel))
			{
				if (feel.AcceptShortcut == shortcut)
				{
					this.OnShortcutPressed ();
					return true;
				}
			}

			if ((this.ButtonStyle == ButtonStyle.DefaultCancel) ||
				(this.ButtonStyle == ButtonStyle.DefaultAcceptAndCancel))
			{
				if (feel.CancelShortcut == shortcut)
				{
					this.OnShortcutPressed ();
					return true;
				}
			}
			
			return base.ProcessShortcut (shortcut);
		}

		protected override double GetBaseLineVerticalOffset()
		{
			//	Le texte dans les boutons standards doit �tre remont� d'un pixel
			//	pour para�tre centr�, mais surtout pas dans les IconButtons !
			
			switch (this.ButtonStyle)
			{
				case ButtonStyle.Normal:
				case ButtonStyle.DefaultAccept:
				case ButtonStyle.DefaultCancel:
				case ButtonStyle.DefaultAcceptAndCancel:
					return 1.0;
				
				default:
					return 0.0;
			}
		}

		protected override Epsitec.Common.Drawing.Size GetTextLayoutSize()
		{
			Drawing.Size size = base.GetTextLayoutSize ();

			switch (this.ButtonStyle)
			{
				case ButtonStyle.Normal:
				case ButtonStyle.DefaultAccept:
				case ButtonStyle.DefaultAcceptAndCancel:
				case ButtonStyle.DefaultCancel:
					size.Width = System.Math.Max (0, size.Width - 8);
					size.Height = System.Math.Max (0, size.Height - 8);
					break;
			}
			
			return size;
		}

		protected override Epsitec.Common.Drawing.Point GetTextLayoutOffset()
		{
			Drawing.Point offset = base.GetTextLayoutOffset ();

			switch (this.ButtonStyle)
			{
				case ButtonStyle.Normal:
				case ButtonStyle.DefaultAccept:
				case ButtonStyle.DefaultAcceptAndCancel:
				case ButtonStyle.DefaultCancel:
					offset.X += 4;
					offset.Y += 4;
					break;
			}
			
			return offset;
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetPaintState  state = this.PaintState;
			Drawing.Point     pos   = this.GetTextLayoutOffset ();
			
			if ( (state & WidgetPaintState.Enabled) == 0 )
			{
				state &= ~WidgetPaintState.Focused;
				state &= ~WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Engaged;
			}
			
			if ( this.BackColor.IsTransparent )
			{
				//	Ne peint pas le fond du bouton si celui-ci a un fond explicitement d�fini
				//	comme "transparent".
			}
			else
			{
				//	Ne reproduit pas l'�tat s�lectionn� si on peint nous-m�me le fond du bouton.
				
				state &= ~WidgetPaintState.Selected;
				adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.ButtonStyle);
			}

			pos.Y += this.GetBaseLineVerticalOffset ();
			
			if ( this.innerZoom != 1.0 )
			{
				Drawing.Transform transform = graphics.Transform;
				graphics.ScaleTransform (this.innerZoom, this.innerZoom, this.Client.Size.Width / 2, this.Client.Size.Height / 2);
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.ButtonStyle);
				graphics.Transform = transform;
			}
			else
			{
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.ButtonStyle);
			}
		}

		public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register ("ButtonStyle", typeof (ButtonStyle), typeof (Button), new Helpers.VisualPropertyMetadata (ButtonStyle.Normal, Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		
		protected double				innerZoom = 1.0;
	}
}
