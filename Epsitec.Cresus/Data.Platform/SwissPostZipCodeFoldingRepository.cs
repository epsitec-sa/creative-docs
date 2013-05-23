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
			SwissPostZipCodeFoldingRepository.foldings = new Dictionary<int, SwissPostZipCodeFolding> ();
			
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var path     = "Epsitec.Data.Platform.DataFiles.ZipCodeFolding.zip";
			var lines    = Epsitec.Common.IO.ZipFile.DecompressTextFile (assembly, path, System.Text.Encoding.Default).Split ('\r', '\n').Where (x => x.Length > 0);

			foreach (var item in lines.Select (x => SwissPostZipCodeFolding.Parse (x)))
			{
				SwissPostZipCodeFolding folding;

				if (SwissPostZipCodeFoldingRepository.foldings.TryGetValue (item.ZipCode, out folding))
				{
					System.Diagnostics.Debug.Assert (folding.BaseZipCode == item.BaseZipCode);

					SwissPostZipCodeFoldingRepository.foldings[item.ZipCode] = new SwissPostZipCodeFolding (item.ZipCode, item.BaseZipCode, SwissPostZipType.Mixed);
				}
				else
				{
					SwissPostZipCodeFoldingRepository.foldings[item.ZipCode] = item;
				}
			}
		}

		public static IEnumerable<SwissPostZipCodeFolding> FindDerived(int baseZipCode)
		{
			return SwissPostZipCodeFoldingRepository.foldings.Values.Where (x => x.BaseZipCode == baseZipCode);
		}

		public static SwissPostZipCodeFolding Resolve(string zipCode)
		{
			return SwissPostZipCodeFoldingRepository.Resolve (InvariantConverter.ParseInt (zipCode));
		}

		public static SwissPostZipCodeFolding Resolve(int zipCode)
		{
			SwissPostZipCodeFolding folding;

			SwissPostZipCodeFoldingRepository.foldings.TryGetValue (zipCode, out folding);

			return folding;
		}

		
		private static readonly Dictionary<int, SwissPostZipCodeFolding> foldings;
	}
}

