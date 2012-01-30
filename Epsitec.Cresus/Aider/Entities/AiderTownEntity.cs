using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;


namespace Epsitec.Aider.Entities
{


	public partial class AiderTownEntity
	{


		public override FormattedText GetSummary()
		{
			var text = StringUtils.Join
			(
				", ",
				StringUtils.Join
				(
					" ",
					StringUtils.Join ("-", this.Country.IsoCode, this.ZipCode),
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
