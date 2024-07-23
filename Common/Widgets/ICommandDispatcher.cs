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
    /// The <c>ICommandDispatcher</c> interface is used to dispatch (i.e. execute) commands.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Epsitec.Common.Widgets.CommandEventArgs"/> instance containing the event data.</param>
        /// <returns><c>true</c> if the command was successfully executed; otherwise, <c>false</c>.</returns>
        bool ExecuteCommand(CommandDispatcher sender, CommandEventArgs e);
    }
}
