//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Reflection;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>SqlMethods</c> class is inspired by what Microsoft implemented for LINQ to SQL.
	/// See http://msdn.microsoft.com/en-us/library/bb302445.
	/// </summary>
	public static class SqlMethods
	{


		public static MethodInfo CompareToMethodInfo
		{
			get
			{
				return SqlMethods.compareToMethodInfo;
			}
		}


		public static MethodInfo LikeMethodInfo
		{
			get
			{
				return SqlMethods.likeMethodInfo;
			}
		}


		public static MethodInfo EscapedLikeMethodInfo
		{
			get
			{
				return SqlMethods.escapedLikeMethodInfo;
			}
		}


		public static MethodInfo IsInValueSetMethodInfo
		{
			get
			{
				return SqlMethods.isInValueSetMethodInfo;
			}
		}


		public static MethodInfo IsNotInValueSetMethodInfo
		{
			get
			{
				return SqlMethods.isNotInValueSetMethodInfo;
			}
		}


		public static MethodInfo IsInSubquerySetMethodInfo
		{
			get
			{
				return SqlMethods.isInSubquerySetMethodInfo;
			}
		}


		public static MethodInfo IsNotInSubquerySetMethodInfo
		{
			get
			{
				return SqlMethods.isNotInSubquerySetMethodInfo;
			}
		}


		public static MethodInfo ConvertMethodInfo
		{
			get
			{
				return SqlMethods.convertMethodInfo;
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
			throw new NotSupportedException ();
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
			throw new NotSupportedException ();
		}


		public static bool IsInSet(object value, IEnumerable<object> set)
		{
			throw new NotImplementedException ();
		}


		public static bool IsNotInSet(object value, IEnumerable<object> set)
		{
			throw new NotImplementedException ();
		}


		public static bool IsInSet(AbstractEntity value, Request subQuery)
		{
			throw new NotImplementedException ();
		}


		public static bool IsNotInSet(AbstractEntity value, Request subQuery)
		{
			throw new NotImplementedException ();
		}


		/// <summary>
		/// This method is used to convert between values of two different type. SQL is not type
		/// safe so we can write a something like 'My string' = 42. However, The C# Expression Trees
		/// are type safe and we can't write that kind of stuff. We can't even make Expression Trees
		/// to compare between a FormattedText and a string, which is kind of weird. So this method
		/// lets you "convert" between the types. What happens is that this method call is simply
		/// discarded by the LambdaConverter so for it, it is as if it did not exist, but it allows
		/// to bypass the type safety checks in the Expression Trees.
		/// </summary>
		/// <typeparam name="TIn">The type that you want to convert from.</typeparam>
		/// <typeparam name="TOut">The type that you want to convert to.</typeparam>
		/// <param name="value">The value that you want to convert.</param>
		/// <exception cref="System.NotImplementedException">
		/// Do not call this method, it is a stub which will always throw an exception
		/// </exception>
		public static TOut Convert<TIn, TOut>(TIn value)
		{
			throw new NotImplementedException ();
		}


		static SqlMethods()
		{
			SqlMethods.compareToMethodInfo = typeof (SqlMethods).GetMethod ("CompareTo", new Type[] { typeof (string), typeof (string) });
			SqlMethods.likeMethodInfo = typeof (SqlMethods).GetMethod ("Like", new Type[] { typeof (string), typeof (string) });
			SqlMethods.escapedLikeMethodInfo = typeof (SqlMethods).GetMethod ("EscapedLike", new Type[] { typeof (string), typeof (string) });
			SqlMethods.isInValueSetMethodInfo = typeof (SqlMethods).GetMethod ("IsInSet", new Type[] { typeof (object), typeof (IEnumerable<object>) });
			SqlMethods.isNotInValueSetMethodInfo = typeof (SqlMethods).GetMethod ("IsNotInSet", new Type[] { typeof (object), typeof (IEnumerable<object>) });
			SqlMethods.isInSubquerySetMethodInfo = typeof (SqlMethods).GetMethod ("IsInSet", new Type[] { typeof (AbstractEntity), typeof (Request) });
			SqlMethods.isNotInSubquerySetMethodInfo = typeof (SqlMethods).GetMethod ("IsNotInSet", new Type[] { typeof (AbstractEntity), typeof (Request) });
			SqlMethods.convertMethodInfo = typeof (SqlMethods).GetMethod ("Convert");
		}


		private static readonly MethodInfo compareToMethodInfo;
		private static readonly MethodInfo likeMethodInfo;
		private static readonly MethodInfo escapedLikeMethodInfo;
		private static readonly MethodInfo isInValueSetMethodInfo;
		private static readonly MethodInfo isNotInValueSetMethodInfo;
		private static readonly MethodInfo isInSubquerySetMethodInfo;
		private static readonly MethodInfo isNotInSubquerySetMethodInfo;
		private static readonly MethodInfo convertMethodInfo;


	}


}
