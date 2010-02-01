//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>IValidator</c> interface is used to find out if a widget contains
	/// valid data or not.
	/// </summary>
	public interface IValidator
	{
		/// <summary>
		/// Gets a value indicating whether the associated widget contains valid data,
		/// i.e. <c>State</c> is set to <c>ValidationState.Ok</c>.
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
		string ErrorMessage
		{
			get;
		}

		/// <summary>
		/// Validates the associated data and updates the <c>State</c>.
		/// </summary>
		void Validate();


		/// <summary>
		/// Marks the validator as dirty, which means that the <c>State</c> will have
		/// to be revalidated.
		/// </summary>
		/// <param name="deep">If set to <c>true</c>, mark all children validators as dirty too.</param>
		void MakeDirty(bool deep);


		/// <summary>
		/// Occurs when the validator state became dirty, i.e. <c>State</c> changed to
		/// <c>ValidationState.Dirty</c>.
		/// </summary>
		event Support.EventHandler	BecameDirty;
	}
}
