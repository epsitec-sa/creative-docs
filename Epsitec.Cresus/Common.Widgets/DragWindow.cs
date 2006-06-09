//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			
			this.Alpha = 0.8;
			this.Name = "DragWindow";
		}

		public bool SuperLight
		{
			get
			{
				return (this.Alpha == 0.4);
			}
			set
			{
				this.Alpha = value ? 0.4 : 0.8;
			}
		}
		
		public void DefineWidget(Widget widget, Drawing.Size initial_size, Drawing.Margins margins)
		{
			this.WindowSize = initial_size + margins.Size;

			widget.Dock     = DockStyle.Fill;

			this.Root.Padding = margins;
			this.Root.Children.Add (widget);
			
			this.MarkForRepaint ();
		}
		
		public void DissolveAndDisposeWindow()
		{
			this.WindowAnimationEnded += new Support.EventHandler (this.HandleWindowAnimationEnded);
			this.AnimateHide (Animation.FadeOut);
		}
		
		
		private void HandleWindowAnimationEnded(object sender)
		{
			this.Hide ();
			this.Dispose ();
		}
	}
}
