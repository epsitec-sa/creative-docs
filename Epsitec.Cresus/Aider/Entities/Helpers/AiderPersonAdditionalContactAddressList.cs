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


		private readonly AiderPersonEntity person;
	}
}
