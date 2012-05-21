//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Tools;

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

			foreach (var item in EChAddressFixesRepository.GetFixes ())
			{
				fixes.Add (item.Key, item.Value);
			}
		}

		public static readonly EChAddressFixesRepository	Current = new EChAddressFixesRepository ();

		public static void FixAddress(ref string street, int? houseNumber, ref int zipCode, ref int zipCodeAddOn, ref int zipCodeId, ref string town)
		{
			if (EChAddressFixesRepository.Current.ApplyQuickFix (ref zipCode, ref street))
			{
				var streetCopy = street;

				var hits = SwissPostStreetRepository.Current
					.FindStreets (zipCode)
					.Where (x => x.MatchName (streetCopy));

				AddressPatchEngine.FixAddress (hits, ref street, houseNumber, ref zipCode, ref zipCodeAddOn, ref zipCodeId, ref town);
			}
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
	}
}
