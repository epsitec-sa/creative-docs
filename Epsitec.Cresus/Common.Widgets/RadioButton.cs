namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe RadioButton réalise un bouton radio.
	/// </summary>
	public class RadioButton : AbstractButton
	{
		public RadioButton()
		{
			this.internalState |= InternalState.AutoToggle;
		}
		
		public RadioButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight+1;
			}
		}

		public string Group
		{
			get
			{
				return this.group;
			}

			set
			{
				this.group = value;
			}
		}
		
		
		protected override void OnActiveStateChanged()
		{
			base.OnActiveStateChanged();
			
			if ( this.ActiveState != WidgetState.ActiveNo )
			{
				RadioButton.TurnOffRadio(this.Parent, this.Group, this);
			}
		}

		public override void Toggle()
		{
			if (this.ActiveState != WidgetState.ActiveYes)
			{
				base.Toggle ();
			}
		}

		
		// Eteint tous les boutons radio du groupe, sauf keep.
		public static void TurnOffRadio(Widget parent, string group, RadioButton keep)
		{
			RadioButton[] list = FindRadioNotOff(parent, group);
			for ( int i=0 ; i<list.Length ; i++ )
			{
				if ( list[i] == keep )  continue;
				list[i].ActiveState = WidgetState.ActiveNo;
			}
		}
		
		// Trouve tous les boutons radio du même groupe dont l'état n'est
		// pas WidgetState.ActiveNo.
		public static RadioButton[] FindRadioNotOff(Widget parent, string group)
		{
			if ( parent == null )  return new RadioButton[0];

			// Compte le nombre de boutons.
			Widget[] children = parent.Children.Widgets;
			int childrenNum = children.Length;
			int total = 0;
			for ( int i=0 ; i<childrenNum ; i++ )
			{
				Widget widget = children[i];
				System.Diagnostics.Debug.Assert(widget != null);
				if ( widget is RadioButton )
				{
					RadioButton radio = (RadioButton)widget;
					if ( radio.Group == group && radio.ActiveState != WidgetState.ActiveNo )
					{
						total ++;
					}
				}
			}
			if ( total == 0 )  return new RadioButton[0];

			// Construit la liste.
			RadioButton[] list = new RadioButton[total];
			int j=0;
			for ( int i=0 ; i<childrenNum ; i++ )
			{
				Widget widget = children[i];
				System.Diagnostics.Debug.Assert(widget != null);
				if ( widget is RadioButton )
				{
					RadioButton radio = (RadioButton)widget;
					if ( radio.Group == group && radio.ActiveState != WidgetState.ActiveNo )
					{
						list[j++] = radio;
					}
				}
			}
			return list;
		}
		

		protected override void UpdateLayoutSize()
		{
			if ( this.textLayout != null )
			{
				Drawing.Point offset = this.LabelOffset;
				double dx = this.Client.Width - offset.X;
				double dy = this.Client.Height;
				this.textLayout.Alignment = this.Alignment;
				this.textLayout.LayoutSize = new Drawing.Size(dx, dy);
			}
		}
		
		// Retourne l'alignement par défaut d'un bouton.
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			Drawing.Rectangle rect = base.GetShapeBounds ();
			
			if ( this.textLayout != null )
			{
				Drawing.Rectangle text = this.textLayout.StandardRectangle;
				text.Offset(this.LabelOffset);
				text.Inflate(1, 1);
				rect.MergeWith(text);
			}
			
			return rect;
		}


		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = 0;
			rect.Right  = RadioButton.radioHeight;
			rect.Bottom = (this.Client.Height-RadioButton.radioHeight)/2;
			rect.Top    = rect.Bottom+RadioButton.radioHeight;
			adorner.PaintRadio(graphics, rect, this.PaintState, this.RootDirection);

			adorner.PaintGeneralTextLayout(graphics, this.LabelOffset, this.textLayout, this.PaintState, this.RootDirection);
		}
		
		protected Drawing.Point LabelOffset
		{
			get
			{
				return new Drawing.Point(RadioButton.radioWidth, 0);
			}
		}


		protected static readonly double	radioHeight = 13;
		protected static readonly double	radioWidth = 20;
		protected string					group;
	}
}
