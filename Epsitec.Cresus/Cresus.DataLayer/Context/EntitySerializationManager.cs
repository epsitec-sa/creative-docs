using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Loader.Data;


namespace Epsitec.Cresus.DataLayer.Context
{


	internal sealed class EntitySerializationManager
	{


		public EntitySerializationManager(DataContext dataContext)
		{
			this.DataContext = dataContext;
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		public EntityData Serialize(AbstractEntity entity)
		{
			throw new System.NotImplementedException ();
		}


		public AbstractEntity Deserialize(EntityData data)
		{
			throw new System.NotImplementedException ();
		}


	}


}
