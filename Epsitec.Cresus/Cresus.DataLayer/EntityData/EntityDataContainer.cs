using Epsitec.Cresus.Database;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer.EntityData
{
	
	
	internal class EntityDataContainer
	{


		public DbKey Key
		{
			get;
			private set;
		}


		public Druid LoadedEntityId
		{
			get;
			private set;
		}

		
		public Druid RealEntityId
		{
			get;
			private set;
		}


		public EntityValueDataContainer ValueData
		{
			get;
			private set;
		}


		public EntityReferenceDataContainer ReferenceData
		{
			get;
			private set;
		}


		public EntityCollectionDataContainer CollectionData
		{
			get;
			private set;
		}


		public EntityDataContainer(DbKey key, Druid loadedEntityId, Druid realEntityId, EntityValueDataContainer valueData, EntityReferenceDataContainer referenceData, EntityCollectionDataContainer collectionData)
		{
			this.Key = key;
			this.LoadedEntityId = loadedEntityId;
			this.RealEntityId = realEntityId;
			this.ValueData = valueData;
			this.ReferenceData = referenceData;
			this.CollectionData = collectionData;
		}


	}


}
