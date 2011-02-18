//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	public class Context : IContextResolver, System.IDisposable
	{
		public Context()
		{
			this.objMap = new MapId<DependencyObject> ();
			this.externalMap = new MapTag<object> ();
			this.typeIds = new Dictionary<System.Type, int> ();
			this.unknownMap = new MapId<object> ();
			this.converters = new Dictionary<System.Type, ISerializationConverter> ();

			Context.Link (this);
		}

		public MapId<DependencyObject>			ObjectMap
		{
			get
			{
				//	The ObjectMap allows to map between known objects and their
				//	id, and map between the known types and their index.
				
				return this.objMap;
			}
		}
		public MapTag<object>					ExternalMap
		{
			get
			{
				//	The ExternalMap is used to name (with tags) all known objects
				//	which are to be treated as external to the serialized graph.
				
				return this.externalMap;
			}
		}
		public MapId<object>					UnknownMap
		{
			get
			{
				//	The UnknownMap records all objects which could not be resolved
				//	as belonging to the graph or to the external objects (these are
				//	usually objects referenced as binding sources).
				
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

		public System.Type RestoreTypeDefinition()
		{
			this.AssertReadable ();
			
			int id = this.ObjectMap.TypeCount;
			string name = this.reader.ReadTypeDefinition (id);

			DependencyObjectType type = DependencyClassManager.Current.FindObjectType (name);

			if (type == null)
			{
				throw new System.ArgumentException (string.Format ("Type {0} cannot be resolved. Did you forget the [assembly: Epsitec.Common.Types.DependencyClass] attribute on the specified class?", name));
			}

			this.typeIds[type.SystemType] = id;
			this.ObjectMap.RecordType (type.SystemType);

			return type.SystemType;
		}
		public DependencyObject RestoreObjectDefinition()
		{
			this.AssertReadable ();

			int id = this.ObjectMap.ValueCount;
			int typeId = this.reader.ReadObjectDefinition (id);

			DependencyObjectType type = DependencyObjectType.FromSystemType (this.ObjectMap.GetType (typeId));
			DependencyObject value = type.CreateEmptyObject ();
			
			this.ObjectMap.Record (value);
			
			return value;
		}
		public string RestoreExternalDefinition()
		{
			this.AssertReadable ();
			
			string tag = this.reader.ReadExternalReference ();

			if (this.ExternalMap.IsTagDefined (tag))
			{
				//	OK. Tag defined.
				
				return tag;
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Required external value '{0}' not defined in context", tag));
			}
		}

		public virtual void StoreObjectData(int id, DependencyObject obj)
		{
			throw new System.InvalidOperationException ("StoreObjectData not supported");
		}
		public virtual void RestoreObjectData(int id, DependencyObject obj)
		{
			throw new System.InvalidOperationException ("RestoreObjectData not supported");
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
				int typeId = this.ObjectMap.GetTypeIndex (property.OwnerType);
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

				System.Type sysType = this.ObjectMap.GetType (Context.ParseId (typeTag));
				DependencyObjectType objType = DependencyObjectType.FromSystemType (sysType);

				return objType.GetProperty (shortName);
			}
			else
			{
				return obj.ObjectType.GetProperty (fullName);
			}
		}

		public ISerializationConverter FindConverter(System.Type type)
		{
			ISerializationConverter converter;
			
			if (this.converters.TryGetValue (type, out converter))
			{
				return converter;
			}
			
			converter = Serialization.DependencyClassManager.Current.FindSerializationConverter (type);
			converter = converter ?? InvariantConverter.GetSerializationConverter (type);

			this.converters[type] = converter;

			return converter;
		}

		public ISerializationConverter FindConverterForCollection(System.Type collectionType)
		{
			System.Type countableType;
			return this.FindConverterForCollection (collectionType, out countableType);
		}

		public ISerializationConverter FindConverterForCollection(System.Type collectionType, out System.Type countableType)
		{
			//	Maybe the provided type is already an ICollection<T> interface :

			ISerializationConverter converter = this.FindConverterForCollectionInterfaceType (collectionType, out countableType);

			//	If not, then look up its interfaces and search for an ICollection<T>
			//	which we could use...
			
			if (converter == null)
			{
				System.Type[] types = collectionType.GetInterfaces ();

				foreach (System.Type type in types)
				{
					converter = this.FindConverterForCollectionInterfaceType (type, out countableType);

					if (converter != null)
					{
						break;
					}
				}
			}
			
			return converter;
		}

		private ISerializationConverter FindConverterForCollectionInterfaceType(System.Type type, out System.Type countableType)
		{
			if ((type.Name == "ICollection`1") &&
				(type.IsGenericType))
			{
				System.Type[] genericArgs = type.GetGenericArguments ();

				if (genericArgs.Length == 1)
				{
					ISerializationConverter converter = this.FindConverter (genericArgs[0]);

					if (converter != null)
					{
						countableType = type;
						return converter;
					}
				}
			}

			countableType = null;
			return null;
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

			if (obj != null)
			{
				if (this.ObjectMap.IsValueDefined (obj))
				{
					return MarkupExtension.ObjRefToString (obj, this);
				}
			}
			
			if (this.ExternalMap.IsValueDefined (value))
			{
				return MarkupExtension.ExtRefToString (value, this);
			}

			return null;
		}
		
		public object ResolveFromMarkup(string tagId, System.Type type)
		{
			//	Map a tag id to an object.

			if (tagId == null)
			{
				throw new System.ArgumentNullException ();
			}

			if (MarkupExtension.IsMarkupExtension (tagId) == false)
			{
				throw new System.NotImplementedException (string.Format ("Cannot resolve '{0}'", tagId));
			}

			return MarkupExtension.Resolve (this, tagId, type);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		public virtual void NotifySerializationCompleted()
		{
		}
		
		/// <summary>
		/// Gets the active context.
		/// </summary>
		/// <returns>Active context or <c>null</c>.</returns>
		public static Context GetActiveContext()
		{
			foreach (Weak<Context> item in Context.links)
			{
				Context context = item.Target;

				if (context != null)
				{
					return context;
				}
			}
			
			return null;
		}

		public static Support.ResourceManager GetResourceManager(IContextResolver context)
		{
			if (context == null)
			{
				return null;
			}
			else
			{
				return context.ExternalMap.GetValue (Serialization.Context.WellKnownTagResourceManager) as Support.ResourceManager;
			}
		}
		
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

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Context.Unlink (this);
			}
		}

		private static void Link(Context context)
		{
			if (Context.links == null)
			{
				Context.links = new List<Weak<Context>> ();
			}

			Context.links.Add (new Weak<Context> (context));
		}

		private static void Unlink(Context context)
		{
			List<Weak<Context>> list = new List<Weak<Context>> ();
			
			foreach (Weak<Context> item in Context.links)
			{
				if (item.IsAlive)
				{
					if (item.Target == context)
					{
						//	Remove item from links.
					}
					else
					{
						list.Add (item);
					}
				}
			}

			Context.links = list;
		}

		public static readonly string			WellKnownTagResourceManager = "_ResourceManager";
		public static readonly string			WellKnownTagDataSource = "_DataSource";
		public static readonly string			WellKnownTagApplication = "_Application";
		public static readonly string			WellKnownTagDocument = "_Document";

		[System.ThreadStatic]
		private static List<Weak<Context>>		links;
		
		private MapId<DependencyObject>			objMap;
		private MapTag<object>					externalMap;
		private Dictionary<System.Type, int>	typeIds;
		private MapId<object>					unknownMap;
		private Dictionary<System.Type, ISerializationConverter> converters;
		
		protected IO.AbstractWriter				writer;
		protected IO.AbstractReader				reader;

		public object GetEntry(object key)
		{
			if (this.entries == null)
			{
				return null;
			}

			object value;

			this.entries.TryGetValue (key, out value);
			
			return value;
		}

		public void SetEntry(object key, object data)
		{
			if (this.entries == null)
			{
				this.entries = new Dictionary<object, object> ();
			}

			this.entries[key] = data;
		}

		private Dictionary<object, object>		entries;
	}
}
