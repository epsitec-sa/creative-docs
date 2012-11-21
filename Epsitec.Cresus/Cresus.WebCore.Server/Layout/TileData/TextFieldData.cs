using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class TextFieldData : AbstractFieldData
	{


		public bool IsPassword
		{
			get;
			set;
		}
		
		
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
				PropertyAccessorId = textPropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = (string) textPropertyAccessor.GetValue (entity),
				IsPassword = this.IsPassword,
				AllowBlank = this.AllowBlank,
			};
		}


		private AbstractField ToTextAreaField(AbstractEntity entity, TextPropertyAccessor textPropertyAccessor)
		{
			return new TextAreaField ()
			{
				PropertyAccessorId = textPropertyAccessor.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = (string) textPropertyAccessor.GetValue (entity),
				AllowBlank = this.AllowBlank,
			};
		}


	}


}
