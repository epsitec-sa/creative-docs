﻿using Epsitec.Aider.Entities;
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

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervParishDataImporter
	{


		public static void Import(BusinessContextManager businessContextManager, string parishName, EervParishData eervParishData)
		{
			// TODO Import legal persons
			// TODO Import group & activity data

			EervParishDataImporter.ImportEervPhysicalPersons (businessContextManager, parishName, eervParishData);
		}


		private static void ImportEervPhysicalPersons(BusinessContextManager businessContextManager, string parishName, EervParishData eervParishData)
		{
			var matches = EervParishDataImporter.FindMatches (businessContextManager, eervParishData);
			
			var newEntities = EervParishDataImporter.ProcessMatches (businessContextManager, parishName, matches);
			
			EervParishDataImporter.ProcessHouseholdMatches (businessContextManager, matches, newEntities, eervParishData.Households);
		}


		private static Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> FindMatches(BusinessContextManager businessContextManager, EervParishData eervParishData)
		{
			var normalizedAiderPersons = EervParishDataImporter.GetNormalizedPersons (businessContextManager);
			var normalizedEervPersons = EervParishDataImporter.GetNormalizedPersons (eervParishData);

			var matches = EervParishDataMatcher.FindMatches (normalizedEervPersons.Keys, normalizedAiderPersons.Keys);

			return matches.ToDictionary
			(
				m => normalizedEervPersons[m.Item1],
				m => m.Item2
					.Select (t => Tuple.Create (normalizedAiderPersons[t.Item1], t.Item2))
					.ToList ()
			);
		}


		private static Dictionary<NormalizedPerson, EntityKey> GetNormalizedPersons(BusinessContextManager businessContextManager)
		{
			Func<BusinessContext, Dictionary<NormalizedPerson, EntityKey>> function = b =>
			{
				return EervParishDataImporter.GetNormalizedPersons (b);
			};

			return businessContextManager.Execute (function);
		}


		private static Dictionary<NormalizedPerson, EntityKey> GetNormalizedPersons(BusinessContext businessContext)
		{
			var aiderHouseholds = businessContext.GetAllEntities<AiderHouseholdEntity> ();
			var aiderPersons = businessContext.GetAllEntities<AiderPersonEntity> ();

			businessContext.GetAllEntities<eCH_PersonEntity> ();
			businessContext.GetAllEntities<AiderAddressEntity> ();
			businessContext.GetAllEntities<AiderTownEntity> ();

			var normalizedAiderData = Normalizer.Normalize (aiderHouseholds, aiderPersons);

			var dataContext = businessContext.DataContext;

			return normalizedAiderData.Item2.ToDictionary
			(
				kvp => kvp.Key,
				kvp => dataContext.GetNormalizedEntityKey (kvp.Value).Value
			);
		}


		private static Dictionary<NormalizedPerson, EervPerson> GetNormalizedPersons(EervParishData eervParishData)
		{
			return Normalizer.Normalize(eervParishData.Households).Item2;
		}


		private static Dictionary<EervPerson, EntityKey> ProcessMatches(BusinessContextManager businessContextManager, string parishName, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches)
		{
			Func<BusinessContext, Dictionary<EervPerson, EntityKey>> function = b =>
			{
				return EervParishDataImporter.ProcessMatches (b, parishName, matches);
			};

			return businessContextManager.Execute (function);
		}


		private static Dictionary<EervPerson, EntityKey> ProcessMatches(BusinessContext businessContext, string parishName, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches)
		{
			var newEntities = new Dictionary<EervPerson, AiderPersonEntity> ();
			
			var dataContext = businessContext.DataContext;

			foreach (var match in matches)
			{
				var eervPerson = match.Key;

				if (match.Value.Any ())
				{
					foreach (var m in match.Value)
					{
						var aiderPerson = (AiderPersonEntity) dataContext.ResolveEntity (m.Item1);
						var matchData = m.Item2;

						EervParishDataImporter.CombineAiderPersonWithEervPerson (businessContext, eervPerson, aiderPerson);
						EervParishDataImporter.AddMatchComment (eervPerson, aiderPerson, matchData, parishName);
					}
				}
				else
				{
					var aiderPerson = EervParishDataImporter.CreateAiderPersonWithEervPerson (businessContext, eervPerson);

					EervParishDataImporter.CombineAiderPersonWithEervPerson (businessContext, eervPerson, aiderPerson);
					EervParishDataImporter.AddMatchComment (eervPerson, aiderPerson, null, parishName);

					newEntities[eervPerson] = aiderPerson;
				}
			}

			businessContext.SaveChanges ();

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
				switch (honorific)
				{
					case "Monsieur":
						aiderPerson.MrMrs = PersonMrMrs.Monsieur;
						break;

					case "Madame":
						aiderPerson.MrMrs = PersonMrMrs.Madame;
						break;

					case "Mademoiselle":
						aiderPerson.MrMrs = PersonMrMrs.Mademoiselle;
						break;

					default:
						aiderPerson.Title = honorific;
						break;
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
			var email = eervPerson.Coordinates.EmailAddress;
			var mobile = eervPerson.Coordinates.MobilePhoneNumber;

			var hasEmail = !string.IsNullOrEmpty (email);
			var hasMobile = !string.IsNullOrEmpty (mobile);

			if (hasEmail || hasMobile)
			{
				// NOTE Here we need to check this because the list is implemented by 4 fields and
				// we don't want to skip the address in this case. It will probably never happen,
				// but in case it does, it won't go unnoticed.
				if (aiderPerson.AdditionalAddresses.Count >= 4)
				{
					throw new InvalidOperationException ();
				}

				var address = businessContext.CreateEntity<AiderAddressEntity> ();

				EervParishDataImporter.SetEmail (address, email);
				EervParishDataImporter.SetPhoneNumber (address, mobile, (a, s) => a.Mobile = s);

				aiderPerson.AdditionalAddresses.Add (address);
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

					EervParishDataImporter.CombineComments (address, text);
				}
			}
		}


		private static void CombineComments(IComment entity, string text)
		{
			// With the null reference virtualizer, we don't need to handle explicitely the case
			// when there is no comment defined yet.

			var comment = entity.Comment;

			var escapedText = FormattedText.Escape (text);
			var combinedText = TextFormatter.FormatText (comment.Text, "~\n\n", escapedText);

			// HACK This is a temporary hack to avoid texts with 800 or more chars with are not
			// allowed in this field. The type of the field should be corrected to allow texts of
			// unlimited size.

			if (combinedText.Length >= 800)
			{
				return;
			}

			comment.Text = combinedText;
		}


		private static AiderPersonEntity CreateAiderPersonWithEervPerson(BusinessContext businessContext, EervPerson eervPerson)
		{
			var aiderPerson = businessContext.CreateEntity<AiderPersonEntity> ();
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
				text = "Cette Personne correspond à la personne N° " + eervPerson.Id + " du fichier de la paroisse de " + parishName + ".";

				text += "\nLa correspondance a été faite sur les critères suivants : ";
				text += "\n - Nom de famille : " + EervParishDataImporter.GetTextForJaroWinklerMatch (match.Lastname);
				text += "\n - Prénom : " + EervParishDataImporter.GetTextForJaroWinklerMatch (match.Firstname);
				text += "\n - Date de naissance : " + EervParishDataImporter.GetTextForJaroWinklerMatch (match.DateOfBirth);
				text += "\n - Sexe : " + EervParishDataImporter.GetTextForSexMatch (match.Sex);
				text += "\n - Adresse : " + EervParishDataImporter.GetTextForAddressMatch (match.Address);
			}
			else
			{
				text = "Cette personne a été crée à partir de la personne N°" + eervPerson.Id + " du fichier de la paroisse de " + parishName + " et n'existe pas dans le registre cantonal des personnes protestantes.";
			}

			EervParishDataImporter.CombineComments (aiderPerson, text);
		}


		private static string GetTextForAddressMatch(AddressMatch match)
		{
			switch (match)
			{
				case AddressMatch.Full:
					return "rue, numéro, numéro postal et localité identiques";

				case AddressMatch.None:
					return "pas de correspondance";

				case AddressMatch.StreetZipCity:
					return "rue, numéro postal et localité identiques";

				case AddressMatch.ZipCity:
					return "numéro postal et localité identiques";

				default:
					throw new NotImplementedException ();
			}
		}


		private static string GetTextForSexMatch(bool? match)
		{
			switch (match)
			{
				case true:
					return "identique";

				case false:
					return "différent";

				case null:
					return "indéterminé";

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
				return "indéterminé";
			}
		}


		private static void ProcessHouseholdMatches(BusinessContextManager businessContextManager, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches, Dictionary<EervPerson, EntityKey> newEntities, IEnumerable<EervHousehold> eervHouseholds)
		{
			Action<BusinessContext> action = b =>
			{
				EervParishDataImporter.ProcessHouseholdMatches (b, matches, newEntities, eervHouseholds);

				b.SaveChanges ();
			};

			businessContextManager.Execute (action);			
		}


		private static void ProcessHouseholdMatches(BusinessContext businessContext, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches, Dictionary<EervPerson, EntityKey> newEntities, IEnumerable<EervHousehold> eervHouseholds)
		{
			var aiderPersons = businessContext.GetAllEntities<AiderPersonEntity> ();
			businessContext.GetAllEntities<AiderHouseholdEntity> ();
			businessContext.GetAllEntities<AiderAddressEntity> ();
			businessContext.GetAllEntities<AiderTownEntity> ();

			EervParishDataImporter.ProcessHouseholdMatches (aiderPersons, eervHouseholds, businessContext, matches, newEntities);
		}
  

		private static void ProcessHouseholdMatches(IEnumerable<AiderPersonEntity> aiderPersons, IEnumerable<EervHousehold> eervHouseholds, BusinessContext businessContext, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches, Dictionary<EervPerson, EntityKey> newEntities)
		{
			var aiderHouseholdToPersons = EervParishDataImporter.GetAiderHouseoldToPersons(aiderPersons);

			foreach (var eervHousehold in eervHouseholds)
			{
				EervParishDataImporter.ProcessHouseholdMatch(businessContext, matches, newEntities, eervHousehold, aiderHouseholdToPersons);
			}
		}


		private static Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> GetAiderHouseoldToPersons(IEnumerable<AiderPersonEntity> aiderPersons)
		{
			var aiderHouseholdToPersons = new Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> ();

			foreach (var aiderPerson in aiderPersons)
			{
				foreach (var aiderHousehold in aiderPerson.GetHouseholds ())
				{
					List<AiderPersonEntity> aiderMembers;

					if (!aiderHouseholdToPersons.TryGetValue (aiderHousehold, out aiderMembers))
					{
						aiderMembers = new List<AiderPersonEntity> ();

						aiderHouseholdToPersons[aiderHousehold] = aiderMembers;
					}

					aiderMembers.Add (aiderPerson);
				}
			}

			return aiderHouseholdToPersons;
		}


		private static void ProcessHouseholdMatch(BusinessContext businessContext, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches, Dictionary<EervPerson, EntityKey> newEntities, EervHousehold eervHousehold, Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> aiderHouseholdToAiderPersons)
		{
			var eervToAiderPersons = EervParishDataImporter.GetEervToAiderPersons (businessContext, matches, newEntities, eervHousehold);
			var aiderHouseholds = EervParishDataImporter.GetAiderHouseholds (eervToAiderPersons.Values);

			if (aiderHouseholds.Count == 0)
			{
				EervParishDataImporter.CreateHousehold (businessContext, eervHousehold, eervToAiderPersons, aiderHouseholdToAiderPersons);
			}
			else if (aiderHouseholds.Count == 1)
			{
				EervParishDataImporter.ExpandHousehold (eervHousehold, eervToAiderPersons, aiderHouseholds.Single (), aiderHouseholdToAiderPersons);
			}
			else
			{
				EervParishDataImporter.CombineHouseholds (businessContext, eervHousehold, eervToAiderPersons, aiderHouseholds, aiderHouseholdToAiderPersons);
			}
		}


		private static List<AiderHouseholdEntity> GetAiderHouseholds(IEnumerable<AiderPersonEntity> aiderPersons)
		{
			return aiderPersons.SelectMany (m => m.GetHouseholds ()).Distinct ().ToList ();
		}


		private static Dictionary<EervPerson, AiderPersonEntity> GetEervToAiderPersons(BusinessContext businessContext, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches, Dictionary<EervPerson, EntityKey> newEntities, EervHousehold eervHousehold)
		{
			var eervToAiderPersons = new Dictionary<EervPerson, AiderPersonEntity>();

			foreach (var eervPerson in eervHousehold.Members)
			{
				var match = matches[eervPerson];

				var entityKey = match.Count > 0
					? match[0].Item1
					: newEntities[eervPerson];

				var aiderPerson = businessContext.DataContext.ResolveEntity(entityKey);

				eervToAiderPersons[eervPerson] = (AiderPersonEntity) aiderPerson;
			}

			return eervToAiderPersons;
		}


		private static void CreateHousehold(BusinessContext businessContext, EervHousehold eervHousehold, Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons, Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> aiderHouseholdToAiderPersons)
		{
			var aiderHousehold = businessContext.CreateEntity<AiderHouseholdEntity>();
			aiderHouseholdToAiderPersons[aiderHousehold] = new List<AiderPersonEntity> ();

			EervParishDataImporter.ExpandHousehold (eervHousehold, eervToAiderPersons, aiderHousehold, aiderHouseholdToAiderPersons);

			EervParishDataImporter.CombineAddress (businessContext, aiderHousehold.Address, eervHousehold.Address);
		}


		private static void ExpandHousehold(EervHousehold eervHousehold, Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons, AiderHouseholdEntity aiderHousehold, Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> aiderHouseholdToAiderPersons)
		{
			List<AiderPersonEntity> aiderPersons;

			if (!aiderHouseholdToAiderPersons.TryGetValue (aiderHousehold, out aiderPersons))
			{
				aiderPersons = new List<AiderPersonEntity> ();
			}

			var personsToAdd = eervToAiderPersons.Values.Except (aiderPersons).ToList ();

			foreach (var person in personsToAdd)
			{
				person.Household1 = aiderHousehold;
				aiderPersons.Add (person);
			}

			EervParishDataImporter.CombineHeadData (eervHousehold, eervToAiderPersons, aiderHousehold);
			EervParishDataImporter.CombineComments (aiderHousehold, eervHousehold.Remarks);
			EervParishDataImporter.CombineCoordinates (aiderHousehold.Address, eervHousehold.Coordinates);
		}


		private static void CombineHouseholds(BusinessContext businessContext, EervHousehold eervHousehold, Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons, List<AiderHouseholdEntity> aiderHouseholds, Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> aiderHouseholdToAiderPersons)
		{
			var effectiveHouseholds = aiderHouseholds
				.GroupBy (h => h.Address, AiderAddressEntityComparer.Instance);

			var mainHouseholds = new List<AiderHouseholdEntity> ();

			foreach (var effectiveHousehold in effectiveHouseholds)
			{
				var households = effectiveHousehold.ToList();

				var mainEffectiveHousehold = EervParishDataImporter.CombineEffectiveHouseholds (businessContext, households, aiderHouseholdToAiderPersons);

				mainHouseholds.Add (mainEffectiveHousehold);
			}

			var mainHousehold = EervParishDataImporter.GetMainHousehold (mainHouseholds, aiderHouseholdToAiderPersons);

			EervParishDataImporter.ExpandHousehold (eervHousehold, eervToAiderPersons, mainHousehold, aiderHouseholdToAiderPersons); 
		}

		
		private static AiderHouseholdEntity CombineEffectiveHouseholds(BusinessContext businessContext, List<AiderHouseholdEntity> aiderHouseholds, Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> aiderHouseholdToAiderPersons)
		{
			var mainHousehold = EervParishDataImporter.GetMainHousehold (aiderHouseholds, aiderHouseholdToAiderPersons);
			var secondaryHouseholds = aiderHouseholds.Where (h => h!= mainHousehold);
			
			foreach (var secondaryHousehold in secondaryHouseholds)
			{
				EervParishDataImporter.CombineEffectiveHouseholds (businessContext, mainHousehold, secondaryHousehold, aiderHouseholdToAiderPersons);
			}

			return mainHousehold;
		}

  
		private static AiderHouseholdEntity GetMainHousehold(List<AiderHouseholdEntity> aiderHouseholds, Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> aiderHouseholdToAiderPersons)
		{
			return aiderHouseholds
				.OrderByDescending(h => aiderHouseholdToAiderPersons[h].Count)
				.First();
		}


		private static void CombineEffectiveHouseholds(BusinessContext businessContext, AiderHouseholdEntity mainHousehold, AiderHouseholdEntity secondaryHousehold, Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> aiderHouseholdToAiderPersons)
		{
			var secondaryHouseholdMembers = aiderHouseholdToAiderPersons[secondaryHousehold];

			foreach (var person in secondaryHouseholdMembers)
			{
				if (person.Household1 == secondaryHousehold)
				{
					person.Household1 = mainHousehold;
				}

				if (person.Household2 == secondaryHousehold)
				{
					person.Household2 = mainHousehold;
				}
			}

			aiderHouseholdToAiderPersons[mainHousehold].AddRange (secondaryHouseholdMembers);
			aiderHouseholdToAiderPersons.Remove (secondaryHousehold);

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

			businessContext.DeleteEntity(secondaryHousehold);

			// TODO Copy info of deleted households to the proper main household ?
		}

		private static void CombineAddress(BusinessContext businessContext, AiderAddressEntity aiderAddress, EervAddress eervAddress)
		{
			if (string.IsNullOrEmpty (eervAddress.ZipCode) || string.IsNullOrEmpty (eervAddress.Town))
			{
				// We have an invalid address or no address at all, thus we ignore it.

				return;
			}

			if (eervAddress.ZipCode.Length == 4 && eervAddress.ZipCode.IsInteger ())
			{
				EervParishDataImporter.CombineSwissAddress (businessContext, eervAddress, aiderAddress);
			}
			else
			{
				EervParishDataImporter.CombineForeignAddress (businessContext, eervAddress, aiderAddress);
			}
		}

  
		private static void CombineSwissAddress(BusinessContext businessContext, EervAddress eervAddress, AiderAddressEntity aiderAddress)
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

			var switzerland = AiderCountryEntity.Find (businessContext, "CH", "Suisse");
			var town = AiderTownEntity.FindOrCreate (businessContext, switzerland, zipCode, townName);

			aiderAddress.Town = town;
		}


		private static void CombineForeignAddress(BusinessContext businessContext, EervAddress eervAddress, AiderAddressEntity aiderAddress)
		{
			aiderAddress.AddressLine1 = eervAddress.FirstAddressLine;
			aiderAddress.Street = eervAddress.StreetName;
			aiderAddress.HouseNumber = eervAddress.HouseNumber;
			aiderAddress.HouseNumberComplement = eervAddress.HouseNumberComplement;
			aiderAddress.Town = AiderTownEntity.FindOrCreate (businessContext, eervAddress.ZipCode, eervAddress.Town);
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
					var text = "Numéro de téléphone supplémentaire: " + privatePhoneNumber;

					EervParishDataImporter.CombineComments (address, text);
				}
			}

			var professionalPhoneNumber = coordinates.ProfessionalPhoneNumber;

			if (!string.IsNullOrWhiteSpace (professionalPhoneNumber))
			{
				var text = "Numéro de téléphone professionel: " + professionalPhoneNumber;

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
					var text = "Numéro de fax supplémentaire: " + faxNumber;

					EervParishDataImporter.CombineComments (address, text);
				}
			}
		}


		private static void CombineHeadData(EervHousehold eervHousehold, Dictionary<EervPerson, AiderPersonEntity> eervToAiderPersons, AiderHouseholdEntity aiderHousehold)
		{
			var headsA = eervHousehold.Heads.Select (p => eervToAiderPersons[p]).ToList ();
			var headsB = aiderHousehold.GetHeads ();

			var childrenA = eervHousehold.Children.Select (p => eervToAiderPersons[p]).ToList ();

			var heads = headsA.Concat (headsB).Distinct ().Except (childrenA).ToList ();

			// If we have more that two heads, we have a mistake somewhere (maybe a false positive
			// or something) and we remove the heads from the eerv data until we have the right
			// number.

			for (int i = headsA.Count - 1; heads.Count > 2; i--)
			{
				heads.Remove (headsA[i]);
			}

			aiderHousehold.Head1 = heads.Count > 0 ? heads[0] : null;
			aiderHousehold.Head2 = heads.Count > 1 ? heads[1] : null;
		}


	}


}