//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public static class GraphVisitor
	{
		public static void VisitSerializableNodes(DependencyObject obj, Context context)
		{
			GraphVisitor.Visitor visitor = new GraphVisitor.Visitor ();
			GraphVisitor.VisitSerializableNodes (obj, context, visitor);

			System.Diagnostics.Debug.Assert (visitor.Level == 0);
		}

		public static void VisitSerializableNodes(DependencyObject obj, Context context, IVisitor visitor)
		{
			if (context.ExternalMap.IsValueDefined (obj))
			{
				context.ExternalMap.IncrementUseValue (obj);
				return;
			}

			if (context.ObjectMap.IsValueDefined (obj) == false)
			{
				visitor.VisitNodeBegin (context, obj);

				//	Visit every locally defined property which either refers to
				//	a DependencyObject or to a collection of such.

				foreach (PropertyValuePair entry in obj.GetSerializableDefinedValues ())
				{
					DependencyPropertyMetadata metadata = entry.Property.GetMetadata (obj);

					if (obj.GetBinding (entry.Property) == null)
					{
						if (entry.Property.IsAttached)
						{
							visitor.VisitAttached (context, entry.Property.OwnerType);
						}

						if (entry.Property.HasTypeConverter)
						{
							//	The property has a specific converter attached to it.
							//	We can safely skip the property, as it won't be saved
							//	through object serialization.

							continue;
						}

						DependencyObject dependencyObjectValue = entry.Value as DependencyObject;

						if (dependencyObjectValue != null)
						{
							if ((metadata.HasSerializationFilter == false) ||
								(metadata.FilterSerializableItem (dependencyObjectValue)))
							{
								GraphVisitor.VisitSerializableNodes (dependencyObjectValue, context, visitor);
							}

							continue;
						}

						ICollection<DependencyObject> dependencyObjectCollection = entry.Value as ICollection<DependencyObject>;

						if ((dependencyObjectCollection == null) &&
							(entry.Value != null) &&
							(TypeRosetta.DoesTypeImplementCollectionOfCompatibleObjects (entry.Value.GetType(), typeof (DependencyObject))))
						{
							dependencyObjectCollection = new List<DependencyObject> ();

							foreach (DependencyObject node in entry.Value as System.Collections.IEnumerable)
							{
								dependencyObjectCollection.Add (node);
							}
						}

						if ((dependencyObjectCollection != null) &&
							(dependencyObjectCollection.Count > 0))
						{
							foreach (DependencyObject node in metadata.FilterSerializableCollection (dependencyObjectCollection))
							{
								GraphVisitor.VisitSerializableNodes (node, context, visitor);
							}

							continue;
						}

					}
				}

				//	Visit also the DependencyObjectTree properties, if any.

				if (DependencyObjectTree.GetHasChildren (obj))
				{
					DependencyProperty property = DependencyObjectTree.ChildrenProperty;
					DependencyPropertyMetadata metadata = property.GetMetadata (obj);
					ICollection<DependencyObject> dependencyObjectCollection = DependencyObjectTree.GetChildren (obj);

					foreach (DependencyObject node in metadata.FilterSerializableCollection (dependencyObjectCollection))
					{
						GraphVisitor.VisitSerializableNodes (node, context, visitor);
					}
				}

				//	Visit also the properties which are data bound.

				foreach (KeyValuePair<DependencyProperty, Binding> entry in obj.GetAllBindings ())
				{
					Binding binding = entry.Value;

					if (binding.Source != null)
					{
						DependencyObject value = binding.Source as DependencyObject;

						if ((value != null) &&
							(context.ObjectMap.IsValueDefined (value)))
						{
							continue;
						}
						if (context.ExternalMap.IsValueDefined (binding.Source))
						{
							context.ExternalMap.IncrementUseValue (binding.Source);
							continue;
						}

						visitor.VisitUnknown (context, binding.Source);
					}
				}

				visitor.VisitNodeEnd (context, obj);
			}
		}

		#region Visitor Class

		private class Visitor : IVisitor
		{
			public int Level
			{
				get
				{
					return this.level;
				}
			}

			#region IVisitor Members

			void IVisitor.VisitNodeBegin(Context context, DependencyObject obj)
			{
				context.ObjectMap.Record (obj);
				this.level++;
			}

			void IVisitor.VisitNodeEnd(Context context, DependencyObject obj)
			{
				this.level--;
			}

			void IVisitor.VisitAttached(Context context, System.Type type)
			{
				context.ObjectMap.RecordType (type);
			}

			void IVisitor.VisitUnknown(Context context, object obj)
			{
				context.UnknownMap.Record (obj);
			}

			#endregion

			private int level;
		}

		#endregion
	}
}
