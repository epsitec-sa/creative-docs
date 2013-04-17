//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data.Normalization;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.TwixClip;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervParishDataImporter
	{


		public static void Import(CoreData coreData, ParishAddressRepository parishRepository, EervParishData eervParishData)
		{
			var persons = EervParishDataImporter.ImportEervPhysicalPersons (coreData, parishRepository, eervParishData);
			var legalPersons = EervParishDataImporter.ImportEervLegalPersons (coreData, eervParishData);
			var groups = EervParishDataImporter.ImportEervGroups (coreData, eervParishData);

			EervParishDataImporter.ImportEervActivities (coreData, eervParishData, persons, legalPersons, groups);

			coreData.ResetIndexes ();
		}


		private static Dictionary<EervPerson, EntityKey> ImportEervPhysicalPersons(CoreData coreData, ParishAddressRepository parishRepository, EervParishData eervParishData)
		{
			var matches = EervParishDataImporter.FindMatches (coreData, eervParishData);
			var newEntities = EervParishDataImporter.ProcessMatches (coreData, eervParishData.Id, matches);
			var mapping = EervParishDataImporter.BuildEervPersonMapping (matches, newEntities);

			EervParishDataImporter.ProcessHouseholdMatches (coreData, matches, newEntities, eervParishData.Households);
			EervParishDataImporter.AssignToParishes (coreData, parishRepository, newEntities.Values);

			// TODO Also assign to some importation group in the other cases ?
			if (eervParishData.Id.Kind == EervKind.Parish)
			{
				EervParishDataImporter.AssignToImportationGroup (coreData, eervParishData.Id, mapping.Values);
			}

			return mapping;
		}


		private static Dictionary<EervPerson, EntityKey> BuildEervPersonMapping(Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matchData, Dictionary<EervPerson, EntityKey> newEntities)
		{
			var mapping = new Dictionary<EervPerson, EntityKey> ();

			foreach (var match in matchData)
			{
				if (match.Value != null)
				{
					mapping[match.Key] = match.Value.Item1;
				}
			}

			foreach (var newEntity in newEntities)
			{
				mapping[newEntity.Key] = newEntity.Value;
			}

			return mapping;
		}


		private static Dictionary<EervPerson, Tuple<EntityKey, MatchData>> FindMatches(CoreData coreData, EervParishData eervParishData)
		{
			var normalizedAiderPersons = Normalizer.Normalize (coreData);
			var normalizedEervPersons = Normalizer.Normalize (eervParishData.Households);

			var matches = NormalizedDataMatcher.FindMatches (normalizedEervPersons.Keys, normalizedAiderPersons.Keys);

			return matches.ToDictionary
			(
				m => normalizedEervPersons[m.Item1],
				m => m.Item2 == null
					? null
					: Tuple.Create (normalizedAiderPersons[m.Item2.Item1], m.Item2.Item2)
			);
		}


		private static Dictionary<EervPerson, EntityKey> ProcessMatches(CoreData coreData, EervId eervId, Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				EervParishDataImporter.ProcessMatchWithValues (businessContext, eervId, matches);
			}

			using (var businessContext = new BusinessContext (coreData, false))
			{
				return EervParishDataImporter.ProcessMatchWithoutValues (businessContext, eervId, matches);
			}
		}


		private static void ProcessMatchWithValues(BusinessContext businessContext, EervId eervId, Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches)
		{
			var matchWithValues = matches
				.Where (m => m.Value != null)
				.ToList ();

			var existingEntities = new List<AiderPersonEntity> ();

			foreach (var item in matchWithValues)
			{
				var eervPerson = item.Key;
				var match = item.Value;

				var aiderPerson = (AiderPersonEntity) businessContext.DataContext.ResolveEntity (match.Item1);
				var matchData = match.Item2;

				EervParishDataImporter.CombineAiderPersonWithEervPerson (businessContext, eervPerson, aiderPerson);
				EervParishDataImporter.AddMatchComment (eervPerson, aiderPerson, matchData, eervId);

				existingEntities.Add (aiderPerson);
			}

			// We load the related data to speed up the business rule execution later on.
			AiderEnumerator.LoadRelatedData (businessContext.DataContext, existingEntities);
			businessContext.Register (existingEntities);

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static Dictionary<EervPerson, EntityKey> ProcessMatchWithoutValues(BusinessContext businessContext, EervId eervId, Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches)
		{
			var matchWithoutValues = matches
				.Where (m => m.Value == null)
				.ToList ();

			var newEntities = new Dictionary<EervPerson, AiderPersonEntity> ();

			foreach (var item in matchWithoutValues)
			{
				var eervPerson = item.Key;

				var aiderPerson = EervParishDataImporter.CreateAiderPersonWithEervPerson (businessContext, eervPerson);

				EervParishDataImporter.CombineAiderPersonWithEervPerson (businessContext, eervPerson, aiderPerson);
				EervParishDataImporter.AddMatchComment (eervPerson, aiderPerson, null, eervId);

				newEntities[eervPerson] = aiderPerson;
			}

			// We assign the new persons to the no parish groups, so that the business rules won't
			// mess with the parish assignation. Later on we will assign these persons to their
			// real parish.
			ParishAssigner.AssignToNoParishGroup (businessContext, newEntities.Values);

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			return newEntities.ToDictionary
			(
				kvp => kvp.Key,
				kvp => businessContext.DataContext.GetNormalizedEntityKey (kvp.Value).Value
			);
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
				var title =	EervParishDataImporter.GetHonorific (honorific);

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


		private static PersonMrMrs GetHonorific(string honorific)
		{
			switch (honorific)
			{
				case "Monsieur":
					return PersonMrMrs.Monsieur;

				case "Madame":
					return PersonMrMrs.Madame;

				case "Mademoiselle":
					return PersonMrMrs.Mademoiselle;

				default:
					return PersonMrMrs.None;
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
			var parsedPhone = TwixTel.ParsePhoneNumber (phone);

			if (!TwixTel.IsValidPhoneNumber (parsedPhone, false))
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

					if (TwixTel.IsValidPhoneNumber (number, acceptEmptyNumbers: false))
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
				var parsedNumber = TwixTel.ParsePhoneNumber (phoneNumber);

				if (TwixTel.IsValidPhoneNumber (parsedNumber, false))
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


		private static void CombineComments(IComment entity, string text)
		{
			if (string.IsNullOrWhiteSpace (text))
			{
				return;
			}

			// With the null reference virtualizer, we don't need to handle explicitely the case
			// when there is no comment defined yet.

			var comment = entity.Comment;
			var combinedText = TextFormatter.FormatText (comment.Text, "~\n\n", text);

			// HACK This is a temporary hack to avoid texts with 800 or more chars with are not
			// allowed in this field. The type of the field should be corrected to allow texts of
			// unlimited size.

			if (combinedText.Length >= 800)
			{
				return;
			}

			comment.Text = combinedText;
		}

		private static void CombineSystemComments(IComment entity, string text)
		{
			// With the null reference virtualizer, we don't need to handle explicitely the case
			// when there is no comment defined yet.

			var comment = entity.Comment;
			var combinedText = string.Concat (comment.SystemText ?? "", "\n\n", text).Trim ();

			// HACK This is a temporary hack to avoid texts with 800 or more chars with are not
			// allowed in this field. The type of the field should be corrected to allow texts of
			// unlimited size.

			if (combinedText.Length >= 800)
			{
				return;
			}

			comment.SystemText = combinedText;
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


		private static void ProcessHouseholdMatches
		(
			CoreData coreData,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches,
			Dictionary<EervPerson, EntityKey> newEntities,
			IEnumerable<EervHousehold> eervHouseholds
		)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				EervParishDataImporter.ProcessHouseholdMatches (businessContext, matches, newEntities, eervHouseholds);
			}
		}


		private static void ProcessHouseholdMatches
		(
			BusinessContext businessContext,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches,
			Dictionary<EervPerson, EntityKey> newEntities,
			IEnumerable<EervHousehold> eervHouseholds
		)
		{
			var aiderTowns = new AiderTownRepository (businessContext);

			// That's all the persons that are in the parish file.
			var fileAiderPersons = EervParishDataImporter.GetAiderPersons
			(
				businessContext,
				matches,
				newEntities
			);

			// That's all the households related to the persons in the parish file.
			var aiderHouseholds = EervParishDataImporter.GetAiderHouseholds (fileAiderPersons);

			// That's all the member of these households (in the database), which might include
			// persons that are not in the parish file and might not include persons that are in the
			// parish file.
			var otherAiderPersons = EervParishDataImporter.GetAiderPersons (aiderHouseholds);

			var aiderPersons = fileAiderPersons
				.Concat (otherAiderPersons)
				.Distinct ()
				.ToList ();

			AiderEnumerator.LoadRelatedData (businessContext.DataContext, aiderPersons);

			businessContext.Register (aiderPersons);

			foreach (var eervHousehold in eervHouseholds)
			{
				EervParishDataImporter.ProcessHouseholdMatch
				(
					businessContext,
					matches,
					newEntities,
					eervHousehold,
					aiderTowns
				);
			}

			businessContext.SaveChanges
			(
				LockingPolicy.KeepLock,
				EntitySaveMode.IgnoreValidationErrors
			);
		}


		private static List<AiderPersonEntity> GetAiderPersons
		(
			BusinessContext businessContext,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches,
			Dictionary<EervPerson, EntityKey> newEntities
		)
		{
			return matches.Values
				.Where (v => v != null)
				.Select (v => v.Item1)
				.Concat (newEntities.Values)
				.Select (pk => (AiderPersonEntity) businessContext.DataContext.ResolveEntity (pk))
				.ToList ();
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


		private static void ProcessHouseholdMatch
		(
			BusinessContext businessContext,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches,
			Dictionary<EervPerson, EntityKey> newEntities,
			EervHousehold eervHousehold,
			AiderTownRepository aiderTowns
		)
		{
			var eervToAiderPersons = EervParishDataImporter.GetEervToAiderPersons
			(
				businessContext,
				matches,
				newEntities,
				eervHousehold
			);

			var aiderHouseholds = EervParishDataImporter.GetAiderHouseholds
			(
				eervToAiderPersons.Values
			);

			if (aiderHouseholds.Count == 0)
			{
				EervParishDataImporter.CreateHousehold
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
		}


		private static Dictionary<EervPerson, AiderPersonEntity> GetEervToAiderPersons
		(
			BusinessContext businessContext,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches,
			Dictionary<EervPerson, EntityKey> newEntities,
			EervHousehold eervHousehold
		)
		{
			var eervToAiderPersons = new Dictionary<EervPerson, AiderPersonEntity> ();

			foreach (var eervPerson in eervHousehold.Members)
			{
				var match = matches[eervPerson];

				var entityKey = match != null
					? match.Item1
					: newEntities[eervPerson];

				var aiderPerson = businessContext.ResolveEntity<AiderPersonEntity> (entityKey);

				eervToAiderPersons[eervPerson] = aiderPerson;
			}

			return eervToAiderPersons;
		}


		private static void CreateHousehold
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

			var newMembers = eervToAiderPersons
				.Where (p => !currentMembers.Contains (p.Value));

			foreach (var newMember in newMembers)
			{
				var eervPerson = newMember.Key;
				var aiderPerson = newMember.Value;

				var isHead = eervHousehold.Heads.Contains (eervPerson);

				var contact = AiderContactEntity.Create
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

				AiderHouseholdEntity.Delete (businessContext, secondaryHousehold);
			}

			return mainHousehold;
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

			if (eervAddress.IsInSwitzerland ())
			{
				EervParishDataImporter.CombineSwissAddress (aiderTowns, eervAddress, aiderAddress);
			}
			else
			{
				EervParishDataImporter.CombineForeignAddress (aiderTowns, eervAddress, aiderAddress);
			}
		}


		private static void CombineSwissAddress(AiderTownRepository aiderTowns, EervAddress eervAddress, AiderAddressEntity aiderAddress)
		{
			var firstAddressLine = eervAddress.FirstAddressLine;
			var street = eervAddress.StreetName;
			var houseNumber = eervAddress.HouseNumber;
			var houseNumberComplement = eervAddress.HouseNumberComplement;
			var zipCode = int.Parse (eervAddress.ZipCode);
			var zipCodeAddOn = 0;
			var zipCodeId = 0;
			var townName = eervAddress.Town;

			AddressPatchEngine.Current.FixAddress (ref firstAddressLine, ref street, houseNumber, ref zipCode, ref zipCodeAddOn, ref zipCodeId, ref townName);

			aiderAddress.AddressLine1 = firstAddressLine;
			aiderAddress.Street = street;
			aiderAddress.HouseNumber = houseNumber;
			aiderAddress.HouseNumberComplement = houseNumberComplement;

			aiderAddress.Town = aiderTowns.GetTown (eervAddress);
		}


		private static void CombineForeignAddress(AiderTownRepository aiderTowns, EervAddress eervAddress, AiderAddressEntity aiderAddress)
		{
			aiderAddress.AddressLine1 = eervAddress.FirstAddressLine;
			aiderAddress.Street = eervAddress.StreetName;
			aiderAddress.HouseNumber = eervAddress.HouseNumber;
			aiderAddress.HouseNumberComplement = eervAddress.HouseNumberComplement;
			aiderAddress.Town = aiderTowns.GetTown (eervAddress);
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


		private static Dictionary<EervLegalPerson, EntityKey> ImportEervLegalPersons(CoreData coreData, EervParishData eervParishData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				return EervParishDataImporter.ImportEervLegalPersons (businessContext, eervParishData);
			}
		}


		private static Dictionary<EervLegalPerson, EntityKey> ImportEervLegalPersons(BusinessContext businessContext, EervParishData eervParishData)
		{
			var aiderLegalPersons = EervParishDataImporter.ImportEervLegalPersons (businessContext, eervParishData, eervParishData.Id);

			if (eervParishData.Id.Kind == EervKind.Parish)
			{
				ParishAssigner.AssignToParish (businessContext, aiderLegalPersons.Values, eervParishData.Id.Name);
			}

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
				aiderContact.PersonFullName =  EervParishDataImporter.GetContactFullName (eervContact);
				aiderContact.PersonMrMrs = EervParishDataImporter.GetHonorific (eervContact.Honorific);
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


		private static void AssignToParishes(CoreData coreData, ParishAddressRepository parishRepository, IEnumerable<EntityKey> aiderPersonKeys)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				EervParishDataImporter.AssignToParishes (businessContext, parishRepository, aiderPersonKeys);
			}
		}


		private static void AssignToParishes(BusinessContext businessContext, ParishAddressRepository parishRepository, IEnumerable<EntityKey> aiderPersonKeys)
		{
			var aiderPersons = aiderPersonKeys
				.Select (k => businessContext.ResolveEntity<AiderPersonEntity> (k))
				.ToList ();

			AiderEnumerator.LoadRelatedData (businessContext.DataContext, aiderPersons);

			ParishAssigner.AssignToParish (parishRepository, businessContext, aiderPersons);

			businessContext.Register (aiderPersons);
			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static void AssignToImportationGroup(CoreData coreData, EervId parishId, IEnumerable<EntityKey> aiderPersonKeys)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				EervParishDataImporter.AssignToImportationGroup (businessContext, parishId, aiderPersonKeys);
			}
		}


		private static void AssignToImportationGroup(BusinessContext businessContext, EervId parishId, IEnumerable<EntityKey> aiderPersonKeys)
		{
			var aiderPersons = aiderPersonKeys
				.Select (k => businessContext.ResolveEntity<AiderPersonEntity> (k))
				.ToList ();

			var importationGroup = EervParishDataImporter.FindImportationGroup (businessContext, parishId);

			foreach (var aiderPerson in aiderPersons)
			{
				var participationData = new ParticipationData
				{
					Person = aiderPerson,
				};

				AiderGroupParticipantEntity.StartParticipation (businessContext, importationGroup, participationData);
			}

			// We load the related data to speed up the execution of the business rules.
			AiderEnumerator.LoadRelatedData (businessContext.DataContext, aiderPersons);

			businessContext.Register (aiderPersons);
			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static AiderGroupEntity FindImportationGroup(BusinessContext businessContext, EervId parishId)
		{
			var parishGroup = EervParishDataImporter
				.FindRootAiderGroups (businessContext, parishId)
				.Single ();

			return parishGroup.Subgroups.Single (g => g.Name == "Personnes importées");
		}


		private static Dictionary<EervGroup, EntityKey> ImportEervGroups(CoreData coreData, EervParishData eervParishData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				return EervParishDataImporter.ImportEervGroups (businessContext, eervParishData);
			}
		}


		private static Dictionary<EervGroup, EntityKey> ImportEervGroups(BusinessContext businessContext, EervParishData eervParishData)
		{
			var eervGroups = eervParishData.Groups;
			var eervId = eervParishData.Id;

			var idToGroups = EervParishDataImporter.BuildIdToGroups (businessContext, eervId);
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


		private static Dictionary<string, AiderGroupEntity> BuildIdToGroups(BusinessContext businessContext, EervId eervId)
		{
			var mapping = new Dictionary<string, AiderGroupEntity> ();

			var rootGroups = EervParishDataImporter.FindRootAiderGroups (businessContext, eervId);

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


		private static IEnumerable<AiderGroupEntity> FindRootAiderGroups(BusinessContext businessContext, EervId eervId)
		{
			switch (eervId.Kind)
			{
				case EervKind.Canton:
					return FindRootCantonGroups (businessContext);

				case EervKind.Region:
					return FindRootRegionGroup (businessContext, eervId);

				case EervKind.Parish:
					return FindRootParisGroup (businessContext, eervId);

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


		private static IEnumerable<AiderGroupEntity> FindRootParisGroup(BusinessContext businessContext, EervId eervId)
		{
			yield return ParishAssigner.FindParishGroup (businessContext, eervId.Name);
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

				var participationData = new ParticipationData ();

				if (eervActivity.Person != null)
				{
					var aiderPersonKey = eervPersonToKeys[eervActivity.Person];
					var aiderPerson = (AiderPersonEntity) dataContext.ResolveEntity (aiderPersonKey);

					aiderPersons.Add (aiderPerson);
					participationData.Person = aiderPerson;
				}
				else if (eervActivity.LegalPerson != null)
				{
					var aiderLegalpersonKey = eervLegalPersonToKeys[eervActivity.LegalPerson];
					var aiderLegalperson = (AiderLegalPersonEntity) dataContext.ResolveEntity (aiderLegalpersonKey);

					participationData.LegalPerson = aiderLegalperson;
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
