using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Helpers
{


	internal class EntityFieldProxy : IEntityProxy
	{


		public EntityFieldProxy(DataContext dataContext, AbstractEntity entity, StructuredTypeField field)
		{
			this.dataContext = dataContext;
			this.entity = entity;
			this.field = field;
		}



		#region IEntityProxy Members


		public object GetReadEntityValue(Common.Types.IValueStore store, string id)
		{
			object value = this.PromoteToRealInstance ();
			store.SetValue (id, value, ValueStoreSetMode.Default);

			return value;
		}


		public object GetWriteEntityValue(Common.Types.IValueStore store, string id)
		{
			return this;
		}


		public bool DiscardWriteEntityValue(Common.Types.IValueStore store, string id, ref object value)
		{
			return false;
		}


		public object PromoteToRealInstance()
		{
			Druid entityId = this.entity.GetEntityStructuredTypeId();
			Druid localEntityId = this.entity.GetEntityContext ().GetLocalEntityId (entityId, this.field.CaptionId);

			return this.dataContext.ReadFieldRelation (this.entity, localEntityId, this.field, EntityResolutionMode.Load).FirstOrDefault ();
		}


		#endregion
		

		private DataContext dataContext;


		private AbstractEntity entity;


		private StructuredTypeField field;


	}


}
