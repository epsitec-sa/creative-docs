//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Accès en lecture aux événements triables d'un objet.
	/// événements d'un objet -> SortableNode
	/// </summary>
	public class ObjectEventsNodeGetter : INodeGetter<SortableNode>  // outputNodes
	{
		public void SetParams(DataObject dataObject, SortingInstructions sortingInstructions)
		{
			this.dataObject          = dataObject;
			this.sortingInstructions = sortingInstructions;
		}


		public int Count
		{
			get
			{
				if (this.dataObject == null)
				{
					return 0;
				}
				else
				{
					return this.dataObject.EventsCount;
				}
			}
		}

		public SortableNode this[int index]
		{
			get
			{
				var e = this.dataObject.GetEvent (index);

				if (e == null)
				{
					return SortableNode.Empty;
				}
				else
				{
					var pp = e.GetProperty (this.sortingInstructions.PrimaryField);
					var sp = e.GetProperty (this.sortingInstructions.SecondaryField);

					var primary   = ObjectProperties.GetComparableData (pp);
					var secondary = ObjectProperties.GetComparableData (sp);

					return new SortableNode (e.Guid, primary, secondary);
				}
			}
		}


		private DataObject						dataObject;
		private SortingInstructions				sortingInstructions;
	}
}
