//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbNumDef d�finit un format num�rique.
	/// digit_precision d�fini le nombre de digits acceptable pour le nombre (1 � 24)
	/// digit_shift d�fini la position de la virgule
	/// par exemple, precision = 5, shift = 3, accepte les nombres de -99.999 � +99.999
	/// </summary>
	public class DbNumDef : System.ICloneable
	{
		public DbNumDef()
		{
		}
		
		public DbNumDef(int digit_precision)
		{
			System.Diagnostics.Debug.Assert ((digit_precision >= 0) && (digit_precision < DbNumDef.digit_max));
			this.digit_precision = digit_precision;
		}
		
		public DbNumDef(int digit_precision, int digit_shift)
		{
			System.Diagnostics.Debug.Assert ((digit_precision >= 0) && (digit_precision < DbNumDef.digit_max));
			System.Diagnostics.Debug.Assert ((digit_shift >= 0) && (digit_shift < DbNumDef.digit_max));
			
			this.digit_precision = digit_precision;
			this.digit_shift     = digit_shift;
		}
		
		public DbNumDef(int digit_precision, int digit_shift, decimal min_value, decimal max_value)
		{
			System.Diagnostics.Debug.Assert ((digit_precision >= 0) && (digit_precision < DbNumDef.digit_max));
			System.Diagnostics.Debug.Assert ((digit_shift >= 0) && (digit_shift < DbNumDef.digit_max));
			
			this.digit_precision = digit_precision;
			this.digit_shift     = digit_shift;
			this.min_value       = min_value;
			this.max_value       = max_value;

			this.InvalidateAndUpdateAutoPrecision ();
		}
		
		
		#region ICloneable Members
		public virtual object Clone()
		{
			//	On ne fait pas un new DbNumDef, car on veut pouvoir r�utiliser ce code
			//	par h�ritage dans d'�ventuelles classes d�riv�es. Vive le dynamisme !
			
			DbNumDef def = System.Activator.CreateInstance (this.GetType ()) as DbNumDef;
			
			def.raw_type  = this.raw_type;
			
			def.min_value = this.min_value;
			def.max_value = this.max_value;
			
			def.digit_precision = this.digit_precision;
			def.digit_shift     = this.digit_shift;
			
			def.InvalidateAndUpdateAutoPrecision ();
			
			return def;
		}
		#endregion
		
		
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
		
		
		protected void UpdateAutoPrecision()
		{
			//	D�termine la pr�cision n�cessaire � la repr�sentation d'un nombre donn�.
			
			//	La routine donne � la fois la pr�cision (nombre total de d�cimales) et
			//	le "shift" (nombre de d�cimales apr�s le point d�cimal).
			
			//	Le r�sultat est m�moris� de mani�re � ne pas devoir recalculer cela �
			//	chaque fois que n�cessaire.
			
			System.Diagnostics.Debug.Assert (this.IsMinMaxDefined);		
			System.Diagnostics.Debug.Assert (this.digit_shift_auto == 0);
			System.Diagnostics.Debug.Assert (this.digit_precision_auto == 0);
			
			decimal min = System.Math.Abs (this.MinValue);
			decimal max = System.Math.Abs (this.MaxValue);

			//	Cherche d�j� le nombre de d�cimales :
			
			while ((System.Decimal.Truncate(min) != min) ||
				   (System.Decimal.Truncate(max) != max))
			{
				this.digit_shift_auto++;
				min *= 10M; 
				max *= 10M; 
			}
			System.Diagnostics.Debug.Assert (this.digit_shift_auto < DbNumDef.digit_max);
			
			//	Puis cherche le nombre de digits :
			
			while ((DbNumDef.digit_table[this.digit_precision_auto] <= min) || 
				   (DbNumDef.digit_table[this.digit_precision_auto] <= max))
			{
				this.digit_precision_auto++;
			}
			System.Diagnostics.Debug.Assert (this.digit_precision_auto < DbNumDef.digit_max);
		}
		
		protected void InvalidateAndUpdateAutoPrecision()
		{
			this.digit_precision_auto = 0;
			this.digit_shift_auto     = 0;

			if (this.IsMinMaxDefined)
			{
				//	D�termine de suite la pr�cision selon Min et Max
				this.UpdateAutoPrecision ();
			}
		}

		[System.Diagnostics.Conditional ("DEBUG")] protected void DebugCheckAbsolute(decimal value)
		{
			//	Utilis� pour v�rifier que ni le min_value, ni le max_value
			//	n'est pas donn� au del� des 24 digits autoris�s. Cette m�thode
			//	de v�rification n'est pas appel�e si le projet est compil� en
			//	mode 'Release'.
			
			System.Diagnostics.Debug.Assert (value >= min_absolute);
			System.Diagnostics.Debug.Assert (value <= max_absolute);
		}
		
		
		public int						DigitPrecision
		{
			get
			{
				if (this.digit_precision == 0)
				{
					return this.digit_precision_auto;
				}
				
				return this.digit_precision;
			}
			set
			{
				//	On ne touche pas aux valeurs min/max, car l'utilisateur a peut-�tre d�cid�
				//	de sp�cifier le min, le max, la pr�cision puis le shift. C'est superflu,
				//	mais il ne faudrait pas que �a alt�re les d�finitions.
				
				this.digit_precision = value;
			}
		}
		
		public int						DigitShift
		{
			get
			{ 
				if (this.digit_precision == 0)
				{
					return this.digit_shift_auto;
				}
				return this.digit_shift;
			}
			set 
			{
				//	Cf. remarque DigitPrecision.
				this.digit_shift = value; 
			}
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
				return this.min_value < this.max_value;
			}
		}
		
		public bool						IsConversionNeeded
		{
			get
			{
				return this.raw_type == DbRawType.Unsupported;
			}
		}
		
		public DbRawType				InternalRawType
		{
			get
			{
				return this.raw_type;
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
				
				//	MaxValue est peut-�tre d�finie. Si ce n'est pas le cas, il y a un
				//	algorithme dans MaxValue qui d�termine une valeur plausible en
				//	fonction du nombre de d�cimales.
				
				return - this.MaxValue;
			}
			set
			{
				this.DebugCheckAbsolute (value);

				this.min_value = value;
				this.InvalidateAndUpdateAutoPrecision ();				
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
				
				//	Le maximum n'est pas d�fini, alors on va utiliser une valeur automatique
				//	li�e au nombre de d�cimales et � la pr�cision (nombre de d�cimales apr�s
				//	le point d�cimal) :
				
				decimal max = DbNumDef.digit_table[this.digit_precision] - 1M;
				
				System.Diagnostics.Debug.Assert (max == decimal.Truncate (max));
				
				max *= DbNumDef.digit_table_scale[this.digit_shift];
				
				return max;
			}
			set
			{
				this.DebugCheckAbsolute (value);

				this.max_value = value;
				this.InvalidateAndUpdateAutoPrecision ();
			}
		}
		
		public int						MinimumBits
		{
			get
			{
				//	Calcule le nombre de bits n�cessaire pour repr�senter un nombre
				//	compris entre le minimum et le maximum (bornes comprises), en tenant
				//	en outre compte de l'�chelle (shift).

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
				
				//	Le nombre ne correspond � aucun format pr�d�fini. On va donc convertir
				//	la plus grande valeur possible en une repr�sentation "transportable"
				//	(entier positif) et d�terminer combien de bits sont n�cessaires � son
				//	stockage :
				
				long span = this.ConvertToInt64 (this.MaxValue);
				int  bits = 0;
				
				System.Diagnostics.Debug.Assert (span > 0);
				
				while (span > 0)
				{
					span = span >> 1;
					bits++;
				}
				
				return bits;
			}
		}
		
		public bool						IsValid
		{
			get
			{
				if (this.IsMinMaxDefined && this.IsDigitDefined)
				{
					//	Contr�le que la pr�cision donn�e est suffisante pour repr�senter le min et le max
					if ( this.digit_precision < this.digit_precision_auto ) return false;
					if ( this.digit_shift     < this.digit_shift_auto     ) return false;
				}
				return true;
			}
		}
		

		public bool CheckCompatibility(decimal value)
		{
			if ((value >= this.MinValue) && (value <= this.MaxValue))
			{
				//	V�rifie que le nombre de d�cimales apr�s le point d�cimal ne
				//	d�passe le maximum autoris� :
				
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
				//	Valeur compatible telle quelle, pas besoin de l'�diter !
				
				return true;
			}
			
			value = this.Round (value);
			value = this.Clip (value);
			
			return false;
		}
		
		
		public decimal Round(decimal value)
		{
			//	Arrondit la valeur selon l'�chelle (shift) :
			
			value *= DbNumDef.digit_table[this.digit_shift];
			value  = System.Decimal.Truncate (value + 0.5M);
			value *= DbNumDef.digit_table_scale[this.digit_shift];
			
			return value;
		}
		
		public decimal Clip(decimal value)
		{
			//	Limite la valeur aux bornes minimales/maximales actives.
			
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
		
		
		public long ConvertToInt64(decimal value)
		{
			//	Convertit (encode) la valeur d�cimale en une repr�sentation compacte,
			//	occupant au maximum 63 bits dans un entier positif.
			
			value -= this.MinValue;
			value *= DbNumDef.digit_table[this.DigitShift];
			
			return (long) value;
		}
		
		public decimal ConvertFromInt64(long value)
		{
			//	Convertit (d�code) une repr�sentation compacte g�n�r�e par la m�thode
			//	ConvertToInt64 en sa valeur d�cimale d'origine.
			
			decimal conv = value;
			conv *= DbNumDef.digit_table_scale[this.DigitShift];
			conv += this.MinValue;
			
			return (decimal) conv;
		}
		
		
		public decimal Parse(string value)
		{
			return this.Parse (value, System.Globalization.CultureInfo.CurrentCulture);
		}
		
		public decimal Parse(string value, System.IFormatProvider format_provider)
		{
			//	Convertit le texte en une valeur num�rique correspondante, v�rifie que la
			//	syntaxe est respect�e et que la valeur est dans les bornes admises. L�ve une
			//	exception dans le cas contraire...
			
			decimal	retval = System.Decimal.Parse (value, format_provider);
			
			if (!this.CheckCompatibility (retval))
			{
				throw new System.OverflowException (value);
			}
			
			return retval;
		}
		
		
		public string ToString(decimal value)
		{
			return this.ToString (value, System.Globalization.CultureInfo.CurrentCulture);
		}
		
		public string ToString(decimal value, System.IFormatProvider format_provider)
		{
			if (!this.CheckCompatibility (value))
			{
				throw new System.OverflowException (value.ToString (format_provider));
			}
			
			int frac_digits = this.DigitShift;
			
			//	Commence par faire en sorte de laisser tomber les z�ros superflus (ce qui
			//	peut arriver avec le type 'decimal', car 1.00M stocke en effet les deux
			//	d�cimales).
			
			value *= DbNumDef.digit_table[frac_digits];
			value  = decimal.Truncate (value);
			value *= DbNumDef.digit_table_scale[frac_digits];
			
			//	Convertit la valeur en un texte, en tenant compte des divers r�glages
			//	internes (nombre de d�cimales, etc.)
			
			//	(1) Convertit d�j� le nombre en cha�ne selon ToString de la classe "decimal" :
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			string decimal_string = value.ToString (format_provider);
			
			buffer.Append (decimal_string);
			
			//	(2) D�termine quel s�parateur utiliser :
			
			System.Globalization.CultureInfo culture = format_provider as System.Globalization.CultureInfo;
			string sep = (culture == null) ? "." : culture.NumberFormat.NumberDecimalSeparator;
			
			//	(3) Retrouve la position du s�parateur d�cimal dans le nombre :
			
			int pos = decimal_string.IndexOf (sep);
			
			if (pos < 0)
			{
				if (frac_digits > 0)
				{
					buffer.Append (sep);
				}
			}
			else
			{
				frac_digits -= decimal_string.Length - pos - 1;
				System.Diagnostics.Debug.Assert (frac_digits >= 0);
			}
			
			if (frac_digits > 0)
			{
				buffer.Append ('0', frac_digits);
			}
			
			return buffer.ToString ();
		}
		
		
		static DbNumDef()
		{
			//	Constructeur statique de la classe.
			
			//	Initialise la table de conversion entre nombre de d�cimales apr�s la
			//	virgule et facteur multiplicatif/facteur d'�chelle.
			
			DbNumDef.digit_table = new decimal[DbNumDef.digit_max];
			DbNumDef.digit_table_scale = new decimal[DbNumDef.digit_max];
			
			decimal multiple = 1;
			decimal scale = 1;
			
			for (int i = 0; i < DbNumDef.digit_max; i++)
			{
				DbNumDef.digit_table[i] = multiple;
				DbNumDef.digit_table_scale[i] = scale;
				multiple *= 10M;
				scale *= 0.1M;
			}
		}
		
		
		protected const int					digit_max		= 24;
		protected const decimal				max_absolute	= 999999999999999999999999.0M;
		protected const decimal				min_absolute	= -max_absolute;
		
		protected static decimal[]			digit_table;
		protected static decimal[]			digit_table_scale;
		
		protected DbRawType					raw_type = DbRawType.Unsupported;
		protected int						digit_precision;
		protected int						digit_shift;
		protected decimal					min_value		=  max_absolute;
		protected decimal					max_value		=  min_absolute;

		protected int						digit_precision_auto;	//	cache les val. d�t. selon Min et Max
		protected int						digit_shift_auto;		//	cache les val. d�t. selon Min et Max
	}
}
