using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;

using System.Text.RegularExpressions;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>ComparisonFieldValue</c> class represents a comparison between a
	/// <see cref="Field"/> and a <see cref="Constant"/>, such as (a = 3).
	/// </summary>
	public class ComparisonFieldValue : Comparison
	{


		/// <summary>
		/// Builds a new <c>ComparisonFieldValue</c>.
		/// </summary>
		/// <param name="left">The <see cref="Field"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <param name="op">The <see cref="BinaryComparator"/> used by the <c>ComparisonFieldValue</c>.</param>
		/// <param name="right">The <see cref="Constant"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="left"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="right"/> is null.</exception>
		public ComparisonFieldValue(Field left, BinaryComparator op, Constant right) : base ()
		{
			left.ThrowIfNull ("left");
			right.ThrowIfNull ("right");

			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		/// <summary>
		/// The left side of the <c>ComparisonFieldValue</c>.
		/// </summary>
		public Field Left
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="BinaryComparator"/> of the <c>ComparisonFieldValue</c>.
		/// </summary>
		public BinaryComparator Operator
		{
			get;
			private set;
		}


		/// <summary>
		/// The right side of the <c>ComparisonFieldValue</c>.
		/// </summary>
		public Constant Right
		{
			get;
			private set;
		}


		/// <summary>
		/// Escapes a string so that it can be used with the <see cref="BinaryComparator"/> dealing
		/// with escaped strings.
		/// </summary>
		/// <param name="value">The string to escape.</param>
		/// <returns>The escaped string.</returns>
		public static string Escape(string value)
		{
			string escapeChar = DbSqlStandard.CompareLikeEscape;
			
			string pattern = "([%_" + escapeChar + "])";
			string replacement = escapeChar + "$1";

			return Regex.Replace (value, pattern, replacement);
		}


		/// <summary>
		/// Gets the sequence of field ids that are used in this instance.
		/// </summary>
		/// <returns>The sequence of field ids that are used in this instance.</returns>
		internal override IEnumerable<Druid> GetFields()
		{
			return ExpressionFields.GetFields (this);
		}


		/// <summary>
		/// Converts this instance to an equivalent <see cref="DbAbstractCondition"/>, using a
		/// resolver to convert the <see cref="Druid"/> of the fields to the appropriate
		/// <see cref="DbTableColumn"/>.
		/// </summary>
		/// <param name="expressionConverter">The <see cref="ExpressionConverter"/> used to convert this instance.</param>
		/// <param name="columnResolver">The function used to resolve the <see cref="DbTableColumn"/> given an <see cref="Druid"/>.</param>
		/// <returns>The new <see cref="DbAbstractCondition"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="expressionConverter"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="columnResolver"/> is <c>null</c>.</exception>
		internal override DbAbstractCondition CreateDbCondition(ExpressionConverter expressionConverter, System.Func<Druid, DbTableColumn> columnResolver)
		{
			expressionConverter.ThrowIfNull ("expressionConverter");
			columnResolver.ThrowIfNull ("columnResolver");

			return expressionConverter.CreateDbCondition (this, columnResolver);
		}


	}


}
