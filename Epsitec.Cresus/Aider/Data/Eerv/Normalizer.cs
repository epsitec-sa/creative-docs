using Epsitec.Aider.Entities;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Data.Platform;

using System.Collections.Generic;

using System.Linq;

using System.Text;


namespace Epsitec.Aider.Data.Eerv
{
	
	
	internal static class Normalizer
	{


		public static Dictionary<NormalizedPerson, EervPerson> Normalize(IEnumerable<EervHousehold> households)
		{
			var normalizedPersons = new Dictionary<NormalizedPerson, EervPerson> ();

			foreach (var household in households)
			{
				Normalizer.Normalize (household, normalizedPersons);
			}

			return normalizedPersons;
		}


		private static NormalizedHousehold Normalize(EervHousehold household, Dictionary<NormalizedPerson, EervPerson> normalizedPersons)
		{
			var normalizedHousehold = new NormalizedHousehold ()
			{
				Address = Normalizer.Normalize (household.Address)
			};

			if (household.Head1 != null)
			{
				normalizedHousehold.Head1 = Normalizer.Normalize (household.Head1, normalizedPersons);
			}

			if (household.Head2 != null)
			{
				normalizedHousehold.Head2 = Normalizer.Normalize (household.Head2, normalizedPersons);
			}

			normalizedHousehold.Children = household.Children
				.Select (c => Normalizer.Normalize (c, normalizedPersons))
				.ToList ();

			foreach (var member in normalizedHousehold.Members)
			{
				if (member.Households == null)
				{
					member.Households = new List<NormalizedHousehold> ();
				}

				member.Households.Add (normalizedHousehold);
			}

			return normalizedHousehold;
		}


		private static NormalizedPerson Normalize(EervPerson person, Dictionary<NormalizedPerson, EervPerson> normalizedPersons)
		{
			var normalizedPerson = new NormalizedPerson ()
			{
				Firstnames = Normalizer.NormalizeComposedName (person.Firstname),
				Lastnames = Normalizer.NormalizeComposedName (person.Lastname),
				DateOfBirth = person.DateOfBirth,
				Sex = person.Sex,
			};

			normalizedPersons[normalizedPerson] = person;

			return normalizedPerson;
		}


		private static NormalizedAddress Normalize(EervAddress address)
		{
			return new NormalizedAddress ()
			{
				Street = Normalizer.NormalizeStreetName (address.StreetName),
				HouseNumber = address.HouseNumber,
				ZipCode = InvariantConverter.ParseInt (address.ZipCode),
				Town = Normalizer.NormalizeText (address.Town),
			};
		}


		public static Dictionary<NormalizedPerson, EntityKey> Normalize(CoreDataManager coreDataManager)
		{
			var keyToHouseholds = new Dictionary<EntityKey, NormalizedHousehold> ();
			var personToKeys = new Dictionary<NormalizedPerson, EntityKey> ();

			AiderEnumerator.Execute
			(
				coreDataManager,
				(b, p) => Normalizer.Normalize (b, p, personToKeys, keyToHouseholds)
			);

			return personToKeys;
		}


		private static void Normalize(BusinessContext businessContext, IEnumerable<AiderPersonEntity> persons, Dictionary<NormalizedPerson, EntityKey> personToKeys, Dictionary<EntityKey, NormalizedHousehold> keyToHouseholds)
		{
			var dataContext = businessContext.DataContext;

			foreach (var person in persons)
			{
				var personKey = dataContext.GetNormalizedEntityKey (person).Value;
				var normalizedPerson = Normalizer.Normalize (person);

				personToKeys[normalizedPerson] = personKey;

				Normalizer.Normalize (person, normalizedPerson, person.Household1, keyToHouseholds, dataContext);
				Normalizer.Normalize (person, normalizedPerson, person.Household2, keyToHouseholds, dataContext);
			}
		}


		private static void Normalize(AiderPersonEntity person, NormalizedPerson normalizedPerson, AiderHouseholdEntity household, Dictionary<EntityKey, NormalizedHousehold> keyToHouseholds, DataContext dataContext)
		{
			if (household.IsNotNull ())
			{
				var householdKey = dataContext.GetNormalizedEntityKey (household).Value;

				NormalizedHousehold normalizedHousehold;

				if (!keyToHouseholds.TryGetValue (householdKey, out normalizedHousehold))
				{
					normalizedHousehold = Normalizer.Normalize (household);

					keyToHouseholds[householdKey] = normalizedHousehold;
				}

				normalizedPerson.Households.Add (normalizedHousehold);

				bool isChild = true;

				if (household.Head1 == person)
				{
					normalizedHousehold.Head1 = normalizedPerson;
					
					isChild = false;
				}

				if (household.Head2 == person)
				{
					normalizedHousehold.Head2 = normalizedPerson;

					isChild = false;
				}

				if (isChild)
				{
					normalizedHousehold.Children.Add (normalizedPerson);
				}
			}
		}


		private static NormalizedHousehold Normalize(AiderHouseholdEntity household)
		{
			return new NormalizedHousehold ()
			{
				Address = Normalizer.Normalize (household.Address),
				Children = new List<NormalizedPerson> ()
			};
		}


		private static NormalizedPerson Normalize(AiderPersonEntity person)
		{
			var eChPerson = person.eCH_Person;

			return new NormalizedPerson ()
			{
				Firstnames = Normalizer.NormalizeComposedName (eChPerson.PersonFirstNames),
				Lastnames = Normalizer.NormalizeComposedName (eChPerson.PersonOfficialName),
				DateOfBirth = eChPerson.PersonDateOfBirth,
				Sex = eChPerson.PersonSex,
				Households = new List<NormalizedHousehold> (),
			};
		}


		private static NormalizedAddress Normalize(AiderAddressEntity address)
		{
			var town = address.Town;

			return new NormalizedAddress ()
			{
				Street = Normalizer.NormalizeStreetName (address.Street),
				HouseNumber = address.HouseNumber,
				ZipCode = town.SwissZipCode ?? 0,
				Town = Normalizer.NormalizeText (town.Name),
			};
		}


		private static string NormalizeStreetName(string streetname)
		{
			var result = SwissPostStreet.NormalizeStreetName (streetname);

			result = Normalizer.NormalizeText (result);

			return result;
		}


		private static string[] NormalizeComposedName(string name)
		{
			return Normalizer.NormalizeText (name).Split (new char[] { ' ' });
		}


		public static string NormalizeText(string data)
		{
			if (data == null)
			{
				return null;
			}

			var result = data;

			result = StringUtils.RemoveDiacritics (result);
			result = result.ToLowerInvariant ();
			result = Normalizer.RemoveUnwantedChars (result);

			return result;
		}


		private static string RemoveUnwantedChars(string data)
		{
			var sb = new StringBuilder (data.Length);

			var addSpace = false;

			for (int i = 0; i < data.Length; i++)
			{
				var c = data[i];

				if (char.IsLetterOrDigit (c))
				{
					sb.Append (c);

					addSpace = true;
				}
				else if (addSpace && i < data.Length - 1)
				{
					sb.Append (" ");

					addSpace = false;
				}
			}

			return sb.ToString ();
		}


	}


}
