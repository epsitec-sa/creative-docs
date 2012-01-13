using Epsitec.Common.Types;

using Epsitec.Cresus.Core;


namespace Epsitec.Aider.Entities
{


	public partial class AiderCountryEntity
	{


		public override FormattedText GetSummary()
		{
			var text = this.Name;

			if (this.IsoCode != null)
			{
				text += " (" + this.IsoCode + ")";
			}

			return TextFormatter.FormatText (text);
		}


		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}


	}


}
