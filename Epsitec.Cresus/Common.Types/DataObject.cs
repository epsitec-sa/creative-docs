//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public abstract class DataObject : DependencyObject
	{
		public static Binding GetDataContext(DependencyObject o)
		{
			return o.GetValue (DataObject.DataContextProperty) as Binding;
		}
		public static void SetDataContext(DependencyObject o, Binding value)
		{
			o.SetValue (DataObject.DataContextProperty, value);
		}
		
		public static DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached ("DataContext", typeof (Binding), typeof (DataObject));
	}
}
