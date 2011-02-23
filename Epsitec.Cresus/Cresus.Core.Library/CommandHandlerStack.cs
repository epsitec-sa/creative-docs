//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public class CommandHandlerStack : System.IEquatable<CommandHandlerStack>
	{
		public CommandHandlerStack(Command command)
		{
			this.command = command;
			this.handlers = new Stack<System.Action> ();
		}


		public Command Command
		{
			get
			{
				return this.command;
			}
		}

		public bool ContainsCommandHandlers
		{
			get
			{
				return this.handlers.Count > 0;
			}
		}


		public void Push(System.Action handler)
		{
			this.handlers.Push (handler);
		}

		public void Pop()
		{
			this.handlers.Pop ();
		}

		public void Remove(System.Action handler)
		{
			if (this.handlers.Contains (handler))
			{
				var temp = new Stack<System.Action> ();
				var item = this.handlers.Pop ();

				while (item != handler)
				{
					temp.Push (item);
					item = this.handlers.Pop ();
				}

				while (temp.Count > 0)
				{
					this.handlers.Push (temp.Pop ());
				}
			}
		}

		public void Execute()
		{
			if (this.handlers.Count > 0)
			{
				var handler = this.handlers.Peek ();
				handler ();
			}
		}

		#region IEquatable<CommandHandlerStack> Members

		public bool Equals(CommandHandlerStack other)
		{
			return this.command == other.command;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is CommandHandlerStack)
			{
				return this.Equals ((CommandHandlerStack) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.command.GetHashCode ();
		}

		private readonly Command command;
		private readonly Stack<System.Action> handlers;
	}
}
