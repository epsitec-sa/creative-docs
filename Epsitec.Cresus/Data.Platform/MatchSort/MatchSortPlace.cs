using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Data.Platform.MatchSort
{
	public sealed class MatchSortPlace
	{
		public MatchSortPlace(string id,string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9, string p10, string p11, string p12, string p13, string p14, string p15, string p16, string p17,string p18,string p19,string p20,string p21)
		{
			this.placeId = id;
			this.bfsNumber = p1;
			this.officialCommunityName = p2;
			this.cantonAbbreviation = p3;
			this.agglomerationNumber = p4;
			this.zipType = (SwissPostZipType)Convert.ToInt32(p5);
			this.zip = p6;
			this.zipExtraDigit = p7;
			this.rootZip = p8;
			this.cityLine18 = p9;
			this.cityLine27 = p10;
			this.canton = p11;
			this.primaryLanguage = (SwissPostLanguageCode)Convert.ToInt32(p12);
			this.secondLanguage = (SwissPostLanguageCode)Convert.ToInt32(p13);
			this.briefsBy = p14;
			this.validFromDate = DateTime.ParseExact(p15,"yyyyMMdd",null);
			this.barCodeLabel = p16;
			if(p17=="J")
			{
				this.isOfficialZip = true;
			}
			else
			{
				this.isOfficialZip = false;
			}
			
			this.cityLineAlternativeType = p18;
            this.cityLine18Alternative = p19;
			this.cityLine27Alternative = p20;
			this.primaryLanguageAlternative = (SwissPostLanguageCode)Convert.ToInt32(p21);
		}

		public readonly string placeId;
		public readonly string bfsNumber;
        public readonly string officialCommunityName;
        public readonly string cantonAbbreviation;
        public readonly string agglomerationNumber;
		public readonly SwissPostZipType zipType;                             
        public readonly string zip;
        public readonly string zipExtraDigit;
		public readonly string rootZip;
        public readonly string cityLine18;
        public readonly string cityLine27;
        public readonly string cityLineAlternativeType;
        public readonly string cityLine18Alternative;
        public readonly string cityLine27Alternative;
        public readonly string canton;
        public readonly SwissPostLanguageCode primaryLanguage;
        public readonly SwissPostLanguageCode primaryLanguageAlternative;
        public readonly SwissPostLanguageCode secondLanguage;
        public readonly string briefsBy;
        public readonly DateTime validFromDate;
        public readonly string barCodeLabel;
		public readonly bool isOfficialZip;
	}
}
