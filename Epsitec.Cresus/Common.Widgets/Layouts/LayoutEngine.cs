//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		public static ILayoutEngine					DockEngine
		{
			get
			{
				return LayoutEngine.dock_engine;
			}
		}
		public static ILayoutEngine					AnchorEngine
		{
			get
			{
				return LayoutEngine.anchor_engine;
			}
		}
		public static ILayoutEngine					NoOpEngine
		{
			get
			{
				return LayoutEngine.no_op_engine;
			}
		}

		public static ILayoutEngine SelectLayoutEngine(Visual visual)
		{
			if (visual.Dock != DockStyle.None)
			{
				return LayoutEngine.DockEngine;
			}
			if (visual.Anchor != AnchorStyles.None)
			{
				return LayoutEngine.AnchorEngine;
			}
			
			return LayoutEngine.NoOpEngine;
		}
		
		private static ILayoutEngine					dock_engine   = new DockLayoutEngine ();
		private static ILayoutEngine					anchor_engine = new AnchorLayoutEngine ();
		private static ILayoutEngine					no_op_engine  = new NoOpLayoutEngine ();
	}
}
