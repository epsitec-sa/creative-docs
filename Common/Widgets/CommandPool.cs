/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


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
                CommandPool.commandList.Add(command);
            }
        }

        internal static void RegisterCommand(string commandId, Command command)
        {
            lock (CommandPool.exclusion)
            {
                System.Diagnostics.Debug.Assert(CommandPool.commandList.Contains(command));

                Weak<Command> weak;

                if (CommandPool.commandDict.TryGetValue(commandId, out weak))
                {
                    if (weak.IsAlive)
                    {
                        throw new System.ArgumentException(
                            string.Format("Command {0} already registered", commandId)
                        );
                    }
                }

                CommandPool.commandDict[commandId] = new Weak<Command>(command);
            }
        }

        public static IEnumerable<Command> GetAllCommands()
        {
            List<Command> copy = new List<Command>();

            lock (CommandPool.exclusion)
            {
                copy.AddRange(CommandPool.commandList);
            }

            return copy;
        }

        public static Command FindCommand(System.Predicate<Command> predicate)
        {
            lock (CommandPool.exclusion)
            {
                foreach (var command in CommandPool.commandList)
                {
                    if (predicate(command))
                    {
                        return command;
                    }
                }
            }

            return null;
        }

        public static Command FindCommand(string id, System.Func<Command> commandResolver)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            lock (CommandPool.exclusion)
            {
                Weak<Command> weak;
                Command command = null;

                if (CommandPool.commandDict.TryGetValue(id, out weak))
                {
                    command = weak.Target;
                }

                return command ?? commandResolver();
            }
        }

        public static IEnumerable<Command> FindCommandsWithPrefix(string prefix)
        {
            List<Command> copy = new List<Command>();

            lock (CommandPool.exclusion)
            {
                copy.AddRange(
                    CommandPool.commandList.Where(x => x.Caption.Name.StartsWith(prefix))
                );
            }

            return copy;
        }

        static CommandPool()
        {
            CommandPool.exclusion = new object();
            CommandPool.commandList = new WeakList<Command>();
            CommandPool.commandDict = new Dictionary<string, Weak<Command>>();
        }

        private static readonly object exclusion;
        private static readonly WeakList<Command> commandList;
        private static readonly Dictionary<string, Weak<Command>> commandDict;
    }
}
