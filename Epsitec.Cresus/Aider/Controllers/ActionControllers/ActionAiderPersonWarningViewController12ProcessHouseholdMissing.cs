//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Data.Platform;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Data.Common;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (12)]
	public sealed class ActionAiderPersonWarningViewController12ProcessHouseholdMissing : ActionAiderPersonWarningViewControllerPassive
	{
		public override bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		protected override void Execute()
		{
			this.ClearWarningAndRefreshCaches ();
		}
#if false
		if (createNewHousehold)
			{
				if (this.Entity.Person.MainContact.IsNotNull ())
				{
					var oldHousehold = this.Entity.Person.MainContact.Household;
					var contacts = this.Entity.Person.Contacts;
					var contact = contacts.FirstOrDefault (x => x.Household == oldHousehold);
					if (contact.IsNotNull ())
					{
						AiderContactEntity.Delete (this.BusinessContext, contact);
					}
				}

				var newHousehold = this.GetNewHousehold();
				var aiderHousehold = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity>();
				aiderHousehold.HouseholdMrMrs = HouseholdMrMrs.Auto;

				var aiderAddressEntity = aiderHousehold.Address;
				var eChAddressEntity = newHousehold.Address;


				var houseNumber = StringUtils.ParseNullableInt(SwissPostStreet.StripHouseNumber(eChAddressEntity.HouseNumber));
				var houseNumberComplement = SwissPostStreet.GetHouseNumberComplement(eChAddressEntity.HouseNumber);

				if (string.IsNullOrWhiteSpace(houseNumberComplement))
				{
					houseNumberComplement = null;
				}

				aiderAddressEntity.AddressLine1 = eChAddressEntity.AddressLine1;
				aiderAddressEntity.Street = eChAddressEntity.Street;
				aiderAddressEntity.HouseNumber = houseNumber;
				aiderAddressEntity.HouseNumberComplement = houseNumberComplement;
				aiderAddressEntity.Town = this.GetAiderTownEntity (newHousehold.Address);

				//Link household to ECh Entity
				if (newHousehold.Adult1.IsNotNull())
				{               
					EChDataImporter.SetupHousehold (this.BusinessContext, this.Entity.Person, aiderHousehold, newHousehold, isHead1: true);
				}

				if (newHousehold.Adult2.IsNotNull())
				{
					var aiderPerson = this.GetAiderPersonEntity (this.BusinessContext, newHousehold.Adult2);
					EChDataImporter.SetupHousehold (this.BusinessContext, aiderPerson, aiderHousehold, newHousehold, isHead2: true);
				}

				foreach (var child in newHousehold.Children)
				{
					var aiderPerson = this.GetAiderPersonEntity (this.BusinessContext, child);
					EChDataImporter.SetupHousehold (this.BusinessContext, aiderPerson, aiderHousehold, newHousehold);
				}
				
			}
			if (subscribe)
			{
				var household = this.Entity.Person.Households.FirstOrDefault ();
				AiderSubscriptionEntity.Create (this.BusinessContext, household);
			}
#endif
	}
}
