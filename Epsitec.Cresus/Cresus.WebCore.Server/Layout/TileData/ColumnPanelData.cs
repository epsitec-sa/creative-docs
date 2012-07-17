using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class ColumnPanelData
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


		public ColumnPanel ToColumnPanel(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new ColumnPanel ()
			{
				EntityId = layoutBuilder.GetEntityId (entity),
				ViewMode = Tools.ViewModeToString (this.ViewMode),
				ViewId = Tools.ViewIdToString (this.ViewId),
				Tiles = layoutBuilder.GetTiles (this.Tiles, entity).ToList (),
			};
		}



	}


}
