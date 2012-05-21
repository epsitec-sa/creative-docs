//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>Accessor</c> class is the base class for <see cref="Accessor{TResult}"/>.
	/// </summary>
	public abstract class Accessor
	{
		/// <summary>
		/// Creates an accessor based on two functions: the first returns the source object
		/// and the second returns the value based on the source.
		/// </summary>
		/// <typeparam name="T">The type of the source.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="sourceValueFunction">The function which returns the source object.</param>
		/// <param name="resultValueFunction">The function which returns the value based on the source.</param>
		/// <returns>The accessor.</returns>
		public static Accessor<TResult> Create<T, TResult>(System.Func<T> sourceValueFunction, System.Func<T, TResult> resultValueFunction)
			where T : new ()
		{
			return new Accessor<TResult> (() => resultValueFunction (sourceValueFunction ()));
		}
	}
}
