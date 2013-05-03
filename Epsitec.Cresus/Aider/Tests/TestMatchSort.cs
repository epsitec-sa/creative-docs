//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Tests
{
	internal static class TestMatchSort
	{
		public static void AnalyzeLogs(System.IO.FileInfo logFile)
		{
			var logPath = logFile.FullName;
			var logDir  = System.IO.Path.GetDirectoryName (logPath);
			var logName = System.IO.Path.GetFileNameWithoutExtension (logPath);
			var logOut  = System.IO.Path.Combine (logDir, logName);

			var logs = Tests.TestMatchSort.ParseLog (logFile)
				.OrderBy (info => info.ZipCode*100+info.ZipCodeAddOn)
				.ThenBy (info => info.StreetName.ToLowerInvariant ())
				.ThenBy (info => info.StreetNumber);

			System.IO.File.WriteAllLines (logOut + "-all.log", logs.Select (info => info.ToString ()));

			using (var etl = new Epsitec.Data.Platform.MatchSort.MatchSortEtl (Epsitec.Aider.Data.Subscription.SubscriptionFileWriter.MatchSortCsvPath))
			{
				var failed  = new List<Tests.TestMatchSort.Info> ();
				var relaxed = new List<Tests.TestMatchSort.Info> ();

				int itemCount = 0;

				foreach (var info in logs)
				{
					var result = info.GetMessenger (etl);

					itemCount++;

					if (result == null)
					{
						result = info.GetMessenger (etl, relaxedStreetNumberComplement: true);

						if (result != null)
						{
							relaxed.Add (info);
						}
						else
						{
							failed.Add (info);
						}
					}
				}

				System.IO.File.WriteAllLines (logOut + "-failed.log", failed.Select (info => info.ToString ()));
				System.IO.File.WriteAllLines (logOut + "-relaxed.log", relaxed.Select (info => info.ToString ()));

				System.Diagnostics.Debug.WriteLine ("Log file: {0}, relaxed: {1}, failed: {2}", itemCount, relaxed.Count, failed.Count);
			}
		}

		private static IEnumerable<Info> ParseLog(System.IO.FileInfo logFile)
		{
			return System.IO.File.ReadLines (logFile.FullName, System.Text.Encoding.Default)
				.Where (x => x.StartsWith (Epsitec.Aider.Data.Subscription.SubscriptionFileWriter.ErrorMessage))
				.Select (x => new Info (x));
		}

		private struct Info
		{
			public Info(string line)
			{
				int pos = Epsitec.Aider.Data.Subscription.SubscriptionFileWriter.ErrorMessage.Length;
				
				this.ZipCode      = int.Parse (line.Substring (pos, 4), System.Globalization.CultureInfo.InvariantCulture);
				this.ZipCodeAddOn = int.Parse (line.Substring (pos+6, 2), System.Globalization.CultureInfo.InvariantCulture);

				int posStreetBegin     = pos+10;
				int posStreetEnd       = line.IndexOf (',', posStreetBegin);
				int posStreetNumBegin  = posStreetEnd + 2;
				int posStreetNumEnd    = line.IndexOf (',', posStreetNumBegin);
				int posStreetCompBegin = posStreetNumEnd + 2;

				this.StreetName = line.Substring (posStreetBegin, posStreetEnd-posStreetBegin);
				
				if (posStreetNumEnd == posStreetNumBegin)
				{
					this.StreetNumber = null;
				}
				else
				{
					string num = line.Substring (posStreetNumBegin, posStreetNumEnd-posStreetNumBegin);
					this.StreetNumber = int.Parse (num, System.Globalization.CultureInfo.InvariantCulture);
				}
				
				this.StreetNumberComplement = line.Substring (posStreetCompBegin);
			}

			public int							ZipCode;
			public int							ZipCodeAddOn;
			public string						StreetName;
			public int?							StreetNumber;
			public string						StreetNumberComplement;

			public override string ToString()
			{
				return string.Format ("{0:0000} {1:00}\t{2}\t{3}\t{4}", this.ZipCode, this.ZipCodeAddOn, this.StreetName, this.StreetNumber, this.StreetNumberComplement);
			}

			public string GetMessenger(Epsitec.Data.Platform.MatchSort.MatchSortEtl etl, bool relaxedStreetNumberComplement = false)
			{
				return etl.GetMessenger (this.ZipCode.ToString ("0000"), this.ZipCodeAddOn.ToString ("00"),
										 this.StreetName, this.StreetNumber.HasValue ? this.StreetNumber.Value.ToString () : "",
										 relaxedStreetNumberComplement ? null : this.StreetNumberComplement);
			}
		}
	}
}
