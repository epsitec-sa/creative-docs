//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			int externalCount = context.ExternalMap.TagCount;

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

			context.NotifySerializationCompleted ();
			context.ActiveWriter.EndStorageBundle ();
		}

		public static Serialization.Context CurrentDeserializationContext
		{
			get
			{
				if ((Storage.serializationnContextStack != null) &&
					(Storage.serializationnContextStack.Count > 0))
				{
					return Storage.serializationnContextStack.Peek ();
				}
				else
				{
					return null;
				}
			}
		}
		
		public static DependencyObject Deserialize(Serialization.Context context)
		{
			int rootId;
			
			int externalCount;
			int typeCount;
			int objectCount;
			
			DependencyObject root;

			if (Storage.serializationnContextStack == null)
			{
				Storage.serializationnContextStack = new Stack<Serialization.Context> ();
			}

			Storage.serializationnContextStack.Push (context);

			try
			{
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

					List<Serialization.IDeserialization> deserialized = new List<Serialization.IDeserialization> ();

					foreach (DependencyObject obj in list)
					{
						Serialization.IDeserialization deserialization = obj as Serialization.IDeserialization;

						if (deserialization != null)
						{
							if (deserialization.NotifyDeserializationStarted (context))
							{
								deserialized.Add (deserialization);
							}
						}
					}

					foreach (DependencyObject obj in list)
					{
						context.RestoreObjectData (context.ObjectMap.GetId (obj), obj);
					}

					foreach (Serialization.IDeserialization deserialization in deserialized)
					{
						deserialization.NotifyDeserializationCompleted (context);
					}

					root = context.ObjectMap.GetValue (rootId);
				}

				context.ActiveReader.EndStorageBundle ();
			}
			finally
			{
				Storage.serializationnContextStack.Pop ();
			}
			return root;
		}

		[System.ThreadStatic]
		private static Stack<Serialization.Context> serializationnContextStack;
	}
}
