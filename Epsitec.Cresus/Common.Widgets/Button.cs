namespace Epsitec.Common.Widgets
{
	public enum ButtonStyle
	{
		Flat,							// pas de cadre, ni de relief
		Normal,							// bouton normal
		Scroller,						// bouton pour Scroller
		Combo,							// bouton pour TextFieldCombo
		UpDown,							// bouton pour TextFieldUpDown
		ExListLeft,						// bouton pour TextFieldExList, � gauche
		ExListMiddle,					// bouton pour TextFieldExList, au milieu
		ExListRight,					// bouton pour TextFieldExList, � droite (cf Combo)
		Tab,							// bouton pour TabButton
		Icon,							// bouton pour une ic�ne
		ToolItem,						// bouton pour barre d'ic�ne
		ListItem,						// bouton pour liste
		HeaderSlider,					// bouton pour modifier une largeur de colonne
		
		DefaultAccept,					// bouton pour accepter un choix dans un dialogue (OK)
		DefaultCancel,					// bouton pour refuser un choix dans un dialogue (Cancel)
	}
	
	/// <summary>
	/// La class Button repr�sente un bouton standard.
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
					if ( this.buttonStyle == ButtonStyle.DefaultAccept ||
						 this.buttonStyle == ButtonStyle.DefaultCancel )
					{
						this.Shortcut = null;
					}
					
					this.buttonStyle = value;
					
					if ( this.buttonStyle == ButtonStyle.DefaultAccept )
					{
						this.Shortcut = Feel.Factory.Active.AcceptShortcut;
					}
					else if ( this.buttonStyle == ButtonStyle.DefaultCancel )
					{
						this.Shortcut = Feel.Factory.Active.CancelShortcut;
					}
					
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
		
		protected override void OnShortcutChanged()
		{
			base.OnShortcutChanged ();
			
			if (this.ButtonStyle == ButtonStyle.Normal)
			{
				IFeel feel = Feel.Factory.Active;
				
				if (feel.AcceptShortcut == this.Shortcut)
				{
					this.ButtonStyle = ButtonStyle.DefaultAccept;
				}
				else if (feel.CancelShortcut == this.Shortcut)
				{
					this.ButtonStyle = ButtonStyle.DefaultCancel;
				}
			}
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			// Dessine le bouton.
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point (0, 0);
			
			if ( (state & WidgetState.Enabled) == 0 )
			{
				state &= ~WidgetState.Focused;
				state &= ~WidgetState.Entered;
				state &= ~WidgetState.Engaged;
			}
			
			if ( this.BackColor.IsTransparent )
			{
				//	Ne peint pas le fond du bouton si celui-ci a un fond explicitement d�fini
				//	comme "transparent".
			}
			else
			{
				//	Ne reproduit pas l'�tat s�lectionn� si on peint nous-m�me le fond du bouton.
				
				state &= ~WidgetState.Selected;
				adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.buttonStyle);
			}
			
			if ( this.innerZoom != 1.0 )
			{
				Drawing.Transform transform = graphics.SaveTransform();
				graphics.ScaleTransform(this.innerZoom, this.innerZoom, this.Client.Width / 2, this.Client.Height / 2);
				adorner.PaintButtonTextLayout(graphics, pos, this.TextLayout, state, this.buttonStyle);
				graphics.RestoreTransform(transform);
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
