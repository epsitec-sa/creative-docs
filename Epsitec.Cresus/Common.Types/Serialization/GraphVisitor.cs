//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public static class GraphVisitor
	{
		public static void VisitSerializableNodes(DependencyObject obj, Context context)
		{
			if (context.ExternalMap.IsValueDefined (obj))
			{
				context.ExternalMap.IncrementUseValue (obj);
				return;
			}
			
			if (context.ObjectMap.Record (obj))
			{
				//	Visit every locally defined property which either refers to
				//	a DependencyObject or to a collection of such.

				foreach (LocalValueEntry entry in obj.LocalValueEntries)
				{
					DependencyPropertyMetadata metadata = entry.Property.GetMetadata (obj);

					if ((entry.Property.IsReadWrite) ||
						(metadata.CanSerializeReadOnly))
					{
						if (entry.Property.IsAttached)
						{
							context.ObjectMap.RecordType (entry.Property.OwnerType);
						}
						
						DependencyObject dependencyObjectValue = entry.Value as DependencyObject;

						if (dependencyObjectValue != null)
						{
							GraphVisitor.VisitSerializableNodes (dependencyObjectValue, context);
							continue;
						}

						ICollection<DependencyObject> dependencyObjectCollection = entry.Value as ICollection<DependencyObject>;

						if (dependencyObjectCollection != null)
						{
							foreach (DependencyObject node in dependencyObjectCollection)
							{
								GraphVisitor.VisitSerializableNodes (node, context);
							}
						}
					}
				}
			}
		}
	}
}
