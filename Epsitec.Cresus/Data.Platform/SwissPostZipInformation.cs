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
	}
}
