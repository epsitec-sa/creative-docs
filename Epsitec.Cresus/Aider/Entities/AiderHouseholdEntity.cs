using Epsitec.Common.Types;

using Epsitec.Cresus.Core;


namespace Epsitec.Aider.Entities
{


	public partial class AiderHouseholdEntity
	{


		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
			(
				"Head1: ", this.Head1.GetCompactSummary (), "\n",
				"Head2: ", this.Head2.GetCompactSummary (), "\n",
				"Address: ", this.Address.GetCompactSummary ()
			);
		}


		public override FormattedText GetCompactSummary()
		{
			return this.Head1.GetCompactSummary ();
		}

	
	
	}


}
