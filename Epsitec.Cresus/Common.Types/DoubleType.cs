//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DoubleType décrit des valeurs de type System.Double.
	/// </summary>
	public class DoubleType : AbstractNumericType
	{
		public DoubleType() : this (DecimalRange.Empty)
		{
		}
		
		public DoubleType(DecimalRange range) : base ("Double", range)
		{
		}

		public DoubleType(decimal min, decimal max, decimal resolution)
			: this (new DecimalRange (min, max, resolution))
		{
		}
		
		
		public override System.Type				SystemType
		{
			get
			{
				return typeof (double);
			}
		}
		
		public override bool IsValidValue(object value)
		{
			if (value is double)
			{
				if (this.Range.IsEmpty)
				{
					return true;
				}
				else
				{
					decimal num = (decimal) (double) value;
					return this.Range.Constrain (num) == num;
				}
			}
			
			return false;
		}

		public static readonly DoubleType Default = new DoubleType ();

		public const decimal TenTo24 = 1000000M*1000000M * 1000000M*1000000M;
		public const decimal TenTo28 = 1000000M*1000000M * 1000000M*1000000M * 10000M;
	}
}
