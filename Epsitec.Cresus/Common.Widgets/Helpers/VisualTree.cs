//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

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
			Drawing.Rectangle bounds = visual.ActualBounds;
			
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
				Drawing.Rectangle bounds = visual.ActualBounds;
				
				x += bounds.Left;
				y += bounds.Bottom;
				
				visual = visual.Parent;
			}
			
			return new Drawing.Point (x, y);
		}
		
		public static Drawing.Point MapParentToVisual(Visual visual, Drawing.Point value)
		{
			Drawing.Rectangle bounds = visual.ActualBounds;
			
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
				Drawing.Rectangle bounds = visual.ActualBounds;
				
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
				CommandCache.Default.InvalidateVisual (visual);
				
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

		public static WindowRoot GetWindowRoot(Visual visual)
		{
			return VisualTree.GetRoot (visual) as WindowRoot;
		}

		public static CommandContext GetCommandContext(Visual visual)
		{
			while (visual != null)
			{
				CommandContext context = CommandContext.GetContext (visual);

				if (context != null)
				{
					return context;
				}

				Visual parent = visual.Parent;

				if (parent == null)
				{
					return VisualTree.GetCommandContext (visual.Window);
				}

				visual = parent;
			}

			return null;
		}

		public static CommandContext GetCommandContext(Window window)
		{
			while (window != null)
			{
				CommandContext context = CommandContext.GetContext (window);

				if (context != null)
				{
					return context;
				}

				window = window.Owner;
			}

			return null;
		}

		public static ValidationContext GetValidationContext(Visual visual)
		{
			while (visual != null)
			{
				ValidationContext context = ValidationContext.GetContext (visual);

				if (context != null)
				{
					return context;
				}

				Visual parent = visual.Parent;

				if (parent == null)
				{
					return VisualTree.GetValidationContext (visual.Window);
				}

				visual = parent;
			}

			return null;
		}

		public static ValidationContext GetValidationContext(Window window)
		{
			while (window != null)
			{
				ValidationContext context = ValidationContext.GetContext (window);

				if (context != null)
				{
					return context;
				}

				window = window.Owner;
			}

			return null;
		}

		public static void RefreshValidationContext(Visual visual)
		{
			ValidationContext context = VisualTree.GetValidationContext (visual);

			if (context != null)
			{
				context.Refresh (visual);
			}
		}
		

		public static Support.OpletQueue GetOpletQueue(Visual visual)
		{
			CommandDispatcherChain chain = CommandDispatcherChain.BuildChain (visual);
			
			foreach (CommandDispatcher dispatcher in chain.Dispatchers)
			{
				if (dispatcher.OpletQueue != null)
				{
					return dispatcher.OpletQueue;
				}
			}
			
			return null;
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

		public static bool ContainsKeyboardFocus(Visual visual)
		{
			//	Retourne true si un widget, ou l'un de ses enfants, contient le
			//	focus du clavier.

			if (visual != null)
			{
				if (visual.KeyboardFocus)
				{
					return true;
				}

				WindowRoot root = VisualTree.GetWindowRoot (visual);

				return root.DoesVisualContainKeyboardFocus (visual);
			}	
			
			return false;
		}

		public static int GetDepth(Visual visual)
		{
			if (visual != null)
			{
				visual = visual.Parent;

				int depth = 1;

				while (visual != null)
				{
					depth++;
					visual = visual.Parent;
				}

				return depth;
			}
			else
			{
				return 0;
			}
		}

		public static Layouts.LayoutContext GetLayoutContext(Visual visual)
		{
			int depth;
			return VisualTree.GetLayoutContext (visual, out depth);
		}
		public static Layouts.LayoutContext GetLayoutContext(Visual visual, out int depth)
		{
			depth = 0;
			
			if (visual == null)
			{
				return null;
			}

			Visual parent = visual.Parent;
			depth++;
			
			while (parent != null)
			{
				visual = parent;
				parent = visual.Parent;
				depth++;
			}

			Layouts.LayoutContext context;

			context = Layouts.LayoutContext.GetLayoutContext (visual);

			if (context == null)
			{
				context = new Layouts.LayoutContext ();
				Layouts.LayoutContext.SetLayoutContext (visual, context);
			}

			return context;
		}

		public static Layouts.LayoutContext FindLayoutContext(Visual visual)
		{
			if (visual == null)
			{
				return null;
			}

			Visual parent = visual.Parent;

			while (parent != null)
			{
				visual = parent;
				parent = visual.Parent;
			}

			return Layouts.LayoutContext.GetLayoutContext (visual);
		}
		public static Layouts.LayoutContext FindLayoutContext(Visual visual, out int depth)
		{
			depth = 0;

			if (visual == null)
			{
				return null;
			}

			Visual parent = visual.Parent;
			depth++;

			while (parent != null)
			{
				visual = parent;
				parent = visual.Parent;
				depth++;
			}

			return Layouts.LayoutContext.GetLayoutContext (visual);
		}

		public static Visual FindParentUsingEvent(Visual visual, Types.DependencyProperty property)
		{
			//	Cherche le premier parent dans la hiérarchie pour lequel un
			//	événement a été attaché pour la propriété spécifiée.
			
			if (visual != null)
			{
				visual = visual.Parent;
				
				while (visual != null)
				{
					if (visual.HasEventHandlerForProperty (property))
					{
						return visual;
					}
					
					visual = visual.Parent;
				}
			}
			
			return null;
		}
		
		#region AddToDispatcherList (Private Methods)
		private static void AddToDispatcherList(List<CommandDispatcher> list, Visual visual)
		{
			while (visual != null)
			{
				VisualTree.AddToDispatcherList (list, CommandDispatcher.GetDispatcher (visual));
				visual = visual.Parent;
			}
		}

		private static void AddToDispatcherList(List<CommandDispatcher> list, Window window)
		{
			while (window != null)
			{
				Widget parent = MenuWindow.GetParentWidget (window);
				
				if (parent != null)
				{
					VisualTree.AddToDispatcherList (list, parent);
				}
				
				VisualTree.AddToDispatcherList (list, CommandDispatcher.GetDispatcher (window));
				window = window.Owner;
			}
		}

		private static void AddToDispatcherList(List<CommandDispatcher> list, IEnumerable<CommandDispatcher> dispatchers)
		{
			foreach (CommandDispatcher dispatcher in dispatchers)
			{
				if (list.Contains (dispatcher) == false)
				{
					VisualTree.AddToDispatcherList (list, dispatcher);
				}
			}
		}

		private static void AddToDispatcherList(List<CommandDispatcher> list, CommandDispatcher dispatcher)
		{
			if (dispatcher != null)
			{
				if (list.Contains (dispatcher) == false)
				{
					list.Add (dispatcher);
					VisualTree.AddToDispatcherList (list, CommandDispatcher.GetDispatcher (dispatcher));
				}
			}
		}
		#endregion
	}
}
