//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.ECh
{
	using DictionaryOfFixes = Dictionary<string, System.Tuple<string, string>>;

	/// <summary>
	/// The <c>EChAddressFixesRepository</c> class is used to apply fixes to known invalid
	/// eCH addresses.
	/// </summary>
	public sealed class EChAddressFixesRepository
	{
		private EChAddressFixesRepository()
		{
			this.fixes = new DictionaryOfFixes ();
			this.failures = new HashSet<string> ();

			foreach (var item in EChAddressFixesRepository.GetFixes ())
			{
				fixes.Add (item.Key, item.Value);
			}
		}


		public static readonly EChAddressFixesRepository	Current = new EChAddressFixesRepository ();


		public IEnumerable<string> GetFailures()
		{
			return this.failures;
		}

		internal bool ApplyQuickFix(ref int zipCode, ref string streetName)
		{
			string key = string.Format ("{0:0000} {1}", zipCode, streetName);

			System.Tuple<string, string> fix;

			if (this.fixes.TryGetValue (key, out fix))
			{
				zipCode    = InvariantConverter.ParseInt (fix.Item1);
				streetName = fix.Item2;

				return true;
			}

			return false;
		}

		internal void RegisterFailure(string message)
		{
			this.failures.Add (message);
		}

		
		private static IEnumerable<KeyValuePair<string, System.Tuple<string, string>>> GetFixes()
		{
			foreach (var line in EChAddressFixesRepository.GetSourceFile ())
			{
				var tokens = line.Split ('\t');

				if (tokens.Length == 4)
				{
					var key   = tokens[0] + " " + tokens[1];
					var tuple = new System.Tuple<string, string> (tokens[2], tokens[3]);

					yield return new KeyValuePair<string, System.Tuple<string, string>> (key, tuple);
				}
			}
		}

		private static IEnumerable<string> GetSourceFile()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var resource = "Epsitec.Aider.DataFiles.eCH-StreetFixes.zip";
			var source   = Epsitec.Common.IO.ZipFile.DecompressTextFile (assembly, resource);

			return Epsitec.Common.IO.StringLineExtractor.GetLines (source);
		}


		private readonly DictionaryOfFixes		fixes;
		private readonly HashSet<string>		failures;
	}
}
