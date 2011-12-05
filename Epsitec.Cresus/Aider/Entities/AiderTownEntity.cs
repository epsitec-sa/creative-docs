using Epsitec.Common.Types;

using Epsitec.Cresus.Core;


namespace Epsitec.Aider.Entities
{
	
	
	public partial class AiderTownEntity
	{


		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
			(
				"Nom: ", this.Name, "\n",
				"Code postal: ", this.SwissZipCode, "\n",
				"Pays: ", this.Country.Name
			);
		}


		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Country.IsoCode, "-", this.SwissZipCode, " ", this.Name);
		}


	}


}
