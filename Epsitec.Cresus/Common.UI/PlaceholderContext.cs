//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>PlaceholderContext</c> class provides information about the
	/// <see cref="Placeholder"/> which is currently being edited by the
	/// user.
	/// </summary>
	public static class PlaceholderContext
	{
		/// <summary>
		/// Gets the interactive placeholder, which is the one which started the
		/// current chain of binding events.
		/// </summary>
		/// <value>The interactive placeholder.</value>
		public static Placeholder				InteractivePlaceholder
		{
			get
			{
				PlaceholderContext.EnsureData ();
				
				if (PlaceholderContext.data.StackRoot == null)
				{
					return null;
				}
				else
				{
					return PlaceholderContext.data.StackRoot.Placeholder;
				}
			}
		}

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

		/// <summary>
		/// Pushes the specified controller onto the <see cref="Placeholder"/>
		/// stack.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public static void Push(Controllers.AbstractController controller)
		{
			PlaceholderContext.EnsureData ();
			PlaceholderContext.data.Push (controller);
		}

		/// <summary>
		/// Pops the specified controller from the <see cref="Placeholder"/>
		/// stack.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public static void Pop(Controllers.AbstractController controller)
		{
			PlaceholderContext.EnsureData ();
			PlaceholderContext.data.Pop (controller);
		}

		/// <summary>
		/// Sets the active.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <returns></returns>
		public static System.IDisposable SetActive(Controllers.AbstractController controller)
		{
			return new ActiveHelper (controller);
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
		/// the active context and used by method <see cref="SetActive"/>.
		/// </summary>
		private sealed class ActiveHelper : System.IDisposable
		{
			public ActiveHelper(Controllers.AbstractController controller)
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

			private readonly Controllers.AbstractController controller;
		}

		#endregion

		#region Record Class

		private class Record
		{
			public Record(Controllers.AbstractController controller)
			{
				this.Controller = controller;
				this.Placeholder = controller.Placeholder;
			}

			public Controllers.AbstractController Controller
			{
				get;
				private set;
			}

			public Placeholder Placeholder
			{
				get;
				private set;
			}
		}

		#endregion

		#region ThreadData Class
		
		private class ThreadData
		{
			public ThreadData()
			{
				this.stack = new Stack<Record> ();
			}

			public Stack<Record> Stack
			{
				get
				{
					return this.stack;
				}
			}

			public Record StackRoot
			{
				get
				{
					return this.stackRoot;
				}
				set
				{
					this.stackRoot = value;
				}
			}

			public void Push(Controllers.AbstractController controller)
			{
				Stack<Record> stack = this.stack;
				Record record = new Record (controller);

				if (this.stackRoot == null)
				{
					System.Diagnostics.Debug.Assert (stack.Count == 0);

					this.stackRoot = record;
				}

				this.stack.Push (record);
				
				this.OnStackPushed (controller);
			}

			public void Pop(Controllers.AbstractController controller)
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
					System.Diagnostics.Debug.Assert (this.stackRoot == record);

					this.stackRoot = null;
				}

				this.OnStackPopped (controller);
			}

			private void OnStackPushed(Controllers.AbstractController controller)
			{
				if (this.StackPushed != null)
				{
					this.StackPushed (controller);
				}
			}

			private void OnStackPopped(Controllers.AbstractController controller)
			{
				if (this.StackPopped != null)
				{
					this.StackPopped (controller);
				}
			}

			public event Support.EventHandler	StackPushed;
			public event Support.EventHandler	StackPopped;
			
			private readonly Stack<Record>		stack;
			private Record						stackRoot;
		}

		#endregion

		[System.ThreadStatic]
		private static ThreadData				data;
	}
}
