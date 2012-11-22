using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class TextFieldData : AbstractFieldData
	{


		public Func<AbstractEntity, string> ValueGetter
		{
			get;
			set;
		}


		public bool IsMultiline
		{
			get;
			set;
		}


		public bool IsPassword
		{
			get;
			set;
		}
		
		
		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return this.IsMultiline
				? this.ToTextAreaField (entity)
				: this.ToTextField (entity);
		}


		private AbstractField ToTextField(AbstractEntity entity)
		{
			return new TextField ()
			{
				Id = this.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = this.ValueGetter (entity),
				IsPassword = this.IsPassword,
				AllowBlank = this.AllowBlank,
			};
		}


		private AbstractField ToTextAreaField(AbstractEntity entity)
		{
			return new TextAreaField ()
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
