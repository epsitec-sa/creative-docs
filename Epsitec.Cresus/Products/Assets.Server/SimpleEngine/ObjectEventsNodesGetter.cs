//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Accès aux événements d'un objet.
	/// </summary>
	public class ObjectEventsNodesGetter : AbstractNodesGetter
	{
		public DataObject DataObject;


		public override int NodesCount
		{
			get
			{
				if (this.DataObject == null)
				{
					return 0;
				}
				else
				{
					return this.DataObject.EventsCount;
				}
			}
		}

		public override Node GetNode(int index)
		{
			var e = this.DataObject.GetEvent (index);

			if (e == null)
			{
				return Node.Empty;
			}
			else
			{
				return new Node (e.Guid);
			}
		}
	}
}
