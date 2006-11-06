//	Copyright � 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbNumDef d�finit un format num�rique.
	/// DigitPrecision d�finit le nombre de digits acceptable pour le nombre (1 � 24)
	/// DigitShift d�finit la position de la virgule
	/// par exemple, precision = 5, shift = 3, accepte les nombres de -99.999 � +99.999
	/// </summary>
	public sealed class DbNumDef : System.ICloneable, System.IEquatable<DbNumDef>
	{
		public DbNumDef()
		{
		}

		public DbNumDef(DecimalRange range)
		{
			this.digit_shift     = (byte) range.FractionalDigits;
			this.digit_precision = (byte) range.GetMaximumDigitCount ();
		}
		
		public DbNumDef(int digit_precision)
		{
			System.Diagnostics.Debug.Assert ((digit_precision >= 0) && (digit_precision < DbNumDef.digit_max));
			this.digit_precision = (byte) digit_precision;
		}
		
		public DbNumDef(int digit_precision, int digit_shift)
		{
			System.Diagnostics.Debug.Assert ((digit_precision >= 0) && (digit_precision < DbNumDef.digit_max));
			System.Diagnostics.Debug.Assert ((digit_shift >= 0) && (digit_shift < DbNumDef.digit_max));

			this.digit_precision = (byte) digit_precision;
			this.digit_shift     = (byte) digit_shift;
		}
		
		public DbNumDef(int digit_precision, int digit_shift, decimal min_value, decimal max_value)
		{
			System.Diagnostics.Debug.Assert ((digit_precision >= 0) && (digit_precision < DbNumDef.digit_max));
			System.Diagnostics.Debug.Assert ((digit_shift >= 0) && (digit_shift < DbNumDef.digit_max));

			this.digit_precision = (byte) digit_precision;
			this.digit_shift     = (byte) digit_shift;
			this.min_value       = min_value;
			this.max_value       = max_value;

			this.InvalidateAndUpdateAutoPrecision ();
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
		
		
		public int								DigitPrecision
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

				this.digit_precision = (byte) value;
			}
		}
		
		public int								DigitShift
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
				this.digit_shift = (byte) value; 
			}
		}
		
		public bool								IsDigitDefined
		{
			get
			{
				return this.digit_precision > 0;
			}
		}
		
		public bool								IsMinMaxDefined
		{
			get
			{
				return this.min_value < this.max_value;
			}
		}
		
		public bool								IsConversionNeeded
		{
			get
			{
				return this.raw_type == DbRawType.Unsupported;
			}
		}
		
		public DbRawType						InternalRawType
		{
			get
			{
				return this.raw_type;
			}
		}
		
		public decimal							MinValue
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
		
		public decimal							MaxValue
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
		
		public int								MinimumBits
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
		
		public bool								IsValid
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
		
		
		public Common.Types.DecimalRange ToDecimalRange()
		{
			decimal resolution = DbNumDef.digit_table_scale[this.DigitShift];
			return new Common.Types.DecimalRange (this.MinValue, this.MaxValue, resolution);
		}
		
		
		#region ICloneable Members
		
		public object Clone()
		{
			DbNumDef copy = new DbNumDef ();
			
			copy.raw_type  = this.raw_type;
			
			copy.min_value = this.min_value;
			copy.max_value = this.max_value;
			
			copy.digit_precision = this.digit_precision;
			copy.digit_shift     = this.digit_shift;
			
			copy.InvalidateAndUpdateAutoPrecision ();
			
			return copy;
		}

		#endregion

		#region IEquatable<DbNumDef> Members

		public bool Equals(DbNumDef other)
		{
			return this == other;
		}

		#endregion

		public override bool Equals(object obj)
		{
			return this.Equals (obj as DbNumDef);
		}

		public override int GetHashCode()
		{
			int hash = (this.digit_precision << 8) | (this.digit_shift);
			
			return this.raw_type.GetHashCode () ^ this.min_value.GetHashCode () ^ this.max_value.GetHashCode () ^ hash;
		}

		public static bool operator==(DbNumDef a, DbNumDef b)
		{
			if (object.ReferenceEquals (a, b))
			{
				return true;
			}
			if (object.ReferenceEquals (a, null))
			{
				return false;
			}
			if (object.ReferenceEquals (b, null))
			{
				return false;
			}

			return (a.raw_type == b.raw_type)
				&& (a.min_value == b.min_value)
				&& (a.max_value == b.max_value)
				&& (a.digit_precision == b.digit_precision)
				&& (a.digit_shift == b.digit_shift);
		}

		public static bool operator!=(DbNumDef a, DbNumDef b)
		{
			return !(a == b);
		}


		private void UpdateAutoPrecision()
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
				System.Diagnostics.Debug.Assert (this.digit_precision_auto < DbNumDef.digit_max);
			}
		}

		private void InvalidateAndUpdateAutoPrecision()
		{
			this.digit_precision_auto = 0;
			this.digit_shift_auto     = 0;

			if (this.IsMinMaxDefined)
			{
				//	D�termine de suite la pr�cision selon Min et Max
				this.UpdateAutoPrecision ();
			}
		}


		[System.Diagnostics.Conditional ("DEBUG")]
		private void DebugCheckAbsolute(decimal value)
		{
			//	Utilis� pour v�rifier que ni le min_value, ni le max_value
			//	n'est pas donn� au del� des 24 digits autoris�s. Cette m�thode
			//	de v�rification n'est pas appel�e si le projet est compil� en
			//	mode 'Release'.
			
			System.Diagnostics.Debug.Assert (value >= min_absolute);
			System.Diagnostics.Debug.Assert (value <= max_absolute);
		}


		public void Serialize(System.Xml.XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("numdef");
			this.SerializeAttributes (xmlWriter, "");
			xmlWriter.WriteEndElement ();
		}

		public void SerializeAttributes(System.Xml.XmlTextWriter xmlWriter, string prefix)
		{
			if (this.InternalRawType == DbRawType.Unsupported)
			{
				xmlWriter.WriteAttributeString (prefix+"digits", InvariantConverter.ToString (this.DigitPrecision));
				xmlWriter.WriteAttributeString (prefix+"shift", InvariantConverter.ToString (this.DigitShift));
				xmlWriter.WriteAttributeString (prefix+"min", InvariantConverter.ToString (this.MinValue));
				xmlWriter.WriteAttributeString (prefix+"max", InvariantConverter.ToString (this.MaxValue));
			}
			else
			{
				xmlWriter.WriteAttributeString (prefix+"raw", DbTools.RawTypeToString (this.InternalRawType));
			}
		}
		
		public static DbNumDef Deserialize(System.Xml.XmlTextReader xmlReader)
		{
			if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
				(xmlReader.Name == "numdef"))
			{
				return DbNumDef.DeserializeAttributes (xmlReader);
			}
			else
			{
				throw new System.Xml.XmlException (string.Format ("Unexpected element {0}", xmlReader.LocalName), null, xmlReader.LineNumber, xmlReader.LinePosition);
			}
		}

		public static DbNumDef DeserializeAttributes(System.Xml.XmlTextReader xmlReader)
		{
			return DbNumDef.DeserializeAttributes (xmlReader, "");
		}

		public static DbNumDef DeserializeAttributes(System.Xml.XmlTextReader xmlReader, string prefix)
		{
			string rawArg = xmlReader.GetAttribute (prefix+"raw");

			if (rawArg == null)
			{
				int digits  = InvariantConverter.ParseInt (xmlReader.GetAttribute (prefix+"digits"));
				int shift   = InvariantConverter.ParseInt (xmlReader.GetAttribute (prefix+"shift"));
				decimal min = InvariantConverter.ParseDecimal (xmlReader.GetAttribute (prefix+"min"));
				decimal max = InvariantConverter.ParseDecimal (xmlReader.GetAttribute (prefix+"max"));

				if ((digits == 0) &&
					(shift == 0) &&
					(min == 0) &&
					(max == 0))
				{
					return null;
				}
				else
				{
					return new DbNumDef (digits, shift, min, max);
				}
			}
			else
			{
				return DbNumDef.FromRawType (DbTools.ParseRawType (rawArg));
			}
		}


		
		
		#region Static Setup
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
		#endregion		
		
		private const int digit_max		= 24;
		private const decimal max_absolute	= 99999999999999999999999.0M;
		private const decimal min_absolute	= -max_absolute;

		private static decimal[] digit_table;
		private static decimal[] digit_table_scale;

		private DbRawType raw_type		= DbRawType.Unsupported;
		private decimal min_value		=  max_absolute;
		private decimal max_value		=  min_absolute;

		private byte digit_precision;
		private byte digit_shift;
		private byte digit_precision_auto;	//	cache les val. d�t. selon Min et Max
		private byte digit_shift_auto;		//	cache les val. d�t. selon Min et Max
	}
}
