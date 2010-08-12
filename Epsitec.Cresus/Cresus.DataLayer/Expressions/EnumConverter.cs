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
		/// Converts an <see cref="UnaryComparator"/> to the corresponding <see cref="DbSimpleConditionOperator"/>.
		/// </summary>
		/// <param name="unaryComparator">The <see cref="UnaryComparator"/> to convert.</param>
		/// <returns>The corresponding <see cref="DbSimpleConditionOperator"/>.</returns>
		/// <exception cref="System.NotSupportedException">If the conversion is not possible.</exception>
		public static DbSimpleConditionOperator ToDbSimpleConditionOperator(UnaryComparator unaryComparator)
		{
			switch (unaryComparator)
			{
				case UnaryComparator.IsNull:
					return DbSimpleConditionOperator.IsNull;
				case UnaryComparator.IsNotNull:
					return DbSimpleConditionOperator.IsNotNull;
				default:
					throw new System.NotSupportedException ("Conversion of '" + unaryComparator + "' is not supported");
			}
		}


		/// <summary>
		/// Converts an <see cref="BinaryComparator"/> to the corresponding <see cref="DbSimpleConditionOperator"/>.
		/// </summary>
		/// <param name="binaryComparator">The <see cref="BinaryComparator"/> to convert.</param>
		/// <returns>The corresponding <see cref="DbSimpleConditionOperator"/>.</returns>
		/// <exception cref="System.NotSupportedException">If the conversion is not possible.</exception>
		public static DbSimpleConditionOperator ToDbSimpleConditionOperator(BinaryComparator binaryComparator)
		{
			switch (binaryComparator)
			{
				case BinaryComparator.IsEqual:
					return DbSimpleConditionOperator.Equal;
				case BinaryComparator.IsNotEqual:
					return DbSimpleConditionOperator.NotEqual;
				case BinaryComparator.IsLower:
					return DbSimpleConditionOperator.LessThan;
				case BinaryComparator.IsLowerOrEqual:
					return DbSimpleConditionOperator.LessThanOrEqual;
				case BinaryComparator.IsGreater:
					return DbSimpleConditionOperator.GreaterThan;
				case BinaryComparator.IsGreaterOrEqual:
					return DbSimpleConditionOperator.GreaterThanOrEqual;
				case BinaryComparator.IsLike:
					return DbSimpleConditionOperator.Like;
				case BinaryComparator.IsNotLike:
					return DbSimpleConditionOperator.NotLike;
				case BinaryComparator.IsLikeEscape:
					return DbSimpleConditionOperator.LikeEscape;
				case BinaryComparator.IsNotLikeEscape:
					return DbSimpleConditionOperator.NotLikeEscape;
				default:
					throw new System.NotSupportedException ("Conversion of '" + binaryComparator + "' is not supported");
			}
		}


		/// <summary>
		/// Converts an <see cref="UnaryOperator"/> to the corresponding <see cref="DbConditionModifierOperator"/>.
		/// </summary>
		/// <param name="unaryOperator">The <see cref="UnaryOperator"/> to convert.</param>
		/// <returns>The corresponding <see cref="DbConditionModifierOperator"/>.</returns>
		/// <exception cref="System.NotSupportedException">If the conversion is not possible.</exception>
		public static DbConditionModifierOperator ToDbConditionModifierOperator(UnaryOperator unaryOperator)
		{
			switch (unaryOperator)
			{
				case UnaryOperator.Not:
					return DbConditionModifierOperator.Not;
				default:
					throw new System.NotSupportedException ("Conversion of '" + unaryOperator + "' is not supported");
			}
		}


		/// <summary>
		/// Converts an <see cref="BinaryOperator"/> to the corresponding <see cref="DbConditionCombinerOperator"/>.
		/// </summary>
		/// <param name="binaryOperator">The <see cref="BinaryOperator"/> to convert.</param>
		/// <returns>The corresponding <see cref="DbConditionCombinerOperator"/>.</returns>
		/// <exception cref="System.NotSupportedException">If the conversion is not possible.</exception>
		public static DbConditionCombinerOperator ToDbConditionCombinerOperator(BinaryOperator binaryOperator)
		{
			switch (binaryOperator)
			{
				case BinaryOperator.And:
					return DbConditionCombinerOperator.And;
				case BinaryOperator.Or:
					return DbConditionCombinerOperator.Or;
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
				case Type.Double:
					return DbRawType.LargeDecimal;
				case Type.Date:
					return DbRawType.Date;
				case Type.Time:
					return DbRawType.Time;
				case Type.DateTime:
					return DbRawType.DateTime;
				case Type.String:
					return DbRawType.String;
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
					return DbSimpleType.Decimal;
				case Type.Int16:
					return DbSimpleType.Decimal;
				case Type.Int32:
					return DbSimpleType.Decimal;
				case Type.Int64:
					return DbSimpleType.Decimal;
				case Type.Double:
					return DbSimpleType.Decimal;
				case Type.Date:
					return DbSimpleType.Date;
				case Type.Time:
					return DbSimpleType.Time;
				case Type.DateTime:
					return DbSimpleType.DateTime;
				case Type.String:
					return DbSimpleType.String;
				default:
					throw new System.NotSupportedException ("Conversion of '" + type + "' is not supported");
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
