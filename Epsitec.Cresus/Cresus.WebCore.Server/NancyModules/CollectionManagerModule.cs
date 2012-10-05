using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Allows to add or delete an entity within a collection
	/// </summary>
	public class CollectionManagerModule : AbstractAuthenticatedModule
	{


		public CollectionManagerModule(CoreServer coreServer)
			: base (coreServer, "/collection")
		{
			Post["/delete"] = p => this.Execute (b => this.DeleteEntity (b));
			Post["/create"] = p => this.Execute (b => this.CreateEntity (b));
		}


		private Response DeleteEntity(BusinessContext businessContext)
		{
			string parentEntityId = Request.Form.parentEntityId;

			AbstractEntity parentEntity = Tools.ResolveEntity (businessContext, parentEntityId);

			string deletedEntityId = Request.Form.deletedEntityId;
			var deletedKey = EntityKey.Parse (deletedEntityId);

			var propertyAccessorId = InvariantConverter.ParseInt ((string) Request.Form.propertyAccessorId);
			var propartyAccessor = this.CoreServer.PropertyAccessorCache.Get (propertyAccessorId) as EntityCollectionPropertyAccessor;

			if (propartyAccessor == null)
			{
				return CoreResponse.Failure ();
			}

			var collection = propartyAccessor.GetCollection (parentEntity);

			var toDelete = collection.Cast<AbstractEntity> ().Where (c => businessContext.DataContext.GetNormalizedEntityKey (c).Equals (deletedKey));

			if (toDelete.IsEmpty ())
			{
				return CoreResponse.Failure ();
			}

			using (businessContext.Bind (parentEntity))
			{
				var d = toDelete.First ();

				collection.Remove (d);
				businessContext.DeleteEntity (d);

				businessContext.SaveChanges (LockingPolicy.KeepLock);
			}

			return CoreResponse.Success ();
		}


		private Response CreateEntity(BusinessContext businessContext)
		{
			string parentEntityId = Request.Form.parentEntityId;
			var parentEntity = Tools.ResolveEntity (businessContext, parentEntityId);

			string typeName = Request.Form.entityType;
			var type = Tools.ParseType (typeName);
			var method = typeof (BusinessContext).GetMethod ("CreateEntity", new Type[0]);
			var m = method.MakeGenericMethod (type);
			var o = m.Invoke (businessContext, new object[0]);
			var newEntity = o as AbstractEntity;

			var propertyAccessorId = InvariantConverter.ParseInt ((string) Request.Form.propertyAccessorId);
			var propertyAccessor = this.CoreServer.PropertyAccessorCache.Get (propertyAccessorId) as EntityCollectionPropertyAccessor;

			if (propertyAccessor == null)
			{
				return CoreResponse.Failure ();
			}

			using (businessContext.Bind (parentEntity, newEntity))
			{
				var collection = propertyAccessor.GetCollection (parentEntity);
				collection.Add (newEntity);

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
			}

			var key = Tools.GetEntityId (businessContext, newEntity);

			var content = new Dictionary<string, object> () {
				{ "key", key }
			};

			return CoreResponse.Success (content);
		}


	}


}
