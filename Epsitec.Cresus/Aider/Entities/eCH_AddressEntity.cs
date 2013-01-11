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
			var lines = this.GetConcanatedAddressLines ("\n");

			return TextFormatter.FormatText (lines);
		}


		public override FormattedText GetCompactSummary()
		{
			var lines = this.GetConcanatedAddressLines (" ");

			return TextFormatter.FormatText (lines);
		}


		private string GetConcanatedAddressLines(string separator)
		{
			return this.GetAddressLines ()
				.Where (l => !l.IsNullOrWhiteSpace ())
				.Join (separator);
		}


		private IEnumerable<string> GetAddressLines()
		{
			yield return this.AddressLine1;
			yield return StringUtils.Join (" ", this.StreetUserFriendly, this.HouseNumber);
			yield return StringUtils.Join (" ", this.SwissZipCode, this.Town);
			yield return IsoCountryNames.Instance[this.Country];
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
				this.Street = SwissPostStreet.ConvertFromUserFriendlyStreetName (this.SwissZipCode, value) ?? value;
			}
		}
	}
}
