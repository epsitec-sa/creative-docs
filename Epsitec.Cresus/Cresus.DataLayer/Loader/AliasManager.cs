using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class AliasManager
	{


		public AliasManager()
		{
			this.entityTableAliases = new Dictionary<Tuple<AbstractEntity, Druid>, string> ();
			this.relationTableAliases = new Dictionary<Tuple<AbstractEntity, Druid, AbstractEntity>, string> ();

			this.AliasCount = 0;
		}


		public int AliasCount
		{
			get;
			set;
		}


		public string GetAlias()
		{
			var alias = "alias" + this.AliasCount;

			this.AliasCount++;

			return alias;
		}


		public string GetAlias(AbstractEntity entity, Druid typeId)
		{
			var key = Tuple.Create (entity, typeId);
			var aliases = this.entityTableAliases;

			return this.GetAlias (key, aliases);
		}


		public string GetAlias(AbstractEntity source, Druid typeId, AbstractEntity target)
		{
			var key = Tuple.Create (source, typeId, target);
			var aliases = this.relationTableAliases;

			return this.GetAlias (key, aliases);
		}


		private string GetAlias<T>(T key, Dictionary<T, string> aliases)
		{
			string alias;

			if (!aliases.TryGetValue (key, out alias))
			{
				alias = this.GetAlias ();

				aliases[key] = alias;
			}

			return alias;
		}


		private readonly Dictionary<Tuple<AbstractEntity, Druid>, string> entityTableAliases;


		private readonly Dictionary<Tuple<AbstractEntity, Druid, AbstractEntity>, string> relationTableAliases;


	}


}
