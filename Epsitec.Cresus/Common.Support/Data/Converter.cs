//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Support.Data
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
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Warning: type {0} default ToString method used. Value={1}.", obj.GetType (), obj.ToString ()));
			}
			
			value = obj.ToString ();
			return true;
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
			
			if (obj == null)
			{
				value = 0;
				return false;
			}
			
			if (obj is System.String)
			{
				//	On va devoir faire un "Parse" coûteux...
				
				string text = obj as string;
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
			
			throw new System.NotSupportedException (string.Format ("Type {0}: no conversion supported.", obj.GetType ().Name));
		}
	}
}
