namespace Epsitec.Common.Widgets
{
	using BundleAttribute  = Support.BundleAttribute;
	
	/// <summary>
	/// La classe RadioButton réalise un bouton radio.
	/// </summary>
	public class RadioButton : AbstractButton
	{
		public RadioButton()
		{
			this.InternalState |= InternalState.AutoToggle;
		}
		
		public RadioButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		// Retourne la hauteur standard.
		public override double					DefaultHeight
		{
			get
			{
				return this.DefaultFontHeight+1;
			}
		}

		[Bundle ("group")]	public string		Group
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
				RadioButton.TurnOff(this.Parent, this.Group, this);
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
		public static void TurnOff(Widget parent, string group, RadioButton keep)
		{
			System.Collections.ArrayList list = RadioButton.FindRadioChildren(parent, group);
			
			foreach (RadioButton radio in list)
			{
				if ((radio != keep) &&
					(radio.ActiveState != WidgetState.ActiveNo))
				{
					radio.ActiveState = WidgetState.ActiveNo;
				}
			}
		}
		
		public static System.Collections.ArrayList FindRadioChildren(Widget parent, string group)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			//	Trouve tous les boutons radio du même groupe :
			
			if (parent == null)
			{
				return list;
			}
			
			foreach (Widget widget in parent.FindAllChildren ())
			{
				RadioButton radio = widget as RadioButton;
				
				if (radio != null)
				{
					if (radio.Group == group)
					{
						list.Add (radio);
					}
				}
			}
			
			return list;
		}
		
		public static void Activate(Widget parent, string group, int index)
		{
			RadioButton radio = RadioButton.FindRadio (parent, group, index);
			
			if (radio != null)
			{
				radio.ActiveState = WidgetState.ActiveYes;
			}
		}
		
		public static RadioButton FindRadio(Widget parent, string group, int index)
		{
			foreach (RadioButton radio in RadioButton.FindRadioChildren (parent, group))
			{
				if ((radio.Group == group) &&
					(radio.Index == index))
				{
					return radio;
				}
			}
			
			return null;
		}
		
		
		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			if ((this.ActiveState == WidgetState.ActiveYes) ||
				(mode != TabNavigationMode.ActivateOnTab))
			{
				return base.AboutToGetFocus (dir, mode, out focus);
			}
			
			//	Ce n'est pas notre bouton radio qui est allumé. TAB voudrait nous donner le
			//	focus, mais ce n'est pas adéquat; mieux vaut mettre le focus sur le frère qui
			//	est activé :
			
			System.Collections.ArrayList list = RadioButton.FindRadioChildren (this.Parent, this.Group);
			
			foreach (RadioButton radio in list)
			{
				if (radio.ActiveState == WidgetState.ActiveYes)
				{
					return radio.AboutToGetFocus (dir, mode, out focus);
				}
			}
			
			return base.AboutToGetFocus (dir, mode, out focus);
		}
		
		protected override System.Collections.ArrayList FindTabWidgetList(TabNavigationMode mode)
		{
			if (mode != TabNavigationMode.ActivateOnTab)
			{
				return base.FindTabWidgetList (mode);
			}
			
			//	On recherche les frères de ce widget, pour déterminer lequel devra être activé par la
			//	pression de la touche TAB. Pour bien faire, il faut supprimer les autres boutons radio
			//	qui appartiennent à notre groupe :
			
			System.Collections.ArrayList list = base.FindTabWidgetList (mode);
			System.Collections.ArrayList copy = new System.Collections.ArrayList ();
			
			string group = this.Group;
			
			foreach (Widget widget in list)
			{
				RadioButton radio = widget as RadioButton;
				
				if ((radio != null) &&
					(radio != this) &&
					(radio.Group == group))
				{
					//	Saute les boutons du même groupe. Ils ne sont pas accessibles par la
					//	touche TAB.
				}
				else
				{
					copy.Add (widget);
				}
			}
			
			return copy;
		}


		protected override void UpdateTextLayout()
		{
			if ( this.TextLayout != null )
			{
				Drawing.Point offset = this.LabelOffset;
				double dx = this.Client.Width - offset.X;
				double dy = this.Client.Height;
				this.TextLayout.Alignment = this.Alignment;
				this.TextLayout.LayoutSize = new Drawing.Size(dx, dy);
			}
		}
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.Type)
			{
				case MessageType.KeyDown:
					if (this.ProcessKeyDown (message.KeyCode))
					{
						message.Consumer = this;
						return;
					}
					break;
			}
			
			base.ProcessMessage (message, pos);
		}

		protected virtual  bool ProcessKeyDown(KeyCode key)
		{
			int dir = 0;
			
			switch (key)
			{
				case KeyCode.ArrowUp:	 dir =  1; break;
				case KeyCode.ArrowDown:  dir = -1; break;
				case KeyCode.ArrowLeft:  dir =  1; break;
				case KeyCode.ArrowRight: dir = -1; break;
				
				default:
					return false;
			}
			
			System.Collections.ArrayList list = RadioButton.FindRadioChildren (this.Parent, this.Group);
			list.Sort (new RadioButtonComparer (dir));
			
			RadioButton turn_on = null;
			
			foreach (RadioButton button in list)
			{
				if (button == this)
				{
					break;
				}
				
				turn_on = button;
			}
			
			if (turn_on != null)
			{
				turn_on.ActiveState = WidgetState.ActiveYes;
				turn_on.SetFocused (true);
				
				return true;
			}
			
			return false;
		}

		
		protected class RadioButtonComparer : System.Collections.IComparer
		{
			public RadioButtonComparer(int dir)
			{
				this.dir = dir;
			}
			
			
			public int Compare(object x, object y)
			{
				RadioButton bx = x as RadioButton;
				RadioButton by = y as RadioButton;
				if (bx == by) return 0;
				if (bx == null) return -this.dir;
				if (by == null) return  this.dir;
				return (bx.Index - by.Index) * this.dir;
			}
			
			
			protected int						dir;
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
			Drawing.Rectangle rect = base.GetShapeBounds();
			
			if ( this.TextLayout != null )
			{
				Drawing.Rectangle text = this.TextLayout.StandardRectangle;
				text.Offset(this.LabelOffset);
				text.Inflate(1, 1);
				text.Inflate(Widgets.Adorner.Factory.Active.GeometryRadioShapeBounds);
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
			adorner.PaintRadio(graphics, rect, this.PaintState);

			adorner.PaintGeneralTextLayout(graphics, this.LabelOffset, this.TextLayout, this.PaintState, PaintTextStyle.RadioButton, this.BackColor);
		}
		
		protected Drawing.Point LabelOffset
		{
			get
			{
				return new Drawing.Point(RadioButton.radioWidth, 0);
			}
		}


		protected static readonly double		radioHeight = 13;
		protected static readonly double		radioWidth = 20;
		protected string						group;
	}
}
