//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>HintComparerResult</c> enumeration defines the values returned by the
	/// <see cref="AutoCompleteTextField.HintComparer"/> callback.
	/// </summary>
	/// <remarks>
	/// Implementation detail: the best match has the smallest value.
	/// </remarks>
	public enum HintComparerResult
	{
		/// <summary>
		/// There was a primary match (e.g. the word starts exactly the same as what
		/// was provided to the hint comparer).
		/// </summary>
		PrimaryMatch = 0,

		/// <summary>
		/// There was a secondary match (e.g. a partial match can be found by the hint
		/// comparer).
		/// </summary>
		SecondaryMatch = 1,

		/// <summary>
		/// There was no match.
		/// </summary>
		NoMatch = 2,
	}

	public delegate HintComparerResult HintComparerPredicate<T1, T2>(T1 arg1, T2 arg2);
}
