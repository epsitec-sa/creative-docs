using System.Collections.Generic;


namespace Epsitec.Cresus.Database
{


	public class DbSimpleCondition : DbAbstractCondition
	{


		private DbSimpleCondition(DbSimpleConditionOperator op, int argumentCount) : base ()
		{
			this.Operator = op;
			this.argumentCount = argumentCount;
		}


		public DbSimpleCondition(DbTableColumn column, DbSimpleConditionOperator op) : this (op, 1)
		{
			this.Left = column;
		}


		public DbSimpleCondition(DbTableColumn leftColumn, DbSimpleConditionOperator op, DbTableColumn rightColumn) : this (op, 2)
		{
			this.Left = leftColumn;
			this.RightColumn = rightColumn;
		}


		public DbSimpleCondition(DbTableColumn leftColumn, DbSimpleConditionOperator op, object rightConstantValue, DbRawType rightConstantRawType) : this (op, 2)
		{
			System.Diagnostics.Debug.Assert (leftColumn.Column.Type.RawType == rightConstantRawType);

			this.Left = leftColumn;
			this.RightConstantValue = rightConstantValue;
			this.RightConstantRawType = rightConstantRawType;
		}

		
		public DbTableColumn Left
		{
			get;
			private set;
		}

		
		public DbTableColumn RightColumn
		{
			get;
			private set;
		}

		
		public DbSimpleConditionOperator Operator
		{
			get;
			private set;
		}

		
		public object RightConstantValue
		{
			get;
			private set;
		}

		
		public DbRawType RightConstantRawType
		{
			get;
			private set;
		}


		internal override IEnumerable<DbTableColumn> Columns
		{
			get
			{
				if (this.Left != null)
				{
					yield return this.Left;
				}

				if (this.RightColumn != null)
				{
					yield return this.RightColumn;
				}
			}
		}


		internal override void ReplaceTableColumns(System.Func<DbTableColumn, DbTableColumn> replaceOperation)
		{
			if (this.Left != null)
			{
				this.Left = replaceOperation (this.Left);
			}

			if (this.RightColumn != null)
			{
				this.RightColumn = replaceOperation (this.RightColumn);
			}
		}


		internal override SqlField CreateSqlField()
		{
			return (this.argumentCount == 1) ? this.CreateSqlFieldWithSingleField () : this.CreateSqlFieldWithBothFields ();
		}


		private SqlField CreateSqlFieldWithSingleField()
		{
			SqlField field = this.CreateSqlField (this.Left);
			SqlFunctionCode op = this.ToSqlFunctionType (this.Operator);

			SqlFunction function = new SqlFunction (op, field);

			return SqlField.CreateFunction (function);
		}


		private SqlField CreateSqlFieldWithBothFields()
		{
			SqlField fieldA = this.CreateSqlField (this.Left);
			SqlField fieldB = (this.RightColumn == null) ? SqlField.CreateConstant (this.RightConstantValue, this.RightConstantRawType) : this.CreateSqlField (this.RightColumn);
			SqlFunctionCode op = this.ToSqlFunctionType (this.Operator);

			SqlFunction function = new SqlFunction (op, fieldA, fieldB);

			return SqlField.CreateFunction (function);
		}


		private SqlField CreateSqlField(DbTableColumn column)
		{
			return SqlField.CreateAliasedName (DbSqlStandard.MakeDelimitedIdentifier (column.TableAlias) ?? column.Table.GetSqlName (), column.Column.GetSqlName (), column.ColumnAlias);
		}


		private SqlFunctionCode ToSqlFunctionType(DbSimpleConditionOperator op)
		{
			switch (op)
			{
				case DbSimpleConditionOperator.Equal:
					return SqlFunctionCode.CompareEqual;
				case DbSimpleConditionOperator.NotEqual:
					return SqlFunctionCode.CompareNotEqual;
				case DbSimpleConditionOperator.LessThan:
					return SqlFunctionCode.CompareLessThan;
				case DbSimpleConditionOperator.LessThanOrEqual:
					return SqlFunctionCode.CompareLessThanOrEqual;
				case DbSimpleConditionOperator.GreaterThan:
					return SqlFunctionCode.CompareGreaterThan;
				case DbSimpleConditionOperator.GreaterThanOrEqual:
					return SqlFunctionCode.CompareGreaterThanOrEqual;
				case DbSimpleConditionOperator.Like:
					return SqlFunctionCode.CompareLike;
				case DbSimpleConditionOperator.LikeEscape:
					return SqlFunctionCode.CompareLikeEscape;
				case DbSimpleConditionOperator.NotLike:
					return SqlFunctionCode.CompareNotLike;
				case DbSimpleConditionOperator.NotLikeEscape:
					return SqlFunctionCode.CompareNotLikeEscape;
				case DbSimpleConditionOperator.IsNull:
					return SqlFunctionCode.CompareIsNull;
				case DbSimpleConditionOperator.IsNotNull:
					return SqlFunctionCode.CompareIsNotNull;
			}

			throw new System.ArgumentException ("Unsupported comparison: " + op);
		}


		public static DbSimpleCondition CreateCondition(DbTableColumn a, DbSimpleConditionOperator comparison, bool value)
		{
			return new DbSimpleCondition (a, comparison, value, DbRawType.Boolean);
		}


		public static DbSimpleCondition CreateCondition(DbTableColumn a, DbSimpleConditionOperator comparison, short value)
		{
			return new DbSimpleCondition (a, comparison, value, DbRawType.Int16);
		}


		public static DbSimpleCondition CreateCondition(DbTableColumn a, DbSimpleConditionOperator comparison, int value)
		{
			return new DbSimpleCondition (a, comparison, value, DbRawType.Int32);
		}


		public static DbSimpleCondition CreateCondition(DbTableColumn a, DbSimpleConditionOperator comparison, long value)
		{
			return new DbSimpleCondition (a, comparison, value, DbRawType.Int64);
		}


		public static DbSimpleCondition CreateCondition(DbTableColumn a, DbSimpleConditionOperator comparison, decimal value, DbNumDef numDef)
		{
			DbRawType rawType  = TypeConverter.GetRawType (DbSimpleType.Decimal, numDef);
			object    rawValue = TypeConverter.ConvertFromSimpleType (value, DbSimpleType.Decimal, numDef);

			return new DbSimpleCondition (a, comparison, rawValue, rawType);
		}


		public static DbSimpleCondition CreateCondition(DbTableColumn a, DbSimpleConditionOperator comparison, string value)
		{
			return new DbSimpleCondition (a, comparison, value, DbRawType.String);
		}


		private readonly int argumentCount;


	}


}
