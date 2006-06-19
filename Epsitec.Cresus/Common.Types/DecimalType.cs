//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DecimalType décrit des valeurs de type System.Decimal.
	/// </summary>
	public class DecimalType : AbstractNumericType
	{
		public DecimalType() : this (int.MinValue, int.MaxValue, 1)
		{
		}
		
		public DecimalType(DecimalRange range) : base ("Decimal", range.Clone () as DecimalRange)
		{
		}
		
		public DecimalType(decimal min, decimal max, decimal resolution) : this (new DecimalRange (min, max, resolution))
		{
		}
		
		
		public override System.Type				SystemType
		{
			get
			{
				return typeof (decimal);
			}
		}
		
		public override bool IsValidValue(object value)
		{
			if (value is decimal)
			{
				decimal num = (decimal) value;
				return this.Range.Constrain (num) == num;
			}
			
			return false;
		}
	}
}
