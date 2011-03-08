//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	using OpCodes = System.Reflection.Emit.OpCodes;
	using InvariantConverter = Epsitec.Common.Types.InvariantConverter;
	
	/// <summary>
	/// The <c>DbTools</c> class provides a set of useful methods.
	/// </summary>
	public static class DbTools
	{
		/// <summary>
		/// Writes the XML attribute to the XML writer if the value is not empty.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		/// <param name="name">The attribute name.</param>
		/// <param name="value">The attribute value.</param>
		public static void WriteAttribute(System.Xml.XmlTextWriter xmlWriter, string name, string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				//	Nothing to do; an empty attribute will never be written to
				//	the XML output.
			}
			else
			{
				xmlWriter.WriteAttributeString (name, value);
			}
		}

		/// <summary>
		/// Makes a composite name based on a list of strings. The individual
		/// names are joined using the "_" character. No trailing "_" will be
		/// generated.
		/// </summary>
		/// <param name="list">The list of strings.</param>
		/// <returns>The composite name.</returns>
		public static string MakeCompositeName(params string[] list)
		{
			int num = list.Length;
			
			while ((num > 0) && (string.IsNullOrEmpty (list[num-1])))
			{
				num--;
			}
			
			return (num == 0) ? "" : string.Join ("_", list, 0, num);
		}

		/// <summary>
		/// Parses the element category.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The value or <c>DbElementCat.Unknown</c> if the text is empty.</returns>
		public static DbElementCat ParseElementCategory(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return DbElementCat.Unknown;
			}
			else
			{
				return (DbElementCat) InvariantConverter.ParseInt (text);
			}
		}

		/// <summary>
		/// Parses the column class.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The value or <c>DbColumnClass.Data</c> if the text is empty.</returns>
		public static DbColumnClass ParseColumnClass(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return DbColumnClass.Data;
			}
			else
			{
				return (DbColumnClass) InvariantConverter.ParseInt (text);
			}
		}

		public static DbCardinality ParseCardinality(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return DbCardinality.None;
			}
			else
			{
				return (DbCardinality) InvariantConverter.ParseInt (text);
			}
		}

		/// <summary>
		/// Parses the type of the raw.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>
		/// The value or <c>DbRawType.Unknown</c> if the text is empty.
		/// </returns>
		public static DbRawType ParseRawType(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return DbRawType.Unknown;
			}

			int num;
			InvariantConverter.Convert (text, out num);
			return (DbRawType) num;
		}

		/// <summary>
		/// Parses the simple type.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>
		/// The value or <c>DbSimpleType.Unknown</c> if the text is empty.
		/// </returns>
		public static DbSimpleType ParseSimpleType(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return DbSimpleType.Unknown;
			}

			int num;
			InvariantConverter.Convert (text, out num);
			return (DbSimpleType) num;
		}

		/// <summary>
		/// Parses the string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The value or <c>null</c> if the text is empty.</returns>
		public static string ParseString(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}
			else
			{
				return text;
			}
		}

		/// <summary>
		/// Parses the int.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The value or <c>0</c> if the text is empty.</returns>
		public static int ParseInt(string text)
		{
			return InvariantConverter.ParseInt (text);
		}

		/// <summary>
		/// Parses the long.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The value or <c>0</c> if the text is empty.</returns>
		public static long ParseLong(string text)
		{
			return InvariantConverter.ParseLong (text);
		}

		/// <summary>
		/// Parses the decimal.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The value or <c>0</c> if the text is empty.</returns>
		public static decimal ParseDecimal(string text)
		{
			return InvariantConverter.ParseDecimal (text);
		}

		/// <summary>
		/// Parses the druid.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The value or <c>Druid.Empty</c> if the text is empty.</returns>
		public static Druid ParseDruid(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return Druid.Empty;
			}
			else
			{
				return Druid.Parse (text);
			}
		}

		/// <summary>
		/// Parses the boolean defaulting to <c>false</c>.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The value or <c>false</c> if the text is empty.</returns>
		public static bool ParseDefaultingToFalseBool(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return false;
			}
			else
			{
				System.Diagnostics.Debug.Assert (text == "Y");

				return true;
			}
		}

		/// <summary>
		/// Parses the boolean defaulting to <c>true</c>.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The value or <c>true</c> if the text is empty.</returns>
		public static bool ParseDefaultingToTrueBool(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return false;
			}
			else
			{
				System.Diagnostics.Debug.Assert (text == "N");

				return true;
			}
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is the default value.</returns>
		public static string ElementCategoryToString(DbElementCat value)
		{
			if (value == DbElementCat.Unknown)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) value);
			}
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is the default value.</returns>
		public static string ColumnClassToString(DbColumnClass value)
		{
			if (value == DbColumnClass.Data)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) value);
			}
		}

		public static string CardinalityToString(DbCardinality value)
		{
			if (value == DbCardinality.None)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) value);
			}
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is <c>Druid.Empty</c>.</returns>
		public static string DruidToString(Druid value)
		{
			if (value.IsEmpty)
			{
				return null;
			}
			else
			{
				return value.ToString ();
			}
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is a <c>null</c> type.</returns>
		public static string TypeToString(INamedType value)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				return value.CaptionId.ToString ();
			}
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is the default value.</returns>
		public static string RawTypeToString(DbRawType value)
		{
			if (value == DbRawType.Unknown)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) value);
			}
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is the default value.</returns>
		public static string SimpleTypeToString(DbSimpleType value)
		{
			if (value == DbSimpleType.Unknown)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString ((int) value);
			}
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is the zero value.</returns>
		public static string IntToString(int value)
		{
			if (value == 0)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString (value);
			}
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is the zero value.</returns>
		public static string LongToString(long value)
		{
			if (value == 0)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString (value);
			}
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is the zero value.</returns>
		public static string DecimalToString(decimal value)
		{
			if (value == 0)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ToString (value);
			}
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is the <c>false</c> value.</returns>
		public static string BoolDefaultingToFalseToString(bool value)
		{
			return value ? "Y" : null;
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is the <c>true</c> value.</returns>
		public static string BoolDefaultingToTrueToString(bool value)
		{
			return value ? "N" : null;
		}

		/// <summary>
		/// Converts the value to a string.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The textual representation of the value or <c>null</c> if it is the empty string.</returns>
		public static string StringToString(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Gets the compact XML representation of a serializable value.
		/// </summary>
		/// <param name="value">The serializable value.</param>
		/// <returns>The compact XML representation.</returns>
		public static string GetCompactXml(IXmlSerializable value)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter writer = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (writer);

			value.Serialize (xmlWriter);

			xmlWriter.Flush ();
			xmlWriter.Close ();

			return buffer.ToString ();
		}

		/// <summary>
		/// Deserializes an object from XML.
		/// </summary>
		/// <param name="value">The source XML.</param>
		/// <returns>The deserialized object.</returns>
		public static T DeserializeFromXml<T>(string value)
		{
			using (System.IO.StringReader reader = new System.IO.StringReader (value))
			{
				using (System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (reader))
				{
					if (xmlReader.Read ())
					{
						return DbTools.DeserializeFromXml<T> (xmlReader);
					}
					else
					{
						throw new System.ArgumentException ("Invalid XML for deserialization", "value");
					}
				}
			}
		}

		/// <summary>
		/// Deserializes an object from XML.
		/// </summary>
		/// <param name="xmlReader">The XML reader.</param>
		/// <returns>The deserialized object.</returns>
		public static T DeserializeFromXml<T>(System.Xml.XmlTextReader xmlReader)
		{
			Deserializer<T> deserializer;
			
			lock (DbTools.exclusion)
			{
				object method;
				DbTools.deserializers.TryGetValue (typeof (T), out method);

				if (method == DbTools.noDeserializer)
				{
					throw new System.ArgumentException (string.Format ("No deserializer for type {0}", typeof (T).Name));
				}
				
				deserializer = method as Deserializer<T>;
			}

			if (deserializer == null)
			{
				deserializer = DbTools.CreateDeserializer<T> ();
				
				lock (DbTools.exclusion)
				{
					if (deserializer == null)
					{
						DbTools.deserializers[typeof (T)] = DbTools.noDeserializer;
						
						throw new System.ArgumentException (string.Format ("No deserializer for type {0}", typeof (T).Name));
					}
					else
					{
						DbTools.deserializers[typeof (T)] = deserializer;
					}
				}
			}

			return deserializer (xmlReader);
		}
		
		#region Private Methods

		/// <summary>
		/// Creates a deserializer for a given type. This builds a small piece
		/// of dynamic code to handle the deserialization.
		/// </summary>
		/// <returns>The deserializer delegate.</returns>
		private static Deserializer<T> CreateDeserializer<T>()
		{
			System.Type type = typeof (T);
			System.Type xmlType   = typeof (System.Xml.XmlTextReader);
			System.Type[] arguments = new System.Type[] { xmlType };
			
			System.Reflection.MethodInfo method = type.GetMethod ("Deserialize", arguments);

			if ((method == null) ||
				(method.IsStatic == false) ||
				(method.ReturnType != type))
			{
				return null;
			}

			string name = string.Concat ("DynamicCode_XmlDeserializer_", type.Name);

			System.Reflection.Emit.DynamicMethod dynamic;
			dynamic = new System.Reflection.Emit.DynamicMethod (name, type, arguments, type);

			System.Reflection.Emit.ILGenerator generator = dynamic.GetILGenerator ();

			generator.Emit (OpCodes.Ldarg_0);
			generator.EmitCall (OpCodes.Call, method, null);
			generator.Emit (OpCodes.Ret);

			return (Deserializer<T>) dynamic.CreateDelegate (typeof (Deserializer<T>));
		}

		private delegate T Deserializer<T>(System.Xml.XmlTextReader xmlReader);

		#endregion

		static object							exclusion      = new object ();
		static Dictionary<System.Type, object>	deserializers  = new Dictionary<System.Type, object> ();
		static object							noDeserializer = new object ();
	}
}
