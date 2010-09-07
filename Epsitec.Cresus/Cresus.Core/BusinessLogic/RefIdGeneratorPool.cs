//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
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


		public RefIdGenerator GetGenerator<T>(string suffix = null)
			where T : AbstractEntity, new ()
		{
			string generatorName = RefIdGeneratorPool.GetGeneratorName<T> (suffix);
			RefIdGenerator generator = null;

			if (this.generators.TryGetValue (generatorName, out generator))
			{
				return generator;
			}

			generator = new RefIdGenerator (generatorName, this);
			this.generators[generatorName] = generator;

			return generator;
		}


		private static string GetGeneratorName<T>(string suffix)
			where T : AbstractEntity, new ()
		{
			Druid entityId = EntityInfo.GetTypeId<T> ();

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
