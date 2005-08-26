//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public interface ICommandDispatcherHost
	{
		Support.CommandDispatcher	CommandDispatcher	{ get; set; }
	}
}
