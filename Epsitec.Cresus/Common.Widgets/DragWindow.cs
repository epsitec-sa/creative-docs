//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe DragWindow implémente une surface transparente dans laquelle on
	/// peut placer un widget pendant une opération de drag & drop.
	/// </summary>
	public class DragWindow : Window
	{
		public DragWindow()
		{
			this.Root.BackColor = Drawing.Color.Transparent;
			
			this.MakeFramelessWindow ();
			this.MakeFloatingWindow();
			this.MakeLayeredWindow ();
			this.DisableMouseActivation ();
			
			this.Alpha = 0.9;
			this.Name = "DragWindow";
		}
		
		public void DefineWidget(Widget widget, Drawing.Size initial_size, Drawing.Margins margins)
		{
			this.WindowSize = initial_size + margins.Size;
			
			widget.Dock     = DockStyle.None;
			widget.Size     = initial_size;
			widget.Parent   = this.Root;
			widget.Location = new Drawing.Point (margins.Bottom, margins.Left);
			
			this.MarkForRepaint ();
		}
		
		public void DissolveAndDisposeWindow()
		{
			this.AnimateHide (Animation.FadeOut);
			this.WindowAnimationEnded += new Support.EventHandler (this.HandleWindowAnimationEnded);
		}
		
		
		private void HandleWindowAnimationEnded(object sender)
		{
			this.Hide ();
			this.Dispose ();
		}
	}
}
