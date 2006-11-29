//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.UI.Collections
{
	public class ItemTableColumnCollection : HostedDependencyObjectList<ItemTableColumn>
	{
		internal ItemTableColumnCollection(ItemTable host)
			: base (host)
		{
		}
	}
}
