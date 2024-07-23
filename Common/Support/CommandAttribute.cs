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


namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>CommandAttribute</c> class defines a <c>[Command]</c> attribute,
    /// which is used by <see cref="CommandDispatcher"/> to locate the methods
    /// implementing specific commands.
    /// </summary>

    [System.Serializable]
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        public CommandAttribute() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        public CommandAttribute(string commandName)
        {
            this.commandName = commandName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
        /// </summary>
        /// <param name="commandId">The command DRUID (encoded as a raw <c>long</c> value).</param>
        public CommandAttribute(long commandId)
        {
            this.commandId = commandId;
        }

        /// <summary>
        /// Gets or sets the name of the command.
        /// </summary>
        /// <value>The name of the command.</value>
        public string CommandName
        {
            get { return this.commandName; }
            set { this.commandName = value; }
        }

        public Druid CommandId
        {
            get { return this.commandId; }
        }

        private string commandName;
        private long commandId;
    }
}
