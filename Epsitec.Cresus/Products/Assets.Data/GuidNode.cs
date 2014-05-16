//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Data
{
	public struct GuidNode
	{
		public GuidNode(Guid guid)
		{
			this.Guid  = guid;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty;
			}
		}

		public static GuidNode Empty = new GuidNode (Guid.Empty);

		public readonly Guid				Guid;
	}
}
