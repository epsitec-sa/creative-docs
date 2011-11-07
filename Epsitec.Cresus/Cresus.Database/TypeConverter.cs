//	Copyright © 2003-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;

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
			TypeConverter.sysTypeToSimpleType[typeof (byte[])]  = DbSimpleType.ByteArray;
			
			TypeConverter.sysTypeToSimpleType[typeof (string)]        = DbSimpleType.String;
			TypeConverter.sysTypeToSimpleType[typeof (FormattedText)] = DbSimpleType.String;

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


		/// <summary>
		/// Gets the native type for a raw type.
		/// </summary>
		/// <param name="rawType">The raw type.</param>
		/// <returns>The native type.</returns>
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


		/// <summary>
		/// Gets the ADO.NET type that corresponds to a given <see cref="DbRawType"/>.
		/// </summary>
		/// <param name="dbRawType">The <see cref="DbRawType"/> whose ADO.NET type to obtain.</param>
		/// <returns>The ADO.NET type that corresponds to the given <see cref="DbRawType"/>.</returns>
		public static System.Type GetAdoType(DbRawType dbRawType)
		{
			switch (dbRawType)
			{
				case DbRawType.Null:			return typeof (System.DBNull);
				case DbRawType.Boolean:			return typeof (System.Boolean);
				case DbRawType.Int16:			return typeof (System.Int16);
				case DbRawType.Int32:			return typeof (System.Int32);
				case DbRawType.Int64:			return typeof (System.Int64);
				case DbRawType.SmallDecimal:	return typeof (System.Decimal);
				case DbRawType.LargeDecimal:	return typeof (System.Decimal);
				case DbRawType.String:			return typeof (System.String);
				case DbRawType.Date:			return typeof (System.DateTime);
				case DbRawType.Time:			return typeof (System.DateTime);
				case DbRawType.DateTime:		return typeof (System.DateTime);
				case DbRawType.ByteArray:		return typeof (byte[]);
				case DbRawType.Guid:			return typeof (System.Guid);
				default:						throw new System.NotImplementedException ();
			}
		}


		/// <summary>
		/// Gets the simple type for a raw type.
		/// </summary>
		/// <param name="rawType">The raw type.</param>
		/// <returns>The simple type.</returns>
		public static DbSimpleType GetSimpleType(DbRawType rawType)
		{
			DbNumDef numDef;
			return TypeConverter.GetSimpleType (rawType, out numDef);
		}

		/// <summary>
		/// Gets the simple type for a raw type.
		/// </summary>
		/// <param name="rawType">The raw type.</param>
		/// <param name="numDef">The numeric definition.</param>
		/// <returns>The simple type.</returns>
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
					
					//	This is one of the numeric types : we must find the numeric definition
					//	to differentiate the raw types (they all map to DbSimpleType.Decimal).
					
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
			
			return DbSimpleType.Unknown;
		}

		/// <summary>
		/// Gets the simple type for a named type.
		/// </summary>
		/// <param name="namedType">The named type.</param>
		/// <param name="numDef">The numeric definition.</param>
		/// <returns>The simple type.</returns>
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
			else if (systemType.IsEnum)
			{
				if (TypeConverter.IsLongEnum (systemType))
				{
					return TypeConverter.GetSimpleType (LongIntegerType.Default, out numDef);
				}
				else
				{
					return TypeConverter.GetSimpleType (IntegerType.Default, out numDef);
				}
			}
			else
			{
				return DbSimpleType.Unknown;
			}
		}

		/// <summary>
		/// Get the raw type for a native type.
		/// </summary>
		/// <param name="systemType">The native type.</param>
		/// <returns>The raw type or <c>DbRawType.Unknown</c> if no mapping exists.</returns>
		public static DbRawType GetRawType(System.Type systemType)
		{
			DbRawType rawType;

			if (TypeConverter.sysTypeToRawType.TryGetValue (systemType, out rawType))
			{
				return rawType;
			}
			else if (systemType.IsEnum)
			{
				if (TypeConverter.IsLongEnum (systemType))
				{
					return DbRawType.Int64;
				}
				else
				{
					return DbRawType.Int32;
				}
			}
			else
			{
				return DbRawType.Unknown;
			}
		}

		/// <summary>
		/// Get the raw type for a simple type.
		/// </summary>
		/// <param name="simpleType">The simple type.</param>
		/// <param name="numDef">The numeric definition.</param>
		/// <returns>
		/// The raw type or <c>DbRawType.Unknown</c> if no mapping exists.
		/// </returns>
		public static DbRawType GetRawType(DbSimpleType simpleType, DbNumDef numDef)
		{
			switch (simpleType)
			{
				case DbSimpleType.Null:
					return DbRawType.Null;
				
				case DbSimpleType.Decimal:
					System.Diagnostics.Debug.Assert (numDef != null);
					
					if (numDef.InternalRawType == DbRawType.Unknown)
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
			
			return DbRawType.Unknown;
		}

		/// <summary>
		/// Get the raw type for a named type.
		/// </summary>
		/// <param name="namedType">The named type.</param>
		/// <returns>The raw type or <c>DbRawType.Unknown</c> if no mapping exists.</returns>
		public static DbRawType GetRawType(INamedType namedType)
		{
			DbNumDef numDef;
			DbSimpleType simpleType = TypeConverter.GetSimpleType (namedType, out numDef);
			return TypeConverter.GetRawType (simpleType, numDef);
		}


		/// <summary>
		/// Determines whether a native type is compatible to the specified simple type.
		/// </summary>
		/// <param name="systemType">The native type.</param>
		/// <param name="simpleType">The simple type.</param>
		/// <returns>
		/// 	<c>true</c> if the types are compatible; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsCompatibleToSimpleType(System.Type systemType, DbSimpleType simpleType)
		{
			if (TypeConverter.sysTypeToSimpleType.ContainsKey (systemType))
			{
				return TypeConverter.sysTypeToSimpleType[systemType] == simpleType;
			}
			else if (systemType.IsEnum)
			{
				if (TypeConverter.IsLongEnum (systemType))
				{
					return TypeConverter.IsCompatibleToSimpleType (typeof (long), simpleType);
				}
				else
				{
					return TypeConverter.IsCompatibleToSimpleType (typeof (int), simpleType);
				}
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Converts a value from ADO.NET to the specified simple type. This can produce
		/// important changes to the data (e.g. map a <c>long</c> to a <c>decimal</c>).
		/// </summary>
		/// <param name="value">The ADO.NET value.</param>
		/// <param name="type">The type definition.</param>
		/// <returns>The converted value.</returns>
		public static object ConvertToSimpleType(object value, DbTypeDef type)
		{
			return TypeConverter.ConvertToSimpleType (value, type.SimpleType, type.NumDef);
		}

		/// <summary>
		/// Converts a value from ADO.NET to the specified simple type. This can produce
		/// important changes to the data (e.g. map a <c>long</c> to a <c>decimal</c>).
		/// </summary>
		/// <param name="value">The ADO.NET value.</param>
		/// <param name="type">The raw type.</param>
		/// <returns>The converted value.</returns>
		public static object ConvertToSimpleType(object value, DbRawType type)
		{
			DbNumDef     numDef;
			DbSimpleType simpleType = TypeConverter.GetSimpleType (type, out numDef);
			
			return TypeConverter.ConvertToSimpleType (value, simpleType, numDef);
		}

		/// <summary>
		/// Converts a value from ADO.NET to the specified simple type. This can produce
		/// important changes to the data (e.g. map a <c>long</c> to a <c>decimal</c>).
		/// </summary>
		/// <param name="value">The ADO.NET value.</param>
		/// <param name="simpleType">The simple type.</param>
		/// <param name="numDef">The numeric definition.</param>
		/// <returns>The converted value.</returns>
		public static object ConvertToSimpleType(object value, DbSimpleType simpleType, DbNumDef numDef)
		{
			if (value == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (simpleType != DbSimpleType.Unknown);
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
					else if (value is System.Boolean)
					{
						num = (System.Boolean) value ? 1 : 0;
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
					else if (value is DbId)
					{
						num = (DbId) value;
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
				case DbSimpleType.Date:			return Common.Types.Date.FromObject (value);
				case DbSimpleType.Time:			return Common.Types.Time.FromObject (value);
				case DbSimpleType.DateTime:		return TypeConverter.NormalizeToUtc ((System.DateTime) value);
				case DbSimpleType.ByteArray:	return value;
				case DbSimpleType.Guid:			return value;
			}
			
			return null;
		}

		private static System.DateTime NormalizeToUtc(System.DateTime value)
		{
			switch (value.Kind)
			{
				case System.DateTimeKind.Utc:
				case System.DateTimeKind.Local:
					return value;
				
				case System.DateTimeKind.Unspecified:
					return System.DateTime.SpecifyKind (value, System.DateTimeKind.Utc);

				default:
					throw new System.NotSupportedException (string.Format ("DateTime.Kind value {0} not supported", value.Kind.GetQualifiedName ()));
			}
		}

		/// <summary>
		/// Converts a value from the specified simple type to an ADO.NET compatible value.
		/// This can produce important changes to the data (e.g. map a <c>decimal</c> to a <c>long</c>).
		/// </summary>
		/// <param name="value">The raw value.</param>
		/// <param name="rawType">The raw type.</param>
		/// <returns>
		/// The converted value, compatible with ADO.NET.
		/// </returns>
		public static object ConvertFromSimpleType(object value, DbRawType rawType)
		{
			DbNumDef     numDef;
			DbSimpleType simpleType = TypeConverter.GetSimpleType (rawType, out numDef);

			return TypeConverter.ConvertFromSimpleType (value, simpleType, numDef);
		}
		
		/// <summary>
		/// Converts a value from the specified simple type to an ADO.NET compatible value.
		/// This can produce important changes to the data (e.g. map a <c>decimal</c> to a <c>long</c>).
		/// </summary>
		/// <param name="value">The raw value.</param>
		/// <param name="simpleType">The simple type.</param>
		/// <param name="numDef">The numeric definition.</param>
		/// <returns>The converted value, compatible with ADO.NET.</returns>
		public static object ConvertFromSimpleType(object value, DbSimpleType simpleType, DbNumDef numDef)
		{
			if (value == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (simpleType != DbSimpleType.Unknown);
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
						System.DateTime dateTime = (System.DateTime) value;
						if (dateTime.Kind == System.DateTimeKind.Local)
						{
							dateTime = dateTime.ToUniversalTime ();
						}
						return dateTime;
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

		/// <summary>
		/// Converts an ADO.NET compatible value to an internal value (such as
		/// it will be stored in the database by a specific provider).
		/// </summary>
		/// <param name="converter">The type converter.</param>
		/// <param name="value">The ADO.NET compatible value.</param>
		/// <param name="rawType">The raw type.</param>
		/// <returns>The value for internal use.</returns>
		public static object ConvertToInternal(ITypeConverter converter, object value, DbRawType rawType)
		{
			if (converter.CheckNativeSupport (rawType))
			{
				return value;
			}
			
			IRawTypeConverter rawConverter;
			
			if (converter.GetRawTypeConverter (rawType, out rawConverter))
			{
				//	This might map a Boolean to a 16-bit integer or a Guid to a fixed length
				//	string, for instance :
				
				return rawConverter.ConvertToInternalType (value);
			}
			
			throw new System.InvalidOperationException (string.Format ("Cannot convert type {0} to internal representation.", rawType));
		}

		/// <summary>
		/// Converts an internal value back to an ADO.NET compatible value.
		/// </summary>
		/// <param name="converter">The type converter.</param>
		/// <param name="value">The internal value.</param>
		/// <param name="rawType">The raw type.</param>
		/// <returns>The ADO.NET compatible value.</returns>
		public static object ConvertFromInternal(ITypeConverter converter, object value, DbRawType rawType)
		{
			if (converter.CheckNativeSupport (rawType))
			{
				return value;
			}
			
			IRawTypeConverter rawConverter;
			
			if (converter.GetRawTypeConverter (rawType, out rawConverter))
			{
				//	This might convert a 16-bit integer back to a Boolean, for instance :
				
				return rawConverter.ConvertFromInternalType (value);
			}
			
			throw new System.InvalidOperationException (string.Format ("Cannot convert from internal representation to type {0}.", rawType));
		}

		/// <summary>
		/// Gets the invariant format provider.
		/// </summary>
		/// <value>The invariant format provider.</value>
		public static System.IFormatProvider	InvariantFormatProvider
		{
			get
			{
				return System.Globalization.CultureInfo.InvariantCulture;
			}
		}

		/// <summary>
		/// Determines whether the specified type is an <c>enum</c> which needs 64-bit to
		/// store its values.
		/// </summary>
		/// <param name="systemType">Type of the system.</param>
		/// <exception cref="System.ArgumentException">When the type is not an <c>enum</c>.</exception>
		/// <returns>
		/// 	<c>true</c> if the specified type is an <c>enum</c> which needs 64-bit to store its values; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsLongEnum(System.Type systemType)
		{
			if (systemType.IsEnum)
			{
				systemType = systemType.GetEnumUnderlyingType ();
				System.Diagnostics.Debug.Assert (systemType != typeof (ulong));
				return systemType == typeof (long);
			}

			throw new System.ArgumentException ("Type not an Enum", "systemType");
		}

		public static object GetDefaultValueForDbRawType(DbRawType dbRawType)
		{
			switch (dbRawType)
			{
				case DbRawType.Boolean:
					return false;

				case DbRawType.ByteArray:
					return new byte[0];

				case DbRawType.Date:
					return new Date ();

				case DbRawType.DateTime:
					return new System.DateTime ();

				case DbRawType.Guid:
					return new System.Guid ();

				case DbRawType.Int16:
					return (short) 0;

				case DbRawType.Int32:
					return (int) 0;

				case DbRawType.Int64:
					return (long) 0;

				case DbRawType.LargeDecimal:
					return (decimal) 0;

				case DbRawType.SmallDecimal:
					return (decimal) 0;

				case DbRawType.String:
					return "";

				case DbRawType.Time:
					return new Time ();

				default:
					throw new System.NotImplementedException ();
			}
		}


		private static Dictionary<System.Type, DbSimpleType>	sysTypeToSimpleType;
		private static Dictionary<System.Type, DbRawType>		sysTypeToRawType;
	}
}
