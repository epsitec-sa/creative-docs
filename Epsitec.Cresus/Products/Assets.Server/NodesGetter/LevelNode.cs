//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public struct LevelNode
	{
		public LevelNode(Guid guid, BaseType baseType, int level)
		{
			this.Guid     = guid;
			this.BaseType = baseType;
			this.Level    = level;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty
					&& this.Level == -1;
			}
		}

		public static LevelNode Empty = new LevelNode (Guid.Empty, BaseType.Objects, -1);

		public readonly Guid				Guid;
		public readonly BaseType			BaseType;
		public readonly int					Level;
	}
}
