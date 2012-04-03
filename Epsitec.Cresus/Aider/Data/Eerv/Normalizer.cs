using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;


namespace Epsitec.Aider.Data.Eerv
{
	
	
	internal static class Normalizer
	{


		public static Tuple<Dictionary<NormalizedHousehold, EervHousehold>, Dictionary<NormalizedPerson, EervPerson>> Normalize(IEnumerable<EervHousehold> households)
		{
			var normalizedPersons = new Dictionary<NormalizedPerson, EervPerson> ();
			var normalizedHouseholds = households.ToDictionary (h => Normalizer.Normalize (h, normalizedPersons));

			return Tuple.Create (normalizedHouseholds, normalizedPersons);
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
				Origins = Normalizer.NormalizeText (person.Origins),
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


		public static Tuple<Dictionary<NormalizedHousehold, AiderHouseholdEntity>, Dictionary<NormalizedPerson, AiderPersonEntity>> Normalize(IEnumerable<AiderHouseholdEntity> households, IEnumerable<AiderPersonEntity> persons)
		{
			var householdToChildren = Normalizer.GetHouseholdToChildren (persons);

			var personsToNormalizedPersons = new Dictionary<AiderPersonEntity, NormalizedPerson> ();

			var normalizedHouseholds = households
				.ToDictionary (h => Normalizer.Normalize (h, personsToNormalizedPersons, householdToChildren));

			var normalizedPersons = personsToNormalizedPersons
				.ToDictionary (x => x.Value, x => x.Key);

			return Tuple.Create (normalizedHouseholds, normalizedPersons);
		}


		private static Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> GetHouseholdToChildren(IEnumerable<AiderPersonEntity> persons)
		{
			var householdToChildren = new Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> ();

			foreach (var person in persons)
			{
				foreach (var household in person.GetHouseholds ())
				{
					if (household.Head1 != person && household.Head2 != person)
					{
						List<AiderPersonEntity> children;

						if (!householdToChildren.TryGetValue (household, out children))
						{
							children = new List<AiderPersonEntity> ();

							householdToChildren[household] = children;
						}

						children.Add (person);
					}
				}
			}

			return householdToChildren;
		}


		private static NormalizedHousehold Normalize(AiderHouseholdEntity household, Dictionary<AiderPersonEntity, NormalizedPerson> normalizedPersons, Dictionary<AiderHouseholdEntity, List<AiderPersonEntity>> householdToChildren)
		{
			var normalizedHousehold = new NormalizedHousehold ()
			{
				Address = Normalizer.Normalize (household.Address)
			};

			if (household.Head1.IsNotNull ())
			{
				normalizedHousehold.Head1 = Normalizer.Normalize (household.Head1, normalizedPersons);
			}

			if (household.Head2.IsNotNull ())
			{
				normalizedHousehold.Head2 = Normalizer.Normalize (household.Head2, normalizedPersons);
			}

			List<AiderPersonEntity> children;

			if (!householdToChildren.TryGetValue (household, out children))
			{
				children = new List<AiderPersonEntity> ();
			}

			normalizedHousehold.Children = children
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


		private static NormalizedPerson Normalize(AiderPersonEntity person, Dictionary<AiderPersonEntity, NormalizedPerson> normalizedPersons)
		{
			NormalizedPerson normalizedPerson;

			if (!normalizedPersons.TryGetValue (person, out normalizedPerson))
			{
				normalizedPerson = Normalizer.Normalize (person);

				normalizedPersons[person] = normalizedPerson;
			}

			return normalizedPerson;
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
				Origins = Normalizer.NormalizeText (eChPerson.Origins),
			};
		}


		private static NormalizedAddress Normalize(AiderAddressEntity address)
		{
			return new NormalizedAddress ()
			{
				Street = Normalizer.NormalizeStreetName (address.Street),
				HouseNumber = address.HouseNumber,
				ZipCode = address.Town.SwissZipCode.Value,
				Town = Normalizer.NormalizeText (address.Town.Name),
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


		private static string NormalizeText(string data)
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
