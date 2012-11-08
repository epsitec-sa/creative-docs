using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Aider.Entities.Helpers
{


	public sealed class AiderGroupPersonList : VirtualEventList<AiderGroupEntity, AiderPersonEntity>
	{


		public AiderGroupPersonList(AiderGroupEntity entity)
			: base (entity)
		{
		}


		protected override IEnumerable<AiderPersonEntity> GetItems()
		{
			var businessContext = BusinessContextPool.GetCurrentContext (this.entity);
			var dataContext = businessContext.DataContext;

			var echPerson = new eCH_PersonEntity ();

			var aiderPerson = new AiderPersonEntity ()
			{
				eCH_Person = echPerson
			};

			var participation = new AiderGroupParticipantEntity ()
			{
				Person = aiderPerson
			};

			var request = new Request ()
			{
				RootEntity = participation,
				RequestedEntity = aiderPerson,
			};

			request.AddCondition (dataContext, participation, g => g.Group == this.entity);
			request.AddCondition (dataContext, participation, g => g.StartDate == null || g.StartDate <= Date.Today);
			request.AddCondition (dataContext, participation, g => g.EndDate == null || g.EndDate > Date.Today);

			request.AddSortClause (ValueField.Create (echPerson, p => p.PersonOfficialName), SortOrder.Ascending);
			request.AddSortClause (ValueField.Create (echPerson, p => p.PersonFirstNames), SortOrder.Ascending);

			return dataContext.GetByRequest<AiderPersonEntity> (request);
		}


		protected override void HandleCollectionAddition(AiderPersonEntity item)
		{
			var businessContext = BusinessContextPool.GetCurrentContext (this.entity);

			var participation = businessContext.CreateAndRegisterEntity<AiderGroupParticipantEntity> ();

			participation.StartDate = Date.Today;
			participation.Group = this.entity;
			participation.Person = item;
		}


		protected override void HandleCollectionRemoval(AiderPersonEntity item)
		{
			var businessContext = BusinessContextPool.GetCurrentContext (this.entity);
			var dataContext = businessContext.DataContext;

			var participation = new AiderGroupParticipantEntity ();
			var request = Request.Create<AiderGroupParticipantEntity> (participation);

			request.AddCondition (dataContext, participation, p => p.Group == this.entity);
			request.AddCondition (dataContext, participation, p => p.Person == item);
			request.AddCondition (dataContext, participation, p => p.EndDate == null);

			foreach (var entity in dataContext.GetByRequest (request))
			{
				entity.EndDate = Date.Today;
			}
		}


	}


}
