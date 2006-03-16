//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The Storage class is used to serialize and deserialize DependencyObject
	/// graphs.
	/// </summary>
	public static class Storage
	{
		public static void Serialize(DependencyObject root, Serialization.Context context)
		{
			context.ExternalMap.ClearUseCount ();
			
			Serialization.Generic.MapId<DependencyObject> map = context.ObjectMap;
			
			int typeCount = map.TypeCount;
			int objCount = map.ValueCount;

			Serialization.GraphVisitor.VisitSerializableNodes (root, context);

			int newTypeCount = map.TypeCount;
			int newObjCount = map.ValueCount;

			context.ActiveWriter.BeginStorageBundle (map.GetId (root));
			
			if (newObjCount > objCount)
			{
				foreach (string name in context.ExternalMap.RecordedTags)
				{
					context.ActiveWriter.WriteExternalReference (name);
				}
				
				for (int id = typeCount; id < newTypeCount; id++)
				{
					context.DefineType (id, map.GetType (id));
				}
				for (int id = objCount; id < newObjCount; id++)
				{
					context.DefineObject (id, map.GetValue (id));
				}
				for (int id = objCount; id < newObjCount; id++)
				{
					context.StoreObject (id, map.GetValue (id));
				}
			}
			
			context.ActiveWriter.EndStorageBundle ();
		}
		public static DependencyObject Deserialize(Serialization.Context context)
		{
			int rootId;
			DependencyObject root;
			
			context.ActiveReader.BeginStorageBundle (out rootId);

			if (context.ObjectMap.IsIdDefined (rootId))
			{
				root = context.ObjectMap.GetValue (rootId);
			}
			else
			{
				//	TODO: deserialize, really
				
				root = null;
			}
			
			context.ActiveReader.EndStorageBundle ();
			
			return root;
		}
	}
}
