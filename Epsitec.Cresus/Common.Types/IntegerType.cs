//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe IntegerType décrit une valeur de type System.Int32.
	/// </summary>
	public class IntegerType : AbstractNumericType
	{
		public IntegerType() : this (int.MinValue, int.MaxValue)
		{
		}
		
		public IntegerType(int min, int max) : base ("Integer", new DecimalRange (min, max))
		{
		}
		
		
		public override System.Type				SystemType
		{
			get
			{
				return typeof (int);
			}
		}
		
		public override bool IsValidValue(object value)
		{
			if (value is int)
			{
				int num = (int) value;
				
				return this.Range.Constrain (num) == num;
			}
			
			return false;
		}

		public static readonly IntegerType Default = new IntegerType ();
	}
}
