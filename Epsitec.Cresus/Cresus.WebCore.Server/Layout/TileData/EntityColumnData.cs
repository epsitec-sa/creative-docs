using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{
	using EntityColumn = Epsitec.Cresus.WebCore.Server.Layout.Tile.EntityColumn;

	internal sealed class EntityColumnData
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


		public EntityColumn ToEntityColumn(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new EntityColumn ()
			{
				EntityId = layoutBuilder.GetEntityId (entity),
				ViewMode = Tools.ViewModeToString (this.ViewMode),
				ViewId = Tools.ViewIdToString (this.ViewId),
				Tiles = layoutBuilder.GetTiles (this.Tiles, entity).ToList (),
			};
		}
		

	}


}
