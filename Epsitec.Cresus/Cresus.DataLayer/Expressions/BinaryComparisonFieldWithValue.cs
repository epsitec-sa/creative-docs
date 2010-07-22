using Epsitec.Cresus.Database;

using System.Text.RegularExpressions;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class BinaryComparisonFieldWithValue : BinaryComparison
	{


		public BinaryComparisonFieldWithValue(Field left, BinaryComparator op, Constant right)
			: base (left, op)
		{
			this.Right = right;
		}


		public Constant Right
		{
			get;
			private set;
		}


		public static string Escape(string value)
		{
			string escapeChar = DbSqlStandard.CompareLikeEscape;
			
			string pattern = "([%_" + escapeChar + "])";
			string replacement = escapeChar + "$1";

			return Regex.Replace (value, pattern, replacement);
		}


	}


}
