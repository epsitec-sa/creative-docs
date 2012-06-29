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


		public IEnumerable<AbstractTile> ToTiles(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			if (this.Items.Count > 0)
			{
				yield return this.ToTile (panelBuilder, entity);
			}

			foreach (var include in this.Includes)
			{
				var includedEntity = include.EntityGetter (entity);

				foreach (var tile in panelBuilder.BuildEditionTiles (includedEntity))
				{
					yield return tile;
				}
			}
		}


		#endregion


		private AbstractTile ToTile(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			var editionTile = new EditionTile ()
			{
				EntityId = panelBuilder.GetEntityId (entity),
				IconClass = panelBuilder.GetIconClass (this.EntityType, this.Icon),
				Title = this.TitleGetter (entity).ToString (),		
			};

			editionTile.Items.AddRange (this.GetEditionTileParts (panelBuilder, entity));

			return editionTile;
		}


		private IEnumerable<AbstractEditionTilePart> GetEditionTileParts(PanelBuilder panelBuilder, AbstractEntity entity)
		{
			return this.Items.Select (i => i.ToAbstractEditionTilePart (panelBuilder, entity));
		}


		private readonly IList<AbstractEditionTilePartData> items = new List<AbstractEditionTilePartData> ();


		private readonly IList<IncludeData> includes = new List<IncludeData> ();


	}


}

