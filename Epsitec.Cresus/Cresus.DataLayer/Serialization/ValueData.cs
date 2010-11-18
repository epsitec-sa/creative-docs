using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Serialization
{


	/// <summary>
	/// The <c>EntityValueData</c> class stores the value data of an <see cref="AbstractEntity"/>. The
	/// value data of an <see cref="AbstractEntity"/> is defined as the data which is stored directly
	/// in the tables of the <see cref="AbstractEntity"/>.
	/// </summary>
	internal sealed class ValueData
	{


		/// <summary>
		/// Initializes a new instance of the <c>EntityValueData</c> class.
		/// </summary>
		public ValueData()
		{
			this.values = new Dictionary<Druid, object> ();
		}


		/// <summary>
		/// Gets or sets the value of the field with the field id <paramref name="fieldId"/>.
		/// </summary>
		/// <param name="fieldId">The id of the field whose value to set or get.</param>
		/// <value>The value of the field with the id <paramref name="fieldId"/>.</value>
		public object this[Druid fieldId]
		{
			get
			{
				return this.values.ContainsKey (fieldId) ? this.values[fieldId] : null;
			}
			set
			{
				this.values[fieldId] = value;
			}
		}


		/// <summary>
		/// Tells whether there is a value in this instance for the given field.
		/// </summary>
		/// <param name="fieldId">The <see cref="Druid"/> of the field to check.</param>
		/// <returns><c>true</c> if this  instance contains a value for the given field, <c>false</c> if it does not.</returns>
		public bool ContainsValue(Druid fieldId)
		{
			return this.values.ContainsKey (fieldId);
		}


		/// <summary>
		/// Stores the values of the fields of the <see cref="AbstractEntity"/>.
		/// </summary>
		private Dictionary<Druid, object> values;


	}


}
