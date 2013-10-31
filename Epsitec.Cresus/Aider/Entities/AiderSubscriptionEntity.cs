//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities
{
	public partial class AiderSubscriptionEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
			(
				"Abonnement N°", this.Id, "\n",
				this.Count, " exemplaire(s)", "\n",
				"Cahier de la ", this.RegionalEdition.Name
			);
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}


		public FormattedText GetAddressLabelText()
		{
			switch (this.SubscriptionType)
			{
				case SubscriptionType.Household:
					return this.Household.GetAddressLabelText ();

				case SubscriptionType.LegalPerson:
					return this.LegalPersonContact.GetAddressLabelText ();

				default:
					throw new NotImplementedException ();
			}
		}


		public void RefreshCache()
		{
			this.DisplayName = this.GetDisplayName ();
			this.DisplayZipCode = this.GetDisplayZipCode ();
			this.DisplayAddress = this.GetDisplayAddress ();
		}


		private string GetDisplayName()
		{
			switch (this.SubscriptionType)
			{
				case SubscriptionType.Household:
					return this.Household.GetDisplayName ();

				case SubscriptionType.LegalPerson:
					return this.LegalPersonContact.GetDisplayName ();

				default:
					throw new NotImplementedException ();
			}
		}


		private string GetDisplayZipCode()
		{
			return this.GetAddress ().GetDisplayZipCode ().ToSimpleText ();
		}


		private string GetDisplayAddress()
		{
			return this.GetAddress ().GetDisplayAddress ().ToSimpleText ();
		}


		public string GetEditionId()
		{
			// Editions ids should be N1 to NB, which by chance is the hexadecimal representation of
			// the region id.

			var regionId = this.RegionalEdition.GetRegionId ();

			return "N" + regionId.ToString ("X1");
		}


		public AiderAddressEntity GetAddress()
		{
			switch (this.SubscriptionType)
			{
				case SubscriptionType.Household:
					return this.Household.Address;

				case SubscriptionType.LegalPerson:
					return this.LegalPersonContact.Address;

				default:
					throw new NotImplementedException ();
			}
		}


		partial void GetFullAddressTextSingleLine(ref string value)
		{
			this.GetFullAddressTextMultiLine (ref value);

			value = value.Replace ("\n", ", ");
		}


		partial void SetFullAddressTextSingleLine(string value)
		{
			throw new NotImplementedException ("Do not use this method");
		}


		partial void GetFullAddressTextMultiLine(ref string value)
		{
			value = this.GetAddressLabelText ().ToSimpleText ();
		}


		partial void SetFullAddressTextMultiLine(string value)
		{
			throw new NotImplementedException ("Do not use this method");
		}


		public static AiderSubscriptionEntity Create(BusinessContext context, AiderHouseholdEntity household)
		{
			if (household.IsNull ())
			{
				return null;
			}

			var edition = AiderSubscriptionEntity.GetEdition (context, household.Address);
			return AiderSubscriptionEntity.Create (context, household, edition, 1);
		}
		
		public static AiderGroupEntity GetEdition(BusinessContext context, AiderAddressEntity address)
		{
			var parishRepository = Epsitec.Aider.Data.Common.ParishAddressRepository.Current;
			var parishName = Epsitec.Aider.Data.Common.ParishAssigner.FindParishName (parishRepository, address);

			// If we can't find the region code, we default to the region 4, which is the one of
			// Lausanne.

			var regionCode = parishName != null
				? parishRepository.GetDetails (parishName).RegionCode
				: 4;

			return Epsitec.Aider.Data.Common.ParishAssigner.FindRegionGroup (context, regionCode);
		}


		public static AiderSubscriptionEntity Create
		(
			BusinessContext businessContext,
			AiderHouseholdEntity household,
			AiderGroupEntity regionalEdition,
			int count
		)
		{
			var subscription = AiderSubscriptionEntity.Create
			(
				businessContext, regionalEdition, count
			);

			subscription.SubscriptionType = SubscriptionType.Household;
			subscription.Household = household;
			subscription.ParishGroupPathCache = household.ParishGroupPathCache;
			return subscription;
		}


		public static AiderSubscriptionEntity Create
		(
			BusinessContext businessContext,
			AiderContactEntity legalPersonContact,
			AiderGroupEntity regionalEdition,
			int count
		)
		{
			var subscription = AiderSubscriptionEntity.Create
			(
				businessContext, regionalEdition, count
			);

			subscription.SubscriptionType = SubscriptionType.LegalPerson;
			subscription.LegalPersonContact = legalPersonContact;
			subscription.ParishGroupPathCache = legalPersonContact.ParishGroupPathCache;

			return subscription;
		}


		private static AiderSubscriptionEntity Create
		(
			BusinessContext businessContext,
			AiderGroupEntity regionalEdition,
			int count
		)
		{
			var subscription = businessContext.CreateAndRegisterEntity<AiderSubscriptionEntity> ();

			subscription.Count = count;
			subscription.RegionalEdition = regionalEdition;
			subscription.ParishGroupPathCache = regionalEdition.Path;
			return subscription;
		}


		public static void Delete
		(
			BusinessContext businessContext,
			AiderSubscriptionEntity subscription
		)
		{
			businessContext.DeleteEntity (subscription);
		}


		public static AiderSubscriptionEntity FindSubscription(BusinessContext businessContext, AiderHouseholdEntity household)
		{
			var example = new AiderSubscriptionEntity ()
			{
				SubscriptionType = SubscriptionType.Household,
				Household = household,
			};

			return AiderSubscriptionEntity.FindSubscription (businessContext, example, household);
		}


		public static AiderSubscriptionEntity FindSubscription
		(
			BusinessContext businessContext,
			AiderContactEntity legalPersonContact
		)
		{
			var example = new AiderSubscriptionEntity ()
			{
				SubscriptionType = SubscriptionType.LegalPerson,
				LegalPersonContact = legalPersonContact,
			};

			return AiderSubscriptionEntity.FindSubscription
			(
				businessContext, example, legalPersonContact
			);
		}


		private static AiderSubscriptionEntity FindSubscription
		(
			BusinessContext businessContext,
			AiderSubscriptionEntity example,
			AbstractEntity entity
		)
		{
			var dataContext = businessContext.DataContext;

			if (!dataContext.IsPersistent (entity))
			{
				return null;
			}

			var request = new Request ()
			{
				RootEntity = example,
			};

			var result = dataContext.GetByRequest<AiderSubscriptionEntity> (request);

			return result.FirstOrDefault ();
		}


		public static IEnumerable<AiderSubscriptionEntity> FindSubscriptions(BusinessContext businessContext, AiderPersonEntity person)
		{
			foreach (var household in person.Households)
			{
				var subscription = AiderSubscriptionEntity.FindSubscription (businessContext, household);

				if (subscription.IsNotNull ())
				{
					yield return subscription;
				}
			}
		}

		public static IList<AiderSubscriptionEntity> FindSubscriptions(BusinessContext businessContext, AiderLegalPersonEntity legalPerson)
		{
			var dataContext = businessContext.DataContext;

			if (!dataContext.IsPersistent (legalPerson))
			{
				return new List<AiderSubscriptionEntity> ();
			}

			var example = new AiderSubscriptionEntity ()
			{
				SubscriptionType   = SubscriptionType.LegalPerson,
				LegalPersonContact = new AiderContactEntity ()
				{
					ContactType = ContactType.Legal,
					LegalPerson = legalPerson
				},
			};

			return dataContext.GetByExample (example);
		}


		public static void CheckSubscriptionDoesNotExist
		(
			BusinessContext businessContext,
			AiderHouseholdEntity receiver
		)
		{
			var result = AiderSubscriptionEntity.FindSubscription (businessContext, receiver);

			AiderSubscriptionEntity.CheckSubscriptionDoesNotExist (result);
		}


		public static void CheckSubscriptionDoesNotExist
		(
			BusinessContext businessContext,
			AiderContactEntity receiver
		)
		{
			var result = AiderSubscriptionEntity.FindSubscription (businessContext, receiver);

			AiderSubscriptionEntity.CheckSubscriptionDoesNotExist (result);
		}


		private static void CheckSubscriptionDoesNotExist(AiderSubscriptionEntity result)
		{
			if (result != null)
			{
				var format = "Un abonnement existe déjà pour ce destinataire: n°{0}.";
				var message = string.Format (format, result.Id);

				throw new BusinessRuleException (message);
			}
		}
	}
}
