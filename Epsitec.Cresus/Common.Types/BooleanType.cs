//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe BooleanType décrit des valeurs de type System.Boolean.
	/// </summary>
	public class BooleanType : AbstractNumericType
	{
		public BooleanType() : base ("Boolean", new DecimalRange (0, 1, 1))
		{
		}
		
		
		public override System.Type				SystemType
		{
			get
			{
				return typeof (bool);
			}
		}
		
		public override bool IsValidValue(object value)
		{
			if (value is bool)
			{
				return true;
			}
			
			return false;
		}
	}
}
