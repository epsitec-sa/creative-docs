using Epsitec.Aider.Entities;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;


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
			this.CheckCount (entity);
			this.CheckRegionalEdition (entity);
		}


		public override void ApplyUpdateRule(AiderSubscriptionEntity entity)
		{
			entity.RefreshCache ();
		}


		private void CheckCount(AiderSubscriptionEntity entity)
		{
			if (entity.Count < 1)
			{
				var message = "Le nombre d'exemplaires minimal est de 1.";

				Logic.BusinessRuleException (entity, message);
			}
		}


		private void CheckRegionalEdition(AiderSubscriptionEntity entity)
		{
			if (entity.RegionalEdition.IsNull ())
			{
				var message = "Le cahier est obligatoire.";

				Logic.BusinessRuleException (entity, message);
			}

			if (!entity.RegionalEdition.IsRegion ())
			{
				var message = "Le cahier doit être une région.";

				Logic.BusinessRuleException (entity, message);
			}
		}


		private static readonly string generatorSuffix = "BN_ID";


		private static readonly long generatorStartId = 1000000;


	}


}
