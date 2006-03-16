//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		public override void StoreObject(int id, DependencyObject obj)
		{
			//	Dump the contents of the object to the writer. This will record
			//	all defined read/write fields and binding settings.
			
			this.AssertWritable ();

			this.writer.BeginObject (id, obj);

			Fields fields = this.GetFields (obj);

			foreach (PropertyValue<int> field in fields.Ids)
			{
				this.writer.WriteObjectFieldReference (obj, this.GetPropertyName (field.Property), field.Value);
			}
			foreach (PropertyValue<string> field in fields.Values)
			{
				this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (field.Property), Context.EscapeString (field.Value));
			}
			foreach (KeyValuePair<DependencyProperty, Binding> field in fields.Bindings)
			{
				this.writer.WriteObjectFieldValue (obj, field.Key.Name, this.ConvertBindingToString (field.Value));
			}

			foreach (PropertyValue<IList<int>> field in fields.IdCollections)
			{
				if (field.Value.Count > 0)
				{
					this.writer.WriteObjectFieldReferenceList (obj, this.GetPropertyName (field.Property), field.Value);
				}
			}

			this.writer.EndObject (id, obj);
		}

		
		private Fields GetFields(DependencyObject obj)
		{
			//	Generate sorted lists describing the different fields, based on
			//	their type :
			//
			//	- DependencyObject references
			//	- Serializable values
			//	
			//	This skips all fields which are data bound, as they do not need
			//	to be serialized; the bindings themselves will be serialized
			//	instead.
			
			Fields fields = new Fields ();

			foreach (LocalValueEntry entry in obj.LocalValueEntries)
			{
				DependencyPropertyMetadata metadata = entry.Property.GetMetadata (obj);

				if ((entry.Property.IsReadWrite) ||
					(metadata.CanSerializeReadOnly))
				{
					if (obj.GetBinding (entry.Property) == null)
					{
						DependencyObject dependencyObjectValue = entry.Value as DependencyObject;

						if (dependencyObjectValue != null)
						{
							int id = this.objMap.GetId (dependencyObjectValue);
							fields.Add (entry.Property, id);
							continue;
						}

						ICollection<DependencyObject> dependencyObjectCollection = entry.Value as ICollection<DependencyObject>;

						if (dependencyObjectCollection != null)
						{
							List<int> ids = new List<int> ();
							foreach (DependencyObject node in dependencyObjectCollection)
							{
								ids.Add (this.ObjectMap.GetId (node));
							}
							fields.Add (entry.Property, ids);
							continue;
						}

						Binding binding = entry.Value as Binding;

						if (binding != null)
						{
							fields.Add (entry.Property, this.ConvertBindingToString (binding));
							continue;
						}

						string value = entry.Property.ConvertToString (entry.Value);

						if (value != null)
						{
							fields.Add (entry.Property, value);
							continue;
						}
					}
				}
			}

			return fields;
		}

		public class Fields
		{
			public Fields()
			{
			}

			public IList<PropertyValue<int>> Ids
			{
				get
				{
					return this.ids;
				}
			}
			public IList<PropertyValue<string>> Values
			{
				get
				{
					return this.values;
				}
			}
			public IList<PropertyValue<IList<int>>> IdCollections
			{
				get
				{
					return this.idCollections;
				}
			}
			public IList<PropertyValue<IList<string>>> ValueCollections
			{
				get
				{
					return this.valueCollections;
				}
			}
			public IList<KeyValuePair<DependencyProperty, Binding>> Bindings
			{
				get
				{
					return this.bindings;
				}
			}

			public void Add(DependencyProperty property, int id)
			{
				this.ids.Add (new PropertyValue<int> (property, id));
			}
			public void Add(DependencyProperty property, string value)
			{
				this.values.Add (new PropertyValue<string> (property, value));
			}
			public void Add(DependencyProperty property, IEnumerable<int> collection)
			{
				List<int> list = new List<int> ();
				list.AddRange (collection);
				this.idCollections.Add (new PropertyValue<IList<int>> (property, list));
			}
			public void Add(DependencyProperty property, IEnumerable<string> collection)
			{
				List<string> list = new List<string> ();
				list.AddRange (collection);
				this.valueCollections.Add (new PropertyValue<IList<string>> (property, list));
			}
			public void Add(DependencyProperty property, Binding binding)
			{
				this.bindings.Add (new KeyValuePair<DependencyProperty, Binding> (property, binding));
			}

			private List<PropertyValue<int>> ids = new List<PropertyValue<int>> ();
			private List<PropertyValue<string>> values = new List<PropertyValue<string>> ();
			private List<PropertyValue<IList<int>>> idCollections = new List<PropertyValue<IList<int>>> ();
			private List<PropertyValue<IList<string>>> valueCollections = new List<PropertyValue<IList<string>>> ();
			private List<KeyValuePair<DependencyProperty, Binding>> bindings = new List<KeyValuePair<DependencyProperty, Binding>> ();
		}

	}
}
