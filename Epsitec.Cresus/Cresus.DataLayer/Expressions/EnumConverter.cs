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
		/// Converts an <see cref="Type"/> to the corresponding <see cref="DbRawType"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> to convert.</param>
		/// <returns>The corresponding <see cref="DbRawType"/>.</returns>
		/// <exception cref="System.NotSupportedException">If the conversion is not possible.</exception>
		public static DbRawType ToDbRawType(Type type)
		{
			switch (type)
			{
				case Type.Boolean:
					return DbRawType.Boolean;
				case Type.Int16:
					return DbRawType.Int16;
				case Type.Int32:
					return DbRawType.Int32;
				case Type.Int64:
					return DbRawType.Int64;
				case Type.Enum:
					return DbRawType.Int32;
				case Type.Decimal:
					return DbRawType.LargeDecimal;
				case Type.Date:
					return DbRawType.Date;
				case Type.Time:
					return DbRawType.Time;
				case Type.DateTime:
					return DbRawType.DateTime;
				case Type.String:
					return DbRawType.String;
				case Type.ByteArray:
					return DbRawType.ByteArray;
				default:
					throw new System.NotSupportedException ("Conversion of '" + type + "' is not supported");
			}
		}


		/// <summary>
		/// Converts a <see cref="Type"/> to the corresponding <see cref="DbSimpleType"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> to convert.</param>
		/// <returns>The corresponding <see cref="DbSimpleType"/>.</returns>
		/// <exception cref="System.NotSupportedException">If the conversion is not possible.</exception>
		public static DbSimpleType ToDbSimpleType(Type type)
		{
			switch (type)
			{
				case Type.Boolean:
				case Type.Int16:
				case Type.Int32:
				case Type.Int64:
				case Type.Decimal:
				case Type.Enum:
					return DbSimpleType.Decimal;
				case Type.Date:
					return DbSimpleType.Date;
				case Type.Time:
					return DbSimpleType.Time;
				case Type.DateTime:
					return DbSimpleType.DateTime;
				case Type.String:
					return DbSimpleType.String;
				case Type.ByteArray:
					return DbSimpleType.ByteArray;
				default:
					throw new System.NotSupportedException ("Conversion of '" + type + "' is not supported");
			}
		}


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


		/// <summary>
		/// Converts a <see cref="Type"/> to the corresponding <see cref="DbNumDef"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> to convert.</param>
		/// <returns>The corresponding <see cref="DbNumDef"/>.</returns>
		/// <exception cref="System.NotSupportedException">If the conversion is not possible.</exception>
		public static DbNumDef ToDbNumDef(Type type)
		{
			DbRawType rawType = EnumConverter.ToDbRawType (type);

			return DbNumDef.FromRawType (rawType);
		}


	}


}
