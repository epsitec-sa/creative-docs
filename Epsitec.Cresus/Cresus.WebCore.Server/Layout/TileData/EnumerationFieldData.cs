using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
{


	internal sealed class EnumerationFieldData : AbstractFieldData
	{


		public Func<AbstractEntity, string> ValueGetter
		{
			get;
			set;
		}


		public Type EnumerationType
		{
			get;
			set;
		}



		public override AbstractField ToAbstractField(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new EnumerationField ()
			{
				Id = this.Id,
				Title = this.Title.ToString (),
				IsReadOnly = this.IsReadOnly,
				Value = this.ValueGetter (entity),
				TypeName = layoutBuilder.GetTypeName (this.EnumerationType),
				AllowBlank = this.AllowBlank,
			};
		}


	}


}
