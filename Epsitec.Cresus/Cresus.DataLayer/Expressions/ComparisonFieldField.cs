using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>ComparisonFieldField</c> class represents a comparison between two
	/// <see cref="Field"/>, such as (a = b).
	/// </summary>
	public class ComparisonFieldField : Comparison
	{


		/// <summary>
		/// Builds a new <c>ComparisonFieldField</c>.
		/// </summary>
		/// <param name="left">The <see cref="Field"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <param name="op">The <see cref="BinaryComparator"/> used by the <c>ComparisonFieldField</c>.</param>
		/// <param name="right">The <see cref="Field"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="left"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="right"/> is null.</exception>
		public ComparisonFieldField(Field left, BinaryComparator op, Field right) : base ()
		{
			left.ThrowIfNull ("left");
			right.ThrowIfNull ("right");
			
			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		/// <summary>
		/// The left side of the <c>ComparisonFieldField</c>.
		/// </summary>
		public Field Left
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="BinaryComparator"/> of the <c>ComparisonFieldField</c>.
		/// </summary>
		public BinaryComparator Operator
		{
			get;
			private set;
		}


		/// <summary>
		/// The right side of the <c>ComparisonFieldField</c>.
		/// </summary>
		public Field Right
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


	}


}
