//	Copyright © 2004-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IValidationResult</c> interface is used to retrieve a validation result.
	/// </summary>
	public interface IValidationResult
	{
		/// <summary>
		/// Gets a value indicating whether the associated widget contains valid data,
		/// ie <see cref="State"/> is set to <c>ValidationState.Ok</c>.
		/// </summary>
		/// <value><c>true</c> if the associated widget contains valid data; otherwise, <c>false</c>.</value>
		bool IsValid
		{
			get;
		}

		/// <summary>
		/// Gets the validation state (basically OK or not OK).
		/// </summary>
		/// <value>The validation state.</value>
		ValidationState State
		{
			get;
		}

		/// <summary>
		/// Gets the error message explaining what is wrong with the data.
		/// </summary>
		/// <value>The error message.</value>
		FormattedText ErrorMessage
		{
			get;
		}
	}
}
