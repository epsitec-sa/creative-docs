//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>SwissPostZipInformation</c> class describes the MAT[CH]zip data provided in
	/// the ZIP plus 1 format.
	/// </summary>
	public sealed class SwissPostZipInformation
	{
		public SwissPostZipInformation(string line)
		{
			string[] args = line.Split ('\t');

			this.OnrpCode       = args[0];
			this.ZipType        = args[1];
			this.ZipCode        = args[2];
			this.ZipComplement  = args[3];
			this.ShortName      = args[4];
			this.LongName       = args[5];
			this.Canton         = args[6];
			this.LanguageCode1  = args[7];
			this.LanguageCode2  = args[8];
			this.MatchSort      = args[9];
			this.DistributionBy = args[10];
			this.ComunityCode   = args[11];
			this.ValidSince     = args[12];

			string shortName = this.ShortName;
			
			if (shortName.EndsWith (" " + this.Canton))
			{
				shortName = shortName.Substring (0, shortName.Length - 3);
			}
			else if (shortName.Contains ("Goumoens"))
			{
				shortName = shortName.Replace ("Goumoens", "Goumoëns");
			}

			this.alternateName = shortName == this.LongName ? null : shortName;
		}

		public bool MatchName(string name)
		{
			return string.Equals (name, this.LongName, System.StringComparison.OrdinalIgnoreCase)
				|| (this.alternateName != null && string.Equals (name, this.alternateName, System.StringComparison.OrdinalIgnoreCase));
		}

		public string							OnrpCode;
		public string							ZipType;
		public string							ZipCode;
		public string							ZipComplement;
		public string							ShortName;
		public string							LongName;
		public string							Canton;
		public string							LanguageCode1;
		public string							LanguageCode2;
		public string							MatchSort;
		public string							DistributionBy;
		public string							ComunityCode;
		public string							ValidSince;

		private readonly string alternateName;
	}
}
