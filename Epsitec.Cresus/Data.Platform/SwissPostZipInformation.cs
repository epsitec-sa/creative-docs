//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>SwissPostZipInformation</c> class describes a MAT[CH]zip entry provided in
	/// the ZIP plus 1 format.
	/// </summary>
	public sealed class SwissPostZipInformation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SwissPostZipInformation"/> class
		/// based on a source line taken from MAT[CH] zip.
		/// http://www.post.ch/en/post-startseite/post-adress-services-match/post-direct-marketing-datengrundlage/post-direct-marketing-match-zip.htm
		/// http://www.post.ch/en/post-startseite/post-adress-services-match/post-direct-marketing-datengrundlage/post-direct-marketing-match-zip/post-match-zip-factsheet.pdf
		/// </summary>
		/// <param name="line">The line.</param>
		public SwissPostZipInformation(string line)
		{
			string[] args = line.Split ('\t');

			int date = InvariantConverter.ParseInt (args[12]);

			this.OnrpCode       = InvariantConverter.ParseInt (args[0]);
			this.ZipType        = InvariantConverter.ParseInt<SwissPostZipType> (args[1]);
			this.ZipCode        = InvariantConverter.ParseInt (args[2]);
			this.ZipComplement  = InvariantConverter.ParseInt (args[3]);
			this.ShortName      = args[4];
			this.LongName       = args[5];
			this.Canton         = args[6];
			this.LanguageCode1  = InvariantConverter.ParseInt<SwissPostLanguageCode> (args[7]);
			this.LanguageCode2  = InvariantConverter.ParseInt<SwissPostLanguageCode> (args[8]);
			this.DistributionBy = args[10];
			this.ComunityCode   = InvariantConverter.ParseInt (args[11]);
			this.ValidSince     = new Date (year: date / 10000, month: (date/100) % 100, day: date % 100);

			string shortName = this.ShortName;
			
			if (shortName.EndsWith (" " + this.Canton))
			{
				shortName = shortName.Substring (0, shortName.Length - 3);
			}

			this.alternateName = SwissPostZipInformation.ConvertToAlternateName (shortName);
		}


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
		public readonly int						ZipComplement;
		public readonly string					ShortName;
		public readonly string					LongName;
		public readonly string					Canton;
		public readonly SwissPostLanguageCode LanguageCode1;
		public readonly SwissPostLanguageCode LanguageCode2;
		public readonly string					DistributionBy;
		public readonly int						ComunityCode;
		public readonly Date					ValidSince;

		private readonly string					alternateName;
	}
}