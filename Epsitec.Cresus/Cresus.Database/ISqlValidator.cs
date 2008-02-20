//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>ISqlValidator</c> interface is used to validate SQL elements such as
	/// names, values, strings, etc.
	/// </summary>
	public interface ISqlValidator
	{
		/// <summary>
		/// Validates the name.
		/// </summary>
		/// <param name="value">The name to validate.</param>
		/// <returns><c>true</c> if validation is successful; <c>false</c> otherwise.</returns>
		bool ValidateName(string value);
		
		/// <summary>
		/// Validates the qualified name.
		/// </summary>
		/// <param name="value">The qualified name to validate.</param>
		/// <returns><c>true</c> if validation is successful; <c>false</c> otherwise.</returns>
		bool ValidateQualifiedName(string value);

		/// <summary>
		/// Validates the string.
		/// </summary>
		/// <param name="value">The string to validate.</param>
		/// <returns><c>true</c> if validation is successful; <c>false</c> otherwise.</returns>
		bool ValidateString(string value);

		/// <summary>
		/// Validates the number.
		/// </summary>
		/// <param name="value">The number to validate.</param>
		/// <returns><c>true</c> if validation is successful; <c>false</c> otherwise.</returns>
		bool ValidateNumber(string value);
	}
}
