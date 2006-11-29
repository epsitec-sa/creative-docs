//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (DataTableBroker))]

namespace Epsitec.Cresus.DataLayer
{
	public class DataTableBroker : IStructuredType, ICollectionType
	{
		public DataTableBroker(IStructuredType structuredType, System.Data.DataTable dataTable)
		{
			this.exclusion = new object ();
			this.structuredType = structuredType;
			this.dataTable = dataTable;
			this.items = new Dictionary<long, DataBrokerItem> ();
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

				data = this.CreateRecordFromRow (rowId);

				if (data != null)
				{
					this.items[id] = DataBrokerItem.CreateReadOnlyItem (data);
				}

				return data;
			}
		}

		private DataBrokerRecord CreateRecordFromRow(DbId rowId)
		{
			System.Data.DataRow dataRow = this.dataTable.Rows.Find (rowId.Value);

			if (dataRow == null)
			{
				return null;
			}
			
			DataBrokerRecord data = new DataBrokerRecord (this);

			foreach (string fieldId in this.structuredType.GetFieldIds ())
			{
				data.SetValue (fieldId, dataRow[fieldId]);
			}

			data.Id = rowId;

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
	}
}
