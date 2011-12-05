using Epsitec.Common.Types;

using Epsitec.Cresus.Core;


namespace Epsitec.Aider.Entities
{


	public partial class AiderCountryEntity
	{


		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
			(
				"Nom: ", this.Name, "\n",
				"Code: ", this.IsoCode 
			);
		}


		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name , " (", this.IsoCode, ")");
		}


	}


}
