//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DecimalType décrit des valeurs de type System.Decimal.
	/// </summary>
	public class DecimalType : INumType, IDataConstraint
	{
		public DecimalType()
		{
			this.range = new DecimalRange (int.MinValue, int.MaxValue);
		}
		
		public DecimalType(DecimalRange range)
		{
			this.range = range.Clone () as DecimalRange;
		}
		
		public DecimalType(decimal min, decimal max, decimal resolution)
		{
			this.range = new DecimalRange (min, max, resolution);
		}
		
		
		#region INamedType Members
		public System.Type						SystemType
		{
			get
			{
				return typeof (System.Decimal);
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
				return "Decimal";
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
		public bool ValidateValue(object value)
		{
			if ((value is decimal) &&
				(this.range != null))
			{
				decimal num = (decimal) value;
				return this.range.Constrain (num) == num;
			}
			
			return false;
		}
		#endregion
		
		private DecimalRange					range;
	}
}
