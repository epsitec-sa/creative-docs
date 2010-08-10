using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

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

	}


}
