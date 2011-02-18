//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public MenuWindow(Behaviors.MenuBehavior behavior, Widget parentWidget) : this ()
		{
			System.Diagnostics.Debug.Assert (behavior != null);
			
			this.Behavior     = behavior;
			this.ParentWidget = parentWidget;
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
		
		public MenuType							MenuType
		{
			get
			{
				return this.menuType;
			}
			set
			{
				this.menuType = value;
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
		
		
		public static Widget GetParentWidget(DependencyObject o)
		{
			return (Widget) o.GetValue (MenuWindow.ParentWidgetProperty);
		}
		
		public static void SetParentWidget(DependencyObject o, Widget value)
		{
			o.SetValue (MenuWindow.ParentWidgetProperty, value);
		}
		
		
		public override void Hide()
		{
			//	Quand on cache un menu, on libère en fait de manière "transparente"
			//	la fenêtre associée; grâce à la méthode OnWindowDisposing, nous
			//	empêchons que ceci n'entraîne notre propre destruction.
			
			base.Hide ();
			base.Close ();
		}

		
		protected override void OnWindowDisposing()
		{
			base.OnWindowDisposing ();
			
			//	Supprime le lien entre les widgets de cette fenêtre et la fenêtre
			//	elle-même, ce qui évite un Dispose automatique du widget menu, par
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
		
		
		private static void SetParentWidgetValue(DependencyObject o, object value)
		{
			MenuWindow that = o as MenuWindow;
			Widget   parent = value as Widget;
			
			that.SetValueBase (MenuWindow.ParentWidgetProperty, value);
			
			Window oldOwner = that.Owner;
			Window newOwner = parent == null ? null : parent.Window;
			
			if (oldOwner != newOwner)
			{
				that.Owner = newOwner;
			}
		}
		
		
		public static readonly DependencyProperty ParentWidgetProperty	= DependencyProperty.Register ("ParentWidget", typeof (Widget), typeof (MenuWindow), new DependencyPropertyMetadata (null, new SetValueOverrideCallback (MenuWindow.SetParentWidgetValue)));
		
		private Behaviors.MenuBehavior			behavior;
		private MenuType						menuType;
	}
}
