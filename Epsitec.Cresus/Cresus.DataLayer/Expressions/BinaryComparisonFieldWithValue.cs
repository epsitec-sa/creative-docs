using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System.Text.RegularExpressions;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>BinaryComparisonFieldWithValue</c> class represents a comparison between a
	/// <see cref="Field"/> and a <see cref="Constant"/>, such as (a = 3).
	/// </summary>
	public class BinaryComparisonFieldWithValue : Comparison
	{


		/// <summary>
		/// Builds a new <c>BinaryComparisonFieldWithValue</c>.
		/// </summary>
		/// <param name="left">The <see cref="Field"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <param name="op">The <see cref="BinaryComparator"/> used by the <c>BinaryComparisonFieldWithValue</c>.</param>
		/// <param name="left">The <see cref="Constant"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="left"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="right"/> is null.</exception>
		public BinaryComparisonFieldWithValue(Field left, BinaryComparator op, Constant right) : base ()
		{
			left.ThrowIfNull ("left");
			right.ThrowIfNull ("right");

			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		/// <summary>
		/// The left side of the <c>BinaryComparisonFieldWithValue</c>.
		/// </summary>
		public Field Left
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="BinaryComparator"/> of the <c>BinaryComparisonFieldWithValue</c>.
		/// </summary>
		public BinaryComparator Operator
		{
			get;
			private set;
		}


		/// <summary>
		/// The right side of the <c>BinaryComparisonFieldWithValue</c>.
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


	}


}
