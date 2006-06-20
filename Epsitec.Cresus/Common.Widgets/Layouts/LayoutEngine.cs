//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// LayoutEngine.
	/// </summary>
	public sealed class LayoutEngine : DependencyObject
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
				case LayoutMode.Stacked:
					return LayoutEngine.StackEngine;
			}

			ILayoutEngine engine = LayoutEngine.GetLayoutEngine (visual);
			
			if (engine == null)
			{
				engine = LayoutEngine.NoOpEngine;
			}
			
			return engine;
		}

		public static LayoutMode GetLayoutMode(Visual visual)
		{
			LayoutMode mode = LayoutEngine.GetLayoutMode (visual.Dock, visual.Anchor);
			
			if (mode == LayoutMode.None)
			{
				Visual parent = visual.Parent;
				
				if (parent != null)
				{
					ILayoutEngine engine = LayoutEngine.GetLayoutEngine (parent);

					if (engine != null)
					{
						mode = engine.LayoutMode;
					}
				}
			}
			
			return mode;
		}
		
		public static LayoutMode GetLayoutMode(DockStyle dock, AnchorStyles anchor)
		{
			if ((dock == DockStyle.Stacked) ||
				(dock == DockStyle.StackBegin) ||
				(dock == DockStyle.StackFill) ||
				(dock == DockStyle.StackEnd))
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

		public static ILayoutEngine GetLayoutEngine(DependencyObject o)
		{
			object value;
			
			if (o.TryGetLocalValue (LayoutEngine.LayoutEngineProperty, out value))
			{
				return value as ILayoutEngine;
			}
			else
			{
				return null;
			}
		}

		public static void SetLayoutEngine(DependencyObject o, ILayoutEngine engine)
		{
			if (engine == null)
			{
				o.ClearValue (LayoutEngine.LayoutEngineProperty);
			}
			else
			{
				o.SetValue (LayoutEngine.LayoutEngineProperty, engine);
			}
		}
		
		
		public static readonly DependencyProperty LayoutEngineProperty = DependencyProperty.RegisterAttached ("LayoutEngine", typeof (ILayoutEngine), typeof (LayoutEngine));

		private static ILayoutEngine					dock_engine   = new DockLayoutEngine ();
		private static ILayoutEngine					anchor_engine = new AnchorLayoutEngine ();
		private static ILayoutEngine					no_op_engine  = new NoOpLayoutEngine ();
		private static ILayoutEngine					stack_engine  = new StackLayoutEngine ();
	}
}
