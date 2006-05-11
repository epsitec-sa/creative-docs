//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public delegate object GetValueOverrideCallback(DependencyObject o);
	public delegate void SetValueOverrideCallback(DependencyObject o, object value);
	public delegate bool ValidateValueCallback(object value);
	public delegate object CoerceValueCallback(DependencyObject o, DependencyProperty p, object value);
	
	public delegate void PropertyInvalidatedCallback(DependencyObject o, object oldValue, object newValue);
}
