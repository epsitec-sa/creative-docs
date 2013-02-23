//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Favorites;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	internal sealed class EntityReferenceField : AbstractField
	{
		public EntityReferenceField()
		{
		}

		
		public string							DatabaseName
		{
			get;
			set;
		}

		
		public void DefineFavorites(System.Collections.IEnumerable collection)
		{
			if (collection != null)
			{
				object firstItem;
				
				if (Collection.TryGetFirst (collection, out firstItem))
				{
					this.favoritesId = FavoritesCache.Current.Push (collection, firstItem.GetType (), Druid.Parse (this.DatabaseName));
				}
			}
		}

		public override Dictionary<string, object> ToDictionary()
		{
			var brick = base.ToDictionary ();

			brick["databaseName"] = this.DatabaseName;

			if (this.favoritesId != null)
			{
				brick["favoritesId"] = this.favoritesId;
			}

			return brick;
		}

		protected override string GetEditionTilePartType()
		{
			return "entityReferenceField";
		}

		private string							favoritesId;
	}
}
