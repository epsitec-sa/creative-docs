
[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Button))]

namespace Epsitec.Common.Widgets
{
	public enum ButtonStyle
	{
		None,							// rien
		Flat,							// pas de cadre, ni de relief
		Normal,							// bouton normal
		Scroller,						// bouton pour Scroller
		Slider,							// bouton pour Slider
		Combo,							// bouton pour TextFieldCombo
		UpDown,							// bouton pour TextFieldUpDown
		ExListLeft,						// bouton pour TextFieldExList, à gauche
		ExListMiddle,					// bouton pour TextFieldExList, au milieu
		ExListRight,					// bouton pour TextFieldExList, à droite (cf Combo)
		Tab,							// bouton pour TabButton
		Icon,							// bouton pour une icône
		ActivableIcon,					// bouton pour une icône activable
		ToolItem,						// bouton pour barre d'icône
		ListItem,						// bouton pour liste
		HeaderSlider,					// bouton pour modifier une largeur de colonne
		
		DefaultAccept,					// bouton pour accepter un choix dans un dialogue (OK)
		DefaultCancel,					// bouton pour refuser un choix dans un dialogue (Cancel)
		DefaultAcceptAndCancel,			// bouton unique pour accepter ou refuser un choix dans un dialogue
	}
	
	/// <summary>
	/// La class Button représente un bouton standard.
	/// </summary>
	public class Button : AbstractButton
	{
		public Button()
		{
			this.buttonStyle = ButtonStyle.Normal;
		}
		
		public Button(string text)
		{
			this.buttonStyle = ButtonStyle.Normal;
			this.Text = text;
		}
		
		public Button(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		static Button()
		{
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata (Widget.DefaultFontHeight+10, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (Button), metadataDy);
		}
		
		public ButtonStyle				ButtonStyle
		{
			get
			{
				return this.buttonStyle;
			}

			set
			{
				if ( this.buttonStyle != value )
				{
					this.buttonStyle = value;
					this.Invalidate();
				}
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
			if (this.buttonStyle == ButtonStyle.ActivableIcon)
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

		
		protected override bool ProcessShortcut(Shortcut shortcut)
		{
			IFeel feel = Feel.Factory.Active;

			if (this.buttonStyle == ButtonStyle.DefaultAccept || this.buttonStyle == ButtonStyle.DefaultAcceptAndCancel)
			{
				if (feel.AcceptShortcut == shortcut)
				{
					this.OnShortcutPressed ();
					return true;
				}
			}

			if (this.buttonStyle == ButtonStyle.DefaultCancel || this.buttonStyle == ButtonStyle.DefaultAcceptAndCancel)
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
			//	Le texte dans les boutons standards doit être remonté d'un pixel
			//	pour paraître centré, mais surtout pas dans les IconButtons !
			
			switch (this.buttonStyle)
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
				//	Ne peint pas le fond du bouton si celui-ci a un fond explicitement défini
				//	comme "transparent".
			}
			else
			{
				//	Ne reproduit pas l'état sélectionné si on peint nous-même le fond du bouton.
				
				state &= ~WidgetPaintState.Selected;
				adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.buttonStyle);
			}

			pos.Y += this.GetBaseLineVerticalOffset ();
			
			if ( this.innerZoom != 1.0 )
			{
				Drawing.Transform transform = graphics.Transform;
				graphics.ScaleTransform (this.innerZoom, this.innerZoom, this.Client.Size.Width / 2, this.Client.Size.Height / 2);
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.buttonStyle);
				graphics.Transform = transform;
			}
			else
			{
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.buttonStyle);
			}
		}
		
		
		protected ButtonStyle			buttonStyle;
		protected double				innerZoom = 1.0;
	}
}
