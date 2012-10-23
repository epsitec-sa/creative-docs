//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>FormattedIdGenerator</c> class implements the assignment of reference numbers
	/// to entities implementing <see cref="IReferenceNumber"/>.
	/// </summary>
	public class FormattedIdGenerator
	{
		public FormattedIdGenerator(RefIdGeneratorPool pool, IEnumerable<GeneratorDefinitionEntity> generatorDefinitions)
		{
			this.pool = pool;
			this.data = this.pool.Host;
			this.generatorDefinitions = generatorDefinitions;
		}


		public RefIdGeneratorPool				RefIdGeneratorPool
		{
			get
			{
				return this.pool;
			}
		}

		
		public bool AssignIds<T>(BusinessContext businessContext, T entity)
			where T : AbstractEntity, IReferenceNumber, new ()
		{
			bool assigned = false;
			var  helper   = new ReferenceIdFormatterHelper (this, businessContext, entity);

			foreach (var def in this.FindDefinition<T> ())
			{
				assigned |= helper.AssignId (def);
			}

			return assigned;
		}

		public IEnumerable<GeneratorDefinitionEntity> FindDefinition<T>()
			where T : AbstractEntity, new ()
		{
			return this.FindDefinition (EntityInfo<T>.GetTypeId ());
		}

		internal long GetGeneratorNextId(string name, long firstId)
		{
			return this.pool.GetGenerator (name, firstId).GetNextId ();
		}

		private IEnumerable<GeneratorDefinitionEntity> FindDefinition(Druid entityId)
		{
			string entityType = entityId.ToCompactString ();

			return generatorDefinitions.Where (x =>
				(x.Entity == entityType) &&
				(!string.IsNullOrEmpty (x.IdField)) &&
				(!string.IsNullOrEmpty (x.Format)));
		}

		internal static System.Action<string> GetAssigner(GeneratorDefinitionEntity definition, IReferenceNumber entity)
		{
			switch (definition.IdField.ToUpperInvariant ())
			{
				case "A": return x => entity.IdA = x;
				case "B": return x => entity.IdB = x;
				case "C": return x => entity.IdC = x;

				default:
					return null;
			}
		}

		private readonly RefIdGeneratorPool		pool;
		private readonly CoreData				data;
		private readonly IEnumerable<GeneratorDefinitionEntity> generatorDefinitions;
	}
}
