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

			Logger.LogToConsole ("Analyzing file...");
			var nbHouseholds = EChFileAnalyzer.GetNbHouseholds (echReportedPersons);
			var nbMergedHouseholds = EChFileAnalyzer.GetNbMergedHouseholds (echReportedPersons);
			var nbPersons = EChFileAnalyzer.GetNbPersons (echReportedPersons);

			Logger.LogToConsole ("Result for file: " + input.FullName);
			Logger.LogToConsole ("Number of households: " + nbHouseholds);
			Logger.LogToConsole ("Number of households after merge: " + nbMergedHouseholds);
			Logger.LogToConsole ("Number of persons: " + nbPersons);

			Logger.LogToConsole("Press [ENTER] to exit...");
			Console.ReadLine ();
		}

		private static int GetNbHouseholds(IList<EChReportedPerson> echReportedPersons)
		{
			return echReportedPersons.Count;
		}

		private static int GetNbMergedHouseholds(IList<EChReportedPerson> echReportedPersons)
		{
			var comparer = new MergedHouseholdComparer ();

			return echReportedPersons
				.GroupBy (rp => rp, comparer)
				.Count ();
		}

		private static int GetNbPersons(IList<EChReportedPerson> echReportedPersons)
		{
			return echReportedPersons
				.SelectMany (rp => rp.GetMembers ())
				.Select (p => p.Id)
				.Distinct ()
				.Count ();
		}

		private class MergedHouseholdComparer : IEqualityComparer<EChReportedPerson>
		{
			#region IEqualityComparer<EChReportedPerson> Members

			public bool Equals(EChReportedPerson x, EChReportedPerson y)
			{
				if (x == null && y == null)
				{
					return true;
				}

				if (x == null || y == null)
				{
					return false;
				}

				return MergedHouseholdComparer.HaveSameAddress (x, y)
					&& MergedHouseholdComparer.HaveSameName (x, y);
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

			private static bool HaveSameName(EChReportedPerson rp1, EChReportedPerson rp2)
			{
				var a1 = rp1.GetAdults ();
				var a2 = rp2.GetAdults ();

				var allNames = a1.Concat(a2)
					.Select (a => a.OfficialName)
					.Distinct ();

				return allNames.Count () == 1;
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
		}
	}
}
