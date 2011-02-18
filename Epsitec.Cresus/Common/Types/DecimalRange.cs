//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DecimalRange</c> structure defines a minimum, maximum and resolution
	/// for a decimal value.
	/// </summary>
	
	[SerializationConverter (typeof (DecimalRange.SerializationConverter))]
	
	public struct DecimalRange : System.IEquatable<DecimalRange>
	{
		public DecimalRange(decimal min, decimal max)
			: this (min, max, 1M)
		{
		}

		public DecimalRange(decimal min, decimal max, decimal resolution)
		{
			this.minimum     = min;
			this.maximum     = max;
			this.resolution  = 0;
			this.digitsDiv   = 0;
			this.digitsMul   = 0;
			this.fracDigits  = 0;

			this.DefineResolution (resolution);
		}


		public decimal							Minimum
		{
			get
			{
				return this.minimum;
			}
		}

		public decimal							Maximum
		{
			get
			{
				return this.maximum;
			}
		}

		public decimal							Resolution
		{
			get
			{
				return this.resolution;
			}
		}

		public int								FractionalDigits
		{
			get
			{
				return this.fracDigits;
			}
		}

		public bool								IsValid
		{
			get
			{
				return (this.minimum <= this.maximum);
			}
		}

		public bool								IsEmpty
		{
			get
			{
				return (this.digitsDiv == 0) || (this.digitsMul == 0);
			}
		}


		public static readonly DecimalRange		Empty = new DecimalRange ();


		public int GetMaximumDigitCount()
		{
			return System.Math.Max (this.GetIntegerDigitCount (this.Maximum * this.digitsMul),
				/* */				this.GetIntegerDigitCount (this.Minimum * this.digitsMul));
		}

		private int GetIntegerDigitCount(decimal value)
		{
			if (value < 0)
			{
				value = -value;
			}
			
			int count = 0;

			while (decimal.Truncate (value) > 0)
			{
				value /= 10M;
				count++;
			}

			return count;
		}
		
		public bool CheckInRange(decimal value)
		{
			decimal constrained = this.Constrain (value);
			return constrained == value;
		}

		public bool CheckInRange(double value)
		{
			return this.CheckInRange ((decimal) value);
		}

		public bool CheckInRange(int value)
		{
			return this.CheckInRange ((decimal) value);
		}

		public bool CheckInRange(long value)
		{
			return this.CheckInRange ((decimal) value);
		}


		public decimal? Constrain(decimal? value)
		{
			if (value.HasValue)
			{
				return this.Constrain (value.Value);
			}
			else
			{
				return null;
			}
		}

		public decimal Constrain(decimal value)
		{
			//	Tronque la précision de la valeur à la résolution courante,
			//	en utilisant un arrondi à la valeur la plus proche, puis
			//	contraint la valeur aux bornes minimum/maximum.

			if (this.IsValid && !this.IsEmpty)
			{
				System.Diagnostics.Debug.Assert (this.digitsDiv > 0);
				System.Diagnostics.Debug.Assert (this.digitsMul > 0);
				
				if (this.resolution != 0)
				{
					decimal scale = 1M / this.resolution;

					if (value < 0)
					{
						value -= this.resolution / 2;
					}
					else
					{
						value += this.resolution / 2;
					}

					value *= scale;
					value  = System.Decimal.Truncate (value);
					value /= scale;
				}

				value = System.Math.Min (value, this.maximum);
				value = System.Math.Max (value, this.minimum);

				//	Assure que le nombre de décimales après le point est constant et conforme à
				//	la précision définie.
				//
				//	Ceci permet de garantir que ToString() se comporte de manière prévisible.

				if (this.resolution != 0)
				{
					value *= this.digitsMul;
					value  = System.Decimal.Truncate (value);
					value *= this.digitsDiv;
				}

				return value;
			}

			throw new System.InvalidOperationException ("DecimalRange is invalid");
		}

		public decimal Constrain(double value)
		{
			return this.Constrain ((decimal) value);
		}

		public decimal Constrain(int value)
		{
			return this.Constrain ((decimal) value);
		}

		public decimal Constrain(long value)
		{
			return this.Constrain ((decimal) value);
		}


		public decimal? ConstrainToZero(decimal? value)
		{
			if (value.HasValue)
			{
				return this.ConstrainToZero (value.Value);
			}
			else
			{
				return null;
			}
		}

		public decimal ConstrainToZero(decimal value)
		{
			//	Comme Constain, mais force l'arrondi vers le nombre le plus proche
			//	de zéro.

			if (this.IsValid)
			{
				if (this.resolution != 0)
				{
					decimal scale = 1M / this.resolution;

					value *= scale;

					if (value < 0)
					{
						value = -System.Decimal.Truncate (-value);
					}
					else
					{
						value = System.Decimal.Truncate (value);
					}

					value /= scale;
				}

				value = System.Math.Min (value, this.maximum);
				value = System.Math.Max (value, this.minimum);

				//	Assure que le nombre de décimales après le point est constant et conforme à
				//	la précision définie.
				//
				//	Ceci permet de garantir que ToString() se comporte de manière prévisible.

				if (this.resolution != 0)
				{
					value *= this.digitsMul;
					value  = System.Decimal.Truncate (value);
					value *= this.digitsDiv;
				}

				return value;
			}

			throw new System.InvalidOperationException ("DecimalRange is invalid");
		}

		public decimal ConstrainToZero(double value)
		{
			return this.ConstrainToZero ((decimal) value);
		}

		public decimal ConstrainToZero(int value)
		{
			return this.ConstrainToZero ((decimal) value);
		}

		public decimal ConstrainToZero(long value)
		{
			return this.ConstrainToZero ((decimal) value);
		}


		public string ConvertToString(decimal value)
		{
			value = this.Constrain (value);

			if (this.fracDigits > 0)
			{
				return value.ToString (string.Format (System.Globalization.CultureInfo.InvariantCulture, "F{0}", this.fracDigits));
			}
			else
			{
				return decimal.Truncate (value).ToString ();
			}
		}

		public string ConvertToString(decimal value, System.Globalization.CultureInfo culture)
		{
			value = this.Constrain (value);

			if (this.fracDigits > 0)
			{
				return value.ToString (string.Format (System.Globalization.CultureInfo.InvariantCulture, "F{0}", this.fracDigits), culture);
			}
			else
			{
				return decimal.Truncate (value).ToString ();
			}
		}

		public static bool operator==(DecimalRange a, DecimalRange b)
		{
			return a.Equals (b);
		}

		public static bool operator!=(DecimalRange a, DecimalRange b)
		{
			return ! a.Equals (b);
		}

		public override bool Equals(object obj)
		{
			if (obj is DecimalRange)
			{
				return base.Equals ((DecimalRange) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.minimum.GetHashCode () ^ this.maximum.GetHashCode () ^ this.resolution.GetHashCode ();
		}

		public override string ToString()
		{
			return string.Format ("{0}..{1}:{2}", this.minimum, this.maximum, this.resolution);
		}

		#region IEquatable<DecimalRange> Members

		public bool Equals(DecimalRange other)
		{
			if ((this.minimum == other.minimum) &&
				(this.maximum == other.maximum) &&
				(this.resolution == other.resolution))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		private void DefineResolution(decimal value)
		{
			if (value < 0)
			{
				throw new System.ArgumentOutOfRangeException ("value", value, "Resolution must be positive");
			}

			this.resolution = value;

			//	Pour une résolution de 0.05, par exemple, on va déterminer les facteurs
			//	multiplicatifs pour créer les 2 décimales après la virgule au moyen d'une
			//	suite d'opérations: multiplier par le, facteur 'digitsMul' tronquer et
			//	multiplier par le facteur 'digitsDiv'.
			//
			//	Ceci est utile, car le nombre decimal 1M n'est pas représenté de la même
			//	manière que les nombres 1.0M ou 1.00M, quand ils sont convertis en string.

			this.digitsDiv  = 1M;
			this.digitsMul  = 1M;
			this.fracDigits = 0;

			decimal iter = value;

			while (iter != System.Decimal.Truncate (iter))
			{
				this.digitsMul *= 10M;
				this.digitsDiv *= 0.1M;
				this.fracDigits++;
				iter *= 10M;
			}
		}

		#region SerializationConverter Class

		public class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				DecimalRange range = (DecimalRange) value;

				return string.Join (" ",
					/* */			new string[] {
					/* */						   range.minimum.ToString (System.Globalization.CultureInfo.InvariantCulture),
					/* */						   range.maximum.ToString (System.Globalization.CultureInfo.InvariantCulture),
					/* */						   range.resolution.ToString (System.Globalization.CultureInfo.InvariantCulture) });
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				string[] args = value.Split (' ');

				if (args.Length != 3)
				{
					throw new System.FormatException (string.Format ("{0} is not a serialized DecimalRange"));
				}

				decimal min = decimal.Parse (args[0], System.Globalization.CultureInfo.InvariantCulture);
				decimal max = decimal.Parse (args[1], System.Globalization.CultureInfo.InvariantCulture);
				decimal res = decimal.Parse (args[2], System.Globalization.CultureInfo.InvariantCulture);
				
				return new DecimalRange (min, max, res);
			}

			#endregion
		}

		#endregion


		private decimal minimum;
		private decimal maximum;
		private decimal resolution;
		private decimal digitsMul;
		private decimal digitsDiv;
		private int fracDigits;
	}
}
