//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Layout;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

namespace Epsitec.Cresus.WebCore.Server.NancyModules
{
	/// <summary>
	/// Gets the layout representations of the various EntityViewControllers.
	/// </summary>
	public class LayoutModule : AbstractAuthenticatedModule
	{
		public LayoutModule(CoreServer coreServer)
			: base (coreServer, "/layout")
		{
			// Gets the layout representation of an EntityViewController based on a single entity.
			// URL arguments:
			// - viewMode:   The view mode of the EntityViewController to use, as used by the
			//               DataIO class.
			// - viewId:     The view id of the EntityViewController to use, as used by the DataIO
			//               class.
			// - entityId:   The entity key of the entity on which the EntityViewController will be
			//               used, in the format used by the EntityIO class.
			Get["/entity/{viewMode}/{viewId}/{entityId}"] = p =>
				this.Execute (b => this.GetLayoutWithEntity (b, p));

			// Gets the layout representation of an EntityViewController, based on an entity and
			// an additional entity.
			// URL arguments:
			// - viewMode:             The view mode of the EntityViewController to use, as used by
			//                         the DataIO class.
			// - viewId:               The view id of the EntityViewController to use, as used by
			//                         the DataIO class.
			// - entityId:             The entity key of the entity on which the EntityViewController
			//                         will be used, in the format used by the EntityIO class.
			// - additionalEntityId:   The entity key of the additional entity on which the
			//                         EntityViewController will be used, in the format used by the
			//                         EntityIO class.
			Get["/entity/{viewMode}/{viewId}/{entityId}/{additionalEntityId}"] = p =>
				this.Execute (b => this.GetLayoutWithEntity (b, p));

			// Gets the layout reprensentation of an EntityViewControoler, based on an entity type.
			// URL arguments:
			// - viewMode:   The view mode of the EntityViewController to use, as used by the
			//               DataIO class.
			// - viewId:     The view id of the EntityViewController to use, as used by the DataIO
			//               class.
			// - typeId:     The id of the entity type with which to used the EntityViewController,
			//               in the format used by the TypeCache class.
			Get["/type/{viewMode}/{viewId}/{typeId}"] = p =>
				this.Execute (b => this.GetLayoutWithType (b, p));
		}


		private Response GetLayoutWithEntity(BusinessContext businessContext, dynamic parameters)
		{
			var entity = EntityIO.ResolveEntity (businessContext, (string) parameters.entityId);
			var additionalEntity = EntityIO.ResolveEntity (businessContext, (string) parameters.additionalEntityId);

			return this.GetLayout (businessContext, entity, additionalEntity, parameters);
		}

		private Response GetLayoutWithType(BusinessContext businessContext, dynamic parameters)
		{
			var type = this.CoreServer.Caches.TypeCache.GetItem ((string) parameters.typeId);
			var dummyEntity = (AbstractEntity) Activator.CreateInstance (type);

			return this.GetLayout (businessContext, dummyEntity, null, parameters);
		}

		private Response GetLayout(BusinessContext businessContext, AbstractEntity entity, AbstractEntity additionalEntity, dynamic parameters)
		{
			var viewMode = DataIO.ParseViewMode ((string) parameters.viewMode);
			var viewId = DataIO.ParseViewId ((string) parameters.viewId);

			var caches = this.CoreServer.Caches;
			var databaseManager = this.CoreServer.DatabaseManager;

			var entityColumn = Carpenter.BuildEntityColumn (businessContext, caches, databaseManager, entity, additionalEntity, viewMode, viewId);
			var content = entityColumn.ToDictionary (caches);

			return CoreResponse.Success (content);
		}
	}
}
