//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities.Helpers
{
	public class AiderPersonHouseholdList : VirtualList<AiderPersonEntity, AiderHouseholdEntity>
	{
		public AiderPersonHouseholdList(AiderPersonEntity person)
			: base (person)
		{
		}


		public override int MaxCount
		{
			get
			{
				return 2;
			}
		}


		protected override IEnumerable<AiderHouseholdEntity> GetItems()
		{
			if (this.entity.Household1.IsNotNull ())
			{
				yield return this.entity.Household1;
			}

			if (this.entity.Household2.IsNotNull ())
			{
				yield return this.entity.Household2;
			}
		}

		protected override void ReplaceItems(IList<AiderHouseholdEntity> list)
		{
			int n = list.Count;

			System.Diagnostics.Debug.Assert (n <= this.MaxCount);

			//	Should we somehow suspend the events here in order to avoid sending
			//	notifications while the person is in a transient state ?

			base.SetField (this.entity, (x, y) => x.Household1 = y, () => list[0], n > 0);
			base.SetField (this.entity, (x, y) => x.Household2 = y, () => list[1], n > 1);
		}
	}
}
