//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

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


		public static void Import(CoreDataManager coreDataManager, ParishAddressRepository parishRepository, EervMainData eervMainData, EervParishData eervParishData)
		{
			var eervPersonMapping = EervParishDataImporter.ImportEervPhysicalPersons (coreDataManager, parishRepository, eervParishData);

			EervParishDataImporter.ImportEervLegalPersons (coreDataManager, eervParishData);

			var eervGroupMapping = EervParishDataImporter.ImportEervGroups (coreDataManager, eervMainData, eervParishData);

			EervParishDataImporter.ImportEervActivities (coreDataManager, eervParishData, eervPersonMapping, eervGroupMapping);

			coreDataManager.CoreData.ResetIndexes ();
		}


		private static Dictionary<EervPerson, EntityKey> ImportEervPhysicalPersons(CoreDataManager coreDataManager, ParishAddressRepository parishRepository, EervParishData eervParishData)
		{
			var matches = EervParishDataImporter.FindMatches (coreDataManager, eervParishData);
			var newEntities = EervParishDataImporter.ProcessMatches (coreDataManager, eervParishData.Id.Name, matches);
			var mapping = EervParishDataImporter.BuildEervPersonMapping (matches, newEntities);

			EervParishDataImporter.ProcessHouseholdMatches (coreDataManager, matches, newEntities, eervParishData.Households);
			EervParishDataImporter.AssignToParishes (coreDataManager, parishRepository, newEntities.Values);
			EervParishDataImporter.AssignToImportationGroup (coreDataManager, eervParishData.Id, mapping.Values);

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


		private static Dictionary<EervPerson, Tuple<EntityKey, MatchData>> FindMatches(CoreDataManager coreDataManager, EervParishData eervParishData)
		{
			var normalizedAiderPersons = Normalizer.Normalize (coreDataManager);
			var normalizedEervPersons = Normalizer.Normalize (eervParishData.Households);

			var matches = EervParishDataMatcher.FindMatches (normalizedEervPersons.Keys, normalizedAiderPersons.Keys);

			return matches.ToDictionary
			(
				m => normalizedEervPersons[m.Item1],
				m => m.Item2 == null
					? null
					: Tuple.Create (normalizedAiderPersons[m.Item2.Item1], m.Item2.Item2)
			);
		}


		private static Dictionary<EervPerson, EntityKey> ProcessMatches(CoreDataManager coreDataManager, string parishName, Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches)
		{
			Func<BusinessContext, Dictionary<EervPerson, EntityKey>> function = b =>
			{
				return EervParishDataImporter.ProcessMatches (b, parishName, matches);
			};

			return coreDataManager.Execute (function);
		}


		private static Dictionary<EervPerson, EntityKey> ProcessMatches(BusinessContext businessContext, string parishName, Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches)
		{
			var newEntities = new Dictionary<EervPerson, AiderPersonEntity> ();

			var dataContext = businessContext.DataContext;

			foreach (var match in matches)
			{
				var eervPerson = match.Key;

				if (match.Value != null)
				{
					var m = match.Value;
					var aiderPerson = (AiderPersonEntity) dataContext.ResolveEntity (m.Item1);
					var matchData = m.Item2;

					EervParishDataImporter.CombineAiderPersonWithEervPerson (businessContext, eervPerson, aiderPerson);
					EervParishDataImporter.AddMatchComment (eervPerson, aiderPerson, matchData, parishName);
				}
				else
				{
					var aiderPerson = EervParishDataImporter.CreateAiderPersonWithEervPerson (businessContext, eervPerson);

					EervParishDataImporter.CombineAiderPersonWithEervPerson (businessContext, eervPerson, aiderPerson);
					EervParishDataImporter.AddMatchComment (eervPerson, aiderPerson, null, parishName);

					newEntities[eervPerson] = aiderPerson;
				}
			}

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
			EervParishDataImporter.CombineRemarks (eervPerson, aiderPerson);
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


		private static void CombineRemarks(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var remarks = eervPerson.Remarks;

			if (!string.IsNullOrEmpty (remarks))
			{
				EervParishDataImporter.CombineComments (aiderPerson, remarks);
			}
		}


		private static void CombineCoordinates(BusinessContext businessContext, EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var cleanEmails  = EervParishDataImporter.GetCleanEmails (eervPerson.Coordinates.EmailAddress);
			var clearMobiles = EervParishDataImporter.GetCleanPhoneNumbers (eervPerson.Coordinates.MobilePhoneNumber);

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
					var address = contact.Address;

					EervParishDataImporter.SetEmail (address, email);
					EervParishDataImporter.SetPhoneNumber (address, mobile, (a, s) => a.Mobile = s);
				}
			}
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


		private static void AddMatchComment(EervPerson eervPerson, AiderPersonEntity aiderPerson, MatchData match, string parishName)
		{
			string text;

			if (match != null)
			{
				text = "Correspondance avec la personne " + eervPerson.Id + " de la paroisse de " + parishName + ": ";
				text += "\n- Nom: " + EervParishDataImporter.GetTextForJaroWinklerMatch (match.Lastname);
				text += "\n- Prénom: " + EervParishDataImporter.GetTextForJaroWinklerMatch (match.Firstname);
				text += "\n- Date de naissance: " + EervParishDataImporter.GetTextForJaroWinklerMatch (match.DateOfBirth);
				text += "\n- Sexe: " + EervParishDataImporter.GetTextForSexMatch (match.Sex);
				text += "\n- Adresse: " + EervParishDataImporter.GetTextForAddressMatch (match.Address);
			}
			else
			{
				text = "Personne créée à partir de la personne " + eervPerson.Id + " de la paroisse de " + parishName + ", sans correspondance dans le fichier ECH.";
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
			CoreDataManager coreDataManager,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches,
			Dictionary<EervPerson, EntityKey> newEntities,
			IEnumerable<EervHousehold> eervHouseholds
		)
		{
			coreDataManager.Execute (b =>
			{
				EervParishDataImporter.ProcessHouseholdMatches (b, matches, newEntities, eervHouseholds);
			});
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

			var aiderPersons = EervParishDataImporter.GetAiderPersons
			(
				businessContext,
				matches,
				newEntities
			);

			var aiderContacts = EervParishDataImporter.GetAiderContacts (aiderPersons);

			var aiderPersonToContacts = EervParishDataImporter.GetAiderPersonToContacts
			(
				aiderContacts,
				aiderPersons
			);

			var aiderHouseholdToContacts = EervParishDataImporter.GetAiderHouseholdToContacts
			(
				aiderContacts
			);

			foreach (var eervHousehold in eervHouseholds)
			{
				EervParishDataImporter.ProcessHouseholdMatch
				(
					businessContext,
					matches,
					newEntities,
					eervHousehold,
					aiderTowns,
					aiderPersonToContacts,
					aiderHouseholdToContacts
				);
			}

			businessContext.SaveChanges
			(
				LockingPolicy.KeepLock,
				EntitySaveMode.IgnoreValidationErrors
			);
		}


		private static Dictionary<AiderPersonEntity, List<AiderContactEntity>> GetAiderPersonToContacts
		(
			IList<AiderContactEntity> aiderContacts,
			IList<AiderPersonEntity> aiderPersons
		)
		{
			var aiderPersonToContacts = aiderContacts
				.GroupBy (c => c.Person)
				.ToDictionary (g => g.Key, g => g.ToList ());

			foreach (var aiderPerson in aiderPersons)
			{
				if (!aiderPersonToContacts.ContainsKey (aiderPerson))
				{
					aiderPersonToContacts[aiderPerson] = new List<AiderContactEntity> ();
				}
			}                
			
			return aiderPersonToContacts;
		}


		private static Dictionary<AiderHouseholdEntity, List<AiderContactEntity>> GetAiderHouseholdToContacts
		(
			IList<AiderContactEntity> aiderContacts
		)
		{
			return aiderContacts
				.GroupBy (c => c.Household)
				.ToDictionary (g => g.Key, g => g.ToList ());
		}


		private static IList<AiderPersonEntity> GetAiderPersons
		(
			BusinessContext businessContext,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches,
			Dictionary<EervPerson, EntityKey> newEntities
		)
		{
			var dataContext = businessContext.DataContext;

			return matches.Values
				.Where (v => v != null)
				.Select (v => v.Item1)
				.Concat (newEntities.Values)
				.Select (pk => (AiderPersonEntity) dataContext.ResolveEntity (pk))
				.ToList ();
		}


		private static IList<AiderContactEntity> GetAiderContacts(IList<AiderPersonEntity> persons)
		{
			return persons
				.SelectMany (p => p.Households)
				.Distinct ()
				.SelectMany (h => h.Contacts)
				.ToList ();
		}


		private static void ProcessHouseholdMatch
		(
			BusinessContext businessContext,
			Dictionary<EervPerson, Tuple<EntityKey, MatchData>> matches,
			Dictionary<EervPerson, EntityKey> newEntities,
			EervHousehold eervHousehold,
			AiderTownRepository aiderTowns,
			Dictionary<AiderPersonEntity, List<AiderContactEntity>> aiderPersonToContacts,
			Dictionary<AiderHouseholdEntity, List<AiderContactEntity>> aiderHouseholdToContacts
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
				eervToAiderPersons.Values,
				aiderPersonToContacts
			);

			if (aiderHouseholds.Count == 0)
			{
			    EervParishDataImporter.CreateHousehold
				(
					businessContext,
					eervHousehold,
					eervToAiderPersons,
					aiderTowns,
					aiderPersonToContacts,
					aiderHouseholdToContacts
				);
			}
			else if (aiderHouseholds.Count == 1)
			{
			    EervParishDataImporter.ExpandHousehold
				(
					businessContext,
					eervHousehold,
					eervToAiderPersons,
					aiderHouseholds.Single (),
					aiderPersonToContacts,
					aiderHouseholdToContacts
				);
			}
			else
			{
				EervParishDataImporter.CombineHouseholds
				(
					businessContext,
					eervHousehold,
					eervToAiderPersons,
					aiderHouseholds,
					aiderPersonToContacts,
					aiderHouseholdToContacts
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


		private static List<AiderHouseholdEntity> GetAiderHouseholds
		(
			IEnumerable<AiderPersonEntity> aiderPersons,
			Dictionary<AiderPersonEntity, List<AiderContactEntity>> aiderPersonToContacts
		)
		{
			return aiderPersons
				.Where (p => aiderPersonToContacts.ContainsKey (p))
				.SelectMany (p => aiderPersonToContacts[p])
				.Select (c => c.Household)
				.Distinct ()
				.ToList ();
		}


		private static void CreateHousehold
		(
			BusinessContext businessContext,
			EervHousehold eervHousehold,
			Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons,
			AiderTownRepository aiderTowns,
			Dictionary<AiderPersonEntity, List<AiderContactEntity>> aiderPersonToContacts,
			Dictionary<AiderHouseholdEntity, List<AiderContactEntity>> aiderHouseholdToContacts
		)
		{
			var aiderHousehold = businessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();

			aiderHouseholdToContacts[aiderHousehold] = new List<AiderContactEntity> ();

			EervParishDataImporter.ExpandHousehold
			(
				businessContext,
				eervHousehold,
				eervToAiderPersons,
				aiderHousehold,
				aiderPersonToContacts,
				aiderHouseholdToContacts
			);

			aiderHousehold.Address = businessContext.CreateAndRegisterEntity<AiderAddressEntity> ();

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
			AiderHouseholdEntity aiderHousehold,
			Dictionary<AiderPersonEntity, List<AiderContactEntity>> aiderPersonToContacts,
			Dictionary<AiderHouseholdEntity, List<AiderContactEntity>> aiderHouseholdToContacts
		)
		{
			EervParishDataImporter.CombineMembers
			(
				businessContext,
				eervHousehold,
				eervToAiderPersons,
				aiderHousehold,
				aiderPersonToContacts,
				aiderHouseholdToContacts
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
			AiderHouseholdEntity aiderHousehold,
			Dictionary<AiderPersonEntity, List<AiderContactEntity>> aiderPersonToContacts,
			Dictionary<AiderHouseholdEntity, List<AiderContactEntity>> aiderHouseholdToContacts
		)
		{
			var householdContacts = aiderHouseholdToContacts[aiderHousehold];

			var currentMembers = householdContacts
				.Select (c => c.Person)
				.ToList ();

			var newMembers = eervToAiderPersons
				.Where (p => !currentMembers.Contains (p.Value));

			var adults   = new List<eCH_PersonEntity> ();
			var children = new List<eCH_PersonEntity> ();

			foreach (var newMember in newMembers)
			{
				var eervPerson = newMember.Key;
				var aiderPerson = newMember.Value;

				var isHead = eervHousehold.Heads.Contains (eervPerson);

				if (isHead)
				{
					adults.Add (aiderPerson.eCH_Person);
				}
				else
				{
					children.Add (aiderPerson.eCH_Person);
				}

				var contact = AiderContactEntity.Create
				(
					businessContext,
					aiderPerson,
					aiderHousehold,
					isHead
				);

				var personContacts = aiderPersonToContacts[aiderPerson];

				householdContacts.Add (contact);
				personContacts.Add (contact);
			}
		}


		private static void CombineHouseholds
		(
			BusinessContext businessContext,
			EervHousehold eervHousehold,
			Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons,
			List<AiderHouseholdEntity> aiderHouseholds,
			Dictionary<AiderPersonEntity, List<AiderContactEntity>> aiderPersonToContacts,
			Dictionary<AiderHouseholdEntity, List<AiderContactEntity>> aiderHouseholdToContacts
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
					households,
					aiderHouseholdToContacts
				);

				mainHouseholds.Add (mainEffectiveHousehold);
			}

			var mainHousehold = EervParishDataImporter.GetMainHousehold
			(
				mainHouseholds,
				aiderHouseholdToContacts
			);

			EervParishDataImporter.ExpandHousehold
			(
				businessContext,
				eervHousehold,
				eervToAiderPersons,
				mainHousehold,
				aiderPersonToContacts,
				aiderHouseholdToContacts
			);
		}


		private static AiderHouseholdEntity CombineEffectiveHouseholds
		(
			BusinessContext businessContext,
			List<AiderHouseholdEntity> aiderHouseholds,
			Dictionary<AiderHouseholdEntity, List<AiderContactEntity>> aiderHouseholdToContacts
		)
		{
			var mainHousehold = EervParishDataImporter.GetMainHousehold
			(
				aiderHouseholds,
				aiderHouseholdToContacts
			);
			
			var secondaryHouseholds = aiderHouseholds.Where (h => h != mainHousehold);

			foreach (var secondaryHousehold in secondaryHouseholds)
			{
				EervParishDataImporter.CombineEffectiveHouseholds
				(
					businessContext,
					mainHousehold,
					secondaryHousehold,
					aiderHouseholdToContacts
				);
			}

			return mainHousehold;
		}


		private static AiderHouseholdEntity GetMainHousehold
		(
			List<AiderHouseholdEntity> aiderHouseholds,
			Dictionary<AiderHouseholdEntity, List<AiderContactEntity>> aiderHouseholdToContacts
		)
		{
			return aiderHouseholds
				.OrderByDescending (h => aiderHouseholdToContacts[h].Count)
				.First ();
		}


		private static void CombineEffectiveHouseholds
		(
			BusinessContext businessContext,
			AiderHouseholdEntity mainHousehold,
			AiderHouseholdEntity secondaryHousehold,
			Dictionary<AiderHouseholdEntity, List<AiderContactEntity>> aiderHouseholdToContacts
		)
		{
			var mainHouseholdContacts = aiderHouseholdToContacts[mainHousehold];
			var secondaryHouseholdContacts = aiderHouseholdToContacts[secondaryHousehold];
			
			foreach (var contact in secondaryHouseholdContacts)
			{
				var newContact = AiderContactEntity.Create (businessContext, contact.Person, mainHousehold, false);
				mainHouseholdContacts.Add (newContact);

				AiderContactEntity.Delete (businessContext, contact);
			}

			aiderHouseholdToContacts.Remove (secondaryHousehold);

			var comment = secondaryHousehold.Comment;

			if (comment.IsNotNull ())
			{
				businessContext.DeleteEntity (comment);
			}

			var address = secondaryHousehold.Address;

			if (address.IsNotNull ())
			{
				businessContext.DeleteEntity (address);
			}

			businessContext.DeleteEntity (secondaryHousehold);
		}


		private static void CombineAddress(AiderTownRepository aiderTowns, AiderAddressEntity aiderAddress, EervAddress eervAddress)
		{
			if (string.IsNullOrEmpty (eervAddress.ZipCode) || string.IsNullOrEmpty (eervAddress.Town))
			{
				// We have an invalid address or no address at all, thus we ignore it.

				return;
			}

			if (eervAddress.IsInSwitzerland())
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


		private static void ImportEervLegalPersons(CoreDataManager coreDataManager, EervParishData eervParishData)
		{
			coreDataManager.Execute (b =>
			{
				EervParishDataImporter.ImportEervLegalPersons (b, eervParishData);
			});
		}


		private static void ImportEervLegalPersons(BusinessContext businessContext, EervParishData eervParishData)
		{
			var parishName = eervParishData.Id.Name;
			var aiderTowns = new AiderTownRepository (businessContext);

			foreach (var legalPerson in eervParishData.LegalPersons)
			{
				EervParishDataImporter.ImportEervLegalPerson (businessContext, aiderTowns, parishName, legalPerson);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static void ImportEervLegalPerson(BusinessContext businessContext, AiderTownRepository aiderTowns, string parishName, EervLegalPerson legalPerson)
		{
			var aiderLegalPerson = businessContext.CreateAndRegisterEntity<AiderLegalPersonEntity> ();

			aiderLegalPerson.Name = EervParishDataImporter.GetCorporateName (legalPerson);
			
			var aiderContact = AiderContactEntity.Create (businessContext, aiderLegalPerson);
			var aiderAddress = aiderContact.Address;

			EervParishDataImporter.CombineAddress (aiderTowns, aiderAddress, legalPerson.Address);
			EervParishDataImporter.CombineCoordinates (aiderAddress, legalPerson.Coordinates);
			EervParishDataImporter.CombineCoordinates (aiderAddress, legalPerson.ContactPerson.Coordinates);

			EervParishDataImporter.ImportEervLegalPersonPerson (businessContext, aiderContact, legalPerson);

			var comment = "Ce contact a été crée à partir du contact N°" + legalPerson.Id + " du fichier de la paroisse de " + parishName + ".";
			EervParishDataImporter.CombineSystemComments (aiderLegalPerson, comment);
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


		private static void ImportEervLegalPersonPerson(BusinessContext businessContext, AiderContactEntity aiderContact, EervLegalPerson legalPerson)
		{
			// NOTE If the corporate name is empty, we used the first or last name as corporate
			// name. Therefore, we assume that there is no contact person there.

			if (string.IsNullOrWhiteSpace (legalPerson.Name))
			{
				return;
			}

			var contactPerson = legalPerson.ContactPerson;

			if (string.IsNullOrWhiteSpace (contactPerson.Firstname))
			{
				return;
			}

			if (string.IsNullOrWhiteSpace (contactPerson.Lastname))
			{
				return;
			}
			
			var aiderPerson = businessContext.CreateAndRegisterEntity<AiderPersonEntity> ();

			aiderPerson.eCH_Person.PersonFirstNames = contactPerson.Firstname;
			aiderPerson.eCH_Person.PersonOfficialName = contactPerson.Lastname;
			aiderPerson.eCH_Person.PersonSex = contactPerson.Sex;

			var honorific = contactPerson.Honorific;

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

			aiderContact.Person = aiderPerson;
		}


		private static void AssignToParishes(CoreDataManager coreDataManager, ParishAddressRepository parishRepository, IEnumerable<EntityKey> aiderPersonKeys)
		{
			coreDataManager.Execute (b =>
				EervParishDataImporter.AssignToParishes (b, parishRepository, aiderPersonKeys)
			);
		}


		private static void AssignToParishes(BusinessContext businessContext, ParishAddressRepository parishRepository, IEnumerable<EntityKey> aiderPersonKeys)
		{
			var aiderPersons = aiderPersonKeys
				.Select (k => businessContext.ResolveEntity<AiderPersonEntity> (k))
				.ToList ();

			ParishAssigner.AssignToParishes (parishRepository, businessContext, aiderPersons);

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static void AssignToImportationGroup(CoreDataManager coreDataManager, EervId parishId, IEnumerable<EntityKey> aiderPersonKeys)
		{
			coreDataManager.Execute (b =>
				EervParishDataImporter.AssignToImportationGroup (b, parishId, aiderPersonKeys)
			);
		}


		private static void AssignToImportationGroup(BusinessContext businessContext, EervId parishId, IEnumerable<EntityKey> aiderPersonKeys)
		{
			var aiderPersons = aiderPersonKeys
				.Select (k => businessContext.ResolveEntity<AiderPersonEntity> (k))
				.ToList ();

			var importationGroup = EervParishDataImporter.FindOrCreateImportationGroup (businessContext, parishId);

			foreach (var aiderPerson in aiderPersons)
			{
				AiderGroupParticipantEntity.StartParticipation (businessContext, aiderPerson, importationGroup, null, null);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);
		}


		private static AiderGroupEntity FindOrCreateImportationGroup(BusinessContext businessContext, EervId parishId)
		{
			var parishGroup = EervParishDataImporter.FindRootAiderGroup (businessContext, parishId);
			var name = "Personnes importées";

			var importationGroup = parishGroup
				.Subgroups
				.FirstOrDefault (g => g.Name == name);

			if (importationGroup == null)
			{
				// TODO Don't use the count here. I don't want to them know in order not to break
				// something before the demo :-P

				var count = parishGroup.Subgroups.Count + 1;
				importationGroup = parishGroup.CreateSubGroup (businessContext, name, count);
			}

			return importationGroup;
		}


		private static Dictionary<EervGroup, EntityKey> ImportEervGroups(CoreDataManager coreDataManager, EervMainData eervMainData, EervParishData eervParishData)
		{
			return coreDataManager.Execute (b =>
			{
				return EervParishDataImporter.ImportEervGroups (b, eervMainData, eervParishData);
			});
		}


		private static Dictionary<EervGroup, EntityKey> ImportEervGroups(BusinessContext businessContext, EervMainData eervMainData, EervParishData eervParishData)
		{
			var eervGroupDefinitions = eervMainData.GroupDefinitions;
			var eervGroups = eervParishData.Groups;
			var eervId = eervParishData.Id;

			var rootAiderGroup = EervParishDataImporter.FindRootAiderGroup (businessContext, eervId);
			var aiderSubGroupMapping = EervParishDataImporter.BuildAiderSubGroupMapping (businessContext, rootAiderGroup);

			var rootEervGroupDefinition = EervParishDataImporter.FindRootEervGroupDefinition (eervId, eervGroupDefinitions);
			var aiderIdMapping = EervParishDataImporter.BuildAiderIdMapping (rootAiderGroup, aiderSubGroupMapping, rootEervGroupDefinition);

			// We sort the groups so that they appear in the right order, that is, the parent before
			// their children.
			var sortedEervGroups = eervGroups.OrderBy (g => g.Id);

			var groupMapping = new Dictionary<EervGroup, AiderGroupEntity> ();

			foreach (var eervGroup in sortedEervGroups)
			{
				if (eervId.IsParish && !eervGroup.Id.StartsWith ("04"))
				{
					throw new Exception ("Invalid group id!");
				}
				else if (!eervId.IsParish && !eervGroup.Id.StartsWith ("03"))
				{
					throw new Exception ("Invalid group id!");
				}

				AiderGroupEntity aiderGroup;

				if (aiderIdMapping.TryGetValue (eervGroup.Id, out aiderGroup))
				{
					groupMapping[eervGroup] = aiderGroup;
				}
				else if (aiderIdMapping.TryGetValue (EervGroupDefinition.GetParentId (eervGroup.Id), out aiderGroup))
				{
					var subgroups = aiderSubGroupMapping[aiderGroup];
					
					// TODO Don't use the count here. This might lead to lots of simplifications
					// but I don't want to them know in order not to break something before the
					// demo :-P

					// HACK This is a hack for group names greater than 200 chars that will throw an
					// exception later. This need to be corrected.
					var name = eervGroup.Name.Substring (0, Math.Min (200, eervGroup.Name.Length));
					var number = subgroups.Count + 1;
					var newAiderGroup = aiderGroup.CreateSubGroup (businessContext, name, number);

					subgroups.Add (newAiderGroup);
					aiderSubGroupMapping[newAiderGroup] = new List<AiderGroupEntity> ();

					aiderIdMapping[eervGroup.Id] = newAiderGroup;
					groupMapping[eervGroup] = newAiderGroup;
				}
				else
				{
					Debug.WriteLine ("WARNING: group " + eervGroup.Id + " has no parent defined.");
				}
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			return groupMapping.ToDictionary
			(
				g => g.Key,
				g => businessContext.DataContext.GetNormalizedEntityKey (g.Value).Value
			);
		}


		private static Dictionary<string, AiderGroupEntity> BuildAiderIdMapping(AiderGroupEntity rootAiderGroup, Dictionary<AiderGroupEntity, IList<AiderGroupEntity>> subGroupMapping, EervGroupDefinition rootEervGroupDefinition)
		{
			var result = new Dictionary<string, AiderGroupEntity> ();

			var groupChains = EervParishDataImporter.GetGroupChains (rootAiderGroup, subGroupMapping);

			foreach (var groupChain in groupChains)
			{
				var eervGroupDefinition = EervParishDataImporter.FindEervGroupDefinition (groupChain, rootEervGroupDefinition);

				if (eervGroupDefinition != null)
				{
					result[eervGroupDefinition.Id] = groupChain.Last ();
				}
			}

			return result;
		}


		private static IEnumerable<IEnumerable<AiderGroupEntity>> GetGroupChains(AiderGroupEntity rootAiderGroup, Dictionary<AiderGroupEntity, IList<AiderGroupEntity>> subGroupMapping)
		{
			// This method looks terrible, but it's not as bad as it seems. It perform a depth first
			// iteration of the tree of groups. That's the job the the loop and of the todo stack.
			// While performing this iteration, we maintain the results in the chain list and return
			// it at each iteration.

			var chain = new List<AiderGroupEntity> ();

			var todo = new Stack<AiderGroupEntity> ();
			todo.Push (rootAiderGroup);

			while (todo.Any ())
			{
				var group = todo.Pop ();

				while (chain.Count > 0 && !subGroupMapping[chain[chain.Count - 1]].Contains (group))
				{
					chain.RemoveAt (chain.Count - 1);
				}

				chain.Add (group);

				yield return chain.AsReadOnly ();

				todo.PushRange (subGroupMapping[group]);
			}
		}


		private static EervGroupDefinition FindEervGroupDefinition(IEnumerable<AiderGroupEntity> aiderGroupChain, EervGroupDefinition eervGroupDefinition)
		{
			var result = eervGroupDefinition;

			// Here we skip the first element because we know that eervGroupDefinition matches the
			// first element in the chain.
			foreach (var aiderGroup in aiderGroupChain.Skip (1))
			{
				result = result
					.Children
					.Where (g => g.Name == aiderGroup.Name)
					.FirstOrDefault ();

				if (result == null)
				{
					break;
				}
			}

			return result;
		}


		private static AiderGroupEntity FindRootAiderGroup(BusinessContext businessContext, EervId eervId)
		{
			if (eervId.IsParish)
			{
				var parishName = eervId.Name;

				return AiderGroupEntity.FindParishGroup (businessContext, parishName);
			}
			else
			{
				var regionNumber = int.Parse (StringUtils.GetDigits (eervId.Name));

				return AiderGroupEntity.FindRegionGroup (businessContext, regionNumber);
			}
		}


		private static EervGroupDefinition FindRootEervGroupDefinition(EervId eervId, IEnumerable<EervGroupDefinition> groupDefinitions)
		{
			var groupName = eervId.IsParish
				? AiderGroupDefEntity.GetParishGroupDefName ()
				: AiderGroupDefEntity.GetRegionGroupDefName ();

			return groupDefinitions.Where (g => g.Name == groupName).Single ();
		}


		private static Dictionary<AiderGroupEntity, IList<AiderGroupEntity>> BuildAiderSubGroupMapping(BusinessContext businessContext, AiderGroupEntity aiderGroup)
		{
			var mapping = new Dictionary<AiderGroupEntity, IList<AiderGroupEntity>> ();

			var todo = new Stack<AiderGroupEntity> ();
			todo.Push (aiderGroup);

			while (todo.Count > 0)
			{
				var currentGroup = todo.Pop ();
				var subGroups = currentGroup.FindSubgroups (businessContext).ToList ();

				mapping[currentGroup] = subGroups;

				todo.PushRange (subGroups);
			}

			return mapping;
		}


		private static void ImportEervActivities(CoreDataManager coreDataManager, EervParishData eervParishData, Dictionary<EervPerson, EntityKey> eervPersonToKeys, Dictionary<EervGroup, EntityKey> eervGroupToKeys)
		{
			coreDataManager.Execute (b =>
			{
				EervParishDataImporter.ImportEervActivities (b, eervParishData, eervPersonToKeys, eervGroupToKeys);
			});
		}


		private static void ImportEervActivities(BusinessContext businessContext, EervParishData eervParishData, Dictionary<EervPerson, EntityKey> eervPersonToKeys, Dictionary<EervGroup, EntityKey> eervGroupToKeys)
		{
			// TODO Test this method to check if it really works.

			var dataContext = businessContext.DataContext;

			foreach (var eervActivity in eervParishData.Activities)
			{
				if (eervActivity.Person != null)
				{
					var eervPerson = eervActivity.Person;
					var eervGroup = eervActivity.Group;

					var aiderPersonKey = eervPersonToKeys[eervPerson];
					var aiderGroupKey = eervGroupToKeys[eervGroup];

					var aiderPerson = (AiderPersonEntity) dataContext.ResolveEntity (aiderPersonKey);
					var aiderGroup = (AiderGroupEntity) dataContext.ResolveEntity (aiderGroupKey);

					var startDate = eervActivity.StartDate;
					var endDate = eervActivity.EndDate;
					var remarks = TextFormatter.FormatText (eervActivity.Remarks);

					AiderGroupParticipantEntity.ImportParticipation (businessContext, aiderPerson, aiderGroup, startDate, endDate, remarks);
				}
				else if (eervActivity.LegalPerson != null)
				{
					// For now we don't consider the activities linked to legal persons as we dont have
					// a way to store them in the database.

					Debug.WriteLine ("WARNING: activity with legal persons are not considered yet.");
				}
				else
				{
					throw new NotImplementedException ();
				}
			}
		}


	}


}