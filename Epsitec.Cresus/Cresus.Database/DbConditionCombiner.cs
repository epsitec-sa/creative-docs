using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Database
{


	public class DbConditionCombiner : DbAbstractCondition
	{


		public DbConditionCombiner() : base()
		{
			this.conditions = new List<DbAbstractCondition> ();
			this.Combiner = DbConditionCombiner.DefaultCombinerOperator;
		}


		public bool IsEmpty
		{
			get
			{
				return !this.conditions.Any ();
			}
		}


		/// <summary>
		/// Gets or sets the logical condition combiner.
		/// </summary>
		/// <value>The combiner.</value>
		public DbConditionCombinerOperator Combiner
		{
			get;
			set;
		}


		/// <summary>
		/// Gets the columns used by the conditions.
		/// </summary>
		/// <value>The columns.</value>
		internal override IEnumerable<DbTableColumn> Columns
		{
			get
			{
				foreach (DbSimpleCondition condition in this.conditions)
				{
					foreach (DbTableColumn column in condition.Columns)
					{
						yield return column;
					}
				}
			}
		}


		public void AddCondition(DbAbstractCondition condition)
		{
			this.conditions.Add (condition);
		}


		/// <summary>
		/// Replaces the table columns used by the currently defined conditions.
		/// </summary>
		/// <param name="replaceOperation">The replace operation.</param>
		internal override void ReplaceTableColumns(System.Func<DbTableColumn, DbTableColumn> replaceOperation)
		{
			foreach (DbAbstractCondition condition in this.conditions)
			{
				condition.ReplaceTableColumns (replaceOperation);
			}
		}


		/// <summary>
		/// Creates the conditions based on the previous <c>AddCondition</c>
		/// calls and using the expected revision.
		/// </summary>
		/// <param name="fields">The collection to which the conditions will be added.</param>
		internal override SqlField CreateSqlField()
		{
			SqlField result = null;

			foreach (SqlField field in this.conditions.Select (c => c.CreateSqlField ()))
			{
				if (result == null)
				{
					result = field;
				}
				else
				{
					SqlField left = result;
					SqlField right = field;
					SqlFunctionCode op = this.ToSqlFunctionType (this.Combiner);

					SqlFunction function = new SqlFunction (op, left, right);

					result = SqlField.CreateFunction (function);
				}

			}

			return result;
		}


		private SqlFunctionCode ToSqlFunctionType(DbConditionCombinerOperator op)
		{
			switch (op)
			{
				case DbConditionCombinerOperator.Or:
					return SqlFunctionCode.LogicOr;
				case DbConditionCombinerOperator.And:
					return SqlFunctionCode.LogicAnd;
			}

			throw new System.ArgumentException ("Unsupported combiner: " + op);
		}


		public static DbConditionCombiner Combine(params DbAbstractCondition[] conditions)
		{
			return DbConditionCombiner.Combine (DbConditionCombiner.DefaultCombinerOperator, conditions);
		}


		public static DbConditionCombiner Combine(DbConditionCombinerOperator op, params DbAbstractCondition[] conditions)
		{
			DbConditionCombiner combiner = new DbConditionCombiner ()
			{
				Combiner = op,
			};
			
			foreach (DbAbstractCondition condition in conditions.Where (c => c != null))
			{
				combiner.AddCondition (condition);
			}

			return combiner;
		}


		public readonly static DbConditionCombinerOperator DefaultCombinerOperator = DbConditionCombinerOperator.And;


		private readonly List<DbAbstractCondition> conditions;
		

	}


}
