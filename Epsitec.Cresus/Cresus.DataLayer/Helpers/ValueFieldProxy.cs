using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.DataLayer.Helpers
{


	internal class ValueFieldProxy : IFieldProxy
	{


		public ValueFieldProxy(DataContext context, AbstractEntity entity, Druid entityId, DbKey rowKey, StructuredTypeField field)
		{
			this.context = context;
			this.entity = entity;
			this.entityId = entityId;
			this.rowKey = rowKey;
			this.field = field;
		}


		public object GetValue()
		{
			return this.context.GetFieldValue (entity, entityId, rowKey, field);
		}


		private readonly DataContext context;


		private readonly AbstractEntity entity;


		private readonly Druid entityId;


		private readonly DbKey rowKey;


		private readonly StructuredTypeField field;
	
	}


}
