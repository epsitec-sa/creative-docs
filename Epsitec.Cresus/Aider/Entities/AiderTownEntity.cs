using Epsitec.Common.Types;

using Epsitec.Cresus.Core;


namespace Epsitec.Aider.Entities
{
	
	
	public partial class AiderTownEntity
	{


		public override FormattedText GetSummary()
		{
			var text = string.Join
			(
				", ",
				string.Join
				(
					" ",
					string.Join ("-", this.Country.IsoCode, this.SwissZipCode),
					this.Name
				),
				this.Country.Name
			);

			return TextFormatter.FormatText (text);
		}


		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}


	}


}
