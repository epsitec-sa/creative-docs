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
		public static ILayoutEngine					StackEngine
		{
			get
			{
				return LayoutEngine.stack_engine;
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
			switch (LayoutEngine.GetLayoutMode (visual))
			{
				case LayoutMode.Docked:
					return LayoutEngine.DockEngine;
				case LayoutMode.Anchored:
					return LayoutEngine.AnchorEngine;
			}
			
			return LayoutEngine.NoOpEngine;
		}

		public static LayoutMode GetLayoutMode(Visual visual)
		{
			return LayoutEngine.GetLayoutMode (visual.Dock, visual.Anchor);
		}
		public static LayoutMode GetLayoutMode(DockStyle dock, AnchorStyles anchor)
		{
			if (dock == DockStyle.Stacked)
			{
				return LayoutMode.Stacked;
			}
			if (dock != DockStyle.None)
			{
				return LayoutMode.Docked;
			}
			if (anchor != AnchorStyles.None)
			{
				return LayoutMode.Anchored;
			}

			return LayoutMode.None;
		}

		private static ILayoutEngine					dock_engine   = new DockLayoutEngine ();
		private static ILayoutEngine					anchor_engine = new AnchorLayoutEngine ();
		private static ILayoutEngine					no_op_engine  = new NoOpLayoutEngine ();
		private static ILayoutEngine					stack_engine  = new StackLayoutEngine ();
	}
}
