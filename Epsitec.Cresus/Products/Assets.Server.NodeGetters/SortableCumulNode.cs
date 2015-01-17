//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public struct SortableCumulNode
	{
		public SortableCumulNode(Guid guid, BaseType baseType, int level, decimal? ratio, NodeType type, int? groupIndex, Dictionary<ObjectField, decimal> cumuls, ComparableData primarySortValue, ComparableData secondarySortValue)
		{
			this.Guid               = guid;
			this.BaseType           = baseType;
			this.Level              = level;
			this.Ratio              = ratio;
			this.Type               = type;
			this.GroupIndex         = groupIndex;
			this.PrimarySortValue   = primarySortValue;
			this.SecondarySortValue = secondarySortValue;
			this.Cumuls             = cumuls;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty
					&& this.Level == -1
					&& this.Type == NodeType.None;
			}
		}

		public static SortableCumulNode Empty = new SortableCumulNode (Guid.Empty, BaseType.Assets, -1, null, NodeType.None, null, null, ComparableData.Empty, ComparableData.Empty);

		public readonly Guid				Guid;
		public readonly BaseType			BaseType;
		public readonly int					Level;
		public readonly decimal?			Ratio;
		public readonly NodeType			Type;
		public readonly int?				GroupIndex;
		public readonly ComparableData		PrimarySortValue;
		public readonly ComparableData		SecondarySortValue;
		public readonly Dictionary<ObjectField, decimal> Cumuls;
	}
}
