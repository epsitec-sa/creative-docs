//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class UserFieldNodeGetter : INodeGetter<GuidNode>  // outputNodes
	{
		public UserFieldNodeGetter(DataAccessor accessor, BaseType baseType)
		{
			this.accessor = accessor;
			this.baseType = baseType;
		}


		public int Count
		{
			get
			{
				return this.accessor.Mandat.GetData (this.baseType).Count;
			}
		}

		public GuidNode this[int index]
		{
			get
			{
				var obj = this.accessor.Mandat.GetData (this.baseType)[index];

				if (obj == null)
				{
					return GuidNode.Empty;
				}
				else
				{
					return new GuidNode (obj.Guid);
				}
			}
		}


		private readonly DataAccessor			accessor;
		private readonly BaseType				baseType;
	}
}
