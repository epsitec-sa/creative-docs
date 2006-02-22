//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The ObjectTree provides a few basic properties used when navigating
	/// through object hierarchies.
	/// </summary>
	public sealed class ObjectTree : Object
	{
		public ObjectTree()
		{
		}

		public ICollection<Object> GetFoo()
		{
			Object[] x = new Object[1];
			return x;
		}
		
		public static Object GetParent(Object o)
		{
			if (o == null)
			{
				return null;
			}
			else
			{
				return o.GetValue (ObjectTree.ParentProperty) as Object;
			}
		}
		public static ICollection<Object> GetChildren(Object o)
		{
			if (o == null)
			{
				return null;
			}
			else
			{
				return o.GetValue (ObjectTree.ChildrenProperty) as ICollection<Object>;
			}
		}

		public static Property ParentProperty = Property.RegisterReadOnly ("Parent", typeof (Object), typeof (ObjectTree));
		public static Property ChildrenProperty = Property.RegisterReadOnly ("Children", typeof (ICollection<Object>), typeof (ObjectTree));
	}
}
