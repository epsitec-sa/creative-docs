//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Helpers
{
	public static class IOHelpers
	{
		#region Encoding
		//	Comme le type natif Windows System.Text.Encoding n'est pas une simple énumération
		//	mais une classe, il faut le sérialiser en passant par une énumération interne
		//	(IOEncoding). Seuls les encodages utilisés dans Assets sont gérés-

		public static System.Text.Encoding ReadEncodingAttribute(System.Xml.XmlReader reader, string name)
		{
			var e = (IOEncoding) IOHelpers.ReadTypeAttribute (reader, name, typeof (IOEncoding));
			return IOHelpers.EncodingFactory (e);
		}

		public static void WriteEncodingAttribute(System.Xml.XmlWriter writer, string name, System.Text.Encoding encoding)
		{
			IOEncoding e = IOHelpers.IOEncodings.Where (x => IOHelpers.EncodingFactory (x) == encoding).FirstOrDefault ();
			IOHelpers.WriteTypeAttribute (writer, name, e);
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

		public static ObjectField ReadObjectFieldAttribute(System.Xml.XmlReader reader, string name)
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

		public static void WriteObjectFieldAttribute(System.Xml.XmlWriter writer, string name, ObjectField value)
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
		public static object ReadTypeAttribute(System.Xml.XmlReader reader, string name, System.Type enumType)
		{
			var s = reader[name];

			if (string.IsNullOrEmpty (s))
			{
				return null;
			}
			else
			{
				return IOHelpers.ParseType (s, enumType);
			}
		}

		public static void WriteTypeAttribute(System.Xml.XmlWriter writer, string name, object value)
		{
			writer.WriteAttributeString (name, value.ToStringIO ());
		}


		public static string ToStringIO(this object value)
		{
			return value.ToString ();
		}

		public static object ParseType(this string s, System.Type enumType)
		{
			return System.Enum.Parse (enumType, s);
		}
		#endregion


		#region Guid
		public static Guid ReadGuidAttribute(System.Xml.XmlReader reader, string name)
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

		public static void WriteGuidAttribute(System.Xml.XmlWriter writer, string name, Guid value)
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


		#region Date
		public static System.DateTime? ReadDateAttribute(System.Xml.XmlReader reader, string name)
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

		public static void WriteDateAttribute(System.Xml.XmlWriter writer, string name, System.DateTime? value)
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
		public static decimal? ReadDecimalAttribute(System.Xml.XmlReader reader, string name)
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

		public static void WriteDecimalAttribute(System.Xml.XmlWriter writer, string name, decimal? value)
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
		public static int? ReadIntAttribute(System.Xml.XmlReader reader, string name)
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

		public static void WriteIntAttribute(System.Xml.XmlWriter writer, string name, int? value)
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
		public static bool ReadBoolAttribute(System.Xml.XmlReader reader, string name)
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

		public static void WriteBoolAttribute(System.Xml.XmlWriter writer, string name, bool value)
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
		public static string ReadStringAttribute(System.Xml.XmlReader reader, string name)
		{
			return reader[name];
		}

		public static void WriteStringAttribute(System.Xml.XmlWriter writer, string name, string value)
		{
			writer.WriteAttributeString (name, value);
		}
		#endregion


		public const string Extension = ".crassets";
	}
}
