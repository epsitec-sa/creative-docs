//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DecimalType d�crit divers types num�riques natifs.
	/// </summary>
	public class DecimalType : INum
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
		
		DecimalRange							range;
	}
}
