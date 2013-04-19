using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;


namespace Epsitec.Aider.Rules
{


	[BusinessRule]
	public sealed class AiderSubscriptionBusinessRules : GenericBusinessRule<AiderSubscriptionEntity>
	{


		public override void ApplySetupRule(AiderSubscriptionEntity entity)
		{
			this.AssignId (entity);
		}


		private void AssignId(AiderSubscriptionEntity entity)
		{
			var generator = this.GetRefIdGenerator ();
			var id = generator.GetNextId ();

			entity.Id = InvariantConverter.ToString (id);
		}


		private RefIdGenerator GetRefIdGenerator()
		{
			var generatorPool = this.GetRefIdGeneratorPool ();

			return generatorPool.GetGenerator<AiderSubscriptionEntity>
			(
				AiderSubscriptionBusinessRules.generatorSuffix,
				AiderSubscriptionBusinessRules.generatorStartId
			);
		}


		public override void ApplyValidateRule(AiderSubscriptionEntity entity)
		{
			this.CheckRegionalEdition (entity);
		}


		private void CheckRegionalEdition(AiderSubscriptionEntity entity)
		{
			if (!entity.RegionalEdition.IsRegion ())
			{
				var message = "L'édition régionale doit être un groupe de région";

				throw new BusinessRuleException (message);
			}
		}


		private static readonly string generatorSuffix = "BN_ID";


		private static readonly long generatorStartId = 1000000;


	}


}
