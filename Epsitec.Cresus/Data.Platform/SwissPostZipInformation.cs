//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

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

			this.alternateName = TextConverter.ConvertToUpperAndStripAccents (shortName);
		}


		/// <summary>
		/// Check if the name matches this instance.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns><c>true</c> if the name matches this instance; otherwise, <c>false</c>.</returns>
		public bool MatchName(string name)
		{
			var altName = TextConverter.ConvertToUpperAndStripAccents (name);

			return (this.alternateName == altName)
				|| string.Equals (name, this.LongName, System.StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString()
		{
			return string.Concat (this.ZipCode, " ", this.LongName, " (", this.Canton, ")");
		}

		
		public readonly string					OnrpCode;
		public readonly string					ZipType;
		public readonly string					ZipCode;
		public readonly string					ZipComplement;
		public readonly string					ShortName;
		public readonly string					LongName;
		public readonly string					Canton;
		public readonly string					LanguageCode1;
		public readonly string					LanguageCode2;
		public readonly string					MatchSort;
		public readonly string					DistributionBy;
		public readonly string					ComunityCode;
		public readonly string					ValidSince;

		private readonly string					alternateName;
	}
}
