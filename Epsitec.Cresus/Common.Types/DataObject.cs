//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public abstract class DataObject : Object
	{
		public static Binding GetDataContext(Object o)
		{
			return o.GetValue (DataObject.DataContextProperty) as Binding;
		}
		public static void SetDataContext(Object o, Binding value)
		{
			o.SetValue (DataObject.DataContextProperty, value);
		}
		
		public static Property DataContextProperty = Property.RegisterAttached ("DataContext", typeof (Binding), typeof (DataObject));
	}
}
