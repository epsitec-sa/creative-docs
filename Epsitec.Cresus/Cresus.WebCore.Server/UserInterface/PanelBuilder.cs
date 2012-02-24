using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;
using Epsitec.Cresus.WebCore.Server.UserInterface.TileData;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


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
			return Carpenter.BuildTileData (brickWall);
		}


		private IEnumerable<AbstractTile> GetTiles(IEnumerable<ITileData> tileData, AbstractEntity entity)
		{
			return tileData.SelectMany
			(
				td => td.ToTiles
				(
					entity,
					e => this.GetEntityId (e),
					(t, u) => this.GetIconClass (t, u),
					l => this.GetLambdaId (l),
					t => this.GetTypeName (t),
					e => this.BuildEditionTiles (e),
					t => this.GetEntities (t),
					l => this.GetPanelFieldAccessor (l)
				)
			);
		}


		private string GetEntityId(AbstractEntity entity)
		{
			string entityId = null;

			if (entity != null)
			{
				var entityKey = this.DataContext.GetNormalizedEntityKey (entity);

				if (entityKey.HasValue)
				{
					entityId = entityKey.Value.ToString ();
				}
			}

			return entityId;
		}


		private string GetIconClass(Type entityType, string uri)
		{
			return IconManager.GetCssClassName (entityType, uri, IconSize.Sixteen);
		}


		private string GetLambdaId(LambdaExpression lambda)
		{
			return this.coreSession.PanelFieldAccessorCache.Get (lambda).Id;
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


		private AbstractPanelFieldAccessor GetPanelFieldAccessor(LambdaExpression lambda)
		{
			return this.coreSession.PanelFieldAccessorCache.Get (lambda);
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
