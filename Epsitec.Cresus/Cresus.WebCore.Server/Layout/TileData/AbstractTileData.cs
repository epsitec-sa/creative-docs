using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal abstract class AbstractTileData
	{


		public string Icon
		{
			get;
			set;
		}


		public Type EntityType
		{
			get;
			set;
		}


		public Func<AbstractEntity, FormattedText> TitleGetter
		{
			get;
			set;
		}


		public abstract IEnumerable<AbstractTile> ToTiles(LayoutBuilder layoutBuilder, AbstractEntity entity);


	}


}

