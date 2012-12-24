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
				}
			}

			return entityId;
		}


		public static EntityKey? ParseEntityId(string entityId)
		{
			return EntityKey.Parse (entityId);
		}


		public static AbstractEntity ResolveEntity(BusinessContext businessContext, string entityId)
		{
			var entityKey = EntityIO.ParseEntityId (entityId);

			return businessContext.DataContext.ResolveEntity (entityKey);
		}


		public static IEnumerable<AbstractEntity> ResolveEntities(BusinessContext businessContext, string entityIds)
		{
			return from id in entityIds.Split (';')
				   where !string.IsNullOrEmpty (id)
				   select EntityIO.ResolveEntity (businessContext, id);
		}


	}


}
