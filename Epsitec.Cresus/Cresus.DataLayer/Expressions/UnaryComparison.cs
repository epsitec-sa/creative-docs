﻿using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{
	

	/// <summary>
	/// The <c>UnaryComparison</c> class represents a predicate on a single <see cref="Field"/> such
	/// as (a is null).
	/// </summary>
	public class UnaryComparison : Comparison
	{


		/// <summary>
		/// Builds a new <c>UnaryComparison</c>.
		/// </summary>
		/// <param name="field">The field on which to apply the <see cref="UnaryComparator"/>.</param>
		/// <param name="op">The predicate to apply on the <see cref="Field"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="field"/> is null.</exception>
		public UnaryComparison(Field field, UnaryComparator op) : base()
		{
			field.ThrowIfNull ("field");
			
			this.Operator = op;
			this.Field = field;
		}


		/// <summary>
		/// The <see cref="UnaryOperator"/> of the current instance.
		/// </summary>
		public UnaryComparator Operator
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="Field"/> of the current instance.
		/// </summary>
		public Field Field
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the sequence of field ids that are used in this instance.
		/// </summary>
		/// <returns>The sequence of field ids that are used in this instance.</returns>
		internal override IEnumerable<Druid> GetFields()
		{
			return ExpressionFields.GetFields (this);
		}


		/// <summary>
		/// Converts this instance to an equivalent <see cref="DbAbstractCondition"/>, using a
		/// resolver to convert the <see cref="Druid"/> of the fields to the appropriate
		/// <see cref="DbTableColumn"/>.
		/// </summary>
		/// <param name="expressionConverter">The <see cref="ExpressionConverter"/> used to convert this instance.</param>
		/// <param name="columnResolver">The function used to resolve the <see cref="DbTableColumn"/> given an <see cref="Druid"/>.</param>
		/// <returns>The new <see cref="DbAbstractCondition"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="expressionConverter"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="columnResolver"/> is <c>null</c>.</exception>
		internal override DbAbstractCondition CreateDbCondition(ExpressionConverter expressionConverter, System.Func<Druid, DbTableColumn> columnResolver)
		{
			expressionConverter.ThrowIfNull ("expressionConverter");
			columnResolver.ThrowIfNull ("columnResolver");

			return expressionConverter.CreateDbCondition (this, columnResolver);
		}


		internal override SqlFunction CreateSqlCondition(System.Func<DbRawType, DbSimpleType, DbNumDef, object, SqlField> sqlConstantResolver, System.Func<Druid, SqlField> sqlColumnResolver)
		{
			return new SqlFunction
			(
				EnumConverter.ToSqlFunctionCode (this.Operator),
				this.Field.CreateSqlField (sqlColumnResolver)
			);
		}


	}


}
