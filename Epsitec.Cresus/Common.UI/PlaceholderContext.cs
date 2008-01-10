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
		public static void Push(Controllers.AbstractController controller)
		{
			Stack<Record> stack = PlaceholderContext.RecordStack;
			Record record = new Record (controller);

			if (PlaceholderContext.stackRoot == null)
			{
				System.Diagnostics.Debug.Assert (stack.Count == 0));
				
				PlaceholderContext.stackRoot = record;
			}

			PlaceholderContext.RecordStack.Push (record);
		}

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

		public static Placeholder GetInteractivePlaceholder()
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

		public static Placeholder GetActivePlaceholder()
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


		private static Stack<Record> RecordStack
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

		[System.ThreadStatic]
		private static Stack<Record>			recordStack;

		[System.ThreadStatic]
		private static Record					stackRoot;
	}
}
     