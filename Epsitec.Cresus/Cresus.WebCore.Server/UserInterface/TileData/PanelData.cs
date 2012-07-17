using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class PanelData
	{


		public ViewControllerMode ViewMode
		{
			get;
			set;
		}


		public int? ViewId
		{
			get;
			set;
		}


		public IList<ITileData> Tiles
		{
			get;
			set;
		}


		public Panel ToPanel(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			return new Panel ()
			{
				EntityId = panelBuilder.GetEntityId (entity),
				ViewMode = Tools.ViewModeToString (this.ViewMode),
				ViewId = Tools.ViewIdToString (this.ViewId),
				Tiles = panelBuilder.GetTiles (this.Tiles, entity).ToList (),
			};
		}



	}


}
