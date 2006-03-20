//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Serialization.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public class SerializerContext : Context
	{
		public SerializerContext(IO.AbstractWriter writer)
		{
			this.writer = writer;
		}
		
		public override void StoreObjectData(int id, DependencyObject obj)
		{
			//	Dump the contents of the object to the writer. This will record
			//	all defined read/write fields and binding settings.
			
			this.AssertWritable ();

			this.writer.BeginObject (id, obj);

			this.StoreObjectBindings (obj);
			this.StoreObjectFields (obj);
			
			this.writer.EndObject (id, obj);
		}

		private void StoreObjectBindings(DependencyObject obj)
		{
			foreach (KeyValuePair<DependencyProperty, Binding> entry in obj.GetAllBindings ())
			{
				string markup = MarkupExtension.BindingToString (this, entry.Value);

				this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Key), markup);
			}
		}
		private void StoreObjectFields(DependencyObject obj)
		{
			foreach (LocalValueEntry entry in obj.LocalValueEntries)
			{
				DependencyPropertyMetadata metadata = entry.Property.GetMetadata (obj);

				if ((entry.Property.IsReadWrite) ||
					(metadata.CanSerializeReadOnly))
				{
					if (obj.GetBinding (entry.Property) == null)
					{
						if (this.ExternalMap.IsValueDefined (entry.Value))
						{
							//	This is an external reference. Record it as {External xxx}.

							string markup = MarkupExtension.ExtRefToString (entry.Value, this);

							this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), markup);
							continue;
						}

						DependencyObject dependencyObjectValue = entry.Value as DependencyObject;

						if (dependencyObjectValue != null)
						{
							//	This is an internal reference. Record it as {Object _nnn}.

							string markup = MarkupExtension.ObjRefToString (dependencyObjectValue, this);

							this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), markup);
							continue;
						}

						ICollection<DependencyObject> dependencyObjectCollection = entry.Value as ICollection<DependencyObject>;

						if (dependencyObjectCollection != null)
						{
							//	This is a collection. Record it as {Collection xxx, xxx, xxx}

							string markup = MarkupExtension.CollectionToString (dependencyObjectCollection, this);

							this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), markup);
							continue;
						}

						string value = entry.Property.ConvertToString (entry.Value, this);

						if (value != null)
						{
							this.writer.WriteObjectFieldValue (obj, this.GetPropertyName (entry.Property), MarkupExtension.Escape (value));
							continue;
						}
					}
				}
			}
		}
	}
}
