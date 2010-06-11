using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;


namespace Epsitec.Cresus.Database
{


	public class DbCondition
	{


		public DbCondition(DbTableColumn column, DbCompare condition)
		{
			this.argumentCount = 1;
			this.ColumnA = column;
			this.Comparison = condition;
		}

		
		public DbCondition(DbTableColumn columnA, DbCompare comparison, DbTableColumn columnB)
		{
			this.argumentCount = 2;
			this.ColumnA = columnA;
			this.ColumnB = columnB;
			this.Comparison = comparison;
		}

		
		public DbCondition(DbTableColumn column, DbCompare comparison, object value, DbRawType valueRawType)
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


		public IEnumerable<DbTableColumn> Columns
		{
			get
			{
				if (this.ColumnA != null)
				{
					yield return this.ColumnA;
				}

				if (this.ColumnB != null)
				{
					yield return this.ColumnB;
				}
			}
		}


		public void ReplaceTableColumns(System.Func<DbTableColumn, DbTableColumn> replaceOperation)
		{
			if (this.ColumnA != null)
			{
				this.ColumnA = replaceOperation (this.ColumnA);
			}

			if (this.ColumnB != null)
			{
				this.ColumnB = replaceOperation (this.ColumnB);
			}
		}


		public SqlField CreateSqlField()
		{
			return (this.argumentCount == 1) ? this.CreateSqlFieldWithSingleField () : this.CreateSqlFieldWithBothFields ();
		}


		private SqlField CreateSqlFieldWithSingleField()
		{
			SqlField field = this.CreateSqlField (this.ColumnA);
			SqlFunctionCode op = this.ToSqlFunctionType (this.Comparison);

			SqlFunction function = new SqlFunction (op, field);

			return SqlField.CreateFunction (function);
		}


		private SqlField CreateSqlFieldWithBothFields()
		{
			SqlField fieldA = this.CreateSqlField (this.ColumnA);
			SqlField fieldB = (this.ColumnB == null) ? SqlField.CreateConstant (this.ConstantValue, this.ConstantValueRawType) : this.CreateSqlField (this.ColumnB);
			SqlFunctionCode op = this.ToSqlFunctionType (this.Comparison);

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

			throw new System.ArgumentException ("Unsupported comparison " + comparison);
		}


		private readonly int argumentCount;


	}


}
