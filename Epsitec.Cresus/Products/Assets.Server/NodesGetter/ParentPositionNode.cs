//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public struct ParentPositionNode
	{
		public ParentPositionNode(Guid guid, Guid parent, int position, bool grouping)
		{
			this.Guid     = guid;
			this.Parent   = parent;
			this.Position = position;
			this.Grouping = grouping;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty;
			}
		}

		public static ParentPositionNode Empty = new ParentPositionNode (Guid.Empty, Guid.Empty, 0, false);

		public readonly Guid				Guid;
		public readonly Guid				Parent;
		public readonly int					Position;
		public readonly bool				Grouping;
	}
}
