//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 06/02/2004

namespace Epsitec.Common.Support
{
	public interface ICommandDispatcherHost
	{
		Support.CommandDispatcher	CommandDispatcher	{ get; set; }
	}
}
