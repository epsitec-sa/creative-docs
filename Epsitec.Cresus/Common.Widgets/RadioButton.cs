namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe RadioButton réalise un bouton radio.
	/// </summary>
	public class RadioButton : AbstractButton
	{
		public RadioButton()
		{
			//? this.internal_state |= InternalState.AutoToggle;
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
			if ( this.text_layout != null )
			{
				double dx = this.Client.Width - this.Client.Height*RadioButton.radioWidth;
				double dy = this.Client.Height;
				this.text_layout.Alignment = this.Alignment;
				this.text_layout.LayoutSize = new Drawing.Size(dx, dy);
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

		// Dessine le bouton.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Height, this.Client.Height);
			adorner.PaintRadio(graphics, rect, this.PaintState, this.RootDirection);

			Drawing.Point origine = new Drawing.Point(this.Client.Height*RadioButton.radioWidth, 0);
			adorner.PaintGeneralTextLayout(graphics, origine, this.text_layout, this.PaintState, this.RootDirection);
		}


		protected static readonly double	radioWidth = 1.5;
		protected string					group;
	}
}
