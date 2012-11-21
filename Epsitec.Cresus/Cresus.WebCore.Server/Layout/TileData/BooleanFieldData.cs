using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class BooleanFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			var booleanPropertyAccessor = (BooleanPropertyAccessor) this.PropertyAccessor;

			return new BooleanField ()
			{
				Id = this.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = (bool?) booleanPropertyAccessor.GetValue (entity),
				AllowBlank = this.AllowBlank,
			};
		}


	}


}