//	Copyright © 2005-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// The <c>CommandCollection</c> class implements an observable collection of
	/// <see cref="Command"/> objects.
	/// </summary>
	public class CommandCollection : Types.Collections.HostedDependencyObjectList<Command>
	{
		public CommandCollection()
			: base (null)
		{
		}
		
		public CommandCollection(IListHost<Command> host)
			: base (host)
		{
		}
	}
}
