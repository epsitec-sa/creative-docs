using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Text;

using Epsitec.Common.Types;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;

using System.Text;


namespace Epsitec.Aider.Data.Normalization
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
	internal static class NormalizedDataMatcher
	{


		public static IEnumerable<Tuple<NormalizedPerson, Tuple<NormalizedPerson, MatchData>>> FindMatches
		(
			IEnumerable<NormalizedPerson> eervPersons,
			IEnumerable<NormalizedPerson> aiderPersons,
			bool considerDateOfBirth,
			bool considerSex,
			bool considerAddressAsMostRelevant = false
		)
		{
			var todo = new HashSet<NormalizedPerson> (eervPersons);
			var done = new Dictionary<NormalizedPerson, NormalizedPerson> ();
			var matched = new HashSet<NormalizedPerson> ();

			NormalizedDataMatcher.FindMatchesWithFuzzyMethod (aiderPersons, todo, done, matched, considerDateOfBirth, considerSex, considerAddressAsMostRelevant);
			NormalizedDataMatcher.FindMatchesWithSplitMethod (aiderPersons, todo, done, matched);

			NormalizedDataMatcher.LogMatched (todo, done);
			NormalizedDataMatcher.LogNotMatched (aiderPersons, todo, done);
			NormalizedDataMatcher.LogWarnings (done);

			NormalizedDataMatcher.AssignUnmatchedPersons (todo, done);

			return NormalizedDataMatcher.GetResultsWithMatchData (done);
		}


		private static void LogMatched
		(
			HashSet<NormalizedPerson> todo,
			Dictionary<NormalizedPerson, NormalizedPerson> done
		)
		{
			var sb  = new StringBuilder ();

			sb.AppendLine ("================================================================");

			var doneCount = done.Count;
			var totalCount = done.Count + todo.Count;
			var format = "Number of file persons with match in DB: {0}/{1}";
			sb.AppendLine (string.Format (format, doneCount, totalCount));

			foreach (var match in done)
			{
				sb.AppendLine ("---------------------------------------------------------------");
				sb.AppendLine ("FILE: " + match.Key.ToString ());
				sb.AppendLine ("DB:   " + match.Value.ToString ());
			}

			Debug.WriteLine (sb.ToString ());
		}


		private static void LogNotMatched
		(
			IEnumerable<NormalizedPerson> aiderPersons,
			HashSet<NormalizedPerson> todo,
			Dictionary<NormalizedPerson, NormalizedPerson> done
		)
		{
			var sb = new StringBuilder ();

			sb.AppendLine ("================================================================");

			var todoCount = todo.Count;
			var totalCount = done.Count + todo.Count;
			var format = "Number of file persons without match in DB {0}/{1}";
			sb.AppendLine (string.Format (format, todoCount, totalCount));

			var namesToPersons = NormalizedDataMatcher.GroupPersonsByNames (aiderPersons);

			foreach (var d in todo)
			{
				sb.AppendLine ("---------------------------------------------------------------");
				sb.AppendLine ("FILE: " + d.ToString ());

				var debugMatches = NormalizedDataMatcher.GetDebugMatches (d, namesToPersons);

				foreach (var m in debugMatches.Take (15))
				{
					sb.AppendLine ("DB:   " + m.ToString ());
				}
			}

			Debug.WriteLine (sb.ToString ());
		}


		private static IEnumerable<NormalizedPerson> GetDebugMatches
		(
			NormalizedPerson person,
			Dictionary<string, Dictionary<string, List<NormalizedPerson>>> namesToPersons
		)
		{
			return from m in NormalizedDataMatcher.FindFuzzyMatches (person, namesToPersons, 0.75, 0.75, JaroWinkler.MinValue, false, AddressMatch.None)
				   let sfn = JaroWinkler.ComputeJaroWinklerDistance (person.Firstname, m.Firstname)
				   where sfn >= 075
				   let sln = JaroWinkler.ComputeJaroWinklerDistance (person.Lastname, m.Lastname)
				   where sln >= 0.75
				   let sdb = NormalizedDataMatcher.GetDateSimilarity (person.DateOfBirth, m.DateOfBirth)
				   let ssx = NormalizedDataMatcher.IsSexMatch (person, m, true)
				   let sad = NormalizedDataMatcher.GetAddressSimilarity (person.GetAddresses (), m.GetAddresses ())
				   let metric = NormalizedDataMatcher.GetMetric (sfn, sln, sdb, ssx, person.DateOfBirth, m.DateOfBirth, person.Sex, m.Sex)
				   orderby metric descending
				   select m;
		}


		private static void LogWarnings(Dictionary<NormalizedPerson, NormalizedPerson> done)
		{
			var sb = new StringBuilder ();

			var reversedDone = done
				.Select (m => Tuple.Create (m.Value, m.Key))
				.Where (t => t.Item1 != null)
				.GroupBy (t => t.Item1)
				.ToDictionary (g => g.Key, g => g.Select (t => t.Item2).ToList ());

			var multipleMatches = reversedDone
				.Where (m => m.Value.Count > 1)
				.ToList ();

			sb.AppendLine ("================================================================");

			var format = "Number of DB persons with multiple match in file: {0}";
			sb.AppendLine (string.Format (format, multipleMatches.Count));

			foreach (var match in multipleMatches)
			{
				sb.AppendLine ("---------------------------------------------------------------");
				sb.AppendLine ("DB:   " + match.Key.ToString ());

				foreach (var p in match.Value)
				{
					sb.AppendLine ("FILE: " + p.ToString ());
				}
			}

			Debug.WriteLine (sb.ToString ());
		}


		private static IEnumerable<Tuple<NormalizedPerson, Tuple<NormalizedPerson, MatchData>>> GetResultsWithMatchData(Dictionary<NormalizedPerson, NormalizedPerson> matches)
		{
			foreach (var match in matches)
			{
				var eervPerson = match.Key;
				var aiderPerson = match.Value;

				Tuple<NormalizedPerson, MatchData> tuple = null;

				if (aiderPerson != null)
				{
					var matchData = NormalizedDataMatcher.GetMatchData (eervPerson, aiderPerson);

					tuple = Tuple.Create (aiderPerson, matchData);
				}

				yield return Tuple.Create (eervPerson, tuple);
			}
		}


		private static MatchData GetMatchData(NormalizedPerson p, NormalizedPerson p2)
		{
			var firstname = JaroWinkler.ComputeJaroWinklerDistance (p.Firstname, p2.Firstname);
			var lastname = JaroWinkler.ComputeJaroWinklerDistance (p.Lastname, p2.Lastname);

			var dateOfBirth = p.DateOfBirth == null
				? (double?) null
				: NormalizedDataMatcher.GetDateSimilarity (p.DateOfBirth, p2.DateOfBirth);

			var sex = p.Sex == PersonSex.Unknown
				? (bool?) null
				: NormalizedDataMatcher.IsSexMatch (p.Sex, p2.Sex);

			var address = NormalizedDataMatcher.GetAddressSimilarity (p.GetAddresses (), p2.GetAddresses ());

			return new MatchData ()
			{
				Firstname = firstname,
				Lastname = lastname,
				DateOfBirth = dateOfBirth,
				Sex = sex,
				Address = address,
			};
		}


		private static void FindMatchesWithFuzzyMethod
		(
			IEnumerable<NormalizedPerson> aiderPersons,
			HashSet<NormalizedPerson> todo,
			Dictionary<NormalizedPerson, NormalizedPerson> done,
			HashSet<NormalizedPerson> matched,
			bool considerDateOfBirth,
			bool considerSex,
			bool considerAddressAsMostRelevant = false
		)
		{
			var namesToAiderPersons = NormalizedDataMatcher.GroupPersonsByNames (aiderPersons);

			var filters = new List<Tuple<double, double, double, bool, AddressMatch>> ()
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
				var fln = filter.Item1;
				var ffn = filter.Item2;
				var fdb = filter.Item3;
				var fsx = filter.Item4;
				var fad = filter.Item5;

				if (considerAddressAsMostRelevant && fad == AddressMatch.None)
				{
					continue;
				}

				if (!considerDateOfBirth)
				{
					fdb = JaroWinkler.MinValue;
				}

				if (!considerSex)
				{
					fsx = false;
				}

				var matches = NormalizedDataMatcher
					.FindFuzzyMatches (todo, namesToAiderPersons, fln, ffn, fdb, fsx, fad)
					.ToList ();

				NormalizedDataMatcher.AssignMatches (matches, todo, done, matched);

				var newMatches = NormalizedDataMatcher.FindNewMatchesWithinMatchingHouseholds (matches, todo)
					.ToList ();

				NormalizedDataMatcher.AssignMatches (newMatches, todo, done, matched);
			}
		}


		private static void FindMatchesWithSplitMethod(IEnumerable<NormalizedPerson> aiderPersons, HashSet<NormalizedPerson> todo, Dictionary<NormalizedPerson, NormalizedPerson> done, HashSet<NormalizedPerson> matched)
		{
			var splitNamesToAiderPersons = NormalizedDataMatcher.GroupPersonsBySplitNames (aiderPersons);

			var matches = NormalizedDataMatcher
				.FindMatchesWithSplitMethod (splitNamesToAiderPersons, todo, matched)
				.ToList ();

			NormalizedDataMatcher.AssignMatches (matches, todo, done, matched);
		}


		private static void AssignMatches(IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> matches, HashSet<NormalizedPerson> todo, Dictionary<NormalizedPerson, NormalizedPerson> done, HashSet<NormalizedPerson> matched)
		{
			foreach (var match in matches)
			{
				var eervPerson = match.Item1;
				var matchingAiderPerson = match.Item2.FirstOrDefault ();

				done.Add (eervPerson, matchingAiderPerson);
				todo.Remove (eervPerson);
				matched.Add (matchingAiderPerson);
			}
		}


		private static IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> FindNewMatchesWithinMatchingHouseholds(IEnumerable<Tuple<NormalizedPerson, List<NormalizedPerson>>> personMatches, HashSet<NormalizedPerson> todo)
		{
			var matchingHouseholds = NormalizedDataMatcher.FindMatchingHouseholds (personMatches);

			return NormalizedDataMatcher.FindNewMatchesInHousenoldMatches (matchingHouseholds, todo);
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
					aiderHouseholds.UnionWith (aiderPerson.Households);
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
					.Where (m => todo.Contains (m))
					.ToList ();

				var aiderHouseholds = match.Value;
				var aiderMembers = aiderHouseholds
					.SelectMany (h => h.Members)
					.Distinct ()
					.ToList ();

				foreach (var eervMember in eervMembers)
				{
					var aiderCandidate = NormalizedDataMatcher
						.GetFuzzyMatches (eervMember, aiderMembers, 0.8, 0.8)
						.Select (c => c.Item1)
						.FirstOrDefault ();

					if (aiderCandidate != null)
					{
						var bestMatchForAcceptedAiderCandidate = NormalizedDataMatcher
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


		private static void AssignUnmatchedPersons(HashSet<NormalizedPerson> todo, Dictionary<NormalizedPerson, NormalizedPerson> done)
		{
			foreach (var person in todo)
			{
				done.Add (person, null);
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
				var lastnames = NormalizedDataMatcher.GetLastnamesForSplitMethod (person.Lastnames);

				foreach (var lastname in lastnames)
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
				   let matches = NormalizedDataMatcher.FindFuzzyMatches (eervPerson, namesToAiderPersons, fln, ffn, fdb, fsx, fad).Take (10).ToList ()
				   where matches.Count > 0
				   select Tuple.Create (eervPerson, matches);
		}


		private static IEnumerable<NormalizedPerson> FindFuzzyMatches(NormalizedPerson eervPerson, Dictionary<string, Dictionary<string, List<NormalizedPerson>>> namesToAiderPersons, double fln, double ffn, double fdb, bool fsx, AddressMatch fad)
		{
			IEnumerable<NormalizedPerson> candidates;

			if (fln == JaroWinkler.MaxValue)
			{
				var tmpCandidates = NormalizedDataMatcher.GetCandidateByExactLastname (eervPerson, namesToAiderPersons);

				if (ffn == JaroWinkler.MaxValue)
				{
					candidates = NormalizedDataMatcher.GetCandidateByExactFirstname (eervPerson, tmpCandidates);
				}
				else
				{
					candidates = NormalizedDataMatcher.GetCandidateByFuzzyFirstname (eervPerson, tmpCandidates, ffn);
				}
			}
			else
			{
				var tmpCandidates = NormalizedDataMatcher.GetCandidateByFuzzyLastname (eervPerson, namesToAiderPersons, fln);

				if (ffn == JaroWinkler.MaxValue)
				{
					candidates = NormalizedDataMatcher.GetCandidateByExactFirstname (eervPerson, tmpCandidates);
				}
				else
				{
					candidates = NormalizedDataMatcher.GetCandidateByFuzzyFirstname (eervPerson, tmpCandidates, ffn);
				}
			}

			return from aiderPerson in candidates
				   where NormalizedDataMatcher.IsSexMatch (eervPerson, aiderPerson, fsx)
				   where NormalizedDataMatcher.AreDateOfBirthSimilar (eervPerson, aiderPerson, fdb)
				   where NormalizedDataMatcher.AreAddressesSimilar (eervPerson, aiderPerson, fad)
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
			var firstnames = NormalizedDataMatcher.GetFuzzyMatches (eervPerson.Firstname, namesToAiderPersons.Keys, ffn);

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
			var lastnames = NormalizedDataMatcher.GetFuzzyMatches (eervPerson.Lastname, namesToAiderPersons.Keys, fln);

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
				   let sdb = NormalizedDataMatcher.GetDateSimilarity (p1.DateOfBirth, p2.DateOfBirth)
				   let ssx = NormalizedDataMatcher.IsSexMatch (p1, p2, true)
				   let sad = NormalizedDataMatcher.GetAddressSimilarity (p1.GetAddresses (), p2.GetAddresses ())
				   let metric = NormalizedDataMatcher.GetMetric (sfn, sln, sdb, ssx, p1.DateOfBirth, p2.DateOfBirth, p1.Sex, p2.Sex)
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
				   let matches = NormalizedDataMatcher.FindMatchesWithSplitMethod (eervPerson, splitNamesToAiderPersons, matched).ToList ()
				   where matches.Count > 0
				   select Tuple.Create (eervPerson, matches);
		}


		private static IEnumerable<NormalizedPerson> FindMatchesWithSplitMethod(NormalizedPerson eervPerson, Dictionary<string, Dictionary<string, List<NormalizedPerson>>> splitNamesToAiderPersons, HashSet<NormalizedPerson> matched)
		{
			var candidates = NormalizedDataMatcher
				.GetCandidatesBySplitNames (eervPerson, splitNamesToAiderPersons)
				.Where (p => !matched.Contains (p))
				.ToList ();

			return from candidate in candidates
				   let ssx = NormalizedDataMatcher.IsSexMatch (eervPerson, candidate, true)
				   where ssx
				   let sdb = NormalizedDataMatcher.AreDateOfBirthSimilar (eervPerson, candidate, JaroWinkler.MaxValue)
				   let sad = NormalizedDataMatcher.AreAddressesSimilar (eervPerson, candidate, AddressMatch.Full)
				   where sdb || sad
				   select candidate;
		}


		private static HashSet<NormalizedPerson> GetCandidatesBySplitNames(NormalizedPerson eervPerson, Dictionary<string, Dictionary<string, List<NormalizedPerson>>> splitNamesToAiderPersons)
		{
			var candidates = new HashSet<NormalizedPerson> ();

			var lastnames = NormalizedDataMatcher.GetLastnamesForSplitMethod (eervPerson.Lastnames);

			foreach (var lastname in lastnames)
			{
				Dictionary<string, List<NormalizedPerson>> d;

				if (splitNamesToAiderPersons.TryGetValue (lastname, out d))
				{
					foreach (var fistname in eervPerson.Firstnames)
					{
						List<NormalizedPerson> c;

						if (d.TryGetValue (fistname, out c))
						{
							candidates.UnionWith (c);
						}
					}
				}
			}

			return candidates;
		}


		private static string[] GetLastnamesForSplitMethod(string[] lastnames)
		{
			// Here we want to remove the particles, so as not to match on the "de", "von", etc.
			// from the names. We assume that everything which is smaller than 4 letters is a
			// particule, except for names that have only one part and names where all names have
			// 3 or less letters.

			if (lastnames.Length == 1)
			{
				return lastnames;
			}

			if (lastnames.All (n => n.Length <= 3))
			{
				return lastnames;
			}

			return lastnames
				.Where (n => n.Length > 3)
				.ToArray ();
		}


		private static bool IsSexMatch(NormalizedPerson eervPerson, NormalizedPerson aiderPerson, bool fsx)
		{
			if (!fsx)
			{
				return true;
			}

			if (eervPerson.Sex == PersonSex.Unknown || aiderPerson.Sex == PersonSex.Unknown)
			{
				return false;
			}

			return NormalizedDataMatcher.IsSexMatch (eervPerson.Sex, aiderPerson.Sex);
		}


		private static bool IsSexMatch(PersonSex s1, PersonSex s2)
		{
			return s1 == s2;
		}


		private static bool AreDateOfBirthSimilar(NormalizedPerson eervPerson, NormalizedPerson aiderPerson, double fdb)
		{
			if (fdb == JaroWinkler.MinValue)
			{
				return true;
			}

			var d1 = eervPerson.DateOfBirth;
			var d2 = aiderPerson.DateOfBirth;

			if (!d1.HasValue || !d2.HasValue)
			{
				return false;
			}

			return d1 == d2 || NormalizedDataMatcher.GetDateSimilarity (d1, d2) >= fdb;
		}


		private static double GetDateSimilarity(Date? d1, Date? d2)
		{
			var s1 = NormalizedDataMatcher.GetDateForSimilarityComputation (d1);
			var s2 = NormalizedDataMatcher.GetDateForSimilarityComputation (d2);

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
			foreach (var eervAddress in eervPerson.GetAddresses ())
			{
				foreach (var aiderAddress in aiderPerson.GetAddresses ())
				{
					var similarity = NormalizedDataMatcher.GetAddressSimilarity (eervAddress, aiderAddress);

					if (NormalizedDataMatcher.IsAddressMatchBetterThanOrEqualTo (similarity, match))
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
					var similarity = NormalizedDataMatcher.GetAddressSimilarity (address1, address2);

					if (NormalizedDataMatcher.IsAddressMatchBetterThanOrEqualTo (similarity, result))
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
			var isEmpty = a.Town.IsNullOrWhiteSpace ()
				 || b.Town.IsNullOrWhiteSpace ()
				 || a.ZipCode == 0
				 || b.ZipCode == 0;

			if (!sameTown || !sameZipCode || isEmpty)
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
