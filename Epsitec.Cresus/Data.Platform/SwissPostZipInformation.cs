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
		/// 
		public bool MatchName(string name)
		{
			return this.MatchAlternateName (SwissPostZipInformation.ConvertToAlternateName (name))
				|| string.Equals (name, this.LongName, System.StringComparison.OrdinalIgnoreCase);
		}

		internal bool MatchAlternateName(string altName)
		{
			return SwissPostZipInformation.ConvertToAlternateName (this.ShortName) == altName;
		}

		public static string ConvertToAlternateName(string shortName)
		{
			return TextConverter.ConvertToUpperAndStripAccents (shortName);
		}

		/// <summary>
		/// Return corresponding Mat[CH]Sort datatable id (REC_ART)
		/// </summary>
		/// <returns></returns>
		public static string GetMatchRecordId()
		{
			return "01";
		}

		public override string ToString()
		{
			return string.Format ("{0:0000} {1} ({2})", this.ZipCode, this.LongName, this.Canton);
		}


		public int OnrpCode
		{
			get;
			set;
		}
		public  SwissPostZipType		ZipType
		{
			get;
			set;
		}
		public  int						ZipCode
		{
			get;
			set;
		}
		public  int						ZipCodeAddOn
		{
			get;
			set;
		}
		public  int						RootZipCode
		{
			get;
			set;
		}
		public  string					ShortName
		{
			get;
			set;
		}
		public  string					LongName
		{
			get;
			set;
		}
		public  string					Canton
		{
			get;
			set;
		}
		public  SwissPostLanguageCode	LanguageCode1
		{
			get;
			set;
		}
		public  SwissPostLanguageCode	LanguageCode2
		{
			get;
			set;
		}
		public  bool					InSortFile
		{
			get;
			set;
		}
		public  int					    DeliveryOnrp
		{
			get;
			set;
		}
		public  int					    DeliveryZipCode
		{
			get;
			set;
		}
		public  string					OnlyOfficial
		{
			get;
			set;
		}
		public  int						CommunityCode
		{
			get;
			set;
		}
		public  Date					ValidSince
		{
			get;
			set;
		}
	}
}
