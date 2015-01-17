//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Data.Normalization;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;


namespace Epsitec.Aider.Data.Subscription
{
	internal static class SubscriptionDataImporter
	{
		public static void Import (CoreData coreData, ParishAddressRepository parishRepository, IEnumerable<SubscriptionData> subscriptions)
		{
			var split = subscriptions.Split (s => s.IsLegalPerson);
			var personSubscriptionsSplit = split.Item1.ToList ().Split (s => s.IsRichData);
			var personSubscriptions = personSubscriptionsSplit.Item1.ToList ();
			var richSubscriptions = personSubscriptionsSplit.Item2.ToList ();
			var legalPersonSubscriptions = split.Item2.ToList ();

			

			using (var businessContext = new BusinessContext (coreData, false))
			{
				var townRepository = new AiderTownRepository (businessContext);

				Debug.WriteLine ("[" + DateTime.Now + "] Importing rich subscriptions");

				SubscriptionDataImporter.ImportRichSubscriptions (coreData, businessContext, parishRepository, townRepository, richSubscriptions);

				Debug.WriteLine ("[" + DateTime.Now + "] Importing person subscriptions");

				SubscriptionDataImporter.ImportPersonSubscriptions (coreData, businessContext, parishRepository, townRepository, personSubscriptions);

				Debug.WriteLine ("[" + DateTime.Now + "] Importing legal person subscriptions");

				SubscriptionDataImporter.ImportLegalPersonSubscriptions (businessContext, parishRepository, townRepository, legalPersonSubscriptions);

				Debug.WriteLine ("[" + DateTime.Now + "] Saving changes");

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);

				Debug.WriteLine ("[" + DateTime.Now + "] Done");
			}
		}


		private static void ImportRichSubscriptions(CoreData coreData, BusinessContext businessContext, ParishAddressRepository parishRepository, AiderTownRepository townRepository, IList<SubscriptionData> subscriptions)
		{
			if (subscriptions.Count == 0)
			{
				return;
			}

			Dictionary<string, List<SubscriptionData>> dict = new Dictionary<string, List<SubscriptionData>> ();

			foreach (var sub in subscriptions)
			{
				List<SubscriptionData> list;

				if (dict.TryGetValue (sub.HouseholdToken, out list) == false)
				{
					list = new List<SubscriptionData> ();
					dict[sub.HouseholdToken] = list;
				}

				list.Add (sub);
			}

			Debug.WriteLine (string.Format ("Found {0} persons, {1} households", subscriptions.Count (), dict.Count));

			foreach (var item in dict)
			{
				SubscriptionDataImporter.ImportHouseholdAndSubscriptions (businessContext, parishRepository, townRepository, item.Value);
			}
		}

		private static void ImportPersonSubscriptions(CoreData coreData, BusinessContext businessContext, ParishAddressRepository parishRepository, AiderTownRepository townRepository, IList<SubscriptionData> subscriptions)
		{
			if (subscriptions.Count == 0)
			{
				return;
			}

			var normalizedSubscriptionPersons = Normalizer.Normalize (subscriptions);
			var normalizedAiderPersons = Normalizer.Normalize (coreData);

			var matches = NormalizedDataMatcher.FindMatches	(normalizedSubscriptionPersons.Keys, normalizedAiderPersons.Keys, considerDateOfBirth: false, considerSex: true, considerAddressAsMostRelevant: true);

			SubscriptionDataImporter.LogMatchResult (normalizedSubscriptionPersons, matches);

			var personKeysInDb = matches
				.Where (m => m.Item2 != null)
				.Select (m => normalizedAiderPersons[m.Item2.Item1])
				.Distinct ()
				.ToList ();

			var keyToSubscription = matches
				.Where (m => m.Item2 != null)
				.GroupBy (m => normalizedAiderPersons[m.Item2.Item1])
				.ToDictionary
				(
					g => g.Key,
					g => normalizedSubscriptionPersons[g.First ().Item1]
				);

			foreach (var personKey in personKeysInDb)
			{
				SubscriptionDataImporter.EnsurePersonSubscriptionExists
				(
					businessContext, keyToSubscription[personKey], personKey
				);
			}

			var subscriptionsWithoutPersonInDb = matches
				.Where (m => m.Item2 == null)
				.Select (m => normalizedSubscriptionPersons[m.Item1])
				.Distinct ()
				.ToList ();

			foreach (var subscription in subscriptionsWithoutPersonInDb)
			{
				SubscriptionDataImporter.ImportHouseholdAndSubscriptions
				(
					businessContext, parishRepository, townRepository, subscription
				);
			}
		}


		private static void LogMatchResult
		(
			Dictionary<NormalizedPerson, SubscriptionData> normalizedSubscriptionPersons,
			IEnumerable<Tuple<NormalizedPerson, Tuple<NormalizedPerson, MatchData>>> matches
		)
		{
			var matchesOk = matches
				.Where (m => m.Item2 != null)
				.ToList ();

			Debug.WriteLine ("==============================================================");
			Debug.WriteLine ("MATCH FOUND: " + matchesOk.Count);

			foreach (var match in matchesOk)
			{
				Debug.WriteLine ("-------------------------------------------------------------");

				var m1 = match.Item1;
				var a1 = m1.Households.Single ().Address;
				var msg1 = "S: " + string.Join
				(
					", ",
					m1.Firstname,
					m1.Lastname,
					m1.Sex,
					a1.Street,
					a1.HouseNumber,
					a1.ZipCode,
					a1.Town
				);
				Debug.WriteLine (msg1);

				var m2 = match.Item2.Item1;
				foreach (var a2 in m2.Households.Select (h => h.Address))
				{
					var msg2 = "M: " + string.Join
					(
						", ",
						m2.Firstname,
						m2.Lastname,
						m2.Sex,
						a2.Street,
						a2.HouseNumber,
						a2.ZipCode,
						a2.Town
					);
					Debug.WriteLine (msg2);
				}

				var d = match.Item2.Item2;
				var msg3 = "R: " + string.Join
				(
					", ",
					d.Firstname,
					d.Lastname,
					d.Sex,
					d.Address
				);
				Debug.WriteLine (msg3);
			}

			var matchesNok = matches
				.Where (m => m.Item2 == null)
				.ToList ();

			Debug.WriteLine ("==============================================================");
			Debug.WriteLine ("MATCH NOT FOUND: " + matchesNok.Count);

			foreach (var match in matchesNok)
			{
				var s = normalizedSubscriptionPersons[match.Item1];
				var msg = string.Join
				(
					", ",
					s.Firstname,
					s.Lastname,
					s.Title,
					s.StreetName,
					s.HouseNumber,
					s.ZipCode,
					s.Town
				);
				Debug.WriteLine (msg);
			}
		}


		private static void EnsurePersonSubscriptionExists
		(
			BusinessContext businessContext,
			SubscriptionData subscription,
			EntityKey personKey
		)
		{
			var dataContext = businessContext.DataContext;
			var person = (AiderPersonEntity) dataContext.ResolveEntity (personKey);
			
			var households = person.Households;
			var household = households.First ();

			var subscriptions = households
				.Select (h => AiderSubscriptionEntity.FindSubscription (businessContext, h))
				.Where (s => s.IsNotNull ())
				.ToList ();

			var count = subscription.NbCopies ?? 1;

			var parish = person.ParishGroup;
			var regionId = subscription.RegionalEdition;
			var region = SubscriptionDataImporter.GetRegion (businessContext, parish, regionId);
			
			if (subscriptions.Any ())
			{
				var existingSubscription = subscriptions.First ();

				existingSubscription.Count = count;
				existingSubscription.RegionalEdition = region;
			}
			else
			{
				AiderSubscriptionEntity.Create (businessContext, household, region, count);
			}
		}


		private static void ImportHouseholdAndSubscriptions
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository,
			AiderTownRepository townRepository,
			SubscriptionData subscription
		)
		{
			var household = businessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
			var address = household.Address;

			address.AddressLine1 = subscription.FirstAddressLine;
			address.Street = subscription.StreetName;
			address.HouseNumber = subscription.HouseNumber;
			address.HouseNumberComplement = subscription.HouseNumberComplement;
			address.PostBox = subscription.PostBox;
			address.Town = townRepository.GetTown
			(
				subscription.ZipCode, subscription.Town, subscription.CountryCode
			);

			for (int i = 0; i < subscription.GetNbPersons (); i++)
			{
				var person = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
				var eChPerson = person.eCH_Person;

				eChPerson.PersonFirstNames = subscription.GetPersonFirstname (i);
				eChPerson.PersonOfficialName = subscription.Lastname;

				var title = subscription.GetPersonTitle (i);
				person.MrMrs = TextParser.ParsePersonMrMrs (title);
				
				eChPerson.PersonSex = EnumUtils.GuessSex (title);
				eChPerson.DataSource = Enumerations.DataSource.Undefined;
				eChPerson.DeclarationStatus = PersonDeclarationStatus.NotDeclared;
				eChPerson.RemovalReason = RemovalReason.None;

				person.RefreshCache ();

				AiderContactEntity.Create (businessContext, person, household, true);
			}

			foreach (var member in household.Members)
			{
				ParishAssigner.AssignToParish (parishRepository, businessContext, member);
			}

			var count = subscription.NbCopies ?? 1;

			var parish = household.Members.First ().ParishGroup;
			var regionId = subscription.RegionalEdition;
			var region = SubscriptionDataImporter.GetRegion (businessContext, parish, regionId);

			AiderSubscriptionEntity.Create (businessContext, household, region, count);
		}


		private static void ImportHouseholdAndSubscriptions(BusinessContext businessContext,
															ParishAddressRepository parishRepository,
															AiderTownRepository townRepository,
															IList<SubscriptionData> subscriptions)
		{
			var subscription = subscriptions.First ();
			AiderHouseholdEntity household;

			var token = subscription.HouseholdToken;

			if (token.StartsWith ("[LVAI2]/"))
			{
				household = businessContext.ResolveEntity<AiderHouseholdEntity> (EntityKey.Parse (token));
			}
			else
			{
				household = businessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
			}

			var address = household.Address;

			address.AddressLine1          = subscription.FirstAddressLine;
			address.Street                = subscription.StreetName;
			address.HouseNumber           = subscription.HouseNumber;
			address.HouseNumberComplement = subscription.HouseNumberComplement;
			address.PostBox               = address.PostBox ?? subscription.PostBox;
			address.Town                  = townRepository.GetTown (subscription.ZipCode, subscription.Town, subscription.CountryCode);

			address.Phone1 = address.Phone1 ?? subscription.Phone;
			address.Mobile = address.Mobile ?? subscription.Mobile;
			address.Email = address.Email ?? subscription.Email;

			foreach (var sub in subscriptions)
			{
				var person = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
				var eChPerson = person.eCH_Person;

				eChPerson.PersonFirstNames = sub.Firstname;
				eChPerson.PersonOfficialName = sub.Lastname;

				var title = sub.Title;
				person.MrMrs = TextParser.ParsePersonMrMrs (title);

				eChPerson.PersonSex = sub.Sex == PersonSex.Unknown ? EnumUtils.GuessSex (title) : sub.Sex;
				eChPerson.DataSource = Enumerations.DataSource.Undefined;
				eChPerson.DeclarationStatus = PersonDeclarationStatus.NotDeclared;
				eChPerson.RemovalReason = RemovalReason.None;
				eChPerson.AdultMaritalStatus = sub.MaritalStatus;
				eChPerson.PersonDateOfBirth = sub.BirthDate;
				eChPerson.Origins = string.Join ("\n", sub.Origin.Split (',', ';', '+').Select (x => x.Trim ()));

				if ((string.IsNullOrEmpty (sub.Nationality) == false) &&
					(sub.Nationality.Length == 2))
				{
					eChPerson.NationalityStatus = PersonNationalityStatus.Defined;
					eChPerson.NationalityCountryCode = sub.Nationality;
				}

				person.Confession = sub.Confession;
				person.Profession = sub.Profession;

				if (string.IsNullOrEmpty (sub.Comment) == false)
				{
					AiderCommentEntity.CombineComments (person, sub.Comment);
				}


				person.RefreshCache ();

				bool isHead = subscriptions.Count == 1 || (sub.BirthDate.HasValue && person.Age >= 18);

				AiderContactEntity.Create (businessContext, person, household, isHead);
			}

			bool hasProtestantInHousehold = false;

			foreach (var member in household.Members)
			{
				if (businessContext.DataContext.IsPersistent (member) == false)
				{
					ParishAssigner.AssignToParish (parishRepository, businessContext, member);
				}

				hasProtestantInHousehold |= member.Confession == PersonConfession.Protestant;
			}

			if ((hasProtestantInHousehold) &&
				(AiderSubscriptionEntity.FindSubscription (businessContext, household) == null))
			{
				var count = subscription.NbCopies ?? 1;

				var parish = household.Members.First ().ParishGroup;
				var regionId = subscription.RegionalEdition;
				var region = SubscriptionDataImporter.GetRegion (businessContext, parish, regionId);

				AiderSubscriptionEntity.Create (businessContext, household, region, count);
			}
		}


		private static void ImportLegalPersonSubscriptions
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository,
			AiderTownRepository townRepository,
			IEnumerable<SubscriptionData> subscriptions
		)
		{
			foreach (var subscription in subscriptions)
			{
				var legalPersonContact = SubscriptionDataImporter.ImportLegalPerson
				(
					businessContext, parishRepository, townRepository, subscription
				);

				SubscriptionDataImporter.ImportLegalPersonSubscription
				(
					businessContext, subscription, legalPersonContact
				);
			}
		}


		private static AiderContactEntity ImportLegalPerson
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository,
			AiderTownRepository townRepository,
			SubscriptionData subscription
		)
		{
			var legalPerson = businessContext.CreateAndRegisterEntity<AiderLegalPersonEntity> ();
			var address = legalPerson.Address;
			var contact = AiderContactEntity.Create (businessContext, legalPerson);

			var corporateNameInPersonName = string.IsNullOrEmpty (subscription.CorporateName);

			if (corporateNameInPersonName)
			{
				var title = subscription.Title;
				var firstname = subscription.Firstname;
				var lastname = subscription.Lastname;

				legalPerson.Name = StringUtils.Join (" ", title, firstname, lastname);
			}
			else
			{
				legalPerson.Name = subscription.CorporateName;

				var firstname = subscription.Firstname;
				var lastname = subscription.Lastname;
				contact.LegalPersonContactFullName = StringUtils.Join (" ", firstname, lastname);

				contact.LegalPersonContactMrMrs = TextParser.ParsePersonMrMrs (subscription.Title);
			}

			address.AddressLine1 = subscription.FirstAddressLine;
			address.Street = subscription.StreetName;
			address.HouseNumber = subscription.HouseNumber;
			address.HouseNumberComplement = subscription.HouseNumberComplement;

			var zipCode = subscription.ZipCode;
			var townName = subscription.Town;
			var countryCode = subscription.CountryCode;
			address.Town = townRepository.GetTown (zipCode, townName, countryCode);

			ParishAssigner.AssignToParish (parishRepository, businessContext, legalPerson);

			return contact;
		}


		private static AiderSubscriptionEntity ImportLegalPersonSubscription
		(
			BusinessContext businessContext,
			SubscriptionData subscription,
			AiderContactEntity legalPersonContact
		)
		{
			var count = subscription.NbCopies ?? 1;

			var parish = legalPersonContact.LegalPerson.ParishGroup;
			var regionId = subscription.RegionalEdition;
			var region = SubscriptionDataImporter.GetRegion (businessContext, parish, regionId);

			return AiderSubscriptionEntity.Create (businessContext, legalPersonContact, region, count);
		}


		private static AiderGroupEntity GetRegion
		(
			BusinessContext businessContext,
			AiderGroupEntity parish,
			int? region
		)
		{
			// If the region is give, we pick that one.

			if (region.HasValue)
			{
				return ParishAssigner.FindRegionGroup (businessContext, region.Value);
			}

			// If we don't have a parish, we pick the default region, which is the one of Lausanne.

			if (parish.IsNull ())
			{
				return ParishAssigner.FindRegionGroup (businessContext, 4);
			}

			return parish.Parent;
		}


	}


}
