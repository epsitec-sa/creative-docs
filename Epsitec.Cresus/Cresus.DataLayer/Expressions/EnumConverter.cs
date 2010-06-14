using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{
	
	
	internal static class EnumConverter
	{


		public static DbSimpleConditionOperator ToDbCompare(UnaryComparator unaryComparator)
		{
			switch (unaryComparator)
			{
				case UnaryComparator.IsNull:
					return DbSimpleConditionOperator.IsNull;
				case UnaryComparator.IsNotNull:
					return DbSimpleConditionOperator.IsNotNull;
				default:
					throw new System.ArgumentException ("Conversion of '" + unaryComparator + "' is not supported");
			}
		}


		public static DbSimpleConditionOperator ToDbCompare(BinaryComparator binaryComparator)
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
					throw new System.ArgumentException ("Conversion of '" + binaryComparator + "' is not supported");
			}
		}


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
					throw new System.ArgumentException ("Conversion of '" + type + "' is not supported");
			}
		}


	}


}
