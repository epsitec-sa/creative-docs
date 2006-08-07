//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe LongIntegerType décrit une valeur de type System.Int64.
	/// </summary>
	public class LongIntegerType : AbstractNumericType
	{
		public LongIntegerType()
			: this (long.MinValue, long.MaxValue)
		{
		}

		public LongIntegerType(long min, long max)
			: base ("LongInteger", new DecimalRange (min, max))
		{
		}

		public LongIntegerType(Caption caption)
			: base (caption)
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
				if (this.Range.IsEmpty)
				{
					return true;
				}
				else
				{
					long num = (long) value;
					return this.Range.Constrain (num) == num;
				}
			}
			
			return false;
		}

		public static readonly LongIntegerType Default = new LongIntegerType ();
	}
}
