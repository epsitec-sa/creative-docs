using Epsitec.Cresus.Database;

using System.Text.RegularExpressions;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class BinaryComparisonFieldWithValue : Comparison
	{


		public BinaryComparisonFieldWithValue(Field left, BinaryComparator op, Constant right) : base ()
		{
			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		public Field Left
		{
			get;
			private set;
		}


		public BinaryComparator Operator
		{
			get;
			private set;
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
