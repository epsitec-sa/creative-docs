//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	public static class SwissPostZipCodeFoldingRepository
	{
		static SwissPostZipCodeFoldingRepository()
		{
			SwissPostZipCodeFoldingRepository.foldings = new Dictionary<SwissPostFullZip, SwissPostZipCodeFolding> ();
			
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var path     = "Epsitec.Data.Platform.DataFiles.ZipCodeFolding.zip";
			var items    = Epsitec.Common.IO.ZipFile.DecompressTextFile (assembly, path, System.Text.Encoding.Default)
				.Split ('\r', '\n').Where (x => x.Length > 0)
				.Select (x => SwissPostZipCodeFolding.Parse (x));

			foreach (var item in items)
			{
				SwissPostZipCodeFolding folding;

				if (SwissPostZipCodeFoldingRepository.foldings.TryGetValue (item.ZipCodeAndAddOn, out folding))
				{
					System.Diagnostics.Debug.Assert (folding.BaseZipCode == item.BaseZipCode);

					SwissPostZipCodeFoldingRepository.foldings[item.ZipCodeAndAddOn] = new SwissPostZipCodeFolding (item.ZipCode, item.ZipCodeAddOn, item.BaseZipCode, SwissPostZipType.Mixed);
				}
				else
				{
					SwissPostZipCodeFoldingRepository.foldings[item.ZipCodeAndAddOn] = item;
				}
			}
		}

		public static IEnumerable<SwissPostZipCodeFolding> FindDerived(int baseZipCode)
		{
			return SwissPostZipCodeFoldingRepository.foldings.Values.Where (x => x.BaseZipCode == baseZipCode);
		}

		public static SwissPostZipCodeFolding Resolve(string zipCode, string zipComplement)
		{
			return SwissPostZipCodeFoldingRepository.Resolve (InvariantConverter.ParseInt (zipCode), InvariantConverter.ParseInt (zipComplement));
		}

		public static SwissPostZipCodeFolding Resolve(int zipCode, int zipComplement)
		{
			SwissPostZipCodeFolding folding;

			SwissPostZipCodeFoldingRepository.foldings.TryGetValue (new SwissPostFullZip (zipCode, zipComplement), out folding);

			return folding;
		}

		
		private static readonly Dictionary<SwissPostFullZip, SwissPostZipCodeFolding> foldings;
	}
}

