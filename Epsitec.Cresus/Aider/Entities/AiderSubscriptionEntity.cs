using Epsitec.Common.Types;


namespace Epsitec.Aider.Entities
{


	public partial class AiderSubscriptionEntity
	{


		public override FormattedText GetSummary()
		{
			return "Subscription summary";
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}


	}


}
