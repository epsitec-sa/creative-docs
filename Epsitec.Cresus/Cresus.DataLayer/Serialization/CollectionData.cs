using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Serialization
{


	/// <summary>
	/// The <c>EntityCollectionData</c> class stores the collection data of an <see cref="AbstractEntity"/>.
	/// The collection data of an <see cref="AbstractEntity"/> is defined as the data which is stored
	/// as a list of references to other <see cref="AbstractEntity"/>.
	/// </summary>
	internal sealed class CollectionData
	{


		/// <summary>
		/// Initializes a new instance of the <c>EntityCollectionData</c> class.
		/// </summary>
		public CollectionData()
		{
			this.collectionKeys = new Dictionary<Druid, List<DbKey>> ();
		}


		/// <summary>
		/// Gets or sets the list of references of the field with the field id
		/// <paramref name="fieldId"/>.
		/// </summary>
		/// <param name="fieldId">The id of the field whose list of reference to set or get.</param>
		/// <value>The list of reference of the field with the id <paramref name="fieldId"/>.</value>
		public List<DbKey> this[Druid fieldId]
		{
			get
			{
				if (!this.collectionKeys.ContainsKey (fieldId))
				{
					this.collectionKeys[fieldId] = new List<DbKey> ();
				}
		
				return this.collectionKeys[fieldId];
			}
		}


		/// <summary>
		/// Stores the collections of the <see cref="AbstractEntity"/> to other
		/// <see cref="AbstractEntity"/>. They are stored by field.
		/// </summary>
		private Dictionary<Druid, List<DbKey>> collectionKeys;


	}


}
