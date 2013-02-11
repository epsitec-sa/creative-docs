using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Data.Platform;

using System;

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
				var normalizedAdult1 = Normalizer.Normalize (household.Head1, normalizedPersons);

				normalizedHousehold.Heads.Add (normalizedAdult1);
			}

			if (household.Head2 != null)
			{
				var normalizedAdult2 = Normalizer.Normalize (household.Head2, normalizedPersons);

				normalizedHousehold.Heads.Add (normalizedAdult2);
			}

			foreach (var child in household.Children)
			{
				var normalizedChild = Normalizer.Normalize (child, normalizedPersons);

				normalizedHousehold.Children.Add (normalizedChild);
			}

			foreach (var member in normalizedHousehold.Members)
			{
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
				Sex = person.Sex
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
			var keyToPersons = new Dictionary<EntityKey, NormalizedPerson> ();

			var personToKeys = new Dictionary<NormalizedPerson, EntityKey> ();

			AiderEnumerator.Execute
			(
				coreDataManager,
				(b, c) => Normalizer.Normalize (b, c, personToKeys, keyToHouseholds, keyToPersons)
			);

			return personToKeys;
		}


		private static void Normalize(BusinessContext businessContext, IEnumerable<AiderContactEntity> contacts, Dictionary<NormalizedPerson, EntityKey> personToKeys, Dictionary<EntityKey, NormalizedHousehold> keyToHouseholds, Dictionary<EntityKey, NormalizedPerson> keyToPersons)
		{
			var dataContext = businessContext.DataContext;

			foreach (var contact in contacts)
			{
				Normalizer.Normalize (dataContext, contact, personToKeys, keyToHouseholds, keyToPersons);
			}
		}


		private static void Normalize(DataContext dataContext, AiderContactEntity contact, Dictionary<NormalizedPerson, EntityKey> personToKeys, Dictionary<EntityKey, NormalizedHousehold> keyToHouseholds, Dictionary<EntityKey, NormalizedPerson> keyToPersons)
		{
			var type = contact.ContactType;

			if (type != ContactType.PersonAddress && type != ContactType.PersonHousehold)
			{
				return;
			}

			var person = contact.Person;

			if (person.IsNull ())
			{
				return;
			}

			var normalizedPerson = Normalizer.Normalize (dataContext, person, personToKeys, keyToPersons);

			if (type == ContactType.PersonHousehold)
			{
				var household = contact.Household;

				if (household.IsNotNull ())
				{
					var normalizedHousehold = Normalizer.Normalize (dataContext, household, keyToHouseholds);

					normalizedPerson.Households.Add (normalizedHousehold);

					if (contact.HouseholdRole == HouseholdRole.Head)
					{
						normalizedHousehold.Heads.Add (normalizedPerson);
					}
					else
					{
						normalizedHousehold.Children.Add (normalizedPerson);
					}
				}
			}
			else
			{
				var address = contact.GetAddress ();

				if (address.IsNotNull () && address.Town.IsNotNull ())
				{
					var normalizedAddress = Normalizer.Normalize (address);

					normalizedPerson.PersonalAddresses.Add (normalizedAddress);
				}
			}
		}


		private static NormalizedPerson Normalize(DataContext dataContext, AiderPersonEntity person, Dictionary<NormalizedPerson, EntityKey> personToKeys, Dictionary<EntityKey, NormalizedPerson> keyToPersons)
		{
			var personKey = dataContext.GetNormalizedEntityKey (person).Value;

			NormalizedPerson normalizedPerson;

			if (!keyToPersons.TryGetValue (personKey, out normalizedPerson))
			{
				normalizedPerson = Normalizer.Normalize (person);

				keyToPersons[personKey] = normalizedPerson;
				personToKeys[normalizedPerson] = personKey;
			}

			return normalizedPerson;
		}


		private static NormalizedHousehold Normalize(DataContext dataContext, AiderHouseholdEntity household, Dictionary<EntityKey, NormalizedHousehold> keyToHouseholds)
		{
			var householdKey = dataContext.GetNormalizedEntityKey (household).Value;

			NormalizedHousehold normalizedHousehold;

			if (!keyToHouseholds.TryGetValue (householdKey, out normalizedHousehold))
			{
				normalizedHousehold = Normalizer.Normalize (household);

				keyToHouseholds[householdKey] = normalizedHousehold;
			}

			return normalizedHousehold;
		}


		private static NormalizedHousehold Normalize(AiderHouseholdEntity household)
		{
			return new NormalizedHousehold ()
			{
				Address = Normalizer.Normalize (household.Address)
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
