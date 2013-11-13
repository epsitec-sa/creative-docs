//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Accès en lecture aux événements d'un objet.
	/// événements d'un objet -> GuidNode
	/// </summary>
	public class ObjectEventsNodesGetter : AbstractNodesGetter<GuidNode>  // outputNodes
	{
		public DataObject DataObject;


		public override int Count
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

		public override GuidNode this[int index]
		{
			get
			{
				var e = this.DataObject.GetEvent (index);

				if (e == null)
				{
					return GuidNode.Empty;
				}
				else
				{
					return new GuidNode (e.Guid);
				}
			}
		}
	}
}
