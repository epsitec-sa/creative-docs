//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public class DataTableBroker : IStructuredType, ICollectionType, System.Collections.IList, INotifyCollectionChanged
	{
		public DataTableBroker(IStructuredType structuredType, DbTable tableDefinition, System.Data.DataTable dataTable)
		{
			this.structuredType = structuredType;
			this.tableDefinition = tableDefinition;
			this.dataTable = dataTable;
			this.items = new Dictionary<long, DataBrokerItem> ();
		}

		public int Count
		{
			get
			{
				lock (this.dataTable)
				{
					return this.dataTable.Rows.Count;
				}
			}
		}

		public System.Data.DataTable DataTable
		{
			get
			{
				return this.dataTable;
			}
		}

		public DbTable TableDefinition
		{
			get
			{
				return this.tableDefinition;
			}
		}

		public IStructuredType StructuredType
		{
			get
			{
				return this.structuredType;
			}
		}

		public DataBroker ParentBroker
		{
			get
			{
				return this.parentBroker;
			}
			internal set
			{
				this.parentBroker = value;
			}
		}

		public IEnumerable<DataBrokerRecord> Records
		{
			get
			{
				int index = 0;

				while (true)
				{
					System.Data.DataRow row;

					lock (this.dataTable)
					{
						if (index < this.dataTable.Rows.Count)
						{
							row = this.dataTable.Rows[index++];
						}
						else
						{
							row = null;
						}
					}

					if (row == null)
					{
						yield break;
					}
					else
					{
						DataBrokerRecord data = null;
						
						long id = (long) row[0];
						
						lock (this.exclusion)
						{
							DataBrokerItem item;
							
							if (this.items.TryGetValue (id, out item))
							{
								data = item.Data;
							}

							if (data == null)
							{
								data = this.CreateRecordFromRow (id);

								if (data != null)
								{
									this.items[id] = DataBrokerItem.CreateReadOnlyItem (data);
								}
							}
						}

						if (data != null)
						{
							yield return data;
						}
					}
				}
			}
		}

		public DataBrokerRecord GetRow(DbId rowId)
		{
			return this.GetRecordFromRowId (rowId);
		}

		public DataBrokerRecord GetRowFromIndex(int index)
		{
			System.Data.DataRow row;

			lock (this.dataTable)
			{
				if ((index >= 0) &&
					(index < this.dataTable.Rows.Count))
				{
					row = this.dataTable.Rows[index];
				}
				else
				{
					return null;
				}
			}

			return this.GetRecordFromRowId ((long) row[0]);
		}

		private DataBrokerRecord GetRecordFromRowId(long id)
		{
			DataBrokerRecord data = null;
			
			lock (this.exclusion)
			{
				DataBrokerItem item;

				if (this.items.TryGetValue (id, out item))
				{
					data = item.Data;
				}

				if (data == null)
				{
					data = this.CreateRecordFromRow (id);

					if (data != null)
					{
						this.items[id] = DataBrokerItem.CreateReadOnlyItem (data);
					}
				}
			}
			
			return data;
		}

		public bool Contains(DataBrokerRecord data)
		{
			if (data == null)
			{
				return false;
			}

			if (data.Broker == this)
			{
				DataBrokerItem item;

				lock (this.exclusion)
				{
					if (this.items.TryGetValue (data.Id, out item))
					{
						return item.Data == data;
					}
				}
			}

			return false;
		}

		public int IndexOf(DataBrokerRecord data)
		{
			if (this.Contains (data))
			{
				lock (this.dataTable)
				{
					long id = data.Id;
					int index = 0;

					foreach (System.Data.DataRow row in this.dataTable.Rows)
					{
						if ((long) row[0] == id)
						{
							return index;
						}

						index++;
					}
				}
			}

			return -1;
		}

		public int CopyChangesToDataTable()
		{
			int changes = 0;
			
			List<DataBrokerItem> modifiedList = new List<DataBrokerItem> ();
			List<DataBrokerItem> deadList = new List<DataBrokerItem> ();
			
			lock (this.exclusion)
			{
				foreach (DataBrokerItem item in this.items.Values)
				{
					if (item.IsWritable)
					{
						modifiedList.Add (item);
					}
				}
			}

			lock (this.dataTable)
			{
				System.Data.DataRow dataRow;
				
				foreach (DataBrokerItem item in modifiedList)
				{
					dataRow = this.dataTable.Rows.Find (item.Id);

					if (dataRow == null)
					{
						deadList.Add (item);
					}
					else
					{
						if (this.CopyDataToRow (item.Data, dataRow))
						{
							changes++;
						}
					}
				}
			}

			lock (this.exclusion)
			{
				foreach (DataBrokerItem item in modifiedList)
				{
					this.items[item.Id] = DataBrokerItem.CreateReadOnlyItem (item.Data);
				}
				foreach (DataBrokerItem item in deadList)
				{
					this.items.Remove (item.Id);
				}
			}
			
			return changes;
		}

		private bool CopyDataToRow(StructuredData data, System.Data.DataRow dataRow)
		{
			bool modified = false;

			dataRow.BeginEdit ();

			foreach (string fieldId in this.structuredType.GetFieldIds ())
			{
				object fieldData = data.GetValue (fieldId);

				if ((UndefinedValue.IsUndefinedValue (fieldData)) ||
					(fieldData == null))
				{
					fieldData = System.DBNull.Value;
				}

				if (dataRow[fieldId] != fieldData)
				{
					dataRow[fieldId] = fieldData;
					modified = true;
				}
			}

			dataRow.EndEdit ();
			
			return modified;
		}

		public DataBrokerRecord AddRow()
		{
			if (this.parentBroker == null)
			{
				throw new System.InvalidOperationException ("Cannot add row, there is no parent broker");
			}

			System.Data.DataRow row;
			int rowIndex;

			lock (this.dataTable)
			{
				row = this.parentBroker.RichCommand.CreateRow (this.dataTable.TableName);
				rowIndex = this.dataTable.Rows.Count-1;
			}

			DataBrokerRecord record = this.GetRecordFromRowId ((long) row[0]);
			record.FillWithDefaultValues ();

			this.CopyDataToRow (record, row);
			this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, rowIndex, new object[] { record }));

			return record;
		}

		internal void Refresh()
		{
			lock (this.exclusion)
			{
				this.items.Clear ();
			}
			
			this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Reset));
		}
		
		
		private DataBrokerRecord CreateRecordFromRow(long id)
		{
			System.Data.DataRow dataRow;

			lock (this.dataTable)
			{
				dataRow = this.dataTable.Rows.Find (id);
			}

			if (dataRow == null)
			{
				return null;
			}
			
			DataBrokerRecord data = new DataBrokerRecord (this);

			foreach (string fieldId in this.structuredType.GetFieldIds ())
			{
				StructuredTypeField field = this.structuredType.GetField (fieldId);

				switch (field.Relation)
				{
					case Relation.Reference:
						break;

					case Relation.Collection:
						this.SetValueFromRowCollection (dataRow, data, id, field, fieldId);
						break;
					
					case Relation.None:
						this.SetValueFromRowValue (dataRow, data, fieldId);
						break;

					default:
						throw new System.NotSupportedException (string.Format ("Unsupported relation {0} for field {1} in table {2}", field.Relation, fieldId, this.dataTable.TableName));
				}
			}

			data.Id = id;

			return data;
		}

		private void SetValueFromRowCollection(System.Data.DataRow dataRow, DataBrokerRecord data, long id, StructuredTypeField field, string fieldId)
		{
			//	TODO: handle collection

			throw new System.NotImplementedException ();
		}

		private void SetValueFromRowValue(System.Data.DataRow dataRow, DataBrokerRecord data, string fieldId)
		{
			if (this.dataTable.Columns.Contains (fieldId))
			{
				StructuredTypeField field = data.StructuredType.GetField (fieldId);
				AbstractType fieldType = field.Type as AbstractType;

				object fieldData = dataRow[fieldId];

				if (fieldData == System.DBNull.Value)
				{
					fieldData = null;
				}

				if ((fieldType != null) &&
					(fieldType.IsValidValue (fieldData) == false))
				{
					fieldData = fieldType.DefaultValue;
				}

				data.SetValue (fieldId, fieldData);
			}
			else
			{
				//	Could not find the specified column in the table.

				throw new System.ArgumentException (string.Format ("Cannot map field {0} in table {1}", fieldId, this.dataTable.TableName));
			}
		}

		internal void NotifyRecordChanged(DataBrokerRecord data)
		{
			long id = data.Id;
			
			lock (this.exclusion)
			{
				DataBrokerItem item;

				if (this.items.TryGetValue (id, out item))
				{
					if (item.IsWritable)
					{
						return;
					}
				}

				this.items[id] = DataBrokerItem.CreateWritableItem (data);
			}
		}

		private void OnCollectionChanged(CollectionChangedEventArgs e)
		{
			EventHandler<CollectionChangedEventArgs> handler;

			lock (this.exclusion)
			{
				handler = this.collectionChangedEvent;
			}
			
			if (handler != null)
			{
				handler (this, e);
			}
		}

		#region IStructuredType Members

		IEnumerable<string> IStructuredType.GetFieldIds()
		{
			return this.structuredType.GetFieldIds ();
		}

		StructuredTypeField IStructuredType.GetField(string fieldId)
		{
			return this.structuredType.GetField (fieldId);
		}

		#endregion

		#region ICollectionType Members

		INamedType ICollectionType.ItemType
		{
			get
			{
				return this.structuredType as INamedType;
			}
		}

		#endregion

		#region IList Members

		int System.Collections.IList.Add(object value)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		void System.Collections.IList.Clear()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		bool System.Collections.IList.Contains(object value)
		{
			return this.Contains (value as DataBrokerRecord);
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return this.IndexOf (value as DataBrokerRecord);
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void System.Collections.IList.Remove(object value)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		void System.Collections.IList.RemoveAt(int index)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this.GetRowFromIndex (index);
			}
			set
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(System.Array array, int index)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		int System.Collections.ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				throw new System.Exception ("The method or operation is not implemented.");
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Records.GetEnumerator ();
		}

		#endregion

		#region INotifyCollectionChanged Members

		public event EventHandler<CollectionChangedEventArgs> CollectionChanged
		{
			add
			{
				lock (this.exclusion)
				{
					this.collectionChangedEvent += value;
				}
			}
			remove
			{
				lock (this.exclusion)
				{
					this.collectionChangedEvent -= value;
				}
			}
		}


		#endregion

		private readonly object exclusion = new object ();
		private event EventHandler<CollectionChangedEventArgs> collectionChangedEvent;
		private DbTable tableDefinition;
		private System.Data.DataTable dataTable;
		private IStructuredType structuredType;
		private Dictionary<long, DataBrokerItem> items;
		private DataBroker parentBroker;
	}
}
