using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal static class FieldTypeSelector
	{


		public static FieldType GetFieldType(Type type)
		{
			if (FieldTypeSelector.IsEntityCollectionType (type))
			{
				return FieldType.EntityCollection;
			}
			else if (FieldTypeSelector.IsEntityReferenceType (type))
			{
				return FieldType.EntityReference;
			}
			else if (FieldTypeSelector.IsEnumerationType (type))
			{
				return FieldType.Enumeration;
			}
			else if (FieldTypeSelector.IsDateType (type))
			{
				return FieldType.Date;
			}
			else if (FieldTypeSelector.IsBooleanType (type))
			{
				return FieldType.Boolean;
			}
			else if (FieldTypeSelector.IsIntegerType (type))
			{
				return FieldType.Integer;
			}
			else if (FieldTypeSelector.IsDecimalType (type))
			{
				return FieldType.Decimal;
			}
			else if (FieldTypeSelector.IsTextType (type))
			{
				return FieldType.Text;
			}
			else
			{
				throw new NotSupportedException ();
			}
		}


		private static bool IsEntityCollectionType(Type type)
		{
			return type.IsGenericIListOfEntities ();
		}


		private static bool IsEntityReferenceType(Type type)
		{
			return type.IsEntity ();
		}


		private static bool IsDateType(Type type)
		{
			return type == typeof (Date)
		        || type == typeof (Date?);
		}


		private static bool IsBooleanType(Type type)
		{
			return type == typeof (bool)
		        || type == typeof (bool?);
		}


		private static bool IsEnumerationType(Type type)
		{
			var underlyingType = type.GetNullableTypeUnderlyingType ();

			return type.IsEnum || (underlyingType != null && underlyingType.IsEnum);
		}


		private static bool IsIntegerType(Type type)
		{
			return type == typeof (short)
		        || type == typeof (short?)
		        || type == typeof (int)
		        || type == typeof (int?)
		        || type == typeof (long)
		        || type == typeof (long?);
		}


		private static bool IsDecimalType(Type type)
		{
			return type == typeof (decimal)
		        || type == typeof (decimal?);
		}


		private static bool IsTextType(Type type)
		{
			return type == typeof (string)
		        || type == typeof (FormattedText)
		        || type == typeof (FormattedText?);
		}


	}


}
