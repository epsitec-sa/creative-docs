//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		public static Drawing.Point MapVisualToAncestor(Visual visual, Visual ancestor, Drawing.Point value)
		{
			if (ancestor == null)
			{
				return value;
			}
			while (visual != ancestor)
			{
				if (visual == null)
				{
					throw new System.ArgumentException ("Ancestor is not valid for this visual");
				}
				
				value  = VisualTree.MapVisualToParent (visual, value);
				visual = visual.Parent;
			}
			
			return value;
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


		public static Drawing.Rectangle MapVisualToAncestor(Visual visual, Visual ancestor, Drawing.Rectangle value)
		{
			if (ancestor == null)
			{
				return value;
			}
			while (visual != ancestor)
			{
				if (visual == null)
				{
					throw new System.ArgumentException ("Ancestor is not valid for this visual");
				}

				value  = VisualTree.MapVisualToParent (visual, value);
				visual = visual.Parent;
			}

			return value;
		}

		public static Drawing.Rectangle MapVisualToParent(Visual visual, Drawing.Rectangle value)
		{
			bool flipX = value.Width < 0;
			bool flipY = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapVisualToParent (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapVisualToParent (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) value.FlipX ();
			if (flipY) value.FlipY ();
			
			return value;
		}
		
		public static Drawing.Rectangle MapVisualToRoot(Visual visual, Drawing.Rectangle value)
		{
			bool flipX = value.Width < 0;
			bool flipY = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapVisualToRoot (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapVisualToRoot (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) value.FlipX ();
			if (flipY) value.FlipY ();
			
			return value;
		}
		
		public static Drawing.Rectangle MapParentToVisual(Visual visual, Drawing.Rectangle value)
		{
			bool flipX = value.Width < 0;
			bool flipY = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapParentToVisual (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapParentToVisual (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) value.FlipX ();
			if (flipY) value.FlipY ();
			
			return value;
		}
		
		public static Drawing.Rectangle MapRootToVisual(Visual visual, Drawing.Rectangle value)
		{
			bool flipX = value.Width < 0;
			bool flipY = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapRootToVisual (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapRootToVisual (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) value.FlipX ();
			if (flipY) value.FlipY ();
			
			return value;
		}
		
		
		public static Drawing.Rectangle MapVisualToScreen(Visual visual, Drawing.Rectangle value)
		{
			bool flipX = value.Width < 0;
			bool flipY = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapVisualToScreen (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapVisualToScreen (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) value.FlipX ();
			if (flipY) value.FlipY ();
			
			return value;
		}
		
		public static Drawing.Rectangle MapScreenToVisual(Visual visual, Drawing.Rectangle value)
		{
			bool flipX = value.Width < 0;
			bool flipY = value.Height < 0;
			
			Drawing.Point p1 = VisualTree.MapScreenToVisual (visual, value.BottomLeft);
			Drawing.Point p2 = VisualTree.MapScreenToVisual (visual, value.TopRight);
			
			value.X = System.Math.Min (p1.X, p2.X);
			value.Y = System.Math.Min (p1.Y, p2.Y);
			
			value.Width  = System.Math.Abs (p1.X - p2.X);
			value.Height = System.Math.Abs (p1.Y - p2.Y);
			
			if (flipX) value.FlipX ();
			if (flipY) value.FlipY ();
			
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
				CommandCache.Instance.InvalidateVisual (visual);
				
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


		public static Support.ICaptionResolver GetCaptionResolver(Visual visual)
		{
			return VisualTree.FindCaptionResolver (visual) ?? Support.Resources.DefaultManager;
		}

		public static Support.ICaptionResolver FindCaptionResolver(Visual visual)
		{
			Support.ICaptionResolver resolver = null;

			while (visual != null)
			{
				resolver = visual.CaptionResolver ?? Support.ResourceManager.GetResourceManager (visual);

				if (resolver != null)
				{
					break;
				}

				WindowRoot root = visual as WindowRoot;

				if (root != null)
				{
					resolver = Support.ResourceManager.GetResourceManager (root.Window);
					break;
				}

				ILogicalTree logicalTree = visual as ILogicalTree;

				if (logicalTree == null)
				{
					visual = visual.Parent;
				}
				else
				{
					visual = logicalTree.Parent;
				}
			}

			return resolver;
		}

		/// <summary>
		/// Gets the resource manager for a specified <see cref="Visual"/> instance.
		/// </summary>
		/// <param name="visual">The visual.</param>
		/// <returns>The specific resource manager if one is defined; otherwise,
		/// <c>Epsitec.Common.Support.Resources.DefaultManager</c>.</returns>
		public static Support.ResourceManager GetResourceManager(Visual visual)
		{
			return VisualTree.FindResourceManager (visual) ?? Support.Resources.DefaultManager;
		}

		/// <summary>
		/// Finds the resource manager for a specified <see cref="Visual"/> instance.
		/// </summary>
		/// <param name="visual">The visual.</param>
		/// <returns>The specific resource manager if one is defined; otherwise, <c>null</c>.</returns>
		public static Support.ResourceManager FindResourceManager(Visual visual)
		{
			while (visual != null)
			{
				Support.ResourceManager manager = Support.ResourceManager.GetResourceManager (visual);

				if (manager != null)
				{
					return manager;
				}

				WindowRoot root = visual as WindowRoot;

				if (root != null)
				{
					manager = Support.ResourceManager.GetResourceManager (root.Window);

					if (manager != null)
					{
						return manager;
					}

					break;
				}

				ILogicalTree logicalTree = visual as ILogicalTree;
				
				if (logicalTree == null)
				{
					visual = visual.Parent;
				}
				else
				{
					visual = logicalTree.Parent;
				}
			}
			
			return null;
		}

		public static CommandContext GetCommandContext(Visual visual)
		{
			CommandContextChain chain = CommandContextChain.BuildChain (visual);

			if ((chain == null) ||
				(chain.IsEmpty))
			{
				return null;
			}
			else
			{
				return chain.FirstContext;
			}
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

				Visual parent;
				
				ILogicalTree logicalTree = visual as ILogicalTree;
				
				if (logicalTree == null)
				{
					parent = visual.Parent;
				}
				else
				{
					parent = logicalTree.Parent;
				}

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

		/// <summary>
		/// Gets the validation groups for the specified visual, including all
		/// its parents, stopping at the window root.
		/// </summary>
		/// <param name="visual">The visual.</param>
		/// <returns>The concatenated validation groups or <c>null</c> if no
		/// validation group can be found at all.</returns>
		public static string GetValidationGroups(Visual visual)
		{
			HashSet<string> groups = new HashSet<string> ();
			
			while (visual != null)
			{
				if (visual.HasValidationGroups)
				{
					foreach (string group in Command.SplitGroupNames (visual.ValidationGroups))
					{
						groups.Add (group);
					}
				}

				visual = visual.Parent;
			}

			if (groups.Count > 0)
			{
				string[] names = Types.Collection.ToArray (groups);

				System.Array.Sort (names);

				return Command.JoinGroupNames (names);
			}
			else
			{
				return null;
			}
		}

		public static void RefreshValidationContext(Visual visual)
		{
			ValidationContext context = VisualTree.GetValidationContext (visual);

			if (context != null)
			{
				context.Refresh (visual);
			}
		}

		public static void UpdateCommandEnable(Visual visual)
		{
			ValidationContext context = VisualTree.GetValidationContext (visual);

			if (context != null)
			{
				context.UpdateCommandEnableBasedOnVisualValidity (visual);
			}
		}

		public static Application GetApplication(Window window)
		{
			while (window != null)
			{
				Application application = Application.GetApplication (window);

				if (application != null)
				{
					return application;
				}

				window = window.Owner;
			}

			return null;
		}


		public static Support.OpletQueue GetOpletQueue(Visual visual)
		{
			CommandDispatcherChain chain = CommandDispatcherChain.BuildChain (visual);

			if (chain != null)
			{
				foreach (CommandDispatcher dispatcher in chain.Dispatchers)
				{
					if (dispatcher.OpletQueue != null)
					{
						return dispatcher.OpletQueue;
					}
				}
			}
			
			return null;
		}
		
		
		public static bool IsAncestor(Visual visual, Visual ancestor)
		{
			if (visual == null)
			{
				return false;
			}

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

				if (root != null)
				{
					return root.DoesVisualContainKeyboardFocus (visual);
				}
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
				ILogicalTree logicalTree = visual as ILogicalTree;

				if (logicalTree == null)
				{
					visual = visual.Parent;
				}
				else
				{
					visual = logicalTree.Parent;
				}
				
				while (visual != null)
				{
					if (visual.HasEventHandlerForProperty (property))
					{
						return visual;
					}

					logicalTree = visual as ILogicalTree;

					if (logicalTree == null)
					{
						visual = visual.Parent;
					}
					else
					{
						visual = logicalTree.Parent;
					}
				}
			}
			
			return null;
		}

		public static void DebugDump(Visual root, int depth)
		{
			string indent = new string (' ', depth*2);
			
			if (root != null)
			{
				Layouts.LayoutMeasure measureDx;
				Layouts.LayoutMeasure measureDy;

				root.GetMeasures (out measureDx, out measureDy);

				System.Diagnostics.Debug.WriteLine (string.Format ("{0}{1}", indent, root.ToString ()));
				System.Diagnostics.Debug.WriteLine (string.Format ("  {0}Dock={1}, Anchor={2}, Visibility={3}, IsVisible={4}", indent, root.Dock, root.Anchor, root.Visibility, root.IsVisible));
				System.Diagnostics.Debug.WriteLine (string.Format ("  {0}Bounds={1} -> {2}", indent, root.ActualBounds, VisualTree.MapVisualToRoot (root, root.Client.Bounds)));
				System.Diagnostics.Debug.WriteLine (string.Format ("  {0}Measure DX={1}", indent, measureDx == null ? "none" : measureDx.ToString ()));
				System.Diagnostics.Debug.WriteLine (string.Format ("  {0}Measure DY={1}", indent, measureDy == null ? "none" : measureDy.ToString ()));

				Widget widget = root as Widget;

				if (widget != null)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("  {0}TabIndex={1}, {2}", indent, widget.TabIndex, widget.TabNavigationMode));
				}

				if (root.HasChildren)
				{
					foreach (Visual child in root.Children)
					{
						VisualTree.DebugDump (child, depth+1);
					}
				}
			}
		}
		
		#region AddToDispatcherList (Private Methods)
		
		private static void AddToDispatcherList(List<CommandDispatcher> list, Visual visual)
		{
			while (visual != null)
			{
				VisualTree.AddToDispatcherList (list, CommandDispatcher.GetDispatcher (visual));
				
				ILogicalTree logicalTree = visual as ILogicalTree;

				if (logicalTree == null)
				{
					visual = visual.Parent;
				}
				else
				{
					visual = logicalTree.Parent;
				}
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
