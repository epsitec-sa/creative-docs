//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public struct ParentNode
	{
		public ParentNode(Guid guid, Guid parent, string orderValue)
		{
			this.Guid       = guid;
			this.Parent     = parent;
			this.OrderValue = orderValue;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty;
			}
		}

		public static ParentNode Empty = new ParentNode (Guid.Empty, Guid.Empty, null);

		public readonly Guid				Guid;
		public readonly Guid				Parent;
		public readonly string				OrderValue;
	}
}
