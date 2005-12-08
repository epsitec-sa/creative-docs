//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe MenuWindow repr�sente une fen�tre utilis�e sp�cifiquement
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
				return (Widget) this.GetValue (MenuWindow.ParentWidgetProperty);
			}
			set
			{
				this.SetValue (MenuWindow.ParentWidgetProperty, value);
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
		
		
		public static Widget GetParentWidget(Object o)
		{
			return (Widget) o.GetValue (MenuWindow.ParentWidgetProperty);
		}
		
		public static void SetParentWidget(Object o, Widget value)
		{
			o.SetValue (MenuWindow.ParentWidgetProperty, value);
		}
		
		
		public override void Hide()
		{
			base.Hide ();
			base.Close ();
		}

		
		protected override void OnWindowDisposing()
		{
			base.OnWindowDisposing ();
			
			//	Supprime le lien entre les widgets de cette fen�tre et la fen�tre
			//	elle-m�me, ce qui �vite un Dispose automatique du widget menu, par
			//	exemple.
			
			this.Root.Children.Clear ();
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
		
		
		private static void SetParentWidgetValue(Object o, object value)
		{
			MenuWindow that = o as MenuWindow;
			Widget   parent = value as Widget;
			
			that.SetValueBase (MenuWindow.ParentWidgetProperty, value);
			
			Window old_owner = that.Owner;
			Window new_owner = parent == null ? null : parent.Window;
			
			if (old_owner != new_owner)
			{
				that.Owner = new_owner;
			}
		}
		
		
		public static readonly Property ParentWidgetProperty	= Property.Register ("ParentWidget", typeof (Widget), typeof (MenuWindow), new PropertyMetadata (null, new SetValueOverrideCallback (MenuWindow.SetParentWidgetValue)));
		
		private Behaviors.MenuBehavior			behavior;
	}
}
