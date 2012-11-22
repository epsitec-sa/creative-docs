using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class DateFieldData : AbstractFieldData
	{


		public Func<AbstractEntity, string> ValueGetter
		{
			get;
			set;
		}


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new DateField ()
			{
				Id = this.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = this.ValueGetter (entity),
				AllowBlank = this.AllowBlank,
			};
		}


	}


}
