//	Copyright © 2006-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DataSource</c> class provides the list of named objects
	/// to which a user interface can be data bound.
	/// </summary>
	public class DataSource : IStructuredType, IStructuredData, IStructuredTypeProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataSource"/> class.
		/// </summary>
		public DataSource()
		{
		}


		/// <summary>
		/// Gets or sets the metadata associated with the data source.
		/// </summary>
		/// <value>The metadata.</value>
		public DataSourceMetadata Metadata
		{
			get
			{
				return this.metadata;
			}
			private set
			{
				this.metadata = value;
			}
		}

		
		/// <summary>
		/// Adds a named data source.
		/// </summary>
		/// <param name="id">The identifier of the data source.</param>
		/// <param name="source">The data source.</param>
		public void AddDataSource(string id, IStructuredData source)
		{
			this.AddDataSource (id, source, Druid.Empty);
		}

		/// <summary>
		/// Adds a named data source.
		/// </summary>
		/// <param name="id">The identifier of the data source.</param>
		/// <param name="source">The data source.</param>
		/// <param name="captionId">The caption DRUID for the data source.</param>
		public void AddDataSource(string id, IStructuredData source, Druid captionId)
		{
			if (this.Contains (id))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot insert duplicate data source with identifier '{0}'", id));
			}
			
			this.items.Add (new ItemRecord (id, source, captionId, null));
		}

		/// <summary>
		/// Sets a named data source. This will replace an existing data source
		/// if there was already one with the specified id.
		/// </summary>
		/// <param name="id">The identifier of the data source.</param>
		/// <param name="source">The data source.</param>
		public void SetDataSource(string id, IStructuredData source)
		{
			int index = this.GetItemRecordIndex (id);

			if (index < 0)
			{
				this.AddDataSource (id, source);
			}
			else
			{
				ItemRecord model = this.items[index];
				this.items[index] = new ItemRecord (id, source, model.CaptionId, model.Handlers);

				if (model.Handlers != null)
				{
					model.Handlers (this, new DependencyPropertyChangedEventArgs (id, model.Data, source));
				}
			}
		}

		/// <summary>
		/// Determines whether the collection contains the named datasource.
		/// </summary>
		/// <param name="id">The identifier of the datasource to get.</param>
		/// <returns>
		/// 	<c>true</c> if the collection contains the specified named datasource; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(string id)
		{
			ItemRecord record = this.GetItemRecord (id);

			return (record.IsEmpty) ? false : true;
		}

		/// <summary>
		/// Gets the data source for the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the data source.</param>
		/// <returns>The data source or <c>null</c> if it is not known.</returns>
		public IStructuredData GetDataSource(string id)
		{
			ItemRecord record = this.GetItemRecord (id);

			if (record.IsEmpty)
			{
				return null;
			}
			else
			{
				return record.Data;
			}
		}


		/// <summary>
		/// Sets the data source metadata. Do not use directly.
		/// </summary>
		/// <param name="metadata">The metadata.</param>
		public void SetDataSourceMetadata(DataSourceMetadata metadata)
		{
			this.Metadata = metadata;
		}


		#region IStructuredType Members

		/// <summary>
		/// Gets the field descriptor for the specified field identifier.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <returns>
		/// The matching field descriptor; otherwise, <c>null</c>.
		/// </returns>
		StructuredTypeField IStructuredType.GetField(string fieldId)
		{
			ItemRecord record = this.GetItemRecord (fieldId);
			IStructuredData data = record.Data;
			object typeObject = TypeRosetta.GetTypeObjectFromValue (data);
			INamedType namedType = TypeRosetta.GetNamedTypeFromTypeObject (typeObject);

			return new StructuredTypeField (fieldId, namedType, record.CaptionId);
		}

		/// <summary>
		/// Gets a collection of field identifiers.
		/// </summary>
		/// <returns>A collection of field identifiers.</returns>
		public IEnumerable<string> GetFieldIds()
		{
			string[] ids = new string[this.items.Count];

			for (int i = 0; i < this.items.Count; i++)
			{
				ids[i] = this.items[i].FieldId;
			}

			System.Array.Sort (ids);
			
			return ids;
		}

		StructuredTypeClass IStructuredType.GetClass()
		{
			return StructuredTypeClass.None;
		}

		#endregion

		#region IStructuredData Members

		public void AttachListener(string id, EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			int index = this.GetItemRecordIndex (id);
			
			if (index < 0)
			{
				throw new System.ArgumentException (string.Format ("Identifier '{0}' cannot be resolved", id));
			}

			ItemRecord model = this.items[index];

			EventHandler<DependencyPropertyChangedEventArgs> handlers = model.Handlers;
			handlers = (EventHandler<DependencyPropertyChangedEventArgs>) System.Delegate.Combine (handlers, handler);
			
			this.items[index] = new ItemRecord (model.FieldId, model.Data, model.CaptionId, handlers);
		}

		public void DetachListener(string id, EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			int index = this.GetItemRecordIndex (id);

			if (index < 0)
			{
				throw new System.ArgumentException (string.Format ("Identifier '{0}' cannot be resolved", id));
			}

			ItemRecord model = this.items[index];

			EventHandler<DependencyPropertyChangedEventArgs> handlers = model.Handlers;
			handlers = (EventHandler<DependencyPropertyChangedEventArgs>) System.Delegate.Remove (handlers, handler);
			
			this.items[index] = new ItemRecord (model.FieldId, model.Data, model.CaptionId, handlers);
		}

		IEnumerable<string> IStructuredData.GetValueIds()
		{
			return this.GetFieldIds ();
		}

		void IStructuredData.SetValue(string id, object value)
		{
			IValueStore store = this;
			store.SetValue (id, value, ValueStoreSetMode.Default);
		}

		object IValueStore.GetValue(string id)
		{
			ItemRecord record = this.GetItemRecord (id);
			
			if (record.IsEmpty)
			{
				return UnknownValue.Value;
			}
			else
			{
				return record.Data;
			}
		}

		void IValueStore.SetValue(string id, object value, ValueStoreSetMode mode)
		{
			throw new System.InvalidOperationException ("You cannot modify a DataSource with SetValue");
		}

		#endregion

		#region IStructuredTypeProvider Members

		public IStructuredType GetStructuredType()
		{
			if (this.metadata == null)
			{
				return this;
			}
			else
			{
				return this.metadata;
			}
		}

		#endregion

		/// <summary>
		/// Gets the item record for the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the item record to get.</param>
		/// <returns>The item record for the identifier.</returns>
		protected ItemRecord GetItemRecord(string id)
		{
			int index = this.GetItemRecordIndex (id);

			return index < 0 ? ItemRecord.Empty : this.items[index];
		}

		protected int GetItemRecordIndex(string id)
		{
			int index = 0;

			foreach (ItemRecord record in this.items)
			{
				if (record.FieldId == id)
				{
					return index;
				}

				index++;
			}

			return -1;
		}

		#region ItemRecord Structure

		/// <summary>
		/// The <c>ItemRecord</c> structure stores a field identifier and a structured data.
		/// </summary>
		protected struct ItemRecord
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ItemRecord"/> structure.
			/// </summary>
			/// <param name="id">The field identifier of the item.</param>
			/// <param name="data">The data of the item.</param>
			/// <param name="captionId">The caption id.</param>
			/// <param name="handlers">The event handlers.</param>
			public ItemRecord(string id, IStructuredData data, Druid captionId, EventHandler<DependencyPropertyChangedEventArgs> handlers)
			{
				this.data = data;
				this.fieldId = id;
				this.captionId = captionId;
				this.handlers = handlers;
			}

			/// <summary>
			/// Gets the datasource.
			/// </summary>
			/// <value>The datasource.</value>
			public IStructuredData				Data
			{
				get
				{
					return this.data;
				}
			}

			/// <summary>
			/// Gets the field identifier of the datasource.
			/// </summary>
			/// <value>The field identifier of the datasource.</value>
			public string						FieldId
			{
				get
				{
					return this.fieldId;
				}
			}

			/// <summary>
			/// Gets or sets the caption DRUID of the datasource.
			/// </summary>
			/// <value>The caption DRUID of the datasource.</value>
			public Druid						CaptionId
			{
				get
				{
					return this.captionId;
				}
			}

			/// <summary>
			/// Gets a value indicating whether this record is empty.
			/// </summary>
			/// <value><c>true</c> if this record is empty; otherwise, <c>false</c>.</value>
			public bool							IsEmpty
			{
				get
				{
					return (this.data == null) && (this.fieldId == null);
				}
			}

			public EventHandler<DependencyPropertyChangedEventArgs> Handlers
			{
				get
				{
					return this.handlers;
				}
			}
			
			/// <summary>
			/// Empty item record.
			/// </summary>
			public static readonly ItemRecord Empty = new ItemRecord ();

			private readonly IStructuredData	data;
			private readonly string				fieldId;
			private readonly Druid				captionId;
			private readonly EventHandler<DependencyPropertyChangedEventArgs> handlers;
		}
		
		#endregion


		public static readonly string DataName = "Data";
		
		private readonly List<ItemRecord> items = new List<ItemRecord> ();
		private DataSourceMetadata metadata;
	}
}
