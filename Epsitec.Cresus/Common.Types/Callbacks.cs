//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public delegate object GetValueOverrideCallback(Object d);
	public delegate void   SetValueOverrideCallback(Object d, object value);
	
	public delegate void PropertyInvalidatedCallback(Object d, object old_value, object new_value);
}
