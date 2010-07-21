using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;


namespace Epsitec.Cresus.DataLayer.Loader
{


	/// <summary>
	/// The <c>EntityDataContainer</c> class is used to store the data of an <see cref="AbstractEntity"/>
	/// and to transfer this data from the <see cref="LoaderQueryGenerator"/> to the <see cref="DataLoader"/>.
	/// </summary>
	/// <remarks>
	/// Note that this class might be used to store incomplete data. The value of th property
	/// <see cref="LoadedEntityId"/> is the most derived type of the <see cref="AbstractEntity"/>
	/// which has been loaded in memory. If the <see cref="AbstractEntity"/> has a more derived type,
	/// that means that its data has not been loaded in memory.
	/// </remarks>
	internal sealed class EntityData
	{


		/// <summary>
		/// Creates a new <c>EntityData</c> for an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entityKey">The <see cref="EntityKey"/> that identifies the <see cref="AbstractEntity"/>.</param>
		/// <param name="loadedEntityId">The loaded entity id of the <see cref="AbstractEntity"/>.</param>
		/// <param name="valueData">The value data of the <see cref="AbstractEntity"/>.</param>
		/// <param name="referenceData">The reference data of the <see cref="AbstractEntity"/>.</param>
		/// <param name="collectionData">The collection data of the <see cref="AbstractEntity"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="valueData"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="referenceData"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="collectionData"/> is null.</exception>
		public EntityData(EntityKey entityKey, Druid loadedEntityId, ValueData valueData, ReferenceData referenceData, CollectionData collectionData)
		{
			if (valueData == null)
			{
				throw new System.ArgumentNullException ("valueData");
			}

			if (referenceData == null)
			{
				throw new System.ArgumentNullException ("referenceData");
			}
			if (collectionData == null)
			{
				throw new System.ArgumentNullException ("collectionData");
			}
			
			this.EntityKey = entityKey;
			this.LoadedEntityId = loadedEntityId;
			this.ValueData = valueData;
			this.ReferenceData = referenceData;
			this.CollectionData = collectionData;
		}


		/// <summary>
		/// Gets or sets the <see cref="EntityKey"/> that identifies the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <value>The key of the <see cref="AbstractEntity"/>.</value>
		public EntityKey EntityKey
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets or sets the loaded entity id of the <see cref="AbstractEntity"/>. This is the id of
		/// the most derived type that it implements and has been loaded in memory.
		/// </summary>
		/// <value>The loaded entity id of the <see cref="AbstractEntity"/>.</value>
		public Druid LoadedEntityId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets or sets the value data of the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <value>The value data of the <see cref="AbstractEntity"/>.</value>
		public ValueData ValueData
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets or sets the reference data of the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <value>The reference data of the <see cref="AbstractEntity"/>.</value>
		public ReferenceData ReferenceData
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets or sets the collection data of the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <value>The collection data of the <see cref="AbstractEntity"/>.</value>
		public CollectionData CollectionData
		{
			get;
			private set;
		}


	}


}
