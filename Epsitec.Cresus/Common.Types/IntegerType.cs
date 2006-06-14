//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe IntegerType décrit une valeur de type System.Int32.
	/// </summary>
	public class IntegerType : INumType, IDataConstraint
	{
		public IntegerType()
		{
			this.range = new DecimalRange (System.Int32.MinValue, System.Int32.MaxValue);
		}
		
		public IntegerType(int min, int max)
		{
			this.range = new DecimalRange (min, max);
		}
		
		
		#region INamedType Members
		public System.Type						SystemType
		{
			get
			{
				return typeof (System.Int32);
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
				return "Integer";
			}
		}

		public string							Caption
		{
			get
			{
				return null;
			}
		}

		public string							Description
		{
			get
			{
				return null;
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
