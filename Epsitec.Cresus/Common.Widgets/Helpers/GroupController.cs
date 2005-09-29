//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// La classe GroupController permet de gérer des groupes de widgets.
	/// </summary>
	public class GroupController : Types.IChange
	{
		public GroupController(Widget parent, string group)
		{
			this.parent = parent;
			this.group  = group;
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
					//	On active le widget correspondant à l'index spécifié, ce
					//	qui va appeler notre méthode SetActiveIndex pour mettre à
					//	jour l'index réellement actif.
					
					Widget widget = this.FindWidget (this.index);
					
					if (widget != null)
					{
						widget.ActiveState = WidgetState.ActiveYes;
					}
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
		
		public Widget[]						Widgets
		{
			get
			{
				return (Widget[]) this.FindWidgets ().ToArray (typeof (Widget));
			}
		}
		
		
		
		public static GroupController GetGroupController(Widget widget)
		{
			System.Diagnostics.Debug.Assert (widget.AutoRadio);
			System.Diagnostics.Debug.Assert (widget.Parent != null);
			System.Diagnostics.Debug.Assert (widget.Group != null);
			System.Diagnostics.Debug.Assert (widget.Group.Length > 0);
			
			return GroupController.GetGroupController (widget.Parent, widget.Group);
		}
		
		public static GroupController GetGroupController(Widget parent, string group)
		{
			if ((parent != null) &&
				(group != null) &&
				(group.Length > 0))
			{
				//	Trouve le contrôleur du groupe, lequel est en principe accessible depuis
				//	le parent. S'il n'existe pas pour ce groupe, on le crée :
			
				string prop_name  = "$radio$group controller$" + group;
				object prop_value = parent.GetProperty (prop_name);
			
				if (prop_value == null)
				{
					Helpers.GroupController controller;
					
					controller = new Helpers.GroupController (parent, group);
					prop_value = controller;
					
					Widget active = controller.FindActiveWidget ();
					
					controller.index = (active == null) ? -1 : active.Index;
				
					parent.SetProperty (prop_name, prop_value);
				}
			
				return prop_value as Helpers.GroupController;
			}
		
			return null;
		}
		
		
		public void TurnOffAllButOne(Widget keep)
		{
			//	Eteint tous les boutons radio du groupe, sauf celui spécifié par
			//	l'argument 'keep'.
			
			foreach (Widget radio in this.FindWidgets ())
			{
				if ((radio != keep) &&
					(radio.ActiveState != WidgetState.ActiveNo))
				{
					radio.ActiveState = WidgetState.ActiveNo;
				}
			}
		}
		
		
		public System.Collections.ArrayList FindWidgets()
		{
			//	Trouve tous les boutons radio qui appartiennent à notre groupe.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if (this.parent == null)
			{
				return list;
			}
			
			foreach (Widget widget in this.parent.FindAllChildren ())
			{
				if (widget.Group == this.group)
				{
					list.Add (widget);
				}
			}
			
			return list;
		}
		
		
		public Widget FindWidget(int index)
		{
			foreach (Widget widget in this.parent.FindAllChildren ())
			{
				if ((widget.AutoRadio) &&
					(widget.Group == this.group) &&
					(widget.Index == index))
				{
					return widget;
				}
			}
			
			return null;
		}
		
		public Widget FindActiveWidget()
		{
			foreach (Widget widget in this.parent.FindAllChildren ())
			{
				if ((widget.AutoRadio) &&
					(widget.Group == this.group) &&
					(widget.ActiveState == WidgetState.ActiveYes))
				{
					return widget;
				}
			}
			
			return null;
		}
		
		public Widget FindXWidget(Widget widget, int direction)
		{
			System.Diagnostics.Debug.Assert ((direction == 1) || (direction == -1));
			
			int index = widget.Index + direction;
			
			while (index >= 0)
			{
				widget = this.FindWidget (index);
				
				if (widget == null)
				{
					break;
				}
				if (widget.IsVisible)
				{
					return widget;
				}
				
				index += direction;
			}
			
			return null;
		}
		
		public Widget FindYWidget(Widget widget, int direction)
		{
			System.Diagnostics.Debug.Assert ((direction == 1) || (direction == -1));
			
			int index = widget.Index + direction * 1000;
			
			while (index >= 0)
			{
				widget = this.FindWidget (index);
				
				if (widget == null)
				{
					break;
				}
				if (widget.IsVisible)
				{
					return widget;
				}
				
				index += direction * 1000;
			}
			
			return null;
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
		
		
		#region IChange Members
		public event Support.EventHandler	Changed;
		#endregion
		
		protected Widget					parent;
		protected int						index;
		protected string					group;
	}
}
