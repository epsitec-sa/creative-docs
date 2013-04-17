using Epsitec.Aider.Data.ECh;

using Epsitec.Common.IO;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;

namespace Epsitec.Aider.Tests
{
	public static class EChFileAnalyzer
	{
		public static void Analyze(FileInfo input)
		{
			ConsoleCreator.RunWithConsole (() => EChFileAnalyzer.AnalyzeInternal (input));
		}

		private static void AnalyzeInternal(FileInfo input)
		{
			Logger.LogToConsole ("Loading file...");
			var echReportedPersons = EChDataLoader.Load (input);

			var nbHouseholds = EChFileAnalyzer.GetNbHouseholds (echReportedPersons);
			Logger.LogToConsole ("Number of households: " + nbHouseholds);

			var nbPersons = EChFileAnalyzer.GetNbPersons (echReportedPersons);
			Logger.LogToConsole ("Number of persons: " + nbPersons);

			foreach (var parameter in EChFileAnalyzer.GetParameters ())
			{
				var considerChildLastnames = parameter.Item1;
				var maxMergeAge = parameter.Item2;

				var nbMergedHouseholds = EChFileAnalyzer.GetNbMergedHouseholds (echReportedPersons, considerChildLastnames, maxMergeAge);
				Logger.LogToConsole ("Number of households after merge (" + considerChildLastnames + ", " + maxMergeAge + "): " + nbMergedHouseholds);
			}

			Logger.LogToConsole("Press [ENTER] to exit...");
			Console.ReadLine ();
		}

		private static int GetNbHouseholds(IList<EChReportedPerson> echReportedPersons)
		{
			return echReportedPersons.Count;
		}

		private static int GetNbPersons(IList<EChReportedPerson> echReportedPersons)
		{
			return echReportedPersons
				.SelectMany (rp => rp.GetMembers ())
				.Select (p => p.Id)
				.Distinct ()
				.Count ();
		}

		private static int GetNbMergedHouseholds(IList<EChReportedPerson> echReportedPersons, bool considerChildLastnames, int? maxMergeAge)
		{
			var comparer = new MergedHouseholdComparer (considerChildLastnames, maxMergeAge);

			return echReportedPersons
				.GroupBy (rp => rp, comparer)
				.Count ();
		}

		private static IEnumerable<Tuple<bool, int?>> GetParameters()
		{
			var bools =  new List<bool> () { true, false };
			var ints = Enumerable.Range (19, 82).Cast<int?> ().Concat (new List<int?> () { null });

			foreach (var b in bools)
			{
				foreach (var i in ints)
				{
					yield return Tuple.Create (b, i);
				}
			}
		}

		private class MergedHouseholdComparer : IEqualityComparer<EChReportedPerson>
		{
			public MergedHouseholdComparer(bool considerChildLastnames, int? maxMergeAge)
			{
				this.considerChildLastnames = considerChildLastnames;
				this.maxMergeAge = maxMergeAge;
			}

			#region IEqualityComparer<EChReportedPerson> Members

			public bool Equals(EChReportedPerson rp1, EChReportedPerson rp2)
			{
				if (rp1 == null && rp2 == null)
				{
					return true;
				}

				if (rp1 == null || rp2 == null)
				{
					return false;
				}

				return MergedHouseholdComparer.HaveSameAddress (rp1, rp2)
					&& this.HaveSameLastname (rp1, rp2)
					&& this.HaveCompatibleAge (rp1, rp2);
			}

			private static bool HaveSameAddress(EChReportedPerson rp1, EChReportedPerson rp2)
			{
				var a1 = rp1.Address;
				var a2 = rp2.Address;

				return a1.AddressLine1 == a2.AddressLine1
					&& a1.HouseNumber == a2.HouseNumber
					&& a1.Street == a2.Street
					&& a1.Town == a2.Town
					&& a1.SwissZipCode == a2.SwissZipCode
					&& a1.SwissZipCodeAddOn == a2.SwissZipCodeAddOn
					&& a1.SwissZipCodeId == a2.SwissZipCodeId
					&& a1.CountryCode == a2.CountryCode;
			}

			private bool HaveSameLastname(EChReportedPerson rp1, EChReportedPerson rp2)
			{
				var m1 = this.GetMembers (rp1);
				var m2 = this.GetMembers (rp2);

				var allNames = m1.Concat(m2)
					.Select (a => a.OfficialName)
					.Distinct ();

				return allNames.Count () == 1;
			}

			private IEnumerable<EChPerson> GetMembers(EChReportedPerson rp)
			{
				return this.considerChildLastnames
					? rp.GetMembers ()
					: rp.GetAdults ();
			}

			private bool HaveCompatibleAge(EChReportedPerson rp1, EChReportedPerson rp2)
			{
				if (!this.maxMergeAge.HasValue)
				{
					return true;
				}

				return rp1.GetAdults ().All (a => a.DateOfBirth.ComputeAge () < this.maxMergeAge.Value)
					|| rp2.GetAdults ().All (a => a.DateOfBirth.ComputeAge () < this.maxMergeAge.Value);
			}

			public int GetHashCode(EChReportedPerson echReportedPerson)
			{
				if (echReportedPerson == null)
				{
					return 0;
				}
					
				int result = 1000000007;

				var address = echReportedPerson.Address;

				if (address.AddressLine1 != null)
				{
					result = 37 * result + address.AddressLine1.GetHashCode ();
				}

				if (address.HouseNumber != null)
				{
					result = 37 * result + address.HouseNumber.GetHashCode ();
				}

				if (address.Street != null)
				{
					result = 37 * result + address.Street.GetHashCode ();
				}

				result = 37 * result + address.Town.GetHashCode ();
				result = 37 * result + address.SwissZipCode.GetHashCode ();
				result = 37 * result + address.SwissZipCodeAddOn.GetHashCode ();
				result = 37 * result + address.SwissZipCodeId.GetHashCode ();
				result = 37 * result + address.CountryCode.GetHashCode ();

				if (echReportedPerson.Adult1 != null)
				{
					result = 37 * result + echReportedPerson.Adult1.OfficialName.GetHashCode ();
				}

				if (echReportedPerson.Adult2 != null)
				{
					result = 37 * result + echReportedPerson.Adult2.OfficialName.GetHashCode ();
				}

				return result;
			}

			#endregion

			private readonly bool considerChildLastnames;
			private readonly int? maxMergeAge;		
		}
	}
}
