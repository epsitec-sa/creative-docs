using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class EnumerationFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			var enumerationPropertyAccessor = (EnumerationPropertyAccessor) this.PropertyAccessor;

			return new EnumerationField ()
			{
				Id = this.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = (string) enumerationPropertyAccessor.GetValue (entity),
				TypeName = layoutBuilder.GetTypeName (enumerationPropertyAccessor.Type),
				AllowBlank = this.AllowBlank,
			};
		}


	}


}
