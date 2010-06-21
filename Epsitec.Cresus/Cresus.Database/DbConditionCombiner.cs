using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Database
{


	public class DbConditionCombiner : DbAbstractCondition
	{


		public DbConditionCombiner() : this (DbConditionCombiner.DefaultCombinerOperator)
		{
		}


		public DbConditionCombiner(DbConditionCombinerOperator op) : this (op, new DbAbstractCondition[0])
		{
		}


		public DbConditionCombiner(params DbAbstractCondition[] conditions)	: this (DbConditionCombiner.DefaultCombinerOperator, conditions)
		{
		}


		public DbConditionCombiner(DbConditionCombinerOperator op, params DbAbstractCondition[] conditions) : base()
		{
			this.Combiner = op;

			this.conditions = conditions.Where (c => c != null).ToList ();
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
			if (condition != null)
			{
				this.conditions.Add (condition);
			}
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
		/// <returns>The <see cref="SqlField"/> or null.</returns>
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


		public readonly static DbConditionCombinerOperator DefaultCombinerOperator = DbConditionCombinerOperator.And;


		private readonly List<DbAbstractCondition> conditions;
		

	}


}
