//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>DataSource</c> class provides the list of named objects
	/// to which a user interface can be data bound.
	/// </summary>
	public class DataSource : IStructuredType, IStructuredData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataSource"/> class.
		/// </summary>
		public DataSource()
		{
		}

		/// <summary>
		/// Adds a named data source.
		/// </summary>
		/// <param name="name">The name of the data source.</param>
		/// <param name="source">The data source.</param>
		public void AddDataSource(string name, IStructuredData source)
		{
			this.AddDataSource (name, source, Druid.Empty);
		}

		/// <summary>
		/// Adds a named data source.
		/// </summary>
		/// <param name="name">The name of the data source.</param>
		/// <param name="source">The data source.</param>
		/// <param name="captionId">The caption DRUID for the data source.</param>
		public void AddDataSource(string name, IStructuredData source, Druid captionId)
		{
			if (this.Contains (name))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot insert duplicate data source with name '{0}'", name));
			}
			
			this.items.Add (new ItemRecord (name, source, captionId));
		}

		/// <summary>
		/// Determines whether the collection contains the named datasource.
		/// </summary>
		/// <param name="name">The name of the datasource to get.</param>
		/// <returns>
		/// 	<c>true</c> if the collection contains the specified named datasource; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(string name)
		{
			ItemRecord record = this.GetItemRecord (name);

			return (record.IsEmpty) ? false : true;
		}

		/// <summary>
		/// Gets the data source for the specified name.
		/// </summary>
		/// <param name="name">The name of the data source.</param>
		/// <returns>The data source or <c>null</c> if it is not known.</returns>
		public IStructuredData GetDataSource(string name)
		{
			ItemRecord record = this.GetItemRecord (name);

			if (record.IsEmpty)
			{
				return null;
			}
			else
			{
				return record.Data;
			}
		}

		#region IStructuredType Members

		StructuredTypeField IStructuredType.GetField(string fieldId)
		{
			ItemRecord record = this.GetItemRecord (fieldId);
			IStructuredData data = record.Data;
			object typeObject = TypeRosetta.GetTypeObjectFromValue (data);
			INamedType namedType = TypeRosetta.GetNamedTypeFromTypeObject (typeObject);

			return new StructuredTypeField (fieldId, namedType, record.CaptionId);
		}

		/// <summary>
		/// Gets the field names.
		/// </summary>
		/// <returns>Enumeration of field names.</returns>
		public IEnumerable<string> GetFieldIds()
		{
			string[] names = new string[this.items.Count];

			for (int i = 0; i < this.items.Count; i++)
			{
				names[i] = this.items[i].Name;
			}

			System.Array.Sort (names);
			
			return names;
		}


		#endregion

		#region IStructuredData Members

		public void AttachListener(string name, EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			ItemRecord record = this.GetItemRecord (name);
			IStructuredData data = record.Data;

			if (data == null)
			{
				throw new System.ArgumentException (string.Format ("Name '{0}' cannot be resolved", name));
			}
			
			//	Immutable roots...
		}

		public void DetachListener(string name, EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			ItemRecord record = this.GetItemRecord (name);
			IStructuredData data = record.Data;

			if (data == null)
			{
				throw new System.ArgumentException (string.Format ("Name '{0}' cannot be resolved", name));
			}
			
			//	Immutable roots...
		}

		IEnumerable<string> IStructuredData.GetValueNames()
		{
			return this.GetFieldIds ();
		}
		
		object IStructuredData.GetValue(string name)
		{
			ItemRecord record = this.GetItemRecord (name);
			IStructuredData data = record.Data;

			if (data == null)
			{
				throw new System.ArgumentException (string.Format ("Name '{0}' cannot be resolved", name));
			}
			
			return record.Data;
		}

		void IStructuredData.SetValue(string name, object value)
		{
			throw new System.InvalidOperationException ("You cannot modify a DataSource with SetValue");
		}

		#endregion

		/// <summary>
		/// Gets the item record for the specified name.
		/// </summary>
		/// <param name="name">The name of the item record to get.</param>
		/// <returns>The item record for the name.</returns>
		protected ItemRecord GetItemRecord(string name)
		{
			foreach (ItemRecord record in this.items)
			{
				if (record.Name == name)
				{
					return record;
				}
			}

			return ItemRecord.Empty;
		}

		#region ItemRecord Structure

		/// <summary>
		/// The <c>ItemRecord</c> structure stores a name and a structured data.
		/// </summary>
		protected struct ItemRecord
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ItemRecord"/> structure.
			/// </summary>
			/// <param name="name">The name of the item.</param>
			/// <param name="data">The data of the item.</param>
			public ItemRecord(string name, IStructuredData data, Druid captionId)
			{
				this.data = data;
				this.name = name;
				this.captionId = captionId;
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
			/// Gets the name of the datasource.
			/// </summary>
			/// <value>The name of the datasource.</value>
			public string						Name
			{
				get
				{
					return this.name;
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
					return this.data == null;
				}
			}
			
			/// <summary>
			/// Empty item record.
			/// </summary>
			public static readonly ItemRecord Empty = new ItemRecord ();

			private IStructuredData				data;
			private string						name;
			private Druid						captionId;
		}
		
		#endregion

		List<ItemRecord> items = new List<ItemRecord> ();
	}
}
