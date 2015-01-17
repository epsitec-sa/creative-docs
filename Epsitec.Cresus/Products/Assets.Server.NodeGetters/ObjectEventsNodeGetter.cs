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
		public ObjectEventsNodeGetter(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public void SetParams(DataObject dataObject, SortingInstructions sortingInstructions)
		{
			this.dataObject          = dataObject;
			this.sortingInstructions = sortingInstructions;

			if (this.dataObject == null)
			{
				this.dataEvents = null;
			}
			else
			{
				this.dataEvents = dataObject.Events;
			}
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
					return this.dataEvents.Count;
				}
			}
		}

		public SortableNode this[int index]
		{
			get
			{
				if (this.dataEvents == null || index < 0 || index >= this.dataEvents.Count)
				{
					return SortableNode.Empty;
				}
				else
				{
					var e = this.dataEvents[index];

					var pp = e.GetProperty (this.sortingInstructions.PrimaryField);
					var sp = e.GetProperty (this.sortingInstructions.SecondaryField);

					var primary   = ObjectProperties.GetComparableData (this.accessor, pp);
					var secondary = ObjectProperties.GetComparableData (this.accessor, sp);

					return new SortableNode (e.Guid, primary, secondary);
				}
			}
		}


		private readonly DataAccessor			accessor;

		private DataObject						dataObject;
		private List<DataEvent>					dataEvents;
		private SortingInstructions				sortingInstructions;
	}
}
