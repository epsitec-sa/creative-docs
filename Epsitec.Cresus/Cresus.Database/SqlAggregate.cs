//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlAggregate</c> structure describes an aggregate SQL function.
	/// The field cannot itself be another aggregate.
	/// </summary>
	public struct SqlAggregate
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlAggregate"/> structure.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="field">The field.</param>
		public SqlAggregate(SqlAggregateFunction type, SqlField field)
			: this (type, SqlSelectPredicate.All, field)
		{
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="type">The aggregate function.</param>
		/// <param name="predicate">The predicate used to customize the aggregate</param>
		/// <param name="field">The field.</param>
		public SqlAggregate(SqlAggregateFunction type, SqlSelectPredicate predicate, SqlField field)
		{
			switch (field.FieldType)
			{
				case SqlFieldType.All:
				case SqlFieldType.Constant:
				case SqlFieldType.Name:
				case SqlFieldType.QualifiedName:
				case SqlFieldType.Function:
				case SqlFieldType.Procedure:
					break;

				default:
					throw new System.NotSupportedException ("SqlField type not supported");
			}

			this.function = type;
			this.field = field;
			this.predicate = predicate;
		}

		/// <summary>
		/// Gets the aggregate function.
		/// </summary>
		/// <value>The aggregate function.</value>
		public SqlAggregateFunction				Function
		{
			get
			{
				return this.function;
			}
		}

		/// <summary>
		/// Gets the field on which to apply the aggregate function.
		/// </summary>
		/// <value>The field.</value>
		public SqlField							Field
		{
			get
			{
				return this.field;
			}
		}

		/// <summary>
		/// Gets the predicate used to modify the aggregate function.
		/// </summary>
		public SqlSelectPredicate Predicate
		{
			get
			{
				return this.predicate;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this aggregate is empty.
		/// </summary>
		/// <value><c>true</c> if this aggregate is empty; otherwise, <c>false</c>.</value>
		public bool								IsEmpty
		{
			get
			{
				return (this.function == SqlAggregateFunction.Unknown) && (this.field == null);
			}
		}
		
		public static readonly SqlAggregate		Empty;

		private readonly SqlAggregateFunction	function;
		private readonly SqlField				field;
		private readonly SqlSelectPredicate		predicate;
	}
}
