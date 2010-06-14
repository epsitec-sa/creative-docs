using System.Collections.Generic;


namespace Epsitec.Cresus.Database
{


	public class DbCondition : DbAbstractCondition
	{


		private DbCondition(DbCompare op, int argumentCount) : base ()
		{
			this.Operator = op;
			this.argumentCount = argumentCount;
		}


		public DbCondition(DbTableColumn column, DbCompare op) : this (op, 1)
		{
			this.Left = column;
		}


		public DbCondition(DbTableColumn leftColumn, DbCompare op, DbTableColumn rightColumn) : this (op, 2)
		{
			this.Left = leftColumn;
			this.RightColumn = rightColumn;
		}


		public DbCondition(DbTableColumn leftColumn, DbCompare op, object rightConstantValue, DbRawType rightConstantRawType) : this (op, 2)
		{
			System.Diagnostics.Debug.Assert (leftColumn.Column.Type.RawType == rightConstantRawType);

			this.Left = leftColumn;
			this.RightConstantValue = rightConstantValue;
			this.RightConstantRawType = rightConstantRawType;
		}

		
		public DbTableColumn Left
		{
			get;
			set;
		}

		
		public DbTableColumn RightColumn
		{
			get;
			set;
		}

		
		public DbCompare Operator
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


		private SqlFunctionCode ToSqlFunctionType(DbCompare comparison)
		{
			switch (comparison)
			{
				case DbCompare.Equal:
					return SqlFunctionCode.CompareEqual;
				case DbCompare.NotEqual:
					return SqlFunctionCode.CompareNotEqual;
				case DbCompare.LessThan:
					return SqlFunctionCode.CompareLessThan;
				case DbCompare.LessThanOrEqual:
					return SqlFunctionCode.CompareLessThanOrEqual;
				case DbCompare.GreaterThan:
					return SqlFunctionCode.CompareGreaterThan;
				case DbCompare.GreaterThanOrEqual:
					return SqlFunctionCode.CompareGreaterThanOrEqual;
				case DbCompare.Like:
					return SqlFunctionCode.CompareLike;
				case DbCompare.LikeEscape:
					return SqlFunctionCode.CompareLikeEscape;
				case DbCompare.NotLike:
					return SqlFunctionCode.CompareNotLike;
				case DbCompare.NotLikeEscape:
					return SqlFunctionCode.CompareNotLikeEscape;
				case DbCompare.IsNull:
					return SqlFunctionCode.CompareIsNull;
				case DbCompare.IsNotNull:
					return SqlFunctionCode.CompareIsNotNull;
			}

			throw new System.ArgumentException ("Unsupported comparison: " + comparison);
		}


		private readonly int argumentCount;


	}


}
