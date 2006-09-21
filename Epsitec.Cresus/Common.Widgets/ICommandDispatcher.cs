//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
