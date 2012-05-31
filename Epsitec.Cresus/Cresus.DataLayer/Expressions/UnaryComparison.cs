using Epsitec.Common.Support.EntityEngine;
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
		/// <param name="op">The predicate to apply on the <see cref="EntityField"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="field"/> is null.</exception>
		public UnaryComparison(EntityField field, UnaryComparator op) : base()
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
		/// The <see cref="EntityField"/> of the current instance.
		/// </summary>
		public EntityField Field
		{
			get;
			private set;
		}


		internal override SqlFunction CreateSqlCondition(SqlFieldBuilder builder)
		{
			return new SqlFunction
			(
				EnumConverter.ToSqlFunctionCode (this.Operator),
				this.Field.CreateSqlField (builder)
			);
		}


		internal override void CheckFields(FieldChecker checker)
		{
			this.Field.CheckField (checker);
		}


		internal override void AddEntities(HashSet<AbstractEntity> entities)
		{
			this.Field.AddEntities (entities);
		}


	}


}
