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


namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>CommandEventArgs</c> class describes a command dispatched by the
    /// <see cref="CommandDispatcher"/>.
    /// </summary>
    public class CommandEventArgs : Support.CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="command">The command.</param>
        /// <param name="contextChain">The context chain.</param>
        /// <param name="context">The context.</param>
        /// <param name="state">The state.</param>
        /// <param name="commandShortcut">The command shortcut, if any.</param>
        /// <param name="commandMessage">The UI message which produced the command, if any.</param>
        internal CommandEventArgs(
            object source,
            Command command,
            CommandContextChain contextChain,
            CommandContext context,
            CommandState state,
            Shortcut commandShortcut,
            Message commandMessage
        )
        {
            this.source = source;
            this.command = command;
            this.commandShortcut = commandShortcut;
            this.commandMessage = commandMessage;
            this.chain = contextChain;
            this.context = context;
            this.state = state;
        }

        public object Source
        {
            get { return this.source; }
        }

        public Command Command
        {
            get { return this.command; }
        }

        public Shortcut CommandShortcut
        {
            get { return this.commandShortcut; }
        }

        public Message CommandMessage
        {
            get { return this.commandMessage; }
        }

        public bool Executed { get; set; }

        public bool Handled { get; set; }

        public CommandContext CommandContext
        {
            get { return this.context; }
        }

        public CommandContextChain CommandContextChain
        {
            get { return this.chain; }
        }

        public CommandState CommandState
        {
            get { return this.state; }
        }

        private readonly object source;
        private readonly Command command;
        private readonly Shortcut commandShortcut;
        private readonly Message commandMessage;
        private readonly CommandContextChain chain;
        private readonly CommandContext context;
        private readonly CommandState state;
    }

    /// <summary>
    /// Le delegate CommandEventHandler permet d'exécuter une commande envoyée
    /// par un CommandDispatcher et décrite par l'événement associé.
    /// </summary>
    public delegate void CommandEventHandler(CommandDispatcher sender, CommandEventArgs e);
}
