using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;
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


		protected override DbSimpleCondition CreateDbSimpleCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbTableColumn left = this.Left.CreateDbTableColumn (entity, dbTableColumnResolver);
			DbSimpleConditionOperator op = OperatorConverter.ToDbSimpleConditionOperator (this.Operator);
			object rightValue = this.Right.Value;
			DbRawType rightType = OperatorConverter.ToDbRawType (this.Right.Type);

			return new DbSimpleCondition (left, op, rightValue, rightType);
		}


		internal override IEnumerable<Druid> GetFields()
		{
			return new Druid[] { this.Left.FieldId, };
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
