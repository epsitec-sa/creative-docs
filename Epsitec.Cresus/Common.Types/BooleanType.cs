//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe BooleanType décrit des valeurs de type System.Boolean.
	/// </summary>
	public class BooleanType : INumType, IDataConstraint
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
				return typeof (System.Boolean);
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
		public bool ValidateValue(object value)
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
