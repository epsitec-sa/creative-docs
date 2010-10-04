//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Helpers
{
	public static class EntityStatusHelper
	{
		public static EntityStatus TreatAsOptional(this EntityStatus status)
		{
			if ((status & EntityStatus.Empty) != 0)
			{
				status &= EntityStatus.Valid;
			}

			return status;
		}


		public static EntityStatus CombineStatus(StatusHelperCardinality cardinality, IEnumerable<AbstractEntity> entities)
		{
			return EntityStatusHelper.CombineStatus (cardinality, entities.ToArray ());
		}

		public static EntityStatus CombineStatus(StatusHelperCardinality cardinality, params AbstractEntity[] entities)
		{
			if (entities == null || entities.Length == 0)
            {
				return EntityStatus.Empty;
            }

			var nonNullEntities  = entities.Where (x => x.IsNotNull ());
			var statusCollection = nonNullEntities.Select (x => x.GetEntityStatus ());

			return EntityStatusHelper.CombineStatus (cardinality, statusCollection.ToArray ());
		}

		public static EntityStatus CombineStatus(StatusHelperCardinality cardinality, params EntityStatus[] collection)
		{
			if (collection == null || collection.Length == 0)
			{
				return EntityStatus.Empty;
			}

			if (collection.Any (x => (x & EntityStatus.Valid) == 0))
			{
				return EntityStatus.None;  // invalide
			}

			if (collection.All (x => (x & EntityStatus.Empty) != 0))
			{
				return EntityStatus.Empty;
			}

			if (cardinality == StatusHelperCardinality.AtLeastOne)
			{
				if (collection.Any (x => (x & EntityStatus.Valid) != 0))
				{
					return EntityStatus.Valid;
				}
			}
			else
			{
				if (collection.All (x => (x & EntityStatus.Valid) != 0))
				{
					return EntityStatus.Valid;
				}
			}

			return EntityStatus.None;
		}
	}
}