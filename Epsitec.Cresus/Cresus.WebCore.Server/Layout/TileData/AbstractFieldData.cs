using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
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


		public bool AllowBlank
		{
			get;
			set;
		}


		public override sealed AbstractEditionTilePart ToAbstractEditionTilePart(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return this.ToAbstractField (layoutBuilder, entity);
		}


		public abstract AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity);


	}


}

