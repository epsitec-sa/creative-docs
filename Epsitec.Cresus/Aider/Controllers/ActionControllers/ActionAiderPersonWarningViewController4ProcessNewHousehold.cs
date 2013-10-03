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
	[ControllerSubType (4)]
	public sealed class ActionAiderPersonWarningViewController4ProcessNewHousehold : ActionAiderPersonWarningViewControllerInteractive
	{
		public override bool IsEnabled
		{
			get
			{
				//	TODO: refactor and validate this implementation
				return false;
			}
		}
		
		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool,bool>(this.Execute);
		}

		private void Execute(bool createNewHousehold,bool subscribe)
		{
			this.Entity.Person.RemoveWarningInternal(this.Entity);
			this.BusinessContext.DeleteEntity(this.Entity);

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
		}

		private AiderTownEntity GetAiderTownEntity(eCH_AddressEntity address)
		{
			var townExample = new AiderTownEntity()
			{
				SwissZipCodeId = address.SwissZipCodeId
			};

			return this.BusinessContext.DataContext.GetByExample<AiderTownEntity>(townExample).FirstOrDefault();
		}

		private eCH_ReportedPersonEntity GetNewHousehold()
		{
			var echHouseholdExample = new eCH_ReportedPersonEntity()
			{
				Adult1 = this.Entity.Person.eCH_Person
			};
			return this.BusinessContext.DataContext.GetByExample<eCH_ReportedPersonEntity>(echHouseholdExample).FirstOrDefault();
		}

		private AiderPersonEntity GetAiderPersonEntity(BusinessContext businessContext, eCH_PersonEntity person)
		{
			if (person == null)
			{
				return null;
			}

			var personExample = new AiderPersonEntity ();

			personExample.eCH_Person = new eCH_PersonEntity ()
			{
				PersonId = person.PersonId
			};

			return businessContext.DataContext.GetByExample<AiderPersonEntity> (personExample).FirstOrDefault ();
		}

		protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
		{
			var analyse = TextFormatter.FormatText ("Résultat de l'analyse:\n");
			if (this.Entity.Person.MainContact.IsNotNull ())
			{
				var householdAddress = this.Entity.Person.MainContact.Household.Address;
				var newHousehold = this.GetNewHousehold ();
				
				if (newHousehold.Address.StreetUserFriendly.Equals (householdAddress.StreetUserFriendly))
				{
					analyse = analyse.AppendLine (TextFormatter.FormatText ("Ménage identique que le précédent!\n", newHousehold.Address.GetCompactSummary ()));
					form
					.Title (this.GetTitle ())
					.Text (analyse)
					.Field<bool> ()
						.Title ("Créer le nouveau ménage")
						.InitialValue (false)
					.End ()
					.Field<bool> ()
						.Title ("Souscrire à un abonnement BN")
						.InitialValue (false)
					.End ();
				}
				else
				{
					analyse = analyse.AppendLine (TextFormatter.FormatText ("Nouveau ménage:\n", newHousehold.Address.GetCompactSummary ()));
					form
						.Title (this.GetTitle ())
						.Text (analyse)
						.Field<bool> ()
							.Title ("Créer le nouveau ménage")
							.InitialValue (true)
						.End ()
						.Field<bool> ()
							.Title ("Souscrire à un abonnement BN")
							.InitialValue (true)
						.End ()
					.End ();
				}
			}
			else
			{
				analyse = analyse.AppendLine (TextFormatter.FormatText ("Traitement de qualité de données"));
				form
					.Title (this.GetTitle ())
					.Text (analyse)
					.Field<bool> ()
						.Title ("Créer le nouveau ménage")
						.InitialValue (true)
					.End ()
					.Field<bool> ()
						.Title ("Souscrire à un abonnement BN")
						.InitialValue (true)
					.End ()
				.End ();
			}
		}
	}
}
