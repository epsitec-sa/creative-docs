//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public struct LevelNode
	{
		public LevelNode(Guid guid, BaseType baseType, int level, decimal? ratio, int? groupIndex)
		{
			this.Guid       = guid;
			this.BaseType   = baseType;
			this.Level      = level;
			this.Ratio      = ratio;
			this.GroupIndex = groupIndex;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty
					&& this.Level == -1;
			}
		}

		public static LevelNode Empty = new LevelNode (Guid.Empty, BaseType.Assets, -1, null, null);

		public readonly Guid				Guid;
		public readonly BaseType			BaseType;
		public readonly int					Level;
		public readonly decimal?			Ratio;
		public readonly int?				GroupIndex;
	}
}
