//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 27/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DecimalType décrit divers types numériques natifs.
	/// </summary>
	public class IntegerType : INum, IDataConstraint
	{
		public IntegerType()
		{
			this.range = new DecimalRange (int.MinValue, int.MaxValue);
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
				return typeof (int);
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
		public bool CheckConstraint(object value)
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
