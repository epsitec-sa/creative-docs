//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public class UidGeneratorPool
	{
		public UidGeneratorPool(CoreData data)
		{
			this.data = data;
			this.generators = new Dictionary<string, UidGenerator> ();
		}


		public UidGenerator GetGenerator<T>(string suffix = null)
			where T : AbstractEntity, new ()
		{
			string generatorName = UidGeneratorPool.GetGeneratorName<T> (suffix);
			UidGenerator generator = null;

			if (this.generators.TryGetValue (generatorName, out generator))
			{
				return generator;
			}

			generator = new UidGenerator (generatorName, this);

			this.generators[generatorName] = generator;

			return generator;
		}


		private static string GetGeneratorName<T>(string suffix)
			where T : AbstractEntity, new ()
		{
			Druid entityId = EntityId.GetTypeId<T> ();

			if (string.IsNullOrEmpty (suffix))
			{
				return entityId.ToString ();
			}
			else
			{
				return string.Concat (entityId.ToString (), ".", suffix);
			}
		}


		private readonly CoreData data;
		private readonly Dictionary<string, UidGenerator> generators;
	}
}
