//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ValidationResult</c> class stores the result of a validation:
	/// the <see cref="ValidationState"/> and the associated error message,
	/// if any.
	/// </summary>
	public sealed class ValidationResult : IValidationResult
	{
		public ValidationResult(ValidationState state, FormattedText errorMessage = default (FormattedText))
		{
			this.state = state;
			this.errorMessage = errorMessage;
		}
		
		/// <summary>
		/// Gets a value indicating whether the associated widget contains valid data,
		/// i.e. <c>State</c> is set to <c>ValidationState.Ok</c>.
		/// </summary>
		/// <value>
		/// <c>true</c> if the associated widget contains valid data; otherwise, <c>false</c>.</value>
		public bool IsValid
		{
			get
			{
				return this.State == ValidationState.Ok;
			}
		}

		/// <summary>
		/// Gets the validation state (basically OK or not OK).
		/// </summary>
		/// <value>The validation state.</value>
		public ValidationState State
		{
			get
			{
				return this.state;
			}
		}
		
		/// <summary>
		/// Gets the error message explaining what is wrong with the data.
		/// </summary>
		/// <value>The error message.</value>
		public FormattedText ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}


		private readonly ValidationState state;
		private readonly FormattedText errorMessage;
	}
}
