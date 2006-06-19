//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe LongIntegerType décrit une valeur de type System.Int64.
	/// </summary>
	public class LongIntegerType : INumericType, IDataConstraint
	{
		public LongIntegerType()
		{
			this.range = new DecimalRange (System.Int64.MinValue, System.Int64.MaxValue);
		}
		
		public LongIntegerType(long min, long max)
		{
			this.range = new DecimalRange (min, max);
		}
		
		
		#region INamedType Members
		public System.Type						SystemType
		{
			get
			{
				return typeof (System.Int64);
			}
		}
		#endregion
		
		#region INumType Members
		public DecimalRange						Range
		{
			get
			{
				return this.range;
			}
		}
		#endregion
		
		#region INameCaption Members
		public string							Name
		{
			get
			{
				return "LongInteger";
			}
		}

		public long								CaptionId
		{
			get
			{
				return -1;
			}
		}

		#endregion
		
		#region IDataConstraint Members
		public bool IsValidValue(object value)
		{
			if ((value is int) &&
				(this.range != null))
			{
				int num = (int) value;
				return this.range.Constrain (num) == num;
			}
			
			return false;
		}
		#endregion
		
		private DecimalRange					range;
	}
}
