using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Expressions;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{
	
	
	internal sealed class Database<T> : Database
		where T : AbstractEntity, new()
	{


		public Database(string title, string name, string iconClass)
			: base (title, name, iconClass)
		{
		}


		public override Dictionary<string, object> GetEntityData(BusinessContext businessContext, AbstractEntity entity)
		{
			var id = Tools.GetEntityId (businessContext, entity);
			var summary = entity.GetCompactSummary ().ToSimpleText ();

			return new Dictionary<string, object> ()
			{
				{ "id", id },
				{ "summary", summary },
			};
		}


		public override IEnumerable<AbstractEntity> GetEntities(BusinessContext businessContext, int skip, int take)
		{
			var example = new T ();

			var request = new DataLayer.Loader.Request ()
			{
				RootEntity = example,
				Skip = skip,
				Take = take,
			};

			request.AddSortClause
			(
				InternalField.CreateId (example),
				SortOrder.Ascending
			);

			return businessContext.DataContext.GetByRequest<T> (request);
		}


		public override int GetCount(BusinessContext businessContext)
		{
			return businessContext.DataContext.GetCount (new T ());
		}


		public override AbstractEntity CreateEntity(BusinessContext businessContext)
		{
			var entity = businessContext.CreateEntity<T> ();

			// NOTE Here we need to include the empty entities, otherwise we might be in the case
			// where the entity that we just have created will be empty and thus not saved and this
			// will lead the user to click like a maniac on the "create" button without noticeable
			// result other than him becoming mad :-P

			businessContext.SaveChanges (EntitySaveMode.IncludeEmpty);

			return entity;
		}


		public override bool DeleteEntity(BusinessContext businessContext, AbstractEntity entity)
		{
			using (businessContext.Bind (entity))
			{
				return businessContext.DeleteEntity (entity);
			}
		}


	}


}
