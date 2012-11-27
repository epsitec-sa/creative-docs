//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
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
		public bool								IsValid
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
		public ValidationState					State
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
		public FormattedText					ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}


		/// <summary>
		/// Gets the valid state.
		/// </summary>
		public static readonly ValidationResult Ok = new ValidationResult (ValidationState.Ok);


		/// <summary>
		/// Gets the invalid error result.
		/// </summary>
		public static readonly ValidationResult Error = new ValidationResult (ValidationState.Error);


		/// <summary>
		/// Create an validation result based on the given boolean.
		/// </summary>
		/// <param name="valid">True to create an ok result, false to create an error result.</param>
		/// <returns>The result.</returns>
		public static IValidationResult Create(bool valid)
		{
			return valid
				? ValidationResult.Ok
				: ValidationResult.Error;
		}

		/// <summary>
		/// Creates an error.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		/// <returns>The error.</returns>
		public static ValidationResult CreateError(FormattedText errorMessage)
		{
			return new ValidationResult (ValidationState.Error, errorMessage);
		}

		public static ValidationResult CreateError(FormattedText errorMessage, string arg)
		{
			return new ValidationResult (ValidationState.Error, FormattedText.Format (errorMessage, arg));
		}

		public static ValidationResult CreateError(FormattedText errorMessage, params object[] args)
		{
			return new ValidationResult (ValidationState.Error, FormattedText.Format (errorMessage, args));
		}

		/// <summary>
		/// Creates a warning.
		/// </summary>
		/// <param name="warningMessage">The warning message.</param>
		/// <returns>The warning.</returns>
		public static ValidationResult CreateWarning(FormattedText warningMessage)
		{
			return new ValidationResult (ValidationState.Warning, warningMessage);
		}

		public static ValidationResult CreateWarning(FormattedText warningMessage, string arg)
		{
			return new ValidationResult (ValidationState.Warning, FormattedText.Format (warningMessage, arg));
		}

		public static ValidationResult CreateWarning(FormattedText warningMessage, params object[] args)
		{
			return new ValidationResult (ValidationState.Warning, FormattedText.Format (warningMessage, args));
		}

		private readonly ValidationState		state;
		private readonly FormattedText			errorMessage;
	}
}
