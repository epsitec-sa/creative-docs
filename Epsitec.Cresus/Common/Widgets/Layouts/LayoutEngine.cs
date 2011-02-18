//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Layouts.LayoutEngine))]

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
				return LayoutEngine.dockEngine;
			}
		}
		public static ILayoutEngine					AnchorEngine
		{
			get
			{
				return LayoutEngine.anchorEngine;
			}
		}
		public static ILayoutEngine					StackEngine
		{
			get
			{
				return LayoutEngine.stackEngine;
			}
		}
		public static ILayoutEngine					NoOpEngine
		{
			get
			{
				return LayoutEngine.noOpEngine;
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
			return LayoutEngine.GetLayoutMode (visual, visual.Dock, visual.Anchor);
		}
		
		public static LayoutMode GetLayoutMode(Visual visual, DockStyle dock, AnchorStyles anchor)
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

			Visual parent = visual.Parent;

			if (parent != null)
			{
				ILayoutEngine engine = LayoutEngine.GetLayoutEngine (parent);

				if (engine != null)
				{
					return engine.LayoutMode;
				}
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

		public static bool GetIgnoreMeasure(DependencyObject o)
		{
			object value;

			if (o.TryGetLocalValue (LayoutEngine.IgnoreMeasureProperty, out value))
			{
				return (bool) value;
			}
			else
			{
				return false;
			}
		}

		public static void SetIgnoreMeasure(DependencyObject o, bool value)
		{
			if (value == false)
			{
				o.ClearValue (LayoutEngine.IgnoreMeasureProperty);
			}
			else
			{
				o.SetValue (LayoutEngine.IgnoreMeasureProperty, value);
			}
		}

		private static void NotifyLayoutEngineChanged(DependencyObject o, object oldValue, object newValue)
		{
			Visual visual = o as Visual;

			if (visual != null)
			{
				if (visual.HasChildren)
				{
					visual.Children.RefreshLayoutStatistics ();
				}

				if (visual.Parent != null)
				{
					LayoutContext.AddToMeasureQueue (visual);
					LayoutContext.AddToArrangeQueue (visual);
				}
			}
		}
		
		
		public static readonly DependencyProperty LayoutEngineProperty = DependencyProperty.RegisterAttached ("LayoutEngine", typeof (ILayoutEngine), typeof (LayoutEngine), new DependencyPropertyMetadata (LayoutEngine.NotifyLayoutEngineChanged));
		public static readonly DependencyProperty IgnoreMeasureProperty = DependencyProperty.RegisterAttached ("IgnoreMeasure", typeof (bool), typeof (LayoutEngine), new DependencyPropertyMetadata (false));

		private static ILayoutEngine					dockEngine   = new DockLayoutEngine ();
		private static ILayoutEngine					anchorEngine = new AnchorLayoutEngine ();
		private static ILayoutEngine					noOpEngine  = new NoOpLayoutEngine ();
		private static ILayoutEngine					stackEngine  = new StackLayoutEngine ();
	}
}
