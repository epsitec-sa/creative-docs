//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public delegate object GetValueOverrideCallback(Object o);
	public delegate void   SetValueOverrideCallback(Object o, object value);
	public delegate bool ValidateValueCallback(object value);
	public delegate object CoerceValueCallback(Object o, Property p, object value);
	
	public delegate void PropertyInvalidatedCallback(Object o, object oldValue, object newValue);
}
