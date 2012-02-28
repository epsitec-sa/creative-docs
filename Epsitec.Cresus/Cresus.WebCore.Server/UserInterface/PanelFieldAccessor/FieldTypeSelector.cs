using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor
{


	internal static class FieldTypeSelector
	{


		public static FieldType GetFieldType(Type type)
		{
			if (FieldTypeSelector.IsTypeSuitableForCollectionField (type))
			{
				return FieldType.EntityCollection;
			}
			else if (FieldTypeSelector.IsTypeSuitableForEntityField (type))
			{
				return FieldType.EntityReference;
			}
			else if (FieldTypeSelector.IsTypeSuitableForEnumField (type))
			{
				return FieldType.Enumeration;
			}
			else if (FieldTypeSelector.IsTypeSuitableForDateField (type))
			{
				return FieldType.Date;
			}
			else if (FieldTypeSelector.IsTypeSuitableForTextField (type))
			{
				return FieldType.Text;
			}
			else
			{
				throw new NotSupportedException ("Type of field is not supported.");
			}
		}


		private static bool IsTypeSuitableForCollectionField(Type type)
		{
			return type.IsGenericIListOfEntities ();
		}


		private static bool IsTypeSuitableForEntityField(Type type)
		{
			return type.IsEntity ();
		}


		private static bool IsTypeSuitableForDateField(Type type)
		{
			return type == typeof (Date)
		        || type == typeof (Date?);
		}


		private static bool IsTypeSuitableForEnumField(Type type)
		{
			var underlyingType = type.GetNullableTypeUnderlyingType ();

			return type.IsEnum || (underlyingType != null && underlyingType.IsEnum);
		}


		private static bool IsTypeSuitableForTextField(Type type)
		{
			return type == typeof (string)
		        || type == typeof (FormattedText)
		        || type == typeof (long)
		        || type == typeof (long?)
		        || type == typeof (decimal)
		        || type == typeof (decimal?)
		        || type == typeof (int)
		        || type == typeof (int?)
		        || type == typeof (bool)
		        || type == typeof (bool?);
		}


	}


}
