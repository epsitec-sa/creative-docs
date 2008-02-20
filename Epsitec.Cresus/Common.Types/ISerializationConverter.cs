//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ISerializationConverter</c> is used by the serialization engine
	/// to convert an object to a string (when serializing) and vice versa (when
	/// deserializing).
	/// </summary>
	public interface ISerializationConverter
	{
		/// <summary>
		/// Converts the value to a serializable string.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="context">The serialization context.</param>
		/// <returns>The value represented as a string.</returns>
		string ConvertToString(object value, IContextResolver context);

		/// <summary>
		/// Converts the serializable string to its live value.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <param name="context">The deserialization context.</param>
		/// <returns>The value represented by the string.</returns>
		object ConvertFromString(string value, IContextResolver context);
	}
}
