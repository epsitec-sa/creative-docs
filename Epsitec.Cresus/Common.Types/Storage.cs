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
			
			int oldTypeCount = map.TypeCount;
			int oldObjCount = map.ValueCount;

			Serialization.GraphVisitor.VisitSerializableNodes (root, context);

			int newTypeCount = map.TypeCount;
			int newObjCount = map.ValueCount;

			int objectCount   = newObjCount - oldObjCount;
			int typeCount     = newTypeCount - oldTypeCount;
			int externalCount = context.ExternalMap.UsedValueCount;

			context.ActiveWriter.BeginStorageBundle (map.GetId (root), externalCount, typeCount, objectCount);

			if (newObjCount > oldObjCount)
			{
				foreach (string tag in context.ExternalMap.RecordedTags)
				{
					context.StoreExternalDefinition (tag);
				}

				for (int id = oldTypeCount; id < newTypeCount; id++)
				{
					context.StoreTypeDefinition (id, map.GetType (id));
				}
				for (int id = oldObjCount; id < newObjCount; id++)
				{
					context.StoreObjectDefinition (id, map.GetValue (id));
				}
				for (int id = oldObjCount; id < newObjCount; id++)
				{
					context.StoreObjectData (id, map.GetValue (id));
				}
			}
			
			context.ActiveWriter.EndStorageBundle ();
		}
		public static DependencyObject Deserialize(Serialization.Context context)
		{
			int rootId;
			
			int externalCount;
			int typeCount;
			int objectCount;
			
			DependencyObject root;
			
			context.ActiveReader.BeginStorageBundle (out rootId, out externalCount, out typeCount, out objectCount);

			if (context.ObjectMap.IsIdDefined (rootId))
			{
				root = context.ObjectMap.GetValue (rootId);
			}
			else
			{
				List<DependencyObject> list = new List<DependencyObject> ();
				
				for (int i = 0; i < externalCount; i++)
				{
					context.RestoreExternalDefinition ();
				}
				for (int i = 0; i < typeCount; i++)
				{
					context.RestoreTypeDefinition ();
				}
				for (int i = 0; i < objectCount; i++)
				{
					list.Add (context.RestoreObjectDefinition ());
				}
				foreach (DependencyObject obj in list)
				{
					context.RestoreObjectData (context.ObjectMap.GetId (obj), obj);
				}

				root = context.ObjectMap.GetValue (rootId);
			}
			
			context.ActiveReader.EndStorageBundle ();
			
			return root;
		}
	}
}
