//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Samuel LOUP

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>SwissPostZipInformation</c> class describes a MAT[CH]Sort NEW_PLZ1 entries
	/// https://www.post.ch/fr/post-startseite/post-adress-services-match/post-direct-marketing-datengrundlage/pm-sortierfile-angebote-datenstrukturen.pdf
	/// </summary>
	public sealed class SwissPostZipInformation
	{
		/// <summary>
		/// Check if the name matches this instance.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns><c>true</c> if the name matches this instance; otherwise, <c>false</c>.</returns>
		public bool MatchName(string name)
		{
			return this.MatchAlternateName (SwissPostZipInformation.ConvertToAlternateName (name))
				|| string.Equals (name, this.LongName, System.StringComparison.OrdinalIgnoreCase);
		}

		internal bool MatchAlternateName(string altName)
		{
			return this.alternateName == altName;
		}

		public static string ConvertToAlternateName(string shortName)
		{
			return TextConverter.ConvertToUpperAndStripAccents (shortName);
		}

		public override string ToString()
		{
			return string.Format ("{0:0000} {1} ({2})", this.ZipCode, this.LongName, this.Canton);
		}

		
		public readonly int						OnrpCode;
		public readonly SwissPostZipType		ZipType;
		public readonly int						ZipCode;
		public readonly int						ZipCodeAddOn;
		public readonly string					ShortName;
		public readonly string					LongName;
		public readonly string					Canton;
		public readonly SwissPostLanguageCode	LanguageCode1;
		public readonly SwissPostLanguageCode	LanguageCode2;
		public readonly bool					InSortFile;
		public readonly string					DistributionBy;
		public readonly int						ComunityCode;
		public readonly Date					ValidSince;

		private readonly string					alternateName;
	}
}
