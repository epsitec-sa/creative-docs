//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public class DataTableBroker : IStructuredType, ICollectionType, System.Collections.IList
	{
		public DataTableBroker(IStructuredType structuredType, System.Data.DataTable dataTable)
		{
			this.exclusion = new object ();
			this.structuredType = structuredType;
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
			long id = rowId;

			lock (this.exclusion)
			{
				DataBrokerItem item;
				DataBrokerRecord data;
				
				if (this.items.TryGetValue (id, out item))
				{
					data = item.Data;

					if (data != null)
					{
						return data;
					}
				}

				data = this.CreateRecordFromRow (id);

				if (data != null)
				{
					this.items[id] = DataBrokerItem.CreateReadOnlyItem (data);
				}

				return data;
			}
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
				object fieldData = dataRow[fieldId];

				if (fieldData == System.DBNull.Value)
				{
					fieldData = null;
				}

				data.SetValue (fieldId, fieldData);
			}

			data.Id = id;

			return data;
		}

		internal void NotifyRecordChanged(DataBrokerRecord data)
		{
			long id = data.Id;
			
			lock (this.exclusion)
			{
				this.items[id] = DataBrokerItem.CreateWritableItem (data);
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

		private object exclusion;
		private System.Data.DataTable dataTable;
		private IStructuredType structuredType;
		private Dictionary<long, DataBrokerItem> items;

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
				throw new System.Exception ("The method or operation is not implemented.");
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
	}
}
