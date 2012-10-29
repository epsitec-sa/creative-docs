using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities.Helpers
{
	
	public sealed class AiderPersonGroupList : VirtualEventList<AiderPersonEntity, AiderGroupParticipantEntity>
	{


		public AiderPersonGroupList(AiderPersonEntity entity)
			: base (entity)
		{
		}


		protected override IEnumerable<AiderGroupParticipantEntity> GetItems()
		{
			var businessContext = BusinessContextPool.GetCurrentContext (this.entity);
			var dataContext = businessContext.DataContext;

			var example = new AiderGroupParticipantEntity ()
			{
				Group = new AiderGroupEntity (),
			};

			var request = new Request ()
			{
				RootEntity = example,
			};

			request.AddCondition (dataContext, example, g => g.Person == this.entity);
			request.AddCondition (dataContext, example, g => g.StartDate == null || g.StartDate <= Date.Today);
			request.AddCondition (dataContext, example, g => g.EndDate == null || g.EndDate > Date.Today);

			request.AddSortClause (ValueField.Create (example.Group, g => g.Name), SortOrder.Ascending);

			return dataContext.GetByRequest<AiderGroupParticipantEntity> (request);
		}


		protected override void HandleCollectionAddition(AiderGroupParticipantEntity item)
		{
			item.Person = this.entity;
			item.StartDate = Date.Today;
		}


		protected override void HandleCollectionRemoval(AiderGroupParticipantEntity item)
		{
			var businessContext = BusinessContextPool.GetCurrentContext (this.entity);

			var newParticipation = businessContext.CreateEntity<AiderGroupParticipantEntity> ();

			newParticipation.Person = item.Person;
			newParticipation.Group = item.Group;
			newParticipation.StartDate = item.StartDate;
			newParticipation.EndDate = Date.Today;
		}


	}


}
