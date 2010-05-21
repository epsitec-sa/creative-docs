using Epsitec.Cresus.Database;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer
{
	
	
	public class EntityData
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


		public EntityValueData ValueData
		{
			get;
			private set;
		}


		public EntityReferenceData ReferenceData
		{
			get;
			private set;
		}


		public EntityCollectionData CollectionData
		{
			get;
			private set;
		}


		public EntityData(DbKey key, Druid loadedEntityId, Druid realEntityId, EntityValueData valueData, EntityReferenceData referenceData, EntityCollectionData collectionData)
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
