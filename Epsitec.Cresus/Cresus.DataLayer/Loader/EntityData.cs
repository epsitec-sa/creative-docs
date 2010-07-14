using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;


namespace Epsitec.Cresus.DataLayer.Loader
{


	/// <summary>
	/// The <c>EntityDataContainer</c> class is used to store the data of an <see cref="AbstractEntity"/>
	/// and to transfer this data from the <see cref="DataBrowser"/> to the <see cref="DataContext"/>.
	/// </summary>
	/// <remarks>
	/// Note that this class might be used to store incomplete data. If the type of the
	/// <see cref="AbstractEntity"/> is a derived type, it might happen that only the data of some of
	/// its base type might be loaded and stored in a <c>EntityDataContainer</c>.
	/// 
	/// If <see cref="LoadedEntityId"/> and <see cref="LeafEntityId"/> are equal, then the data
	/// of an <see cref="AbstractEntity"/> is fully stored in the <see cref="EntityData"/>.
	/// If they are not equal, the data of the type represented by <see cref="LoadedEntityId"/> until
	/// the type at the top of the inheritance chain are stored in the <see cref="EntityData"/>
	/// but the data of the all the types that are lower in the inheritance chain is not stored in
	/// the <see cref="EntityData"/>.
	/// </remarks>
	internal sealed class EntityData
	{


		/// <summary>
		/// Creates a new <c>EntityDataContainer</c> for an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="key">The key of the <see cref="AbstractEntity"/>.</param>
		/// <param name="loadedEntityId">The loaded entity id of the <see cref="AbstractEntity"/>.</param>
		/// <param name="concreteEntityId">The concrete entity id of the <see cref="AbstractEntity"/>.</param>
		/// <param name="valueData">The value data of the <see cref="AbstractEntity"/>.</param>
		/// <param name="referenceData">The reference data of the <see cref="AbstractEntity"/>.</param>
		/// <param name="collectionData">The collection data of the <see cref="AbstractEntity"/>.</param>
		public EntityData(DbKey key, Druid loadedEntityId, Druid concreteEntityId, ValueData valueData, ReferenceData referenceData, CollectionData collectionData)
		{
			this.Key = key;
			this.LoadedEntityId = loadedEntityId;
			this.LeafEntityId = concreteEntityId;
			this.ValueData = valueData;
			this.ReferenceData = referenceData;
			this.CollectionData = collectionData;
		}


		/// <summary>
		/// Gets or sets the key of the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <value>The key of the <see cref="AbstractEntity"/>.</value>
		public DbKey Key
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
		/// Gets or sets the concrete entity id of the <see cref="AbstractEntity"/>. This is the id
		/// of the most derived type that it implements.
		/// </summary>
		/// <value>The concrete entity id of the <see cref="AbstractEntity"/>.</value>
		public Druid LeafEntityId
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
