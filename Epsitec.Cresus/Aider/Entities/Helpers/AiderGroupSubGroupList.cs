using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;


namespace Epsitec.Aider.Entities.Helpers
{


	public sealed class AiderGroupSubGroupList : VirtualEventList<AiderGroupEntity, AiderGroupEntity>
	{


		public AiderGroupSubGroupList(AiderGroupEntity entity)
			: base (entity)
		{
		}


		protected override IEnumerable<AiderGroupEntity> GetItems()
		{
			var businessContext = BusinessContextPool.GetCurrentContext (this.Entity);

			return this.Entity.FindSubgroups (businessContext);
		}


		protected override void HandleCollectionAddition(AiderGroupEntity item)
		{
			this.Entity.SetupSubGroup (item, this.Entity.GetNextSubGroupNumber ());
		}


		protected override void HandleCollectionRemoval(AiderGroupEntity item)
		{
		}


	}


}
