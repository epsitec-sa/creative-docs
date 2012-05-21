//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>PlaceholderContext</c> class provides information about the
	/// <see cref="Placeholder"/> which is currently being edited by the
	/// user (see <see cref="Controllers.AbstractController"/>).
	/// </summary>
	public static class PlaceholderContext
	{
		/// <summary>
		/// Gets the active placeholder, which is the one which is currently
		/// generating the binding event.
		/// </summary>
		/// <value>The active placeholder.</value>
		public static Placeholder				ActivePlaceholder
		{
			get
			{
				PlaceholderContext.EnsureData ();
				
				Stack<Record> stack = PlaceholderContext.data.Stack;

				if (stack.Count == 0)
				{
					return null;
				}
				else
				{
					return stack.Peek ().Placeholder;
				}
			}
		}

		/// <summary>
		/// Gets the context depth.
		/// </summary>
		/// <value>The context depth.</value>
		public static int						Depth
		{
			get
			{
				PlaceholderContext.EnsureData ();

				return PlaceholderContext.data.Stack.Count;
			}
		}

		/// <summary>
		/// Sets the active controller.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <returns>A disposable object to use in a <c>using</c> block.</returns>
		public static System.IDisposable SetActive(IController controller)
		{
			return new ActiveHelper (controller);
		}

		/// <summary>
		/// Sets the active controller.
		/// </summary>
		/// <param name="placeholder">The placeholder.</param>
		/// <returns>A disposable object to use in a <c>using</c> block.</returns>
		public static System.IDisposable SetActive(AbstractPlaceholder placeholder)
		{
			return new ActiveHelper (placeholder.ControllerInstance);
		}

		/// <summary>
		/// Gets the interactive placeholder, which is the one which started the
		/// current chain of binding events. The search is done for the specified
		/// window.
		/// </summary>
		/// <param name="window">The window.</param>
		/// <returns>The interactive placeholder.</returns>
		public static Placeholder GetInteractivePlaceholder(Window window)
		{
			PlaceholderContext.EnsureData ();

			Record record = PlaceholderContext.data.GetStackRoot (window.GetWindowSerialId ());

			if (record == null)
			{
				return null;
			}
			else
			{
				return record.Placeholder;
			}
		}

		/// <summary>
		/// Determines whether the context contains the specified controller.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <returns>
		/// 	<c>true</c> if the context contains the specified controller; otherwise, <c>false</c>.
		/// </returns>
		internal static bool Contains(IController controller)
		{
			PlaceholderContext.EnsureData ();

			foreach (Record record in PlaceholderContext.data.Stack)
			{
				if (record.Controller == controller)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Pushes the specified controller onto the <see cref="Placeholder"/>
		/// stack.
		/// </summary>
		/// <param name="controller">The controller.</param>
		private static void Push(IController controller)
		{
			PlaceholderContext.EnsureData ();
			PlaceholderContext.data.Push (controller);
		}

		/// <summary>
		/// Pops the specified controller from the <see cref="Placeholder"/>
		/// stack.
		/// </summary>
		/// <param name="controller">The controller.</param>
		private static void Pop(IController controller)
		{
			PlaceholderContext.EnsureData ();
			PlaceholderContext.data.Pop (controller);
		}

		/// <summary>
		/// Ensures the thread static data is initialized.
		/// </summary>
		private static void EnsureData()
		{
			if (PlaceholderContext.data	== null)
			{
				PlaceholderContext.data = new ThreadData ();
			}
		}

		#region ActiveHelper Class

		/// <summary>
		/// The <c>ActiveHelper</c> class is used to automatically push/pop
		/// the active context and used by method <see cref="SetActive(IController)"/>
		/// and <see cref="SetActive(AbstractPlaceholder)"/>.
		/// </summary>
		private sealed class ActiveHelper : System.IDisposable
		{
			public ActiveHelper(IController controller)
			{
				this.controller = controller;

				PlaceholderContext.Push (this.controller);
			}

			~ActiveHelper()
			{
				throw new System.InvalidOperationException ("Caller of SetActive forgot to call Dispose");
			}

			#region IDisposable Members

			public void Dispose()
			{
				PlaceholderContext.Pop (this.controller);
				System.GC.SuppressFinalize (this);
			}

			#endregion

			private readonly IController controller;
		}

		#endregion

		#region Record Class

		private class Record
		{
			public Record(IController controller)
			{
				this.Controller = controller;
				this.Placeholder = controller.Placeholder;
			}

			public IController Controller
			{
				get;
				private set;
			}

			public Placeholder Placeholder
			{
				get;
				private set;
			}

			public long WindowId
			{
				get
				{
					if ((this.Placeholder == null) ||
						(this.Placeholder.Window == null))
					{
						return 0;
					}
					else
					{
						return this.Placeholder.Window.GetWindowSerialId ();
					}
				}
			}
		}

		#endregion

		#region ThreadData Class
		
		private class ThreadData
		{
			public ThreadData()
			{
				this.stack = new Stack<Record> ();
				this.stackRoots = new Dictionary<long, Record> ();
			}

			public Stack<Record> Stack
			{
				get
				{
					return this.stack;
				}
			}

			public Record GetStackRoot(long windowId)
			{
				Record record;
				
				if (this.stackRoots.TryGetValue (windowId, out record))
				{
					return record;
				}
				else
				{
					return null;
				}
			}

			public void Push(IController controller)
			{
				Stack<Record> stack = this.stack;
				Record record = new Record (controller);

				if (this.GetStackRoot (record.WindowId) == null)
				{
					System.Diagnostics.Debug.Assert (stack.Count == 0);

					this.stackRoots[record.WindowId] = record;
				}

				this.stack.Push (record);
				
				this.OnStackPushed (controller);
			}

			public void Pop(IController controller)
			{
				Stack<Record> stack = this.stack;

				if ((stack.Count == 0) ||
					(stack.Peek ().Controller != controller))
				{
					throw new System.InvalidOperationException ("Pop does not match Push");
				}

				Record record = stack.Pop ();

				if (stack.Count == 0)
				{
					System.Diagnostics.Debug.Assert (this.GetStackRoot (record.WindowId) == record);

					this.stackRoots.Remove (record.WindowId);
				}

				this.OnStackPopped (controller);
			}

			private void OnStackPushed(IController controller)
			{
				if (this.StackPushed != null)
				{
					this.StackPushed (controller);
				}
			}

			private void OnStackPopped(IController controller)
			{
				if (this.StackPopped != null)
				{
					this.StackPopped (controller);
				}
			}

			public event Support.EventHandler	StackPushed;
			public event Support.EventHandler	StackPopped;
			
			readonly Stack<Record>				stack;
			readonly Dictionary<long, Record>	stackRoots;
		}

		#endregion

		/// <summary>
		/// Occurs when a context is activated.
		/// </summary>
		public static event Support.EventHandler ContextActivated
		{
			add
			{
				PlaceholderContext.EnsureData ();
				PlaceholderContext.data.StackPushed += value;
			}
			remove
			{
				PlaceholderContext.EnsureData ();
				PlaceholderContext.data.StackPushed -= value;
			}
		}

		/// <summary>
		/// Occurs when a context is deactivated.
		/// </summary>
		public static event Support.EventHandler ContextDeactivated
		{
			add
			{
				PlaceholderContext.EnsureData ();
				PlaceholderContext.data.StackPopped += value;
			}
			remove
			{
				PlaceholderContext.EnsureData ();
				PlaceholderContext.data.StackPopped -= value;
			}
		}

		[System.ThreadStatic]
		private static ThreadData				data;
	}
}
