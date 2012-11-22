using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.Core;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout
{


	/// <summary>
	/// Allow to create an ExtJS 4 panel by inferring the layout using AbstractEntities and their
	/// ViewControllers.
	/// </summary>
	internal sealed class LayoutBuilder
	{


		public LayoutBuilder(BusinessContext businessContext, Caches caches)
		{
			this.businessContext = businessContext;
			this.caches = caches;
		}


		public Dictionary<string, object> Build(AbstractEntity entity, ViewControllerMode viewMode, int? viewId)
		{
			var entityColumnData = new EntityColumn ()
			{
				EntityId = Tools.GetEntityId (this.businessContext, entity),
				ViewMode = Tools.ViewModeToString (viewMode),
				ViewId = Tools.ViewIdToString (viewId),
				Tiles = LayoutBuilder.GetTiles (this.businessContext, this.caches, entity, viewMode, viewId).ToList (),
			};

			return entityColumnData.ToDictionary ();
		}

		public static IEnumerable<AbstractTile> GetTiles(BusinessContext businessContext, Caches caches, AbstractEntity entity, ViewControllerMode viewMode, int? viewId)
		{
			var brickWall = Mason.BuildBrickWall (entity, viewMode, viewId);

			return Carpenter.BuildTiles (businessContext, caches, brickWall, viewMode, entity);
		}
		
		
		private readonly BusinessContext businessContext;


		private readonly Caches caches;


	}


}
