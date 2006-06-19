//	Copyright � 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DecimalRange</c> structure defines a minimum, maximum and resolution
	/// for a decimal value.
	/// </summary>
	public struct DecimalRange
	{
		public DecimalRange(decimal min, decimal max)
			: this (min, max, 1.0M)
		{
		}

		public DecimalRange(decimal min, decimal max, decimal resolution)
		{
			this.minimum     = min;
			this.maximum     = max;
			this.resolution  = 0;
			this.digitsDiv  = 0;
			this.digitsMul  = 0;
			this.fracDigits = 0;

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


		public decimal Constrain(decimal value)
		{
			//	Tronque la pr�cision de la valeur � la r�solution courante,
			//	en utilisant un arrondi � la valeur la plus proche, puis
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

				//	Assure que le nombre de d�cimales apr�s le point est constant et conforme �
				//	la pr�cision d�finie.
				//
				//	Ceci permet de garantir que ToString() se comporte de mani�re pr�visible.

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


		public decimal ConstrainToZero(decimal value)
		{
			//	Comme Constain, mais force l'arrondi vers le nombre le plus proche
			//	de z�ro.

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

				//	Assure que le nombre de d�cimales apr�s le point est constant et conforme �
				//	la pr�cision d�finie.
				//
				//	Ceci permet de garantir que ToString() se comporte de mani�re pr�visible.

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
				return ((int) value).ToString ();
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
				return ((int) value).ToString (culture);
			}
		}


		private void DefineResolution(decimal value)
		{
			if (value < 0)
			{
				throw new System.ArgumentOutOfRangeException ("value", value, "Resolution must be positive");
			}

			this.resolution = value;

			//	Pour une r�solution de 0.05, par exemple, on va d�terminer les facteurs
			//	multiplicatifs pour cr�er les 2 d�cimales apr�s la virgule au moyen d'une
			//	suite d'op�rations: multiplier par le, facteur 'digits_mul', tronquer et
			//	multiplier par le facteur 'digits_div'.
			//
			//	Ceci est utile, car le nombre decimal 1M n'est pas repr�sent� de la m�me
			//	mani�re que les nombres 1.0M ou 1.00M, quand ils sont convertis en string.

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


		private decimal minimum;
		private decimal maximum;
		private decimal resolution;
		private decimal digitsMul;
		private decimal digitsDiv;
		private int fracDigits;
	}
}
