using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.WebCore.Server.NancyModules;

using System;

using System.Collections.Generic;

using System.Globalization;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal static class ValueConverter
	{


		public static object ConvertEntityToField(object value, FieldType fieldType)
		{
			switch (fieldType)
			{
				case FieldType.Boolean:
					return ValueConverter.ConvertEntityToFieldForBool (value);

				case FieldType.Date:
					return ValueConverter.ConvertEntityToFieldForDate (value);

				case FieldType.Decimal:
					return ValueConverter.ConvertEntityToFieldForDecimal (value);

				case FieldType.Enumeration:
					return ValueConverter.ConvertEntityToFieldForEnumeration (value);

				case FieldType.Integer:
					return ValueConverter.ConvertEntityToFieldForInteger (value);

				case FieldType.Text:
					return ValueConverter.ConvertEntityToFieldForText (value);
				
				case FieldType.EntityCollection:
				case FieldType.EntityReference:
				default:
					throw new NotImplementedException ();
			}
		}


		public static bool? ConvertEntityToFieldForBool(object value)
		{
			if (value == null)
			{
				return null;
			}

			return (bool) value;
		}


		public static Date? ConvertEntityToFieldForDate(object value)
		{
			if (value == null)
			{
				return null;
			}

			return (Date) value;
		}


		public static decimal? ConvertEntityToFieldForDecimal(object value)
		{
			if (value == null)
			{
				return null;
			}

			return (decimal) value;
		}


		public static Enum ConvertEntityToFieldForEnumeration(object value)
		{
			return (Enum) value;
		}


		public static long? ConvertEntityToFieldForInteger(object value)
		{
			if (value == null)
			{
				return null;
			}

			return InvariantConverter.ToLong (value);
		}


		public static string ConvertEntityToFieldForText(object value)
		{
			if (value == null)
			{
				return null;
			}
			else if (value is FormattedText)
			{
				var formattedText = (FormattedText) value;

				return formattedText.ToString ();
			}
			else if (value is FormattedText?)
			{
				var formattedText = (FormattedText?) value;

				return formattedText.Value.ToString ();
			}
			else
			{
				return (string) value;
			}
		}


		public static object ConvertFieldToClient(object value, FieldType fieldType)
		{
			switch (fieldType)
			{
				case FieldType.Boolean:
					return ValueConverter.ConvertFieldToClientForBool ((bool?) value);

				case FieldType.Date:
					return ValueConverter.ConvertFieldToClientForDate ((Date?) value);

				case FieldType.Decimal:
					return ValueConverter.ConvertFieldToClientForDecimal ((decimal?) value);

				case FieldType.Enumeration:
					return ValueConverter.ConvertFieldToClientForEnumeration ((Enum) value);

				case FieldType.Integer:
					return ValueConverter.ConvertFieldToClientForInteger ((long?) value);

				case FieldType.Text:
					return ValueConverter.ConvertFieldToClientForText ((string) value);

				case FieldType.EntityCollection:
				case FieldType.EntityReference:
				default:
					throw new NotImplementedException ();
			}
		}


		public static bool? ConvertFieldToClientForBool(bool? value)
		{
			return value;
		}


		public static string ConvertFieldToClientForDate(Date? value)
		{
			if (value == null)
			{
				return null;
			}

			var format = ValueConverter.dateFormat;
			var culture = ValueConverter.dateCulture;

			return value.Value.ToString (format, culture);
		}


		public static decimal? ConvertFieldToClientForDecimal(decimal? value)
		{
			return value;
		}


		public static string ConvertFieldToClientForEnumeration(Enum value)
		{
			if (value == null)
			{
				return Constants.KeyForNullValue;
			}

			var intValue = EnumType.ConvertToInt (value);

			return InvariantConverter.ToString (intValue);
		}


		public static long? ConvertFieldToClientForInteger(long? value)
		{
			return value;
		}


		public static string ConvertFieldToClientForText(string value)
		{
			return value;
		}


		public static object ConvertClientToField(object value, FieldType fieldType, Type valueType)
		{
			switch (fieldType)
			{
				case FieldType.Boolean:
					return ValueConverter.ConvertClientToFieldForBool (value);

				case FieldType.Date:
					return ValueConverter.ConvertClientToFieldForDate (value);

				case FieldType.Decimal:
					return ValueConverter.ConvertClientToFieldForDecimal (value);

				case FieldType.Enumeration:
					return ValueConverter.ConvertClientToFieldForEnumeration (value, valueType);

				case FieldType.Integer:
					return ValueConverter.ConvertClientToFieldForInteger (value);

				case FieldType.Text:
					return ValueConverter.ConvertClientToFieldForText (value);

				case FieldType.EntityCollection:
				case FieldType.EntityReference:
				default:
					throw new NotImplementedException ();
			}
		}


		public static bool? ConvertClientToFieldForBool(object value)
		{
			if (value == null)
			{
				return null;
			}

			return (bool) value;
		}


		public static Date? ConvertClientToFieldForDate(object value)
		{
			var stringValue = (string) value;

			if (string.IsNullOrEmpty (stringValue))
			{
				return null;
			}

			var format = ValueConverter.dateFormat;
			var culture = ValueConverter.dateCulture;

			return new Date (DateTime.ParseExact (stringValue, format, culture));
		}


		public static decimal? ConvertClientToFieldForDecimal(object value)
		{
			if (value == null)
			{
				return null;
			}

			return (decimal) value;
		}


		public static Enum ConvertClientToFieldForEnumeration(object value, Type valueType)
		{
			var stringValue = (string) value;

			if (string.IsNullOrEmpty (stringValue) || stringValue == Constants.KeyForNullValue)
			{
				return null;
			}

			if (valueType.IsNullable ())
			{
				valueType = valueType.GetNullableTypeUnderlyingType ();
			}

			return (Enum) Enum.Parse (valueType, stringValue);
		}


		public static long? ConvertClientToFieldForInteger(object value)
		{
			if (value == null)
			{
				return null;
			}

			return (long) value;
		}


		public static string ConvertClientToFieldForText(object value)
		{
			if (value == null)
			{
				return null;
			}

			return (string) value;
		}


		public static object ConvertFieldToEntity(object value, FieldType fieldType, Type valueType)
		{
			switch (fieldType)
			{
				case FieldType.Boolean:
					return ValueConverter.ConvertFieldToEntityForBool ((bool?) value);

				case FieldType.Date:
					return ValueConverter.ConvertFieldToEntityForDate ((Date?) value);

				case FieldType.Decimal:
					return ValueConverter.ConvertFieldToEntityForDecimal ((decimal?) value);

				case FieldType.Enumeration:
					return ValueConverter.ConvertFieldToEntityForEnumeration ((Enum) value);

				case FieldType.Integer:
					return ValueConverter.ConvertFieldToEntityForInteger ((long?) value, valueType);

				case FieldType.Text:
					return ValueConverter.ConvertFieldToEntityForText ((string) value, valueType);

				case FieldType.EntityCollection:
				case FieldType.EntityReference:
				default:
					throw new NotImplementedException ();
			}
		}


		public static bool? ConvertFieldToEntityForBool(bool? value)
		{
			return value;
		}


		public static Date? ConvertFieldToEntityForDate(Date? value)
		{
			return value;
		}


		public static decimal? ConvertFieldToEntityForDecimal(decimal? value)
		{
			return value;
		}


		public static Enum ConvertFieldToEntityForEnumeration(Enum value)
		{
			return value;
		}


		public static object ConvertFieldToEntityForInteger(long? value, Type valueType)
		{
			if (valueType == typeof (long?))
			{
				return (long?) value;
			}
			else if (valueType == typeof (long))
			{
				return (long) value;
			}
			else if (valueType == typeof (int?))
			{
				return (int?) (long?) value;
			}
			else if (valueType == typeof (int))
			{
				return (int) (long) value;
			}
			else if (valueType == typeof (short?))
			{
				return (short?) (long?) value;
			}
			else if (valueType == typeof (short))
			{
				return (short) (long) value;
			}
			else
			{
				throw new NotImplementedException ();
			}
		}


		public static object ConvertFieldToEntityForText(string value, Type valueType)
		{
			if (valueType == typeof (FormattedText?))
			{
				return (FormattedText?) (value);
			}
			else if (valueType == typeof (FormattedText))
			{
				return (FormattedText) value;
			}
			else if (valueType == typeof (string))
			{
				return value;
			}
			else
			{
				throw new NotImplementedException ();
			}
		}


		private static readonly string dateFormat = "dd.MM.yyyy";


		private static readonly CultureInfo dateCulture = CultureInfo.InvariantCulture;


	}


}
