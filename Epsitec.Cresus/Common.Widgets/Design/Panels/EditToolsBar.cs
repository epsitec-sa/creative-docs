using System;

namespace Epsitec.Common.Widgets.Design.Panels
{
	/// <summary>
	/// Summary description for EditToolsPalette.
	/// </summary>
	public class EditToolsBar : AbstractPalette
	{
		public EditToolsBar()
		{
			this.size = new Drawing.Size (200, 26);
		}
		
		protected override Widget CreateWidget()
		{
			HToolBar tools = new HToolBar ();
			
			tools.Size = this.Size;
			
			tools.Items.Add (new IconButton ("new window", "file:images/new1.icon"));
			tools.Items.Add (new IconSeparator ());
			tools.Items.Add (new IconButton ("delete selection", "file:images/delete1.icon"));
			
			return tools;
		}
	}
}
