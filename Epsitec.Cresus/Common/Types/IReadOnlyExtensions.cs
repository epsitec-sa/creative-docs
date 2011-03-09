//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Types.Exceptions;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IReadOnlyExtensions</c> class contains some helper methods related to
	/// <see cref="IReadOnly"/> instances.
	/// </summary>
	public static class IReadOnlyExtensions
	{
		/// <summary>
		/// Asserts that <paramref name="obj"/> is in read only mode.
		/// </summary>
		/// <param name="obj">The object whose state to check.</param>
		/// <exception cref="ReadOnlyException">If <paramref name="obj"/> is not in read only mode.</exception>
		public static void ThrowIfReadWrite(this IReadOnly obj)
		{
			if (!obj.IsReadOnly)
			{
				string message = "Object " + obj + "  is not read only.";

				throw new ReadOnlyException (obj, message);
			}
		}

		/// <summary>
		/// Asserts that <paramref name="obj"/> is not in read only mode.
		/// </summary>
		/// <param name="obj">The object whose state to check.</param>
		/// <exception cref="ReadOnlyException">If <paramref name="obj"/> is in read only mode.</exception>
		public static void ThrowIfReadOnly(this IReadOnly obj)
		{
			if (obj.IsReadOnly)
			{
				string message = string.Format ("Object {0} is read only", obj);

				throw new ReadOnlyException (obj, message);
			}
		}
	}
}
