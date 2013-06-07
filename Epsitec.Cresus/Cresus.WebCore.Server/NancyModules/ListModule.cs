﻿using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Allows to add or delete an entity within a collection
	/// </summary>
	public class ListModule : AbstractAuthenticatedModule
	{


		public ListModule(CoreServer coreServer)
			: base (coreServer, "/list")
		{
			Post["/delete"] = p => this.Execute (b => this.DeleteEntity (b));
			Post["/create"] = p => this.Execute (b => this.CreateEntity (b));
		}


		private Response DeleteEntity(BusinessContext businessContext)
		{
			string parentEntityId = Request.Form.parentEntityId;

			var parentEntity = EntityIO.ResolveEntity (businessContext, parentEntityId);

			string deletedEntityId = Request.Form.deletedEntityId;
			var deletedKey = EntityIO.ParseEntityId (deletedEntityId);

			string propertyAccessorId = Request.Form.propertyAccessorId;
			var propertyAccessorCache = this.CoreServer.Caches.PropertyAccessorCache;
			var propertyAccessor = propertyAccessorCache.Get (propertyAccessorId) as EntityCollectionPropertyAccessor;

			if (propertyAccessor == null)
			{
				return CoreResponse.Failure ();
			}

			var dataContext = businessContext.DataContext;
			var collection = (IList) propertyAccessor.GetValue (parentEntity);

			var toDelete = collection
				.Cast<AbstractEntity> ()
				.Where (c => dataContext.GetNormalizedEntityKey (c).Equals (deletedKey));

			if (toDelete.IsEmpty ())
			{
				return CoreResponse.Failure ();
			}

			using (businessContext.Bind (parentEntity))
			{
				var d = toDelete.First ();

				collection.Remove (d);
				businessContext.DeleteEntity (d);

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
			}

			return CoreResponse.Success ();
		}


		private Response CreateEntity(BusinessContext businessContext)
		{
			string parentEntityId = Request.Form.parentEntityId;
			var parentEntity = EntityIO.ResolveEntity (businessContext, parentEntityId);

			string propertyAccessorId = Request.Form.propertyAccessorId;
			var propertyAccessorCache = this.CoreServer.Caches.PropertyAccessorCache;
			var propertyAccessor = propertyAccessorCache.Get (propertyAccessorId) as EntityCollectionPropertyAccessor;

			if (propertyAccessor == null)
			{
				return CoreResponse.Failure ();
			}

			var type = propertyAccessor.CollectionType;
			var method = typeof (BusinessContext).GetMethod ("CreateEntity", new Type[0]);
			var genericMethod = method.MakeGenericMethod (type);
			var newEntity = (AbstractEntity) genericMethod.Invoke (businessContext, new object[0]);

			using (businessContext.Bind (parentEntity, newEntity))
			{
				var collection = (IList) propertyAccessor.GetValue (parentEntity);
				collection.Add (newEntity);

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
			}

			var key = EntityIO.GetEntityId (businessContext, newEntity);

			var content = new Dictionary<string, object> ()
			{
				{ "key", key }
			};

			return CoreResponse.Success (content);
		}


	}


}
