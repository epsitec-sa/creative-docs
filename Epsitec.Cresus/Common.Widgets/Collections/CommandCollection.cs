//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
