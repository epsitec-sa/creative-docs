//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public struct LevelNode
	{
		public LevelNode(Guid guid, int level = 0)
		{
			this.Guid  = guid;
			this.Level = level;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty
					&& this.Level == -1;
			}
		}

		public static LevelNode Empty = new LevelNode (Guid.Empty, -1);

		public readonly Guid				Guid;
		public readonly int					Level;
	}
}
