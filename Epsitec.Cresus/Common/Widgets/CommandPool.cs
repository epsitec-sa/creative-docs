//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandPool</c> class maintains a global list of all known commands. This class
	/// is thread-safe.
	/// </summary>
	public static class CommandPool
	{
		internal static void RegisterCommand(Command command)
		{
			lock (CommandPool.exclusion)
			{
				CommandPool.commandList.Add (command);
			}
		}

		internal static void RegisterCommand(string commandId, Command command)
		{
			lock (CommandPool.exclusion)
			{
				System.Diagnostics.Debug.Assert (CommandPool.commandList.Contains (command));

				if (CommandPool.commandDict.ContainsKey (commandId))
				{
					throw new System.ArgumentException (string.Format ("Command {0} already registered", commandId));
				}

				CommandPool.commandDict[commandId] = new Weak<Command> (command);
			}
		}

		public static IEnumerable<Command> GetAllCommands()
		{
			List<Command> copy = new List<Command> ();

			lock (CommandPool.exclusion)
			{
				copy.AddRange (CommandPool.commandList);
			}

			return copy;
		}

		public static Command FindCommand(string id, System.Func<Command> commandResolver)
		{
			if (string.IsNullOrEmpty (id))
			{
				return null;
			}

			lock (CommandPool.exclusion)
			{
				Weak<Command> weak;
				Command command = null;

				if (CommandPool.commandDict.TryGetValue (id, out weak))
				{
					command = weak.Target;
				}

				return command ?? commandResolver ();
			}
		}

		public static IEnumerable<Command> FindCommandsWithPrefix(string prefix)
		{
			List<Command> copy = new List<Command> ();

			lock (CommandPool.exclusion)
			{
				copy.AddRange (CommandPool.commandList.Where (x => x.Caption.Name.StartsWith (prefix)));
			}

			return copy;
		}

		static CommandPool()
		{
			CommandPool.exclusion   = new object ();
			CommandPool.commandList = new WeakList<Command> ();
			CommandPool.commandDict = new Dictionary<string, Weak<Command>> ();
		}

		private static readonly object exclusion;
		private static readonly WeakList<Command>	commandList;
		private static readonly Dictionary<string, Weak<Command>> commandDict;
	}
}
