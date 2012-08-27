using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{
	
	
	/// <summary>
	/// The <c>EnumConverter</c> class provides the tools required to convert between enumerations
	/// defined in this layer and enumerations defined in the lower layers.
	/// </summary>
	internal static class EnumConverter
	{


		/// <summary>
		/// Gets the <see cref="SqlFunctionCode"/> that is equivalent to the given
		/// <see cref="UnaryComparator"/>.
		/// </summary>
		/// <param name="unaryComparator">The <see cref="UnaryComparator"/> to convert.</param>
		/// <returns>The converted <see cref="UnaryComparator"/>.</returns>
		public static SqlFunctionCode ToSqlFunctionCode(UnaryComparator unaryComparator)
		{
			switch (unaryComparator)
			{
				case UnaryComparator.IsNull:
					return SqlFunctionCode.CompareIsNull;
				case UnaryComparator.IsNotNull:
					return SqlFunctionCode.CompareIsNotNull;
				default:
					throw new System.NotSupportedException ("Conversion of '" + unaryComparator + "' is not supported");
			}
		}


		/// <summary>
		/// Gets the <see cref="SqlFunctionCode"/> that is equivalent to the given
		/// <see cref="BinaryComparator"/>.
		/// </summary>
		/// <param name="binaryComparator">The <see cref="BinaryComparator"/> to convert.</param>
		/// <returns>The converted <see cref="BinaryComparator"/>.</returns>
		public static SqlFunctionCode ToSqlFunctionCode(BinaryComparator binaryComparator)
		{
			switch (binaryComparator)
			{
				case BinaryComparator.IsEqual:
					return SqlFunctionCode.CompareEqual;
				case BinaryComparator.IsNotEqual:
					return SqlFunctionCode.CompareNotEqual;
				case BinaryComparator.IsLower:
					return SqlFunctionCode.CompareLessThan;
				case BinaryComparator.IsLowerOrEqual:
					return SqlFunctionCode.CompareLessThanOrEqual;
				case BinaryComparator.IsGreater:
					return SqlFunctionCode.CompareGreaterThan;
				case BinaryComparator.IsGreaterOrEqual:
					return SqlFunctionCode.CompareGreaterThanOrEqual;
				case BinaryComparator.IsLike:
					return SqlFunctionCode.CompareLike;
				case BinaryComparator.IsNotLike:
					return SqlFunctionCode.CompareNotLike;
				case BinaryComparator.IsLikeEscape:
					return SqlFunctionCode.CompareLikeEscape;
				case BinaryComparator.IsNotLikeEscape:
					return SqlFunctionCode.CompareNotLikeEscape;
				default:
					throw new System.NotSupportedException ("Conversion of '" + binaryComparator + "' is not supported");
			}
		}


		/// <summary>
		/// Gets the <see cref="SqlFunctionCode"/> that is equivalent to the given
		/// <see cref="SetComparator"/>.
		/// </summary>
		/// <param name="setComparator">The <see cref="SetComparator"/> to convert.</param>
		/// <returns>The converted <see cref="SetComparator"/>.</returns>
		public static SqlFunctionCode ToSqlFunctionCode(SetComparator setComparator)
		{
			switch (setComparator)
			{
				case SetComparator.In:
					return SqlFunctionCode.SetIn;
				case SetComparator.NotIn:
					return SqlFunctionCode.SetNotIn;
				default:
					throw new System.NotSupportedException ("Conversion of '" + setComparator + "' is not supported");
			}
		}


		/// <summary>
		/// Gets the <see cref="SqlFunctionCode"/> that is equivalent to the given
		/// <see cref="UnaryOperator"/>.
		/// </summary>
		/// <param name="unaryOperator">The <see cref="UnaryOperator"/> to convert.</param>
		/// <returns>The converted <see cref="UnaryOperator"/>.</returns>
		public static SqlFunctionCode ToSqlFunctionCode(UnaryOperator unaryOperator)
		{
			switch (unaryOperator)
			{
				case UnaryOperator.Not:
					return SqlFunctionCode.LogicNot;
				default:
					throw new System.NotSupportedException ("Conversion of '" + unaryOperator + "' is not supported");
			}
		}


		/// <summary>
		/// Gets the <see cref="SqlFunctionCode"/> that is equivalent to the given
		/// <see cref="BinaryOperator"/>.
		/// </summary>
		/// <param name="binaryOperator">The <see cref="BinaryOperator"/> to convert.</param>
		/// <returns>The converted <see cref="BinaryOperator"/>.</returns>
		public static SqlFunctionCode ToSqlFunctionCode(BinaryOperator binaryOperator)
		{
			switch (binaryOperator)
			{
				case BinaryOperator.And:
					return SqlFunctionCode.LogicAnd;
				case BinaryOperator.Or:
					return SqlFunctionCode.LogicOr;
				default:
					throw new System.NotSupportedException ("Conversion of '" + binaryOperator + "' is not supported");
			}
		}


		/// <summary>
		/// Converts a <see cref="SortOrder"/> to the corresponding <see cref="SqlSortOrder"/>.
		/// </summary>
		/// <param name="sortOrder">The <see cref="SortOrder"/> to convert.</param>
		/// <returns>The corresponding <see cref="SqlSortOrder"/>.</returns>
		/// <exception cref="System.NotSupportedException">If the conversion is not possible.</exception>
		public static SqlSortOrder ToSqlSortOrder(SortOrder sortOrder)
		{
			switch (sortOrder)
			{
				case SortOrder.Ascending:
					return SqlSortOrder.Ascending;

				case SortOrder.Descending:
					return SqlSortOrder.Descending;

				default:
					throw new System.NotSupportedException ("Conversion of '" + sortOrder + "' is not supported");
			}
		}


	}


}
