using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.UserInterface.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


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


		public IEnumerable<AbstractTile> ToTiles(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, string, string> iconClassGetter, Func<Type, string> typeGetter, Func<AbstractEntity, IEnumerable<AbstractTile>> editionTileBuilder, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			if (this.Items.Count > 0)
			{
				yield return this.ToTile (entity, entityIdGetter, iconClassGetter, typeGetter, entitiesGetter);
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


		private AbstractTile ToTile(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, string, string> iconClassGetter, Func<Type, string> typeGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			var editionTile = new EditionTile ()
			{
				EntityId = entityIdGetter (entity),
				IconClass = iconClassGetter (this.EntityType, this.Icon),
				Title = this.TitleGetter (entity).ToString (),		
			};

			editionTile.Items.AddRange (this.GetEditionTileParts (entity, entityIdGetter, entitiesGetter));

			return editionTile;
		}


		private IEnumerable<AbstractEditionTilePart> GetEditionTileParts(AbstractEntity entity, Func<AbstractEntity, string> entityIdGetter, Func<Type, IEnumerable<AbstractEntity>> entitiesGetter)
		{
			return this.Items.Select (i => i.ToAbstractEditionTilePart (entity, entityIdGetter, entitiesGetter));
		}


		private readonly IList<AbstractEditionTilePartData> items = new List<AbstractEditionTilePartData> ();


		private readonly IList<IncludeData> includes = new List<IncludeData> ();


	}


}

