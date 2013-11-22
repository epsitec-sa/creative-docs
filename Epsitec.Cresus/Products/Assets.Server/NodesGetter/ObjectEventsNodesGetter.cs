//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Accès en lecture aux événements triables d'un objet.
	/// événements d'un objet -> SortableNode
	/// </summary>
	public class ObjectEventsNodesGetter : AbstractNodesGetter<SortableNode>  // outputNodes
	{
		public DataObject						DataObject;
		public SortingInstructions				SortingInstructions;


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

		public override SortableNode this[int index]
		{
			get
			{
				var e = this.DataObject.GetEvent (index);

				if (e == null)
				{
					return SortableNode.Empty;
				}
				else
				{
					var pp = e.GetProperty (this.SortingInstructions.PrimaryField);
					var sp = e.GetProperty (this.SortingInstructions.SecondaryField);

					var primary   = ObjectCalculator.GetComparableData (pp);
					var secondary = ObjectCalculator.GetComparableData (sp);

					return new SortableNode (e.Guid, primary, secondary);
				}
			}
		}
	}
}
