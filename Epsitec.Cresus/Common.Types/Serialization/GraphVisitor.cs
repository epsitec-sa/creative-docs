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

					if (dependencyObjectValue != null)
					{
						fields.Add (entry.Property, this.objMap.GetId (dependencyObjectValue));
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
						fields.Add (entry.Property, ids);
						continue;
					}

					//	TODO: convert to string
					
					string value = entry.Value as string;

					if (value != null)
					{
						fields.Add (entry.Property, value);
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
			
			private List<PropertyValue<int>> ids = new List<PropertyValue<int>> ();
			private List<PropertyValue<string>> values = new List<PropertyValue<string>> ();
			private List<PropertyValue<IList<int>>> idCollections = new List<PropertyValue<IList<int>>> ();
			private List<PropertyValue<IList<string>>> valueCollections = new List<PropertyValue<IList<string>>> ();
		}

		private Generic.Map<DependencyObject>	objMap = new Generic.Map<DependencyObject> ();
	}
}
