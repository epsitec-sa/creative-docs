//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

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

		public void VisitSerializableNodes(DependencyObject obj)
		{
			if (this.objMap.Record (obj))
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

		public Fields GetFields(DependencyObject obj)
		{
			Fields fields = new Fields ();
			
			foreach (LocalValueEntry entry in obj.LocalValueEntries)
			{
				DependencyPropertyMetadata metadata = entry.Property.GetMetadata (obj);

				if ((entry.Property.IsReadWrite) ||
					(metadata.CanSerializeReadOnly))
				{
					DependencyObject dependencyObjectValue = entry.Value as DependencyObject;
					Binding binding = obj.GetBinding (entry.Property);

					bool isDataBound;

					if (binding == null)
					{
						isDataBound = false;
					}
					else
					{
						isDataBound = true;
						
						fields.Add (entry.Property, binding);
					}

					if (dependencyObjectValue != null)
					{
						fields.Add (entry.Property, this.objMap.GetId (dependencyObjectValue), isDataBound);
						continue;
					}
					
					ICollection<DependencyObject> dependencyObjectCollection = entry.Value as ICollection<DependencyObject>;
					
					if (dependencyObjectCollection != null)
					{
						List<int> ids = new List<int> ();
						foreach (DependencyObject node in dependencyObjectCollection)
						{
							ids.Add (this.objMap.GetId (node));
						}
						fields.Add (entry.Property, ids, isDataBound);
						continue;
					}

					string value = entry.Property.ConvertToString (entry.Value);

					if (value != null)
					{
						fields.Add (entry.Property, value, isDataBound);
						continue;
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

			public void Add(DependencyProperty property, int id, bool isDataBound)
			{
				this.ids.Add (new PropertyValue<int> (property, id, isDataBound));
			}
			public void Add(DependencyProperty property, string value, bool isDataBound)
			{
				this.values.Add (new PropertyValue<string> (property, value, isDataBound));
			}
			public void Add(DependencyProperty property, IEnumerable<int> collection, bool isDataBound)
			{
				List<int> list = new List<int> ();
				list.AddRange (collection);
				this.idCollections.Add (new PropertyValue<IList<int>> (property, list, isDataBound));
			}
			public void Add(DependencyProperty property, IEnumerable<string> collection, bool isDataBound)
			{
				List<string> list = new List<string> ();
				list.AddRange (collection);
				this.valueCollections.Add (new PropertyValue<IList<string>> (property, list, isDataBound));
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

		private Generic.Map<DependencyObject>	objMap = new Generic.Map<DependencyObject> ();
	}
}
