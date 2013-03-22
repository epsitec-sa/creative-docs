using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Data.Platform
{
	public sealed class MatchSortHouse
	{
		public MatchSortHouse(string id,string p1,string p2,string p3,string p4,string p5,string p6,string p7,string p8,string p9,string p10)
		{
			this.streetId = id;
			this.houseNumber = p1;
			this.houseNumberAlphaNum = p2;
			if(p3=="J")
			{
				this.isOfficialHouseNumber = true;
			}
			else
			{
				this.isOfficialHouseNumber = false;
			}
			
			this.additionalDescription = p4;
			this.houseAlternativeType = p5;
			this.zipDistrictMessenger  = p6;
			this.messengerDistrictNumber = p7;
			this.stageNumber = p8;
			this.runningNumber = p9;
			this.depotNumber = p10;
		}

		public readonly string streetId;
		public readonly string houseNumber;
        public readonly string houseNumberAlphaNum;
        public readonly bool isOfficialHouseNumber;
        public readonly string additionalDescription;
        public readonly string houseAlternativeType;
        public readonly string zipDistrictMessenger;
        public readonly string messengerDistrictNumber;
        public readonly string stageNumber;
        public readonly string runningNumber;
		public readonly string depotNumber;


	}
}
