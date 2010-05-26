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

		
		public void Register(Collections.SqlFieldList fields)
		{
			if (this.argumentCount == 1)
			{
				SqlField field = SqlField.CreateAliasedName (DbSqlStandard.MakeDelimitedIdentifier (this.ColumnA.TableAlias) ?? this.ColumnA.Table.GetSqlName (), this.ColumnA.Column.GetSqlName (), this.ColumnA.ColumnAlias);

				SqlFunction function = new SqlFunction (DbCondition.MapDbCompareToSqlFunctionType (this.Comparison), field);

				fields.Add (SqlField.CreateFunction (function));
			}
			else if (this.ColumnB == null)
			{
				SqlField fieldA = SqlField.CreateAliasedName (DbSqlStandard.MakeDelimitedIdentifier (this.ColumnA.TableAlias) ?? this.ColumnA.Table.GetSqlName (), this.ColumnA.Column.GetSqlName (), this.ColumnA.ColumnAlias);
				SqlField fieldB = SqlField.CreateConstant (this.ConstantValue, this.ConstantValueRawType);

				SqlFunction function = new SqlFunction (DbCondition.MapDbCompareToSqlFunctionType (this.Comparison), fieldA, fieldB);

				fields.Add (SqlField.CreateFunction (function));
			}
			else
			{
				SqlField fieldA = SqlField.CreateAliasedName (DbSqlStandard.MakeDelimitedIdentifier (this.ColumnA.TableAlias) ?? this.ColumnA.Table.GetSqlName (), this.ColumnA.Column.GetSqlName (), this.ColumnA.ColumnAlias);
				SqlField fieldB = SqlField.CreateAliasedName (DbSqlStandard.MakeDelimitedIdentifier (this.ColumnB.TableAlias) ?? this.ColumnB.Table.GetSqlName (), this.ColumnB.Column.GetSqlName (), this.ColumnB.ColumnAlias);

				SqlFunction function = new SqlFunction (DbCondition.MapDbCompareToSqlFunctionType (this.Comparison), fieldA, fieldB);

				fields.Add (SqlField.CreateFunction (function));
			}
		}


		private static SqlFunctionCode MapDbCompareToSqlFunctionType(DbCompare comparison)
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

			throw new System.ArgumentException (string.Format ("Unsupported comparison {0}", comparison), "comparison");
		}


		private readonly int argumentCount;


	}


}
