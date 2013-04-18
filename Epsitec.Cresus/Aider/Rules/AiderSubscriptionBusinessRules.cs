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


		private static readonly string generatorSuffix = "BN_ID";


		private static readonly long generatorStartId = 1000000;


	}


}
