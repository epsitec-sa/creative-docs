using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Data.Platform.MatchSort
{
	public sealed class MatchSortStreet
	{

		public MatchSortStreet(string id,string placeId,string p1,string p2, string p3, string p4,string p5,string p6,string p7,string p8,string p9,string p10,string p11)
		{
			this.streetId = id;
			this.placeId = placeId;
			this.publishedStreetNameAbbreviated = p1;
			this.publishedStreetNameAbbreviatedAlternative = p2;
			this.publishedStreetName = p3;
			this.publishedStreetNameAlternative = p4;
			this.streetNameAbbreviated = p5;
			this.streetNameAbbreviatedAlternative = p6;
			this.streetName = p7;
			this.streetNameAlternative = p8;
			this.streetType = p9;
			this.streetLanguage = (SwissPostLanguageCode)Convert.ToInt32(p10);
			if (p11=="J")
			{
				this.isOfficialDesignation = true;
			}
			else
			{
				this.isOfficialDesignation = false;
			}
		}

		public readonly string streetId;
		public readonly string placeId;
		public readonly string publishedStreetNameAbbreviated;
        public readonly string publishedStreetNameAbbreviatedAlternative;
        public readonly string publishedStreetName;
        public readonly string publishedStreetNameAlternative;
		public readonly string streetNameAbbreviated;
		public readonly string streetNameAbbreviatedAlternative;
		public readonly string streetName;
		public readonly string streetNameAlternative;
		public readonly string streetType;
		public readonly SwissPostLanguageCode streetLanguage;
		public readonly bool isOfficialDesignation;
	}
}
