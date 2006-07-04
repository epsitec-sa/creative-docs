//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// </summary>
	public class CommandCollection : Types.Collections.HostedDependencyObjectList<Command>
	{
		public CommandCollection()
			: base (null)
		{
		}
	}
}
