namespace Epsitec.Common.Widgets.Design.Panels
{
	/// <summary>
	/// La classe WidgetPalette permet de remplir un panel servant de base à la
	/// palette des widgets servant à la construction de la GUI.
	/// </summary>
	public class WidgetPalette
	{
		public WidgetPalette(PreferredLayout preference)
		{
			this.preference = preference;
			this.size       = new Drawing.Size (180, 130);
		}
		
		public Drawing.Size				Size
		{
			get
			{
				return this.size;
			}
		}
		
		public PreferredLayout			PreferredLayout
		{
			get
			{
				return this.preference;
			}
		}
		
		
		public void CreateWidgets(Widget parent, Drawing.Point origin)
		{
			System.Diagnostics.Debug.Assert (this.preference != PreferredLayout.None);
			System.Diagnostics.Debug.Assert (this.parent == null);
			
			this.parent = parent;
			this.origin = origin;
			
			this.CreateDragSource (typeof (Button),          "Button",   0,   0, 86, 23+6);
			this.CreateDragSource (typeof (CheckButton),     "Check",    0,  30, 66, 14+6);
			this.CreateDragSource (typeof (RadioButton),     "Radio",    0,  50, 66, 14+6);
			this.CreateDragSource (typeof (TextField),       "",         0,  70, 86, 21+6);
			this.CreateDragSource (typeof (TextFieldUpDown), "10",       0,  98, 43, 21+6);
			this.CreateDragSource (typeof (TextFieldSlider), "40",      43,  98, 43, 21+6);
			this.CreateDragSource (typeof (TextFieldMulti),  "",        88,  70, 86, 55);
			this.CreateDragSource (typeof (GroupBox),        "Group",   88,   0, 86, 69);
			
//			this.CreateDragSource (typeof (VScroller), x, ref y, 0, dy);
//			this.CreateDragSource (typeof (TextFieldCombo),  x, ref y, dx2, 0);
//			this.CreateDragSource (typeof (HScroller),       x, ref y, dx2, 0);
		}
		
		protected void CreateDragSource(System.Type type, string text, double x, double y, double dx, double dy)
		{
			Widget     widget = System.Activator.CreateInstance (type) as Widget;
			DragSource source = new DragSource (parent);
			
			widget.Text = text;
			
			source.Widget   = widget;
			source.Parent   = this.parent;
			source.Location = new Drawing.Point (this.origin.X + x, this.origin.Y + this.size.Height - y - dy);
			source.Size     = new Drawing.Size (dx, dy);
		}
		
		protected Widget			parent;
		protected PreferredLayout	preference;
		protected Drawing.Size		size;
		protected Drawing.Point		origin;
	}
}
