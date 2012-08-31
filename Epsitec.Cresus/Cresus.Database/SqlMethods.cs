//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Reflection;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlMethods</c> class is inspired by what Microsoft implemented for LINQ to SQL.
	/// See http://msdn.microsoft.com/en-us/library/bb302445.
	/// </summary>
	public static class SqlMethods
	{
		public static MethodInfo				LikeMethodInfo
		{
			get
			{
				return SqlMethods.likeMethodInfo;
			}
		}

		public static MethodInfo				EscapedLikeMethodInfo
		{
			get
			{
				return SqlMethods.escapedLikeMethodInfo;
			}
		}

		public static MethodInfo				IsNullMethodInfo
		{
			get
			{
				return SqlMethods.isNullMethodInfo;
			}
		}

		public static MethodInfo				IsNotNullMethodInfo
		{
			get
			{
				return SqlMethods.isNotNullMethodInfo;
			}
		}


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

		public static bool IsNull(object value)
		{
			return value == null;
		}

		public static bool IsNotNull(object value)
		{
			return value != null;
		}

		
		static SqlMethods()
		{
			SqlMethods.likeMethodInfo        = typeof (SqlMethods).GetMethod ("Like", new System.Type[] { typeof (string), typeof (string) });
			SqlMethods.escapedLikeMethodInfo = typeof (SqlMethods).GetMethod ("Like", new System.Type[] { typeof (string), typeof (string), typeof (char) });
			SqlMethods.isNullMethodInfo      = typeof (SqlMethods).GetMethod ("IsNull");
			SqlMethods.isNotNullMethodInfo   = typeof (SqlMethods).GetMethod ("IsNotNull");
		}


		private static readonly MethodInfo		likeMethodInfo;
		private static readonly MethodInfo		escapedLikeMethodInfo;
		private static readonly MethodInfo		isNullMethodInfo;
		private static readonly MethodInfo		isNotNullMethodInfo;
	}
}
