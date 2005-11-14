//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		}

		
		public void Initialise(Widget parent)
		{
			Window owner = parent.Window;
			
			this.Owner             = owner;
			this.CommandDispatcher = owner.CommandDispatcher;
			
			IAdorner adorner = Widgets.Adorners.Factory.Active;
#if false
			if ( adorner.AlphaMenu < 1.0 )
			{
				this.window.MakeLayeredWindow();
				this.window.Alpha = adorner.AlphaMenu;
				this.window.Root.BackColor = Drawing.Color.Transparent;
			}
			this.window.DisableMouseActivation();
			this.window.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.Width, this.Height);
			
			this.SetParent (this.window.Root);
			AbstractMenu.RegisterFilter(this);

			this.window.AnimateShow(Animation.FadeIn);
			this.SetFocused(true);
#endif
		}
	}
}
