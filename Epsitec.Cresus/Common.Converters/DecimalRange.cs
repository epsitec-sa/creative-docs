//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Converters
{
	/// <summary>
	/// La classe DecimalRange permet de définir une plage de valeurs numériques
	/// (de type decimal).
	/// </summary>
	public class DecimalRange
	{
		public DecimalRange()
		{
		}
		
		public DecimalRange(decimal min, decimal max)
		{
			this.Minimum = min;
			this.Maximum = max;
		}
		
		public DecimalRange(decimal min, decimal max, decimal resolution)
		{
			this.Minimum    = min;
			this.Maximum    = max;
			this.Resolution = resolution;
		}
		
		
		public decimal						Minimum
		{
			get { return this.minimum; }
			set { this.minimum = value; }
		}
		
		public decimal						Maximum
		{
			get { return this.maximum; }
			set { this.maximum = value; }
		}
		
		public bool							IsValid
		{
			get
			{
				return (this.minimum < this.maximum);
			}
		}
		
		public decimal						Resolution
		{
			get { return this.resolution; }
			set
			{
				if (this.resolution != value)
				{
					if (value < 0)
					{
						throw new System.ArgumentOutOfRangeException ("value", value, "Resolution must be positive.");
					}
					
					this.resolution = value;
					
					//	Pour une résolution de 0.05, par exemple, on va déterminer les facteurs
					//	multiplicatifs pour créer les 2 décimales après la virgule au moyen d'une
					//	suite d'opérations: multiplier par le, facteur 'digits_mul', tronquer et
					//	multiplier par le facteur 'digits_div'.
					//
					//	Ceci est utile, car le nombre decimal 1M n'est pas représenté de la même
					//	manière que les nombres 1.0M ou 1.00M, quand il sont convertis en string.
					
					this.digits_div = 1M;
					this.digits_mul = 1M;
					
					decimal iter = value;
					
					while (iter != System.Decimal.Truncate (iter))
					{
						this.digits_mul *= 10M;
						this.digits_div *= 0.1M;
						iter *= 10M;
					}
				}
			}
		}
		
		
		public decimal Constrain(decimal value)
		{
			if (this.IsValid)
			{
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
				
				value *= this.digits_mul;
				value  = System.Decimal.Truncate (value);
				value *= this.digits_div;
				
				return value;
			}
			
			throw new System.InvalidOperationException (string.Format ("DecimalRange cannot constrain while invalid."));
		}
		
		public decimal Constrain(double value)
		{
			return this.Constrain ((decimal) value);
		}
		
		public decimal Constrain(int value)
		{
			return this.Constrain ((decimal) value);
		}
		
		
		private decimal						minimum		=   0.0M;
		private decimal						maximum		= 100.0M;
		private decimal						resolution	=   1.0M;
		private decimal						digits_mul	=   1M;
		private decimal						digits_div	=   1M;
	}
}
