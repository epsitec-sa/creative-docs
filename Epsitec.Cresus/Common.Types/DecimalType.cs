//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DecimalType décrit divers types numériques natifs.
	/// </summary>
	public class DecimalType : INum, IDataConstraint
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
				return typeof (decimal);
			}
		}
		#endregion
		
		#region INum Members
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
		public bool CheckConstraint(object value)
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
