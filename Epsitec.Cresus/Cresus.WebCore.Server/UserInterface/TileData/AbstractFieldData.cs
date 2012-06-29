using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal abstract class AbstractFieldData : AbstractEditionTilePartData
	{


		// TODO Add PickFromCollection, ReadOnly, Width, Height


		public FormattedText Title
		{
			get;
			set;
		}


		public AbstractPropertyAccessor PropertyAccessor
		{
			get;
			set;
		}


		public bool IsReadOnly
		{
			get;
			set;
		}


		public override sealed AbstractEditionTilePart ToAbstractEditionTilePart(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			return this.ToAbstractField (panelBuilder, entity);
		}


		public abstract AbstractField ToAbstractField(PanelBuilder panelBuilder, AbstractEntity entity);


	}


}

