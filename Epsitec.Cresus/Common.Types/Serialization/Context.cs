//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public class Context
	{
		public Context()
		{
			this.visitor = new GraphVisitor ();
			this.typeIds = new Dictionary<System.Type, int> ();
		}
		public Context(IO.AbstractReader reader)
			: this ()
		{
			this.reader = reader;
		}
		public Context(IO.AbstractWriter writer)
			: this ()
		{
			this.writer = writer;
		}
		
		public GraphVisitor						Visitor
		{
			get
			{
				return this.visitor;
			}
		}
		public Map<DependencyObject>			ObjectMap
		{
			get
			{
				return this.visitor.ObjectMap;
			}
		}
		
		public IO.AbstractReader				Reader
		{
			get
			{
				return this.reader;
			}
		}
		public IO.AbstractWriter				Writer
		{
			get
			{
				return this.writer;
			}
		}
		
		internal void DefineType(int id, System.Type type)
		{
			this.AssertWritable ();
			
			System.Diagnostics.Debug.Assert (this.typeIds.ContainsKey (type) == false);
			
			this.typeIds[type] = id;
			this.writer.WriteTypeDefinition (id, type.FullName);
		}

		internal void DefineObject(int id, DependencyObject obj)
		{
			this.AssertWritable ();
			
			System.Type type = obj.GetType ();
			int typeId = this.typeIds[type];

			this.writer.WriteObjectDefinition (id, typeId);
		}

		internal void StoreObject(int id, DependencyObject obj)
		{
			this.AssertWritable ();

			this.writer.BeginObject (id, obj);

			Fields fields = this.GetFields (obj);

			foreach (PropertyValue<int> field in fields.Ids)
			{
				if (field.IsDataBound == false)
				{
					this.writer.WriteObjectFieldReference (obj, this.GetPropertyName (field.Property), field.Value);
				}
			}
			foreach (PropertyValue<string> field in fields.Values)
			{
				if (field.IsDataBound == false)
				{
					this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (field.Property), Context.EscapeString (field.Value));
				}
			}
			foreach (KeyValuePair<DependencyProperty, Binding> field in fields.Bindings)
			{
				this.writer.WriteObjectFieldValue (obj, field.Key.Name, this.ConvertBindingToString (field.Value));
			}
			
			foreach (PropertyValue<IList<int>> field in fields.IdCollections)
			{
				if ((field.IsDataBound == false) &&
					(field.Value.Count > 0))
				{
					this.writer.WriteObjectFieldReferenceList (obj, this.GetPropertyName (field.Property), field.Value);
				}
			}
			
			this.writer.EndObject (id, obj);
		}

		private string GetPropertyName(DependencyProperty property)
		{
			if (property.IsAttached)
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "_{0}.{1}", this.ObjectMap.GetTypeIndex (property.OwnerType), property.Name);
			}
			else
			{
				return property.Name;
			}
		}

		private static string EscapeString(string value)
		{
			if ((value == null) ||
				(value.IndexOfAny (new char[] { '{', '}' }) < 0))
			{
				return value;
			}
			else
			{
				return string.Concat ("{}", value);
			}
		}
		private static string UnescapeString(string value)
		{
			if ((value != null) &&
				(value.StartsWith ("{}")))
			{
				return value.Substring (2);
			}
			else
			{
				return value;
			}
		}
		
		private static bool IsMarkupExtension(string value)
		{
			if ((value != null) &&
				(value.StartsWith ("{")) &&
				(value.StartsWith ("{}") == false) &&
				(value.EndsWith ("}")))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		private static string ConvertToMarkupExtension(string value)
		{
			return string.Concat ("{", value, "}");
		}
		private static string ConvertFromMarkupExtension(string value)
		{
			System.Diagnostics.Debug.Assert (Context.IsMarkupExtension (value));
			return value.Substring (1, value.Length-2);
		}

		public string ConvertBindingToString(Binding binding)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("{");
			buffer.Append ("Binding");

			string space = " ";

			DependencyObject source = binding.Source as DependencyObject;
			BindingMode mode = binding.Mode;
			DependencyPropertyPath path = binding.Path;

			if (source != null)
			{
				int id = this.ObjectMap.GetId (source);

				if (id < 0)
				{
					//	TODO: handle unknown sources
				}
				else
				{
					buffer.Append (space);
					space = ", ";
					
					buffer.Append ("Source={ref _");
					buffer.Append (id.ToString (System.Globalization.CultureInfo.InvariantCulture));
					buffer.Append ("}");
				}
			}

			if (path != null)
			{
				string value = path.GetFullPath ();

				if (value.Length > 0)
				{
					buffer.Append (space);
					space = ", ";

					buffer.Append ("Path=");
					buffer.Append (value);
				}
			}

			if (mode != BindingMode.None)
			{
				string value = mode.ToString ();

				buffer.Append (space);
				space = ", ";
				
				buffer.Append ("Mode=");
				buffer.Append (value);
			}
			
			buffer.Append ("}");
			
			return buffer.ToString ();
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
						fields.Add (entry.Property, this.ObjectMap.GetId (dependencyObjectValue), isDataBound);
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
						fields.Add (entry.Property, ids, isDataBound);
						continue;
					}

					binding = entry.Value as Binding;

					if (binding != null)
					{
						fields.Add (entry.Property, this.ConvertBindingToString (binding), false);
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
		
		private void AssertWritable()
		{
			if (this.writer == null)
			{
				throw new System.InvalidOperationException (string.Format ("No writer associated with serialization context"));
			}
		}
		private void AssertReadable()
		{
			if (this.reader == null)
			{
				throw new System.InvalidOperationException (string.Format ("No reader associated with serialization context"));
			}
		}
		
		
		private GraphVisitor visitor;
		private Dictionary<System.Type, int> typeIds;
		private IO.AbstractWriter writer;
		private IO.AbstractReader reader;
	}
}
