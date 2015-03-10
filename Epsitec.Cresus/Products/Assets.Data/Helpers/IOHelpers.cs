﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data.Helpers
{
	public static class IOHelpers
	{
		#region Encoding
		//	Comme le type natif Windows System.Text.Encoding n'est pas une simple énumération
		//	mais une classe, il faut le sérialiser en passant par une énumération interne
		//	(IOEncoding). Seuls les encodages utilisés dans Assets sont gérés-

		public static System.Text.Encoding ReadEncodingAttribute(this System.Xml.XmlReader reader, string name)
		{
			var e = reader.ReadTypeAttribute<IOEncoding> (name);
			return IOHelpers.EncodingFactory (e);
		}

		public static void WriteEncodingAttribute(this System.Xml.XmlWriter writer, string name, System.Text.Encoding encoding)
		{
			IOEncoding e = IOHelpers.IOEncodings.Where (x => IOHelpers.EncodingFactory (x) == encoding).FirstOrDefault ();
			writer.WriteTypeAttribute (name, e);
		}

		private static System.Text.Encoding EncodingFactory(IOEncoding e)
		{
			switch (e)
			{
				case IOEncoding.UTF7:
					return System.Text.Encoding.UTF7;

				case IOEncoding.UTF8:
					return System.Text.Encoding.UTF8;

				case IOEncoding.UTF32:
					return System.Text.Encoding.UTF32;

				case IOEncoding.Unicode:
					return System.Text.Encoding.Unicode;

				case IOEncoding.BigEndianUnicode:
					return System.Text.Encoding.BigEndianUnicode;

				case IOEncoding.ASCII:
					return System.Text.Encoding.ASCII;

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid encoding {0}", e));
			}
		}

		private static IEnumerable<IOEncoding> IOEncodings
		{
			get
			{
				yield return IOEncoding.UTF7;
				yield return IOEncoding.UTF8;
				yield return IOEncoding.UTF32;
				yield return IOEncoding.Unicode;
				yield return IOEncoding.BigEndianUnicode;
				yield return IOEncoding.ASCII;
			}
		}

		private enum IOEncoding
		{
			UTF7,
			UTF8,
			UTF32,
			Unicode,
			BigEndianUnicode,
			ASCII,
		}
		#endregion


		#region ObjectField
		//	Afin d'être robuste, le type ObjectField doit être sérialisé spécialement.
		//	Si rien n'était fait, le type ObjectField.UserFieldFirst+10 (par exemple)
		//	serait sérialisé sous la forme "20010". Avec la méthode employée, il est
		//	sérailisé avec "UserField+10".

		public static ObjectField ReadObjectFieldAttribute(this System.Xml.XmlReader reader, string name)
		{
			var s = reader[name];

			if (string.IsNullOrEmpty (s))
			{
				return ObjectField.Unknown;
			}
			else
			{
				return IOHelpers.ParseObjectField (s);
			}
		}

		public static void WriteObjectFieldAttribute(this System.Xml.XmlWriter writer, string name, ObjectField value)
		{
			writer.WriteAttributeString (name, value.ToStringIO ());
		}


		public static string ToStringIO(this ObjectField value)
		{
			if (value >= ObjectField.GroupGuidRatioFirst &&
				value <= ObjectField.GroupGuidRatioLast)
			{
				return "GroupGuidRatio+" + (value-ObjectField.GroupGuidRatioFirst).ToStringIO ();
			}
			else if (value >= ObjectField.ArgumentFirst &&
					 value <= ObjectField.ArgumentLast)
			{
				return "Argument+" + (value-ObjectField.ArgumentFirst).ToStringIO ();
			}
			else if (value >= ObjectField.UserFieldFirst &&
					 value <= ObjectField.UserFieldLast)
			{
				return "UserField+" + (value-ObjectField.UserFieldFirst).ToStringIO ();
			}
			else if (value >= ObjectField.MCH2Report)
			{
				return "MCH2Report+" + (value-ObjectField.MCH2Report).ToStringIO ();
			}
			else
			{
				return value.ToString ();
			}
		}

		public static ObjectField ParseObjectField(this string s)
		{
			if (s.StartsWith ("GroupGuidRatio+"))
			{
				return ObjectField.GroupGuidRatioFirst + s.Substring (15).ParseInt ();
			}
			else if (s.StartsWith ("Argument+"))
			{
				return ObjectField.ArgumentFirst + s.Substring (9).ParseInt ();
			}
			else if (s.StartsWith ("UserField+"))
			{
				return ObjectField.UserFieldFirst + s.Substring (10).ParseInt ();
			}
			else if (s.StartsWith ("MCH2Report+"))
			{
				return ObjectField.MCH2Report + s.Substring (11).ParseInt ();
			}
			else
			{
				return (ObjectField) System.Enum.Parse (typeof (ObjectField), s);
			}
		}
		#endregion


		#region Type
		public static T ReadTypeAttribute<T>(this System.Xml.XmlReader reader, string name)
		{
			var s = reader[name];

			if (string.IsNullOrEmpty (s))
			{
				throw new System.ArgumentException ("Enum value cannot be empty.");
			}
			else
			{
				return IOHelpers.ParseType<T> (s);
			}
		}

		public static void WriteTypeAttribute<T>(this System.Xml.XmlWriter writer, string name, T value)
		{
			writer.WriteAttributeString (name, value.ToStringIO ());
		}


		public static string ToStringIO<T>(this T value)
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", value);
		}

		public static T ParseType<T>(this string s)
		{
			return (T) System.Enum.Parse (typeof (T), s);
		}
		#endregion


		#region Guid
		public static Guid ReadGuidAttribute(this System.Xml.XmlReader reader, string name)
		{
			var s = reader[name];

			if (string.IsNullOrEmpty (s))
			{
				return Guid.Empty;
			}
			else
			{
				return s.ParseGuid ();
			}
		}

		public static void WriteGuidAttribute(this System.Xml.XmlWriter writer, string name, Guid value)
		{
			if (!value.IsEmpty)
			{
				writer.WriteAttributeString (name, value.ToStringIO ());
			}
		}


		public static string ToStringIO(this Guid guid)
		{
			return guid.ToString ();
		}

		public static Guid ParseGuid(this string s)
		{
			return Guid.Parse (s);
		}
		#endregion


		#region Range
		public static DateRange ReadDateRangeAttribute(this System.Xml.XmlReader reader, string name)
		{
			var includeFrom = reader.ReadDateAttribute (string.Format (X.Attr.FormatedIncludeFrom, name));
			var excludeTo   = reader.ReadDateAttribute (string.Format (X.Attr.FormatedExcludeTo,   name));

			if (includeFrom.HasValue && excludeTo.HasValue)
			{
				return new DateRange (includeFrom.Value, excludeTo.Value);
			}
			else
			{
				return DateRange.Empty;
			}
		}

		public static void WriteDateRangeAttribute(this System.Xml.XmlWriter writer, string name, DateRange value)
		{
			if (!value.IsEmpty)
			{
				writer.WriteDateAttribute (string.Format (X.Attr.FormatedIncludeFrom, name), value.IncludeFrom);
				writer.WriteDateAttribute (string.Format (X.Attr.FormatedExcludeTo,   name),   value.ExcludeTo);
			}
		}
		#endregion


		#region Date
		public static System.DateTime? ReadDateAttribute(this System.Xml.XmlReader reader, string name)
		{
			var s = reader[name];

			if (string.IsNullOrEmpty (s))
			{
				return null;
			}
			else
			{
				return s.ParseDate ();
			}
		}

		public static void WriteDateAttribute(this System.Xml.XmlWriter writer, string name, System.DateTime? value)
		{
			if (value.HasValue)
			{
				writer.WriteAttributeString (name, value.Value.ToStringIO ());
			}
		}


		public static string ToStringIO(this System.DateTime date)
		{
			//?return date.ToString ("yyyy-MM-ddTHH:mm:ss");
			return date.ToString ("yyyy-MM-dd");
		}

		public static System.DateTime ParseDate(this string s)
		{
			//?return System.DateTime.ParseExact (s, "yyyy-MM-ddTHH:mm:ss", null);
			return System.DateTime.ParseExact (s, "yyyy-MM-dd", null);
		}
		#endregion


		#region Decimal
		public static decimal? ReadDecimalAttribute(this System.Xml.XmlReader reader, string name)
		{
			var s = reader[name];

			if (string.IsNullOrEmpty (s))
			{
				return null;
			}
			else
			{
				return IOHelpers.ParseDecimal (s);
			}
		}

		public static void WriteDecimalAttribute(this System.Xml.XmlWriter writer, string name, decimal? value)
		{
			if (value.HasValue)
			{
				writer.WriteAttributeString (name, value.Value.ToStringIO ());
			}
		}


		public static string ToStringIO(this decimal value)
		{
			return value.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}

		public static decimal ParseDecimal(this string s)
		{
			return decimal.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
		}
		#endregion


		#region Int
		public static int? ReadIntAttribute(this System.Xml.XmlReader reader, string name)
		{
			var s = reader[name];

			if (string.IsNullOrEmpty (s))
			{
				return null;
			}
			else
			{
				return IOHelpers.ParseInt (s);
			}
		}

		public static void WriteIntAttribute(this System.Xml.XmlWriter writer, string name, int? value)
		{
			if (value.HasValue)
			{
				writer.WriteAttributeString (name, value.Value.ToStringIO ());
			}
		}


		public static string ToStringIO(this int value)
		{
			return value.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}

		public static int ParseInt(this string s)
		{
			return int.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
		}
		#endregion


		#region Bool
		public static bool ReadBoolAttribute(this System.Xml.XmlReader reader, string name)
		{
			var s = reader[name];

			if (string.IsNullOrEmpty (s))
			{
				return false;
			}
			else
			{
				return IOHelpers.ParseBool (s);
			}
		}

		public static void WriteBoolAttribute(this System.Xml.XmlWriter writer, string name, bool value)
		{
			writer.WriteAttributeString (name, value.ToStringIO ());
		}


		public static string ToStringIO(this bool value)
		{
			return value.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}

		public static bool ParseBool(this string s)
		{
			return bool.Parse (s);
		}
		#endregion


		#region String
		public static string ReadStringAttribute(this System.Xml.XmlReader reader, string name)
		{
			return reader[name];
		}

		public static void WriteStringAttribute(this System.Xml.XmlWriter writer, string name, string value)
		{
			writer.WriteAttributeString (name, value);
		}
		#endregion


		public const string Extension = ".crassets";
	}
}
