using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities
{
	
	
	public partial class eCH_AddressEntity
	{


		public override FormattedText GetSummary()
		{
			var lines = this.GetConcanatedAddressLines("\n");

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
			yield return string.Join (" ", this.Street, this.HouseNumber);
			yield return string.Join (" ", this.SwissZipCode, this.Town);
			yield return this.Country;
		}


	}


}
