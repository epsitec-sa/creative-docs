//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
			this.sqlFields     = new Collections.SqlFields ();
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
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbColumn a, DbCompare comparison, short value)
		{
			this.AddConditionWithRawValue (a, comparison, value, DbRawType.Int16);
		}

		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbColumn a, DbCompare comparison, int value)
		{
			this.AddConditionWithRawValue (a, comparison, value, DbRawType.Int32);
		}

		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbColumn a, DbCompare comparison, long value)
		{
			this.AddConditionWithRawValue (a, comparison, value, DbRawType.Int64);
		}

		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		/// <param name="numDef">The numeric definition.</param>
		public void AddCondition(DbColumn a, DbCompare comparison, decimal value, DbNumDef numDef)
		{
			DbRawType rawType  = TypeConverter.GetRawType (DbSimpleType.Decimal, numDef);
			object    rawValue = TypeConverter.ConvertFromSimpleType (value, DbSimpleType.Decimal, numDef);
			
			this.AddConditionWithRawValue (a, comparison, rawValue, rawType);
		}

		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="value">The value.</param>
		public void AddCondition(DbColumn a, DbCompare comparison, string value)
		{
			this.AddConditionWithRawValue (a, comparison, value, DbRawType.String);
		}

		/// <summary>
		/// Adds a condition.
		/// </summary>
		/// <param name="a">The first column.</param>
		/// <param name="comparison">The comparison.</param>
		/// <param name="b">The second column.</param>
		public void AddCondition(DbColumn a, DbCompare comparison, DbColumn b)
		{
			SqlField    fieldA   = SqlField.CreateName (a);
			SqlField    fieldB   = SqlField.CreateName (b);
			SqlFunction function = new SqlFunction (this.MapDbCompareToSqlFunctionType (comparison), fieldA, fieldB);
			
			this.sqlFields.Add (SqlField.CreateFunction (function));
		}

		/// <summary>
		/// Adds the "is null" condition.
		/// </summary>
		/// <param name="a">The column.</param>
		public void AddConditionIsNull(DbColumn a)
		{
			SqlField    field    = SqlField.CreateName (a);
			SqlFunction function = new SqlFunction (SqlFunctionCode.CompareIsNull, field);
			
			this.sqlFields.Add (SqlField.CreateFunction (function));
		}

		/// <summary>
		/// Adds the "is not null" condition.
		/// </summary>
		/// <param name="a">The column.</param>
		public void AddConditionIsNotNull(DbColumn a)
		{
			SqlField    field    = SqlField.CreateName (a);
			SqlFunction function = new SqlFunction (SqlFunctionCode.CompareIsNotNull, field);
			
			this.sqlFields.Add (SqlField.CreateFunction (function));
		}


		/// <summary>
		/// Creates the conditions based on the previous <c>AddCondition</c>
		/// calls and using the expected revision.
		/// </summary>
		/// <param name="mainTable">The main table.</param>
		/// <param name="fields">The collection to which the conditions will be added.</param>
		internal void CreateConditions(DbTable mainTable, Collections.SqlFields fields)
		{
			switch (this.revision)
			{
				case DbSelectRevision.LiveAll:
					
					//	Select all live revisions of the rows: live (0), copied (1)
					//	and archive copy (2) are all < deleted (3).
					
					this.AddCondition (mainTable.Columns[Tags.ColumnStatus],
						/**/		   DbCompare.LessThan,
						/**/		   DbKey.ConvertToIntStatus (DbRowStatus.Deleted));
					
					break;
				
				case DbSelectRevision.LiveActive:
					
					//	Select only the active revisions of the rows: live (0) and
					//	copied (1) both describe active rows and are < archive copy (2).
					
					this.AddCondition (mainTable.Columns[Tags.ColumnStatus],
						/**/		   DbCompare.LessThan,
						/**/		   DbKey.ConvertToIntStatus (DbRowStatus.ArchiveCopy));
					
					break;
				
				case DbSelectRevision.All:

					//	Select all revisions of the rows; there is no need to add an
					//	additional condition for this !
					
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("DbSelectRevision.{0} not supported", this.revision));
			}
			
			switch (this.combiner)
			{
				case DbCompareCombiner.And:
					fields.AddRange (this.sqlFields);
					break;
				
				case DbCompareCombiner.Or:
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
		
		private void AddConditionWithRawValue(DbColumn a, DbCompare comparison, object rawValue, DbRawType rawType)
		{
			SqlField fieldA = SqlField.CreateName (a);
			SqlField fieldB = SqlField.CreateConstant (rawValue, rawType);
			
			SqlFunction function = new SqlFunction (this.MapDbCompareToSqlFunctionType (comparison), fieldA, fieldB);
			
			this.sqlFields.Add (SqlField.CreateFunction (function));
		}
		
		private SqlFunctionCode MapDbCompareToSqlFunctionType(DbCompare comparison)
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
				case DbCompare.NotLike:				return SqlFunctionCode.CompareNotLike;
			}
			
			throw new System.ArgumentException (string.Format ("Unsupported comparison {0}", comparison), "comparison");
		}

		private ITypeConverter					typeConverter;
		private Collections.SqlFields			sqlFields;
		private DbSelectRevision				revision;
		private DbCompareCombiner				combiner = DbCompareCombiner.And;
	}
}
