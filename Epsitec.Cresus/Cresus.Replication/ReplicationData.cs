//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// La classe ReplicationData représente les données à répliquer dans
	/// un format facilement sérialisable.
	/// </summary>
	
	[System.Serializable]
	public class ReplicationData : System.Runtime.Serialization.ISerializable
	{
		public ReplicationData()
		{
			this.packed_table_array = null;
			this.packed_table_list  = null;
		}
		
		
		public PackedTableData[]				TableData
		{
			get
			{
				this.GeneratePackedTableArray ();
				return (PackedTableData[]) this.packed_table_array.Clone ();
			}
		}
		
		
		public void Add(PackedTableData table_data)
		{
			this.GeneratePackedTableList ();
			this.packed_table_list.Add (table_data);
		}
		
		
		#region ISerializable Members
		public ReplicationData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			this.packed_table_list  = null;
			this.packed_table_array = (PackedTableData[]) info.GetValue ("Tables", typeof (PackedTableData[]));
		}
		
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			this.GeneratePackedTableArray ();
			info.AddValue ("Tables", this.packed_table_array);
		}
		#endregion
		
		private void GeneratePackedTableArray()
		{
			if (this.packed_table_array == null)
			{
				int n = (this.packed_table_list == null) ? 0 : this.packed_table_list.Count;
				this.packed_table_array = new PackedTableData[n];
				
				if (n > 0)
				{
					this.packed_table_list.CopyTo (this.packed_table_array);
				}
			}
		}
		
		private void GeneratePackedTableList()
		{
			if (this.packed_table_list == null)
			{
				this.packed_table_list = new System.Collections.ArrayList ();
				
				if (this.packed_table_array != null)
				{
					this.packed_table_list.AddRange (this.packed_table_array);
					this.packed_table_array = null;
				}
			}
		}
		
		
		private PackedTableData[]				packed_table_array;
		private System.Collections.ArrayList	packed_table_list;
	}
}
