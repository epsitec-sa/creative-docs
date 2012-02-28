using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.TileData
{


	internal sealed class HorizontalGroupData : AbstractEditionTilePartData
	{


		public FormattedText Title
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


		public override AbstractEditionTilePart ToAbstractEditionTilePart(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, AbstractPropertyAccessor> propertyAccessorGetter)
		{
			var group = new HorizontalGroup ()
			{
				Title = this.Title.ToString (),
			};

			group.Fields.AddRange (this.Fields.Select (f => f.ToAbstractField (entity, entityIdGetter, entitiesGetter, propertyAccessorGetter)));

			return group;
		}


		private readonly IList<AbstractFieldData> fields = new List<AbstractFieldData> ();
	}


}
