using Epsitec.Common.Types;

using Epsitec.Cresus.Core;


namespace Epsitec.Aider.Entities
{


	public partial class AiderPersonRelationshipEntity
	{


		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
			(
				"Person1: ", this.Person1.GetCompactSummary(), "\n",
				"Person2: ", this.Person2.GetCompactSummary(), "\n",
				"Type: ", this.Type, "\n",
				"StartDate: ", this.StartDate, "\n",
				"EndDate: ", this.EndDate, "\n",
				"Comment: ", this.Comment
			);
		}


		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Type, ": ", this.Person1.GetCompactSummary (), " and ", this.Person2.GetCompactSummary ());
		}


	}


}
