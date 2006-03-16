//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public abstract class Context
	{
		protected Context()
		{
			this.objMap = new Map<DependencyObject> ();
			this.typeIds = new Dictionary<System.Type, int> ();
		}
		
		public Map<DependencyObject>			ObjectMap
		{
			get
			{
				return this.objMap;
			}
		}
		
		public IO.AbstractReader				ActiveReader
		{
			get
			{
				return this.reader;
			}
		}
		public IO.AbstractWriter				ActiveWriter
		{
			get
			{
				return this.writer;
			}
		}
		
		public void DefineType(int id, System.Type type)
		{
			this.AssertWritable ();
			
			System.Diagnostics.Debug.Assert (this.typeIds.ContainsKey (type) == false);
			
			this.typeIds[type] = id;
			this.writer.WriteTypeDefinition (id, type.FullName);
		}
		public void DefineObject(int id, DependencyObject obj)
		{
			this.AssertWritable ();
			
			System.Type type = obj.GetType ();
			int typeId = this.typeIds[type];

			this.writer.WriteObjectDefinition (id, typeId);
		}

		public virtual void StoreObject(int id, DependencyObject obj)
		{
			throw new System.InvalidOperationException ("StoreObject not supported");
		}

		public string GetPropertyName(DependencyProperty property)
		{
			if (property.IsAttached)
			{
				int typeId = this.objMap.GetTypeIndex (property.OwnerType);
				string typeTag = IO.XmlSupport.IdToString (typeId);
				return string.Concat (typeTag, ".", property.Name);
			}
			else
			{
				return property.Name;
			}
		}
		public DependencyProperty GetProperty(string name)
		{
			return null;
		}

		protected static string EscapeString(string value)
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
		protected static string UnescapeString(string value)
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

		protected static bool IsMarkupExtension(string value)
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

		protected static string ConvertToMarkupExtension(string value)
		{
			return string.Concat ("{", value, "}");
		}
		protected static string ConvertFromMarkupExtension(string value)
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

		protected void AssertWritable()
		{
			if (this.writer == null)
			{
				throw new System.InvalidOperationException (string.Format ("No writer associated with serialization context"));
			}
		}
		protected void AssertReadable()
		{
			if (this.reader == null)
			{
				throw new System.InvalidOperationException (string.Format ("No reader associated with serialization context"));
			}
		}


		protected Generic.Map<DependencyObject> objMap = new Generic.Map<DependencyObject> ();
		protected Dictionary<System.Type, int> typeIds;
		protected IO.AbstractWriter writer;
		protected IO.AbstractReader reader;
	}
}
