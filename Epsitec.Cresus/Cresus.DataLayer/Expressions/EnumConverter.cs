using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{
	
	
	internal static class EnumConverter
	{


		public static DbCompare ToDbCompare(UnaryComparator unaryComparator)
		{
			switch (unaryComparator)
			{
				case UnaryComparator.IsNull:
					return DbCompare.IsNull;
				case UnaryComparator.IsNotNull:
					return DbCompare.IsNotNull;
				default:
					throw new System.ArgumentException ("Conversion of '" + unaryComparator + "' is not supported");
			}
		}


		public static DbCompare ToDbCompare(BinaryComparator binaryComparator)
		{
			switch (binaryComparator)
			{
				case BinaryComparator.IsEqual:
					return DbCompare.Equal;
				case BinaryComparator.IsNotEqual:
					return DbCompare.NotEqual;
				case BinaryComparator.IsLower:
					return DbCompare.LessThan;
				case BinaryComparator.IsLowerOrEqual:
					return DbCompare.LessThanOrEqual;
				case BinaryComparator.IsGreater:
					return DbCompare.GreaterThan;
				case BinaryComparator.IsGreaterOrEqual:
					return DbCompare.GreaterThanOrEqual;
				case BinaryComparator.IsLike:
					return DbCompare.Like;
				case BinaryComparator.IsNotLike:
					return DbCompare.NotLike;
				case BinaryComparator.IsLikeEscape:
					return DbCompare.LikeEscape;
				case BinaryComparator.IsNotLikeEscape:
					return DbCompare.NotLikeEscape;
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
