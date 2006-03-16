//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public class Context : IContextResolver
	{
		public Context()
		{
			this.objMap = new MapId<DependencyObject> ();
			this.externalMap = new MapId<object> ();
			this.typeIds = new Dictionary<System.Type, int> ();
		}
		
		public MapId<DependencyObject>			ObjectMap
		{
			get
			{
				return this.objMap;
			}
		}
		public MapId<object> ExternalMap
		{
			get
			{
				return this.externalMap;
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
			//	Convert a property to a full name :
			//
			//	- For standard properties, returns "Name".
			//	- For attached properties, returns "_nnn.Name" where _nnn is the
			//	  id tag for the type which owns the property.
			
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
		public DependencyProperty GetProperty(DependencyObject obj, string fullName)
		{
			//	Return the property which matches the specified full name. See
			//	method GetProperyName.

			if (fullName.Contains ("."))
			{
				string[] args = fullName.Split ('.');

				System.Diagnostics.Debug.Assert (args.Length == 2);

				string typeTag = args[0];
				string shortName = args[1];

				System.Type sysType = this.objMap.GetType (Context.ParseId (typeTag));
				DependencyObjectType objType = DependencyObjectType.FromSystemType (sysType);

				return objType.GetProperty (shortName);
			}
			else
			{
				return obj.ObjectType.GetProperty (fullName);
			}
		}

		#region IContextResolver Members

		public string ResolveToId(object value)
		{
			//	Map an object reference to a tag id. If the object is not known
			//	in this context, returns null.
			
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
		public object ResolveFromId(string tagId)
		{
			//	Map a tag id to an object.

			if (tagId == null)
			{
				throw new System.ArgumentNullException ();
			}

			if (tagId == "null")
			{
				return null;
			}

			return this.objMap.GetValue (Context.ParseId (tagId));
		}

		#endregion

		public static string IdToString(int id)
		{
			//	Convert an id to a string representation, which is compatible
			//	with the XML naming conventions.
			
			return string.Concat ("_", id.ToString (System.Globalization.CultureInfo.InvariantCulture));
		}
		public static int ParseId(string value)
		{
			//	Convert a string representation of an id to its integer value.
			//	See method IdToString.
			
			System.Diagnostics.Debug.Assert (value.Length > 1);
			System.Diagnostics.Debug.Assert (value[0] == '_');

			return int.Parse (value.Substring (1), System.Globalization.CultureInfo.InvariantCulture);
		}

		public static string EscapeString(string value)
		{
			//	If needed, inserts a special escape sequence to make the value
			//	valid and easily recognizable as escaped by the markup extension
			//	parser.
			//
			//	NB: A value string may not contain { and } curly braces, since
			//		these are used to define the markup extensions.
			
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
			//	If the string was escaped, remove the escape sequence and
			//	return the original string (see EscapeString).
			
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
			//	Return true is the value is a markup extension. This does not
			//	check for a valid syntax; it only analyses the value to see if
			//	the "{" and "}" markers are found.
			//
			//	Escaped values are recognized as such and won't be considered
			//	to be markup extensions.
			
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
			//	Convert a value to a markup extension by embedding it within a
			//	pair of { and }.
			
			if (value == null)
			{
				throw new System.ArgumentNullException ("Invalid null markup extension");
			}
			if (value.Length == 0)
			{
				throw new System.ArgumentException ("Invalid empty markup extension");
			}
			
			return string.Concat ("{", value, "}");
		}
		public static string ConvertFromMarkupExtension(string value)
		{
			//	Remove the { and } which embed the markup extension.
			
			System.Diagnostics.Debug.Assert (Context.IsMarkupExtension (value));
			return value.Substring (1, value.Length-2);
		}

		public static string ConvertBindingToString(Binding binding, IContextResolver resolver)
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
				string id = resolver.ResolveToId (source);

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


		protected MapId<DependencyObject>			objMap;
		protected MapId<object>					externalMap;
		protected Dictionary<System.Type, int>	typeIds;
		
		protected IO.AbstractWriter				writer;
		protected IO.AbstractReader				reader;
	}
}
