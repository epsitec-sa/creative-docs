using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;


namespace Epsitec.Cresus.DataLayer.Serialization
{


	/// <summary>
	/// The <c>EntityDataContainer</c> class is used to store the data of an <see cref="AbstractEntity"/>
	/// and to transfer this data from the <see cref="LoaderQueryGenerator"/> to the <see cref="DataLoader"/>.
	/// </summary>
	/// <remarks>
	/// Note that this class might be used to store incomplete data. The value of the property
	/// <see cref="LoadedEntityId"/> is the most derived type of the <see cref="AbstractEntity"/>
	/// which has been loaded in memory. If the <see cref="AbstractEntity"/> has a more derived type,
	/// that means that its data has not been fully loaded in memory.
	/// </remarks>
	internal sealed class EntityData
	{


		/// <summary>
		/// Creates a new <c>EntityData</c> for an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="rowKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/> in the database.</param>
		/// <param name="leafEntityId">The concrete entity id of the <see cref="AbstractEntity"/>.</param>
		/// <param name="loadedEntityId">The loaded entity id of the <see cref="AbstractEntity"/>.</param>
		/// <param name="logId">The sequence number of log entry currently associated with the <see cref="AbstractEntity"/>.</param>
		/// <param name="valueData">The value data of the <see cref="AbstractEntity"/>.</param>
		/// <param name="referenceData">The reference data of the <see cref="AbstractEntity"/>.</param>
		/// <param name="collectionData">The collection data of the <see cref="AbstractEntity"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="valueData"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="referenceData"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="collectionData"/> is null.</exception>
		public EntityData(DbKey rowKey, Druid leafEntityId, Druid loadedEntityId, long logId, ValueData valueData, ReferenceData referenceData, CollectionData collectionData)
		{
			valueData.ThrowIfNull ("valueData");
			referenceData.ThrowIfNull ("referenceData");
			collectionData.ThrowIfNull ("collectionData");
			
			this.RowKey = rowKey;
			this.LeafEntityId = leafEntityId;
			this.LoadedEntityId = loadedEntityId;
			this.LogId = logId;
			this.ValueData = valueData;
			this.ReferenceData = referenceData;
			this.CollectionData = collectionData;
		}


		/// <summary>
		/// Gets the <see cref="DbKey"/> of the <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <value>The <see cref="DbKey"/> of the <see cref="AbstractEntity"/>.</value>
		public DbKey RowKey
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the <see cref="Druid"/> that identifies the concrete type of the
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		public Druid LeafEntityId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the loaded entity id of the <see cref="AbstractEntity"/>. This is the id of
		/// the most derived type that it implements and has been loaded in memory.
		/// </summary>
		/// <value>The loaded entity id of the <see cref="AbstractEntity"/>.</value>
		public Druid LoadedEntityId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the id of the log entry currently associated with the <see cref="AbstractEntity"/>.
		/// </summary>
		public long LogId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the value data of the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <value>The value data of the <see cref="AbstractEntity"/>.</value>
		public ValueData ValueData
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the reference data of the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <value>The reference data of the <see cref="AbstractEntity"/>.</value>
		public ReferenceData ReferenceData
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the collection data of the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <value>The collection data of the <see cref="AbstractEntity"/>.</value>
		public CollectionData CollectionData
		{
			get;
			private set;
		}


	}


}
