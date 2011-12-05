using Epsitec.Common.Types;

using Epsitec.Cresus.Core;


namespace Epsitec.Aider.Entities
{


	public partial class AiderPersonEntity
	{


		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
			(
				"DisplayName: ", this.DisplayName, "\n",
				"CallName: ", this.CallName
			);
		}


		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.CallName);
		}
		

	}


}
