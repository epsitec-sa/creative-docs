using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{
	
	
	internal sealed class EntityModificationViewer
	{


		public EntityModificationViewer(DataContext dataContext)
		{
			this.DataContext = dataContext;
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		public bool HasValueChanged(AbstractEntity entity, Druid fieldId)
		{
			object originalValue = this.GetOriginalValue (entity, fieldId);
			object modifiedValue = this.GetModifiedValue (entity, fieldId);

			return (originalValue != modifiedValue);
		}


		public bool HasReferenceChanged(AbstractEntity entity, Druid fieldId)
		{
			object originalValue = this.GetOriginalValue (entity, fieldId);
			object modifiedValue = this.GetModifiedValue (entity, fieldId);

			return (originalValue != modifiedValue);
		}


		public bool HasCollectionChanged(AbstractEntity entity, Druid fieldId)
		{
			IList<object> originalValues = (IList<object>) this.GetOriginalValue (entity, fieldId);
			IList<object> modifiedValues = (IList<object>) this.GetModifiedValue (entity, fieldId);

			return originalValues.SequenceEqual (modifiedValues);
		}
		

		private object GetOriginalValue(AbstractEntity entity, Druid fieldId)
		{
			IValueStore originalValues = entity.GetOriginalValues ();

			return this.GetValue (originalValues, fieldId);
		}


		private object GetModifiedValue(AbstractEntity entity, Druid fieldId)
		{
			IValueStore modifiedValues = entity.GetModifiedValues ();

			return this.GetValue (modifiedValues, fieldId);
		}


		private object GetValue(IValueStore valueStore, Druid fieldId)
		{
			return valueStore.GetValue (fieldId.ToResourceId ());
		}


	}


}
