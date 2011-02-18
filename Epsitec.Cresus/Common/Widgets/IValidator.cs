//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>IValidator</c> interface is used to find out if a widget contains
	/// valid data or not.
	/// </summary>
	public interface IValidator : IValidationResult
	{
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
