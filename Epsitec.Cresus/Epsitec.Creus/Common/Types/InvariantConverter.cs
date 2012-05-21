//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe InvariantConverter permet de convertir des données simples entre
	/// elles.
	/// </summary>
	public static class InvariantConverter
	{
		static InvariantConverter()
		{
			InvariantConverter.OverrideSerializationConverter (typeof (byte[]), new Converters.ByteArrayConverter ());
		}

		public static ISerializationConverter GetSerializationConverter(System.Type type)
		{
			if (type == null)
			{
				return null;
			}


			ISerializationConverter converter;
			
			if (InvariantConverter.typeConverters.TryGetValue (type, out converter))
			{
				return converter;
			}

			lock (InvariantConverter.typeConverters)
			{
				if (InvariantConverter.typeConverters.TryGetValue (type, out converter))
				{
					return converter;
				}

				object[] attributes = type.GetCustomAttributes (typeof (SerializationConverterAttribute), false);

				if (attributes.Length > 0)
				{
					//	The type specifies a dedicated ISerializationConverter which must
					//	be used to convert to/from strings.
					
					SerializationConverterAttribute attribute = attributes[0] as SerializationConverterAttribute;
					converter = attribute.Converter;
				}
				else
				{
					//	There is no dedicated ISerializationConverter. Instead, use the type
					//	converter provided by the ComponentModel infrastructure.
					
					converter = new GenericSerializationConverterAdaptor (System.ComponentModel.TypeDescriptor.GetConverter (type));
				}
				
				InvariantConverter.typeConverters[type] = converter;
				return converter;
			}
		}
		
		public static void OverrideSerializationConverter(System.Type type, ISerializationConverter converter)
		{
			InvariantConverter.typeConverters[type] = converter;
		}

		public static string ConvertToString<T>(T value)
		{
			return (string) Converters.AutomaticValueConverter.Instance.Convert (value, typeof (string), null, System.Globalization.CultureInfo.InvariantCulture);
		}
		
		public static T ConvertFromString<T>(string value)
		{
			return (T) Converters.AutomaticValueConverter.Instance.ConvertBack (value, typeof (T), null, System.Globalization.CultureInfo.InvariantCulture);
		}

		#region GenericSerializationConverter Class

		/// <summary>
		/// The GenericSerializationConverterAdaptor wraps the type converter provided by
		/// the ComponentModel services to our own ISerializationConverter infrastructure.
		/// </summary>
		
		private class GenericSerializationConverterAdaptor : ISerializationConverter
		{
			public GenericSerializationConverterAdaptor(System.ComponentModel.TypeConverter converter)
			{
				this.converter = converter;
			}

			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				return this.converter.ConvertToInvariantString (value);
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				return this.converter.ConvertFromInvariantString (value);
			}

			#endregion

			private System.ComponentModel.TypeConverter converter;
		}
		
		#endregion

		public static bool IsNull(object obj)
		{
			return (obj == null) || (obj == System.DBNull.Value);
		}
		public static bool IsNotNull(object obj)
		{
			return (obj != null) && !(obj == System.DBNull.Value);
		}
		public static bool IsSimple(object obj)
		{
			//	Considère comme simple les cas suivants:
			//
			//	- obj est nul
			//	- obj est une valeur (ValueType)
			//	- obj est une chaîne de caractères
			
			if ((obj == null) ||
				(obj == System.DBNull.Value) ||
				(obj is string) ||
				(obj is System.ValueType))
			{
				return true;
			}
			
			return false;
		}
		
		public static string  ToString(object obj)
		{
			string value;
			if (InvariantConverter.Convert (obj, out value))
			{
				return value;
			}
			else
			{
				throw new System.ArgumentException ("Value cannot be converted", "obj");
			}
		}
		public static double  ToDouble(object obj)
		{
			decimal value;
			if (InvariantConverter.Convert (obj, out value))
			{
				return (double) value;
			}
			else
			{
				throw new System.ArgumentException ("Value cannot be converted", "obj");
			}
		}
		public static int     ToInt(object obj)
		{
			int value;

			if (InvariantConverter.Convert (obj, out value))
			{
				return value;
			}
			else
			{
				throw new System.ArgumentException ("Value cannot be converted", "obj");
			}
		}
		public static short   ToShort(object obj)
		{
			short value;

			if (InvariantConverter.Convert (obj, out value))
			{
				return value;
			}
			else
			{
				throw new System.ArgumentException ("Value cannot be converted", "obj");
			}
		}
		public static long    ToLong(object obj)
		{
			long value;

			if (InvariantConverter.Convert (obj, out value))
			{
				return value;
			}
			else
			{
				throw new System.ArgumentException ("Value cannot be converted", "obj");
			}
		}
		public static decimal ToDecimal(object obj)
		{
			decimal value;

			if (InvariantConverter.Convert (obj, out value))
			{
				return value;
			}
			else
			{
				throw new System.ArgumentException ("Value cannot be converted", "obj");
			}
		}

		public static System.DateTime ToDateTime(object obj)
		{
			System.DateTime value;

			if (InvariantConverter.Convert (obj, out value))
			{
				return value;
			}
			else
			{
				throw new System.ArgumentException ("Value cannot be converted", "obj");
			}
		}

		/// <summary>
		/// Parses the integer number and stops as soon as a non digit character is encountered.
		/// Minus sign and dash are treated as equivalents and can appear any number of times
		/// before the first digit. This method never throws an exception.
		/// </summary>
		/// <param name="value">The string containing a numeric value.</param>
		/// <param name="startIndex">The start index.</param>
		/// <returns>
		/// The number or <c>0</c> if the value is not a number.
		/// </returns>
		public static int ParseInt(string value, int startIndex = 0)
		{
			if ((value == null) ||
				(startIndex >= value.Length))
			{
				return 0;
			}

			int  num = 0;
			bool neg = false;
			bool ok  = false;
			int  len = value.Length;

			for (int i = startIndex; i < len; i++)
			{
				char c = value[i];

				switch (c)
				{
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						num = num * 10 + c - '0';
						ok  = true;
						break;

					case '-':		//	dash
					case '−':		//	minus sign (2212)
						if (ok)
						{
							goto exit;
						}
						neg = !neg;
						break;

					case '+':
						if (ok)
						{
							goto exit;
						}
						neg = false;
						break;

					default:
						goto exit;
				}
			}

		exit:
			return neg ? -num : num;
		}

		/// <summary>
		/// Parses the integer number (just like the non-generic version of this method), and
		/// casts the result to the specified type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The enumeration type.</typeparam>
		/// <param name="value">The string containing a numeric value.</param>
		/// <param name="startIndex">The start index.</param>
		/// <returns></returns>
		public static T ParseInt<T>(string value, int startIndex = 0)
			where T : struct
		{
			return string.IsNullOrEmpty (value)
				? default(T)
				: (T) (System.Enum.ToObject (typeof (T), InvariantConverter.ParseInt (value, startIndex)));
		}

		public static long ParseLong(string value)
		{
			//	TODO: change to use same logic as ParseInt
			return string.IsNullOrEmpty (value) ? 0 : long.Parse (value, System.Globalization.CultureInfo.InvariantCulture);
		}
		
		public static decimal ParseDecimal(string value)
		{
			//	TODO: change to use same logic as ParseInt
			return string.IsNullOrEmpty (value) ? 0 : decimal.Parse (value, System.Globalization.CultureInfo.InvariantCulture);
		}

		
		public static bool SafeConvert(object obj, out string value)
		{
			try
			{
				return InvariantConverter.Convert (obj, out value);
			}
			catch
			{
				value = null;
				return false;
			}
		}
		public static bool SafeConvert(object obj, out bool value)
		{
			try
			{
				return InvariantConverter.Convert (obj, out value);
			}
			catch (System.ArgumentException)
			{
				value = false;
				return false;
			}
			catch (System.FormatException)
			{
				value = false;
				return false;
			}
		}
		public static bool SafeConvert(object obj, out int value)
		{
			try
			{
				return InvariantConverter.Convert (obj, out value);
			}
			catch (System.ArgumentException)
			{
				value = 0;
				return false;
			}
			catch (System.FormatException)
			{
				value = 0;
				return false;
			}
		}
		public static bool SafeConvert(object obj, out long value)
		{
			try
			{
				return InvariantConverter.Convert (obj, out value);
			}
			catch (System.ArgumentException)
			{
				value = 0;
				return false;
			}
			catch (System.FormatException)
			{
				value = 0;
				return false;
			}
		}
		public static bool SafeConvert(object obj, out decimal value)
		{
			try
			{
				return InvariantConverter.Convert (obj, out value);
			}
			catch (System.ArgumentException)
			{
				value = 0;
				return false;
			}
			catch (System.FormatException)
			{
				value = 0;
				return false;
			}
		}
		public static bool SafeConvert(object obj, System.Type type, out System.Enum value)
		{
			try
			{
				return InvariantConverter.Convert (obj, type, out value);
			}
			catch (System.ArgumentException)
			{
				value = null;
				return false;
			}
			catch (System.FormatException)
			{
				value = null;
				return false;
			}
		}
		
		public static bool Convert(object obj, out string value)
		{
			if ((obj == null) || (obj == System.DBNull.Value))
			{
				value = null;
				return false;
			}
			if (obj is string)
			{
				value = obj as string;
				return true;
			}
			
			if (obj is System.ValueType)
			{
				System.TypeCode code = System.Convert.GetTypeCode (obj);
				
#if false
				long num;
#endif
				
				switch (code)
				{
					case System.TypeCode.Boolean:	value = ((bool)   obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Decimal:	value = ((decimal)obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Single:	value = ((float)  obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Double:	value = ((double) obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Byte:		value = ((byte)   obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Int16:		value = ((short)  obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Int32:		value = ((int)    obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Int64:		value = ((long)   obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					
					case System.TypeCode.DateTime:
						value = ((System.DateTime)obj).ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss.fffffffZ", System.Globalization.CultureInfo.InvariantCulture);
						return true;
				}
			}
			
			System.ComponentModel.TypeConverter conv = System.ComponentModel.TypeDescriptor.GetConverter (obj);
			System.Diagnostics.Debug.Assert (conv != null);
			
			value = conv.ConvertToInvariantString (obj);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Warning: type {0} used {1} converter. Value={2}.", obj.GetType ().FullName, conv.GetType ().FullName, value));
			
			return true;
		}
		public static bool Convert(object obj, out bool value)
		{
			decimal valueDecimal;
			bool ok = InvariantConverter.Convert (obj, out valueDecimal);
			value = (valueDecimal == 0) ? false : true;
			return ok;
		}
		public static bool Convert(object obj, out int value)
		{
			decimal valueDecimal;
			bool ok = InvariantConverter.Convert (obj, out valueDecimal);
			value = (int) valueDecimal;
			return ok;
		}
		public static bool Convert(object obj, out long value)
		{
			decimal valueDecimal;
			bool ok = InvariantConverter.Convert (obj, out valueDecimal);
			value = (long) valueDecimal;
			return ok;
		}
		public static bool Convert(object obj, out short value)
		{
			decimal valueDecimal;
			bool ok = InvariantConverter.Convert (obj, out valueDecimal);
			value = (short) valueDecimal;
			return ok;
		}
		public static bool Convert(object obj, out decimal value)
		{
			//	Retourne true si la valeur de 'obj' n'est pas 'null' ou une
			//	de ses variantes, false sinon. Si le type n'est pas reconnu ou
			//	que la syntaxe n'est pas correcte, une exception est levée.
			
			if ((obj == null) || (obj == System.DBNull.Value))
			{
				value = 0;
				return false;
			}
			
			string text = obj as string;
			
			if (text != null)
			{
				//	On va devoir faire un "Parse" coûteux... On pourrait bien sûr aussi utiliser TypeConverter,
				//	mais ça ne nous apporterait rien ici.
				
				text = text.Trim ();
				
				if (text == "")
				{
					value = 0;
					return false;
				}
				
				switch (text.ToLower (System.Globalization.CultureInfo.InvariantCulture))
				{
					case "false":
					case "off":
					case "no":
						value = 0;
						return true;
					
					case "true":
					case "on":
					case "yes":
						value = 1;
						return true;
				}

				text = text.Replace ((char) Epsitec.Common.Text.Unicode.Code.MinusSign, '-');
				
				value = System.Decimal.Parse (text, System.Globalization.CultureInfo.InvariantCulture);
				return true;
			}
			
			if (obj is System.ValueType)
			{
				System.TypeCode code = System.Convert.GetTypeCode (obj);
				
				switch (code)
				{
					case System.TypeCode.Boolean:	value = (decimal) ((bool) obj ? 1 : 0);	return true;
					
					case System.TypeCode.Decimal:	value = (decimal) (decimal) obj;		return true;
					
					case System.TypeCode.Single:	value = (decimal) (float) obj;			return true;
					case System.TypeCode.Double:	value = (decimal) (double) obj;			return true;
					
					case System.TypeCode.Byte:		value = (decimal) (byte) obj;			return true;
					case System.TypeCode.Int16:		value = (decimal) (short) obj;			return true;
					case System.TypeCode.Int32:		value = (decimal) (int) obj;			return true;
					case System.TypeCode.Int64:		value = (decimal) (long) obj;			return true;
					
					case System.TypeCode.DBNull:	value = 0;								return false;
				}
			}
			
			throw new System.NotSupportedException (string.Format ("Type {0}: conversion not supported.", obj.GetType ().Name));
		}
		public static bool Convert(object obj, System.Type type, out System.Enum value)
		{
			if ((obj == null) ||
				(obj == System.DBNull.Value))
			{
				value = null;
				return false;
			}
			
			if (obj.GetType () == type)
			{
				value = (System.Enum) obj;
				return true;
			}

			string name = obj as string;

			if (name == null)
			{
				name = string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", obj);
			}

			object item;
			
			if (InvariantConverter.TryParseEnum (type, name, out item))
			{
				value = (System.Enum) item;
				return true;
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Could not convert value '{0}' to type {1}.", obj, type.Name));
			
			value = InvariantConverter.CreateDefaultEnumValue (type);
			return false;
		}
		public static bool Convert(object obj, out System.DateTime value)
		{
			if ((obj == null) || (obj == System.DBNull.Value))
			{
				value = new System.DateTime ();
				return false;
			}
			
			System.TypeCode code = System.Convert.GetTypeCode (obj);
			
			if (code == System.TypeCode.DateTime)
			{
				value = (System.DateTime) obj;
				return true;
			}
			if (code == System.TypeCode.String)
			{
				string text = obj as string;
				
				if (text.Length == 0)
				{
					value = new System.DateTime ();
					return false;
				}
				
				value = System.DateTime.Parse (text, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal);
				return true;
			}
			
			long num;

			if (InvariantConverter.Convert (obj, out num))
			{
				value = new System.DateTime (num);
				return true;
			}
			
			
			throw new System.NotSupportedException (string.Format ("Type {0}: conversion not supported.", obj.GetType ().Name));
		}

		
		public static bool Convert(object obj, System.Type type, out object value)
		{
			if (obj == null)
			{
				value = null;
				
				if (type.IsValueType)
				{
					//	On ne peut pas convertir 'null' en une valeur...
					
					return false;
				}
				
				return true;
			}
			
			if (obj.GetType () == type)
			{
				//	Le type est déjà correct, on n'a donc rien à faire :
				
				value = obj;
				return true;
			}
			
			if (type.IsValueType)
			{
				if (type == typeof (bool))
				{
					bool result;
					
					if (InvariantConverter.Convert (obj, out result))
					{
						value = result;
						return true;
					}
				}
				else if (type == typeof (short))
				{
					short result;

					if (InvariantConverter.Convert (obj, out result))
					{
						value = result;
						return true;
					}
				}
				else if (type == typeof (int))
				{
					int result;
					
					if (InvariantConverter.Convert (obj, out result))
					{
						value = result;
						return true;
					}
				}
				else if (type == typeof (long))
				{
					long result;
					
					if (InvariantConverter.Convert (obj, out result))
					{
						value = result;
						return true;
					}
				}
				else if (type == typeof (decimal))
				{
					decimal result;
					
					if (InvariantConverter.Convert (obj, out result))
					{
						value = result;
						return true;
					}
				}
				else if (type == typeof (System.DateTime))
				{
					System.DateTime result;
					
					if (InvariantConverter.Convert (obj, out result))
					{
						value = result;
						return true;
					}
				}
				else if (type.IsEnum)
				{
					System.Enum result;
					if (InvariantConverter.Convert (obj, type, out result))
					{
						value = result;
						return true;
					}
				}
			}
			else
			{
				if (type == typeof (string))
				{
					string result;
					
					if (InvariantConverter.Convert (obj, out result))
					{
						value = result;
						return true;
					}
				}
				else if (obj is string)
				{
					System.ComponentModel.TypeConverter conv = System.ComponentModel.TypeDescriptor.GetConverter (type);
					
					if (conv != null)
					{
						value = conv.ConvertFromInvariantString (obj as string);
						return true;
					}
				}
			}
			
			value = null;
			return false;
		}
		
		public static string[] GetSplitEnumValues(System.Enum value)
		{
			string[] values = value.ToString ().Split (InvariantConverter.EnumFlagSeperators, System.StringSplitOptions.RemoveEmptyEntries);
			
			if (values.Length > 0)
			{
				for (int i = 0; i < values.Length; i++)
				{
					values[i] = values[i].Trim ();
				}
			}
			
			return values;
		}
		
		public static string ExtractDecimal(ref string value)
		{
			char dotChar = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
			
			if (value == null)
			{
				return null;
			}
			
			value = value.TrimStart ();
			
			int  n   = value.Length;
			bool dot = false;
			int  i   = 0;
			
			while (i < n)
			{
				char c = value[i++];
				
				if (i == 1)
				{
					if (c == '-')
					{
						continue;
					}
				}
				
				if (System.Char.IsDigit (c))
				{
					continue;
				}
				if ((c == dotChar) &&
					(dot == false))
				{
					dot = true;
					continue;
				}
				
				i--;
				break;
			}
			
			string num = value.Substring (0, i);
			value = value.Substring (i);
			
			return num;
		}

		public static string Format(string format, object arg0)
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, format, arg0);
		}

		public static string Format(string format, params object[] args)
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, format, args);
		}


		/// <summary>
		/// Converts the text to an enum value. If the conversion is not possible, this will
		/// throw an exception.
		/// </summary>
		/// <typeparam name="T">The enum type.</typeparam>
		/// <param name="text">The text.</param>
		/// <returns>The enum value.</returns>
		public static T ToEnum<T>(this string text)
		{
			//	We do not want to enforce T to be struct here, since some users of this
			//	method must operate on unconstrained generic types.

			return (T) System.Enum.Parse (typeof (T), text);
		}

		/// <summary>
		/// Converts the text to an enum value. If the conversion is not possible, fall back
		/// to the default value.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enum.</typeparam>
		/// <param name="text">The text.</param>
		/// <param name="defaultValue">The default enum value.</param>
		/// <returns>The enum value.</returns>
		public static TEnum ToEnum<TEnum>(this string text, TEnum defaultValue)
			where TEnum : struct
		{
			if (string.IsNullOrEmpty (text))
			{
				return defaultValue;
			}
			else
			{
				object value;
				
				if (InvariantConverter.TryParseEnum (typeof (TEnum), text, out value))
				{
					return (TEnum) value;
				}

				return defaultValue;
			}
		}

		public static bool CheckEnumValue(System.Type type, System.Enum value)
		{
			string[] values = InvariantConverter.GetSplitEnumValues (value);

			for (int i = 0; i < values.Length; i++)
			{
				string name = values[i];
				object item;

				if ((InvariantConverter.TryParseEnum (type, name, out item) == false) ||
					(System.Enum.IsDefined (type, item) == false))
				{
					return false;
				}
			}

			return true;
		}

		
		/// <summary>
		/// Converts the string representation of an enum to its <c>enum</c>-equivalent value,
		/// and returns whether the operation succeeded or not.
		/// This method does not rely on <see cref="System.Enum.Parse"/> and will therefore
		/// never raise any first or second chance exception.
		/// </summary>
		/// <param name="type">The enum target type. May not be null.</param>
		/// <param name="input">The input text. May be null.</param>
		/// <param name="value">When this method returns, contains an <c>enum</c>-equivalent value to the enum contained in input, if the conversion succeeded.</param>
		/// <returns>
		/// <c>true</c> if the value was converted successfully; otherwise, <c>false</c>.
		/// </returns>
		public static bool TryParseEnum(System.Type type, string input, out object value)
		{
			if (type == null)
			{
				throw new System.ArgumentNullException ("type");
			}

			if (!type.IsEnum)
			{
				throw new System.ArgumentException (null, "type");
			}

			if (input == null)
			{
				value = InvariantConverter.CreateDefaultEnumValue (type);
				return false;
			}

			input = input.Trim ();
			
			if (input.Length == 0)
			{
				value = InvariantConverter.CreateDefaultEnumValue (type);
				return false;
			}

			ulong ul = 0;
			string[] names = System.Enum.GetNames (type);

			if (names.Length == 0)
			{
				value = InvariantConverter.CreateDefaultEnumValue (type);
				return false;
			}

			System.Type underlyingType = System.Enum.GetUnderlyingType (type);
			System.Array values = System.Enum.GetValues (type);

			//	Some enums like System.CodeDom.MemberAttributes *are* flags but are not declared with Flags...

			if ((type.IsDefined (typeof (System.FlagsAttribute), true) == false) &&
				(input.IndexOfAny (EnumFlagSeperators) < 0))
			{
				return InvariantConverter.EnumTokenToObject (type, underlyingType, names, values, input, out value);
			}

			//	Multi-value enum:
			string[] tokens = input.Split (EnumFlagSeperators, System.StringSplitOptions.RemoveEmptyEntries);

			if (tokens.Length == 0)
			{
				value = InvariantConverter.CreateDefaultEnumValue (type);
				return false;
			}

			foreach (string tok in tokens)
			{
				string token = tok.Trim (); // NOTE: we don't consider empty tokens as errors

				if (token.Length == 0)
				{
					continue;
				}

				object tokenValue;
				
				if (!InvariantConverter.EnumTokenToObject (type, underlyingType, names, values, token, out tokenValue))
				{
					value = InvariantConverter.CreateDefaultEnumValue (type);
					return false;
				}

				switch (System.Convert.GetTypeCode (tokenValue))
				{
					case System.TypeCode.Int16:
					case System.TypeCode.Int32:
					case System.TypeCode.Int64:
					case System.TypeCode.SByte:
						ul |= (ulong) System.Convert.ToInt64 (tokenValue, System.Globalization.CultureInfo.InvariantCulture);
						break;

					//case System.TypeCode.Byte:
					//case System.TypeCode.UInt16:
					//case System.TypeCode.UInt32:
					//case System.TypeCode.UInt64:
					default:
						ul |= System.Convert.ToUInt64 (tokenValue, System.Globalization.CultureInfo.InvariantCulture);
						break;
				}
			}
			
			value = (System.Enum) System.Enum.ToObject (type, ul);
			return true;
		}

		private static System.Enum CreateDefaultEnumValue(System.Type type)
		{
			return (System.Enum) System.Enum.ToObject (type, 0);
		}


		private static bool EnumTokenToObject(System.Type type, System.Type underlyingType, string[] names, System.Array values, string input, out object value)
		{
			for (int i = 0; i < names.Length; i++)
			{
				if (string.Compare (names[i], input, System.StringComparison.OrdinalIgnoreCase) == 0)
				{
					value = (System.Enum) values.GetValue (i);
					return true;
				}
			}

			if (input.IsInteger ())
			{
				object obj = InvariantConverter.EnumNumericValueToObject (type, underlyingType, input);
				if (obj == null)
				{
					value = InvariantConverter.CreateDefaultEnumValue (type);
					return false;
				}
				value = obj;
				return true;
			}

			value = value = InvariantConverter.CreateDefaultEnumValue (type);
			return false;
		}

		private static object EnumNumericValueToObject(System.Type type, System.Type underlyingType, string input)
		{
			if (underlyingType == typeof (int))
			{
				int s;
				if (int.TryParse (input, out s))
				{
					return System.Enum.ToObject (type, s);
				}
			}

			if (underlyingType == typeof (uint))
			{
				uint s;
				if (uint.TryParse (input, out s))
				{
					return System.Enum.ToObject (type, s);
				}
			}

			if (underlyingType == typeof (ulong))
			{
				ulong s;
				if (ulong.TryParse (input, out s))
				{
					return System.Enum.ToObject (type, s);
				}
			}

			if (underlyingType == typeof (long))
			{
				long s;
				if (long.TryParse (input, out s))
				{
					return System.Enum.ToObject (type, s);
				}
			}

			if (underlyingType == typeof (short))
			{
				short s;
				if (short.TryParse (input, out s))
				{
					return System.Enum.ToObject (type, s);
				}
			}

			if (underlyingType == typeof (ushort))
			{
				ushort s;
				if (ushort.TryParse (input, out s))
				{
					return System.Enum.ToObject (type, s);
				}
			}

			if (underlyingType == typeof (byte))
			{
				byte s;
				if (byte.TryParse (input, out s))
				{
					return System.Enum.ToObject (type, s);
				}
			}

			if (underlyingType == typeof (sbyte))
			{
				sbyte s;
				if (sbyte.TryParse (input, out s))
				{
					return System.Enum.ToObject (type, s);
				}
			}

			return null;
		}
		
		private static char[] EnumFlagSeperators = new char[] { ',', ';', '+', '|', ' ' };


		private static Dictionary<System.Type, ISerializationConverter> typeConverters = new Dictionary<System.Type, ISerializationConverter> ();

	}
}
