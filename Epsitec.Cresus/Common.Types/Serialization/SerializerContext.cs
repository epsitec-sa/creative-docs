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

			foreach (PropertyValue<string> field in fields.Values)
			{
				this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (field.Property), field.Value);
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

			foreach (KeyValuePair<DependencyProperty, Binding> entry in obj.GetAllBindings ())
			{
				string markup = MarkupExtension.BindingToString (entry.Value, this);
				
				fields.Add (entry.Key, markup);
			}
			
			foreach (LocalValueEntry entry in obj.LocalValueEntries)
			{
				DependencyPropertyMetadata metadata = entry.Property.GetMetadata (obj);

				if ((entry.Property.IsReadWrite) ||
					(metadata.CanSerializeReadOnly))
				{
					if (obj.GetBinding (entry.Property) == null)
					{
						if (this.ExternalMap.IsValueDefined (entry.Value))
						{
							//	This is an external reference. Record it as {External xxx}.

							string markup = MarkupExtension.ExtRefToString (entry.Value, this);
							
							fields.Add (entry.Property, markup);
							continue;
						}
						
						DependencyObject dependencyObjectValue = entry.Value as DependencyObject;

						if (dependencyObjectValue != null)
						{
							//	This is an internal reference. Record it as {Object _nnn}.

							int id = this.ObjectMap.GetId (dependencyObjectValue);

							string markup = MarkupExtension.ObjRefToString (dependencyObjectValue, this);
							
							fields.Add (entry.Property, markup);
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

						string value = entry.Property.ConvertToString (entry.Value, this);

						if (value != null)
						{
							fields.Add (entry.Property, MarkupExtension.Escape (value));
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

			private List<PropertyValue<string>> values = new List<PropertyValue<string>> ();
			private List<PropertyValue<IList<int>>> idCollections = new List<PropertyValue<IList<int>>> ();
			private List<PropertyValue<IList<string>>> valueCollections = new List<PropertyValue<IList<string>>> ();
		}

	}
}
