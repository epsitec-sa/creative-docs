//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public static class Storage
	{
		public static void Serialize(DependencyObject obj, Serialization.Context context)
		{
			Serialization.Generic.Map<DependencyObject> map = context.ObjectMap;
			
			int typeCount = map.TypeCount;
			int objCount = map.ValueCount;

			context.Visitor.VisitSerializableNodes (obj);

			int newTypeCount = map.TypeCount;
			int newObjCount = map.ValueCount;

			if (newObjCount > objCount)
			{
				context.Writer.BeginStorageBundle ();
				
				for (int id = typeCount; id < newTypeCount; id++)
				{
					context.DefineType (id+1, map.GetType (id));
				}
				for (int id = objCount; id < newObjCount; id++)
				{
					context.DefineObject (id, map.GetValue (id));
				}
				for (int id = objCount; id < newObjCount; id++)
				{
					context.StoreObject (id, map.GetValue (id));
				}
				
				context.Writer.EndStorageBundle ();
			}
		}
	}
}
