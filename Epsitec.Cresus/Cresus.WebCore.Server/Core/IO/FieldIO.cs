using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.NancyModules;

using Epsitec.Cresus.DataLayer.Context;

using Nancy;

using System;

using System.Collections;
using System.Collections.Generic;

using System.Globalization;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.IO
{


	internal static class FieldIO
	{


		public static object ConvertToClientRecursive(BusinessContext businessContext, object value)
		{
			return FieldIO.ConvertToClientRecursive (businessContext.DataContext, value);
		}


		public static object ConvertToClientRecursive(DataContext dataContext, object value)
		{
			if (value == null)
			{
				return "null";
			}
			else if (value is IDictionary)
			{
				return FieldIO.ConvertToClientRecursive (dataContext, (IDictionary) value);
			}
			else if (value is IList)
			{
				return FieldIO.ConvertToClientRecursive (dataContext, (IList) value);
			}
			else if (value is Array)
			{
				return FieldIO.ConvertToClientRecursive (dataContext, (Array) value);
			}
			else
			{
				return FieldIO.ConvertToClient (dataContext, value);
			}
		}


		public static object ConvertToClientRecursive(DataContext dataContext, IDictionary dictionary)
		{
			return dictionary
				.AsEntries ()
				.ToDictionary
				(
					e => (string) e.Key,
					e => (object) FieldIO.ConvertToClientRecursive (dataContext, e.Value)
				);
		}


		public static object ConvertToClientRecursive(DataContext dataContext, IList list)
		{
			return list
				.Cast<object> ()
				.Select (e => FieldIO.ConvertToClientRecursive (dataContext, e))
				.ToList ();
		}


		public static object ConvertToClientRecursive(DataContext dataContext, Array array)
		{
			return array
				.Cast<object> ()
				.Select (e => FieldIO.ConvertToClientRecursive (dataContext, e))
				.ToArray ();
		}


		public static object ConvertToClient(BusinessContext businessContext, object value, FieldType? fieldType = null)
		{
			return FieldIO.ConvertToClient (businessContext.DataContext, value, fieldType);
		}


		public static object ConvertToClient(DataContext dataContext, object value, FieldType? fieldType = null)
		{
			switch (fieldType ?? FieldTypeSelector.GetFieldType (value.GetType ()))
			{
				case FieldType.Boolean:
					return FieldIO.ConvertBooleanToClient (value);

				case FieldType.Date:
					return FieldIO.ConvertDateToClient (value);

				case FieldType.Decimal:
					return FieldIO.ConvertDecimalToClient (value);

				case FieldType.Enumeration:
					return FieldIO.ConvertEnumerationToClient (value);

				case FieldType.Integer:
					return FieldIO.ConvertIntegerToClient (value);

				case FieldType.Text:
					return FieldIO.ConvertTextToClient (value);

				case FieldType.EntityCollection:
					return FieldIO.ConvertEntityCollectionToClient (dataContext, value);

				case FieldType.EntityReference:
					return FieldIO.ConvertEntityReferenceToClient (dataContext, value);

				default:
					throw new NotImplementedException ();
			}
		}


		private static object ConvertBooleanToClient(object value)
		{
			if (value == null)
			{
				return null;
			}

			return (bool) value;
		}


		private static object ConvertDateToClient(object value)
		{
			if (value == null)
			{
				return null;
			}

			var date = (Date) value;
			var format = FieldIO.dateFormat;
			var culture = FieldIO.dateCulture;

			return date.ToString (format, culture);
		}


		private static object ConvertDecimalToClient(object value)
		{
			if (value == null)
			{
				return null;
			}

			return (decimal) value;
		}


		private static object ConvertEnumerationToClient(object value)
		{
			if (value == null)
			{
				return Constants.KeyForNullValue;
			}
			
			var intValue = EnumType.ConvertToInt ((Enum) value);

			return InvariantConverter.ToString (intValue);
		}


		private static object ConvertIntegerToClient(object value)
		{
			if (value == null)
			{
				return null;
			}

			return InvariantConverter.ToLong (value);
		}


		private static object ConvertTextToClient(object value)
		{
			if (value == null)
			{
				return null;
			}
			
			if (value is FormattedText)
			{
				var formattedText = (FormattedText) value;

				return formattedText.ToSimpleText ();
			}
			else if (value is FormattedText?)
			{
				var formattedText = (FormattedText?) value;

				return formattedText.HasValue
					? formattedText.Value.ToSimpleText ()
					: "";
			}
			else
			{
				return (string) value;
			}
		}


		private static object ConvertEntityCollectionToClient(DataContext dataContext, object value)
		{
			var entities = (IEnumerable<AbstractEntity>) value;

			return entities
				.Select (e => FieldIO.ConvertEntityToClient (dataContext, e))
				.ToList ();
		}


		private static object ConvertEntityReferenceToClient(DataContext dataContext, object value)
		{
			var entity = (AbstractEntity) value;

			return FieldIO.ConvertEntityToClient (dataContext, entity);
		}

		
		private static object ConvertEntityToClient(DataContext dataContext, AbstractEntity entity)
		{
			string displayed;
			string submitted;
			
			if (entity == null || entity.IsNull ())
			{
				displayed = Res.Strings.EmptyValue.ToSimpleText ();
				submitted = Constants.KeyForNullValue;
			}
			else
			{
				displayed = entity.GetCompactSummary ().ToString ();
				submitted = EntityIO.GetEntityId (dataContext, entity);
			};

			return new Dictionary<object, string> ()
			{
				{ "displayed", displayed },
				{ "submitted", submitted },
			};
		}


		public static object ConvertFromClient(BusinessContext businessContext, DynamicDictionaryValue value, Type type, FieldType? fieldType = null)
		{
			return FieldIO.ConvertFromClient (businessContext.DataContext, value, type, fieldType);
		}


		public static object ConvertFromClient(DataContext dataContext, DynamicDictionaryValue value, Type type, FieldType? fieldType = null)
		{
			switch (fieldType ?? FieldTypeSelector.GetFieldType (type))
			{
				case FieldType.Boolean:
					return FieldIO.ConvertBooleanFromClient (value);

				case FieldType.Date:
					return FieldIO.ConvertDateFromClient (value);

				case FieldType.Decimal:
					return FieldIO.ConvertDecimalFromClient (value);

				case FieldType.Enumeration:
					return FieldIO.ConvertEnumerationFromClient (value, type);

				case FieldType.Integer:
					return FieldIO.ConvertIntegerFromClient (value, type);

				case FieldType.Text:
					return FieldIO.ConvertTextFromClient (value, type);

				case FieldType.EntityCollection:
					return FieldIO.ConvertEntityCollectionFromClient (dataContext, value, type);

				case FieldType.EntityReference:
					return FieldIO.ConvertEntityReferenceFromClient (dataContext, value);

				default:
					throw new NotImplementedException ();
			}
		}


		private static object ConvertBooleanFromClient(DynamicDictionaryValue value)
		{
			var val = FieldIO.ConvertFromNancyValue (value, v => (bool) v);

			if (val == null)
			{
				return null;
			}

			return (bool) val;
		}


		private static object ConvertDateFromClient(DynamicDictionaryValue value)
		{
			var val = FieldIO.ConvertFromNancyValue (value, v => (string) v);
			var stringValue = val == null ? null : (string) val;

			if (string.IsNullOrEmpty (stringValue))
			{
				return null;
			}

			var format = FieldIO.dateFormat;
			var culture = FieldIO.dateCulture;

			return new Date (DateTime.ParseExact (stringValue, format, culture));
		}


		private static object ConvertDecimalFromClient(DynamicDictionaryValue value)
		{
			var val = FieldIO.ConvertFromNancyValue (value, v => (decimal) v);

			if (val == null)
			{
				return null;
			}
			
			return val;
		}


		private static object ConvertEnumerationFromClient(DynamicDictionaryValue value, Type valueType)
		{
			var val = FieldIO.ConvertFromNancyValue (value, v => (string) v);
			var stringValue = val == null ? null : (string) val;

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


		private static object ConvertIntegerFromClient(DynamicDictionaryValue value, Type valueType)
		{
			var val = FieldIO.ConvertFromNancyValue (value, v => (long) v);
			
			if (val == null)
			{
				return null;
			}

			if (valueType == typeof (long?))
			{
				return (long?) val;
			}
			else if (valueType == typeof (long))
			{
				return (long) val;
			}
			else if (valueType == typeof (int?))
			{
				return (int?) (long?) val;
			}
			else if (valueType == typeof (int))
			{
				return (int) (long) val;
			}
			else if (valueType == typeof (short?))
			{
				return (short?) (long?) val;
			}
			else if (valueType == typeof (short))
			{
				return (short) (long) val;
			}
			else
			{
				throw new NotImplementedException ();
			}
		}


		private static object ConvertTextFromClient(DynamicDictionaryValue value, Type valueType)
		{
			var stringValue = (string) FieldIO.ConvertFromNancyValue (value, v => (string) v) ?? "";

			if (valueType == typeof (FormattedText?))
			{
				return value == null
					? (FormattedText?) FormattedText.Empty
					: (FormattedText?) FormattedText.FromSimpleText (stringValue);
			}
			else if (valueType == typeof (FormattedText))
			{
				return FormattedText.FromSimpleText (stringValue);
			}
			else if (valueType == typeof (string))
			{
				return stringValue;
			}
			else
			{
				throw new NotImplementedException ();
			}
		}


		private static object ConvertEntityCollectionFromClient(DataContext dataContext, DynamicDictionaryValue value, Type valueType)
		{
			var listType = typeof (List<>);
			var typeArgument = valueType.GetGenericArguments ()[0];
			var genericListType = listType.MakeGenericType (typeArgument);

			var list = (IList) Activator.CreateInstance (genericListType);

			list.AddRange
			(
				((string) value.Value)
					.Split (';')
					.Select (id => FieldIO.ConvertEntityFromClient (dataContext, id))
			);

			return list;
		}


		private static object ConvertEntityReferenceFromClient(DataContext dataContext, DynamicDictionaryValue value)
		{
			var entityId = (string) value;

			return FieldIO.ConvertEntityFromClient (dataContext, entityId);
		}


		private static object ConvertEntityFromClient(DataContext dataContext, string entityId)
		{
			if (string.IsNullOrEmpty (entityId) || Constants.KeyForNullValue ==  entityId)
			{
				return null;
			}

			return EntityIO.ResolveEntity (dataContext, entityId);
		}


		private static object ConvertFromNancyValue(DynamicDictionaryValue value, Func<DynamicDictionaryValue, object> converter)
		{
			if (!value.HasValue || string.IsNullOrEmpty ((string) value))
			{
				return null;
			}

			return converter (value);
		}


		private static readonly string dateFormat = "dd.MM.yyyy";


		private static readonly CultureInfo dateCulture = CultureInfo.InvariantCulture;


	}


}
