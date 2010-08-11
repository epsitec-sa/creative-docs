using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Serialization
{


	/// <summary>
	/// The <c>EntityReferenceData</c> class stores the reference data of an <see cref="AbstractEntity"/>.
	/// The reference data of an <see cref="AbstractEntity"/> is defined as the data which is stored
	/// as a simple relation to another <see cref="AbstractEntity"/>, excluding the collections.
	/// </summary>
	internal sealed class ReferenceData
	{


		/// <summary>
		/// Initializes a new instance of the <c>EntityReferenceData</c> class.
		/// </summary>
		public ReferenceData()
		{
			this.referenceKeys = new Dictionary<Druid, DbKey> ();
		}


		/// <summary>
		/// Gets or sets the reference of the field with the field id <paramref name="fieldId"/>.
		/// </summary>
		/// <param name="fieldId">The id of the field whose reference to set or get.</param>
		/// <value>The reference of the field with the id <paramref name="fieldId"/>.</value>
		/// <exception cref="System.ArgumentException">If the provided value is null.</exception>
		public DbKey? this[Druid fieldId]
		{
			get
			{
				if (this.referenceKeys.ContainsKey (fieldId))
				{
					return this.referenceKeys[fieldId];
				}
				else
				{
					return null;
				}
			}
			set
			{
				value.ThrowIfWithoutValue ("value");

				this.referenceKeys[fieldId] = value.Value;
			}
		}


		/// <summary>
		/// Stores the references of the <see cref="AbstractEntity"/> to other
		/// <see cref="AbstractEntity"/>. They are stored by field.
		/// </summary>
		private Dictionary<Druid, DbKey> referenceKeys;


	}


}
