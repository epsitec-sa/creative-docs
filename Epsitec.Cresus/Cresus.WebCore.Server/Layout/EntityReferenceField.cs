//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Favorites;

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents an edition field used to edit entity references values.
	/// </summary>
	internal sealed class EntityReferenceField : AbstractField
	{
		public string							DatabaseName
		{
			get;
			set;
		}


		public void DefineFavorites(IEnumerable<AbstractEntity> collection, bool favoritesOnly)
		{
			if (collection != null)
			{
				object firstItem;

				if (Collection.TryGetFirst (collection, out firstItem))
				{
					var type = firstItem.GetType ();
					var databaseId = Druid.Parse (this.DatabaseName);

					this.favoritesId   = FavoritesCache.Current.Push (collection, type, databaseId);
					this.favoritesOnly = favoritesOnly;
				}
			}
		}


		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["databaseName"] = this.DatabaseName;

			if (this.favoritesId != null)
			{
				brick["favoritesId"]   = this.favoritesId;
				brick["favoritesOnly"] = this.favoritesOnly;
			}

			return brick;
		}

		protected override string GetEditionTilePartType()
		{
			return "entityReferenceField";
		}


		private string							favoritesId;
		private bool							favoritesOnly;
	}
}
