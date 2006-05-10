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

		public static WindowRoot GetWindowRoot(Visual visual)
		{
			return VisualTree.GetRoot (visual) as WindowRoot;
		}
		
		
		public static Support.OpletQueue GetOpletQueue(Visual visual)
		{
			CommandDispatcher[] dispatchers = VisualTree.GetAllDispatchers (visual);
			
			foreach (CommandDispatcher dispatcher in dispatchers)
			{
				if (dispatcher.OpletQueue != null)
				{
					return dispatcher.OpletQueue;
				}
			}
			
			return null;
		}
		
		
		public static CommandDispatcher[] GetDispatchers(CommandDispatcher dispatcher)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			VisualTree.GetDispatchers (list, dispatcher);
			
			return (CommandDispatcher[]) list.ToArray (typeof (CommandDispatcher));
		}
		
		public static CommandDispatcher[] GetDispatchers(Visual visual)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			VisualTree.GetDispatchers (list, visual);
			VisualTree.GetDispatchers (list, Helpers.VisualTree.GetWindow (visual));
			
			return (CommandDispatcher[]) list.ToArray (typeof (CommandDispatcher));
		}
		
		public static CommandDispatcher[] GetAllDispatchers(Visual visual)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			VisualTree.GetDispatchers (list, visual);
			VisualTree.GetDispatchers (list, Helpers.VisualTree.GetWindow (visual));
			VisualTree.GetDispatchers (list, CommandDispatcher.GetFocusedPrimaryDispatcher ());
			
			return (CommandDispatcher[]) list.ToArray (typeof (CommandDispatcher));
		}
		
		

		public static CommandState GetCommandState(Visual visual)
		{
			if (visual == null)
			{
				return null;
			}
			
			string name = visual.CommandName;
			
			if (name == null)
			{
				return null;
			}
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			VisualTree.GetDispatchers (list, visual);
			VisualTree.GetDispatchers (list, Helpers.VisualTree.GetWindow (visual));
			VisualTree.GetDispatchers (list, CommandDispatcher.GetFocusedPrimaryDispatcher ());
			
			foreach (CommandDispatcher dispatcher in list)
			{
				CommandState command = dispatcher.FindCommandState (name);
				
				if (command != null)
				{
					return command;
				}
			}
			
			foreach (CommandDispatcher dispatcher in list)
			{
				if (dispatcher.ContainsCommandHandler (name))
				{
					System.Diagnostics.Debug.WriteLine ("Command '" + name + "' created in dispatcher '" + dispatcher.Name + "'.");
					return dispatcher.GetCommandState (name);
				}
			}
			
			return null;
		}
		
		public static CommandState GetCommandState(Shortcut shortcut, Visual context)
		{
			if (context == null)
			{
				return null;
			}
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			VisualTree.GetDispatchers (list, context);
			VisualTree.GetDispatchers (list, Helpers.VisualTree.GetWindow (context));
			VisualTree.GetDispatchers (list, CommandDispatcher.GetFocusedPrimaryDispatcher ());
			
			foreach (CommandDispatcher dispatcher in list)
			{
				CommandState command = dispatcher.FindCommandState (shortcut);
				
				if (command != null)
				{
					return command;
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
		
		#region GetDispatchers (Private Methods)
		private static void GetDispatchers(System.Collections.ArrayList list, Visual visual)
		{
			while (visual != null)
			{
				VisualTree.GetDispatchers (list, CommandDispatcher.GetDispatcher (visual));
				visual = visual.Parent;
			}
		}
		
		private static void GetDispatchers(System.Collections.ArrayList list, Window window)
		{
			while (window != null)
			{
				Widget parent = MenuWindow.GetParentWidget (window);
				
				if (parent != null)
				{
					VisualTree.GetDispatchers (list, parent);
				}
				
				VisualTree.GetDispatchers (list, CommandDispatcher.GetDispatcher (window));
				window = window.Owner;
			}
		}
		
		private static void GetDispatchers(System.Collections.ArrayList list, IEnumerable<CommandDispatcher> dispatchers)
		{
			foreach (CommandDispatcher dispatcher in dispatchers)
			{
				if (list.Contains (dispatcher) == false)
				{
					VisualTree.GetDispatchers (list, dispatcher);
				}
			}
		}
		
		private static void GetDispatchers(System.Collections.ArrayList list, CommandDispatcher dispatcher)
		{
			if ((dispatcher != null) &&
				(list.Contains (dispatcher) == false))
			{
				list.Add (dispatcher);
				VisualTree.GetDispatchers (list, CommandDispatcher.GetDispatcher (dispatcher));
			}
		}
		#endregion
	}
}
