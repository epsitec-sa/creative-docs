//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandHandlerPair</c> structure defines a command/handler
	/// pair which can be used to register/unregister collections of items
	/// in the <see cref="CommandDispatcher"/> class.
	/// </summary>
	public struct CommandHandlerPair : System.IEquatable<CommandHandlerPair>
	{
		public CommandHandlerPair(Command command, CommandEventHandler handler)
		{
			this.command = command;
			this.handler = handler;
		}

		public Command							Command
		{
			get
			{
				return this.command;
			}
		}

		public CommandEventHandler				Handler
		{
			get
			{
				return this.handler;
			}
		}

		#region IEquatable<CommandHandlerPair> Members

		public bool Equals(CommandHandlerPair other)
		{
			return this.command == other.command
				&& this.handler == other.handler;
		}

		#endregion

		public override bool Equals(object obj)
		{
			if (obj is CommandHandlerPair)
			{
				return this.Equals ((CommandHandlerPair) obj);
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
		private readonly CommandEventHandler handler;
	}
}
