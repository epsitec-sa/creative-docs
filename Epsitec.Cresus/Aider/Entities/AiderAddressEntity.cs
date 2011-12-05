using Epsitec.Common.Types;

using Epsitec.Cresus.Core;


namespace Epsitec.Aider.Entities
{


	public partial class AiderAddressEntity
	{


		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
			(
				"Type: ", this.Type, "\n",
				"Description: ", this.Description, "\n",
				"AddressLine1: ", this.AddressLine1, "\n",
				"Postbox: ", this.PostBox, "\n",
				"Street: ", this.Street, "\n",
				"HouseNumber: ", this.HouseNumber, "\n",
				"HouseNumberComplement", this.HouseNumberComplement, "\n",
				"Town: ", this.Town.GetCompactSummary(), "\n",
				"Phone1: ", this.Phone1, "\n",
				"Phone2: ", this.Phone2, "\n",
				"Email: ", this.Email
			);
		}


		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Street, ", ", this.Town.GetCompactSummary ());
		}


	}


}
