//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>RefIdGeneratorPool</c> maintains a collection of <see cref="RefIdGenerator"/>
	/// instances.
	/// </summary>
	public class RefIdGeneratorPool
	{
		public RefIdGeneratorPool(CoreData data)
		{
			this.data = data;
			this.generators = new Dictionary<string, RefIdGenerator> ();
		}


		public CoreData Data
		{
			get
			{
				return this.data;
			}
		}


		public RefIdGenerator GetGenerator(string generatorName, long firstId = 0)
		{
			RefIdGenerator generator = null;

			if (this.generators.TryGetValue (generatorName, out generator))
			{
				return generator;
			}

			generator = new RefIdGenerator(generatorName, this, firstId);
			this.generators[generatorName] = generator;

			return generator;
		}
		
		public RefIdGenerator GetGenerator<T>(string suffix = null)
			where T : AbstractEntity, new ()
		{
			return this.GetGenerator (RefIdGeneratorPool.GetGeneratorName<T> (suffix), 1000L);
		}


		private static string GetGeneratorName<T>(string suffix)
			where T : AbstractEntity, new ()
		{
			Druid entityId = EntityInfo<T>.GetTypeId ();

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
		private readonly Dictionary<string, RefIdGenerator> generators;
	}
}
