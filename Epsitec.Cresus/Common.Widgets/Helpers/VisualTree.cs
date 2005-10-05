namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// VisualTree.
	/// </summary>
	public sealed class VisualTree
	{
		private VisualTree()
		{
		}
		
		
		public static Drawing.Point MapVisualToParent(Visual visual, Drawing.Point value)
		{
			Drawing.Rectangle bounds = visual.Bounds;
			
			double x = value.X + bounds.Left;
			double y = value.Y + bounds.Bottom;
			
			return new Drawing.Point (x, y);
		}
		
		public static Drawing.Point MapVisualToRoot(Visual visual, Drawing.Point value)
		{
			double x = value.X;
			double y = value.Y;
			
			for ( ; visual != null; )
			{
				Drawing.Rectangle bounds = visual.Bounds;
				
				x += bounds.Left;
				y += bounds.Bottom;
				
				visual = visual.Parent;
			}
			
			return new Drawing.Point (x, y);
		}
		
		public static Drawing.Point MapParentToVisual(Visual visual, Drawing.Point value)
		{
			Drawing.Rectangle bounds = visual.Bounds;
			
			double x = value.X - bounds.Left;
			double y = value.Y - bounds.Bottom;
			
			return new Drawing.Point (x, y);
		}
		
		public static Drawing.Point MapRootToVisual(Visual visual, Drawing.Point value)
		{
			double x = value.X;
			double y = value.Y;
			
			for ( ; visual != null; )
			{
				Drawing.Rectangle bounds = visual.Bounds;
				
				x -= bounds.Left;
				y -= bounds.Bottom;
				
				visual = visual.Parent;
			}
			
			return new Drawing.Point (x, y);
		}
		
		
		public static Drawing.Size MapVisualToParent(Visual visual, Drawing.Size value)
		{
			return value;
		}
		
		public static Drawing.Size MapVisualToRoot(Visual visual, Drawing.Size value)
		{
			return value;
		}
		
		public static Drawing.Size MapParentToVisual(Visual visual, Drawing.Size value)
		{
			return value;
		}
		public static Drawing.Size MapRootToVisual(Visual visual, Drawing.Size value)
		{
			return value;
		}
		
		
		public static Drawing.Point MapScreenToVisual(Visual visual, Drawing.Point value)
		{
			Window window = VisualTree.GetWindow (visual);
			return VisualTree.MapRootToVisual (visual, window.MapScreenToWindow (value));
		}
		
		public static Drawing.Point MapScreenToParent(Visual visual, Drawing.Point value)
		{
			Window window = VisualTree.GetWindow (visual);
			return VisualTree.MapVisualToParent (visual, VisualTree.MapRootToVisual (visual, window.MapScreenToWindow (value)));
		}
		
		public static Drawing.Point MapVisualToScreen(Visual visual, Drawing.Point value)
		{
			Window window = VisualTree.GetWindow (visual);
			return window.MapWindowToScreen (VisualTree.MapVisualToRoot (visual, value));
		}
		
		public static Drawing.Point MapParentToScreen(Visual visual, Drawing.Point value)
		{
			Window window = VisualTree.GetWindow (visual);
			return window.MapWindowToScreen (VisualTree.MapVisualToRoot (visual, VisualTree.MapParentToVisual (visual, value)));
		}
		
		
		public static Drawing.Rectangle MapVisualToParent(Visual visual, Drawing.Rectangle value)
		{
			bool flip_x = value.Width < 0;
			bool flip_y = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapVisualToParent (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapVisualToParent (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) value.FlipX ();
			if (flip_y) value.FlipY ();
			
			return value;
		}
		
		public static Drawing.Rectangle MapVisualToRoot(Visual visual, Drawing.Rectangle value)
		{
			bool flip_x = value.Width < 0;
			bool flip_y = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapVisualToRoot (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapVisualToRoot (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) value.FlipX ();
			if (flip_y) value.FlipY ();
			
			return value;
		}
		
		public static Drawing.Rectangle MapParentToVisual(Visual visual, Drawing.Rectangle value)
		{
			bool flip_x = value.Width < 0;
			bool flip_y = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapParentToVisual (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapParentToVisual (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) value.FlipX ();
			if (flip_y) value.FlipY ();
			
			return value;
		}
		
		public static Drawing.Rectangle MapRootToVisual(Visual visual, Drawing.Rectangle value)
		{
			bool flip_x = value.Width < 0;
			bool flip_y = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapRootToVisual (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapRootToVisual (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) value.FlipX ();
			if (flip_y) value.FlipY ();
			
			return value;
		}
		
		
		public static Drawing.Rectangle MapVisualToScreen(Visual visual, Drawing.Rectangle value)
		{
			bool flip_x = value.Width < 0;
			bool flip_y = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapVisualToScreen (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapVisualToScreen (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) value.FlipX ();
			if (flip_y) value.FlipY ();
			
			return value;
		}
		
		public static Drawing.Rectangle MapScreenToVisual(Visual visual, Drawing.Rectangle value)
		{
			bool flip_x = value.Width < 0;
			bool flip_y = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapScreenToVisual (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapScreenToVisual (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flip_x) value.FlipX ();
			if (flip_y) value.FlipY ();
			
			return value;
		}
		
		
		public static Window GetWindow(Visual visual)
		{
			for ( ; visual != null; )
			{
				if (visual is WindowRoot)
				{
					WindowRoot root = visual as WindowRoot;
					
					if (root != null)
					{
						return root.Window;
					}
				}
				
				visual = visual.Parent;
			}
			
			return null;
		}
		
		public static Visual GetRoot(Visual visual)
		{
			for (;;)
			{
				Visual parent = visual.Parent;
				
				if (parent == null)
				{
					return visual;
				}
				
				visual = parent;
			}
		}
		
		
		public static bool IsAncestor(Visual visual, Visual ancestor)
		{
			for (;;)
			{
				Visual parent = visual.Parent;
				
				if (parent == ancestor)
				{
					return true;
				}
				if (parent == null)
				{
					return false;
				}
				
				visual = parent;
			}
		}
		
		public static bool IsDescendant(Visual visual, Visual descendant)
		{
			if (descendant == null)
			{
				return false;
			}
			else
			{
				return VisualTree.IsAncestor (descendant, visual);
			}
		}
		
		public static bool IsVisible(Visual visual)
		{
			while ((visual != null) && (visual.Visibility))
			{
				if (visual is WindowRoot)
				{
					WindowRoot root = visual as WindowRoot;
					return true;
				}
				
				visual = visual.Parent;
			}
			
			return false;
		}
		
		public static bool IsEnabled(Visual visual)
		{
			while (visual.Enable)
			{
				visual = visual.Parent;
				
				if (visual == null)
				{
					return true;
				}
			}
			
			return false;
		}
	}
}
