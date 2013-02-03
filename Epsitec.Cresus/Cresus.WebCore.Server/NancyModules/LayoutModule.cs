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


	public class LayoutModule : AbstractAuthenticatedModule
	{


		/// <summary>
		/// Call the <see cref="PanelBuilder"/> to create the ExtJS interface.
		/// It is called to show the summary of the edition interface.
		/// </summary>
		public LayoutModule(CoreServer coreServer)
			: base (coreServer, "/layout")
		{
			Get["/entity/{viewMode}/{viewId}/{entityId}"] = p => this.Execute (b => this.GetLayoutWithEntity (b, p));
			Get["/entity/{viewMode}/{viewId}/{entityId}/{additionalEntityId}"] = p => this.Execute (b => this.GetLayoutWithEntity (b, p));
			Get["/type/{viewMode}/{viewId}/{typeId}"] = p => this.Execute (b => this.GetLayoutWithType (b, p));
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
