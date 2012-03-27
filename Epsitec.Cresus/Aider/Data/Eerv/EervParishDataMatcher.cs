using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Text;

using Epsitec.Common.Types;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;

using System.Text;


namespace Epsitec.Aider.Data.Eerv
{


	/// <summary>
	/// The purpose of this class is to match a group of persons to another group of persons.
	///
	/// Beside obvious stuff, we have the following assumptions
	/// - The persons of the first group are in exactly one household
	/// - The persons of the second group are in at least one household
	/// - There can be duplicates in either group. By duplicates I mean a person that is defined
	///   more than once (maybe with some slight difference in the name, date of birth, address,
	///   household, etc. ) but that is really the same person.
	/// - There can be mistakes in the names, the addresses, the date of birth, the gender
	///   information.
	/// - Some data might be missing, like dates of birth, information about gender, parts of names,
	///   parts of addresses
	/// - Some data might be outdated, like addresses.
	/// - The two groups are not supposed to match exactly. There is a lot more persons in the
	///   second one than in the first, and it is known that there are persons in the first one that
	///   are not in the second one.
	///   
	/// Given that, I implemented the following algorithm. It is composed of two parts.
	/// 
	/// The first part is basically a loop which finds matches using criteria that are relaxed at
	/// each iteration of the loop. Moreover, at each iteration of the loop, each household of each
	/// matched persons are investigated in order to find matches in them.
	/// The idea behind this algorithm is that if two persons are in a household in one group, they
	/// are likely to be in the same household in the other group. The other idea is that by
	/// starting with restrictive criteria that are relaxed over time, we avoid a lot of false
	/// positive, because we don't consider relaxed criteria for persons that are matched with a
	/// strong one.
	/// The criteria are the following.
	/// - For the firstnames and lastnames, we use the Jaro-Winkler distance to detect their
	///   similarity. This algorithm is supposed to be good at fuzzy string matching for names.
	/// - For the date of birth, we use the Jaro distance to detect their similarity. I choose the
	///   Jaro distance instead of the Jaro-Winkler distance because the Jaro-Winkler distance gives
	///   a higher similarity for two strings with a common prefix and this would probably give bad
	///   result for date similarity.
	/// - For the gender information, we simply compare one gender with the other.
	/// - For the address comparison, we simply compare the data of each address and this gives us a
	///   indication whether the addresses matche completly, or only street name, the town and the
	///   zip code or only the town and the zip code or nothing.
	/// The sequence of criteria used is based on experiment I made with the data of the parish of
	/// Morges and might required to be tweaked for other data.
	/// There are some optimizations made here by using dictionnaries to drastically reduce the
	/// number of comparisons that must be made to find fuzzy matches.
	/// 
	/// The second part is here to address the major drawback with the Jaro-Winkler distance. It is
	/// very good in the majority of the cases, but it gives bad results if one name is long and the
	/// other is short, or if composed names are out of order.
	/// These case are however surprisingly common in the data of the parish of Morges. There is the
	/// case of a person hat has a composed name in the ECh data and only one of it is used in the
	/// parish Data. There is also the case of women whose name is their original name in the ECh
	/// data but whose name if the name of their husband or both names together in the data of the
	/// parish.
	/// In order to address these shortcomings, I look at each person that has at least one of the
	/// firstname and one of the lastname. Of course, this gives a lot of false positives, so I
	/// filter those results to ensure that the matches have the same sex and either the same date
	/// of birth or the same address. Experimentally, this lead to few false positive, even if I
	/// would have expected that such an aggressive technique would lead to much more. This is
	/// probably because most of the persons are already matched by the first part of the algorithm.
	/// 
	/// There is a third element in the algorithm. I had a few false positives in the sense that
	/// persons in the ECh group where matched by two persons from the EERV group. In half of the
	/// cases this was normal as it was because of duplicates in the EERV data. However, for some
	/// cases, one of the two person was clearly a false positive. This was because I was too
	/// aggressive in the second part of the algorithm and in the part where I makes matches with
	/// the household data. It is common to have several person with similar names in a familiy,
	/// say a father names his son with a name followed by his name, or two persons have similar
	/// names such as Martin and Mario. In order to avoid these false positives, I keep track of the
	/// persons that have been matched and they cannot be matched again the the second part of the
	/// algorithm or in the part where first part of the algorithm where the matches are made based
	/// on the household members. 
	/// </summary>
	internal static class EervParishDataMatcher
	{


		public static Dictionary<NormalizedPerson, List<NormalizedPerson>> FindMatches(IEnumerable<NormalizedPerson> eervPersons, IEnumerable<NormalizedPerson> aiderPersons)
		{
			var todo = new HashSet<NormalizedPerson> (eervPersons);
			var done = new Dictionary<NormalizedPerson, List<NormalizedPerson>> ();
			var matched = new HashSet<NormalizedPerson> ();

			EervParishDataMatcher.FindMatchesWithFuzzyMethod(aiderPersons, todo, done, matched);
			EervParishDataMatcher.FindMatchesWithSplitMethod(aiderPersons, todo, done, matched);
			EervParishDataMatcher.AssignUnmatchedPersons(todo, done);

			EervParishDataMatcher.Warn (done);

			return done;
		}


		private static void Warn(Dictionary<NormalizedPerson, List<NormalizedPerson>> done)
		{
			var sb = new StringBuilder ();

			foreach (var match in done.Where (m => m.Value.Count > 1))
			{
				sb.AppendLine ("======================================================================");
				sb.AppendLine ("WARNING: multiple matches for person:");
				sb.AppendLine (match.Key.ToString ());
				sb.AppendLine ("----------------------------------------------------------------------");

				foreach (var p in match.Value)
				{
					sb.AppendLine (p.ToString ());
				}
			}

			var reversedDone = done
				.SelectMany (m => m.Value.Select (p => Tuple.Create (p, m.Key)))
				.GroupBy (t => t.Item1)
				.ToDictionary (g => g.Key, g => g.Select (t => t.Item2).ToList ());

			foreach (var match in reversedDone.Where (m => m.Value.Count > 1))
			{
				sb.AppendLine ("======================================================================");
				sb.AppendLine ("WARNING: person has been matched by multiple persons:");
				sb.AppendLine (match.Key.ToString ());
				sb.AppendLine ("----------------------------------------------------------------------");

				foreach (var p in match.Value)
				{
					sb.AppendLine (p.ToString ());
				}
			}

			Debug.WriteLine (sb.ToString ());
		}

  
		private static void FindMatchesWithFuzzyMethod(IEnumerable<NormalizedPerson> aiderPersons, HashSet<NormalizedPerson> todo, Dictionary<NormalizedPerson, List<NormalizedPerson>> done, HashSet<NormalizedPerson> matched)
		{
			var namesToAiderPersons = EervParishDataMatcher.GroupPersonsByNames(aiderPersons);
  			
			var filters = new List<Tuple<double, double, double, bool, AddressMatch>>()
			{
				Tuple.Create(JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MaxValue, true, AddressMatch.None),
				Tuple.Create(JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MaxValue, false, AddressMatch.None),
				Tuple.Create(JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MinValue, true, AddressMatch.Full),
				Tuple.Create(JaroWinkler.MaxValue, 0.95, JaroWinkler.MinValue, true, AddressMatch.Full),
				Tuple.Create(JaroWinkler.MaxValue, 0.95, JaroWinkler.MaxValue, true, AddressMatch.None),
				Tuple.Create(JaroWinkler.MaxValue, 0.90, JaroWinkler.MaxValue, true, AddressMatch.None),
				Tuple.Create(JaroWinkler.MaxValue, 0.85, JaroWinkler.MaxValue, true, AddressMatch.None),
				Tuple.Create(JaroWinkler.MaxValue, 0.80, JaroWinkler.MaxValue, true, AddressMatch.Full),
				Tuple.Create(JaroWinkler.MaxValue, 0.85, JaroWinkler.MinValue, true, AddressMatch.Full),
				Tuple.Create(JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MinValue, true, AddressMatch.StreetZipCity),
				Tuple.Create(JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MinValue, true, AddressMatch.ZipCity),
				Tuple.Create(JaroWinkler.MaxValue, 0.74, JaroWinkler.MaxValue, true, AddressMatch.ZipCity),
				Tuple.Create(0.95, 0.95, JaroWinkler.MaxValue, true, AddressMatch.None),
			};

			foreach (var filter in filters)
			{
				var matches = EervParishDataMatcher
					.FindFuzzyMatches (todo, namesToAiderPersons, filter.Item1, filter.Item2, filter.Item3, filter.Item4, filter.Item5)
					.ToList ();

				EervParishDataMatcher.AssignMatches (matches, todo, done, matched);

				var newMatches = EervParishDataMatcher.FindMatchesInHouseholdMembers (matches, todo, matched)
					.ToList ();

				EervParishDataMatcher.AssignMatches (newMatches, todo, done, matched);
			}
		}


		private static void FindMatchesWithSplitMethod(IEnumerable<NormalizedPerson> aiderPersons, HashSet<NormalizedPerson> todo, Dictionary<NormalizedPerson, List<NormalizedPerson>> done, HashSet<NormalizedPerson> matched)
		{
			var splitNamesToAiderPersons = EervParishDataMatcher.GroupPersonsBySplitNames (aiderPersons);

			var matches = EervParishDataMatcher
				.FindMatchesWithSplitMethod (splitNamesToAiderPersons, todo, matched)
				.ToList ();

			EervParishDataMatcher.AssignMatches (matches, todo, done, matched);
		}


		private static void AssignMatches(IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> matches, HashSet<NormalizedPerson> todo, Dictionary<NormalizedPerson, List<NormalizedPerson>> done, HashSet<NormalizedPerson> matched)
		{
			foreach (var match in matches)
			{
				var eervPerson = match.Item1;
				var matchingAiderPersons = match.Item2;

				done.Add (eervPerson, matchingAiderPersons);
				todo.Remove (eervPerson);
				matched.AddRange (matchingAiderPersons);
			}
		}


		private static IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> FindMatchesInHouseholdMembers(IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> personMatches, HashSet<NormalizedPerson> todo, HashSet<NormalizedPerson> matched)
		{
			var newMatches = new List<Tuple<NormalizedPerson, List<NormalizedPerson>>> ();

			var householdMatches = new Dictionary<NormalizedHousehold, HashSet<NormalizedHousehold>> ();

			foreach (var match in personMatches)
			{
				var eervPerson = match.Item1;
				var eervHousehold = eervPerson.Households.Single();

				HashSet<NormalizedHousehold> aiderHouseholds;

				if (!householdMatches.TryGetValue (eervHousehold, out aiderHouseholds))
				{
					aiderHouseholds = new HashSet<NormalizedHousehold> ();

					householdMatches[eervHousehold] = aiderHouseholds;
				}

				var aiderPersons = match.Item2;
				
				foreach (var aiderPerson in aiderPersons)
				{
					aiderHouseholds.AddRange (aiderPerson.Households);
				}
			}

			foreach (var householdMatch in householdMatches)
			{
				var eervHousehold = householdMatch.Key;
				var eervMembers = eervHousehold
					.Members
					.Where (m => todo.Contains (m))
					.ToList ();

				var aiderHouseholds = householdMatch.Value;
				var aiderMembers = aiderHouseholds
					.SelectMany (h => h.Members)
					.Where (m => !matched.Contains (m))
					.Distinct ()
					.ToList ();

				foreach (var eervMember in eervMembers)
				{
					var aiderCandidates = EervParishDataMatcher
						.GetFuzzyMatches (eervMember, aiderMembers)
						.ToList ();

					var acceptedAiderCandidate = aiderCandidates
						.Where (c => c.Item2.Item1 >= 0.8)
						.Where (c => c.Item2.Item2 >= 0.8)
						.Where (c => c.Item2.Item4)
						.Select (c => c.Item1)
						.FirstOrDefault ();

					if (acceptedAiderCandidate != null)
					{
						var aiderPersons = new List<NormalizedPerson> () { acceptedAiderCandidate };
						var newMatch = Tuple.Create (eervMember, aiderPersons);

						newMatches.Add (newMatch);
					}
				}
			}

			return newMatches;
		}


		private static void AssignUnmatchedPersons(HashSet<NormalizedPerson> todo, Dictionary<NormalizedPerson, List<NormalizedPerson>> done)
		{
			foreach (var person in todo)
			{
				done.Add (person, new List<NormalizedPerson> ());
			}
		}


		private static Dictionary<string, Dictionary<string, List<NormalizedPerson>>> GroupPersonsByNames(IEnumerable<NormalizedPerson> persons)
		{
			return persons
				.GroupBy (p => p.Lastname)
				.ToDictionary
				(
					g1 => g1.Key,
					g1 => g1.GroupBy (p2 => p2.Firstname).ToDictionary
					(
						g2 => g2.Key,
						g2 => g2.ToList ()
					)
				);
		}


		private static Dictionary<string, Dictionary<string, List<NormalizedPerson>>> GroupPersonsBySplitNames(IEnumerable<NormalizedPerson> persons)
		{
			var result = new Dictionary<string, Dictionary<string, List<NormalizedPerson>>> ();

			foreach (var person in persons)
			{
				foreach (var lastname in person.Lastnames)
				{
					Dictionary<string, List<NormalizedPerson>> d;

					if (!result.TryGetValue (lastname, out d))
					{
						d = new Dictionary<string, List<NormalizedPerson>> ();

						result[lastname] = d;
					}

					foreach (var firstname in person.Firstnames)
					{
						List<NormalizedPerson> l;

						if (!d.TryGetValue (firstname, out l))
						{
							l = new List<NormalizedPerson> ();

							d[firstname] = l;
						}

						l.Add (person);
					}
				}
			}

			return result;
		}


		private static IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> FindFuzzyMatches(IEnumerable<NormalizedPerson> eervPersons, Dictionary<string, Dictionary<string, List<NormalizedPerson>>> namesToAiderPersons, double fln, double ffn, double fdb, bool fsx, AddressMatch fad)
		{
			return from eervPerson in eervPersons
				   let matches = EervParishDataMatcher.FindFuzzyMatches (eervPerson, namesToAiderPersons, fln, ffn, fdb, fsx, fad).Take (10).ToList ()
				   where matches.Count > 0
				   select Tuple.Create (eervPerson, matches);
		}


		private static IEnumerable<NormalizedPerson> FindFuzzyMatches(NormalizedPerson eervPerson, Dictionary<string, Dictionary<string, List<NormalizedPerson>>> namesToAiderPersons, double fln, double ffn, double fdb, bool fsx, AddressMatch fad)
		{
			IEnumerable<NormalizedPerson> candidates;

			if (fln == JaroWinkler.MaxValue)
			{
				var tmpCandidates = EervParishDataMatcher.GetCandidateByExactLastname (eervPerson, namesToAiderPersons);

				if (ffn == JaroWinkler.MaxValue)
				{
					candidates = EervParishDataMatcher.GetCandidateByExactFirstname (eervPerson, tmpCandidates);
				}
				else
				{
					candidates = EervParishDataMatcher.GetCandidateByFuzzyFirstname (eervPerson, tmpCandidates, ffn);
				}
			}
			else
			{
				var tmpCandidates = EervParishDataMatcher.GetCandidateByFuzzyLastname (eervPerson, namesToAiderPersons, fln);

				if (ffn == JaroWinkler.MaxValue)
				{
					candidates = EervParishDataMatcher.GetCandidateByExactFirstname (eervPerson, tmpCandidates);
				}
				else
				{
					candidates = EervParishDataMatcher.GetCandidateByFuzzyFirstname (eervPerson, tmpCandidates, ffn);
				}
			}

			return from aiderPerson in candidates
				   where EervParishDataMatcher.IsSexMatch (eervPerson, aiderPerson, fsx)
				   where EervParishDataMatcher.AreDateOfBirthSimilar (eervPerson, aiderPerson, fdb)
				   where EervParishDataMatcher.AreAddressesSimilar (eervPerson, aiderPerson, fad)
				   select aiderPerson;
		}


		private static Dictionary<string, List<NormalizedPerson>> GetCandidateByExactLastname(NormalizedPerson eervPerson, Dictionary<string, Dictionary<string, List<NormalizedPerson>>> namesToAiderPersons)
		{
			Dictionary<string, List<NormalizedPerson>> firstnamesToAiderPersons;

			if (namesToAiderPersons.TryGetValue (eervPerson.Lastname, out firstnamesToAiderPersons))
			{
				return firstnamesToAiderPersons;
			}

			return new Dictionary<string, List<NormalizedPerson>> ();
		}


		private static IEnumerable<NormalizedPerson> GetCandidateByExactFirstname(NormalizedPerson eervPerson, Dictionary<string, List<NormalizedPerson>> namesToAiderPersons)
		{
			List<NormalizedPerson> result;

			if (namesToAiderPersons.TryGetValue (eervPerson.Firstname, out result))
			{
				return result;
			}

			return new List<NormalizedPerson> ();
		}


		private static IEnumerable<NormalizedPerson> GetCandidateByFuzzyFirstname(NormalizedPerson eervPerson, Dictionary<string, List<NormalizedPerson>> namesToAiderPersons, double ffn)
		{
			var firstnames = EervParishDataMatcher.GetFuzzyMatches (eervPerson.Firstname, namesToAiderPersons.Keys, ffn);

			foreach (var firstname in firstnames)
			{
				var aiderPersons = namesToAiderPersons[firstname.Item1];

				foreach (var aiderPerson in aiderPersons)
				{
					yield return aiderPerson;
				}
			}
		}


		private static IEnumerable<NormalizedPerson> GetCandidateByFuzzyLastname(NormalizedPerson eervPerson, Dictionary<string, Dictionary<string, List<NormalizedPerson>>> namesToAiderPersons, double fln)
		{
			var lastnames = EervParishDataMatcher.GetFuzzyMatches (eervPerson.Lastname, namesToAiderPersons.Keys, fln);

			foreach (var lastname in lastnames)
			{
				var aiderPersons = namesToAiderPersons[lastname.Item1];

				foreach (var aiderPerson in aiderPersons.Values)
				{
					foreach (var ap in aiderPerson)
					{
						yield return ap;
					}
				}
			}
		}


		private static IEnumerable<NormalizedPerson> GetCandidateByExactFirstname(NormalizedPerson eervPerson, IEnumerable<NormalizedPerson> aiderPersons)
		{
			return aiderPersons.Where (p => p.Firstname == eervPerson.Firstname);
		}


		private static IEnumerable<NormalizedPerson> GetCandidateByFuzzyFirstname(NormalizedPerson eervPerson, IEnumerable<NormalizedPerson> aiderPersons, double ffn)
		{
			return from aiderPerson in aiderPersons
				   let metric = JaroWinkler.ComputeJaroWinklerDistance (eervPerson.Firstname, aiderPerson.Firstname)
				   where metric >= ffn
				   select aiderPerson;
		}


		private static IEnumerable<Tuple<string, double>> GetFuzzyMatches(string name, IEnumerable<string> names, double f)
		{
			return from n in names
				   let metric = JaroWinkler.ComputeJaroWinklerDistance (name, n)
				   where metric >= f
				   orderby metric descending
				   select Tuple.Create (n, metric);
		}


		private static IEnumerable<Tuple<NormalizedPerson, Tuple<double, double, double, bool, AddressMatch>>> GetFuzzyMatches(NormalizedPerson eervPerson, IEnumerable<NormalizedPerson> aiderPersons)
		{
			return from aiderPerson in aiderPersons
				   let sfn = JaroWinkler.ComputeJaroWinklerDistance (eervPerson.Firstname, aiderPerson.Firstname)
				   let sln = JaroWinkler.ComputeJaroWinklerDistance (eervPerson.Lastname, aiderPerson.Lastname)
				   let sdb = EervParishDataMatcher.GetDateSimilarity (eervPerson.DateOfBirth, aiderPerson.DateOfBirth)
				   let ssx = EervParishDataMatcher.IsSexMatch (eervPerson, aiderPerson, true)
				   let sad = EervParishDataMatcher.GetAddressSimilarity (eervPerson.Addresses.First (), aiderPerson.Addresses.First ())
				   orderby (sfn + sln) / 2 descending
				   select Tuple.Create (aiderPerson, Tuple.Create (sfn, sln, sdb, ssx, sad));
		}


		private static IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> FindMatchesWithSplitMethod(Dictionary<string, Dictionary<string, List<NormalizedPerson>>> splitNamesToAiderPersons, HashSet<NormalizedPerson> eervPersons, HashSet<NormalizedPerson> matched)
		{
			return from eervPerson in eervPersons
				   let matches = EervParishDataMatcher.FindMatchesWithSplitMethod (eervPerson, splitNamesToAiderPersons, matched).ToList ()
				   where matches.Count > 0
				   select Tuple.Create (eervPerson, matches);
		}


		private static IEnumerable<NormalizedPerson> FindMatchesWithSplitMethod(NormalizedPerson eervPerson, Dictionary<string, Dictionary<string, List<NormalizedPerson>>> splitNamesToAiderPersons, HashSet<NormalizedPerson> matched)
		{
			var candidates = EervParishDataMatcher
				.GetCandidatesBySplitNames (eervPerson, splitNamesToAiderPersons)
				.Where (p => !matched.Contains (p))
				.ToList ();

			return from candidate in candidates
				   let ssx = EervParishDataMatcher.IsSexMatch (eervPerson, candidate, true)
				   where ssx
				   let sdb = EervParishDataMatcher.AreDateOfBirthSimilar (eervPerson, candidate, JaroWinkler.MaxValue)
				   let sad = EervParishDataMatcher.AreAddressesSimilar (eervPerson, candidate, AddressMatch.Full)
				   where sdb || sad
				   select candidate;
		}
  
		private static HashSet<NormalizedPerson> GetCandidatesBySplitNames(NormalizedPerson eervPerson, Dictionary<string, Dictionary<string, List<NormalizedPerson>>> splitNamesToAiderPersons)
		{
			var candidates = new HashSet<NormalizedPerson>();

			foreach (var lastname in eervPerson.Lastnames)
			{
				Dictionary<string, List<NormalizedPerson>> d;

				if (splitNamesToAiderPersons.TryGetValue(lastname, out d))
				{
					foreach (var fistname in eervPerson.Firstnames)
					{
						List<NormalizedPerson> c;

						if (d.TryGetValue(fistname, out c))
						{
							candidates.AddRange(c);
						}
					}
				}
			}

			return candidates;
		}


		private static bool IsSexMatch(NormalizedPerson eervPerson, NormalizedPerson aiderPerson, bool fsx)
		{
			return !fsx || eervPerson.Sex == aiderPerson.Sex;
		}


		private static bool AreDateOfBirthSimilar(NormalizedPerson eervPerson, NormalizedPerson aiderPerson, double fdb)
		{
			var d1 = eervPerson.DateOfBirth;
			var d2 = aiderPerson.DateOfBirth;

			if (d1 == d2)
			{
				return true;
			}
			else
			{
				var similarity = EervParishDataMatcher.GetDateSimilarity (d1, d2);

				return similarity >= fdb;
			}
		}


		private static double GetDateSimilarity(Date? d1, Date? d2)
		{
			var s1 = EervParishDataMatcher.GetDateForSimilarityComputation (d1);
			var s2 = EervParishDataMatcher.GetDateForSimilarityComputation (d2);

			return JaroWinkler.ComputeJaroDistance (s1, s2);
		}


		private static string GetDateForSimilarityComputation(Date? date)
		{
			return date.HasValue
				? date.ToString ().Replace (".", " ")
				: "xxxxxxxx";
		}


		private static bool AreAddressesSimilar(NormalizedPerson eervPerson, NormalizedPerson aiderPerson, AddressMatch match)
		{
			foreach (var eervAddress in eervPerson.Addresses)
			{
				foreach (var aiderAddress in aiderPerson.Addresses)
				{
					var addressMatch = EervParishDataMatcher.GetAddressSimilarity (eervAddress, aiderAddress);

					switch (match)
					{
						case AddressMatch.None:
							return true;

						case AddressMatch.ZipCity:

							if (addressMatch == AddressMatch.ZipCity)
							{
								return true;
							}
							else
							{
								goto case AddressMatch.StreetZipCity;
							}

						case AddressMatch.StreetZipCity:

							if (addressMatch == AddressMatch.StreetZipCity)
							{
								return true;
							}
							else
							{
								goto case AddressMatch.Full;
							}

						case AddressMatch.Full:

							if (addressMatch == AddressMatch.Full)
							{
								return true;
							}

							break;

						default:
							throw new NotImplementedException ();
					}
				}
			}

			return false;
		}


		private static AddressMatch GetAddressSimilarity(NormalizedAddress a, NormalizedAddress b)
		{
			var sameTown = a.Town == b.Town;
			var sameZipCode = a.ZipCode == b.ZipCode;

			if (!sameTown && !sameZipCode)
			{
				return AddressMatch.None;
			}

			var sameStreet = a.Street == b.Street;

			if (!sameStreet)
			{
				return AddressMatch.ZipCity;
			}

			var sameHouseNumber = a.HouseNumber == b.HouseNumber;

			if (!sameHouseNumber)
			{
				return AddressMatch.StreetZipCity;
			}

			return AddressMatch.Full;
		}


	}


}
