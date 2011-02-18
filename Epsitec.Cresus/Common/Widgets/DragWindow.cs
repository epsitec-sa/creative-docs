//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: DependencyClass (typeof (DragWindow))]

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
		
		public void DefineWidget(Widget widget, Drawing.Size initialSize, Drawing.Margins margins)
		{
			this.WindowSize = initialSize + margins.Size;

			widget.Dock     = DockStyle.Fill;

			this.Root.Padding = margins;
			this.Root.Children.Add (widget);
			
			this.MarkForRepaint ();
		}
		
		public void DissolveAndDisposeWindow()
		{
			this.WindowAnimationEnded += this.HandleWindowAnimationEnded;
			this.AnimateHide (Animation.FadeOut);
		}
		
		
		private void HandleWindowAnimationEnded(object sender)
		{
			this.Hide ();
			this.Dispose ();
		}


		public static void SetDragWindow(DependencyObject obj, DragWindow value)
		{
			if (value == null)
			{
				obj.ClearValue (DragWindow.DragWindowProperty);
			}
			else
			{
				obj.SetValue (DragWindow.DragWindowProperty, value);
			}
		}

		public static DragWindow GetDragWindow(DependencyObject obj)
		{
			return obj.GetValue (DragWindow.DragWindowProperty) as DragWindow;
		}
		
		public static readonly DependencyProperty DragWindowProperty = DependencyProperty.RegisterAttached ("DragWindow", typeof (DragWindow), typeof (DragWindow));
	}
}
