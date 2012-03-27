using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

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
			// TODO Import household data

			var matches = EervParishDataImporter.FindMatches (businessContextManager, eervParishData);
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);

			EervParishDataImporter.ProcessMatches (businessContextManager, parishName, matches);
			GC.Collect (GC.MaxGeneration, GCCollectionMode.Forced);
		}


		private static Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> FindMatches(BusinessContextManager businessContextManager, EervParishData eervParishData)
		{
			Func<BusinessContext, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>>> function = b =>
			{
				return EervParishDataImporter.FindMatches (b, eervParishData);
			};

			return businessContextManager.Execute (function);
		}


		private static Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> FindMatches(BusinessContext businessContext, EervParishData eervParishData)
		{
			var w = System.Diagnostics.Stopwatch.StartNew ();
			
			var aiderHouseholds = businessContext.GetAllEntities<AiderHouseholdEntity> ();
			var aiderPersons = businessContext.GetAllEntities<AiderPersonEntity> ();

			// NOTE Here we fetch all this stuff in memory at once, so we don't make gazillions of
			// requests to the database later, by fetching them one by one.
			businessContext.GetAllEntities<eCH_PersonEntity> ();
			businessContext.GetAllEntities<AiderAddressEntity> ();
			businessContext.GetAllEntities<AiderTownEntity> ();

			w.Stop ();

			var dataContext = businessContext.DataContext;

			var matches = EervParishDataImporter.FindMatches (eervParishData, aiderHouseholds, aiderPersons);

			return matches.ToDictionary
			(
				kvp => kvp.Key,
				kvp => kvp.Value.Select (m => Tuple.Create (dataContext.GetNormalizedEntityKey (m.Item1).Value, m.Item2)).ToList ()
			);
		}


		private static Dictionary<EervPerson, List<Tuple<AiderPersonEntity, MatchData>>> FindMatches(EervParishData eervParishData, IEnumerable<AiderHouseholdEntity> aiderHouseholds, IEnumerable<AiderPersonEntity> aiderPersons)
		{
			var normalizedAiderData = Normalizer.Normalize (aiderHouseholds, aiderPersons);
			var normalizedAiderPersons = normalizedAiderData.Item2;

			var normalizedEervData = Normalizer.Normalize (eervParishData.Households);
			var normalizedEervPersons = normalizedEervData.Item2;

			var matches = EervParishDataMatcher.FindMatches (normalizedEervPersons.Keys, normalizedAiderPersons.Keys);

			return matches.ToDictionary
			(
				kvp => normalizedEervPersons[kvp.Key],
				kvp => kvp.Value.Select (m => Tuple.Create (normalizedAiderPersons[m], new MatchData ())).ToList ()
			);
		}


		private static void ProcessMatches(BusinessContextManager businessContextManager, string parishName, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches)
		{
			Action<BusinessContext> action = b =>
			{
				EervParishDataImporter.ProcessMatches (b, parishName, matches);
			};

			businessContextManager.Execute (action);
		}


		private static void ProcessMatches(BusinessContext businessContext, string parishName, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches)
		{
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
				}
			}
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

				EervParishDataImporter.AddEmail (address, email);
				EervParishDataImporter.AddMobilePhoneNumber (address, mobile);

				aiderPerson.AdditionalAddresses.Add (address);
			}
		}


		private static void AddEmail(AiderAddressEntity address, string email)
		{
			if (!string.IsNullOrEmpty (email))
			{
				address.Email = email;
			}
		}


		private static void AddMobilePhoneNumber(AiderAddressEntity address, string mobilePhoneNumber)
		{
			if (!string.IsNullOrEmpty (mobilePhoneNumber))
			{
				var parsedNumber = TwixTel.ParsePhoneNumber (mobilePhoneNumber);

				if (TwixTel.IsValidPhoneNumber (parsedNumber, false))
				{
					address.Mobile = parsedNumber;
				}
				else
				{
					var text = "Téléphone invalide ou non reconnu par le système : " + mobilePhoneNumber;
					
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
				text += "\n - Nom de famille : " + EervParishDataImporter.GetTextForNameMatch (match.Lastname);
				text += "\n - Prénom : " + EervParishDataImporter.GetTextForNameMatch (match.Firstname);
				text += "\n - Sexe : " + EervParishDataImporter.GetTextForSexMatch (match.Sex);
				text += "\n - Date de naissance : " + EervParishDataImporter.GetTextForDateOfBirthMatch (match.DateOfBirth);
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
					return "la rue, le numéro dans la rue, le numéro postal et la localité correspondent";

				case AddressMatch.None:
					return "l'adresse ne correspond pas";

				case AddressMatch.StreetZipCity:
					return "la rue, le numéro postal et la localité correspondent";

				case AddressMatch.ZipCity:
					return "le numéro postal et la localité correspondent";

				default:
					throw new NotImplementedException ();
			}
		}


		private static string GetTextForNameMatch(NameMatch match)
		{
			switch (match)
			{
				case NameMatch.Full:
					return "le nom correspond";

				case NameMatch.OrderedPartial:
					return "une partie du nom manque (nom composé)";

				case NameMatch.Partial:
					return "une partie du nom manque ou est dans le désordre (nom composé)";

				default:
					throw new NotImplementedException ();
			}
		}


		private static string GetTextForSexMatch(bool? match)
		{
			switch (match)
			{
				case true:
					return "le sexe correspond";

				case false:
					return "le sexe ne correspond pas";

				case null:
					return "la correspondance n'a pas pu être établie (sexe manquant)";

				default:
					throw new NotImplementedException ();
			}
		}


		private static string GetTextForDateOfBirthMatch(bool? match)
		{
			switch (match)
			{
				case true:
					return "la date de naissance correspond";

				case false:
					return "la date de naissance ne correspond pas";

				case null:
					return "la correspondance n'a pas pu être établie (date manquante)";

				default:
					throw new NotImplementedException ();
			}
		}


	}


}