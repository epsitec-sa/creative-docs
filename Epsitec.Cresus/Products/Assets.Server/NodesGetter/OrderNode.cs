//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public struct OrderNode
	{
		public OrderNode(Guid guid, ComparableData primaryOrderedValue, ComparableData secondaryOrderedValue)
		{
			this.Guid                  = guid;
			this.PrimaryOrderedValue   = primaryOrderedValue;
			this.SecondaryOrderedValue = secondaryOrderedValue;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty;
			}
		}

		public static OrderNode Empty = new OrderNode (Guid.Empty, ComparableData.Empty, ComparableData.Empty);

		public readonly Guid				Guid;
		public readonly ComparableData		PrimaryOrderedValue;
		public readonly ComparableData		SecondaryOrderedValue;
	}
}
