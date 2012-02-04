//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Compta.Widgets
{
	/// <summary>
	/// The <c>HintComparerPredicate</c> is a function which takes two arguments and
	/// returns a <see cref="HintComparerResult"/>.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st argument.</typeparam>
	/// <typeparam name="T2">The type of the 2nd argument.</typeparam>
	/// <param name="arg1">The source argument.</param>
	/// <param name="arg2">The candidate argument (usually what was typed in by the user).</param>
	/// <returns>The result of the comparison.</returns>
	public delegate HintComparerResult HintComparerPredicate<T1, T2>(T1 arg1, T2 arg2);
}
