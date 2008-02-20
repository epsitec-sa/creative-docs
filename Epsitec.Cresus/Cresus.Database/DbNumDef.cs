//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbNumDef</c> class defines a numeric format used to represent
	/// decimal and integer numbers. A number has at most <c>DigitPrecision</c>
	/// digits (1..24). The <c>DigitShift</c> specifies the number of digits
	/// after the decimal point.
	/// </summary>
	public sealed class DbNumDef : System.ICloneable, System.IEquatable<DbNumDef>, IXmlSerializable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbNumDef"/> class.
		/// </summary>
		public DbNumDef()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbNumDef"/> class.
		/// </summary>
		/// <param name="range">The range definition.</param>
		public DbNumDef(DecimalRange range)
			: this (range.GetMaximumDigitCount (), range.FractionalDigits, range.Minimum, range.Maximum)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbNumDef"/> class.
		/// </summary>
		/// <param name="digitPrecision">The digit precision.</param>
		public DbNumDef(int digitPrecision)
		{
			System.Diagnostics.Debug.Assert ((digitPrecision >= 0) && (digitPrecision < DbNumDef.digitMax));
			this.digitPrecision = (byte) digitPrecision;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbNumDef"/> class.
		/// </summary>
		/// <param name="digitPrecision">The digit precision.</param>
		/// <param name="digitShift">The digit shift.</param>
		public DbNumDef(int digitPrecision, int digitShift)
		{
			System.Diagnostics.Debug.Assert ((digitPrecision >= 0) && (digitPrecision < DbNumDef.digitMax));
			System.Diagnostics.Debug.Assert ((digitShift >= 0) && (digitShift < DbNumDef.digitMax));

			this.digitPrecision = (byte) digitPrecision;
			this.digitShift     = (byte) digitShift;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbNumDef"/> class.
		/// </summary>
		/// <param name="digitPrecision">The digit precision.</param>
		/// <param name="digitShift">The digit shift.</param>
		/// <param name="minValue">The minimum value.</param>
		/// <param name="maxValue">The maximum value.</param>
		public DbNumDef(int digitPrecision, int digitShift, decimal minValue, decimal maxValue)
		{
			System.Diagnostics.Debug.Assert ((digitPrecision >= 0) && (digitPrecision < DbNumDef.digitMax));
			System.Diagnostics.Debug.Assert ((digitShift >= 0) && (digitShift < DbNumDef.digitMax));

			this.digitPrecision = (byte) digitPrecision;
			this.digitShift     = (byte) digitShift;
			this.minValue       = minValue;
			this.maxValue       = maxValue;

			this.InvalidateAndUpdateAutoPrecision ();

			if (this.digitShift == 0)
			{
				if ((this.minValue == System.Int16.MinValue) &&
					(this.maxValue == System.Int16.MaxValue))
				{
					this.rawType = DbRawType.Int16;
				}
				else if ((this.minValue == System.Int32.MinValue) &&
					     (this.maxValue == System.Int32.MaxValue))
				{
					this.rawType = DbRawType.Int32;
				}
				else if ((this.minValue == System.Int64.MinValue) &&
					     (this.maxValue == System.Int64.MaxValue))
				{
					this.rawType = DbRawType.Int64;
				}
			}
		}


		/// <summary>
		/// Creates a <c>DbNumDef</c> matching a specific raw type.
		/// </summary>
		/// <param name="rawType">The raw type.</param>
		/// <returns>The <c>DbNumDef</c>.</returns>
		public static DbNumDef FromRawType(DbRawType rawType)
		{
			DbNumDef def;

			switch (rawType)
			{
				case DbRawType.Boolean:
					def = new DbNumDef (1, 0, 0, 1);
					def.rawType = rawType;
					break;

				case DbRawType.Int16:
					def = new DbNumDef (5, 0, System.Int16.MinValue, System.Int16.MaxValue);
					def.rawType = rawType;
					break;

				case DbRawType.Int32:
					def = new DbNumDef (10, 0, System.Int32.MinValue, System.Int32.MaxValue);
					def.rawType = rawType;
					break;

				case DbRawType.Int64:
					def = new DbNumDef (19, 0, System.Int64.MinValue, System.Int64.MaxValue);
					def.rawType = rawType;
					break;

				case DbRawType.SmallDecimal:
					def = new DbNumDef (18, 9);
					def.rawType = rawType;
					break;

				case DbRawType.LargeDecimal:
					def = new DbNumDef (18, 3);
					def.rawType = rawType;
					break;

				default:
					def = null;
					break;
			}

			return def;
		}


		/// <summary>
		/// Gets or sets the digit precision.
		/// </summary>
		/// <value>The digit precision.</value>
		public int								DigitPrecision
		{
			get
			{
				if (this.digitPrecision == 0)
				{
					return this.digitPrecisionAuto;
				}
				else
				{
					return this.digitPrecision;
				}
			}
			set
			{
				//	On ne touche pas aux valeurs min/max, car l'utilisateur a peut-être décidé
				//	de spécifier le min, le max, la précision puis le shift. C'est superflu,
				//	mais il ne faudrait pas que ça altère les définitions.

				this.digitPrecision = (byte) value;
			}
		}

		/// <summary>
		/// Gets or sets the digit shift.
		/// </summary>
		/// <value>The digit shift.</value>
		public int								DigitShift
		{
			get
			{
				if (this.digitPrecision == 0)
				{
					return this.digitShiftAuto;
				}
				else
				{
					return this.digitShift;
				}
			}
			set 
			{
				//	Cf. remarque DigitPrecision.
				this.digitShift = (byte) value; 
			}
		}

		/// <summary>
		/// Gets a value indicating whether a digit precision is defined.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if a digit precision is defined; otherwise, <c>false</c>.
		/// </value>
		public bool								IsDigitDefined
		{
			get
			{
				return this.digitPrecision > 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the minimum and maximum values are defined.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the minimum and maximum values are defined; otherwise, <c>false</c>.
		/// </value>
		public bool								IsMinMaxDefined
		{
			get
			{
				return this.minValue < this.maxValue;
			}
		}

		/// <summary>
		/// Gets a value indicating whether numbers adhering to this <c>DbNumDef</c>
		/// need to be converted.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if numbers adhering to this <c>DbNumDef</c> need to be converted; otherwise, <c>false</c>.
		/// </value>
		public bool								IsConversionNeeded
		{
			get
			{
				if (this.rawType == DbRawType.Unknown)
				{
					if ((this.DigitShift == 0) &&
						(this.GetPackOffset () == 0))
					{
						return false;
					}
					else
					{
						return true;
					}
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the internal raw type.
		/// </summary>
		/// <value>The internal raw type or <c>DbRawType.Unknown</c> if none has been specified.</value>
		public DbRawType						InternalRawType
		{
			get
			{
				return this.rawType;
			}
		}

		/// <summary>
		/// Gets or sets the minimum value.
		/// </summary>
		/// <value>The minimum value.</value>
		public decimal							MinValue
		{
			get
			{
				if (this.IsMinMaxDefined)
				{
					return this.minValue;
				}
				
				//	MaxValue est peut-être définie. Si ce n'est pas le cas, il y a un
				//	algorithme dans MaxValue qui détermine une valeur plausible en
				//	fonction du nombre de décimales.
				
				return - this.MaxValue;
			}
			set
			{
				this.DebugCheckAbsolute (value);

				this.minValue = value;
				this.InvalidateAndUpdateAutoPrecision ();				
			}
		}

		/// <summary>
		/// Gets or sets the maximum value.
		/// </summary>
		/// <value>The maximum value.</value>
		public decimal							MaxValue
		{
			get
			{
				if (this.IsMinMaxDefined)
				{
					return this.maxValue;
				}
				
				//	Le maximum n'est pas défini, alors on va utiliser une valeur automatique
				//	liée au nombre de décimales et à la précision (nombre de décimales après
				//	le point décimal) :

				decimal max = DbNumDef.digitTable[this.digitPrecision] - 1M;
				
				System.Diagnostics.Debug.Assert (max == decimal.Truncate (max));

				max *= DbNumDef.digitTableScale[this.digitShift];
				
				return max;
			}
			set
			{
				this.DebugCheckAbsolute (value);

				this.maxValue = value;
				this.InvalidateAndUpdateAutoPrecision ();
			}
		}

		/// <summary>
		/// Gets the half range defined as the difference between the maximum
		/// and the minium divided by two, rounded upwards.
		/// </summary>
		/// <value>The half range.</value>
		internal decimal						HalfRange
		{
			get
			{
				return (this.MaxValue - this.MinValue + DbNumDef.digitTableScale[this.DigitShift]) / 2;
			}
		}

		/// <summary>
		/// Gets the minimum number of bits required to encode the full range
		/// of numbers.
		/// </summary>
		/// <value>The minimum number of bits.</value>
		public int								MinimumBits
		{
			get
			{
				//	Calcule le nombre de bits nécessaire pour représenter un nombre
				//	compris entre le minimum et le maximum (bornes comprises), en tenant
				//	en outre compte de l'échelle (shift).

				switch (this.rawType)
				{
					case DbRawType.Boolean:			return 1;
					case DbRawType.Int16:			return 16;
					case DbRawType.Int32:			return 32;
					case DbRawType.Int64:			return 64;
					case DbRawType.SmallDecimal:	return 64;
					case DbRawType.LargeDecimal:	return 64;
				}
				
				//	Le nombre ne correspond à aucun format prédéfini. On va donc convertir
				//	la plus grande valeur possible en une représentation "transportable"
				//	(entier positif) et déterminer combien de bits sont nécessaires à son
				//	stockage :
				
				ulong span = this.ConvertToUnsignedInt64 (this.MaxValue);
				int   bits = 0;
				
				System.Diagnostics.Debug.Assert (span != 0);
				
				while (span != 0)
				{
					span = span >> 1;
					bits++;
				}
				
				return bits;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
		public bool								IsValid
		{
			get
			{
				if ((this.IsMinMaxDefined) &&
					(this.IsDigitDefined))
				{
					//	Contrôle que la précision donnée est suffisante pour représenter
					//	les bornes extrêmes.

					if (this.digitPrecision < this.digitPrecisionAuto)
					{
						return false;
					}
					if (this.digitShift < this.digitShiftAuto)
					{
						return false;
					}
				}
				
				return true;
			}
		}


		/// <summary>
		/// Checks if a value is compatible with the decimal range, including
		/// the precision (a value having to many digits will be considered
		/// to be incompatible).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the value is compatible; otherwise, <c>false</c>.</returns>
		public bool CheckCompatibility(decimal value)
		{
			if ((value >= this.MinValue) &&
				(value <= this.MaxValue))
			{
				//	Vérifie que le nombre de décimales après le point décimal ne
				//	dépasse le maximum autorisé :

				decimal v1 = value * DbNumDef.digitTable[this.digitShift];
				decimal v2 = System.Decimal.Truncate (v1);
				
				return v1 == v2;
			}
			
			return false;
		}

		/// <summary>
		/// Checks if a value is compatible; clip and round the value.
		/// </summary>
		/// <param name="value">The value to clip and round.</param>
		/// <returns><c>true</c> if the value is compatible; otherwise, <c>false</c>.</returns>
		public bool CheckCompatibilityAndClipRound(ref decimal value)
		{
			if (this.CheckCompatibility (value))
			{
				//	Valeur compatible telle quelle, pas besoin de l'éditer !

				return true;
			}
			else
			{
				value = this.Round (value);
				value = this.Clip (value);

				return false;
			}
		}


		/// <summary>
		/// Rounds the specified value based on the defined precision.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The rounded value.</returns>
		public decimal Round(decimal value)
		{
			value *= DbNumDef.digitTable[this.digitShift];
			value  = System.Decimal.Truncate (value + 0.5M);
			value *= DbNumDef.digitTableScale[this.digitShift];
			
			return value;
		}

		/// <summary>
		/// Clips the specified value to the defined range.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The clipped value.</returns>
		public decimal Clip(decimal value)
		{
			if (value < this.MinValue)
			{
				return this.MinValue;
			}
			else if (value > this.MaxValue)
			{
				return this.MaxValue;
			}
			else
			{
				return value;
			}
		}


		/// <summary>
		/// Converts the value to an encoded 64-bit unsigned integer. The encoding
		/// is simply a distance from the minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The encoded value.</returns>
		private ulong ConvertToUnsignedInt64(decimal value)
		{
			value -= this.MinValue;
			value *= DbNumDef.digitTable[this.DigitShift];

			return (ulong) value;
		}

		/// <summary>
		/// Converts the value to an encoded 64-bit integer. This attempts to
		/// make the encoding as transparent as possible (e.g. only scaling,
		/// if possible).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The encoded value.</returns>
		public long ConvertToInt64(decimal value)
		{
			value -= this.GetPackOffset ();
			value *= DbNumDef.digitTable[this.DigitShift];
			
			return (long) value;
		}

		/// <summary>
		/// Converts from back from an encoded 64-bit integer to a decimal
		/// value.
		/// </summary>
		/// <param name="value">The encoded value.</param>
		/// <returns>The decimal value.</returns>
		public decimal ConvertFromInt64(long value)
		{
			decimal conv = value;
			
			conv *= DbNumDef.digitTableScale[this.DigitShift];
			conv += this.GetPackOffset ();
			
			return (decimal) conv;
		}


		/// <summary>
		/// Parses the specified value and checks that the value is compatible.
		/// If not, an exception will be raised.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <returns>The value parsed using the current culture.</returns>
		public decimal Parse(string value)
		{
			return this.Parse (value, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Parses the specified value and checks that the value is compatible.
		/// If not, an exception will be raised.
		/// </summary>
		/// <param name="value">The string.</param>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns>The value parsed using the specified format provider.</returns>
		public decimal Parse(string value, System.IFormatProvider formatProvider)
		{
			decimal	retval = System.Decimal.Parse (value, formatProvider);
			
			if (!this.CheckCompatibility (retval))
			{
				throw new System.OverflowException (value);
			}
			
			return retval;
		}


		/// <summary>
		/// Converts the decimal value to a string. This takes in account the
		/// digit resolution and adds trailing zeroes, if needed.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The string representation using the current culture.</returns>
		public string ToString(decimal value)
		{
			return this.ToString (value, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Converts the decimal value to a string. This takes in account the
		/// digit resolution and adds trailing zeroes, if needed.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns>The string representation using the specified format provider.</returns>
		public string ToString(decimal value, System.IFormatProvider formatProvider)
		{
			if (!this.CheckCompatibility (value))
			{
				throw new System.OverflowException (value.ToString (formatProvider));
			}
			
			int fracDigits = this.DigitShift;
			
			//	Commence par faire en sorte de laisser tomber les zéros superflus (ce qui
			//	peut arriver avec le type 'decimal', car 1.00M stocke en effet les deux
			//	décimales).
			
			value *= DbNumDef.digitTable[fracDigits];
			value  = decimal.Truncate (value);
			value *= DbNumDef.digitTableScale[fracDigits];
			
			//	Convertit la valeur en un texte, en tenant compte des divers réglages
			//	internes (nombre de décimales, etc.)
			
			//	(1) Convertit déjà le nombre en chaîne selon ToString de la classe "decimal" :
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			string decimalString = value.ToString (formatProvider);
			
			buffer.Append (decimalString);
			
			//	(2) Détermine quel séparateur utiliser :
			
			System.Globalization.CultureInfo culture = formatProvider as System.Globalization.CultureInfo;
			string sep = (culture == null) ? "." : culture.NumberFormat.NumberDecimalSeparator;
			
			//	(3) Retrouve la position du séparateur décimal dans le nombre :
			
			int pos = decimalString.IndexOf (sep);
			
			if (pos < 0)
			{
				if (fracDigits > 0)
				{
					buffer.Append (sep);
				}
			}
			else
			{
				fracDigits -= decimalString.Length - pos - 1;
				System.Diagnostics.Debug.Assert (fracDigits >= 0);
			}
			
			if (fracDigits > 0)
			{
				buffer.Append ('0', fracDigits);
			}
			
			return buffer.ToString ();
		}


		/// <summary>
		/// Converts the <c>DbNumDef</c> to a decimal range encoding.
		/// </summary>
		/// <returns>The decimal range.</returns>
		public Common.Types.DecimalRange ToDecimalRange()
		{
			decimal resolution = DbNumDef.digitTableScale[this.DigitShift];
			return new Common.Types.DecimalRange (this.MinValue, this.MaxValue, resolution);
		}


		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>The clone of this <c>DbNumDef</c>.</returns>
		public DbNumDef Clone()
		{
			DbNumDef copy = new DbNumDef ();

			copy.rawType  = this.rawType;

			copy.minValue = this.minValue;
			copy.maxValue = this.maxValue;

			copy.digitPrecision = this.digitPrecision;
			copy.digitShift     = this.digitShift;

			copy.InvalidateAndUpdateAutoPrecision ();

			return copy;
		}
		
		#region ICloneable Members
		
		object System.ICloneable.Clone()
		{
			return this.Clone ();
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
			int hash = (this.digitPrecision << 8) | (this.digitShift);
			
			hash ^= this.rawType.GetHashCode ();
			hash ^= this.minValue.GetHashCode ();
			hash ^= this.maxValue.GetHashCode ();
			
			return hash;
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

			return (a.rawType == b.rawType)
				&& (a.minValue == b.minValue)
				&& (a.maxValue == b.maxValue)
				&& (a.digitPrecision == b.digitPrecision)
				&& (a.digitShift == b.digitShift);
		}

		public static bool operator!=(DbNumDef a, DbNumDef b)
		{
			return !(a == b);
		}

		#region IXmlSerializable Members

		/// <summary>
		/// Serializes the instance using the specified XML writer.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		public void Serialize(System.Xml.XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("numdef");
			this.SerializeAttributes (xmlWriter);
			xmlWriter.WriteEndElement ();
		}

		#endregion

		/// <summary>
		/// Serializes the instance as XML attributes.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		public void SerializeAttributes(System.Xml.XmlTextWriter xmlWriter)
		{
			this.SerializeAttributes (xmlWriter, "");
		}

		/// <summary>
		/// Serializes the instance as XML attributes. The attributes will be
		/// prefixed as specified.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		/// <param name="prefix">The prefix.</param>
		public void SerializeAttributes(System.Xml.XmlTextWriter xmlWriter, string prefix)
		{
			if (this.InternalRawType == DbRawType.Unknown)
			{
				DbTools.WriteAttribute (xmlWriter, prefix+"digits", DbTools.IntToString (this.DigitPrecision));
				DbTools.WriteAttribute (xmlWriter, prefix+"shift", DbTools.IntToString (this.DigitShift));
				DbTools.WriteAttribute (xmlWriter, prefix+"min", DbTools.DecimalToString (this.MinValue));
				DbTools.WriteAttribute (xmlWriter, prefix+"max", DbTools.DecimalToString (this.MaxValue));
			}
			else
			{
				xmlWriter.WriteAttributeString (prefix+"raw", DbTools.RawTypeToString (this.InternalRawType));
			}
		}

		/// <summary>
		/// Deserializes a <c>DbNumDef</c> from the specified XML reader.
		/// </summary>
		/// <param name="xmlReader">The XML reader.</param>
		/// <returns>The <c>DbNumDef</c> instance.</returns>
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

		/// <summary>
		/// Deserializes a <c>DbNumDef</c> based on XML attributes.
		/// </summary>
		/// <param name="xmlReader">The XML reader.</param>
		/// <returns>The <c>DbNumDef</c> instance.</returns>
		public static DbNumDef DeserializeAttributes(System.Xml.XmlTextReader xmlReader)
		{
			return DbNumDef.DeserializeAttributes (xmlReader, "");
		}

		/// <summary>
		/// Deserializes a <c>DbNumDef</c> based on XML attributes.
		/// </summary>
		/// <param name="xmlReader">The XML reader.</param>
		/// <param name="prefix">The prefix.</param>
		/// <returns>The <c>DbNumDef</c> instance.</returns>
		public static DbNumDef DeserializeAttributes(System.Xml.XmlTextReader xmlReader, string prefix)
		{
			string rawArg = xmlReader.GetAttribute (prefix+"raw");

			if (rawArg == null)
			{
				int digits  = DbTools.ParseInt (xmlReader.GetAttribute (prefix+"digits"));
				int shift   = DbTools.ParseInt (xmlReader.GetAttribute (prefix+"shift"));
				decimal min = DbTools.ParseDecimal (xmlReader.GetAttribute (prefix+"min"));
				decimal max = DbTools.ParseDecimal (xmlReader.GetAttribute (prefix+"max"));

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



		/// <summary>
		/// Gets the pack offset, i.e. the value to substract to a decimal value
		/// in order to center into the available range. If the available range
		/// does not require centering, the pack offset will be zero.
		/// </summary>
		/// <returns>The pack offset.</returns>
		private decimal GetPackOffset()
		{
			decimal offset = this.MinValue + this.HalfRange;
			decimal mul    = DbNumDef.digitTable[this.DigitShift];

			decimal range  = mul * (this.MaxValue - this.MinValue);
			decimal max    = mul * this.MaxValue;
			decimal min    = mul * this.MinValue;

			if (range <= 255)
			{
				if ((max <= 127) &&
					(min >= -128))
				{
					offset = 0;
				}
			}
			else if (range <= System.UInt16.MaxValue)
			{
				if ((max <= System.Int16.MaxValue) &&
					(min >= System.Int16.MinValue))
				{
					offset = 0;
				}
			}
			else if (range <= System.UInt32.MaxValue)
			{
				if ((max <= System.Int32.MaxValue) &&
					(min >= System.Int32.MinValue))
				{
					offset = 0;
				}
			}
			else if (range <= System.UInt64.MaxValue)
			{
				if ((max <= System.Int64.MaxValue) &&
					(min >= System.Int64.MinValue))
				{
					offset = 0;
				}
			}
			else
			{
				throw new System.ArithmeticException ("Value cannot be packed into a 64-bit integer");
			}

			return offset;
		}

		/// <summary>
		/// Updates the auto precision based on the minimum and maximum values.
		/// This generates a synthetic <c>DigitShift</c> and <c>DigitPrecision</c>.
		/// </summary>
		private void UpdateAutoPrecision()
		{
			//	Détermine la précision nécessaire à la représentation d'un nombre donné.

			//	La routine donne à la fois la précision (nombre total de décimales) et
			//	le "shift" (nombre de décimales après le point décimal).

			//	Le résultat est mémorisé de manière à ne pas devoir recalculer cela à
			//	chaque fois que nécessaire.

			System.Diagnostics.Debug.Assert (this.IsMinMaxDefined);
			System.Diagnostics.Debug.Assert (this.digitShiftAuto == 0);
			System.Diagnostics.Debug.Assert (this.digitPrecisionAuto == 0);

			decimal min = System.Math.Abs (this.MinValue);
			decimal max = System.Math.Abs (this.MaxValue);

			//	Cherche déjà le nombre de décimales :

			while ((System.Decimal.Truncate (min) != min)
				|| (System.Decimal.Truncate (max) != max))
			{
				this.digitShiftAuto++;
				min *= 10M;
				max *= 10M;
			}

			System.Diagnostics.Debug.Assert (this.digitShiftAuto < DbNumDef.digitMax);

			//	Puis cherche le nombre de digits :

			while ((DbNumDef.digitTable[this.digitPrecisionAuto] <= min)
				|| (DbNumDef.digitTable[this.digitPrecisionAuto] <= max))
			{
				this.digitPrecisionAuto++;
				System.Diagnostics.Debug.Assert (this.digitPrecisionAuto < DbNumDef.digitMax);
			}
		}

		private void InvalidateAndUpdateAutoPrecision()
		{
			this.digitPrecisionAuto = 0;
			this.digitShiftAuto     = 0;

			if (this.IsMinMaxDefined)
			{
				this.UpdateAutoPrecision ();
			}
		}


		[System.Diagnostics.Conditional ("DEBUG")]
		private void DebugCheckAbsolute(decimal value)
		{
			//	Utilisé pour vérifier que ni le minValue, ni le maxValue
			//	n'est pas donné au delà des 24 digits autorisés. Cette méthode
			//	de vérification n'est pas appelée si le projet est compilé en
			//	mode 'Release'.

			System.Diagnostics.Debug.Assert (value >= minAbsolute);
			System.Diagnostics.Debug.Assert (value <= maxAbsolute);
		}
		
		
		#region Static Setup
		
		static DbNumDef()
		{
			//	Initialise la table de conversion entre nombre de décimales après la
			//	virgule et facteur multiplicatif/facteur d'échelle.
			
			DbNumDef.digitTable = new decimal[DbNumDef.digitMax];
			DbNumDef.digitTableScale = new decimal[DbNumDef.digitMax];
			
			decimal multiple = 1;
			decimal scale = 1;
			
			for (int i = 0; i < DbNumDef.digitMax; i++)
			{
				DbNumDef.digitTable[i] = multiple;
				DbNumDef.digitTableScale[i] = scale;
				multiple *= 10M;
				scale *= 0.1M;
			}
		}
		
		#endregion		
		
		private const int						digitMax		= 24;
		private const decimal					maxAbsolute	= 99999999999999999999999.0M;
		private const decimal					minAbsolute	= -maxAbsolute;

		private static decimal[]				digitTable;
		private static decimal[]				digitTableScale;

		private DbRawType						rawType;
		private decimal							minValue	=  maxAbsolute;
		private decimal							maxValue	=  minAbsolute;

		private byte							digitPrecision;
		private byte							digitShift;
		private byte							digitPrecisionAuto;	//	cache les val. dét. selon Min et Max
		private byte							digitShiftAuto;		//	cache les val. dét. selon Min et Max
	}
}
