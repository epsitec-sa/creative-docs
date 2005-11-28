//	Copyright � 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface ICommandDispatcher donne acc�s � une m�thode permettant
	/// l'ex�cution de commandes.
	/// </summary>
	public interface ICommandDispatcher
	{
		bool DispatchCommand(CommandDispatcher sender, CommandEventArgs e);
	}
}
