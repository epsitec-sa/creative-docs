using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.EntityData
{


	/// <summary>
	/// The <c>EntityReferenceData</c> class stores the reference data of an <see cref="AbstractEntity"/>.
	/// The reference data of an <see cref="AbstractEntity"/> is defined as the data which is stored
	/// as a simple relation to another <see cref="AbstractEntity"/>, excluding the collections.
	/// </summary>
	internal class EntityReferenceData
	{


		/// <summary>
		/// Gets or sets the reference of the field with the field id <paramref name="fieldId"/>.
		/// </summary>
		/// <param name="fieldId">The id of the field whose reference to set or get.</param>
		/// <value>The reference of the field with the id <paramref name="fieldId"/>.</value>
		public DbKey this[Druid fieldId]
		{
			get
			{
				return this.referenceKeys.ContainsKey (fieldId) ? this.referenceKeys[fieldId] : DbKey.Empty;
			}
			set
			{
				this.referenceKeys[fieldId] = value;
			}
		}


		/// <summary>
		/// Initializes a new instance of the <c>EntityReferenceData</c> class.
		/// </summary>
		public EntityReferenceData()
		{
			this.referenceKeys = new Dictionary<Druid, DbKey> ();
		}


		/// <summary>
		/// Determines whether the <c>EntityReferenceData</c> contains a reference for the field
		/// given by <paramref name="fieldId"/>.
		/// </summary>
		/// <param name="fieldId">The id of the field.</param>
		/// <returns>
		/// <c>true</c> if the <c>EntityValueData</c> contains a reference for the field and
		/// <c>false</c> otherwise.
		/// </returns>
		public bool Contains(Druid fieldId)
		{
			return this.referenceKeys.ContainsKey (fieldId);
		}


		/// <summary>
		/// Stores the references of the <see cref="AbstractEntity"/> to other
		/// <see cref="AbstractEntity"/>. They are stored by field.
		/// </summary>
		private Dictionary<Druid, DbKey> referenceKeys;


	}


}
