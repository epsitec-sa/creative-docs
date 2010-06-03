using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.Helpers
{


	public class EntityCollectionFieldProxy: IEntityProxy
	{


		public EntityCollectionFieldProxy(DataContext dataContext, AbstractEntity entity, StructuredTypeField field)
		{
			this.dataContext = dataContext;
			this.entity = entity;
			this.field = field;
		}


		#region IEntityProxy Members


		public object GetReadEntityValue(IValueStore store, string id)
		{
			object value = this.PromoteToRealInstance ();

			store.SetValue (id, value, ValueStoreSetMode.Default);

			return value;
		}


		public object GetWriteEntityValue(IValueStore store, string id)
		{
			return this;
		}

		
		public bool DiscardWriteEntityValue(IValueStore store, string id, ref object value)
		{
			return false;
		}


		public object PromoteToRealInstance()
		{
			Druid entityId = this.entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.dataContext.EntityContext.GetLocalEntityId (entityId, this.field.CaptionId);

			//System.Type itemType = this.dataContext.EntityContext.CreateEmptyEntity (this.field.TypeId).GetType ();
			//System.Type collectionType = typeof (EntityCollection<>).MakeGenericType (itemType);

			//IList targets = System.Activator.CreateInstance (collectionType, this.field.Id, this.entity, false) as IList;

			IList targets = new EntityCollection<AbstractEntity> (this.field.Id, this.entity, false) as IList;

			using (this.entity.DefineOriginalValues ())
			{
				foreach (object item in this.dataContext.ReadFieldRelation (this.entity, localEntityId, this.field, EntityResolutionMode.Load))
				{
					targets.Add (item);
				}
			}

			return targets;
		}


		#endregion
	
		
		private DataContext dataContext;


		private AbstractEntity entity;


		private StructuredTypeField field;


	}


}
