using System;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class AliasManager
	{


		public AliasManager(EntityTypeEngine typeEngine)
		{
			this.typeEngine = typeEngine;
			this.aliases = new Dictionary<Tuple<AbstractEntity, Druid>, string> ();

			this.aliasCount = 0;
		}


		public string GetAlias()
		{
			var alias = "alias" + aliasCount;

			this.aliasCount++;

			return alias;
		}


		public string GetAlias(AbstractEntity entity)
		{
			var leafEntityId = entity.GetEntityStructuredTypeId ();
			var rootEntityId = this.typeEngine.GetRootType (leafEntityId).CaptionId;

			return this.GetAlias (entity, rootEntityId);
		}


		public string GetAlias(AbstractEntity entity, Druid typeId)
		{
			string alias;

			var key = Tuple.Create (entity, typeId);

			if (!this.aliases.TryGetValue (key, out alias))
			{
				alias = this.GetAlias ();

				this.aliases[key] = alias;
			}

			return alias;
		}


		private readonly EntityTypeEngine typeEngine;


		private readonly Dictionary<Tuple<AbstractEntity, Druid>, string> aliases;


		private int aliasCount;


	}


}
