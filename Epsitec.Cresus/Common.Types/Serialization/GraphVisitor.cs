//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public class GraphVisitor
	{
		public GraphVisitor()
		{
		}

		public Map								Map
		{
			get
			{
				return this.map;
			}
		}

		public void VisitSerializableNodes(DependencyObject obj)
		{
			if (this.map.Record (obj))
			{
				//	Visit every locally defined property which either refers to
				//	a DependencyObject or to a collection of such.
				
				foreach (LocalValueEntry entry in obj.LocalValueEntries)
				{
					DependencyPropertyMetadata metadata = entry.Property.GetMetadata (obj);

					if ((entry.Property.IsReadWrite) ||
						(metadata.CanSerializeReadOnly))
					{
						DependencyObject dependencyObjectValue = entry.Value as DependencyObject;

						if (dependencyObjectValue != null)
						{
							this.VisitSerializableNodes (dependencyObjectValue);
							continue;
						}

						ICollection<DependencyObject> dependencyObjectCollection = entry.Value as ICollection<DependencyObject>;

						if (dependencyObjectCollection != null)
						{
							foreach (DependencyObject node in dependencyObjectCollection)
							{
								this.VisitSerializableNodes (node);
							}
						}
					}
				}
			}
		}

		private Map								map = new Map ();
	}
}
