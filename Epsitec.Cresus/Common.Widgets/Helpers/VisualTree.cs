//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		
		public static void InvalidateCommandDispatcher(Window window)
		{
			if (window != null)
			{
				VisualTree.InvalidateCommandDispatcher (window.Root);
				
				foreach (Window owned in window.OwnedWindows)
				{
					VisualTree.InvalidateCommandDispatcher (owned);
				}
			}
		}
		
		public static void InvalidateCommandDispatcher(Visual visual)
		{
			if (visual != null)
			{
				CommandCache.Default.Invalidate (visual);
				
				if (visual.HasChildren)
				{
					foreach (Visual child in visual.Children)
					{
						VisualTree.InvalidateCommandDispatcher (child);
					}
				}
			}
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
					WindowRoot root   = visual as WindowRoot;
					Window     window = root.Window;
					
					return window == null ? false : window.IsVisible;
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
		
		public static bool IsFocused(Visual visual)
		{
			//	Retourne true si un widget, ou l'un de ses parents (pour autant
			//	que l'héritage soit activé) contient le focus.
			
			while (visual != null)
			{
				if (visual.IsKeyboardFocused)
				{
					Window window = VisualTree.GetWindow (visual);
					return window == null ? false : window.IsFocused;
				}
				
				if (visual.InheritParentFocus)
				{
					visual = visual.Parent;
				}
				else
				{
					break;
				}
			}
			
			return false;
		}
		
		
		public static bool ContainsKeyboardFocus(Visual visual)
		{
			//	Retourne true si un widget, ou l'un de ses enfants, contient le
			//	focus du clavier.
			
			if (visual != null)
			{
				if (visual.IsKeyboardFocused)
				{
					return true;
				}
				
				if (visual.HasChildren)
				{
					Visual[] children = visual.Children.ToArray ();
					int  children_num = children.Length;
					
					for (int i = 0; i < children_num; i++)
					{
						if (VisualTree.ContainsKeyboardFocus (children[i]))
						{
							return true;
						}
					}
				}
			}	
			
			return false;
		}
		
		
		public static Visual FindParentUsingEvent(Visual visual, Types.Property property)
		{
			//	Cherche le premier parent dans la hiérarchie pour lequel un
			//	événement a été attaché pour la propriété spécifiée.
			
			if (visual != null)
			{
				visual = visual.Parent;
				
				while (visual != null)
				{
					if (visual.IsEventUsed (property))
					{
						return visual;
					}
					
					visual = visual.Parent;
				}
			}
			
			return null;
		}
		
		
		public static VisualTreeSnapshot SnapshotProperties(Visual visual, Types.Property property)
		{
			VisualTreeSnapshot snapshot = new VisualTreeSnapshot ();
			
			snapshot.Record (visual, property);
			VisualTree.SnapshotChildrenProperties (snapshot, visual, property);
			
			return snapshot;
		}
		
		public static VisualTreeSnapshot SnapshotChildrenProperties(Visual visual, Types.Property property)
		{
			VisualTreeSnapshot snapshot = new VisualTreeSnapshot ();
			
			VisualTree.SnapshotChildrenProperties (snapshot, visual, property);
			
			return snapshot;
		}
		
		
		private static void SnapshotChildrenProperties(VisualTreeSnapshot snapshot, Visual visual, Types.Property property)
		{
			if (visual.HasChildren)
			{
				foreach (Visual child in visual.GetChildrenCollection ())
				{
					snapshot.Record (child, property);
					
					VisualTree.SnapshotChildrenProperties (snapshot, child, property);
				}
			}
		}
	}
}
