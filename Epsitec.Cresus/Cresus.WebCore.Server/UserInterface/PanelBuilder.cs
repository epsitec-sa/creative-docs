using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.CoreServer;
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


		private PanelBuilder(AbstractEntity entity, ViewControllerMode mode, int? controllerSubTypeId, CoreSession coreSession)
		{
			this.rootEntity = entity;
			this.controllerMode = mode;
			this.controllerSubTypeId = controllerSubTypeId;
			this.coreSession = coreSession;
		}


		private DataContext DataContext
		{
			get
			{
				return this.BusinessContext.DataContext;
			}
		}


		private BusinessContext BusinessContext
		{
			get
			{
				return this.coreSession.GetBusinessContext ();
			}
		}


		/// <summary>
		/// Creates a controller according to an entity and a ViewMode
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="mode"></param>
		/// <returns>Name of the generated panel</returns>
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
			var panels = tiles.Select (t => t.ToDictionary ()).ToList ();

			panels[0]["isRoot"] = true;

			return panels;
		}


		private BrickWall GetBrickWall(AbstractEntity entity, ViewControllerMode controllerMode, int? controllerSubTypeId)
		{
			return Mason.BuildBrickWall (entity, controllerMode, controllerSubTypeId);
		}


		private IEnumerable<ITileData> GetTileData(BrickWall brickWall)
		{
			var propertyAccessorCache = this.coreSession.PropertyAccessorCache;
			var autoCreatorCache = this.coreSession.AutoCreatorCache;

			return Carpenter.BuildTileData(brickWall, propertyAccessorCache, autoCreatorCache);
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
			return Tools.GetEntityId (this.BusinessContext, entity);
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
			return this.BusinessContext.Data.GetAllEntities (entityType, DataExtractionMode.Sorted, this.DataContext);
		}


		public static Dictionary<string, object> BuildController(AbstractEntity entity, ViewControllerMode mode, int? controllerSubTypeId, CoreSession coreSession)
		{
			return new PanelBuilder (entity, mode, controllerSubTypeId, coreSession).Run ();
		}


		private readonly AbstractEntity rootEntity;


		private readonly ViewControllerMode controllerMode;


		private readonly int? controllerSubTypeId;


		private readonly CoreSession coreSession;


	}


}
