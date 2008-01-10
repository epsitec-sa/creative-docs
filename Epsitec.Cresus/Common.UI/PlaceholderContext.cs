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
		private static Stack<Record>			RecordStack
		{
			get
			{
				if (PlaceholderContext.recordStack == null)
				{
					PlaceholderContext.recordStack = new Stack<Record> ();
				}
				
				return PlaceholderContext.recordStack;
			}
		}

		/// <summary>
		/// Gets the interactive placeholder, which is the one which started the
		/// current chain of binding events.
		/// </summary>
		/// <value>The interactive placeholder.</value>
		public static Placeholder				InteractivePlaceholder
		{
			get
			{
				if (PlaceholderContext.stackRoot == null)
				{
					return null;
				}
				else
				{
					return PlaceholderContext.stackRoot.Placeholder;
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
				Stack<Record> stack = PlaceholderContext.RecordStack;

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
		/// Pushes the specified controller onto the <see cref="Placeholder"/>
		/// stack.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public static void Push(Controllers.AbstractController controller)
		{
			Stack<Record> stack = PlaceholderContext.RecordStack;
			Record record = new Record (controller);

			if (PlaceholderContext.stackRoot == null)
			{
				System.Diagnostics.Debug.Assert (stack.Count == 0);
				
				PlaceholderContext.stackRoot = record;
			}

			PlaceholderContext.RecordStack.Push (record);
		}

		/// <summary>
		/// Pops the specified controller from the <see cref="Placeholder"/>
		/// stack.
		/// </summary>
		/// <param name="controller">The controller.</param>
		public static void Pop(Controllers.AbstractController controller)
		{
			Stack<Record> stack = PlaceholderContext.RecordStack;

			if ((stack.Count == 0) ||
				(stack.Peek ().Controller != controller))
			{
				throw new System.InvalidOperationException ("Pop does not match Push");
			}

			Record record = stack.Pop ();

			if (stack.Count == 0)
			{
				System.Diagnostics.Debug.Assert (PlaceholderContext.stackRoot == record);

				PlaceholderContext.stackRoot = null;
			}
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

		#region ActiveHelper Class

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

		[System.ThreadStatic]
		private static Stack<Record>			recordStack;

		[System.ThreadStatic]
		private static Record					stackRoot;
	}
}
