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
		}
		
		public void DefineWidget(Widget widget, Drawing.Margins margins)
		{
			this.WindowSize = widget.Size + margins.Size;
			
			widget.Dock     = DockStyle.None;
			widget.Parent   = this.Root;
			widget.Location = new Drawing.Point (margins.Bottom, margins.Left);
			
			this.MarkForRepaint ();
		}
		
		
		
	}
}
