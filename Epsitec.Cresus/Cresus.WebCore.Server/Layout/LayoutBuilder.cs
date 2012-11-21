﻿using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.Core;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;
using Epsitec.Cresus.WebCore.Server.Layout.TileData;

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
			var entityColumnData = new EntityColumnData ()
			{
				ViewMode = viewMode,
				ViewId = viewId,
				Tiles = this.GetTileData (entity, viewMode, viewId).ToList (),
			};

			return entityColumnData
				.ToEntityColumn (this, entity)
				.ToDictionary ();
		}

		private IEnumerable<AbstractTileData> GetTileData(AbstractEntity entity, ViewControllerMode viewMode, int? viewId)
		{
			var brickWall = Mason.BuildBrickWall (entity, viewMode, viewId);

			return Carpenter.BuildTileData (viewMode, brickWall, this.caches);
		}


		public IEnumerable<AbstractTile> GetTiles(IEnumerable<AbstractTileData> tileData, AbstractEntity entity)
		{
			return tileData.SelectMany (td => td.ToTiles (this, entity));
		}


		public string GetEntityId(AbstractEntity entity)
		{
			return Tools.GetEntityId (this.businessContext, entity);
		}


		public string GetIconClass(Type entityType, string uri)
		{
			return IconManager.GetCssClassName (uri, IconSize.Sixteen, entityType);
		}


		public string GetTypeName(Type type)
		{
			return Tools.TypeToString (type);
		}


		public IEnumerable<AbstractTile> BuildEditionTiles(AbstractEntity entity)
		{
			var tileData = this.GetTileData (entity, ViewControllerMode.Edition, null);

			return this.GetTiles (tileData, entity);
		}


		public IEnumerable<AbstractEntity> GetEntities(Type entityType)
		{
			var mode = DataExtractionMode.Sorted;
			var dataContext = this.businessContext.DataContext;

			return this.businessContext.Data.GetAllEntities (entityType, mode, dataContext);
		}
		
		
		private readonly BusinessContext businessContext;


		private readonly Caches caches;


	}


}
