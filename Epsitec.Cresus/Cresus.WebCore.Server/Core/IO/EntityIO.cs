using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.IO
{


	internal static class EntityIO
	{


		public static string GetEntityId(BusinessContext businessContext, AbstractEntity entity)
		{
			return EntityIO.GetEntityId (businessContext.DataContext, entity);
		}


		public static string GetEntityId(DataContext dataContext, AbstractEntity entity)
		{
			string entityId = null;

			if (entity != null)
			{
				var entityKey = dataContext.GetNormalizedEntityKey (entity);

				if (entityKey.HasValue)
				{
					entityId = entityKey.Value.ToString ();

					// Here we replace the slash by the dash. That way, the entity ids won't mess up
					// urls like url/with/entityId/inside by introducing an extra slash where it
					// shouldn't.
					entityId = entityId.Replace ('/', '-');
				}
			}

			return entityId;
		}


		public static EntityKey? ParseEntityId(string entityId)
		{
			if ((string.IsNullOrEmpty (entityId)) ||
				(entityId == "null"))
			{
				return null;
			}

			entityId = entityId.Replace ('-', '/');

			return EntityKey.Parse (entityId);
		}


		public static AbstractEntity ResolveEntity(BusinessContext businessContext, string entityId)
		{
			return EntityIO.ResolveEntity (businessContext.DataContext, entityId);
		}


		public static AbstractEntity ResolveEntity(DataContext dataContext, string entityId)
		{
			var entityKey = EntityIO.ParseEntityId (entityId);

			return dataContext.ResolveEntity (entityKey);
		}


		public static IEnumerable<AbstractEntity> ResolveEntities(BusinessContext businessContext, string entityIds)
		{
			return EntityIO.ResolveEntities (businessContext.DataContext, entityIds);
		}


		public static IEnumerable<AbstractEntity> ResolveEntities(DataContext dataContext, string entityIds)
		{
			return from id in entityIds.Split (';')
				   where !string.IsNullOrEmpty (id)
				   select EntityIO.ResolveEntity (dataContext, id);
		}


	}


}
