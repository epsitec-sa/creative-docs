//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// LayoutEngine.
	/// </summary>
	public sealed class LayoutEngine
	{
		private LayoutEngine()
		{
		}
		
		
		public static ILayout					Dock
		{
			get
			{
				return LayoutEngine.dock_layout;
			}
		}
		
		public static ILayout					Anchor
		{
			get
			{
				return LayoutEngine.anchor_layout;
			}
		}
		
		
		private static ILayout					dock_layout   = new DockLayout ();
		private static ILayout					anchor_layout = new AnchorLayout ();
	}
}
