//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		public void Add(string fieldId)
		{
			this.Add (new ItemTableColumn (fieldId));
		}

		public void Add(string fieldId, double width)
		{
			this.Add (new ItemTableColumn (fieldId, width));
		}

		public void Add(string fieldId, Epsitec.Common.Widgets.Layouts.GridLength width)
		{
			this.Add (new ItemTableColumn (fieldId, width));
		}
	}
}
