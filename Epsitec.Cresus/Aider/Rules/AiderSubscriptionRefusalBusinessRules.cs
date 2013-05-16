using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;


namespace Epsitec.Aider.Rules
{


	[BusinessRule]
	public sealed class AiderSubscriptionRefusalBusinessRules : GenericBusinessRule<AiderSubscriptionRefusalEntity>
	{


		public override void ApplyUpdateRule(AiderSubscriptionRefusalEntity entity)
		{
			entity.RefreshCache ();
		}


	}


}
