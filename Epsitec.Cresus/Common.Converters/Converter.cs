//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Converters
{
	/// <summary>
	/// La classe Converter permet de convertir des données simples entre
	/// elles.
	/// </summary>
	public class Converter
	{
		private Converter()
		{
		}
		
		
		public static bool IsNull(object obj)
		{
			return (obj == null) || (obj is System.DBNull);
		}
		
		public static bool IsNotNull(object obj)
		{
			return (obj != null) && !(obj is System.DBNull);
		}
		
		
		public static string ToString(object obj)
		{
			string value;
			Converter.Convert (obj, out value);
			return value;
		}
		
		
		public static bool Convert(object obj, out string value)
		{
			if ((obj == null) || (obj is System.DBNull))
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
				
				switch (code)
				{
					case System.TypeCode.Boolean:	value = ((System.Boolean) obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Decimal:	value = ((System.Decimal) obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Single:	value = ((System.Single)  obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Double:	value = ((System.Double)  obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Byte:		value = ((System.Byte)    obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Int16:		value = ((System.Int16)   obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Int32:		value = ((System.Int32)   obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
					case System.TypeCode.Int64:		value = ((System.Int64)   obj).ToString (System.Globalization.CultureInfo.InvariantCulture); return true;
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
			decimal value_decimal;
			bool ok = Converter.Convert (obj, out value_decimal);
			value = (value_decimal == 0) ? false : true;
			return ok;
		}
		
		public static bool Convert(object obj, out int value)
		{
			decimal value_decimal;
			bool ok = Converter.Convert (obj, out value_decimal);
			value = (int) value_decimal;
			return ok;
		}
		
		public static bool Convert(object obj, out long value)
		{
			decimal value_decimal;
			bool ok = Converter.Convert (obj, out value_decimal);
			value = (long) value_decimal;
			return ok;
		}
		
		public static bool Convert(object obj, out decimal value)
		{
			//	Retourne true si la valeur de 'obj' n'est pas 'null' ou une
			//	de ses variantes, false sinon. Si le type n'est pas reconnu ou
			//	que la syntaxe n'est pas correcte, une exception est levée.
			
			if ((obj == null) || (obj is System.DBNull))
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
				
				value = System.Decimal.Parse (text, System.Globalization.CultureInfo.InvariantCulture);
				return true;
			}
			
			if (obj is System.ValueType)
			{
				System.TypeCode code = System.Convert.GetTypeCode (obj);
				
				switch (code)
				{
					case System.TypeCode.Boolean:	value = (decimal) ((System.Boolean) obj ? 1 : 0);	return true;
					
					case System.TypeCode.Decimal:	value = (decimal) (System.Decimal) obj;				return true;
					
					case System.TypeCode.Single:	value = (decimal) (System.Single) obj;				return true;
					case System.TypeCode.Double:	value = (decimal) (System.Double) obj;				return true;
					
					case System.TypeCode.Byte:		value = (decimal) (System.Byte) obj;				return true;
					case System.TypeCode.Int16:		value = (decimal) (System.Int16) obj;				return true;
					case System.TypeCode.Int32:		value = (decimal) (System.Int32) obj;				return true;
					case System.TypeCode.Int64:		value = (decimal) (System.Int64) obj;				return true;
					
					case System.TypeCode.DBNull:	value = 0;											return false;
				}
			}
			
			throw new System.NotSupportedException (string.Format ("Type {0}: conversion not supported.", obj.GetType ().Name));
		}
		
		public static bool Convert(object obj, System.Type type, out object value)
		{
			if (type == typeof (string))
			{
				string result;
				
				if (Converter.Convert (obj, out result))
				{
					value = result;
					return true;
				}
			}
			else if (type == typeof (bool))
			{
				bool result;
				
				if (Converter.Convert (obj, out result))
				{
					value = result;
					return true;
				}
			}
			else if (type == typeof (int))
			{
				int result;
				
				if (Converter.Convert (obj, out result))
				{
					value = result;
					return true;
				}
			}
			else if (type == typeof (long))
			{
				long result;
				
				if (Converter.Convert (obj, out result))
				{
					value = result;
					return true;
				}
			}
			else if (type == typeof (decimal))
			{
				decimal result;
				
				if (Converter.Convert (obj, out result))
				{
					value = result;
					return true;
				}
			}
			
			value = null;
			return false;
		}
	}
}
