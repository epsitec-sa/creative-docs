using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class DecimalFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			var decimalPropertyAccessor = (DecimalPropertyAccessor) this.PropertyAccessor;

			return new DecimalField ()
			{
				PropertyAccessorId = decimalPropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = (decimal?) decimalPropertyAccessor.GetValue (entity),
				AllowBlank = this.AllowBlank,
			};
		}


	}


}
