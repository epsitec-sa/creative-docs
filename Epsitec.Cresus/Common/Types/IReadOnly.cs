//	Copyright © 2004-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Exceptions;


namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IReadOnly</c> interface is used to check if an object is read
	/// only or not.
	/// </summary>
	public interface IReadOnly
	{
		bool IsReadOnly
		{
			get;
		}
	}

	/// <summary>
	/// The <c>IReadOnlyExtensions</c> class contains some helper methods related to
	/// <see cref="IReadOnly"/> instances.
	/// </summary>
	public static class IReadOnlyExtensions
	{
		/// <summary>
		/// Asserts that <paramref name="iReadOnlyObject"/> is in read only mode.
		/// </summary>
		/// <param name="iReadOnlyObject">The object whose state to check.</param>
		/// <exception cref="ReadOnlyException">If <paramref name="iReadOnlyObject"/> is not in read only mode.</exception>
		public static void AssertIsReadOnly(this IReadOnly iReadOnlyObject)
		{
			if (!iReadOnlyObject.IsReadOnly)
			{
				string message = "Object " + iReadOnlyObject + "  is not read only.";

				throw new ReadOnlyException (iReadOnlyObject, message);
			}
		}
		
		/// <summary>
		/// Asserts that <paramref name="iReadOnlyObject"/> is not in read only mode.
		/// </summary>
		/// <param name="iReadOnlyObject">The object whose state to check.</param>
		/// <exception cref="ReadOnlyException">If <paramref name="iReadOnlyObject"/> is in read only mode.</exception>
		public static void AssertIsNotReadOnly(this IReadOnly iReadOnlyObject)
		{
			if (iReadOnlyObject.IsReadOnly)
			{
				string message = "Object " + iReadOnlyObject + "  is read only.";

				throw new ReadOnlyException (iReadOnlyObject, message);
			}
		}
	}

}
