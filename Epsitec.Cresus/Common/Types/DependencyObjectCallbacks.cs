//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public delegate object GetValueOverrideCallback(DependencyObject o);
	public delegate void SetValueOverrideCallback(DependencyObject o, object value);
	public delegate bool ValidateValueCallback(object value);
	public delegate object CoerceValueCallback(DependencyObject o, DependencyProperty p, object value);
	
	public delegate void PropertyInvalidatedCallback(DependencyObject o, object oldValue, object newValue);
	
	public delegate void PropertyInvalidatedCallback<in T, in TValue>(T o, TValue oldValue, TValue newValue)
		where T : DependencyObject;
}
