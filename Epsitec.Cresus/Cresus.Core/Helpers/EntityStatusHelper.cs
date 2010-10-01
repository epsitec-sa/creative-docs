//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Helpers
{
	public static class EntityStatusHelper
	{
		public static EntityStatus CombineStatus(params AbstractEntity[] entities)
		{
			if ((entities == null) ||
				(entities.Length == 0))
            {
				return EntityStatus.Empty;
            }

			var nonNullEntities  = entities.Where (x => x.IsNotNull ());
			var statusCollection = nonNullEntities.Select (x => x.EntityStatus);
			
			return EntityStatusHelper.CombineStatus (statusCollection.ToArray ());
		}

		public static EntityStatus CombineStatus(IEnumerable<AbstractEntity> entities)
		{
			return EntityStatusHelper.CombineStatus (entities.ToArray ());
		}

		public static EntityStatus CombineStatus(params EntityStatus[] collection)
		{
			if ((collection == null) ||
				(collection.Length == 0))
			{
				return EntityStatus.Empty;
			}

			if (collection.Any (x => x == EntityStatus.Invalid))
			{
				return EntityStatus.Invalid;
			}
			if (collection.Any (x => x == EntityStatus.Unknown))
			{
				return EntityStatus.Unknown;
			}
			if (collection.All (x => x == EntityStatus.Empty))
            {
				return EntityStatus.Empty;
            }

			return EntityStatus.Valid;
		}
	}
}
