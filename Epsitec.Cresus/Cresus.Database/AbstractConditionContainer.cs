using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Database
{


	public class AbstractConditionContainer
	{


		public AbstractConditionContainer()
		{
			this.conditions = new List<DbCondition> ();
			this.Combiner = DbCompareCombiner.And;
		}


		public bool IsEmpty
		{
			get
			{
				return this.conditions.Count == 0;
			}
		}


		/// <summary>
		/// Gets or sets the logical condition combiner.
		/// </summary>
		/// <value>The combiner.</value>
		public DbCompareCombiner Combiner
		{
			get;
			set;
		}


		/// <summary>
		/// Gets the columns used by the conditions.
		/// </summary>
		/// <value>The columns.</value>
		public IEnumerable<DbTableColumn> Columns
		{
			get
			{
				foreach (DbCondition condition in this.conditions)
				{
					foreach (DbTableColumn column in condition.Columns)
					{
						yield return column;
					}
				}
			}
		}


		/// <summary>
		/// Replaces the table columns used by the currently defined conditions.
		/// </summary>
		/// <param name="replaceOperation">The replace operation.</param>
		public void ReplaceTableColumns(System.Func<DbTableColumn, DbTableColumn> replaceOperation)
		{
			foreach (DbCondition condition in this.conditions)
			{
				condition.ReplaceTableColumns (replaceOperation);
			}
		}


		public void AddCondition(DbCondition condition)
		{
			this.conditions.Add (condition);
		}


		public void AddCondition(DbTableColumn a, DbCompare comparison, bool value)
		{
			this.conditions.Add (new DbCondition (a, comparison, value, DbRawType.Boolean));
		}


		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, short value)
		{
			this.conditions.Add (new DbCondition (a, comparison, value, DbRawType.Int16));
		}


		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, int value)
		{
			this.conditions.Add (new DbCondition (a, comparison, value, DbRawType.Int32));
		}


		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, long value)
		{
			this.conditions.Add (new DbCondition (a, comparison, value, DbRawType.Int64));
		}


		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		/// <param name="numDef">The numeric definition.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, decimal value, DbNumDef numDef)
		{
			DbRawType rawType  = TypeConverter.GetRawType (DbSimpleType.Decimal, numDef);
			object    rawValue = TypeConverter.ConvertFromSimpleType (value, DbSimpleType.Decimal, numDef);

			this.conditions.Add (new DbCondition (a, comparison, rawValue, rawType));
		}


		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, string value)
		{
			this.conditions.Add (new DbCondition (a, comparison, value, DbRawType.String));
		}


		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The first column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="b">The second column.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, DbTableColumn b)
		{
			this.conditions.Add (new DbCondition (a, comparison, b));
		}


		/// <summary>
		/// Adds the "is null" condition.
		/// </summary>
		/// <param name="a">The column.</param>
		public void AddConditionIsNull(DbTableColumn a)
		{
			this.conditions.Add (new DbCondition (a, DbCompare.IsNull));
		}


		/// <summary>
		/// Adds the "is not null" condition.
		/// </summary>
		/// <param name="a">The column.</param>
		public void AddConditionIsNotNull(DbTableColumn a)
		{
			this.conditions.Add (new DbCondition (a, DbCompare.IsNotNull));
		}


		/// <summary>
		/// Creates the conditions based on the previous <c>AddCondition</c>
		/// calls and using the expected revision.
		/// </summary>
		/// <param name="fields">The collection to which the conditions will be added.</param>
		protected SqlField CreateSqlField()
		{
			if (this.conditions.Count == 0)
			{
				throw new System.Exception ("No conditions in container.");
			}

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
					SqlFunctionCode op = this.ConvertToSqlFunctionType (this.Combiner);

					SqlFunction function = new SqlFunction (op, left, right);

					result = SqlField.CreateFunction (function);
				}

			}

			return result;
		}


		protected SqlFunctionCode ConvertToSqlFunctionType(DbCompareCombiner combiner)
		{
			switch (combiner)
			{
				case DbCompareCombiner.Or:
					return SqlFunctionCode.LogicOr;
				case DbCompareCombiner.And:
					return SqlFunctionCode.LogicAnd;
			}

			throw new System.ArgumentException ("Unsupported combiner: " + combiner);
		}
		

		private readonly List<DbCondition> conditions;
		

	}


}
