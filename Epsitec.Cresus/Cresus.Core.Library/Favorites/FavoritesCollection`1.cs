//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Favorites
{
	public sealed class FavoritesCollection<T> : FavoritesCollection
			where T : AbstractEntity, new ()
	{
		public FavoritesCollection(System.Collections.IEnumerable collection, Druid databaseId)
			: base (collection.Cast<AbstractEntity> (), databaseId)
		{
		}

		public override AbstractEntity GetExample()
		{
			return new T ();
		}

		public override Druid GetTypeId()
		{
			return EntityInfo<T>.GetTypeId ();
		}
	}
}
