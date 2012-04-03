//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

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
			return this.entity.GetHouseholds ();
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
