//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class GuidNodeGetter : INodeGetter<GuidNode>
	{
		public GuidNodeGetter(DataMandat mandat, BaseType baseType)
		{
			this.mandat   = mandat;
			this.baseType = baseType;
		}

		public int							Count
		{
			get
			{
				return this.Data.Count;
			}
		}

		public GuidNode						this[int index]
		{
			get
			{
				var obj = this.Data[index];
				return new GuidNode (obj.Guid);
			}
		}

		private GuidList<DataObject>		Data
		{
			get
			{
				return this.mandat.GetData (this.baseType);
			}
		}

		private readonly DataMandat			mandat;
		private readonly BaseType			baseType;
	}
}
