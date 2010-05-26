using System.Collections.Generic;


namespace Epsitec.Cresus.Database
{


	public class AbstractConditionContainer
	{


		public AbstractConditionContainer()
		{
			this.conditions = new List<DbCondition> ();
			this.Combiner = DbCompareCombiner.And;
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
		public IEnumerable<DbTableColumn>		Columns
		{
			get
			{
				foreach (DbCondition condition in this.conditions)
				{
					if (condition.ColumnA != null)
					{
						yield return condition.ColumnA;
					}

					if (condition.ColumnB != null)
					{
						yield return condition.ColumnB;
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
				if (condition.ColumnA != null)
				{
					condition.ColumnA = replaceOperation (condition.ColumnA);
				}
				if (condition.ColumnB != null)
				{
					condition.ColumnB = replaceOperation (condition.ColumnB);
				}
			}
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
		internal void CreateConditions(Collections.SqlFieldList fields)
		{
			this.CreateConditions (null, null, fields);
		}


		/// <summary>
		/// Creates the conditions based on the previous <c>AddCondition</c>
		/// calls and using the expected revision.
		/// </summary>
		/// <param name="mainTable">The main table.</param>
		/// <param name="fields">The collection to which the conditions will be added.</param>
		internal void CreateConditions(DbTable mainTable, Collections.SqlFieldList fields)
		{
			this.CreateConditions (mainTable, null, fields);
		}


		internal void CreateConditions(DbTable mainTable, string mainTableAlias, Collections.SqlFieldList fields)
		{
			Collections.SqlFieldList sqlFields = new Collections.SqlFieldList ();

			foreach (DbCondition condition in this.conditions)
			{
				condition.Register (sqlFields);
			}

			switch (this.Combiner)
			{
				case DbCompareCombiner.And:
					fields.AddRange (sqlFields);
					break;
				
				case DbCompareCombiner.Or:
					if (sqlFields.Count > 0)
					{
						SqlField or = sqlFields.Merge (SqlFunctionCode.LogicOr);
						fields.Add (or);
					}
					break;
				
				default:
					throw new System.InvalidOperationException (string.Format ("Cannot create conditions with {0} combiner", this.Combiner));
			}
		}
		

		private readonly List<DbCondition> conditions;
		

	}


}
