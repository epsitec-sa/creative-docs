//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ExceptionHelper</c> class provides support for managing
	/// exceptions.
	/// </summary>
	public static class ExceptionHelper
	{
		/// <summary>
		/// Preserves the full stack trace for the given exception, when it is
		/// either being rethrown using <code>throw;</code> or <code>throw ex;</code>.
		/// </summary>
		/// <param name="ex">The exception which will be rethrown.</param>
		public static void PreserveStackTrace(System.Exception ex)
		{
			System.Type exceptionType = typeof (System.Exception);
			System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
			System.Reflection.MethodInfo preserve = exceptionType.GetMethod ("InternalPreserveStackTrace", flags);

			if (preserve != null)
			{
				preserve.Invoke (ex, null);
			}
		}
	}
}
