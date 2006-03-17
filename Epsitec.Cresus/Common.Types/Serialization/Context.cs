//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	/// <summary>
	/// The Context class is either used by itself to analyse a DependencyObject
	/// graph, or as a base class to SerializerContext and DeserializerContext;
	/// see also the Epsitec.Common.Types.Storage class.
	/// </summary>
	public class Context : IContextResolver
	{
		public Context()
		{
			this.objMap = new MapId<DependencyObject> ();
			this.externalMap = new MapTag<object> ();
			this.typeIds = new Dictionary<System.Type, int> ();
			this.unknownMap = new MapId<object> ();
		}
		
		public MapId<DependencyObject>			ObjectMap
		{
			get
			{
				return this.objMap;
			}
		}
		public MapTag<object>					ExternalMap
		{
			get
			{
				return this.externalMap;
			}
		}
		public MapId<object>					UnknownMap
		{
			get
			{
				return this.unknownMap;
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
		
		public void StoreTypeDefinition(int id, System.Type type)
		{
			this.AssertWritable ();
			
			System.Diagnostics.Debug.Assert (this.typeIds.ContainsKey (type) == false);
			
			this.typeIds[type] = id;
			this.writer.WriteTypeDefinition (id, type.FullName);
		}
		public void StoreObjectDefinition(int id, DependencyObject obj)
		{
			this.AssertWritable ();
			
			System.Type type = obj.GetType ();
			int typeId = this.typeIds[type];

			this.writer.WriteObjectDefinition (id, typeId);
		}
		public void StoreExternalDefinition(string tag)
		{
			this.AssertWritable ();
			this.writer.WriteExternalReference (tag);
		}

		public void RestoreTypeDefinition()
		{
			this.AssertReadable ();
			
			int id = this.ObjectMap.TypeCount;
			string name = this.reader.ReadTypeDefinition (id);

			DependencyObjectType type = DependencyClassManager.Current.FindObjectType (name);

			if (type == null)
			{
				throw new System.ArgumentException (string.Format ("Type {0} cannot be resolved", name));
			}

			this.typeIds[type.SystemType] = id;
			this.ObjectMap.RecordType (type.SystemType);
		}
		public void RestoreObjectDefinition()
		{
			this.AssertReadable ();

			int id = this.ObjectMap.ValueCount;
			int typeId = this.reader.ReadObjectDefinition (id);

			DependencyObjectType type = DependencyObjectType.FromSystemType (this.objMap.GetType (typeId));
			DependencyObject value = type.CreateEmptyObject ();
			
			this.ObjectMap.Record (value);
		}
		public void RestoreExternalDefinition()
		{
			this.AssertReadable ();
			
			string tag = this.reader.ReadExternalReference ();

			if (this.ExternalMap.IsTagDefined (tag))
			{
				//	OK. Tag defined.
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Required external value '{0}' not defined in context", tag));
			}
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

		public string ResolveToMarkup(object value)
		{
			//	Map an object reference to a markup extension. If the object is
			//	not known in this context, returns null.
			
			if (value == null)
			{
				return MarkupExtension.NullToString ();
			}
			
			DependencyObject obj = value as DependencyObject;
			
			if ((obj != null) &&
				(this.objMap.IsValueDefined (obj)))
			{
				return MarkupExtension.ObjRefToString (obj, this);
			}

			return null;
		}
		public object ResolveFromMarkup(string tagId)
		{
			//	Map a tag id to an object.

			if (tagId == null)
			{
				throw new System.ArgumentNullException ();
			}

			//	TODO: use MarkupExtension to parse the tag
			
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
		
		public static string NumToString(int num)
		{
			return num.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}
		public static int ParseNum(string value)
		{
			return int.Parse (value, System.Globalization.CultureInfo.InvariantCulture);
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


		private MapId<DependencyObject>			objMap;
		private MapTag<object>					externalMap;
		private Dictionary<System.Type, int>	typeIds;
		private MapId<object>					unknownMap;
		
		protected IO.AbstractWriter				writer;
		protected IO.AbstractReader				reader;
	}
}
