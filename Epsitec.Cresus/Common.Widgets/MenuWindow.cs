//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe MenuWindow représente une fenêtre utilisée spécifiquement
	/// pour contenir des menus (ou des palettes de type pop-up).
	/// </summary>
	public class MenuWindow : Window
	{
		public MenuWindow()
		{
			this.MakeFramelessWindow ();
			this.MakeFloatingWindow ();
			
			this.DisableMouseActivation ();
		}
		
		public MenuWindow(Behaviors.MenuBehavior behavior, Widget parent_widget) : this ()
		{
			System.Diagnostics.Debug.Assert (behavior != null);
			
			this.Behavior     = behavior;
			this.ParentWidget = parent_widget;
		}
		
		
		public Behaviors.MenuBehavior			Behavior
		{
			get
			{
				return this.behavior;
			}
			set
			{
				if (this.behavior != value)
				{
					this.behavior = value;
				}
			}
		}
		
		public Widget							ParentWidget
		{
			get
			{
				return this.parent_widget;
			}
			set
			{
				if (this.parent_widget != value)
				{
					this.DefineParent (value);
				}
			}
		}
		
		
		
		
		public new void Show()
		{
			if (this.IsVisible == false)
			{
				this.AnimateShow (Animation.FadeIn);
			}
			
			this.FocusWidget (this.Root);
		}
		
		
		private void DefineParent(Widget parent_widget)
		{
			this.parent_widget = parent_widget;
			
			Window owner_window = this.parent_widget == null ? null : this.parent_widget.Window;
			
			this.Owner             = owner_window;
			this.CommandDispatcher = owner_window == null ? null : owner_window.CommandDispatcher;
		}
		
		
		protected override void OnAboutToShowWindow()
		{
			System.Diagnostics.Debug.Assert (this.behavior != null);
			System.Diagnostics.Debug.Assert (this.IsVisible == false);
			
			double alpha = Widgets.Adorners.Factory.Active.AlphaMenu;
			
			if (alpha < 1.0)
			{
				this.MakeLayeredWindow (true);
				
				this.Alpha          = alpha;
				this.Root.BackColor = Drawing.Color.Transparent;
			}
			else
			{
				this.MakeLayeredWindow (false);
				
				this.Alpha = 1.0;
				this.Root.ClearLocalValue (Widget.BackColorProperty);
			}
			
			this.Root.Invalidate ();
			this.behavior.HandleAboutToShowMenuWindow (this);
			
			base.OnAboutToShowWindow ();
		}
		
		protected override void OnAboutToHideWindow()
		{
			System.Diagnostics.Debug.Assert (this.behavior != null);
			
			this.behavior.HandleAboutToHideMenuWindow (this);
			
			base.OnAboutToHideWindow ();
		}
		
		
		private Widget							parent_widget;
		private Behaviors.MenuBehavior			behavior;
	}
}
