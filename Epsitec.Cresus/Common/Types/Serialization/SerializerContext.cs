//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public class SerializerContext : Context
	{
		public SerializerContext(IO.AbstractWriter writer)
		{
			this.writer = writer;
		}
		
		public override void StoreObjectData(int id, DependencyObject obj)
		{
			//	Dump the contents of the object to the writer. This will record
			//	all defined read/write fields and binding settings.
			
			this.AssertWritable ();

			ISerialization serialization = obj as ISerialization;

			if (serialization != null)
			{
				if (serialization.NotifySerializationStarted (this))
				{
					this.serializedObjects.Add (serialization);
				}
			}

			this.writer.BeginObject (id, obj);

			this.StoreObjectBindings (obj);
			this.StoreObjectFields (obj);
//			this.StoreObjectChildren (obj);
			
			this.writer.EndObject (id, obj);
		}

		public override void NotifySerializationCompleted()
		{
			foreach (ISerialization serialization in this.serializedObjects)
			{
				serialization.NotifySerializationCompleted (this);
			}
		}

		private void StoreObjectBindings(DependencyObject obj)
		{
			foreach (KeyValuePair<DependencyProperty, Binding> entry in obj.GetAllBindings ())
			{
				string markup = MarkupExtension.BindingToString (this, entry.Value);

				this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Key), markup);
			}
		}
		
		private void StoreObjectFields(DependencyObject obj)
		{
			foreach (PropertyValuePair entry in obj.GetSerializableDefinedValues ())
			{
				DependencyPropertyMetadata metadata = entry.Property.GetMetadata (obj);

				if (obj.GetBinding (entry.Property) == null)
				{
					if (entry.Value == null)
					{
						this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), MarkupExtension.NullToString ());
						continue;
					}
					
					if (this.ExternalMap.IsValueDefined (entry.Value))
					{
						//	This is an external reference. Record it as {External xxx}.

						string markup = MarkupExtension.ExtRefToString (entry.Value, this);

						this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), markup);
						continue;
					}

					if (entry.Property.HasTypeConverter)
					{
						//	The property has a specific converter attached to it.
						//	Use it instead of the standard object serialization.
						
						//	Caution: This means that such objects won't be shared when
						//			 they get deserialized, unless the converter handles
						//			 this issue
						
						goto stringify;
					}
					
					DependencyObject dependencyObjectValue = entry.Value as DependencyObject;

					if (dependencyObjectValue != null)
					{
						//	This is an internal reference. Record it as {Object _nnn}.

						if ((metadata.HasSerializationFilter == false) ||
							(metadata.FilterSerializableItem (dependencyObjectValue)))
						{
							string markup = MarkupExtension.ObjRefToString (dependencyObjectValue, this);
							this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), markup);
						}
						
						continue;
					}

					if (this.StoreFieldAsDependencyObjectCollection (obj, entry, metadata))
					{
						continue;
					}
					if (this.StoreFieldAsStringCollection (obj, entry, metadata))
					{
						continue;
					}
					if (this.StoreFieldAsStringifyableCollection (obj, entry, metadata))
					{
						continue;
					}
				
				stringify:
					
					string value = entry.Property.ConvertToString (entry.Value, this);

					if (value != null)
					{
						this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), MarkupExtension.Escape (value));
						continue;
					}
				}
			}
		}

		private bool StoreFieldAsDependencyObjectCollection(DependencyObject obj, PropertyValuePair entry, DependencyPropertyMetadata metadata)
		{
			ICollection<DependencyObject> dependencyObjectCollection = entry.Value as ICollection<DependencyObject>;

			if ((dependencyObjectCollection == null) &&
				(TypeRosetta.DoesTypeImplementCollectionOfCompatibleObjects (entry.Value.GetType (), typeof (DependencyObject))))
			{
				dependencyObjectCollection = new List<DependencyObject> ();
				
				foreach (DependencyObject node in entry.Value as System.Collections.IEnumerable)
				{
					dependencyObjectCollection .Add (node);
				}
			}

			if (dependencyObjectCollection != null)
			{
				//	This is a collection. Record it as {Collection xxx, xxx, xxx}

				if (dependencyObjectCollection.Count > 0)
				{
					string markup = MarkupExtension.CollectionToString (metadata.FilterSerializableCollection (dependencyObjectCollection), this);

					if (!string.IsNullOrEmpty (markup))
					{
						this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), markup);
					}
				}

				return true;
			}
			
			return false;
		}

		private bool StoreFieldAsStringCollection(DependencyObject obj, PropertyValuePair entry, DependencyPropertyMetadata metadata)
		{
			ICollection<string> stringCollection = entry.Value as ICollection<string>;

			if (stringCollection != null)
			{
				//	This is a collection. Record it as {Collection xxx, xxx, xxx}

				if (stringCollection.Count > 0)
				{
					string markup;

					if (metadata.HasSerializationFilter)
					{
						markup = MarkupExtension.CollectionToString (Collection.Filter<string> (stringCollection,
							/* */																delegate (string item)
							/* */																{
							/* */																	return metadata.FilterSerializableItem (item);
							/* */																}), this);
					}
					else
					{
						markup = MarkupExtension.CollectionToString (stringCollection, this);
					}

					if (!string.IsNullOrEmpty (markup))
					{
						this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), markup);
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		private bool StoreFieldAsStringifyableCollection(DependencyObject obj, PropertyValuePair entry, DependencyPropertyMetadata metadata)
		{
			System.Collections.IEnumerable enumerableOfAnything = entry.Value as System.Collections.IEnumerable;

			if (enumerableOfAnything == null)
			{
				return false;
			}

			IEnumerable<object> enumerable = Collection.EnumerateObjects (enumerableOfAnything);

			object collection = entry.Value;
			System.Type type;
			ISerializationConverter converter = this.FindConverterForCollection (collection.GetType (), out type);

			if (converter != null)
			{
				int count = (int) type.InvokeMember ("Count", System.Reflection.BindingFlags.GetProperty, null, collection, new object[0]);

				if (count > 0)
				{
					string markup;
					if (metadata.HasSerializationFilter)
					{
						markup = MarkupExtension.EnumerableToString (Collection.Filter<object> (enumerable,
							/* */																delegate (object item)
							/* */																{
							/* */																	return metadata.FilterSerializableItem (item);
							/* */																}), this, converter);
					}
					else
					{
						markup = MarkupExtension.EnumerableToString (enumerable, this, converter);
					}

					if (!string.IsNullOrEmpty (markup))
					{
						this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), markup);
					}
					
				}

				return true;
			}
			
			return false;
		}

		private void StoreObjectChildren(DependencyObject obj)
		{
			if (DependencyObjectTree.GetHasChildren (obj))
			{
				DependencyProperty property = DependencyObjectTree.ChildrenProperty;
				DependencyPropertyMetadata metadata = property.GetMetadata (obj);
				ICollection<DependencyObject> dependencyObjectCollection = DependencyObjectTree.GetChildren (obj);

				string markup = MarkupExtension.CollectionToString (metadata.FilterSerializableCollection (dependencyObjectCollection), this);
				
				if (!string.IsNullOrEmpty (markup))
				{
					this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (property), markup);
				}
			}

		}

		private List<ISerialization> serializedObjects = new List<ISerialization> ();
	}
}
