using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Schema;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class FieldChecker
	{


		public FieldChecker(HashSet<AbstractEntity> entities, EntityTypeEngine entityTypeEngine, Action<HashSet<AbstractEntity>, EntityTypeEngine, AbstractEntity, Druid> publicFieldChecker, Action<HashSet<AbstractEntity>, AbstractEntity, string> internalFieldChecker)
		{
			this.entities = entities;
			this.entityTypeEngine = entityTypeEngine;
			this.publicFieldChecker = publicFieldChecker;
			this.internalFieldChecker = internalFieldChecker;
		}


		public void Check(AbstractEntity entity, Druid fieldId)
		{
			this.publicFieldChecker (this.entities, this.entityTypeEngine, entity, fieldId);
		}


		public void Check(AbstractEntity entity, string name)
		{
			this.internalFieldChecker (this.entities, entity, name);
		}


		private readonly HashSet<AbstractEntity> entities;


		private readonly EntityTypeEngine entityTypeEngine;


		private readonly Action<HashSet<AbstractEntity>, EntityTypeEngine, AbstractEntity, Druid> publicFieldChecker;


		private readonly Action<HashSet<AbstractEntity>, AbstractEntity, string> internalFieldChecker;


	}


}
