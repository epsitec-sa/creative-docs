//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface ICommandDispatcherHost permet de lier un objet à un ou
	/// plusieurs CommandDispatcher(s) spécifique(s).
	/// </summary>
	public interface ICommandDispatcherHost
	{
		CommandDispatcher[] CommandDispatchers { get; }
	}
}
