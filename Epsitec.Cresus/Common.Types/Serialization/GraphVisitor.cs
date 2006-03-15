//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public class GraphVisitor
	{
		public GraphVisitor()
		{
		}

		public Generic.Map<DependencyObject>	ObjectMap
		{
			get
			{
				return this.objMap;
			}
		}
		public Generic.Map<System.Type>			TypeMap
		{
			get
			{
				return this.typeMap;
			}
		}

		public void VisitSerializableNodes(DependencyObject obj)
		{
			if (this.objMap.Record (obj))
			{
				//	Visit every locally defined property which either refers to
				//	a DependencyObject or to a collection of such.

				this.typeMap.Record (obj.GetType ());
				
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

		private Generic.Map<DependencyObject>	objMap = new Generic.Map<DependencyObject> ();
		private Generic.Map<System.Type>		typeMap = new Generic.Map<System.Type> ();
	}
}
