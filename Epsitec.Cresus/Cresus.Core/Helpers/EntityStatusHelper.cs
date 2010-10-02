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
		public static EntityStatus Optional(EntityStatus status)
		{
			if (status == EntityStatus.Empty)
			{
				//	Un champ optionnel vide est à la fois vide et valide !
				return EntityStatus.EmptyAndValid;
			}

			return status;
		}


		public static EntityStatus GetStatus(string s)
		{
			if (string.IsNullOrWhiteSpace (s))
			{
				return EntityStatus.Empty;
			}
			else
			{
				return EntityStatus.Valid;
			}
		}

		public static EntityStatus GetStatus(FormattedText s)
		{
			if (s.IsNullOrWhiteSpace)
			{
				return EntityStatus.Empty;
			}
			else
			{
				return EntityStatus.Valid;
			}
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
			var statusCollection = nonNullEntities.Select (x => x.EntityStatus);

			return EntityStatusHelper.CombineStatus (cardinality, statusCollection.ToArray ());
		}

		public static EntityStatus CombineStatus(StatusHelperCardinality cardinality, params EntityStatus[] collection)
		{
			if (collection == null || collection.Length == 0)
			{
				return EntityStatus.Empty;
			}

			if (collection.Any (x => x == EntityStatus.Invalid))
			{
				return EntityStatus.Invalid;
			}

			if (collection.All (x => x == EntityStatus.Empty || x == EntityStatus.EmptyAndValid))
			{
				return EntityStatus.Empty;
			}

			if (cardinality == StatusHelperCardinality.AtLeastOne)
			{
				if (collection.Any (x => x == EntityStatus.Valid || x == EntityStatus.EmptyAndValid))
				{
					return EntityStatus.Valid;
				}
			}
			else
			{
				if (collection.All (x => x == EntityStatus.Valid || x == EntityStatus.EmptyAndValid))
				{
					return EntityStatus.Valid;
				}
			}

			return EntityStatus.Unknown;
		}
	}
}