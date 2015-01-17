//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public struct ParentNode
	{
		public ParentNode(Guid guid, Guid parent, ComparableData primaryOrderedValue, ComparableData secondaryOrderedValue)
		{
			this.Guid                  = guid;
			this.Parent                = parent;
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

		public static ParentNode Empty = new ParentNode (Guid.Empty, Guid.Empty, ComparableData.Empty, ComparableData.Empty);

		public readonly Guid				Guid;
		public readonly Guid				Parent;
		public readonly ComparableData		PrimaryOrderedValue;
		public readonly ComparableData		SecondaryOrderedValue;
	}
}
