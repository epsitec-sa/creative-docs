namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbNumDef définit un format numérique.
	/// </summary>
	public class DbNumDef
	{
		public DbNumDef()
		{
		}
		
		public DbNumDef(int digit_precision)
		{
			this.digit_precision = digit_precision;
		}
		
		public DbNumDef(int digit_precision, int digit_shift)
		{
			System.Diagnostics.Debug.Assert (digit_precision < DbNumDef.digit_max);
			System.Diagnostics.Debug.Assert (digit_shift < DbNumDef.digit_max);
			
			this.digit_precision = digit_precision;
			this.digit_shift     = digit_shift;
		}
		
		public DbNumDef(int digit_precision, int digit_shift, decimal min_value, decimal max_value)
		{
			System.Diagnostics.Debug.Assert (digit_precision < DbNumDef.digit_max);
			System.Diagnostics.Debug.Assert (digit_shift < DbNumDef.digit_max);
			
			this.digit_precision = digit_precision;
			this.digit_shift     = digit_shift;
			this.min_value       = min_value;
			this.max_value       = max_value;
		}
		
		
		public static DbNumDef FromRawType(DbRawType raw_type)
		{
			DbNumDef def;
			
			switch (raw_type)
			{
				case DbRawType.Boolean:
					def = new DbNumDef ( 1, 0, 0, 1);
					def.raw_type = raw_type;
					break;
				
				case DbRawType.Int16:
					def = new DbNumDef ( 5, 0, System.Int16.MinValue, System.Int16.MaxValue);
					def.raw_type = raw_type;
					break;
				
				case DbRawType.Int32:
					def = new DbNumDef (10, 0, System.Int32.MinValue, System.Int32.MaxValue);
					def.raw_type = raw_type;
					break;
				
				case DbRawType.Int64:
					def = new DbNumDef (19, 0, System.Int64.MinValue, System.Int64.MaxValue);
					def.raw_type = raw_type;
					break;
				
				case DbRawType.SmallDecimal:
					def = new DbNumDef (18, 9);
					def.raw_type = raw_type;
					break;
				
				case DbRawType.LargeDecimal:
					def = new DbNumDef (18, 3);
					def.raw_type = raw_type;
					break;
				
				default:
					def = null;
					break;
			}
			
			return def;
		}
		
		
		public int						DigitPrecision
		{
			get
			{
				if (this.digit_precision == 0)
				{
					//	TODO: extrait la précision des valeurs minimum et maximum.
					
					return 0;
				}
				
				return this.digit_precision;
			}
			set
			{
				this.digit_precision = value;
			}
		}
		
		public int						DigitShift
		{
			get { return this.digit_shift; }
			set { this.digit_shift = value; }
		}
		
		public bool						IsDigitDefined
		{
			get
			{
				return this.digit_precision > 0;
			}
		}
		
		public bool						IsMinMaxDefined
		{
			get
			{
				return this.min_value <= this.max_value;
			}
		}
		
		public bool						IsConversionNeeded
		{
			get
			{
				return this.raw_type == DbRawType.Unsupported;
			}
		}
		
		public decimal					MinValue
		{
			get
			{
				if (this.IsMinMaxDefined)
				{
					return this.min_value;
				}
				
				return - this.MaxValue;
			}
			set
			{
				this.min_value = value;
			}
		}
		
		public decimal					MaxValue
		{
			get
			{
				if (this.IsMinMaxDefined)
				{
					return this.max_value;
				}
				
				decimal max = DbNumDef.digit_table[this.digit_precision] - 1M;
				
				return max / DbNumDef.digit_table[this.digit_shift];
			}
			set
			{
				this.max_value = value;
			}
		}
		
		public int						MinimumBits
		{
			get
			{
				//	Calcule le nombre de bits nécessaire pour représenter un nombre
				//	compris entre le minimum et le maximum (bornes comprises), en tenant
				//	en outre compte de l'échelle (shift).
				
				switch (this.raw_type)
				{
					case DbRawType.Boolean:
						return 1;
					case DbRawType.Int16:
						return 16;
					case DbRawType.Int32:
						return 32;
					case DbRawType.Int64:
						return 64;
					case DbRawType.SmallDecimal:
					case DbRawType.LargeDecimal:
						return 64;
				}
				
				long span = this.ConvertToInt64 (this.MaxValue);
				
				System.Diagnostics.Debug.Assert (span > 0);
				
				int bits = 0;
				
				while (span > 0)
				{
					span = span >> 1;
					bits++;
				}
				
				return bits;
			}
		}
		
		
		public bool CheckCompatibility(decimal value)
		{
			if ((value >= this.MinValue) && (value <= this.MaxValue))
			{
				//	TODO: détermine le nombre de chiffres après la virgule et vérifie
				//	qu'elle n'excède pas DigitShift.
				
				decimal v1 = value * DbNumDef.digit_table[this.digit_shift];
				decimal v2 = System.Decimal.Truncate (v1);
				
				return v1 == v2;
			}
			
			return false;
		}
		
		public bool CheckCompatibilityAndClipRound(ref decimal value)
		{
			if (this.CheckCompatibility (value))
			{
				//	Valeur compatible telle quelle, pas besoin de l'éditer !
				
				return true;
			}
			
			value = this.Round (value);
			value = this.Clip (value);
			
			return false;
		}
		
		
		/// <summary>
		/// Arrondit la valeur selon l'échelle (shift).
		/// </summary>
		/// <param name="value">valeur à arrondir</param>
		/// <returns>valeur arrondie</returns>
		public decimal Round(decimal value)
		{
			value *= DbNumDef.digit_table[this.digit_shift];
			value  = System.Decimal.Truncate (value + 0.5M);
			value /= DbNumDef.digit_table[this.digit_shift];
			
			return value;
		}
		
		/// <summary>
		///	Limite la valeur aux bornes minimales/maximales actives.
		/// </summary>
		/// <param name="value">valeur à limiter</param>
		/// <returns>valeur limitée</returns>
		public decimal Clip(decimal value)
		{
			if (value < this.MinValue)
			{
				return this.MinValue;
			}
			if (value > this.MaxValue)
			{
				return this.MaxValue;
			}
			
			return value;
		}
		
		
		/// <summary>
		/// Convertit (encode) la valeur décimale en une représentation
		/// compacte, occupant au maximum 63 bits dans un entier positif.
		/// </summary>
		/// <param name="value">valeur à encoder</param>
		/// <returns>valeur encodée</returns>
		public long ConvertToInt64(decimal value)
		{
			value -= this.MinValue;
			value *= DbNumDef.digit_table[this.DigitShift];
			
			return (long) value;
		}
		
		/// <summary>
		/// Convertit (décode) une représentation compacte générée par
		/// <c>ConvertToInt64</c> en sa valeur décimale d'origine.
		/// </summary>
		/// <param name="value">valeur à décoder</param>
		/// <returns>valeur décodée</returns>
		public decimal ConvertFromInt64(long value)
		{
			decimal conv = value;
			conv /= DbNumDef.digit_table[this.DigitShift];
			conv += this.MinValue;
			
			return (decimal) conv;
		}
		
		
		public decimal Parse(string value)
		{
			return this.Parse (value, System.Globalization.CultureInfo.CurrentCulture);
		}
		
		public decimal Parse(string value, System.IFormatProvider format_provider)
		{
			//	TODO: convertit le texte en une valeur numérique correspondant, vérifie que la
			//	syntaxe est respectée et que la valeur est dans les bornes admises. Lève une
			//	exception dans le cas contraire...
			
			return System.Decimal.Parse (value, format_provider);
		}
		
		
		public string ToString(decimal value)
		{
			return this.ToString (value, System.Globalization.CultureInfo.CurrentCulture);
		}
		
		public string ToString(decimal value, System.IFormatProvider format_provider)
		{
			//	TODO: convertit la valeur en un texte, en tenant compte des divers réglages
			//	internes (nombre de décimales, etc.)
			
			return value.ToString (format_provider);
		}
		
		
		static DbNumDef()
		{
			//	Initialise la table de conversion entre nombre de décimales après la
			//	virgule et facteur multiplicatif.
			
			DbNumDef.digit_table = new decimal[DbNumDef.digit_max];
			decimal multiple = 1;
			
			for (int i = 0; i < DbNumDef.digit_max; i++)
			{
				DbNumDef.digit_table[i] = multiple;
				multiple *= 10.0M;
			}
		}
		
		
		protected static readonly int	digit_max	= 24;
		protected static decimal[]		digit_table;
		
		protected DbRawType				raw_type = DbRawType.Unsupported;
		protected int					digit_precision;
		protected int					digit_shift;
		protected decimal				min_value	=  0.0M;
		protected decimal				max_value	= -1.0M;
	}
}
