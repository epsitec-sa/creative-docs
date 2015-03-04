//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

namespace Epsitec.Data.Platform
{
	public sealed class SwissPostHouseInformation
	{
		/// <summary>
		/// Return corresponding Mat[CH]Sort datatable id (REC_ART)
		/// </summary>
		/// <returns></returns>
		public static string GetMatchRecordId()
		{
			return "06";
		}

		public override string ToString()
		{
			return string.Format ("{1}{2}", this.HouseNumber, this.HouseLetter);
		}

		public int			HouseCode
		{
			get;
			set;
		}
		public int			StreetCode
		{
			get;
			set;
		}
		public int			HouseNumber
		{
			get;
			set;
		}
		public string		HouseLetter
		{
			get;
			set;
		}
		public string		OfficialHouse
		{
			get;
			set;
		}
	}
}
