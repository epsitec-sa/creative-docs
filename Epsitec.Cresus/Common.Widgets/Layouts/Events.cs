//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 02/02/2004

namespace Epsitec.Common.Widgets.Layouts
{
	public delegate void UpdateEventHandler(object sender, UpdateEventArgs e);
	
	public class UpdateEventArgs : System.ComponentModel.CancelEventArgs
	{
		public UpdateEventArgs(Widget widget, Widget[] children, Layouts.LayoutInfo layout_info)
		{
			this.widget      = widget;
			this.children    = children;
			this.layout_info = layout_info;
		}
		
		
		public Widget							Widget
		{
			get { return this.widget; }
		}
		
		public Widget[]							Children
		{
			get { return this.children; }
		}
		
		public LayoutInfo						LayoutInfo
		{
			get { return this.layout_info; }
		}
		
		
		private Widget							widget;
		private Widget[]						children;
		private LayoutInfo						layout_info;
	}
}


