//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public delegate object GetValueOverrideCallback(Object o);
	public delegate void   SetValueOverrideCallback(Object o, object value);
	
	public delegate void PropertyInvalidatedCallback(Object o, object old_value, object new_value);
}
