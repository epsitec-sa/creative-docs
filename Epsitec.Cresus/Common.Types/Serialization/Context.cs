//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public abstract class Context : IContextResolver
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
				string typeTag = Context.IdToString (typeId);
				return string.Concat (typeTag, ".", property.Name);
			}
			else
			{
				return property.Name;
			}
		}
		public DependencyProperty GetProperty(DependencyObject obj, string name)
		{
			if (name.Contains ("."))
			{
				string[] args = name.Split ('.');

				System.Diagnostics.Debug.Assert (args.Length == 2);

				string typeTag = args[0];
				string shortName = args[1];

				System.Type sysType = this.objMap.GetType (Context.ParseId (typeTag));
				DependencyObjectType objType = DependencyObjectType.FromSystemType (sysType);

				return objType.GetProperty (shortName);
			}
			else
			{
				return obj.ObjectType.GetProperty (name);
			}
		}

		#region IContextResolver Members

		public string ResolveToId(object value)
		{
			if (value == null)
			{
				return "null";
			}
			
			DependencyObject obj = value as DependencyObject;
			
			if (obj != null)
			{
				return Context.IdToString (this.objMap.GetId (obj));
			}

			return null;
		}
		public object ResolveFromId(string id)
		{
			if (id == null)
			{
				throw new System.ArgumentNullException ();
			}
			
			if (id == "null")
			{
				return null;
			}

			return this.objMap.GetValue (Context.ParseId (id));
		}

		#endregion

		public static string IdToString(int id)
		{
			return string.Concat ("_", id.ToString (System.Globalization.CultureInfo.InvariantCulture));
		}
		public static int ParseId(string value)
		{
			System.Diagnostics.Debug.Assert (value.Length > 1);
			System.Diagnostics.Debug.Assert (value[0] == '_');

			return int.Parse (value.Substring (1), System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string EscapeString(string value)
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
		public static string UnescapeString(string value)
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

		public static bool IsMarkupExtension(string value)
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

		public static string ConvertToMarkupExtension(string value)
		{
			return string.Concat ("{", value, "}");
		}
		public static string ConvertFromMarkupExtension(string value)
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
				string id = this.ResolveToId (source);

				if (id == null)
				{
					//	TODO: handle unknown sources
				}
				else
				{
					buffer.Append (space);
					space = ", ";
					
					buffer.Append ("Source={ref ");
					buffer.Append (id);
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
