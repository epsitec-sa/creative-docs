using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class TextFieldData : AbstractFieldData
	{


		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			var textPropertyAccessor = (TextPropertyAccessor) this.PropertyAccessor;

			if (StringType.IsMultilineText (textPropertyAccessor.Property.Type))
			{
				return this.ToTextAreaField (entity, textPropertyAccessor);
			}
			else
			{
				return this.ToTextField (entity, textPropertyAccessor);
			}
		}


		private AbstractField ToTextField(AbstractEntity entity, TextPropertyAccessor textPropertyAccessor)
		{
			return new TextField ()
			{
				PropertyAccessorId = InvariantConverter.ToString (textPropertyAccessor.Id),
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = (string) textPropertyAccessor.GetValue (entity),
			};
		}


		private AbstractField ToTextAreaField(AbstractEntity entity, TextPropertyAccessor textPropertyAccessor)
		{
			return new TextAreaField ()
			{
				PropertyAccessorId = InvariantConverter.ToString (textPropertyAccessor.Id),
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = (string) textPropertyAccessor.GetValue (entity),
			};
		}


	}


}
