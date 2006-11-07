//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	///	The <c>TypeConverter</c> class provides type conversion between the low
	///	level types (<c>DbRawType</c>) and the simplified types (<c>DbSimpleType</c>).
	/// </summary>
	public static class TypeConverter
	{
		static TypeConverter()
		{
			TypeConverter.sysTypeToSimpleType = new Dictionary<System.Type, DbSimpleType> ();
			
			//	Remplit la table de correspondances entre les types natifs et les types
			//	simplifiés. Cette table est utilisée par la méthode IsCompatibleToSimpleType
			
			TypeConverter.sysTypeToSimpleType[typeof (bool)]    = DbSimpleType.Decimal;
			TypeConverter.sysTypeToSimpleType[typeof (byte)]    = DbSimpleType.Decimal;
			TypeConverter.sysTypeToSimpleType[typeof (short)]   = DbSimpleType.Decimal;
			TypeConverter.sysTypeToSimpleType[typeof (int)]     = DbSimpleType.Decimal;
			TypeConverter.sysTypeToSimpleType[typeof (long)]    = DbSimpleType.Decimal;
			TypeConverter.sysTypeToSimpleType[typeof (decimal)] = DbSimpleType.Decimal;
			TypeConverter.sysTypeToSimpleType[typeof (string)]  = DbSimpleType.String;
			TypeConverter.sysTypeToSimpleType[typeof (byte[])]  = DbSimpleType.ByteArray;
			
			TypeConverter.sysTypeToSimpleType[typeof (Date)]            = DbSimpleType.Date;
			TypeConverter.sysTypeToSimpleType[typeof (Time)]            = DbSimpleType.Time;
			TypeConverter.sysTypeToSimpleType[typeof (System.DateTime)] = DbSimpleType.DateTime;
			
			TypeConverter.sysTypeToSimpleType[typeof (System.Guid)] = DbSimpleType.Guid;

			TypeConverter.sysTypeToRawType = new Dictionary<System.Type, DbRawType> ();

			TypeConverter.sysTypeToRawType[typeof (System.DBNull)] = DbRawType.Null;
			TypeConverter.sysTypeToRawType[typeof (System.Boolean)] = DbRawType.Boolean;
			TypeConverter.sysTypeToRawType[typeof (System.Int16)] = DbRawType.Int16;
			TypeConverter.sysTypeToRawType[typeof (System.Int32)] = DbRawType.Int32;
			TypeConverter.sysTypeToRawType[typeof (System.Int64)] = DbRawType.Int64;
			TypeConverter.sysTypeToRawType[typeof (System.Decimal)] = DbRawType.SmallDecimal;
			TypeConverter.sysTypeToRawType[typeof (System.String)] = DbRawType.String;
			TypeConverter.sysTypeToRawType[typeof (Date)] = DbRawType.Date;
			TypeConverter.sysTypeToRawType[typeof (Time)] = DbRawType.Time;
			TypeConverter.sysTypeToRawType[typeof (System.DateTime)] = DbRawType.DateTime;
			TypeConverter.sysTypeToRawType[typeof (byte[])] = DbRawType.ByteArray;
			TypeConverter.sysTypeToRawType[typeof (System.Guid)] = DbRawType.Guid;
		}
		
		
		public static System.Type GetNativeType(DbRawType rawType)
		{
			switch (rawType)
			{
				case DbRawType.Null:			return typeof (System.DBNull);
				case DbRawType.Boolean:			return typeof (System.Boolean);
				case DbRawType.Int16:			return typeof (System.Int16);
				case DbRawType.Int32:			return typeof (System.Int32);
				case DbRawType.Int64:			return typeof (System.Int64);
				case DbRawType.SmallDecimal:	return typeof (System.Decimal);
				case DbRawType.LargeDecimal:	return typeof (System.Decimal);
				case DbRawType.String:			return typeof (System.String);
				case DbRawType.Date:			return typeof (Common.Types.Date);
				case DbRawType.Time:			return typeof (Common.Types.Time);
				case DbRawType.DateTime:		return typeof (System.DateTime);
				case DbRawType.ByteArray:		return typeof (byte[]);
				case DbRawType.Guid:			return typeof (System.Guid);
			}
			
			return null;
		}

		public static DbSimpleType GetSimpleType(DbRawType rawType)
		{
			DbNumDef numDef;
			return TypeConverter.GetSimpleType (rawType, out numDef);
		}
		
		public static DbSimpleType GetSimpleType(DbRawType rawType, out DbNumDef numDef)
		{
			numDef = null;
			
			switch (rawType)
			{
				case DbRawType.Null:
					return DbSimpleType.Null;
				
				case DbRawType.Boolean:
				case DbRawType.Int16:
				case DbRawType.Int32:
				case DbRawType.Int64:
				case DbRawType.SmallDecimal:
				case DbRawType.LargeDecimal:
					
					//	C'est l'un des types numériques : il faut le convertir en une représentation
					//	numérique (DbNumDef), puisque tous correspondent au même type simplifié.
					
					numDef = DbNumDef.FromRawType (rawType);
					System.Diagnostics.Debug.Assert (numDef != null);
					return DbSimpleType.Decimal;
				
				case DbRawType.String:		return DbSimpleType.String;
				case DbRawType.Date:		return DbSimpleType.Date;
				case DbRawType.Time:		return DbSimpleType.Time;
				case DbRawType.DateTime:	return DbSimpleType.DateTime;
				case DbRawType.ByteArray:	return DbSimpleType.ByteArray;
				case DbRawType.Guid:		return DbSimpleType.Guid;
			}
			
			return DbSimpleType.Unsupported;
		}

		public static DbSimpleType GetSimpleType(INamedType namedType, out DbNumDef numDef)
		{
			numDef = null;
			
			DbSimpleType simpleType;
			System.Type  systemType = namedType.SystemType;

			if (TypeConverter.sysTypeToSimpleType.TryGetValue (systemType, out simpleType))
			{
				if (simpleType == DbSimpleType.Decimal)
				{
					INumericType numericType = namedType as INumericType;

					if (numericType == null)
					{
						throw new System.ArgumentException (string.Format ("Type {0} cannot be mapped to {1}; missing INumericType interface", namedType.Name, simpleType));
					}

					if (numericType.UseCompactStorage)
					{
						//	The numeric type has request a compact storage of the data.
						//	Let DbNumDef decide of the best possible representation which
						//	uses the least amount of bits.
						
						numDef = new DbNumDef (numericType.Range);
					}
					else
					{
						//	The numeric type will be mapped to one of the standard raw
						//	types (16-bit, 32-bit, 64-bit integer or small/large decimal)
						//	depending on its settings.
						
						DbRawType rawType = TypeConverter.GetRawType (systemType);

						if ((rawType == DbRawType.SmallDecimal) ||
							(rawType == DbRawType.LargeDecimal))
						{
							//	Try to optimize the decimal representation in order to
							//	properly encode all digits in the numeric type.

							int shift     = numericType.Range.FractionalDigits;
							int precision = numericType.Range.GetMaximumDigitCount ();

							precision -= shift;

							//	SmallDecimal : nnnnnnnnn.nnnnnnnnn
							//	LargeDecimal : nnnnnnnnnnnnnnn.nnn

							if (shift > 9)
							{
								throw new System.ArgumentException (string.Format ("Type {0} needs {1} fractional digits (max. is 9)", namedType.Name, shift));
							}
							else if (shift > 3)
							{
								//	Small decimal with 9 fractional digits

								rawType = DbRawType.SmallDecimal;
							}
							else
							{
								//	Large decimal with only 3 fractional digits
								
								rawType = DbRawType.LargeDecimal;
							}

							precision += shift;
							
							if (precision > 18)
							{
								throw new System.ArgumentException (string.Format ("Type {0} needs {1} digits (max. is 18)", namedType.Name, precision));
							}
						}
						
						numDef = DbNumDef.FromRawType (rawType);
					}
				}
				
				return simpleType;
			}

			return DbSimpleType.Unsupported;
		}

		public static DbRawType GetRawType(System.Type type)
		{
			DbRawType rawType;

			if (TypeConverter.sysTypeToRawType.TryGetValue (type, out rawType))
			{
				return rawType;
			}

			return DbRawType.Unknown;
		}

		public static DbRawType GetRawType(DbSimpleType simpleType, DbNumDef numDef)
		{
			switch (simpleType)
			{
				case DbSimpleType.Null:
					return DbRawType.Null;
				
				case DbSimpleType.Decimal:
					System.Diagnostics.Debug.Assert (numDef != null);
					
					if (numDef.InternalRawType == DbRawType.Unsupported)
					{
						//	Ce n'est pas un type numérique standard, donc il faut prévoir
						//	une conversion éventuelle.
						
						int bits = numDef.MinimumBits;
						
						if (bits <= 16)
						{
							return DbRawType.Int16;
						}
						if (bits <= 32)
						{
							return DbRawType.Int32;
						}
						if (bits <= 64)
						{
							return DbRawType.Int64;
						}
						
						throw new Exceptions.FormatException (string.Format ("Unsupported numeric format, {0} bits required", bits));
					}
					
					return numDef.InternalRawType;
				
				case DbSimpleType.String:		return DbRawType.String;
				case DbSimpleType.Date:			return DbRawType.Date;
				case DbSimpleType.Time:			return DbRawType.Time;
				case DbSimpleType.DateTime:		return DbRawType.DateTime;
				case DbSimpleType.ByteArray:	return DbRawType.ByteArray;
				case DbSimpleType.Guid:			return DbRawType.Guid;
			}
			
			return DbRawType.Unsupported;
		}

		public static DbRawType GetRawType(INamedType namedType)
		{
			DbNumDef numDef;
			DbSimpleType simpleType = TypeConverter.GetSimpleType (namedType, out numDef);
			return TypeConverter.GetRawType (simpleType, numDef);
		}
		
		
		public static bool IsCompatibleToSimpleType(System.Type type, DbSimpleType simpleType)
		{
			if (TypeConverter.sysTypeToSimpleType.ContainsKey (type))
			{
				//	Le type natif est connu, on le compare simplement au type simplifié qui
				//	a été stocké dans la table.

				return TypeConverter.sysTypeToSimpleType[type] == simpleType;
			}
			else
			{
				return false;
			}
		}
		
		
		public static object ConvertToSimpleType(object value, DbSimpleType simpleType, DbNumDef numDef)
		{
			//	Convertit un objet en provenance de ADO.NET en sa représentation équivalente basée
			//	sur le type simplifié 'simpleType' et la définition de son format numérique.
			
			//	Cette méthode peut transformer une donnée numérique de manière significative,
			//	par exemple de Int64 à decimal.
			
			if (value == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (simpleType != DbSimpleType.Unsupported);
			System.Diagnostics.Debug.Assert (simpleType != DbSimpleType.Null);
			
			switch (simpleType)
			{
				case DbSimpleType.Decimal:
					
					//	Le type numérique "Decimal" regroupe tous les types numériques stockés dans la
					//	base de données. Il faut donc convertir le type utilisé par ADO.NET en son type
					//	générique, en faisant attention aux types nécessitant une conversion supplémen-
					//	taire (dénotés par IsConversionNeeded).
					
					decimal num;
					
					if (value is System.Decimal)
					{
						num = (System.Decimal) value;
					}
					else if (value is System.Int16)
					{
						num = (System.Int16) value;
					}
					else if (value is System.Int32)
					{
						num = (System.Int32) value;
					}
					else if (value is System.Int64)
					{
						num = (System.Int64) value;
					}
					else if (value is System.String)
					{
						string text = (string) value;
						num = System.Decimal.Parse (text, TypeConverter.InvariantFormatProvider);
					}
					else
					{
						throw new Exceptions.FormatException (string.Format ("Expected numeric format, got {0}", value.GetType ().ToString ()));
					}
						
					if (numDef.IsConversionNeeded)
					{
						long i64 = (long) num;
						num = numDef.ConvertFromInt64 (i64);
					}
					
					if (numDef.CheckCompatibility (num))
					{
						return num;
					}
					
					throw new Exceptions.FormatException (string.Format ("Incompatible numeric format for {0}", num));
				
				case DbSimpleType.String:		return value;
				case DbSimpleType.Date:			return new Common.Types.Date (value);
				case DbSimpleType.Time:			return new Common.Types.Time (value);
				case DbSimpleType.DateTime:		return value;
				case DbSimpleType.ByteArray:	return value;
				case DbSimpleType.Guid:			return value;
			}
			
			return null;
		}
		
		public static object ConvertFromSimpleType(object value, DbSimpleType simpleType, DbNumDef numDef)
		{
			//	Convertit un objet basé sur le type simplifié 'simpleType' et la définition de son
			//	format numérique en un objet compatible avec ADO.NET.
			
			//	Cette méthode peut transformer une donnée numérique de manière siginificative,
			//	par exemple de Int64 en decimal.
			
			if (value == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (simpleType != DbSimpleType.Unsupported);
			System.Diagnostics.Debug.Assert (simpleType != DbSimpleType.Null);
			
			switch (simpleType)
			{
				case DbSimpleType.Decimal:
					System.Diagnostics.Debug.Assert (value is decimal);
					
					//	Le type simplifié numérique "Decimal" est toujours représenté au moyen du
					//	type decimal de .NET. Par contre, son type ADO.NET peut être très différent,
					//	en fonction des définitions numériques fournies par numDef.
					//
					//	En particulier:
					//
					//	- Entiers 16, 32 et 64-bits, booléens, stockés comme tels.
					//	- Nombres à virgule fixe (SmallDecimal, LargeDecimal) stockés comme tels.
					//	- Autres formats à virgule ou offset, nécessitant une conversion vers un
					//	  format numérique neutre Int64.
					
					decimal num = (decimal) value;
					
					switch (numDef.InternalRawType)
					{
						case DbRawType.Boolean:			return (bool)(num != 0);
						case DbRawType.Int16:			return (System.Int16) num;
						case DbRawType.Int32:			return (System.Int32) num;
						case DbRawType.Int64:			return (System.Int64) num;
						case DbRawType.SmallDecimal:	return num;
						case DbRawType.LargeDecimal:	return num;
					}
					
					//	Le format numérique est vraiment particulier... il faut le convertir en
					//	quelque chose d'universellement stockable (int) et déterminer ensuite
					//	la précision requise.
					
					long i64 = numDef.ConvertToInt64 (num);
					
					if (numDef.MinimumBits <= 16)		return (System.Int16) i64;
					if (numDef.MinimumBits <= 32)		return (System.Int32) i64;
					
					return i64;
				
				case DbSimpleType.String:
					System.Diagnostics.Debug.Assert (value is string);
					return value;
				
				case DbSimpleType.Date:
				case DbSimpleType.Time:
				case DbSimpleType.DateTime:
					if (value is Common.Types.Date)
					{
						Common.Types.Date date = (Common.Types.Date) value;
						return date.ToDateTime ();
					}
					if (value is Common.Types.Time)
					{
						Common.Types.Time time = (Common.Types.Time) value;
						return time.ToDateTime ();
					}
					if (value is System.DateTime)
					{
						return value;
					}
					break;
				
				case DbSimpleType.ByteArray:
					System.Diagnostics.Debug.Assert (value is byte[]);
					return value;
				
				case DbSimpleType.Guid:
					System.Diagnostics.Debug.Assert (value is System.Guid);
					return value;
			}
			
			throw new Exceptions.FormatException (string.Format ("Invalid value format {0}, cannot convert to {1}", value.GetType ().ToString (), simpleType.ToString ()));
		}
		
		
		public static object ConvertToInternal(ITypeConverter converter, object value, DbRawType rawType)
		{
			if (converter.CheckNativeSupport (rawType))
			{
				return value;
			}
			
			IRawTypeConverter rawConverter;
			
			if (converter.GetRawTypeConverter (rawType, out rawConverter))
			{
				return rawConverter.ConvertToInternalType (value);
			}
			
			throw new System.InvalidOperationException (string.Format ("Cannot convert type {0} to internal representation.", rawType));
		}
		
		public static object ConvertFromInternal(ITypeConverter converter, object value, DbRawType rawType)
		{
			if (converter.CheckNativeSupport (rawType))
			{
				return value;
			}
			
			IRawTypeConverter rawConverter;
			
			if (converter.GetRawTypeConverter (rawType, out rawConverter))
			{
				return rawConverter.ConvertFromInternalType (value);
			}
			
			throw new System.InvalidOperationException (string.Format ("Cannot convert from internal representation to type {0}.", rawType));
		}


		public static System.IFormatProvider	CurrentFormatProvider
		{
			get
			{
				return System.Globalization.CultureInfo.CurrentCulture;
			}
		}

		public static System.IFormatProvider	InvariantFormatProvider
		{
			get
			{
				return System.Globalization.CultureInfo.InvariantCulture;
			}
		}


		private static Dictionary<System.Type, DbSimpleType>	sysTypeToSimpleType;
		private static Dictionary<System.Type, DbRawType>		sysTypeToRawType;
	}
}
