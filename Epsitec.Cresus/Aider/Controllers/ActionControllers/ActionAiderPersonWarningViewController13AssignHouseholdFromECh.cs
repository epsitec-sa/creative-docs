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
using Epsitec.Cresus.DataLayer.Expressions;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (13)]
	public sealed class ActionAiderPersonWarningViewController13AssignHouseholdFromECh : ActionAiderPersonWarningViewControllerPassive
	{
		public override bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public override FormattedText GetTitle()
		{
			return TextFormatter.FormatText ("Créer le ménage");
		}

		protected override void Execute()
		{
			var person			= this.Entity.Person;
			var personEchData	= person.eCH_Person;
			var EchReportedPerson = personEchData.ReportedPerson1;

			if (personEchData.ReportedPersons.Count () < 1)
			{
				EchReportedPerson = this.GetEchReportedPersonEntity (this.BusinessContext, personEchData);
				if (EchReportedPerson.IsNull ())
				{
					throw new BusinessRuleException ("Erreur");
				}
			}
			

			var existingAiderHousehold = this.GetAiderHousehold (this.BusinessContext, EchReportedPerson.Adult1);

			var isHead = false;

			if (personEchData.PersonId == EchReportedPerson.Adult1.PersonId ||
					personEchData.PersonId == EchReportedPerson.Adult2.PersonId)
			{
				isHead = true;
			}

			if (existingAiderHousehold.IsNotNull ())
			{
				AiderContactEntity.Create (this.BusinessContext, person, existingAiderHousehold, isHead);
			}
			else
			{
				var aiderHousehold = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
				aiderHousehold.HouseholdMrMrs = HouseholdMrMrs.Auto;

				var aiderAddressEntity = aiderHousehold.Address;
				var eChAddressEntity = EchReportedPerson.Address;


				var houseNumber = StringUtils.ParseNullableInt (SwissPostStreet.StripHouseNumber (eChAddressEntity.HouseNumber));
				var houseNumberComplement = SwissPostStreet.GetHouseNumberComplement (eChAddressEntity.HouseNumber);

				if (string.IsNullOrWhiteSpace (houseNumberComplement))
				{
					houseNumberComplement = null;
				}

				aiderAddressEntity.AddressLine1 = eChAddressEntity.AddressLine1;
				aiderAddressEntity.Street = eChAddressEntity.Street;
				aiderAddressEntity.HouseNumber = houseNumber;
				aiderAddressEntity.HouseNumberComplement = houseNumberComplement;
				aiderAddressEntity.Town = this.GetAiderTownEntity (this.BusinessContext, EchReportedPerson.Address);

				AiderContactEntity.Create (this.BusinessContext, person, aiderHousehold, isHead);
			}

			//Assign to parish
			ParishAssigner.AssignToParish (ParishAddressRepository.Current, this.BusinessContext, person);
		}

		private eCH_ReportedPersonEntity GetEchReportedPersonEntity(BusinessContext businessContext, eCH_PersonEntity eChPerson)
		{
			var reportedPersonExample = new eCH_ReportedPersonEntity ();

			reportedPersonExample.Adult1 = new eCH_PersonEntity ()
			{
				PersonId = eChPerson.PersonId
			};

			var result = businessContext.DataContext.GetByExample<eCH_ReportedPersonEntity> (reportedPersonExample).FirstOrDefault ();

			if (result.IsNull ())
			{
				reportedPersonExample = new eCH_ReportedPersonEntity ();

				reportedPersonExample.Adult2 = new eCH_PersonEntity ()
				{
					PersonId = eChPerson.PersonId
				};

				result = businessContext.DataContext.GetByExample<eCH_ReportedPersonEntity> (reportedPersonExample).FirstOrDefault ();

				if (result.IsNull ())
				{
					return null;
				}
				else
				{
					return result;
				}
			}
			else
			{
				return result = businessContext.DataContext.GetByExample<eCH_ReportedPersonEntity> (reportedPersonExample).FirstOrDefault ();
			}
		}

		private AiderTownEntity GetAiderTownEntity(BusinessContext businessContext, eCH_AddressEntity address)
		{
			var townExample = new AiderTownEntity ()
			{
				SwissZipCodeId = address.SwissZipCodeId
			};

			return businessContext.DataContext.GetByExample<AiderTownEntity> (townExample).FirstOrDefault ();
		}

		private AiderHouseholdEntity GetAiderHousehold(BusinessContext businessContext, eCH_PersonEntity refPerson)
		{
			if (refPerson.IsNull ())
			{
				return null;
			}

			var personExample = new AiderPersonEntity ();
			var contactExample = new AiderContactEntity ();
			var householdExample = new AiderHouseholdEntity ();
			personExample.eCH_Person = refPerson;
			contactExample.Person = personExample;
			contactExample.Household = householdExample;
			var request = new Request ()
			{
				RootEntity = contactExample,
				RequestedEntity = householdExample
			};

			return businessContext.DataContext.GetByRequest<AiderHouseholdEntity> (request).FirstOrDefault ();
		}
	}
}
