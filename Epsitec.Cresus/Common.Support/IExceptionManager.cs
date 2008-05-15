//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>IExceptionManager</c> interface provides a generic means of
	/// executing code which might throw exceptions and for which special
	/// handling is required (i.e. eat the exception and log the error).
	/// </summary>
	public interface IExceptionManager
	{
		/// <summary>
		/// Executes the specified function with special exception handling.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="func">The function to execute.</param>
		/// <param name="logSourceInfoGetter">The function to call in order
		/// to get source information if logging is required.</param>
		/// <returns>The result of the function execution.</returns>
		TResult Execute<TResult>(System.Func<TResult> func, System.Func<string> logSourceInfoGetter);
	}
}
