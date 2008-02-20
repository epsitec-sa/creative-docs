//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>IRawTypeConverter</c> interface can be used to convert between
	/// an unsupported (external) raw type and its emulated (internal) variant.
	/// </summary>
	public interface IRawTypeConverter
	{
		/// <summary>
		/// Gets the external raw type for this converter.
		/// </summary>
		/// <value>The external raw type.</value>
		DbRawType ExternalType
		{
			get;
		}

		/// <summary>
		/// Gets the internal raw type for this converter.
		/// </summary>
		/// <value>The internal raw type.</value>
		DbRawType InternalType
		{
			get;
		}

		/// <summary>
		/// Gets the emulated raw type length.
		/// </summary>
		/// <value>The raw type length.</value>
		int Length
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the emulated raw type has a fixed length.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the emulated raw type has a fixed length; otherwise, <c>false</c>.
		/// </value>
		bool IsFixedLength
		{
			get;
		}

		/// <summary>
		/// Gets the character encoding.
		/// </summary>
		/// <value>The character encoding.</value>
		DbCharacterEncoding Encoding
		{
			get;
		}

		/// <summary>
		/// Converts the value from the external raw type to its internal representation.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The converted value.</returns>
		object ConvertToInternalType(object value);

		/// <summary>
		/// Converts the value from the internal raw type to its external representation.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The converted value.</returns>
		object ConvertFromInternalType(object value);
	}
}
