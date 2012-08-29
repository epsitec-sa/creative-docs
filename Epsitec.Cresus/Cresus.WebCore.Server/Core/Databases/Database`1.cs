using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{
	
	
	internal sealed class Database<T> : Database
		where T : AbstractEntity, new()
	{


		public Database(string title, string name, string iconClass, IEnumerable<Column> columns, IEnumerable<Sorter> sorters)
			: base (title, name, iconClass, columns, sorters)
		{
		}


		public override Dictionary<string, object> GetEntityData(BusinessContext businessContext, AbstractEntity entity, PropertyAccessorCache propertyAccessorCache)
		{
			var id = Tools.GetEntityId (businessContext, entity);
			var summary = entity.GetCompactSummary ().ToSimpleText ();

			var data = new Dictionary<string, object> ()
			{
				{ "id", id },
				{ "summary", summary },
			};

			foreach (var column in this.Columns.Where (c => !c.Hidden))
			{
				var propertyAccessor = propertyAccessorCache.Get (column.LambdaExpression);
				var stringPropertyAccessor = (AbstractStringPropertyAccessor) propertyAccessor;
				var name = column.Name;
				var value = stringPropertyAccessor.GetValue (entity);

				data[name] = value;
			}

			return data;
		}


		public override IEnumerable<AbstractEntity> GetEntities(BusinessContext businessContext, IEnumerable<Sorter> sorters, int skip, int take)
		{
			var example = new T ();

			var request = new DataLayer.Loader.Request ()
			{
				RootEntity = example,
				Skip = skip,
				Take = take,
			};

			request.SortClauses.AddRange (this.CreateSortClauses (example, sorters));

			return businessContext.DataContext.GetByRequest<T> (request);
		}


		private IEnumerable<SortClause> CreateSortClauses(T example, IEnumerable<Sorter> sorters)
		{
			foreach (var sorter in sorters)
			{
				yield return sorter.ToSortClause (example);
			}

			yield return new SortClause (InternalField.CreateId (example), SortOrder.Ascending);
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
