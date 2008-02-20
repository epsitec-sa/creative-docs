//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public class DeserializerContext : Context
	{
		public DeserializerContext(IO.AbstractReader reader)
		{
			this.reader = reader;
		}

		public override void RestoreObjectData(int id, DependencyObject obj)
		{
			this.AssertReadable ();

			this.reader.BeginObject (id, obj);

			string field;
			string value;

			while (this.reader.ReadObjectFieldValue (obj, out field, out value))
			{
				this.RestoreObjectField (obj, field, value);
			}
			
			this.reader.EndObject (id, obj);
		}

		private void RestoreObjectField(DependencyObject obj, string field, string value)
		{
			DependencyProperty property = null;

			if (field.IndexOf ('.') < 0)
			{
				//	This is a standard, simple field.

				property = obj.ObjectType.GetProperty (field);
			}
			else
			{
				string[] args = field.Split ('.');
				
				string typeTag = args[0];
				string name    = args[1];

				DependencyObjectType type = DependencyObjectType.FromSystemType (this.ObjectMap.GetType (Context.ParseId (typeTag)));

				property = type.GetProperty (name);
			}

			if (property == null)
			{
				throw new System.ArgumentException (string.Format ("Property {0} could not be resolved", field));
			}

			if (MarkupExtension.IsMarkupExtension (value))
			{
				//	This is a markup extension
				
				object data = this.ResolveFromMarkup (value, property.PropertyType);
				System.Type dataType = data == null ? typeof (object) : data.GetType ();

				if ((TypeRosetta.DoesTypeImplementInterface (dataType, typeof (IEnumerable<DependencyObject>))) &&
					(property.IsPropertyTypeAnICollectionOfDependencyObject))
				{
					//	Assign a collection of DependencyObject to a property which implements
					//	such a collection.

					DeserializerContext.RestoreCollection<DependencyObject> (obj, field, property, data);
				}
				else if ((TypeRosetta.DoesTypeImplementInterface (dataType, typeof (IEnumerable<string>))) &&
					     (property.IsPropertyTypeAnICollectionOfString))
				{
					DeserializerContext.RestoreCollection<string> (obj, field, property, data);
				}
				else if ((TypeRosetta.DoesTypeImplementInterface (dataType, typeof (System.Collections.IEnumerable))) &&
					     (property.IsPropertyTypeAnICollectionOfAny))
				{
					DeserializerContext.RestoreEnumerable (obj, field, property, data);
				}
				else if (data is Binding)
				{
					//	Rebind property if it was a binding...
					
					obj.SetBinding (property, data as Binding);
				}
				else
				{
					obj.SetValue (property, data);
				}
			}
			else
			{
				object data = property.ConvertFromString (MarkupExtension.Unescape (value), this);
				obj.SetValue (property, data);
			}
		}

		private static void RestoreCollection<T>(DependencyObject obj, string field, DependencyProperty property, object data)
		{
			ICollection<T> collection = obj.GetValue (property) as ICollection<T>;
			IEnumerable<T> dataSource = data as IEnumerable<T>;

			if (collection == null)
			{
				throw new System.ArgumentException (string.Format ("Property {0} does not follow Collection semantics", field));
			}
			if (dataSource == null)
			{
				throw new System.ArgumentException (string.Format ("Property {0} cannot be restored", field));
			}

			collection.Clear ();

			foreach (T item in dataSource)
			{
				collection.Add (item);
			}
		}

		private static void RestoreEnumerable(DependencyObject obj, string field, DependencyProperty property, object data)
		{
			System.Type                    collectionType;
			object                         collection = obj.GetValue (property);
			ISerializationConverter        converter  = Context.GetActiveContext ().FindConverterForCollection (property.PropertyType, out collectionType);
			System.Collections.IEnumerable dataSource = data as System.Collections.IEnumerable;

			if (collectionType == null)
			{
				throw new System.ArgumentException (string.Format ("Property {0} does not follow Collection semantics", field));
			}
			if (dataSource == null)
			{
				throw new System.ArgumentException (string.Format ("Property {0} cannot be restored", field));
			}

			collectionType.InvokeMember ("Clear", System.Reflection.BindingFlags.InvokeMethod, null, collection, new object[0]);

			foreach (object item in dataSource)
			{
				collectionType.InvokeMember ("Add", System.Reflection.BindingFlags.InvokeMethod, null, collection, new object[] { item });
			}
		}
	}
}
