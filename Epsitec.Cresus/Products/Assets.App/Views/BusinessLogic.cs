//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class BusinessLogic
	{
		public BusinessLogic(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public void GénèreAmortissementsAuto()
		{
			int count = this.accessor.ObjectsCount;
			for (int i=0; i<count; i++)
			{
				var guid = this.accessor.GetObjectGuid (i);
				this.GénèreAmortissementsAuto (guid);
			}
		}

		private void GénèreAmortissementsAuto(Guid objectGuid)
		{
			this.accessor.RemoveAmortissementsAuto (objectGuid);

			this.CreateEvent (objectGuid, new System.DateTime (2013, 3, 1));
			this.CreateEvent (objectGuid, new System.DateTime (2013, 6, 1));
			this.CreateEvent (objectGuid, new System.DateTime (2013, 9, 1));
		}

		private void CreateEvent(Guid objectGuid, System.DateTime date)
		{
			var timestamp = this.accessor.CreateObjectEvent (objectGuid, date, EventType.AmortissementAuto);

			if (timestamp.HasValue)
			{
				var v = new ComputedAmount(123.0m);
				var p = new DataComputedAmountProperty((int) ObjectField.Valeur1, v);

				this.accessor.AddObjectEventProperty(objectGuid, timestamp.Value, p);
			}
		}


		private readonly DataAccessor accessor;
	}
}
