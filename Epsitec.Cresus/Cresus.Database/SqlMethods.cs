//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using System.Reflection;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlMethods</c> class is inspired by what Microsoft implemented for LINQ to SQL.
	/// See http://msdn.microsoft.com/en-us/library/bb302445.
	/// </summary>
	public static class SqlMethods
	{
		public static MethodInfo				CompareToMethodInfo
		{
			get
			{
				return SqlMethods.compareToMethodInfo;
			}
		}

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

		public static MethodInfo				IsInSetMethodInfo
		{
			get
			{
				return SqlMethods.isInSetMethodInfo;
			}
		}

		public static MethodInfo				IsNotInSetMethodInfo
		{
			get
			{
				return SqlMethods.isNotInSetMethodInfo;
			}
		}


		/// <summary>
		/// Compares two strings. This method is used when generating SQL queries based
		/// on expressions, since we cannot encode <c>a &lt; "x"</c> in an expression
		/// tree if we want to generate SQL from it.
		/// </summary>
		/// <param name="arg1">The first string argument.</param>
		/// <param name="arg2">The second string argument.</param>
		/// <returns>zero if both arguments are equal, <c>-1</c> if <paramref name="arg1"/> comes
		/// before <paramref name="arg2"/> or <c>1</c> if <paramref name="arg1"/> comes after
		/// <paramref name="arg2"/>.</returns>
		public static int CompareTo(string arg1, string arg2)
		{
			return arg1.CompareTo (arg2);
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
		/// trees. The pattern matching characters can be escaped in the value by using
		/// the escape character in Database.DbSqlStandard.CompareLikeEscape. The method
		/// DataLayer.Expressions.Constant.Escape provides a way to escape the pattern.
		/// <remarks>Calling this method will throw a <see cref="System.NotSupportedException"/> exception.</remarks>
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="pattern">The pattern.</param>
		/// <returns>
		///   <c>true</c> if the value matches the pattern; otherwise, <c>false</c>.
		/// </returns>
		public static bool EscapedLike(string value, string pattern)
		{
			throw new System.NotSupportedException ();
		}

		public static bool IsInSet(object value, IEnumerable<object> set)
		{
			throw new System.NotImplementedException ();
		}

		public static bool IsNotInSet(object value, IEnumerable<object> set)
		{
			throw new System.NotImplementedException ();
		}

		
		static SqlMethods()
		{
			SqlMethods.compareToMethodInfo   = typeof (SqlMethods).GetMethod ("CompareTo", new System.Type[] { typeof (string), typeof (string) });
			SqlMethods.likeMethodInfo        = typeof (SqlMethods).GetMethod ("Like", new System.Type[] { typeof (string), typeof (string) });
			SqlMethods.escapedLikeMethodInfo = typeof (SqlMethods).GetMethod ("EscapedLike", new System.Type[] { typeof (string), typeof (string) });
			SqlMethods.isInSetMethodInfo	 = typeof (SqlMethods).GetMethod ("IsInSet", new System.Type[] { typeof (object), typeof (IEnumerable<object>) });
			SqlMethods.isNotInSetMethodInfo	 = typeof (SqlMethods).GetMethod ("IsNotInSet", new System.Type[] { typeof (object), typeof (IEnumerable<object>) });
		}

		private static readonly MethodInfo		compareToMethodInfo;
		private static readonly MethodInfo		likeMethodInfo;
		private static readonly MethodInfo		escapedLikeMethodInfo;
		private static readonly MethodInfo		isInSetMethodInfo;
		private static readonly MethodInfo		isNotInSetMethodInfo;
	}
}
