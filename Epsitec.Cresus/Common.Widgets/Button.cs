namespace Epsitec.Common.Widgets
{
	public enum ButtonStyle
	{
		Flat,							// pas de cadre, ni de relief
		Normal,							// bouton normal
		Scroller,						// bouton pour Scroller
		Combo,							// bouton pour TextFieldCombo
		UpDown,							// bouton pour TextFieldUpDown
		Icon,							// bouton pour une icône
		ToolItem,						// bouton pour barre d'icône
		ListItem,						// bouton pour liste
		DefaultActive,					// bouton pour l'action par défaut (OK)
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
		
		
		public override double			DefaultHeight
		{
			// Retourne la hauteur standard d'un bouton.
			get
			{
				return this.DefaultFontHeight+10;
			}
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

		public override Drawing.Point	BaseLine
		{
			get
			{
				if (this.TextLayout != null)
				{
					return this.TextLayout.GetLineOrigin (0);
				}
				
				return base.BaseLine;
			}
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			// Dessine le bouton.
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point ();
			
			if ( (state & WidgetState.Enabled) == 0 )
			{
				state &= ~WidgetState.Focused;
				state &= ~WidgetState.Entered;
				state &= ~WidgetState.Engaged;
			}
			adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.buttonStyle);
			adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.buttonStyle);
		}
		
		
		protected ButtonStyle			buttonStyle;
	}
}
