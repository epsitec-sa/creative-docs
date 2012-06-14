using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor;

using Nancy;

using System;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Allows to add or delete an entity within a collection
	/// </summary>
	public class CollectionManagerModule : AbstractCoreSessionModule
	{


		public CollectionManagerModule(ServerContext serverContext)
			: base (serverContext, "/collection")
		{
			Post["/delete"] = p => this.ExecuteWithCoreSession (cs => this.DeleteEntity (cs));
			Post["/create"] = p => this.ExecuteWithCoreSession (cs => this.CreateEntity (cs));
		}


		private Response DeleteEntity(CoreSession coreSession)
		{
			var context = coreSession.GetBusinessContext ();

			string parentEntityId = Request.Form.parentEntity;

			AbstractEntity parentEntity = Tools.ResolveEntity (context, parentEntityId);

			string deleteEntity = Request.Form.deleteEntity;
			var deleteKey = EntityKey.Parse (deleteEntity);

			string propertyAccessorId = Request.Form.propertyAccessorId;
			var propartyAccessor = coreSession.PropertyAccessorCache.Get (propertyAccessorId) as EntityCollectionPropertyAccessor;

			if (propartyAccessor == null)
			{
				return CoreResponse.AsError ();
			}

			var collection = propartyAccessor.GetCollection (parentEntity);

			var toDelete = collection.Cast<AbstractEntity> ().Where (c => context.DataContext.GetNormalizedEntityKey (c).Equals (deleteKey));

			if (toDelete.IsEmpty ())
			{
				return CoreResponse.AsError ();
			}

			using (context.Bind (parentEntity))
			{
				var d = toDelete.First ();

				collection.Remove (d);
				context.DeleteEntity (d);

				context.SaveChanges ();
			}

			return CoreResponse.AsSuccess ();
		}


		private Response CreateEntity(CoreSession coreSession)
		{
			var context = coreSession.GetBusinessContext ();

			string parentEntityId = Request.Form.parentEntity;
			AbstractEntity parentEntity = Tools.ResolveEntity (context, parentEntityId);

			string typeName = Request.Form.entityType;
			var type = Type.GetType (typeName);
			var method = typeof (BusinessContext).GetMethod ("CreateEntity", new Type[0]);
			var m = method.MakeGenericMethod (type);
			var o = m.Invoke (context, new object[0]);
			var newEntity = o as AbstractEntity;

			string propertyAccessorId = Request.Form.propertyAccessorId;
			var propertyAccessor = coreSession.PropertyAccessorCache.Get (propertyAccessorId) as EntityCollectionPropertyAccessor;

			if (propertyAccessor == null)
			{
				return CoreResponse.AsError ();
			}

			using (context.Bind (parentEntity, newEntity))
			{
				var collection = propertyAccessor.GetCollection (parentEntity);
				collection.Add (newEntity);

				context.SaveChanges (EntitySaveMode.IncludeEmpty);
			}

			var key = Tools.GetEntityId (context, newEntity);

			return CoreResponse.AsSuccess (key);
		}


	}


}
