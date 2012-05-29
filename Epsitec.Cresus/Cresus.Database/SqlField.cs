//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlField</c> class represents a field in an SQL query. This can
	/// be a qualified column name, a constant, a function, a subquery, etc. The
	/// representation is independent from the SQL dialect.
	/// </summary>
	public sealed class SqlField
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlField"/> class.
		/// </summary>
		public SqlField()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlField"/> class.
		/// </summary>
		/// <param name="fieldType">Type of the field.</param>
		private SqlField(SqlFieldType fieldType)
		{
			this.fieldType = fieldType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlField"/> class.
		/// </summary>
		/// <param name="fieldType">Type of the field.</param>
		/// <param name="value">The raw value.</param>
		/// <param name="rawType">The raw type of the value.</param>
		private SqlField(SqlFieldType fieldType, object value, DbRawType rawType)
		{
			this.fieldType = fieldType;
			this.value     = value;
			this.rawType   = rawType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlField"/> class.
		/// </summary>
		/// <param name="name">The field name or qualified field name.</param>
		public SqlField(string name)
		{
			if (DbSqlStandard.ValidateQualifiedName (name))
			{
				this.fieldType  = SqlFieldType.QualifiedName;
				this.value = name;
			}
			else
			{
				this.fieldType  = SqlFieldType.Name;
				this.value = name;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlField"/> class.
		/// </summary>
		/// <param name="name">The field name or qualified field name.</param>
		/// <param name="alias">The alias.</param>
		public SqlField(string name, string alias)
			: this (name)
		{
			this.alias = alias;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlField"/> class.
		/// </summary>
		/// <param name="name">The field name or qualified field name.</param>
		/// <param name="alias">The alias.</param>
		/// <param name="order">The SQL sort order.</param>
		public SqlField(string name, string alias, SqlSortOrder order)
			: this (name, alias)
		{
			this.sortOrder = order;
		}


		/// <summary>
		/// Gets the field type.
		/// </summary>
		/// <value>The field type.</value>
		public SqlFieldType						FieldType
		{
			get
			{
				return this.fieldType;
			}
		}

		/// <summary>
		/// Gets the raw type for the field.
		/// </summary>
		/// <value>The raw type for the field.</value>
		public DbRawType						RawType
		{
			get
			{
				return this.rawType;
			}
		}

		/// <summary>
		/// Gets or sets the sort order.
		/// </summary>
		/// <value>The sort order.</value>
		public SqlSortOrder						SortOrder
		{
			get
			{
				return this.sortOrder;
			}
			set
			{
				this.sortOrder = value;
			}
		}

		/// <summary>
		/// Gets or sets the alias.
		/// </summary>
		/// <value>The alias.</value>
		public string							Alias
		{
			get
			{
				return this.alias;
			}
			set
			{
				this.alias = value;
			}
		}

		/// <summary>
		/// Gets the field value.
		/// </summary>
		/// <value>The field value.</value>
		public object							Value
		{
			get
			{
				return this.value;
			}
		}

		/// <summary>
		/// Gets the field value as a constant.
		/// </summary>
		/// <value>The field value as a constant.</value>
		public object							AsConstant
		{
			get
			{
				if (this.fieldType == SqlFieldType.Constant)
				{
					return this.value;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as a parameter.
		/// </summary>
		/// <value>The field value as a parameter.</value>
		public object							AsParameter
		{
			get
			{
				if ((this.fieldType == SqlFieldType.ParameterIn) ||
					(this.fieldType == SqlFieldType.ParameterInOut) ||
					(this.fieldType == SqlFieldType.ParameterOut) ||
					(this.fieldType == SqlFieldType.ParameterResult))
				{
					return this.value;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as a name.
		/// </summary>
		/// <value>The field value as a name.</value>
		public string							AsName
		{
			get
			{
				if (this.fieldType == SqlFieldType.Name)
				{
					return this.value as string;
				}
				else if (this.fieldType == SqlFieldType.QualifiedName)
				{
					string qualifier;
					string name;

					DbSqlStandard.SplitQualifiedName (this.AsQualifiedName, out qualifier, out name);

					return name;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as a qualified name.
		/// </summary>
		/// <value>The field value as a qualified name.</value>
		public string							AsQualifiedName
		{
			get
			{
				if (this.fieldType == SqlFieldType.QualifiedName)
				{
					return this.value as string;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as a qualifier.
		/// </summary>
		/// <value>The field value as a qualifier.</value>
		public string							AsQualifier
		{
			get
			{
				if (this.fieldType == SqlFieldType.Name)
				{
					throw new Exceptions.FormatException (string.Format ("{0} is not a qualified name", this.AsName));
				}
				
				if (this.fieldType == SqlFieldType.QualifiedName)
				{
					string qualifier;
					string name;

					DbSqlStandard.SplitQualifiedName (this.AsQualifiedName, out qualifier, out name);

					return qualifier;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as an aggregate.
		/// </summary>
		/// <value>The field value as an aggregate.</value>
		public SqlAggregate						AsAggregate
		{
			get
			{
				if (this.fieldType == SqlFieldType.Aggregate)
				{
					return (SqlAggregate) this.value;
				}
				else
				{
					return SqlAggregate.Empty;
				}
			}
		}

		/// <summary>
		/// Gets the field value as a variable.
		/// </summary>
		/// <value>The field value as a variable.</value>
		public object							AsVariable
		{
			get
			{
				if (this.fieldType == SqlFieldType.Variable)
				{
					return this.value;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as a function.
		/// </summary>
		/// <value>The field value as a function.</value>
		public SqlFunction						AsFunction
		{
			get
			{
				if (this.fieldType == SqlFieldType.Function)
				{
					return this.value as SqlFunction;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as a join.
		/// </summary>
		/// <value>The field value as a join.</value>
		public SqlJoin							AsJoin
		{
			get
			{
				if (this.fieldType == SqlFieldType.Join)
				{
					return this.value as SqlJoin;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as a procedure.
		/// </summary>
		/// <value>The field value as a procedure.</value>
		public string							AsProcedure
		{
			get
			{
				if (this.fieldType == SqlFieldType.Procedure)
				{
					return this.value as string;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as a subquery.
		/// </summary>
		/// <value>The field value as a subquery.</value>
		public SqlSelect						AsSubQuery
		{
			get
			{
				if (this.fieldType == SqlFieldType.SubQuery)
				{
					return this.value as SqlSelect;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as a string that represents a raw sql query snippet.
		/// </summary>
		public string AsRawSql
		{
			get
			{
				if (this.fieldType == SqlFieldType.RawSql)
				{
					return (string) this.value;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the field value as an <see cref="SqlSet"/> that represent a set of constant values.
		/// </summary>
		public SqlSet AsSet
		{
			get
			{
				if (this.fieldType == SqlFieldType.Set)
				{
					return ((SqlSet) this.value);
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Validates the field contents.
		/// </summary>
		/// <param name="validator">The validator.</param>
		/// <returns><c>true</c> if the field is valid; otherwise, <c>false</c>.</returns>
		public bool Validate(ISqlValidator validator)
		{
			switch (this.FieldType)
			{
				case SqlFieldType.Name:				return validator.ValidateName (this.AsName);
				case SqlFieldType.QualifiedName:	return validator.ValidateQualifiedName (this.AsQualifiedName);
			}

			return true;
		}

		/// <summary>
		/// Clones this SQL field.
		/// </summary>
		/// <returns>The copied SQL field.</returns>
		public SqlField Clone()
		{
			SqlField copy = new SqlField ();

			copy.Overwrite (this);

			return copy;
		}

		/// <summary>
		/// Sets the out result of <c>ISqlBuilder.GetSqlParameters</c>.
		/// </summary>
		/// <param name="value">The value.</param>
		public void SetParameterOutResult(object value)
		{
			if ((this.fieldType == SqlFieldType.ParameterInOut) ||
				(this.fieldType == SqlFieldType.ParameterOut) ||
				(this.fieldType == SqlFieldType.ParameterResult))
			{
				this.value = value;
			}
			else
			{
				throw new System.InvalidOperationException ("Field must be an 'out' parameter");
			}
		}

		/// <summary>
		/// Overwrites this field with the contents of the specified field.
		/// </summary>
		/// <param name="field">The source field.</param>
		public void Overwrite(SqlField field)
		{
			this.fieldType = field.fieldType;
			this.sortOrder = field.sortOrder;
			this.rawType   = field.rawType;
			this.alias     = field.alias;
			this.value     = field.value;
		}


		/// <summary>
		/// Creates a field representing the <c>null</c> value.
		/// </summary>
		/// <returns>The field.</returns>
		public static SqlField CreateNull()
		{
			return new SqlField (SqlFieldType.Null);
		}

		/// <summary>
		/// Creates a field representing the default value.
		/// </summary>
		/// <returns>The field.</returns>
		public static SqlField CreateDefault()
		{
			return new SqlField (SqlFieldType.Default);
		}

		/// <summary>
		/// Creates a field representing the specified constant value. The caller
		/// must ensure that the raw value is compatible with the raw type, as
		/// this is not enforced by this method.
		/// </summary>
		/// <param name="value">The raw value.</param>
		/// <param name="type">The raw type.</param>
		/// <returns>The field.</returns>
		public static SqlField CreateConstant(object value, DbRawType type)
		{
			//	A constant has usually no alias name; if the constant is used for
			//	an insert, then the alias will be mapped to the column name.

			//	Automatically convert DbId to long and null to DBNull...

			if (value is DbId)
			{
				value = ((DbId) value).Value;
			}
			else if (value == null)
			{
				value = System.DBNull.Value;
			}

			return new SqlField (SqlFieldType.Constant, value, type);
		}

		public static SqlField CreateParameterIn(object value, DbRawType type)
		{
			return new SqlField (SqlFieldType.ParameterIn, value, type);
		}

		public static SqlField CreateParameterInOut(object value, DbRawType type)
		{
			return new SqlField (SqlFieldType.ParameterInOut, value, type);
		}

		public static SqlField CreateParameterOut()
		{
			return new SqlField (SqlFieldType.ParameterOut);
		}

		public static SqlField CreateParameterOut(DbRawType type)
		{
			return new SqlField (SqlFieldType.ParameterOut, null, type);
		}

		public static SqlField CreateParameterResult(DbRawType type)
		{
			return new SqlField (SqlFieldType.ParameterResult, null, type);
		}

		public static SqlField CreateAll()
		{
			return new SqlField (SqlFieldType.All);
		}

		public static SqlField CreateAliasedName(string name, string alias)
		{
			if (DbSqlStandard.ValidateName (name))
			{
				return new SqlField (name, alias);
			}

			throw new Exceptions.FormatException (string.Format ("{0} is not a valid SQL name", name));
		}

		public static SqlField CreateAliasedName(string tableName, string columnName, string alias)
		{
			string name = DbSqlStandard.QualifyName (tableName, columnName);

			if (DbSqlStandard.ValidateQualifiedName (name))
			{
				return new SqlField (name, alias);
			}

			throw new Exceptions.FormatException (string.Format ("{0} is not a valid SQL name.", name));
		}

		public static SqlField CreateName(string name)
		{
			if (DbSqlStandard.ValidateName (name))
			{
				return new SqlField (name);
			}

			throw new Exceptions.FormatException (string.Format ("{0} is not a valid SQL name", name));
		}

		/// <summary>
		/// Creates the qualified name based on the high level table name and
		/// the SQL column name.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns>The field.</returns>
		public static SqlField CreateName(DbColumn column)
		{
			string tableName  = DbSqlStandard.MakeDelimitedIdentifier (column.Table.Name);
			string columnName = column.GetSqlName ();

			return SqlField.CreateName (tableName, columnName);
		}

		public static SqlField CreateName(string tableName, string columnName)
		{
			string name = DbSqlStandard.QualifyName (tableName, columnName);

			if (DbSqlStandard.ValidateQualifiedName (name))
			{
				return new SqlField (name);
			}

			throw new Exceptions.FormatException (string.Format ("{0} is not a valid SQL name.", name));
		}

		public static SqlField CreateAggregate(SqlAggregate aggregate)
		{
			return new SqlField (SqlFieldType.Aggregate, aggregate, DbRawType.Unknown);
		}

		public static SqlField CreateAggregate(SqlAggregateFunction aggregateFunction, SqlField field)
		{
			return SqlField.CreateAggregate (new SqlAggregate (aggregateFunction, field));
		}

		public static SqlField CreateAggregate(SqlAggregateFunction aggregateFunction, SqlSelectPredicate predicate, SqlField field)
		{
			return SqlField.CreateAggregate (new SqlAggregate (aggregateFunction, predicate, field));
		}

		public static SqlField CreateVariable()
		{
			return new SqlField (SqlFieldType.Variable);
		}

		public static SqlField CreateFunction(SqlFunction sqlFunction)
		{
			return new SqlField (SqlFieldType.Function, sqlFunction, DbRawType.Unknown);
		}

		public static SqlField CreateJoin(SqlJoin sqlJoin)
		{
			return new SqlField (SqlFieldType.Join, sqlJoin, DbRawType.Unknown);
		}

		public static SqlField CreateProcedure(string procedureName)
		{
			return new SqlField (SqlFieldType.Procedure, procedureName, DbRawType.Unknown);
		}

		public static SqlField CreateSubQuery(SqlSelect subQuery)
		{
			return new SqlField (SqlFieldType.SubQuery, subQuery, DbRawType.Unknown);
		}

		public static SqlField CreateSubQuery(SqlSelect subQuery, string alias)
		{
			return new SqlField (SqlFieldType.SubQuery, subQuery, DbRawType.Unknown)
			{
				Alias = alias
			};
		}

		public static SqlField CreateRawSql(string rawSql)
		{
			return new SqlField (SqlFieldType.RawSql, rawSql, DbRawType.Unknown);
		}

		public static SqlField CreateSet(SqlSet sqlSet)
		{
			return new SqlField (SqlFieldType.Set, sqlSet, sqlSet.Type);
		}

		private SqlFieldType					fieldType;
		private SqlSortOrder					sortOrder;
		private DbRawType						rawType;
		private object							value;
		private string							alias;
	}
}
