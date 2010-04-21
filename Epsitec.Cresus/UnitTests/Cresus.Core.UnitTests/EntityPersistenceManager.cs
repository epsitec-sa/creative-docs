//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>EntityPersistenceManager</c> class implements a simple persistence
	/// manager which associates entities with ids through a dictionary.
	/// </summary>
	sealed class EntityPersistenceManager : IEntityPersistenceManager
	{
		#region IEntityPersistenceManager Members

		public string GetPersistedId(AbstractEntity entity)
		{
			foreach (var item in this.Map)
			{
				if (item.Value == entity)
				{
					return item.Key;
				}
			}

			return null;
		}

		public AbstractEntity GetPeristedEntity(string id)
		{
			AbstractEntity entity;

			if (this.Map.TryGetValue (id, out entity))
			{
				return entity;
			}
			else
			{
				return null;
			}
		}

		#endregion

		public readonly Dictionary<string, AbstractEntity> Map = new Dictionary<string, AbstractEntity> ();
	}
}
