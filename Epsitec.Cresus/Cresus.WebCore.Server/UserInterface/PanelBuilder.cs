using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;
using Epsitec.Cresus.WebCore.Server.UserInterface.TileData;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface
{


	/// <summary>
	/// Allow to create an ExtJS 4 panel by inferring the layout using
	/// AbstractEntities 
	/// </summary>
	internal sealed class PanelBuilder
	{


		private PanelBuilder(AbstractEntity rootEntity, ViewControllerMode controllerMode, int? controllerSubTypeId, BusinessContext businessContext, PropertyAccessorCache propertyAccessorCache, AutoCreatorCache autoCreatorCache)
		{
			this.rootEntity = rootEntity;
			this.controllerMode = controllerMode;
			this.controllerSubTypeId = controllerSubTypeId;
			this.businessContext = businessContext;
			this.propertyAccessorCache = propertyAccessorCache;
			this.autoCreatorCache = autoCreatorCache;
		}


		private DataContext DataContext
		{
			get
			{
				return this.businessContext.DataContext;
			}
		}


		private Dictionary<string, object> Run()
		{
			var panel = new Dictionary<string, object> ();

			panel["parentEntity"] = this.GetEntityId (this.rootEntity);
			panel["controllerMode"] = Tools.ViewControllerModeToString (this.controllerMode);
			panel["controllerSubTypeId"] = Tools.ControllerSubTypeIdToString (this.controllerSubTypeId);
			panel["items"] = this.GetPanels (this.rootEntity, this.controllerMode, this.controllerSubTypeId);

			return panel;
		}


		private List<Dictionary<string, object>> GetPanels(AbstractEntity entity, ViewControllerMode controllerMode, int? controllerSubTypeId)
		{
			var brickWall = this.GetBrickWall (entity, controllerMode, controllerSubTypeId);

			var tileData = this.GetTileData (brickWall);
			var tiles = this.GetTiles (tileData, entity);
			var items = tiles.Select (t => t.ToDictionary ());

			return items.ToList ();
		}


		private BrickWall GetBrickWall(AbstractEntity entity, ViewControllerMode controllerMode, int? controllerSubTypeId)
		{
			return Mason.BuildBrickWall (entity, controllerMode, controllerSubTypeId);
		}


		private IEnumerable<ITileData> GetTileData(BrickWall brickWall)
		{
			return Carpenter.BuildTileData(brickWall, this.propertyAccessorCache, this.autoCreatorCache);
		}


		private IEnumerable<AbstractTile> GetTiles(IEnumerable<ITileData> tileData, AbstractEntity entity)
		{
			return tileData.SelectMany
			(
				td => td.ToTiles
				(
					entity,
					this.GetEntityId,
					this.GetIconClass,
					this.GetTypeName,
					this.BuildEditionTiles,
					this.GetEntities
				)
			);
		}


		private string GetEntityId(AbstractEntity entity)
		{
			return Tools.GetEntityId (this.businessContext, entity);
		}


		private string GetIconClass(Type entityType, string uri)
		{
			return IconManager.GetCssClassName (entityType, uri, IconSize.Sixteen);
		}


		private string GetTypeName(Type type)
		{
			return type.AssemblyQualifiedName;
		}


		private IEnumerable<AbstractTile> BuildEditionTiles(AbstractEntity entity)
		{
			var brickWall = this.GetBrickWall (entity, ViewControllerMode.Edition, null);
			var tileData = this.GetTileData (brickWall);

			return this.GetTiles (tileData, entity);
		}


		private IEnumerable<AbstractEntity> GetEntities(Type entityType)
		{
			return this.businessContext.Data.GetAllEntities (entityType, DataExtractionMode.Sorted, this.DataContext);
		}


		public static Dictionary<string, object> BuildController(AbstractEntity rootEntity, ViewControllerMode controllerMode, int? controllerSubTypeId, BusinessContext businessContext, PropertyAccessorCache propertyAccessorCache, AutoCreatorCache autoCreatorCache)
		{
			return new PanelBuilder (rootEntity, controllerMode, controllerSubTypeId, businessContext, propertyAccessorCache, autoCreatorCache).Run ();
		}


		private readonly AbstractEntity rootEntity;


		private readonly ViewControllerMode controllerMode;


		private readonly int? controllerSubTypeId;


		private readonly BusinessContext businessContext;


		private readonly PropertyAccessorCache propertyAccessorCache;


		private readonly AutoCreatorCache autoCreatorCache;


	}


}
