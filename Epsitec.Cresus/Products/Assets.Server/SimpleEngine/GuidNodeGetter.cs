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

			this.Update ();
		}

		public void Update()
		{
			this.dataObjects = this.mandat.GetData (this.baseType).ToArray ();
		}

		public int							Count
		{
			get
			{
				return this.dataObjects.Length;
			}
		}

		public GuidNode						this[int index]
		{
			get
			{
				if (index < 0 || index >= this.dataObjects.Length)
				{
					return GuidNode.Empty;
				}
				else
				{
					var obj = this.dataObjects[index];
					return new GuidNode (obj.Guid);
				}
			}
		}

		private readonly DataMandat			mandat;
		private readonly BaseType			baseType;
		private DataObject[]				dataObjects;
	}
}
