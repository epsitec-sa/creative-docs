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


	internal sealed class EditionTileData : ITileData
	{


		// TODO Add CompactTitle, Text, CompactText ?


		public string Icon
		{
			get;
			set;
		}


		public Type EntityType
		{
			get;
			set;
		}
		
		
		public Func<AbstractEntity, FormattedText> TitleGetter
		{
			get;
			set;
		}


		public IList<AbstractEditionTilePartData> Items
		{
			get
			{
				return this.items;
			}
		}


		public IList<IncludeData> Includes
		{
			get
			{
				return this.includes;
			}
		}


		#region ITileData Members


		public IEnumerable<AbstractTile> ToTiles(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, string, string> iconClassGetter, Func<LambdaExpression, string> lambdaIdGetter, Func<Type, string> typeGetter, Func<AbstractEntity, IEnumerable<AbstractTile>> editionTileBuilder, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, AbstractPropertyAccessor> propertyAccessorGetter)
		{
			if (this.Items.Count > 0)
			{
				yield return this.ToTile (entity, entityIdGetter, iconClassGetter, lambdaIdGetter, typeGetter, entitiesGetter, propertyAccessorGetter);
			}

			foreach (var include in this.Includes)
			{
				var includedEntity = include.EntityGetter (entity);

				foreach (var tile in editionTileBuilder (includedEntity))
				{
					yield return tile;
				}
			}
		}


		#endregion


		private AbstractTile ToTile(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, string, string> iconClassGetter, Func<LambdaExpression, string> lambdaIdGetter, Func<Type, string> typeGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, AbstractPropertyAccessor> propertyAccessorGetter)
		{
			var editionTile = new EditionTile ()
			{
				EntityId = entityIdGetter (entity),
				IconClass = iconClassGetter (this.EntityType, this.Icon),
				Title = this.TitleGetter (entity).ToString (),		
			};

			editionTile.Items.AddRange (this.GetEditionTileParts (entity, entityIdGetter, entitiesGetter, propertyAccessorGetter));

			return editionTile;
		}


		private IEnumerable<AbstractEditionTilePart> GetEditionTileParts(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter, Func<LambdaExpression, AbstractPropertyAccessor> propertyAccessorGetter)
		{
			return this.Items.Select (i => i.ToAbstractEditionTilePart (entity, entityIdGetter, entitiesGetter, propertyAccessorGetter));
		}


		private readonly IList<AbstractEditionTilePartData> items = new List<AbstractEditionTilePartData> ();


		private readonly IList<IncludeData> includes = new List<IncludeData> ();


	}


}

