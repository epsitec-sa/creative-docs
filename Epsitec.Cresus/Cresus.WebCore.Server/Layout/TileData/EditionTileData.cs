using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.Layout.Tile;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Layout.TileData
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


		public IList<AbstractEditionTilePartData> Bricks
		{
			get
			{
				return this.bricks;
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


		public IEnumerable<AbstractTile> ToTiles(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			if (this.Bricks.Count > 0)
			{
				yield return this.ToTile (layoutBuilder, entity);
			}

			foreach (var include in this.Includes)
			{
				var includedEntity = include.EntityGetter (entity);

				foreach (var tile in layoutBuilder.BuildEditionTiles (includedEntity))
				{
					yield return tile;
				}
			}
		}


		#endregion


		private AbstractTile ToTile(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return new EditionTile ()
			{
				EntityId = layoutBuilder.GetEntityId (entity),
				IconClass = layoutBuilder.GetIconClass (this.EntityType, this.Icon),
				Title = this.TitleGetter (entity).ToString (),
				Bricks = this.GetEditionTileParts (layoutBuilder, entity).ToList ()
			};
		}


		private IEnumerable<AbstractEditionTilePart> GetEditionTileParts(LayoutBuilder layoutBuilder, AbstractEntity entity)
		{
			return this.Bricks.Select (i => i.ToAbstractEditionTilePart (layoutBuilder, entity));
		}


		private readonly IList<AbstractEditionTilePartData> bricks = new List<AbstractEditionTilePartData> ();


		private readonly IList<IncludeData> includes = new List<IncludeData> ();


	}


}

