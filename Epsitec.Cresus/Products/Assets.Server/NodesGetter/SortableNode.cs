//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public struct SortableNode
	{
		public SortableNode(Guid guid, ComparableData primarySortValue, ComparableData secondarySortValue)
		{
			this.Guid                  = guid;
			this.PrimarySortValue   = primarySortValue;
			this.SecondarySortValue = secondarySortValue;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty;
			}
		}

		public static SortableNode Empty = new SortableNode (Guid.Empty, ComparableData.Empty, ComparableData.Empty);

		public readonly Guid				Guid;
		public readonly ComparableData		PrimarySortValue;
		public readonly ComparableData		SecondarySortValue;
	}
}
