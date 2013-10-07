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
	public partial class AiderSubscriptionRefusalEntity
	{
		public void RefreshCache()
		{
			this.DisplayName = this.GetDisplayName ();
			this.DisplayZipCode = this.GetDisplayZipCode ();
			this.DisplayAddress = this.GetDisplayAddress ();
		}


		private string GetDisplayName()
		{
			switch (this.RefusalType)
			{
				case SubscriptionType.Household:
					return this.Household.GetDisplayName ();

				case SubscriptionType.LegalPerson:
					return this.LegalPersonContact.GetDisplayName ();

				default:
					throw new NotImplementedException ();
			}
		}


		public FormattedText GetAddressLabelText()
		{
			switch (this.RefusalType)
			{
				case SubscriptionType.Household:
					return this.Household.GetAddressLabelText ();

				case SubscriptionType.LegalPerson:
					return this.LegalPersonContact.GetAddressLabelText ();

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


		private string GetDisplayZipCode()
		{
			return this.GetAddress ().GetDisplayZipCode ().ToSimpleText ();
		}


		private string GetDisplayAddress()
		{
			return this.GetAddress ().GetDisplayAddress ().ToSimpleText ();
		}


		public AiderAddressEntity GetAddress()
		{
			switch (this.RefusalType)
			{
				case SubscriptionType.Household:
					return this.Household.Address;

				case SubscriptionType.LegalPerson:
					return this.LegalPersonContact.Address;

				default:
					throw new NotImplementedException ();
			}
		}


		public static AiderSubscriptionRefusalEntity Create
		(
			BusinessContext businessContext,
			AiderHouseholdEntity household
		)
		{
			var refusal = businessContext.CreateAndRegisterEntity<AiderSubscriptionRefusalEntity> ();

			refusal.RefusalType = SubscriptionType.Household;
			refusal.Household = household;

			return refusal;
		}


		public static AiderSubscriptionRefusalEntity Create
		(
			BusinessContext businessContext,
			AiderContactEntity legalPersonContact
		)
		{
			var refusal = businessContext.CreateAndRegisterEntity<AiderSubscriptionRefusalEntity> ();

			refusal.RefusalType = SubscriptionType.LegalPerson;
			refusal.LegalPersonContact = legalPersonContact;

			return refusal;
		}


		public static AiderSubscriptionRefusalEntity Create
		(
			BusinessContext businessContext,
			AiderSubscriptionEntity subscription
		)
		{
			switch (subscription.SubscriptionType)
			{
				case SubscriptionType.Household:
					var household = subscription.Household;
					return AiderSubscriptionRefusalEntity.Create (businessContext, household);

				case SubscriptionType.LegalPerson:
					var contact = subscription.LegalPersonContact;
					return AiderSubscriptionRefusalEntity.Create (businessContext, contact);

				default:
					throw new NotImplementedException ();
			}
		}


		public static void Delete
		(
			BusinessContext businessContext,
			AiderSubscriptionRefusalEntity refusal
		)
		{
			businessContext.DeleteEntity (refusal);
		}


		public static AiderSubscriptionRefusalEntity FindRefusal
		(
			BusinessContext businessContext,
			AiderHouseholdEntity household
		)
		{
			var example = new AiderSubscriptionRefusalEntity ()
			{
				RefusalType = SubscriptionType.Household,
				Household = household,
			};

			return AiderSubscriptionRefusalEntity.FindRefusal (businessContext, example, household);
		}


		public static AiderSubscriptionRefusalEntity FindRefusal
		(
			BusinessContext businessContext,
			AiderContactEntity legalPersonContact
		)
		{
			var example = new AiderSubscriptionRefusalEntity ()
			{
				RefusalType = SubscriptionType.LegalPerson,
				LegalPersonContact = legalPersonContact,
			};

			return AiderSubscriptionRefusalEntity.FindRefusal
			(
				businessContext, example, legalPersonContact
			);
		}


		private static AiderSubscriptionRefusalEntity FindRefusal
		(
			BusinessContext businessContext,
			AiderSubscriptionRefusalEntity example,
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

			var result = dataContext.GetByRequest<AiderSubscriptionRefusalEntity> (request);

			return result.FirstOrDefault ();
		}


		public static IEnumerable<AiderSubscriptionRefusalEntity> FindRefusals(BusinessContext businessContext, AiderPersonEntity person)
		{
			foreach (var household in person.Households)
			{
				var refusal = AiderSubscriptionRefusalEntity.FindRefusal (businessContext, household);

				if (refusal.IsNotNull ())
				{
					yield return refusal;
				}
			}
		}


		public static IList<AiderSubscriptionRefusalEntity> FindRefusals
		(
			BusinessContext businessContext,
			AiderLegalPersonEntity legalPerson
		)
		{
			var dataContext = businessContext.DataContext;

			if (!dataContext.IsPersistent (legalPerson))
			{
				return new List<AiderSubscriptionRefusalEntity> ();
			}

			var example = new AiderSubscriptionRefusalEntity ()
			{
				RefusalType = SubscriptionType.LegalPerson,
				LegalPersonContact = new AiderContactEntity ()
				{
					ContactType = ContactType.Legal,
					LegalPerson = legalPerson
				},
			};

			return dataContext.GetByExample (example);
		}


		public static void CheckRefusalDoesNotExist
		(
			BusinessContext businessContext,
			AiderHouseholdEntity receiver
		)
		{
			var result = AiderSubscriptionRefusalEntity.FindRefusal (businessContext, receiver);

			AiderSubscriptionRefusalEntity.CheckRefusalDoesNotExist (result);
		}


		public static void CheckRefusalDoesNotExist
		(
			BusinessContext businessContext,
			AiderContactEntity receiver
		)
		{
			var result = AiderSubscriptionRefusalEntity.FindRefusal (businessContext, receiver);

			AiderSubscriptionRefusalEntity.CheckRefusalDoesNotExist (result);
		}


		private static void CheckRefusalDoesNotExist(AiderSubscriptionRefusalEntity result)
		{
			if (result != null)
			{
				var message = "Un refus existe déjà pour ce destinataire.";

				throw new BusinessRuleException (message);
			}
		}
	}
}
