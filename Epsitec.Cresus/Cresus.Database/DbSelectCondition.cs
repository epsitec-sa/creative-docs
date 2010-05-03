//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbSelectCondition</c> class represents a condition for the
	/// WHERE clause.
	/// </summary>
	public sealed class DbSelectCondition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbSelectCondition"/> class.
		/// </summary>
		/// <param name="typeConverter">The type converter.</param>
		public DbSelectCondition(ITypeConverter typeConverter)
			: this (typeConverter, DbSelectRevision.All)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbSelectCondition"/> class.
		/// </summary>
		/// <param name="typeConverter">The type converter.</param>
		/// <param name="revision">The expected revision.</param>
		public DbSelectCondition(ITypeConverter typeConverter, DbSelectRevision revision)
		{
			this.typeConverter = typeConverter;
			this.sqlFields     = new Collections.SqlFieldList ();
			this.conditions    = new List<Condition> ();
			this.revision      = revision;
		}


		/// <summary>
		/// Gets or sets the expected revision.
		/// </summary>
		/// <value>The revision.</value>
		public DbSelectRevision					Revision
		{
			get
			{
				return this.revision;
			}
			set
			{
				this.revision = value;
			}
		}

		/// <summary>
		/// Gets or sets the logical condition combiner.
		/// </summary>
		/// <value>The combiner.</value>
		public DbCompareCombiner				Combiner
		{
			get
			{
				return this.combiner;
			}
			set
			{
				this.combiner = value;
			}
		}

		/// <summary>
		/// Gets the columns used by the conditions.
		/// </summary>
		/// <value>The columns.</value>
		public IEnumerable<DbTableColumn>		Columns
		{
			get
			{
				foreach (Condition condition in this.conditions)
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
			foreach (Condition condition in this.conditions)
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
			this.conditions.Add (new Condition (a, comparison, value, DbRawType.Boolean));
		}

		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, short value)
		{
			this.conditions.Add (new Condition (a, comparison, value, DbRawType.Int16));
		}

		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, int value)
		{
			this.conditions.Add (new Condition (a, comparison, value, DbRawType.Int32));
		}

		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, long value)
		{
			this.conditions.Add (new Condition (a, comparison, value, DbRawType.Int64));
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

			this.conditions.Add (new Condition (a, comparison, rawValue, rawType));
		}

		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, string value)
		{
			this.conditions.Add (new Condition (a, comparison, value, DbRawType.String));
		}

		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The first column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="b">The second column.</param>
		public void AddCondition(DbTableColumn a, DbCompare comparison, DbTableColumn b)
		{
			this.conditions.Add (new Condition (a, comparison, b));
		}

		/// <summary>
		/// Adds the "is null" condition.
		/// </summary>
		/// <param name="a">The column.</param>
		public void AddConditionIsNull(DbTableColumn a)
		{
			this.conditions.Add (new Condition (a, DbCompare.IsNull));
		}

		/// <summary>
		/// Adds the "is not null" condition.
		/// </summary>
		/// <param name="a">The column.</param>
		public void AddConditionIsNotNull(DbTableColumn a)
		{
			this.conditions.Add (new Condition (a, DbCompare.IsNotNull));
		}


		#region Condition Class

		private class Condition
		{
			public Condition(DbTableColumn column, DbCompare condition)
			{
				this.argumentCount = 1;
				this.ColumnA = column;
				this.Comparison = condition;
			}

			public Condition(DbTableColumn columnA, DbCompare comparison, DbTableColumn columnB)
			{
				this.argumentCount = 2;
				this.ColumnA = columnA;
				this.ColumnB = columnB;
				this.Comparison = comparison;
			}

			public Condition(DbTableColumn column, DbCompare comparison, object value, DbRawType valueRawType)
			{
				System.Diagnostics.Debug.Assert (column.Column.Type.RawType == valueRawType);

				this.argumentCount = 2;
				this.ColumnA = column;
				this.Comparison = comparison;
				this.ConstantValue = value;
				this.ConstantValueRawType = valueRawType;
			}

			public DbTableColumn ColumnA
			{
				get;
				set;
			}
			
			public DbTableColumn ColumnB
			{
				get;
				set;
			}

			public DbCompare Comparison
			{
				get;
				private set;
			}

			public object ConstantValue
			{
				get;
				private set;
			}

			public DbRawType ConstantValueRawType
			{
				get;
				private set;
			}

			public void Register(Collections.SqlFieldList fields)
			{
				if (this.argumentCount == 1)
				{
					SqlField field = SqlField.CreateAliasedName (DbSqlStandard.MakeDelimitedIdentifier (this.ColumnA.TableAlias) ?? this.ColumnA.Table.GetSqlName (), this.ColumnA.Column.GetSqlName (), this.ColumnA.ColumnAlias);
					
					SqlFunction function = new SqlFunction (DbSelectCondition.MapDbCompareToSqlFunctionType (this.Comparison), field);

					fields.Add (SqlField.CreateFunction (function));
				}
				else if (this.ColumnB == null)
				{
					SqlField fieldA = SqlField.CreateAliasedName (DbSqlStandard.MakeDelimitedIdentifier (this.ColumnA.TableAlias) ?? this.ColumnA.Table.GetSqlName (), this.ColumnA.Column.GetSqlName (), this.ColumnA.ColumnAlias);
					SqlField fieldB = SqlField.CreateConstant (this.ConstantValue, this.ConstantValueRawType);

					SqlFunction function = new SqlFunction (DbSelectCondition.MapDbCompareToSqlFunctionType (this.Comparison), fieldA, fieldB);

					fields.Add (SqlField.CreateFunction (function));
				}
				else
				{
					SqlField fieldA = SqlField.CreateAliasedName (DbSqlStandard.MakeDelimitedIdentifier (this.ColumnA.TableAlias) ?? this.ColumnA.Table.GetSqlName (), this.ColumnA.Column.GetSqlName (), this.ColumnA.ColumnAlias);
					SqlField fieldB = SqlField.CreateAliasedName (DbSqlStandard.MakeDelimitedIdentifier (this.ColumnB.TableAlias) ?? this.ColumnB.Table.GetSqlName (), this.ColumnB.Column.GetSqlName (), this.ColumnB.ColumnAlias);
					
					SqlFunction function = new SqlFunction (DbSelectCondition.MapDbCompareToSqlFunctionType (this.Comparison), fieldA, fieldB);

					fields.Add (SqlField.CreateFunction (function));
				}
			}

			private readonly int argumentCount;
		}

		#endregion


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
			this.sqlFields.Clear ();

			foreach (Condition condition in this.conditions)
			{
				condition.Register (this.sqlFields);
			}

			Condition revisionCondition = null;

			if (mainTable != null)
			{
				switch (this.revision)
				{
					case DbSelectRevision.LiveAll:
						//	Select all live revisions of the rows: live (0), copied (1)
						//	and archive copy (2) are all < deleted (3).

						revisionCondition = new Condition (new DbTableColumn (mainTableAlias, mainTable.Columns[Tags.ColumnStatus]),
							/* */						   DbCompare.LessThan,
							/* */						   DbKey.ConvertToIntStatus (DbRowStatus.Deleted), DbRawType.Int16);
						break;

					case DbSelectRevision.LiveActive:
						//	Select only the active revisions of the rows: live (0) and
						//	copied (1) both describe active rows and are < archive copy (2).

						revisionCondition = new Condition (new DbTableColumn (mainTableAlias, mainTable.Columns[Tags.ColumnStatus]),
							/* */						   DbCompare.LessThan,
							/* */						   DbKey.ConvertToIntStatus (DbRowStatus.ArchiveCopy), DbRawType.Int16);
						break;

					case DbSelectRevision.All:
						//	Select all revisions of the rows; there is no need to add an
						//	additional condition for this !

						break;

					default:
						throw new System.NotSupportedException (string.Format ("DbSelectRevision.{0} not supported", this.revision));
				}
			}
			
			switch (this.combiner)
			{
				case DbCompareCombiner.And:
					if (revisionCondition != null)
					{
						revisionCondition.Register (fields);
					}
					fields.AddRange (this.sqlFields);
					break;
				
				case DbCompareCombiner.Or:
					if (revisionCondition != null)
					{
						revisionCondition.Register (fields);
					}
					if (this.sqlFields.Count > 0)
					{
						SqlField or = this.sqlFields.Merge (SqlFunctionCode.LogicOr);
						fields.Add (or);
					}
					break;
				
				default:
					throw new System.InvalidOperationException (string.Format ("Cannot create conditions with {0} combiner", this.combiner));
			}
		}
		
		private static SqlFunctionCode MapDbCompareToSqlFunctionType(DbCompare comparison)
		{
			switch (comparison)
			{
				case DbCompare.Equal:				return SqlFunctionCode.CompareEqual;
				case DbCompare.NotEqual:			return SqlFunctionCode.CompareNotEqual;
				case DbCompare.LessThan:			return SqlFunctionCode.CompareLessThan;
				case DbCompare.LessThanOrEqual:		return SqlFunctionCode.CompareLessThanOrEqual;
				case DbCompare.GreaterThan:			return SqlFunctionCode.CompareGreaterThan;
				case DbCompare.GreaterThanOrEqual:	return SqlFunctionCode.CompareGreaterThanOrEqual;
				case DbCompare.Like:				return SqlFunctionCode.CompareLike;
				case DbCompare.LikeEscape:			return SqlFunctionCode.CompareLikeEscape;
				case DbCompare.NotLike:				return SqlFunctionCode.CompareNotLike;
				case DbCompare.NotLikeEscape:		return SqlFunctionCode.CompareNotLikeEscape;
				case DbCompare.IsNull:				return SqlFunctionCode.CompareIsNull;
				case DbCompare.IsNotNull:			return SqlFunctionCode.CompareIsNotNull;
			}
			
			throw new System.ArgumentException (string.Format ("Unsupported comparison {0}", comparison), "comparison");
		}

		private readonly ITypeConverter				typeConverter;
		private readonly Collections.SqlFieldList	sqlFields;
		private readonly List<Condition>			conditions;
		private DbSelectRevision					revision;
		private DbCompareCombiner					combiner = DbCompareCombiner.And;
	}
}
