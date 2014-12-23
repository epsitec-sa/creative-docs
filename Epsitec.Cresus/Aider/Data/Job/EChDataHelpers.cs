//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;
using System.Data;
using Epsitec.Aider.Entities;
using Epsitec.Common.Types;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Data.Platform;
using Epsitec.Aider.Rules;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Enumerations;

namespace Epsitec.Aider.Data.Job
{
	internal static class EChDataHelpers
	{
		public static AiderPersonEntity GetOrCreateAiderPersonEntity(BusinessContext businessContext, EChPerson eChPerson)
		{
			AiderPersonEntity aiderPerson = new AiderPersonEntity ();
			aiderPerson = EChDataHelpers.GetAiderPersonEntity (businessContext, eChPerson);
			//create ref aiderPerson if needed
			if (aiderPerson.IsNull ())
			{
				var eChPersonEntity = EChDataHelpers.GetEchPersonEntity (businessContext, eChPerson);
				var mrMrs = EChDataImporter.GuessMrMrs (eChPersonEntity.PersonSex, eChPersonEntity.PersonDateOfBirth.Value, eChPersonEntity.AdultMaritalStatus);
				aiderPerson = AiderPersonEntity.Create (businessContext, eChPersonEntity, mrMrs);
			}

			return aiderPerson;
		}

		public static eCH_PersonEntity CreateEChPersonEntity(BusinessContext businessContext, EChPerson eChPerson)
		{
			var personEntity = businessContext.CreateAndRegisterEntity<eCH_PersonEntity> ();
			personEntity.PersonId = eChPerson.Id;
			personEntity.PersonOfficialName = eChPerson.OfficialName;
			personEntity.PersonFirstNames = eChPerson.FirstNames;
			personEntity.PersonDateOfBirth = eChPerson.DateOfBirth;
			personEntity.PersonSex = eChPerson.Sex;
			personEntity.NationalityStatus = eChPerson.NationalityStatus;
			personEntity.NationalityCountryCode = eChPerson.NationalCountryCode;
			personEntity.Origins = eChPerson.OriginPlaces
				.Select (p => p.Name + " (" + p.Canton + ")")
				.Join ("\n");
			personEntity.AdultMaritalStatus = eChPerson.MaritalStatus;
			personEntity.CreationDate = Date.Today;
			personEntity.DataSource = Enumerations.DataSource.Government;
			personEntity.DeclarationStatus = PersonDeclarationStatus.Declared;
			personEntity.RemovalReason = RemovalReason.None;

			return personEntity;
		}

		public static eCH_PersonEntity CreateEChPersonEntity(BusinessContext businessContext, EChPerson eChPerson, Enumerations.DataSource source, PersonDeclarationStatus status)
		{
			var personEntity = businessContext.CreateAndRegisterEntity<eCH_PersonEntity> ();
			personEntity.PersonId = eChPerson.Id;
			personEntity.PersonOfficialName = eChPerson.OfficialName;
			personEntity.PersonFirstNames = eChPerson.FirstNames;
			personEntity.PersonDateOfBirth = eChPerson.DateOfBirth;
			personEntity.PersonSex = eChPerson.Sex;
			personEntity.NationalityStatus = eChPerson.NationalityStatus;
			personEntity.NationalityCountryCode = eChPerson.NationalCountryCode;
			personEntity.Origins = eChPerson.OriginPlaces
				.Select (p => p.Name + " (" + p.Canton + ")")
				.Join ("\n");
			personEntity.AdultMaritalStatus = eChPerson.MaritalStatus;
			personEntity.CreationDate = Date.Today;
			personEntity.DataSource = source;
			personEntity.DeclarationStatus = status;
			personEntity.RemovalReason = RemovalReason.None;

			return personEntity;
		}

		public static void UpdateEChPersonEntity(eCH_PersonEntity existingPersonEntity, EChPerson eChPerson)
		{
			existingPersonEntity.PersonOfficialName = eChPerson.OfficialName;
			existingPersonEntity.PersonFirstNames = eChPerson.FirstNames;
			existingPersonEntity.PersonDateOfBirth = eChPerson.DateOfBirth;
			existingPersonEntity.PersonSex = eChPerson.Sex;
			existingPersonEntity.NationalityStatus = eChPerson.NationalityStatus;
			existingPersonEntity.NationalityCountryCode = eChPerson.NationalCountryCode;
			existingPersonEntity.Origins = eChPerson.OriginPlaces
				.Select (p => p.Name + " (" + p.Canton + ")")
				.Join ("\n");
			existingPersonEntity.AdultMaritalStatus = eChPerson.MaritalStatus;
			existingPersonEntity.DataSource = Enumerations.DataSource.Government;
			existingPersonEntity.DeclarationStatus = PersonDeclarationStatus.Declared;
			existingPersonEntity.RemovalReason = RemovalReason.None;
		}

		public static void UpdateAiderPersonEntity(AiderPersonEntity existingAiderPersonEntity, eCH_PersonEntity eChPersonEntity, PersonMrMrs mrMrs)
		{
			existingAiderPersonEntity.eCH_Person = eChPersonEntity;
			existingAiderPersonEntity.MrMrs = mrMrs;
			existingAiderPersonEntity.Visibility = PersonVisibilityStatus.Default;
			existingAiderPersonEntity.Confession = PersonConfession.Protestant;
		}

		public static void ApplyEChReportedPersonChanges (List<FormattedText> changes, eCH_PersonEntity eChPerson,eCH_PersonEntity eChPersonNew)
		{
			if (StringUtils.NotEqualOrEmpty (eChPerson.PersonOfficialName, eChPersonNew.PersonOfficialName))
			{
				changes.Add (TextFormatter.FormatText ("Nom:", eChPerson.PersonOfficialName, "->", eChPersonNew.PersonOfficialName));
				eChPerson.PersonOfficialName = eChPersonNew.PersonOfficialName ?? "";
			}

			if (StringUtils.NotEqualOrEmpty (eChPerson.PersonFirstNames, eChPersonNew.PersonFirstNames))
			{
				changes.Add (TextFormatter.FormatText ("Prénom:", eChPerson.PersonFirstNames, "->", eChPersonNew.PersonFirstNames));
				eChPerson.PersonFirstNames = eChPersonNew.PersonFirstNames ?? "";
			}

			if (eChPerson.PersonDateOfBirth != eChPersonNew.PersonDateOfBirth)
			{
	//-								changes.Add (TextFormatter.FormatText ("Date de naissance:", eChPerson.PersonDateOfBirth, "->", eChPersonNew.PersonDateOfBirth));
				eChPerson.PersonDateOfBirth = eChPersonNew.PersonDateOfBirth;
			}

			if (eChPerson.AdultMaritalStatus != eChPersonNew.AdultMaritalStatus)
			{
				if (eChPerson.AdultMaritalStatus != PersonMaritalStatus.None)
				{
					changes.Add (TextFormatter.FormatText ("État civil:", eChPerson.AdultMaritalStatus, "->", eChPersonNew.AdultMaritalStatus));
				}
				eChPerson.AdultMaritalStatus = eChPersonNew.AdultMaritalStatus;
			}

			if (eChPerson.PersonSex != eChPersonNew.PersonSex)
			{
	//-								changes.Add (TextFormatter.FormatText ("Sexe:", eChPerson.PersonSex, "->", eChPersonNew.PersonSex));
				eChPerson.PersonSex = eChPersonNew.PersonSex;
			}

			if (eChPerson.NationalityCountryCode != eChPersonNew.NationalityCountryCode)
			{
	//-								changes.Add (TextFormatter.FormatText ("Nationalité:", eChPerson.NationalityCountryCode, "->", eChPersonNew.NationalityCountryCode));
				eChPerson.NationalityCountryCode = eChPersonNew.NationalityCountryCode;
			}

			if (eChPerson.NationalityStatus != eChPersonNew.NationalityStatus)
			{
	//-								changes.Add (TextFormatter.FormatText ("Statut nationalité:", eChPerson.NationalityStatus, "->", eChPersonNew.NationalityStatus));
				eChPerson.NationalityStatus = eChPersonNew.NationalityStatus;
			}

			if (eChPerson.Origins != eChPersonNew.Origins)
			{
	//-								changes.Add (TextFormatter.FormatText ("Origines:", eChPerson.Origins, "->", eChPersonNew.Origins));
				eChPerson.Origins = eChPersonNew.Origins;
			}
		}

		public static List<FormattedText> GetAddressChanges(AiderAddressEntity familyAddress, EChAddress newRchAddress)
		{
			var oldAddress = familyAddress.GetPostalAddress ();
			var newAddress = newRchAddress.GetSwissPostalAddress ();
			var changes = new List<FormattedText> ();

			changes.Add (TextFormatter.FormatText ("Changement dans l'adresse:"));
			changes.Add (TextFormatter.FormatText (oldAddress, "\n->\n", newAddress));

			return changes;
		}

		public static bool UpdateAddress(eCH_AddressEntity familyAddress, Epsitec.Aider.Data.ECh.EChAddress newRchAddress, List<FormattedText> changes)
		{
			var oldAddress = familyAddress.GetSummary ();

			if (EChDataHelpers.UpdateAddress (familyAddress, newRchAddress))
			{
				var newAddress = familyAddress.GetSummary ();

				changes.Add (TextFormatter.FormatText ("Changement dans l'adresse:"));
				changes.Add (TextFormatter.FormatText (oldAddress, "\n->\n", newAddress));
				
				return true;
			}

			return false;
		}

		private static bool UpdateAddress(eCH_AddressEntity familyAddress, EChAddress newRchAddress)
		{
			bool changed = false;

			if (StringUtils.NotEqualOrEmpty (familyAddress.AddressLine1, newRchAddress.AddressLine1))
			{
				familyAddress.AddressLine1 = newRchAddress.AddressLine1 ?? "";
			}

			if (StringUtils.NotEqualOrEmpty (familyAddress.HouseNumber, newRchAddress.HouseNumber))
			{
				familyAddress.HouseNumber = newRchAddress.HouseNumber ?? "";
				changed = true;
			}

			if (StringUtils.NotEqualOrEmpty (familyAddress.Street, newRchAddress.Street))
			{
				familyAddress.Street = newRchAddress.Street ?? "";
				changed = true;
			}

			if ((familyAddress.SwissZipCode != newRchAddress.SwissZipCode) ||
									(familyAddress.SwissZipCodeAddOn != newRchAddress.SwissZipCodeAddOn) ||
									(familyAddress.SwissZipCodeId != newRchAddress.SwissZipCodeId))
			{
				familyAddress.SwissZipCode      = newRchAddress.SwissZipCode;
				familyAddress.SwissZipCodeAddOn = newRchAddress.SwissZipCodeAddOn;
				familyAddress.SwissZipCodeId    = newRchAddress.SwissZipCodeId;
				changed = true;
			}

			if (StringUtils.NotEqualOrEmpty (familyAddress.Town, newRchAddress.Town))
			{
				familyAddress.Town = newRchAddress.Town ?? "";
				changed = true;
			}

			if (StringUtils.NotEqualOrEmpty (familyAddress.Country, newRchAddress.CountryCode))
			{
				familyAddress.Country = newRchAddress.CountryCode ?? "";
				changed = true;
			}

			return changed;
		}


		public static void RevertEChAddress(eCH_ReportedPersonEntity family, AiderHouseholdEntity household)
		{
			var echAddress = family.Address;
			var oldAddress = household.Address;
			var oldTown = oldAddress.Town;

			echAddress.AddressLine1       = oldAddress.AddressLine1;
			echAddress.StreetUserFriendly = oldAddress.StreetUserFriendly;
			echAddress.HouseNumber        = oldAddress.HouseNumberAndComplement;
			echAddress.SwissZipCode       = oldTown.SwissZipCode.GetValueOrDefault (0);
			echAddress.SwissZipCodeAddOn  = oldTown.SwissZipCodeAddOn.GetValueOrDefault (0);
			echAddress.SwissZipCodeId     = oldTown.SwissZipCodeId.GetValueOrDefault (0);
			echAddress.Country            = oldTown.Country.IsoCode;
			echAddress.Town               = oldTown.Name;
		}


		public static void DeleteAiderHouseholdAndSubscription(BusinessContext businessContext, AiderHouseholdEntity household)
		{
			//Remove subscription if needed
			var oldSubscription = AiderSubscriptionEntity.FindSubscription (businessContext, household);
			if (oldSubscription.IsNotNull ())
			{
				businessContext.DeleteEntity (oldSubscription);
			}

			//Remove old household
			businessContext.DeleteEntity (household);
		}

		public static AiderAddressEntity CreateAiderAddressEntityTemplate(BusinessContext businessContext, eCH_ReportedPersonEntity eChReportedPerson)
		{
			var town = EChDataHelpers.GetAiderTownEntity (businessContext, eChReportedPerson.Address);
			return EChDataHelpers.CreateAiderAddressEntityTemplate (eChReportedPerson.Address, town);
		}

		public static AiderAddressEntity CreateAiderAddressEntityTemplate(BusinessContext businessContext, EChReportedPerson eChReportedPerson)
		{
			var eChAddressEntity = EChDataHelpers.GetEchAddressEntity (businessContext, eChReportedPerson.Address);
			var town = EChDataHelpers.GetAiderTownEntity (businessContext, eChReportedPerson.Address);
			return EChDataHelpers.CreateAiderAddressEntityTemplate (eChAddressEntity, town);
		}

		private static AiderAddressEntity CreateAiderAddressEntityTemplate(eCH_AddressEntity address, AiderTownEntity town)
		{
			var template = new AiderAddressEntity ();
			var houseNumber = StringUtils.ParseNullableInt (SwissPostStreet.StripHouseNumber (address.HouseNumber));
			var houseNumberComplement = SwissPostStreet.GetHouseNumberComplement (address.HouseNumber);

			if (string.IsNullOrWhiteSpace (houseNumberComplement))
			{
				houseNumberComplement = null;
			}

			template.AddressLine1 = address.AddressLine1;
			template.Street = address.Street;
			template.HouseNumber = houseNumber;
			template.HouseNumberComplement = houseNumberComplement;
			template.Town = town;

			return template;
		}

		public static void UpdateAiderHouseholdAddress(BusinessContext businessContext, AiderHouseholdEntity aiderHousehold, eCH_ReportedPersonEntity family)
		{
			var houseNumberAlpha      = SwissPostStreet.StripHouseNumber (family.Address.HouseNumber);
			var houseNumber           = StringUtils.ParseNullableInt (houseNumberAlpha);
			var houseNumberComplement = SwissPostStreet.GetHouseNumberComplement (family.Address.HouseNumber);

			if (string.IsNullOrWhiteSpace (houseNumberComplement))
			{
				houseNumberComplement = null;
			}

			var address = aiderHousehold.Address;
			var town    = EChDataHelpers.GetAiderTownEntity (businessContext, family.Address.SwissZipCodeId);
			var line1   = family.Address.AddressLine1;

			//	Properly maps the post box to the post box field...
			if (!string.IsNullOrEmpty(line1))
			{
				if (line1.StartsWith ("case postale", System.StringComparison.OrdinalIgnoreCase))
				{
					AiderAddressBusinessRules.UpdatePostBox (address, line1);
					line1 = "";
				}
			}

			if ((string.IsNullOrEmpty (line1)) &&
				(string.IsNullOrEmpty (address.AddressLine1) == false))
			{
				//	The eCH address has no address complement (e.g. "c/o ...", "p/a ...", "Chalet Xyz", "EMS Zzz", etc.)

				var houseNumberAndComplement = AiderAddressEntity.GetCleanHouseNumberAndComplement (houseNumber, houseNumberComplement);

				if ((address.Street == family.Address.Street) &&
					((address.HouseNumberAndComplement == houseNumberComplement) || string.IsNullOrEmpty (address.HouseNumberAndComplement)))
				{
					//	The person did not move to a new address, as the house number did not
					//	change (or was simply missing before). Keep the extra address line !

					line1 = address.AddressLine1;
				}
			}


			address.AddressLine1          = line1;
			address.Street                = family.Address.Street;
			address.HouseNumber           = houseNumber;
			address.HouseNumberComplement = houseNumberComplement;
			address.Town                  = town;

			aiderHousehold.RefreshCache ();
		}

		public static void CreateOrUpdateAiderSubscription(BusinessContext businessContext, AiderHouseholdEntity household)
		{
			var dataContext = businessContext.DataContext;
			var hasSubscription = false;

			if (dataContext.IsPersistent (household))
			{
				var subscriptionExample = new AiderSubscriptionEntity ()
				{
					Household = household
				};

				var subscription = businessContext.DataContext.GetByExample<AiderSubscriptionEntity> (subscriptionExample).FirstOrDefault ();

				if (subscription.IsNotNull ())
				{
					subscription.RefreshCache ();
					hasSubscription = true;
				}

				var refusalExample = new AiderSubscriptionRefusalEntity ()
				{
					Household = household
				};

				var refusal = businessContext.DataContext.GetByExample<AiderSubscriptionRefusalEntity> (refusalExample).FirstOrDefault ();

				if (refusal.IsNotNull ())
				{
					refusal.RefreshCache ();
					hasSubscription = true;
				}
			}

			if (hasSubscription == false)
			{
				AiderSubscriptionEntity.Create (businessContext, household);
			}
		}

		public static eCH_PersonEntity GetEchPersonEntity(BusinessContext businessContext, EChPerson person)
		{
			if (person == null)
			{
				return null;
			}

			var personExample = new eCH_PersonEntity ()
			{
				PersonId = person.Id
			};

			return businessContext.DataContext.GetByExample<eCH_PersonEntity> (personExample).FirstOrDefault ();
		}

		public static AiderPersonEntity GetAiderPersonEntity(BusinessContext businessContext, EChPerson person)
		{
			if (person == null)
			{
				return null;
			}

			var personExample = new AiderPersonEntity();

			personExample.eCH_Person = new eCH_PersonEntity()
			{
				PersonId = person.Id
			};

			return businessContext.DataContext.GetByExample<AiderPersonEntity>(personExample).FirstOrDefault();
		}

		public static AiderPersonEntity GetAiderPersonEntity(BusinessContext businessContext, eCH_PersonEntity person)
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

		public static void SetupHousehold(BusinessContext businessContext, AiderPersonEntity aiderPerson, AiderHouseholdEntity aiderHousehold,
			/**/						  eCH_ReportedPersonEntity eChReportedPerson, bool isHead1 = false, bool isHead2 = false)
		{
			var isHead = isHead1 || isHead2;

			//If we need to add AiderPerson in AiderHousehold
			if (!aiderHousehold.Members.Contains (aiderPerson))
			{
				//Check for an already non Ech person (based on birthdate and firstname)
				if (aiderHousehold.Contacts.Where (c => !c.Person.IsGovernmentDefined).Any (c => c.Person.GetPersonCheckKey () == aiderPerson.GetPersonCheckKey ()))
				{
					//Merge all matching non ECh AiderPerson with current AiderPerson
					foreach (var person in aiderHousehold.Members.Where (p => p.GetPersonCheckKey () == aiderPerson.GetPersonCheckKey () && !p.IsGovernmentDefined))
					{
						try
						{
							AiderPersonEntity.MergePersons (businessContext, aiderPerson, person);
						}
						catch
						{

						}
					}
					if (aiderPerson.MainContact.IsNull ())
					{
						var contact = AiderContactEntity.Create (businessContext, aiderPerson, aiderHousehold, isHead);
					}
				}
				else
				{
					var contact = AiderContactEntity.Create (businessContext, aiderPerson, aiderHousehold, isHead);
				}				
			}

			var eChPerson = aiderPerson.eCH_Person;

			if (eChPerson.ReportedPerson1.IsNull ())
			{
				eChPerson.ReportedPerson1 = eChReportedPerson;
				eChPerson.Address1 = eChReportedPerson.Address;
			}
			else
			{
				eChPerson.ReportedPerson2 = eChReportedPerson;
				eChPerson.Address2 = eChReportedPerson.Address;
			}

			if (isHead1)
			{
				eChReportedPerson.Adult1 = eChPerson;
			}
			else if (isHead2)
			{
				eChReportedPerson.Adult2 = eChPerson;
			}
			else
			{
				eChReportedPerson.Children.Add (eChPerson);
				eChReportedPerson.RemoveDuplicates ();
			}
		}

		public static AiderPersonEntity GetAiderPersonEntity(BusinessContext businessContext, string eChPersonId)
		{
			var personExample = new AiderPersonEntity();

			personExample.eCH_Person = new eCH_PersonEntity()
			{
				PersonId = eChPersonId
			};

			return businessContext.DataContext.GetByExample<AiderPersonEntity>(personExample).FirstOrDefault();
		}

		public static eCH_ReportedPersonEntity GetEchReportedPersonEntity(BusinessContext businessContext, EChReportedPerson reportedPerson)
		{
			var reportedPersonExample = new eCH_ReportedPersonEntity ();
			var req = new Request ();
			if (reportedPerson.Adult1 != null && reportedPerson.Adult2 != null)
			{
				reportedPersonExample.Adult1 = new eCH_PersonEntity ()
				{
					PersonId = reportedPerson.Adult1.Id
				};
				reportedPersonExample.Adult2 = new eCH_PersonEntity ()
				{
					PersonId = reportedPerson.Adult2.Id
				};

			}

			if (reportedPerson.Adult1 != null && reportedPerson.Adult2 == null)
			{
				reportedPersonExample.Adult1 = new eCH_PersonEntity ()
				{
					PersonId = reportedPerson.Adult1.Id
				};
			}

			return businessContext.DataContext.GetByExample<eCH_ReportedPersonEntity> (reportedPersonExample).FirstOrDefault ();
		}

		private static eCH_AddressEntity GetEchAddressEntity(BusinessContext businessContext, EChAddress address)
		{
			var addressExample = new eCH_AddressEntity ()
			{
				SwissZipCode = address.SwissZipCode,
				SwissZipCodeAddOn = address.SwissZipCodeAddOn,
				Street = address.Street,
				HouseNumber = address.HouseNumber
			};

			return businessContext.DataContext.GetByExample<eCH_AddressEntity> (addressExample).FirstOrDefault ();
		}

		public static AiderHouseholdEntity GetAiderHousehold(BusinessContext businessContext, eCH_PersonEntity refPerson)
		{
			if (refPerson.IsNull ())
			{
				return null;
			}

			var personExample = new AiderPersonEntity();
			var contactExample = new AiderContactEntity();
			var householdExample = new AiderHouseholdEntity();
			personExample.eCH_Person = refPerson;
			contactExample.Person = personExample;
			contactExample.Household = householdExample;
			var request = new Request()
			{
				RootEntity = contactExample,
				RequestedEntity = householdExample
			};

			return businessContext.DataContext.GetByRequest<AiderHouseholdEntity>(request).FirstOrDefault();
		}

		public static AiderHouseholdEntity GetAiderHousehold(BusinessContext businessContext, AiderPersonEntity refPerson)
		{
			return EChDataHelpers.GetAiderHouseholds (businessContext, refPerson).FirstOrDefault ();
		}

		public static IEnumerable<AiderHouseholdEntity> GetAiderHouseholds(BusinessContext businessContext, AiderPersonEntity refPerson)
		{
			var contactExample = new AiderContactEntity();
			var householdExample = new AiderHouseholdEntity();
			contactExample.Person = refPerson;
			contactExample.Household = householdExample;
			var request = new Request()
			{
				RootEntity = contactExample,
				RequestedEntity = householdExample
			};

			return businessContext.GetByRequest<AiderHouseholdEntity> (request);
		}

		public static AiderTownEntity GetAiderTownEntity(BusinessContext businessContext, EChAddress address)
		{
			var townExample = new AiderTownEntity ()
			{
				SwissZipCodeId = address.SwissZipCodeId
			};

			return businessContext.DataContext.GetByExample<AiderTownEntity> (townExample).FirstOrDefault ();
		}

		public static AiderTownEntity GetAiderTownEntity(BusinessContext businessContext, eCH_AddressEntity address)
		{
			var townExample = new AiderTownEntity ()
			{
				SwissZipCodeId = address.SwissZipCodeId
			};

			return businessContext.DataContext.GetByExample<AiderTownEntity> (townExample).FirstOrDefault ();
		}

		public static AiderTownEntity GetAiderTownEntity(BusinessContext businessContext, int swissZipCodeId)
		{
			var townExample = new AiderTownEntity()
			{
				SwissZipCodeId = swissZipCodeId
			};

			return businessContext.DataContext.GetByExample<AiderTownEntity>(townExample).FirstOrDefault();
		}

		/// <summary>
		/// Return true when same address is detected
		/// </summary>
		/// <param name="householdAddress"></param>
		/// <param name="rchAddress"></param>
		/// <returns></returns>
		public static bool AddressComparator(AiderAddressEntity householdAddress, EChAddress rchAddress)
		{
			if (householdAddress.Street.IsNullOrWhiteSpace () || householdAddress.Town.IsNull ())
			{
				return false;
			}

			var h1 = householdAddress.Street
						.FirstToken (",")
						.Replace(" ","")
						.ToUpper () + 
						householdAddress.HouseNumberAndComplement.ToUpper ()
						.Replace (" ", "") + 
						householdAddress.Town.SwissZipCodeId.ToString ();

			var h2 = rchAddress.Street
						.FirstToken(",")
						.Replace(" ","")
						.ToUpper () + 
						rchAddress.HouseNumber.ToUpper ()
						.Replace (" ", "") + 
						rchAddress.SwissZipCodeId.ToString ();

			if (h1 == h2)
				return true;
			else
				return false;
		}
	}
}
