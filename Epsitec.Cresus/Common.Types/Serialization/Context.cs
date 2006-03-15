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

			GraphVisitor.Fields fields = this.visitor.GetFields (obj);

			foreach (PropertyValue<int> field in fields.Ids)
			{
				if (field.IsDataBound == false)
				{
					this.writer.WriteObjectFieldReference (obj, field.Name, field.Value);
				}
			}
			foreach (PropertyValue<string> field in fields.Values)
			{
				if (field.IsDataBound == false)
				{
					this.writer.WriteObjectFieldValue (obj, field.Name, Context.EscapeString (field.Value));
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
					this.writer.WriteObjectFieldReferenceList (obj, field.Name, field.Value);
				}
			}
			
			this.writer.EndObject (id, obj);
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

		private string ConvertBindingToString(Binding binding)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("{");
			buffer.Append ("Binding ");

			int args = 0;

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
					if (args++ > 0)
					{
						buffer.Append (", ");
					}
					
					buffer.Append ("Source={ref ");
					buffer.Append (id.ToString (System.Globalization.CultureInfo.InvariantCulture));
					buffer.Append ("}");
				}
			}

			if (path != null)
			{
				string value = path.GetFullPath ();

				if (value.Length > 0)
				{
					if (args++ > 0)
					{
						buffer.Append (", ");
					}

					buffer.Append ("Path=");
					buffer.Append (value);
				}
			}

			if (mode != BindingMode.None)
			{
				string value = mode.ToString ();

				if (args++ > 0)
				{
					buffer.Append (", ");
				}
				
				buffer.Append ("Mode=");
				buffer.Append (value);
			}
			
			buffer.Append ("}");
			
			return buffer.ToString ();
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
