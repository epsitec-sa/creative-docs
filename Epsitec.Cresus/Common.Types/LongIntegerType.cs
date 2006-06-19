//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe LongIntegerType d�crit une valeur de type System.Int64.
	/// </summary>
	public class LongIntegerType : AbstractNumericType
	{
		public LongIntegerType() : this (long.MinValue, long.MaxValue)
		{
		}
		
		public LongIntegerType(long min, long max) : base ("LongInteger", new DecimalRange (min, max))
		{
		}
		
		
		public override System.Type				SystemType
		{
			get
			{
				return typeof (long);
			}
		}
		
		public override bool IsValidValue(object value)
		{
			if (value is long)
			{
				long num = (long) value;
				return this.Range.Constrain (num) == num;
			}
			
			return false;
		}
	}
}
