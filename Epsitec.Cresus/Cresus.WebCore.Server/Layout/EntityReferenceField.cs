//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Favorites;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Layout
{
	/// <summary>
	/// This class represents an edition field used to edit entity references values.
	/// </summary>
	internal sealed class EntityReferenceField : AbstractField<EntityReferenceField>
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
				System.Type type;

				if (Collection.TryGetFirst (collection, out firstItem))
				{
					type = firstItem.GetType ();
				}
				else
				{
					type = collection.GetType ().GetGenericArguments ().First ();
				}

				var databaseId = Druid.Parse (this.DatabaseName);

				this.favoritesId   = FavoritesCache.Current.Push (collection, type, databaseId);
				this.favoritesOnly = favoritesOnly;
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


		private string							favoritesId;
		private bool							favoritesOnly;
	}
}
