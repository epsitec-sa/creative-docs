//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
