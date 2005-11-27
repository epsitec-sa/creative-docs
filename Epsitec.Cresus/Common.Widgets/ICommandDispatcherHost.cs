//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface ICommandDispatcherHost permet de lier un objet à un
	/// CommandDispatcher spécifique.
	/// </summary>
	public interface ICommandDispatcherHost
	{
		CommandDispatcher	CommandDispatcher	{ get; }
	}
}
