//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		public bool AssignIds<T>(T entity)
			where T : AbstractEntity, IReferenceNumber, new ()
		{
			bool assigned = false;

			foreach (var def in this.FindDefinition<T> ())
			{
				var assigner = FormattedIdGenerator.GetAssigner (def, entity);

				if (assigner != null)
				{
					string name  = this.GetKeyName (def);
					long   id    = this.pool.GetGenerator (name).GetNextId ();
					string value = this.FormatId (def, id);
					
					assigner (value);
					assigned = true;
				}
			}

			return assigned;
		}

		public IEnumerable<GeneratorDefinitionEntity> FindDefinition<T>()
			where T : AbstractEntity, new ()
		{
			return this.FindDefinition (EntityInfo<T>.GetTypeId ());
		}

		private IEnumerable<GeneratorDefinitionEntity> FindDefinition(Druid entityId)
		{
			string entityType = entityId.ToString ();

			return generatorDefinitions.Where (x =>
				(x.Entity == entityType) &&
				(!string.IsNullOrEmpty (x.IdField)) &&
				(!string.IsNullOrEmpty (x.Format)));
		}

		private static System.Action<string> GetAssigner(GeneratorDefinitionEntity definition, IReferenceNumber entity)
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

		private string GetKeyName(GeneratorDefinitionEntity definition)
		{
			return definition.Entity;
		}

		private string FormatId(GeneratorDefinitionEntity definition, long id)
		{
			return string.Format ("{0}", id);
		}
		
		private readonly RefIdGeneratorPool		pool;
		private readonly CoreData				data;
		private readonly IEnumerable<GeneratorDefinitionEntity> generatorDefinitions;
	}
}
