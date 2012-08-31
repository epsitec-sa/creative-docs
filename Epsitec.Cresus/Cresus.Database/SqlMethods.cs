//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlMethods</c> class is inspired by what Microsoft implemented for LINQ to SQL.
	/// See http://msdn.microsoft.com/en-us/library/bb302445.
	/// </summary>
	public static class SqlMethods
	{
		/// <summary>
		/// Determines whether a specific value matches a specified pattern. This method
		/// is currently only supported when generating SQL queries based on expression
		/// trees.
		/// <remarks>Calling this method will throw a <see cref="System.NotSupportedException"/> exception.</remarks>
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="pattern">The pattern.</param>
		/// <returns><c>true</c> if the value matches the pattern; otherwise, <c>false</c>.</returns>
		public static bool Like(string value, string pattern)
		{
			throw new System.NotSupportedException ();
		}

		/// <summary>
		/// Determines whether a specific value matches a specified pattern. This method
		/// is currently only supported when generating SQL queries based on expression
		/// trees. The pattern matching characters can be escaped in the value.
		/// <remarks>Calling this method will throw a <see cref="System.NotSupportedException"/> exception.</remarks>
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="pattern">The pattern.</param>
		/// <param name="escapeCharacter">The escape character.</param>
		/// <returns>
		///   <c>true</c> if the value matches the pattern; otherwise, <c>false</c>.
		/// </returns>
		public static bool Like(string value, string pattern, char escapeCharacter)
		{
			throw new System.NotSupportedException ();
		}
	}
}
