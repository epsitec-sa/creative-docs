namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe TypeConverter réalise les conversions entre les données de bas
	/// niveau (type décrit par DbRawType) et les données simplifiées (type décrit
	/// par DbSimpleType).
	/// </summary>
	public class TypeConverter
	{
		private TypeConverter()
		{
		}
		
		static TypeConverter()
		{
			TypeConverter.type_hash = new System.Collections.Hashtable ();
			
			//	Remplit la table de correspondances entre les types natifs et les types
			//	simplifiés. Cette table est utilisée par la méthode IsCompatibleToSimpleType
			
			TypeConverter.type_hash[typeof (bool)]   = DbSimpleType.Decimal;
			TypeConverter.type_hash[typeof (byte)]   = DbSimpleType.Decimal;
			TypeConverter.type_hash[typeof (short)]  = DbSimpleType.Decimal;
			TypeConverter.type_hash[typeof (int)]    = DbSimpleType.Decimal;
			TypeConverter.type_hash[typeof (long)]   = DbSimpleType.Decimal;
			TypeConverter.type_hash[typeof (string)] = DbSimpleType.String;
			TypeConverter.type_hash[typeof (byte[])] = DbSimpleType.ByteArray;
			
			TypeConverter.type_hash[typeof (System.Date)]     = DbSimpleType.Date;
			TypeConverter.type_hash[typeof (System.Time)]     = DbSimpleType.Time;
			TypeConverter.type_hash[typeof (System.DateTime)] = DbSimpleType.DateTime;
			
			TypeConverter.type_hash[typeof (System.Guid)] = DbSimpleType.Guid;
		}
		
		
		public static DbSimpleType MapToSimpleType(DbRawType raw_type)
		{
			DbNumDef num_def;
			return TypeConverter.MapToSimpleType (raw_type, out num_def);
		}
		
		public static DbSimpleType MapToSimpleType(DbRawType raw_type, out DbNumDef num_def)
		{
			num_def = null;
			
			switch (raw_type)
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
					
					num_def = DbNumDef.FromRawType (raw_type);
					System.Diagnostics.Debug.Assert (num_def != null);
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
		
		public static DbRawType MapToRawType(DbSimpleType simple_type, DbNumDef num_def)
		{
			switch (simple_type)
			{
				case DbSimpleType.Null:
					return DbRawType.Null;
				
				case DbSimpleType.Decimal:
					System.Diagnostics.Debug.Assert (num_def != null);
					
					if (num_def.IsConversionNeeded)
					{
						//	Ce n'est pas un type numérique standard, donc il faut prévoir
						//	une conversion éventuelle.
						
						int bits = num_def.MinimumBits;
						
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
						
						throw new DbFormatException (string.Format ("Unsupported numeric format, {0} bits required", bits));
					}
					
					return num_def.InternalRawType;
				
				case DbSimpleType.String:		return DbRawType.String;
				case DbSimpleType.Date:			return DbRawType.Date;
				case DbSimpleType.Time:			return DbRawType.Time;
				case DbSimpleType.DateTime:		return DbRawType.DateTime;
				case DbSimpleType.ByteArray:	return DbRawType.ByteArray;
				case DbSimpleType.Guid:			return DbRawType.Guid;
			}
			
			return DbRawType.Unsupported;
		}
		
		
		public static bool IsCompatibleToSimpleType(System.Type type, DbSimpleType simple_type)
		{
			if (TypeConverter.type_hash.Contains (type))
			{
				//	Le type natif est connu, on le compare simplement au type simplifié qui
				//	a été stocké dans la table.
				
				DbSimpleType analysed_type = (DbSimpleType) TypeConverter.type_hash[type];
				
				return analysed_type == simple_type;
			}
			
			return false;
		}
		
		
		/// <summary>
		/// Convertit un objet en provenance de ADO.NET en sa représentation équivalente
		/// basée sur le type simplifié <c>simple_type</c> et la définition de son format
		/// numérique. Cette méthode peut transformer une donnée numérique de manière
		/// significative.
		/// </summary>
		/// <param name="value">objet brut fourni par ADO.NET</param>
		/// <param name="simple_type">type simplifié attendu</param>
		/// <param name="num_def">format numérique attendu</param>
		/// <returns>objet converti</returns>
		public static object ConvertToSimpleType(object value, DbSimpleType simple_type, DbNumDef num_def)
		{
			if (value == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Unsupported);
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Null);
			
			switch (simple_type)
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
						throw new DbFormatException (string.Format ("Expected numeric format, got {0}", value.GetType ().ToString ()));
					}
						
					if (num_def.IsConversionNeeded)
					{
						long i64 = (long) num;
						num = num_def.ConvertFromInt64 (i64);
					}
					
					if (num_def.CheckCompatibility (num))
					{
						return num;
					}
					
					throw new DbFormatException (string.Format ("Incompatible numeric format for {0}", num));
				
				case DbSimpleType.String:		return value;
				case DbSimpleType.Date:			return new System.Date (value);
				case DbSimpleType.Time:			return new System.Time (value);
				case DbSimpleType.DateTime:		return value;
				case DbSimpleType.ByteArray:	return value;
				case DbSimpleType.Guid:			return value;
			}
			
			return null;
		}
		
		/// <summary>
		/// Convertit un objet basé sur le type simplifié <c>simple_type</c> et la définition
		/// de son format numérique en un objet compatible avec ADO.NET. Cette méthode peut
		/// transformer une donnée numérique de manière siginificative.
		/// </summary>
		/// <param name="value">objet de type simplifié</param>
		/// <param name="simple_type">type de l'objet</param>
		/// <param name="num_def">format numérique de l'objet</param>
		/// <returns>objet converti</returns>
		public static object ConvertFromSimpleType(object value, DbSimpleType simple_type, DbNumDef num_def)
		{
			if (value == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Unsupported);
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Null);
			
			switch (simple_type)
			{
				case DbSimpleType.Decimal:
					System.Diagnostics.Debug.Assert (value is decimal);
					
					//	Le type simplifié numérique "Decimal" est toujours représenté au moyen du
					//	type decimal de .NET. Par contre, son type ADO.NET peut être très différent,
					//	en fonction des définitions numériques fournies par num_def.
					//
					//	En particulier:
					//
					//	- Entiers 16, 32 et 64-bits, booléens, stockés comme tels.
					//	- Nombres à virgule fixe (SmallDecimal, LargeDecimal) stockés comme tels.
					//	- Autres formats à virgule ou offset, nécessitant une conversion vers un
					//	  format numérique neutre Int64.
					
					decimal num = (decimal) value;
					
					switch (num_def.InternalRawType)
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
					
					long i64 = num_def.ConvertToInt64 (num);
					
					if (num_def.MinimumBits <= 16)		return (System.Int16) i64;
					if (num_def.MinimumBits <= 32)		return (System.Int32) i64;
					
					return i64;
				
				case DbSimpleType.String:
					System.Diagnostics.Debug.Assert (value is string);
					return value;
				
				case DbSimpleType.Date:
				case DbSimpleType.Time:
				case DbSimpleType.DateTime:
					if (value is System.Date)
					{
						System.Date date = (System.Date) value;
						return date.ToDateTime ();
					}
					if (value is System.Time)
					{
						System.Time time = (System.Time) value;
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
			
			throw new DbFormatException (string.Format ("Invalid value format {0}, cannot convert to {1}", value.GetType ().ToString (), simple_type.ToString ()));
		}
		
		
		
		public static System.IFormatProvider		CurrentFormatProvider
		{
			get { return System.Globalization.CultureInfo.CurrentCulture; }
		}
		
		public static System.IFormatProvider		InvariantFormatProvider
		{
			get { return System.Globalization.CultureInfo.InvariantCulture; }
		}
		
		
		private static System.Collections.Hashtable type_hash;
	}
}
