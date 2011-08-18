using System.Collections.Generic;
using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Nancy;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Common.Support.Extensions;
using System.Linq;
using Epsitec.Cresus.Core.Business;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class CreatorModule : CoreModule
	{
		public CreatorModule()
		{
			Post["/delete"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				string parentEntity = Request.Form.parentEntity;
				var parentKey = EntityKey.Parse (parentEntity);
				AbstractEntity entity = context.DataContext.ResolveEntity (parentKey);

				string deleteEntity = Request.Form.deleteEntity;
				var deleteKey = EntityKey.Parse (deleteEntity);

				var accessor = coreSession.GetPanelFieldAccessor (InvariantConverter.ToInt ((string) Request.Form.lambda));

				if (!accessor.IsCollectionType)
				{
					return Response.AsCoreError ();
				}

				var collection = accessor.GetCollection (entity);

				var toDelete = collection.Cast<AbstractEntity> ().Where (c => context.DataContext.GetNormalizedEntityKey (c).Equals (deleteKey));

				if (toDelete.IsEmpty ())
				{
					return Response.AsCoreError ();
				}

				var d = toDelete.First ();
				collection.Remove (d);
				context.DeleteEntity (d);
				context.SaveChanges ();

				return Response.AsCoreSuccess ();
			};


			Post["/create"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				string parentEntity = Request.Form.parentEntity;
				var parentKey = EntityKey.Parse (parentEntity);
				AbstractEntity entity = context.DataContext.ResolveEntity (parentKey);

				string typeName = Request.Form.entityType;
				var type = System.Type.GetType (typeName);
				var method = typeof (BusinessContext).GetMethod ("CreateEntity", new System.Type[0]);
				var m = method.MakeGenericMethod (type);
				var o = m.Invoke (context, new object[0]);
				var newEntity = o as AbstractEntity;

				var accessor = coreSession.GetPanelFieldAccessor (InvariantConverter.ToInt ((string) Request.Form.lambda));

				if (!accessor.IsCollectionType)
				{
					return Response.AsCoreError ();
				}

				var collection = accessor.GetCollection (entity);
				collection.Add (newEntity);

				context.SaveChanges (EntitySaveMode.IncludeEmpty);

				var key = context.DataContext.GetNormalizedEntityKey (newEntity).ToString ();
				return Response.AsCoreSuccess (key);
			};
		}
	}
}
