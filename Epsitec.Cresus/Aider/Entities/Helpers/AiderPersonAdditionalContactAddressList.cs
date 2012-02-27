//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities.Helpers
{
	/// <summary>
	/// The <c>AiderPersonAdditionalContactAddressList</c> class gives access to the additional
	/// address fields defined in <see cref="AiderPersonEntity"/> as if they belonged to a list.
	/// </summary>
	public class AiderPersonAdditionalContactAddressList : VirtualList<AiderPersonEntity, AiderAddressEntity>
	{
		public AiderPersonAdditionalContactAddressList(AiderPersonEntity person)
			: base (person)
		{
		}


		public override int						MaxCount
		{
			get
			{
				return 4;
			}
		}

		
		protected override IEnumerable<AiderAddressEntity> GetItems()
		{
			if (this.entity.AdditionalAddress1.IsNotNull ())
			{
				yield return this.entity.AdditionalAddress1;
			}

			if (this.entity.AdditionalAddress2.IsNotNull ())
			{
				yield return this.entity.AdditionalAddress2;
			}

			if (this.entity.AdditionalAddress3.IsNotNull ())
			{
				yield return this.entity.AdditionalAddress3;
			}

			if (this.entity.AdditionalAddress4.IsNotNull ())
			{
				yield return this.entity.AdditionalAddress4;
			}
		}

		protected override void ReplaceItems(IList<AiderAddressEntity> list)
		{
			int n = list.Count;

			System.Diagnostics.Debug.Assert (n <= this.MaxCount);

			//	Should we somehow suspend the events here in order to avoid sending
			//	notifications while the person is in a transient state ?

			base.SetField (this.entity, (x, y) => x.AdditionalAddress1 = y, () => list[0], n > 0);
			base.SetField (this.entity, (x, y) => x.AdditionalAddress2 = y, () => list[1], n > 1);
			base.SetField (this.entity, (x, y) => x.AdditionalAddress3 = y, () => list[0], n > 2);
			base.SetField (this.entity, (x, y) => x.AdditionalAddress4 = y, () => list[1], n > 3);
		}
	}
}
