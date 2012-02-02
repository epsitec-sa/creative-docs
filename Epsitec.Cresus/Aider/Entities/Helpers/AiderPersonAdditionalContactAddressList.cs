//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities.Helpers
{
	class AiderPersonAdditionalContactAddressList : ObservableList<AiderAddressEntity>
	{
		public AiderPersonAdditionalContactAddressList(AiderPersonEntity person)
		{
			this.person = person;
			this.AddRange (this.GetAddresses ());
		}

		private IEnumerable<AiderAddressEntity> GetAddresses()
		{
			if (this.person.AdditionalAddress1.IsNotNull ())
			{
				yield return this.person.AdditionalAddress1;
			}

			if (this.person.AdditionalAddress2.IsNotNull ())
			{
				yield return this.person.AdditionalAddress2;
			}

			if (this.person.AdditionalAddress3.IsNotNull ())
			{
				yield return this.person.AdditionalAddress3;
			}

			if (this.person.AdditionalAddress4.IsNotNull ())
			{
				yield return this.person.AdditionalAddress4;
			}
		}

		public override void Add(AiderAddressEntity item)
		{
			if (this.Contains (item))
			{
				throw new System.InvalidOperationException ("Duplicate address");
			}

			if (this.person.AdditionalAddress1.IsNull ())
			{
				this.person.AdditionalAddress1 = item;
			}
			else if (this.person.AdditionalAddress2.IsNull ())
			{
				this.person.AdditionalAddress2 = item;
			}
			else if (this.person.AdditionalAddress3.IsNull ())
			{
				this.person.AdditionalAddress3 = item;
			}
			else if (this.person.AdditionalAddress4.IsNull ())
			{
				this.person.AdditionalAddress4 = item;
			}
			else
			{
				return;
			}
			
			base.Add (item);
		}

		public override bool Remove(AiderAddressEntity item)
		{
			if (this.person.AdditionalAddress1 == item)
			{
				this.person.AdditionalAddress1 = this.person.AdditionalAddress2;
				this.person.AdditionalAddress2 = this.person.AdditionalAddress3;
				this.person.AdditionalAddress3 = this.person.AdditionalAddress4;
				this.person.AdditionalAddress4 = null;
			}
			else if (this.person.AdditionalAddress2 == item)
			{
				this.person.AdditionalAddress2 = this.person.AdditionalAddress3;
				this.person.AdditionalAddress3 = this.person.AdditionalAddress4;
				this.person.AdditionalAddress4 = null;
			}
			else if (this.person.AdditionalAddress3 == item)
			{
				this.person.AdditionalAddress3 = this.person.AdditionalAddress4;
				this.person.AdditionalAddress4 = null;
			}
			else if (this.person.AdditionalAddress4 == item)
			{
				this.person.AdditionalAddress4 = null;
			}
			else
			{
				return false;
			}

			this.ReplaceWithRange (this.GetAddresses ());

			return true;
		}

		private readonly AiderPersonEntity person;
	}
}
