//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 27/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe BooleanType décrit divers types numériques natifs.
	/// </summary>
	public class BooleanType : INum, IDataConstraint
	{
		public BooleanType()
		{
			this.range = new DecimalRange (0, 1, 1);
		}
		
		
		#region INamedType Members
		public System.Type						SystemType
		{
			get
			{
				return typeof (bool);
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
				return "Boolean";
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
			if (value is bool)
			{
				return true;
			}
			
			return false;
		}
		#endregion
		
		private DecimalRange					range;
	}
}
