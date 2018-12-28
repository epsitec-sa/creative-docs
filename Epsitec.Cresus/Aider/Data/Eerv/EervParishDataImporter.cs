﻿//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Data.Normalization;
using Epsitec.Aider.Data.Subscription;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervParishDataImporter
	{


		public static void Import
		(
			CoreData coreData,
			ParishAddressRepository parishRepository,
			EervParishData eervParishData,
			bool considerDateOfBirth,
			bool considerSex
		)
		{
			var persons = EervParishDataImporter.ImportEervPhysicalPersons
			(
				coreData, parishRepository, eervParishData, considerDateOfBirth, considerSex
			);

			var legalPersons = EervParishDataImporter.ImportEervLegalPersons
			(
				coreData, parishRepository, eervParishData
			);

			var groups = EervParishDataImporter.ImportEervGroups
			(
				coreData, parishRepository, eervParishData
			);

			EervParishDataImporter.ImportEervActivities
			(
				coreData, eervParishData, persons, legalPersons, groups
			);

			coreData.ResetIndexes ();
		}


		private static Dictionary<EervPerson, EntityKey> ImportEervPhysicalPersons
		(
			CoreData coreData,
			ParishAddressRepository parishRepository,
			EervParishData eervParishData,
			bool considerDateOfBirth,
			bool considerSex
		)
		{
			var matches = EervParishDataImporter.FindMatches
			(
				coreData, eervParishData, considerDateOfBirth, considerSex
			);

			using (var businessContext = new BusinessContext (coreData, false))
			{
				var eervToAiderPersons = EervParishDataImporter.ProcessPersonMatches
				(
					businessContext, eervParishData.Id, matches
				);

				var newHouseholds = EervParishDataImporter.ProcessHouseholdMatches
				(
					businessContext, eervToAiderPersons, eervParishData.Households
				);

				EervParishDataImporter.AssignToParishes
				(
					businessContext, parishRepository, matches, eervToAiderPersons
				);

				SubscriptionGenerator.SubscribeHouseholds
				(
					businessContext, parishRepository, newHouseholds
				);

				EervParishDataImporter.AssignToImportationGroup
				(
					businessContext, parishRepository, eervParishData.Id, eervToAiderPersons.Values
				);

				businessContext.SaveChanges
				(
					LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors
				);

				return eervToAiderPersons.ToDictionary
				(
					i => i.Key,
					i => businessContext.DataContext.GetNormalizedEntityKey (i.Value).Value
				);
			}
		}


		private static Dictionary<EervPerson, Tuple<EntityKey, MatchData>> FindMatches
		(
			CoreData coreData,
			EervParishData eervParishData,
			bool considerDateOfBirth,
			bool considerSex
		)
		{
			var normalizedAiderPersons = Normalizer.Normalize (coreData);
			var normalizedEervPersons = Normalizer.Normalize (eervParishData.Households);

			var matches = NormalizedDataMatcher.FindMatches
			(
				normalizedEervPersons.Keys, normalizedAiderPersons.Keys, considerDateOfBirth,
				considerSex
			);

			return matches.ToDictionary
			(
				m => normalizedEervPersons[m.Item1],
				m => m.Item2 == null
					? null
					: Tuple.Create (normalizedAiderPersons[m.Item2.Item1], m.Item2.Item2)
			);
		}


		private static Dictionary<EervPerson, AiderPersonEntity> ProcessPersonMatches
		(
			BusinessContext businessContext,
			EervId eervId,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches
		)
		{
			var eervToAiderPersons = new Dictionary<EervPerson, AiderPersonEntity> ();

			eervToAiderPersons.AddRange
			(
				EervParishDataImporter.ProcessMatchWithValues (businessContext, eervId, matches)
			);

			eervToAiderPersons.AddRange
			(
				EervParishDataImporter.ProcessMatchWithoutValues (businessContext, eervId, matches)
			);

			return eervToAiderPersons;
		}


		private static Dictionary<EervPerson, AiderPersonEntity> ProcessMatchWithValues
		(
			BusinessContext businessContext,
			EervId eervId,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches
		)
		{
			var matchWithValues = matches
				.Where (m => m.Value != null)
				.ToList ();

			var entities = new Dictionary<EervPerson, AiderPersonEntity> ();

			foreach (var item in matchWithValues)
			{
				var eervPerson = item.Key;
				var match = item.Value;

				var aiderPerson = (AiderPersonEntity) businessContext.DataContext.ResolveEntity
				(
					match.Item1
				);

				var matchData = match.Item2;

				EervParishDataImporter.CombineAiderPersonWithEervPerson
				(
					businessContext, eervPerson, aiderPerson
				);

				EervParishDataImporter.AddMatchComment (eervPerson, aiderPerson, matchData, eervId);

				entities[eervPerson] = aiderPerson;
			}

			// We load the related data to speed up the business rule execution later on.
			AiderEnumerator.LoadRelatedData (businessContext.DataContext, entities.Values);
			businessContext.Register (entities.Values);

			return entities;
		}


		private static Dictionary<EervPerson, AiderPersonEntity> ProcessMatchWithoutValues
		(
			BusinessContext businessContext,
			EervId eervId,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches
		)
		{
			var matchWithoutValues = matches
				.Where (m => m.Value == null)
				.ToList ();

			var entities = new Dictionary<EervPerson, AiderPersonEntity> ();

			foreach (var item in matchWithoutValues)
			{
				var eervPerson = item.Key;

				var aiderPerson = EervParishDataImporter.CreateAiderPersonWithEervPerson
				(
					businessContext, eervPerson
				);

				EervParishDataImporter.CombineAiderPersonWithEervPerson
				(
					businessContext, eervPerson, aiderPerson
				);

				EervParishDataImporter.AddMatchComment (eervPerson, aiderPerson, null, eervId);

				entities[eervPerson] = aiderPerson;
			}

			return entities;
		}


		private static void CombineAiderPersonWithEervPerson(BusinessContext businessContext, EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			// TODO ADD PlaceOfBirth ?
			// TODO ADD PlaceOfBaptism ?
			// TODO ADD DateOfBaptism ?
			// TODO ADD PlaceOfChildBenediction ?
			// TODO ADD DateOfChildBenediction ?
			// TODO ADD PlaceOfCatechismBenediction ?
			// TODO ADD DateOfCatechismBenediction ?
			// TODO ADD SchoolYearOffset ?

			EervParishDataImporter.CombineOriginalName (eervPerson, aiderPerson);
			EervParishDataImporter.CombineHonorific (eervPerson, aiderPerson);
			EervParishDataImporter.CombineProfession (eervPerson, aiderPerson);
			EervParishDataImporter.CombineDateOfDeath (eervPerson, aiderPerson);
			EervParishDataImporter.CombineConfession (eervPerson, aiderPerson);
			EervParishDataImporter.CombineComments (aiderPerson, eervPerson.Remarks);
			EervParishDataImporter.CombineCoordinates (businessContext, eervPerson, aiderPerson);
		}


		private static void CombineOriginalName(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var originalName = eervPerson.OriginalName;

			if (!string.IsNullOrEmpty (originalName))
			{
				aiderPerson.OriginalName = originalName;
			}
		}


		private static void CombineHonorific(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var honorific = eervPerson.Honorific;

			if (!string.IsNullOrEmpty (honorific))
			{
				var title =	TextParser.ParsePersonMrMrs (honorific);

				if (title != PersonMrMrs.None)
				{
					aiderPerson.MrMrs = title;
				}
				else
				{
					aiderPerson.Title = honorific;
				}
			}
		}


		private static void CombineProfession(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var profession = eervPerson.Profession;

			if (!string.IsNullOrEmpty (profession))
			{
				aiderPerson.Profession = profession;
			}
		}


		private static void CombineDateOfDeath(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var dateOfDeath = eervPerson.DateOfDeath;

			if (dateOfDeath.HasValue)
			{
				aiderPerson.eCH_Person.PersonDateOfDeath = dateOfDeath;
			}
		}


		private static void CombineConfession(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var confession = eervPerson.Confession;

			if (confession != PersonConfession.Unknown)
			{
				aiderPerson.Confession = confession;
			}
		}


		private static void CombineCoordinates(BusinessContext businessContext, EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var cleanEmails  = EervParishDataImporter
				.GetCleanEmails (eervPerson.Coordinates.EmailAddress)
				.Where (e => !EervParishDataImporter.HasEmail (aiderPerson, e));

			var clearMobiles = EervParishDataImporter
				.GetCleanPhoneNumbers (eervPerson.Coordinates.MobilePhoneNumber)
				.Where (p => !EervParishDataImporter.HasPhone (aiderPerson, p));

			//	Produce a stream of email/phone pairs until both source collections are empty.

			var tuples = cleanEmails.CombineToTuples (clearMobiles);

			foreach (var tuple in tuples)
			{
				var email  = tuple.Item1;
				var mobile = tuple.Item2;

				var hasEmail  = !string.IsNullOrWhiteSpace (email);
				var hasMobile = !string.IsNullOrWhiteSpace (mobile);

				if (hasEmail || hasMobile)
				{
					var contact = AiderContactEntity.Create (businessContext, aiderPerson, AddressType.Other);
					var address = contact.GetAddress ();

					EervParishDataImporter.SetEmail (address, email);
					EervParishDataImporter.SetPhoneNumber (address, mobile, (a, s) => a.Mobile = s);
				}
			}
		}


		private static bool HasEmail(AiderPersonEntity person, string email)
		{
			return person.AdditionalAddresses.Any (c => c.Address.Email == email);
		}


		private static bool HasPhone(AiderPersonEntity person, string phone)
		{
			var parsedPhone = TwixClip.TwixTel.ParsePhoneNumber (phone);

			if (!TwixClip.TwixTel.IsValidPhoneNumber (parsedPhone, false))
			{
				return false;
			}

			return person.AdditionalAddresses.Any (c =>
			{
				var address = c.Address;

				return address.Mobile == parsedPhone
					|| address.Phone1 == parsedPhone
					|| address.Phone2 == parsedPhone;
			});
		}


		private static IEnumerable<string> GetCleanEmails(string rawEmail)
		{
			if (string.IsNullOrEmpty (rawEmail))
			{
				yield break;
			}

			foreach (var email in rawEmail.Split (' ', ';', ':', '/').Select (x => x.Trim ()))
			{
				var fixedEmail = Epsitec.Common.IO.UriBuilder.FixScheme (email);

				if (Epsitec.Common.IO.UriBuilder.IsValidMailTo (fixedEmail))
				{
					yield return email;
				}
			}
		}

		private static IEnumerable<string> GetCleanPhoneNumbers(string rawPhoneNumber)
		{
			if (string.IsNullOrEmpty (rawPhoneNumber))
			{
				yield break;
			}

			if ((rawPhoneNumber.StartsWith ("+")) ||
				(rawPhoneNumber.StartsWith ("00")))
			{
				yield return rawPhoneNumber;
				yield break;
			}

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (char c in rawPhoneNumber)
			{
				if ((c >= '0') &&
					(c <= '9'))
				{
					buffer.Append (c);
				}

				if (buffer.Length >= 10)
				{
					var number = buffer.ToString ();

					if (TwixClip.TwixTel.IsValidPhoneNumber (number, acceptEmptyNumbers: false))
					{
						yield return number;

						buffer.Clear ();
					}
				}
			}
		}


		private static void SetEmail(AiderAddressEntity address, string email)
		{
			if (!string.IsNullOrEmpty (email))
			{
				address.Email = email;
			}
		}


		private static void SetPhoneNumber(AiderAddressEntity address, string phoneNumber, Action<AiderAddressEntity, string> setter)
		{
			if (!string.IsNullOrEmpty (phoneNumber))
			{
				var parsedNumber = TwixClip.TwixTel.ParsePhoneNumber (phoneNumber);

				if (TwixClip.TwixTel.IsValidPhoneNumber (parsedNumber, false))
				{
					setter (address, parsedNumber);
				}
				else
				{
					var text = "Téléphone invalide ou non reconnu par le système : " + phoneNumber;

					EervParishDataImporter.CombineSystemComments (address, text);
				}
			}
		}


		private static void CombineComments(IComment comment, string text)
		{
			AiderCommentEntity.CombineComments (comment, text);
		}

		private static void CombineSystemComments(IComment comment, string text)
		{
			AiderCommentEntity.CombineSystemComments (comment, text);
		}



		private static AiderPersonEntity CreateAiderPersonWithEervPerson(BusinessContext businessContext, EervPerson eervPerson)
		{
			var aiderPerson = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
			var eChPerson = aiderPerson.eCH_Person;

			eChPerson.PersonFirstNames = eervPerson.Firstname;
			eChPerson.PersonOfficialName = eervPerson.Lastname;
			eChPerson.PersonDateOfBirth = eervPerson.DateOfBirth;
			eChPerson.PersonSex = eervPerson.Sex;
			eChPerson.AdultMaritalStatus = eervPerson.MaritalStatus;

			var origins = eervPerson.Origins;

			if (!string.IsNullOrEmpty (origins))
			{
				eChPerson.Origins = origins;
			}

			eChPerson.DataSource = Enumerations.DataSource.Undefined;
			eChPerson.DeclarationStatus = PersonDeclarationStatus.NotDeclared;
			eChPerson.RemovalReason = RemovalReason.None;

			return aiderPerson;
		}


		private static void AddMatchComment(EervPerson eervPerson, AiderPersonEntity aiderPerson, MatchData match, EervId eervId)
		{
			string text;

			if (match != null)
			{
				text = "Correspondance avec la personne " + eervPerson.Id + " du " + eervId.GetFileName () + ":";
				text += "\n- Nom: " + EervParishDataImporter.GetTextForJaroWinklerMatch (match.Lastname);
				text += "\n- Prénom: " + EervParishDataImporter.GetTextForJaroWinklerMatch (match.Firstname);
				text += "\n- Date de naissance: " + EervParishDataImporter.GetTextForJaroWinklerMatch (match.DateOfBirth);
				text += "\n- Sexe: " + EervParishDataImporter.GetTextForSexMatch (match.Sex);
				text += "\n- Adresse: " + EervParishDataImporter.GetTextForAddressMatch (match.Address);
			}
			else
			{
				text = "Personne créée à partir de la personne " + eervPerson.Id + " du " + eervId.GetFileName () + ", sans correspondance dans le fichier ECH.";
			}

			EervParishDataImporter.CombineSystemComments (aiderPerson, text);
		}


		private static string GetTextForAddressMatch(AddressMatch match)
		{
			switch (match)
			{
				case AddressMatch.Full:
					return "complête";

				case AddressMatch.None:
					return "non";

				case AddressMatch.ZipCity:
				case AddressMatch.StreetZipCity:
					return "partielle";

				default:
					throw new NotImplementedException ();
			}
		}


		private static string GetTextForSexMatch(bool? match)
		{
			switch (match)
			{
				case true:
					return "oui";

				case false:
					return "non";

				case null:
					return "N/A";

				default:
					throw new NotImplementedException ();
			}
		}


		private static string GetTextForJaroWinklerMatch(double? match)
		{
			if (match.HasValue)
			{
				var maxValue = 10;
				var grade = match.Value * maxValue;

				return string.Format ("{0:0.#}/{1}", grade, maxValue);
			}
			else
			{
				return "N/A";
			}
		}


		private static List<AiderHouseholdEntity> ProcessHouseholdMatches
		(
			BusinessContext businessContext,
			Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons,
			IEnumerable<EervHousehold> eervHouseholds
		)
		{
			var aiderTowns = new AiderTownRepository (businessContext);

			// That's all the persons that are in the parish file.
			var fileAiderPersons = eervToAiderPersons.Values.Distinct ().ToList ();

			// That's all the households related to the persons in the parish file.
			var aiderHouseholds = EervParishDataImporter.GetAiderHouseholds (fileAiderPersons);

			// That's all the member of these households (in the database), which might include
			// persons that are not in the parish file and might not include persons that are in the
			// parish file.
			var otherAiderPersons = EervParishDataImporter.GetAiderPersons (aiderHouseholds);

			AiderEnumerator.LoadRelatedData (businessContext.DataContext, otherAiderPersons);
			businessContext.Register (otherAiderPersons);

			var newHouseholds = new List<AiderHouseholdEntity> ();

			foreach (var eervHousehold in eervHouseholds)
			{
				var newHousehold = EervParishDataImporter.ProcessHouseholdMatch
				(
					businessContext,
					eervToAiderPersons,
					eervHousehold,
					aiderTowns
				);

				if (newHousehold != null)
				{
					newHouseholds.Add (newHousehold);
				}
			}

			return newHouseholds;
		}


		private static List<AiderHouseholdEntity> GetAiderHouseholds
		(
			IEnumerable<AiderPersonEntity> aiderPersons
		)
		{
			return aiderPersons
				.SelectMany (p => p.Households)
				.Distinct ()
				.ToList ();
		}


		private static List<AiderPersonEntity> GetAiderPersons
		(
			IEnumerable<AiderHouseholdEntity> aiderHouseholds
		)
		{
			return aiderHouseholds
				.SelectMany (p => p.Members)
				.Distinct ()
				.ToList ();
		}


		private static AiderHouseholdEntity ProcessHouseholdMatch
		(
			BusinessContext businessContext,
			Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons,
			EervHousehold eervHousehold,
			AiderTownRepository aiderTowns
		)
		{
			var aiderHouseholds = EervParishDataImporter.GetAiderHouseholds
			(
				eervHousehold, eervToAiderPersons
			);

			if (aiderHouseholds.Count == 0)
			{
				return EervParishDataImporter.CreateHousehold
				(
					businessContext,
					eervHousehold,
					eervToAiderPersons,
					aiderTowns
				);
			}
			else if (aiderHouseholds.Count == 1)
			{
				EervParishDataImporter.ExpandHousehold
				(
					businessContext,
					eervHousehold,
					eervToAiderPersons,
					aiderHouseholds.Single ()
				);
			}
			else
			{
				EervParishDataImporter.CombineHouseholds
				(
					businessContext,
					eervHousehold,
					eervToAiderPersons,
					aiderHouseholds
				);
			}

			return null;
		}


		private static List<AiderHouseholdEntity> GetAiderHouseholds
		(
			EervHousehold eervHousehold,
			Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons
		)
		{
			var aiderHouseholdMembers = eervHousehold
				.Members
				.Select (m => eervToAiderPersons[m])
				.Distinct ();

			return EervParishDataImporter.GetAiderHouseholds (aiderHouseholdMembers);
		}


		private static AiderHouseholdEntity CreateHousehold
		(
			BusinessContext businessContext,
			EervHousehold eervHousehold,
			Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons,
			AiderTownRepository aiderTowns
		)
		{
			var aiderHousehold = businessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();

			EervParishDataImporter.ExpandHousehold
			(
				businessContext,
				eervHousehold,
				eervToAiderPersons,
				aiderHousehold
			);

			EervParishDataImporter.CombineAddress
			(
				aiderTowns,
				aiderHousehold.Address,
				eervHousehold.Address
			);

			return aiderHousehold;
		}


		private static void ExpandHousehold
		(
			BusinessContext businessContext,
			EervHousehold eervHousehold,
			Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons,
			AiderHouseholdEntity aiderHousehold
		)
		{
			EervParishDataImporter.CombineMembers
			(
				businessContext,
				eervHousehold,
				eervToAiderPersons,
				aiderHousehold
			);

			EervParishDataImporter.CombineComments (aiderHousehold, eervHousehold.Remarks);

			EervParishDataImporter.CombineCoordinates
			(
				aiderHousehold.Address,
				eervHousehold.Coordinates
			);
		}


		private static void CombineMembers
		(
			BusinessContext businessContext,
			EervHousehold eervHousehold,
			Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons,
			AiderHouseholdEntity aiderHousehold
		)
		{
			var currentMembers = aiderHousehold.Members;

			var newMembers = eervHousehold.Members
				.Select (p => Tuple.Create (p, eervToAiderPersons[p]))
				.Where (p => !currentMembers.Contains (p.Item2));

			foreach (var newMember in newMembers)
			{
				var eervPerson = newMember.Item1;
				var aiderPerson = newMember.Item2;

				var isHead = eervHousehold.Heads.Contains (eervPerson);

				AiderContactEntity.Create
				(
					businessContext,
					aiderPerson,
					aiderHousehold,
					isHead
				);
			}
		}


		private static void CombineHouseholds
		(
			BusinessContext businessContext,
			EervHousehold eervHousehold,
			Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons,
			List<AiderHouseholdEntity> aiderHouseholds
		)
		{
			var effectiveHouseholds = aiderHouseholds
				.GroupBy (h => h.Address, AiderAddressEntityComparer.Instance);

			var mainHouseholds = new List<AiderHouseholdEntity> ();

			foreach (var effectiveHousehold in effectiveHouseholds)
			{
				var households = effectiveHousehold.ToList ();

				var mainEffectiveHousehold = EervParishDataImporter.CombineEffectiveHouseholds
				(
					businessContext,
					households
				);

				mainHouseholds.Add (mainEffectiveHousehold);
			}

			var mainHousehold = EervParishDataImporter.GetMainHousehold (mainHouseholds);

			EervParishDataImporter.ExpandHousehold
			(
				businessContext,
				eervHousehold,
				eervToAiderPersons,
				mainHousehold
			);
		}


		private static AiderHouseholdEntity CombineEffectiveHouseholds
		(
			BusinessContext businessContext,
			List<AiderHouseholdEntity> aiderHouseholds
		)
		{
			var mainHousehold = EervParishDataImporter.GetMainHousehold (aiderHouseholds);

			var secondaryHouseholds = aiderHouseholds.Where (h => h != mainHousehold);

			foreach (var secondaryHousehold in secondaryHouseholds)
			{
				foreach (var contact in secondaryHousehold.Contacts)
				{
					AiderContactEntity.Create (businessContext, contact.Person, mainHousehold, false);
				}

				EervParishDataImporter.CombineSubscriptionsAndRefusals
				(
					businessContext, mainHousehold, secondaryHousehold
				);

				AiderHouseholdEntity.Delete (businessContext, secondaryHousehold);
			}

			return mainHousehold;
		}


		private static void CombineSubscriptionsAndRefusals
		(
			BusinessContext businessContext,
			AiderHouseholdEntity mainHousehold,
			AiderHouseholdEntity secondaryHousehold
		)
		{
			var mainSubscription = AiderSubscriptionEntity.FindSubscription
			(
				businessContext, mainHousehold
			);

			var mainRefusal = AiderSubscriptionRefusalEntity.FindRefusal
			(
				businessContext, mainHousehold
			);

			if (mainSubscription.IsNull () && mainRefusal.IsNull ())
			{
				var secondarySubscription = AiderSubscriptionEntity.FindSubscription
				(
					businessContext, secondaryHousehold
				);

				var secondaryRefusal = AiderSubscriptionRefusalEntity.FindRefusal
				(
					businessContext, secondaryHousehold
				);

				if (secondarySubscription.IsNotNull ())
				{
					var edition = secondarySubscription.RegionalEdition;
					var count = secondarySubscription.Count;

					AiderSubscriptionEntity.Create (businessContext, mainHousehold, edition, count);
				}
				else if (secondaryRefusal.IsNotNull ())
				{
					AiderSubscriptionRefusalEntity.Create (businessContext, mainHousehold);
				}
			}
		}


		private static AiderHouseholdEntity GetMainHousehold
		(
			List<AiderHouseholdEntity> aiderHouseholds
		)
		{
			return aiderHouseholds
				.OrderByDescending (h => h.Members.Count)
				.First ();
		}


		private static void CombineAddress(AiderTownRepository aiderTowns, AiderAddressEntity aiderAddress, EervAddress eervAddress)
		{
			if (string.IsNullOrEmpty (eervAddress.ZipCode) || string.IsNullOrEmpty (eervAddress.Town))
			{
				// We have an invalid address or no address at all, thus we ignore it.

				return;
			}

			aiderAddress.AddressLine1 = eervAddress.FirstAddressLine;
			aiderAddress.Street = eervAddress.StreetName;
			aiderAddress.HouseNumber = eervAddress.HouseNumber;
			aiderAddress.HouseNumberComplement = eervAddress.HouseNumberComplement;

			var zipCode = eervAddress.ZipCode;
			var townName = eervAddress.Town;
			var countryCode = eervAddress.CountryCode;
			aiderAddress.Town = aiderTowns.GetTown (zipCode, townName, countryCode);
		}


		private static void CombineCoordinates(AiderAddressEntity address, EervCoordinates coordinates)
		{
			var privatePhoneNumber = coordinates.PrivatePhoneNumber;

			if (!string.IsNullOrWhiteSpace (privatePhoneNumber))
			{
				if (string.IsNullOrWhiteSpace (address.Phone1))
				{
					EervParishDataImporter.SetPhoneNumber (address, privatePhoneNumber, (a, s) => a.Phone1 =s);
				}
				else if (string.IsNullOrWhiteSpace (address.Phone2))
				{
					EervParishDataImporter.SetPhoneNumber (address, privatePhoneNumber, (a, s) => a.Phone2 =s);
				}
				else
				{
					var text = "Tél. supplémentaire: " + privatePhoneNumber;

					EervParishDataImporter.CombineComments (address, text);
				}
			}

			var professionalPhoneNumber = coordinates.ProfessionalPhoneNumber;

			if (!string.IsNullOrWhiteSpace (professionalPhoneNumber))
			{
				var text = "Tél. professionel: " + professionalPhoneNumber;

				EervParishDataImporter.CombineComments (address, text);
			}

			var faxNumber = coordinates.FaxNumber;

			if (!string.IsNullOrWhiteSpace (faxNumber))
			{
				if (string.IsNullOrWhiteSpace (address.Fax))
				{
					EervParishDataImporter.SetPhoneNumber (address, faxNumber, (a, s) => a.Fax = s);
				}
				else
				{
					var text = "Fax supplémentaire: " + faxNumber;

					EervParishDataImporter.CombineComments (address, text);
				}
			}

			var mobilePhoneNumber = coordinates.MobilePhoneNumber;

			if (!string.IsNullOrWhiteSpace (mobilePhoneNumber))
			{
				if (string.IsNullOrWhiteSpace (address.Mobile))
				{
					EervParishDataImporter.SetPhoneNumber (address, mobilePhoneNumber, (a, s) => a.Mobile = s);
				}
				else
				{
					var text = "Mobile supplémentaire: " + mobilePhoneNumber;

					EervParishDataImporter.CombineComments (address, text);
				}
			}

			var emailAddress = coordinates.EmailAddress;

			if (!string.IsNullOrWhiteSpace (emailAddress))
			{
				if (string.IsNullOrWhiteSpace (address.Email))
				{
					address.Email = emailAddress;
				}
				else
				{
					var text = "E-mail supplémentaire: " + mobilePhoneNumber;

					EervParishDataImporter.CombineComments (address, text);
				}
			}
		}


		private static Dictionary<EervLegalPerson, EntityKey> ImportEervLegalPersons(CoreData coreData, ParishAddressRepository parishRepository, EervParishData eervParishData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				return EervParishDataImporter.ImportEervLegalPersons (businessContext, parishRepository, eervParishData);
			}
		}


		private static Dictionary<EervLegalPerson, EntityKey> ImportEervLegalPersons(BusinessContext businessContext, ParishAddressRepository parishRepository, EervParishData eervParishData)
		{
			var aiderLegalPersons = EervParishDataImporter.ImportEervLegalPersons (businessContext, eervParishData, eervParishData.Id);

			ParishAssigner.AssignToParish (parishRepository, businessContext, aiderLegalPersons.Values);

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			return aiderLegalPersons.ToDictionary
			(
				lp => lp.Key,
				lp => businessContext.DataContext.GetNormalizedEntityKey (lp.Value).Value
			);
		}


		private static Dictionary<EervLegalPerson, AiderLegalPersonEntity> ImportEervLegalPersons(BusinessContext businessContext, EervParishData eervParishData, EervId eervId)
		{
			var aiderTowns = new AiderTownRepository (businessContext);

			return eervParishData
				.LegalPersons
				.ToDictionary
				(
					lp => lp,
					lp => EervParishDataImporter.ImportEervLegalPerson (businessContext, aiderTowns, eervId, lp)
				);
		}


		private static AiderLegalPersonEntity ImportEervLegalPerson(BusinessContext businessContext, AiderTownRepository aiderTowns, EervId eervId, EervLegalPerson legalPerson)
		{
			var aiderLegalPerson = businessContext.CreateAndRegisterEntity<AiderLegalPersonEntity> ();
			aiderLegalPerson.Name = EervParishDataImporter.GetCorporateName (legalPerson);

			var aiderAddress = aiderLegalPerson.Address;
			EervParishDataImporter.CombineAddress (aiderTowns, aiderAddress, legalPerson.Address);
			EervParishDataImporter.CombineCoordinates (aiderAddress, legalPerson.Coordinates);
			EervParishDataImporter.CombineCoordinates (aiderAddress, legalPerson.ContactPerson.Coordinates);

			EervParishDataImporter.ImportEervLegalPersonContact (businessContext, aiderLegalPerson, legalPerson);

			var comment = "Ce contact a été crée à partir du contact N°" + legalPerson.Id + " du " + eervId.GetFileName () + ".";
			EervParishDataImporter.CombineSystemComments (aiderLegalPerson, comment);

			return aiderLegalPerson;
		}


		private static AiderContactEntity ImportEervLegalPersonContact(BusinessContext businessContext, AiderLegalPersonEntity aiderLegalperson, EervLegalPerson eervLegalPerson)
		{
			var aiderContact = AiderContactEntity.Create (businessContext, aiderLegalperson);
			var eervContact = eervLegalPerson.ContactPerson;

			// NOTE If the corporate name is empty, we used the first or last name as corporate
			// name. Therefore, we assume that there is no contact person there.

			if (!string.IsNullOrWhiteSpace (eervLegalPerson.Name))
			{
				aiderContact.LegalPersonContactFullName =  EervParishDataImporter.GetContactFullName (eervContact);
				aiderContact.LegalPersonContactMrMrs = TextParser.ParsePersonMrMrs (eervContact.Honorific);
			}

			return aiderContact;
		}


		private static string GetCorporateName(EervLegalPerson legalPerson)
		{
			// NOTE It happens often that the corporate name is empty but that the firstname or the
			// lastname is filled with the value that should have been in the corporate name field.

			if (!string.IsNullOrWhiteSpace (legalPerson.Name))
			{
				return legalPerson.Name;
			}
			else if (!string.IsNullOrWhiteSpace (legalPerson.ContactPerson.Lastname))
			{
				return legalPerson.ContactPerson.Lastname;
			}
			else if (!string.IsNullOrWhiteSpace (legalPerson.ContactPerson.Firstname))
			{
				return legalPerson.ContactPerson.Firstname;
			}
			else
			{
				throw new NotImplementedException ();
			}
		}


		private static string GetContactFullName(EervPerson contactPerson)
		{
			var emptyFirstName = string.IsNullOrWhiteSpace (contactPerson.Firstname);
			var emptyLastName = string.IsNullOrWhiteSpace (contactPerson.Lastname);

			if (emptyFirstName && emptyLastName)
			{
				return "";
			}
			else if (!emptyFirstName && emptyLastName)
			{
				return contactPerson.Firstname;
			}
			else if (emptyFirstName && !emptyLastName)
			{
				return contactPerson.Lastname;
			}
			else
			{
				return contactPerson.Firstname + " " + contactPerson.Lastname;
			}
		}


		private static void AssignToParishes
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches,
			Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons
		)
		{
			var newEntities = matches
				.Where (m => m.Value == null)
				.Select (m => eervToAiderPersons[m.Key])
				.ToList ();

			ParishAssigner.AssignToParish (parishRepository, businessContext, newEntities);
		}


		private static void AssignToImportationGroup
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository, 
			EervId parishId,
			IEnumerable<AiderPersonEntity> aiderPersons
		)
		{
			// TODO Also assign to some importation group in the other cases ?
			if (parishId.Kind == EervKind.Parish)
			{
				var importationGroup = EervParishDataImporter.FindImportationGroup
				(
					businessContext, parishRepository, parishId
				);

				foreach (var aiderPerson in aiderPersons)
				{
					var participationData = new ParticipationData (aiderPerson);

					AiderGroupParticipantEntity.StartParticipation
					(
						businessContext, importationGroup, participationData
					);
				}
			}
		}


		private static AiderGroupEntity FindImportationGroup(BusinessContext businessContext, ParishAddressRepository parishRepository, EervId parishId)
		{
			var parishGroup = EervParishDataImporter
				.FindRootAiderGroups (businessContext, parishRepository, parishId)
				.Single ();

			return parishGroup.Subgroups.Single (g => g.Name == "Personnes importées");
		}


		private static Dictionary<EervGroup, EntityKey> ImportEervGroups(CoreData coreData, ParishAddressRepository parishRepository, EervParishData eervParishData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				return EervParishDataImporter.ImportEervGroups (businessContext, parishRepository, eervParishData);
			}
		}


		private static Dictionary<EervGroup, EntityKey> ImportEervGroups(BusinessContext businessContext, ParishAddressRepository parishRepository, EervParishData eervParishData)
		{
			var eervGroups = eervParishData.Groups;
			var eervId = eervParishData.Id;

			var idToGroups = EervParishDataImporter.BuildIdToGroups (businessContext, parishRepository, eervId);
			var eervToAiderGroups = new Dictionary<EervGroup, AiderGroupEntity> ();

			// We sort the groups so that they appear in the right order, that is, the parent before
			// their children.
			foreach (var eervGroup in eervGroups.OrderBy (g => g.Id))
			{
				EervParishDataImporter.CheckGroupId (eervId, eervGroup);

				AiderGroupEntity aiderGroup;

				if (idToGroups.TryGetValue (eervGroup.Id, out aiderGroup))
				{
					eervToAiderGroups[eervGroup] = aiderGroup;
				}
				else if (idToGroups.TryGetValue (EervGroupDefinition.GetParentId (eervGroup.Id), out aiderGroup))
				{
					// HACK This is a hack for group names greater than 200 chars that will throw an
					// exception later. This need to be corrected.
					var name = eervGroup.Name.Substring (0, Math.Min (200, eervGroup.Name.Length));
					var newAiderGroup = aiderGroup.CreateSubgroup (businessContext, name);

					idToGroups[eervGroup.Id] = newAiderGroup;
					eervToAiderGroups[eervGroup] = newAiderGroup;
				}
				else
				{
					Debug.WriteLine ("WARNING: group " + eervGroup.Id + " has no parent defined.");
				}
			}

			businessContext.Register (idToGroups.Values);
			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			return eervToAiderGroups.ToDictionary
			(
				g => g.Key,
				g => businessContext.DataContext.GetNormalizedEntityKey (g.Value).Value
			);
		}


		private static void CheckGroupId(EervId eervId, EervGroup eervGroup)
		{
			switch (eervId.Kind)
			{
				case EervKind.Canton:
					EervParishDataImporter.CheckCantonGroupId (eervGroup);
					break;

				case EervKind.Region:
					EervParishDataImporter.CheckRegionGroupId (eervGroup);
					break;

				case EervKind.Parish:
					EervParishDataImporter.CheckParishGroupId (eervGroup);
					break;

				default:
					throw new NotImplementedException ();
			}
		}


		private static void CheckCantonGroupId(EervGroup eervGroup)
		{
			if (eervGroup.Id.StartsWith ("03") || eervGroup.Id.StartsWith ("04"))
			{
				throw new Exception ("Invalid group id!");
			}
		}


		private static void CheckRegionGroupId(EervGroup eervGroup)
		{
			if (!eervGroup.Id.StartsWith ("03"))
			{
				throw new Exception ("Invalid group id!");
			}
		}


		private static void CheckParishGroupId(EervGroup eervGroup)
		{
			if (!eervGroup.Id.StartsWith ("04"))
			{
				throw new Exception ("Invalid group id!");
			}
		}


		private static Dictionary<string, AiderGroupEntity> BuildIdToGroups(BusinessContext businessContext, ParishAddressRepository parishRepository, EervId eervId)
		{
			var mapping = new Dictionary<string, AiderGroupEntity> ();

			var rootGroups = EervParishDataImporter.FindRootAiderGroups (businessContext, parishRepository, eervId);

			var todo = new Stack<AiderGroupEntity> ();
			todo.PushRange (rootGroups);

			while (todo.Count > 0)
			{
				var group = todo.Pop ();
				var definition = group.GroupDef;

				if (definition.IsNull ())
				{
					continue;
				}

				if (eervId.Kind != EervKind.Parish && definition.Classification == GroupClassification.Parish)
				{
					continue;
				}

				mapping[definition.Number] = group;

				todo.PushRange (group.Subgroups);
			}

			return mapping;
		}


		private static IEnumerable<AiderGroupEntity> FindRootAiderGroups(BusinessContext businessContext, ParishAddressRepository parishRepository, EervId eervId)
		{
			switch (eervId.Kind)
			{
				case EervKind.Canton:
					return FindRootCantonGroups (businessContext);

				case EervKind.Region:
					return FindRootRegionGroup (businessContext, eervId);

				case EervKind.Parish:
					return FindRootParishGroup (businessContext, parishRepository, eervId);

				default:
					throw new NotImplementedException ();
			}
		}


		private static IEnumerable<AiderGroupEntity> FindRootCantonGroups(BusinessContext businessContext)
		{
			return AiderGroupEntity
				.FindRootGroups (businessContext)
				.Where (g => g.GroupDef.Classification != GroupClassification.Region)
				.ToList ();
		}


		private static IEnumerable<AiderGroupEntity> FindRootRegionGroup(BusinessContext businessContext, EervId eervId)
		{
			yield return ParishAssigner.FindRegionGroup (businessContext, eervId.GetRegionCode ());
		}


		private static IEnumerable<AiderGroupEntity> FindRootParishGroup(BusinessContext businessContext, ParishAddressRepository parishRepository, EervId eervId)
		{
			yield return ParishAssigner.FindParishGroup (businessContext, parishRepository, eervId.Name);
		}


		private static void ImportEervActivities(CoreData coreData, EervParishData eervParishData, Dictionary<EervPerson, EntityKey> eervPersonToKeys, Dictionary<EervLegalPerson, EntityKey> eervLegalPersonToKeys, Dictionary<EervGroup, EntityKey> eervGroupToKeys)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				EervParishDataImporter.ImportEervActivities (businessContext, eervParishData, eervPersonToKeys, eervLegalPersonToKeys, eervGroupToKeys);
			}
		}


		private static void ImportEervActivities(BusinessContext businessContext, EervParishData eervParishData, Dictionary<EervPerson, EntityKey> eervPersonToKeys, Dictionary<EervLegalPerson, EntityKey> eervLegalPersonToKeys, Dictionary<EervGroup, EntityKey> eervGroupToKeys)
		{
			var dataContext = businessContext.DataContext;

			var aiderPersons = new HashSet<AiderPersonEntity> ();
			var aiderLegalPersons = new HashSet<AiderLegalPersonEntity> ();

			foreach (var eervActivity in eervParishData.Activities)
			{
				var aiderGroupKey = eervGroupToKeys[eervActivity.Group];
				var aiderGroup = (AiderGroupEntity) dataContext.ResolveEntity (aiderGroupKey);

				var startDate = eervActivity.StartDate;
				var endDate = eervActivity.EndDate;
				var remarks = TextFormatter.FormatText (eervActivity.Remarks);

				ParticipationData participationData;

				if (eervActivity.Person != null)
				{
					var aiderPersonKey = eervPersonToKeys[eervActivity.Person];
					var aiderPerson = (AiderPersonEntity) dataContext.ResolveEntity (aiderPersonKey);

					aiderPersons.Add (aiderPerson);
					participationData = new ParticipationData (aiderPerson);
				}
				else if (eervActivity.LegalPerson != null)
				{
					var aiderLegalpersonKey = eervLegalPersonToKeys[eervActivity.LegalPerson];
					var aiderLegalperson = (AiderLegalPersonEntity) dataContext.ResolveEntity (aiderLegalpersonKey);

					participationData = new ParticipationData (aiderLegalperson);
				}
				else
				{
					throw new NotImplementedException ();
				}

				AiderGroupParticipantEntity.ImportParticipation (businessContext, aiderGroup, participationData, startDate, endDate, remarks);
			}

			// We load the related data to speed up the execution of the business rules.
			AiderEnumerator.LoadRelatedData (dataContext, aiderPersons);
			AiderEnumerator.LoadRelatedData (dataContext, aiderLegalPersons);

			businessContext.Register (aiderPersons);
			businessContext.Register (aiderLegalPersons);

			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
		}


	}


}
