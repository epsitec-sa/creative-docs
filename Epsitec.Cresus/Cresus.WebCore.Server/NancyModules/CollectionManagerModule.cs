using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor;

using Nancy;

using System;

using System.Linq;
using Epsitec.Cresus.WebCore.Server.UserInterface;


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
			var parentKey = EntityKey.Parse (parentEntityId);
			AbstractEntity parentEntity = context.DataContext.ResolveEntity (parentKey);

			string deleteEntity = Request.Form.deleteEntity;
			var deleteKey = EntityKey.Parse (deleteEntity);

			var panelFieldAccessor = coreSession.PanelFieldAccessorCache.Get ((string) Request.Form.lambdaId) as EntityListPanelFieldAccessor;

			if (panelFieldAccessor == null)
			{
				return Response.AsCoreError ();
			}

			var collection = panelFieldAccessor.GetCollection (parentEntity);

			var toDelete = collection.Cast<AbstractEntity> ().Where (c => context.DataContext.GetNormalizedEntityKey (c).Equals (deleteKey));

			if (toDelete.IsEmpty ())
			{
				return Response.AsCoreError ();
			}

			using (context.Bind (parentEntity))
			{
				var d = toDelete.First ();

				collection.Remove (d);
				context.DeleteEntity (d);

				context.ApplyRulesToRegisteredEntities (RuleType.Update);
				context.SaveChanges ();
			}

			return Response.AsCoreSuccess ();
		}


		private Response CreateEntity(CoreSession coreSession)
		{
			var context = coreSession.GetBusinessContext ();

			string parentEntityId = Request.Form.parentEntity;
			var parentKey = EntityKey.Parse (parentEntityId);
			AbstractEntity parentEntity = context.DataContext.ResolveEntity (parentKey);

			string typeName = Request.Form.entityType;
			var type = Type.GetType (typeName);
			var method = typeof (BusinessContext).GetMethod ("CreateEntity", new Type[0]);
			var m = method.MakeGenericMethod (type);
			var o = m.Invoke (context, new object[0]);
			var newEntity = o as AbstractEntity;
			
			var panelFieldAccessor = coreSession.PanelFieldAccessorCache.Get ((string) Request.Form.lambdaId) as EntityListPanelFieldAccessor;

			if (panelFieldAccessor == null)
			{
				return Response.AsCoreError ();
			}

			using (context.Bind (parentEntity, newEntity))
			{
				var collection = panelFieldAccessor.GetCollection (parentEntity);
				collection.Add (newEntity);
				
				context.ApplyRulesToRegisteredEntities (RuleType.Update);
				context.SaveChanges (EntitySaveMode.IncludeEmpty);
			}

			var key = context.DataContext.GetNormalizedEntityKey (newEntity).ToString ();
			return Response.AsCoreSuccess (key);
		}


	}


}
