//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Tools;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Aider.Entities
{
	public partial class eCH_AddressEntity
	{
		public override FormattedText GetSummary()
		{
			var lines = this.GetJoinedAddressLines ("\n");

			return TextFormatter.FormatText (lines);
		}


		public override FormattedText GetCompactSummary()
		{
			var lines = this.GetJoinedAddressLines (" ");

			return TextFormatter.FormatText (lines);
		}


		private string GetJoinedAddressLines(string separator)
		{
			return this.GetAddressLines ()
				.Where (l => !l.IsNullOrWhiteSpace ())
				.Join (separator);
		}


		private IEnumerable<string> GetAddressLines()
		{
			yield return this.AddressLine1;

			if (string.IsNullOrEmpty (this.StreetUserFriendly) == false)
			{
				yield return StringUtils.Join (" ", this.StreetUserFriendly, this.HouseNumber);
			}

			if (string.IsNullOrEmpty (this.Town) == false)
			{
				yield return StringUtils.Join (" ", this.SwissZipCode, this.Town);
			}

			if ((this.Country != "CH") &&
				(string.IsNullOrEmpty (this.Country) == false))
			{
				yield return IsoCountryNames.Instance[this.Country];
			}
		}
		
		partial void GetStreetUserFriendly(ref string value)
		{
			value = SwissPostStreet.ConvertToUserFriendlyStreetName (this.Street);
		}

		partial void SetStreetUserFriendly(string value)
		{
			if (this.SwissZipCode == 0)
			{
				this.Street = value;
			}
			else
			{
				this.Street = SwissPostStreet.ConvertFromUserFriendlyStreetName (this.SwissZipCode, this.SwissZipCodeAddOn, value) ?? value;
			}
		}
	}
}
