//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Layouts.Collections
{
	/// <summary>
	/// The <c>RowDefinitionCollection</c> class provides access to an ordered,
	/// strongly typed collection of <see cref="T:RowDefinition"/> objects.
	/// </summary>
	public class RowDefinitionCollection : Types.Collections.HostedDependencyObjectList<RowDefinition>
	{
		public RowDefinitionCollection(GridLayoutEngine grid) : base (grid)
		{
		}
	}
}
