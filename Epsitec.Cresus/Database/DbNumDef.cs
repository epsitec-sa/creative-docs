namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbNumDef d�finit un format num�rique.
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
			this.digit_precision = digit_precision;
			this.digit_shift     = digit_shift;
		}
		
		public DbNumDef(int digit_precision, int digit_shift, decimal min_value, decimal max_value)
		{
			this.digit_precision = digit_precision;
			this.digit_shift     = digit_shift;
			this.min_value       = min_value;
			this.max_value       = max_value;
		}
		
		
		public int						DigitPrecision
		{
			get
			{
				if (this.digit_precision == 0)
				{
					//	TODO: extrait la pr�cision des valeurs minimum et maximum.
					
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
				
				//	TODO: d�rive la valeur maximale des informations relatives
				//	au nombre de chiffres (precision = nb. total de chiffres,
				//	shift = nb. de chiffres apr�s la virgule).
				
				return 0;
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
				//	TODO: calcule le nombre de bits n�cessaire pour repr�senter un nombre
				//	compris entre min_value et max_value (bornes comprises), en tenant en
				//	outre compte de l'�chelle (shift).
				
				return 1;
			}
		}
		
		
		public bool CheckCompatibility(decimal value)
		{
			//	TODO: d�termine le nombre de chiffres apr�s la virgule et v�rifie
			//	qu'elle n'exc�de pas DigitShift.
			
			return (value >= this.MinValue) && (value <= this.MaxValue);
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
			//	TODO: arrondit la valeur au nombre maximal de chiffres apr�s la
			//	virgule accept� par cette d�finition...
			
			return value;
		}
		
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
		
		
		protected int					digit_precision;
		protected int					digit_shift;
		protected decimal				min_value	=  0.0M;
		protected decimal				max_value	= -1.0M;
	}
}
