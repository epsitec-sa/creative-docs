using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class HorizontalGroupData : AbstractEditionTilePartData
	{


		public Func<AbstractEntity, FormattedText> TitleGetter
		{
			get;
			set;
		}


		public IList<AbstractFieldData> Fields
		{
			get
			{
				return this.fields;
			}
		}


		public override AbstractEditionTilePart ToAbstractEditionTilePart(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new HorizontalGroup ()
			{
				Title = this.TitleGetter (entity).ToString (),
				Fields = this.Fields
					.Select (f => f.ToAbstractField (layoutBuilder, entity))
					.ToList (),
			};
		}


		private readonly IList<AbstractFieldData> fields = new List<AbstractFieldData> ();
	}


}
