//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core.Business
{
	public class FormattedIdGenerator
	{
		public FormattedIdGenerator(RefIdGeneratorPool pool, IEnumerable<GeneratorDefinitionEntity> generatorDefinitions)
		{
			this.pool = pool;
			this.data = this.pool.Host;
			this.generatorDefinitions = generatorDefinitions;
		}

		public bool AssignIds<T>(IBusinessContext businessContext, T entity)
			where T : AbstractEntity, IReferenceNumber, new ()
		{
			bool assigned = false;
			var  helper   = new Helpers.FormatterHelper (this, businessContext, entity);

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

		internal long GetGeneratorNextId(string name)
		{
			return this.pool.GetGenerator (name).GetNextId ();
		}

		private IEnumerable<GeneratorDefinitionEntity> FindDefinition(Druid entityId)
		{
			string entityType = entityId.ToString ();

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
