//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using BundleAttribute  = Support.BundleAttribute;
	using ContentAlignment = Drawing.ContentAlignment;
	
	/// <summary>
	/// La classe RadioButton réalise un bouton radio.
	/// </summary>
	public class RadioButton : AbstractButton
	{
		public RadioButton()
		{
			this.InternalState |= InternalState.AutoToggle;
		}
		
		public RadioButton(Widget embedder) : this ()
		{
			this.SetEmbedder(embedder);
		}
		
		public RadioButton(Widget parent, string group, int index) : this ()
		{
			this.Parent = parent;
			this.Group  = group;
			this.Index  = index;
		}
		
		
		public override double					DefaultHeight
		{
			get
			{
				return System.Math.Ceiling (this.DefaultFontHeight + 1);
			}
		}

		public override ContentAlignment		DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.MiddleLeft;
			}
		}
		
		public Drawing.Point					LabelOffset
		{
			get
			{
				return new Drawing.Point (RadioButton.RadioWidth, 0);
			}
		}

		
		[Bundle]	public string				Group
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
		
		
		public RadioButton.GroupController		Controller
		{
			get
			{
				if ((this.Parent != null) &&
					(this.Group != null) &&
					(this.Group.Length > 0))
				{
					//	Trouve le contrôleur du groupe, lequel est en principe accessible depuis
					//	le parent. S'il n'existe pas pour ce groupe, on le crée :
					
					string prop_name  = "$radio$group controller$" + this.Group;
					object prop_value = this.Parent.GetProperty (prop_name);
					
					if (prop_value == null)
					{
						RadioButton active = RadioButton.FindActiveRadio (this.Parent, this.Group);
						
						prop_value = new GroupController (this.Parent, this.Group, active == null ? -1 : active.Index);
						
						this.Parent.SetProperty (prop_name, prop_value);
					}
					
					return prop_value as GroupController;
				}
				
				return null;
			}
		}
		
		
		public static void TurnOff(Widget parent, string group, RadioButton keep)
		{
			// Eteint tous les boutons radio du groupe, sauf keep.
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
		
		public static RadioButton FindActiveRadio(Widget parent, string group)
		{
			foreach (RadioButton radio in RadioButton.FindRadioChildren (parent, group))
			{
				if ((radio.Group == group) &&
					(radio.ActiveState == WidgetState.ActiveYes))
				{
					return radio;
				}
			}
			
			return null;
		}
		
		
		public override void Toggle()
		{
			if (this.ActiveState != WidgetState.ActiveYes)
			{
				base.Toggle ();
			}
		}
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			Drawing.Rectangle base_rect = base.GetShapeBounds ();
			
			if ((this.TextLayout == null) ||
				(this.Text.Length == 0))
			{
				return base_rect;
			}
			
			Drawing.Rectangle text_rect = this.TextLayout.StandardRectangle;
			
			text_rect.Offset (this.LabelOffset);
			text_rect.Inflate (1, 1);
			text_rect.Inflate (Widgets.Adorner.Factory.Active.GeometryRadioShapeBounds);
			base_rect.MergeWith (text_rect);
			
			return base_rect;
		}

		public override Epsitec.Common.Drawing.Size GetBestFitSize()
		{
			if ((this.TextLayout == null) ||
				(this.Text.Length == 0))
			{
				return new Drawing.Size (RadioButton.RadioWidth, RadioButton.RadioHeight);
			}
			
			Drawing.Size size = this.TextLayout.SingleLineSize;
			
			size.Width  = System.Math.Ceiling (RadioButton.RadioWidth + size.Width + 3);
			size.Height = System.Math.Max (System.Math.Ceiling (size.Height), RadioButton.RadioHeight);
			
			return size;
		}

		
		protected override void OnActiveStateChanged()
		{
			base.OnActiveStateChanged ();
			
			if (this.ActiveState == WidgetState.ActiveYes)
			{
				//	Eteint les autres boutons du groupe (s'il y en a), puis notifie le contrôleur
				//	du groupe à propos du changement :
				
				RadioButton.TurnOff (this.Parent, this.Group, this);
				
				GroupController controller = this.Controller;
				
				if (controller != null)
				{
					controller.SetActiveIndex (this.Index);
				}
			}
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
			System.Diagnostics.Debug.Assert (this.TextLayout != null);
			
			Drawing.Point offset = this.LabelOffset;
			
			double dx = this.Client.Width - offset.X;
			double dy = this.Client.Height;
			
			this.TextLayout.Alignment  = this.Alignment;
			this.TextLayout.LayoutSize = new Drawing.Size (dx, dy);
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

		
		#region RadioButtonComparer Class
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
		#endregion
		
		#region GroupController Class
		public class GroupController : Support.Data.IChangedSource
		{
			public GroupController(Widget parent, string group, int index)
			{
				this.parent = parent;
				this.group  = group;
				this.index  = index;
			}
			
			
			public int							ActiveIndex
			{
				get
				{
					return this.index;
				}
				set
				{
					if (this.index != value)
					{
						RadioButton.Activate (this.parent, this.group, value);
						
						//	On ne met pas à jour this.index directement, car le fait d'activer
						//	le bouton en question va provoquer un événement qui va à son tour
						//	appeler notre méthode SetActiveIndex...
					}
				}
			}
			
			public string						Group
			{
				get
				{
					return this.group;
				}
			}
			
			
			internal void SetActiveIndex(int value)
			{
				if (this.index != value)
				{
					this.index = value;
					this.OnChanged ();
				}
			}
			
			
			protected virtual void OnChanged()
			{
				if (this.Changed != null)
				{
					this.Changed (this);
				}
			}
			
			
			#region IChangedSource Members
			public event Support.EventHandler	Changed;
			#endregion
			
			protected Widget					parent;
			protected int						index;
			protected string					group;
		}
		#endregion
		

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle (0, (this.Client.Height-RadioButton.RadioHeight)/2, RadioButton.RadioHeight, RadioButton.RadioHeight);
			WidgetState       state = this.PaintState;
			
			adorner.PaintRadio (graphics, rect, state);
			adorner.PaintGeneralTextLayout (graphics, this.LabelOffset, this.TextLayout, state, PaintTextStyle.RadioButton, this.BackColor);
		}
		

		protected static readonly double		RadioHeight = 13;
		protected static readonly double		RadioWidth  = 20;
		protected string						group;
	}
}
