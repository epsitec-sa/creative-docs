using Epsitec.Aider.Enumerations;

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
	/// firstname and one of the lastname in common with the person I want to match. Of course, this
	/// gives a lot of false positives, so I filter those results to ensure that the matches have
	/// the same sex and either the same date of birth or the same address and I never consider
	/// persons that have already been matched by anoter one. Experimentally, this lead to few false
	/// positive, even if I would have expected that such an aggressive technique would lead to much
	/// more. This is probably because most of the persons are already matched by the first part of
	/// the algorithm.
	/// </summary>
	internal static class EervParishDataMatcher
	{


		public static IEnumerable<Tuple<NormalizedPerson, List<Tuple<NormalizedPerson, MatchData>>>> FindMatches(IEnumerable<NormalizedPerson> eervPersons, IEnumerable<NormalizedPerson> aiderPersons)
		{
			var todo = new HashSet<NormalizedPerson> (eervPersons);
			var done = new Dictionary<NormalizedPerson, List<NormalizedPerson>> ();
			var matched = new HashSet<NormalizedPerson> ();

			EervParishDataMatcher.FindMatchesWithFuzzyMethod(aiderPersons, todo, done, matched);
			EervParishDataMatcher.FindMatchesWithSplitMethod(aiderPersons, todo, done, matched);
			EervParishDataMatcher.AssignUnmatchedPersons(todo, done);

			EervParishDataMatcher.Warn (done);

			return EervParishDataMatcher.GetResultsWithMatchData(done);
		}


		private static IEnumerable<Tuple<NormalizedPerson, List<Tuple<NormalizedPerson, MatchData>>>> GetResultsWithMatchData(Dictionary<NormalizedPerson, List<NormalizedPerson>> matches)
		{
			return from match in matches
				   let eervPerson = match.Key
				   let aiderPersons = match.Value
				   let aiderPersonsWithMatchData = EervParishDataMatcher.GetResultsWithMatchData (eervPerson, aiderPersons)
				   select Tuple.Create (eervPerson, aiderPersonsWithMatchData.ToList ());
		}


		private static IEnumerable<Tuple<NormalizedPerson, MatchData>> GetResultsWithMatchData(NormalizedPerson eervPerson, List<NormalizedPerson> aiderPersons)
		{
			return from aiderPerson in aiderPersons
				   let matchData = EervParishDataMatcher.GetMatchData (eervPerson, aiderPerson)
				   select Tuple.Create (aiderPerson, matchData);
		}
  

		private static MatchData GetMatchData(NormalizedPerson p, NormalizedPerson p2)
		{
			var firstname = JaroWinkler.ComputeJaroWinklerDistance(p.Firstname, p2.Firstname);
			var lastname = JaroWinkler.ComputeJaroWinklerDistance(p.Lastname, p2.Lastname);
			
			var dateOfBirth = p.DateOfBirth == null
				? (double?) null
				: EervParishDataMatcher.GetDateSimilarity(p.DateOfBirth, p2.DateOfBirth);

			var sex = p.Sex == PersonSex.Unknown
				? (bool?) null
				: EervParishDataMatcher.IsSexMatch(p.Sex, p2.Sex);

			var address = EervParishDataMatcher.GetAddressSimilarity(p.Addresses, p2.Addresses);

			return new MatchData()
			{
				Firstname = firstname,
				Lastname = lastname,
				DateOfBirth = dateOfBirth,
				Sex = sex,
				Address = address,
			};
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
				Tuple.Create (JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MaxValue, true, AddressMatch.Full),
				Tuple.Create (JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MaxValue, true, AddressMatch.StreetZipCity),
				Tuple.Create (JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MaxValue, true, AddressMatch.ZipCity),
				Tuple.Create (JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MaxValue, true, AddressMatch.None),
				Tuple.Create (JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MaxValue, false, AddressMatch.None),
				Tuple.Create (JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MinValue, true, AddressMatch.Full),			
				Tuple.Create (JaroWinkler.MaxValue, 0.95, JaroWinkler.MinValue, true, AddressMatch.Full),
				Tuple.Create (JaroWinkler.MaxValue, 0.95, JaroWinkler.MaxValue, true, AddressMatch.None),
				Tuple.Create (JaroWinkler.MaxValue, 0.90, JaroWinkler.MaxValue, true, AddressMatch.None),
				Tuple.Create (JaroWinkler.MaxValue, 0.85, JaroWinkler.MaxValue, true, AddressMatch.None),
				Tuple.Create (JaroWinkler.MaxValue, 0.80, JaroWinkler.MaxValue, true, AddressMatch.Full),
				Tuple.Create (JaroWinkler.MaxValue, 0.85, JaroWinkler.MinValue, true, AddressMatch.Full),			
				Tuple.Create (JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MinValue, true, AddressMatch.Full),
				Tuple.Create (JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MinValue, true, AddressMatch.StreetZipCity),
				Tuple.Create (JaroWinkler.MaxValue, JaroWinkler.MaxValue, JaroWinkler.MinValue, true, AddressMatch.ZipCity),
				Tuple.Create (JaroWinkler.MaxValue, 0.74, JaroWinkler.MaxValue, true, AddressMatch.ZipCity),
				Tuple.Create (0.95, 0.95, JaroWinkler.MaxValue, true, AddressMatch.None),
			};

			foreach (var filter in filters)
			{
				var matches = EervParishDataMatcher
					.FindFuzzyMatches (todo, namesToAiderPersons, filter.Item1, filter.Item2, filter.Item3, filter.Item4, filter.Item5)
					.ToList ();

				EervParishDataMatcher.AssignMatches (matches, todo, done, matched);

				var newMatches = EervParishDataMatcher.FindNewMatchesWithinMathingHouseholds (matches, todo)
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


		private static IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> FindNewMatchesWithinMathingHouseholds(IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> personMatches, HashSet<NormalizedPerson> todo)
		{
			var matchingHouseholds = EervParishDataMatcher.FindMatchingHouseholds(personMatches);

			return EervParishDataMatcher.FindNewMatchesInHousenoldMatches (matchingHouseholds, todo);
		}


		private static Dictionary<NormalizedHousehold, HashSet<NormalizedHousehold>> FindMatchingHouseholds(IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> personMatches)
		{
			var matchingHouseholds = new Dictionary<NormalizedHousehold, HashSet<NormalizedHousehold>> ();

			foreach (var match in personMatches)
			{
				var eervPerson = match.Item1;
				var eervHousehold = eervPerson.Households.Single ();

				HashSet<NormalizedHousehold> aiderHouseholds;

				if (!matchingHouseholds.TryGetValue (eervHousehold, out aiderHouseholds))
				{
					aiderHouseholds = new HashSet<NormalizedHousehold> ();

					matchingHouseholds[eervHousehold] = aiderHouseholds;
				}

				var aiderPersons = match.Item2;

				foreach (var aiderPerson in aiderPersons)
				{
					aiderHouseholds.AddRange (aiderPerson.Households);
				}
			}

			return matchingHouseholds;
		}

  
		private static IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> FindNewMatchesInHousenoldMatches(Dictionary<NormalizedHousehold, HashSet<NormalizedHousehold>> matchingHouseholds, HashSet<NormalizedPerson> todo)
		{
			foreach (var match in matchingHouseholds)
			{
				var eervHousehold = match.Key;
				var eervMembers = eervHousehold
					.Members
					.Where(m => todo.Contains(m))
					.ToList();

				var aiderHouseholds = match.Value;
				var aiderMembers = aiderHouseholds
					.SelectMany(h => h.Members)
					.Distinct()
					.ToList();

				foreach (var eervMember in eervMembers)
				{
					var aiderCandidate = EervParishDataMatcher
						.GetFuzzyMatches(eervMember, aiderMembers, 0.8, 0.8)
						.Select (c => c.Item1)
						.FirstOrDefault ();

					if (aiderCandidate != null)
					{
						var bestMatchForAcceptedAiderCandidate = EervParishDataMatcher
							.GetFuzzyMatches (aiderCandidate, eervHousehold.Members, 0, 0)
							.Select (m => m.Item1)
							.FirstOrDefault ();

						if (eervMember == bestMatchForAcceptedAiderCandidate)
						{
							var aiderPersons = new List<NormalizedPerson> ()
							{
								aiderCandidate
							};

							yield return Tuple.Create (eervMember, aiderPersons);
						}
					}
				}
			}
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


		private static IEnumerable<Tuple<NormalizedPerson, Tuple<double, double, double, bool, AddressMatch>>> GetFuzzyMatches(NormalizedPerson person, IEnumerable<NormalizedPerson> persons, double minSfn, double minSln)
		{
			var p1 = person;
			
			return from p2 in persons
				   let sfn = JaroWinkler.ComputeJaroWinklerDistance (p1.Firstname, p2.Firstname)
				   where sfn >= minSfn
				   let sln = JaroWinkler.ComputeJaroWinklerDistance (p1.Lastname, p2.Lastname)
				   where sln >= minSln
				   let sdb = EervParishDataMatcher.GetDateSimilarity (p1.DateOfBirth, p2.DateOfBirth)
				   let ssx = EervParishDataMatcher.IsSexMatch (p1, p2, true)
				   let sad = EervParishDataMatcher.GetAddressSimilarity (p1.Addresses, p2.Addresses)
				   let metric = EervParishDataMatcher.GetMetric (sfn, sln, sdb, ssx, p1.DateOfBirth, p2.DateOfBirth, p1.Sex, p2.Sex)
				   orderby metric descending
				   select Tuple.Create (p2, Tuple.Create (sfn, sln, sdb, ssx, sad));
		}


		private static double GetMetric(double sfn, double sln, double sdb, bool ssx, Date? d1, Date? d2, PersonSex s1, PersonSex s2)
		{
			double numerator = sfn + sln;
			double denominator = 2;

			if (d1.HasValue && d2.HasValue)
			{
				numerator += sdb;
				denominator++;
			}

			if (s1 != PersonSex.Unknown && s2 != PersonSex.Unknown)
			{
				if (ssx)
				{
					numerator += JaroWinkler.MaxValue;
				}
				else
				{
					numerator += JaroWinkler.MinValue;
				}

				denominator++;
			}

			return numerator / denominator;
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
			return !fsx || EervParishDataMatcher.IsSexMatch (eervPerson.Sex, aiderPerson.Sex);
		}


		private static bool IsSexMatch(PersonSex s1, PersonSex s2)
		{
			return s1 == s2;
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
					var similarity = EervParishDataMatcher.GetAddressSimilarity (eervAddress, aiderAddress);

					if (EervParishDataMatcher.IsAddressMatchBetterThanOrEqualTo (similarity, match))
					{
						return true;
					}
				}
			}

			return false;
		}


		private static AddressMatch GetAddressSimilarity(IEnumerable<NormalizedAddress> addresses1, IEnumerable<NormalizedAddress> addresses2)
		{
			var result = AddressMatch.None;

			foreach (var address1 in addresses1)
			{
				foreach (var address2 in addresses2)
				{
					var similarity = EervParishDataMatcher.GetAddressSimilarity (address1, address2);

					if (EervParishDataMatcher.IsAddressMatchBetterThanOrEqualTo (similarity, result))
					{
						result = similarity;
					}
				}
			}

			return result;
		}


		private static bool IsAddressMatchBetterThanOrEqualTo(AddressMatch a, AddressMatch b)
		{
			switch (a)
			{
				case AddressMatch.Full:
					
					return true;
					

				case AddressMatch.StreetZipCity:

					return b == AddressMatch.StreetZipCity
						|| b == AddressMatch.ZipCity
						|| b == AddressMatch.None;

				case AddressMatch.ZipCity:

					return b == AddressMatch.ZipCity
						|| b == AddressMatch.None;
				
				
				case AddressMatch.None:

					return b == AddressMatch.None;

				default:
					throw new NotImplementedException ();
			}
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
