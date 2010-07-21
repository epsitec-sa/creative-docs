namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>Type</c> enumeration defines the types that can be used for a <see cref="Expression"/>.
	/// </summary>
	public enum Type
	{


		/// <summary>
		/// The type for boolean values.
		/// </summary>
		Boolean,


		/// <summary>
		/// The type for short integer values.
		/// </summary>
		Int16,

		
		/// <summary>
		/// The type for regular integer values.
		/// </summary>
		Int32,

		
		/// <summary>
		/// The type for long integer values.
		/// </summary>
		Int64,


		/// <summary>
		/// The type for floating point values.
		/// </summary>
		Double,


		/// <summary>
		/// The type for string values.
		/// </summary>
		String,


		/// <summary>
		/// The type for date values. A date defines a day.
		/// </summary>
		Date,


		/// <summary>
		/// The type for time values. A time defines a point in time within a day.
		/// </summary>
		Time,


		/// <summary>
		/// The type for datetime values. A datetime defines a point in time within a day and defines
		/// the day.
		/// </summary>
		DateTime,


	}


}
