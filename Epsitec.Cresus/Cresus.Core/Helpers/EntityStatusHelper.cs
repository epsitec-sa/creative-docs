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
				status |= EntityStatus.Valid;
			}

			return status;
		}


#if false
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

		public struct EntityStatusCollection
		{
			public EntityStatusCollection(EntityStatus status)
			{
				this.collection = new EntityStatus[] { status };
			}

			public EntityStatusCollection(EntityStatus[] collection)
			{
				this.collection = collection;
			}

			public static implicit operator EntityStatusCollection(EntityStatus status)
			{
				return new EntityStatusCollection (status);
			}
			public static implicit operator EntityStatusCollection(EntityStatus[] collection)
			{
				return new EntityStatusCollection (collection);
			}

			public EntityStatus[] Items
			{
				get
				{
					return this.collection;
				}
			}

			private readonly EntityStatus[] collection;
		}

		public static EntityStatus CombineStatus(StatusHelperCardinality cardinality, params EntityStatusCollection[] collectionMix)
		{
			if (collectionMix == null)
			{
				return EntityStatus.Empty;
			}

			EntityStatus[] collection = collectionMix.SelectMany (x => x.Items).ToArray ();

			//	S'il existe un seul invalide, tout est considéré comme invalide.
			if (collection.Any (x => (x & EntityStatus.Empty) == 0 && (x & EntityStatus.Valid) == 0))
			{
				return EntityStatus.None;  // invalide
			}

			//	Si tout est vide, on dit que c'est vide.
			if (collection.All (x => (x & EntityStatus.Empty) != 0))
			{
				return EntityStatus.Empty;
			}

			if (cardinality == StatusHelperCardinality.AtLeastOne)
			{
				//	Si un seul est valide, on dit que c'est valide.
				if (collection.Any (x => (x & EntityStatus.Valid) != 0))
				{
					return EntityStatus.Valid;
				}
			}
			else
			{
				//	Si tout est valide, on dit que c'est valide.
				if (collection.All (x => (x & EntityStatus.Valid) != 0))
				{
					return EntityStatus.Valid;
				}
			}

			return EntityStatus.None;  // invalide
		}
#endif
	}
}