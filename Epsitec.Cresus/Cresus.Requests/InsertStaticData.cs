//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe InsertStaticData définit une insertion de données statiques
	/// (pré-calculées) dans une table.
	/// </summary>
	
	[System.Serializable]
	
	public class InsertStaticData : AbstractData, System.Runtime.Serialization.ISerializable
	{
		public InsertStaticData() : base (RequestType.InsertStaticData)
		{
		}
		
		public InsertStaticData(System.Data.DataRow row) : this ()
		{
			this.DefineRow (row);
		}
		
		
		public override string[]				ColumnNames
		{
			get
			{
				return this.col_names;
			}
		}
		
		public override object[]				ColumnValues
		{
			get
			{
				return this.col_values;
			}
		}
		
		
		public void DefineRow(System.Data.DataRow row)
		{
			System.Data.DataTable            table   = row.Table;
			System.Data.DataColumnCollection columns = table.Columns;
			
			int n = columns.Count;
			
			this.DefineTableName (table.TableName);
			
			this.col_names  = new string[n];
			this.col_values = row.ItemArray;
			
			for (int i = 0; i < n; i++)
			{
				this.col_names[i] = columns[i].ColumnName;
			}
		}
		
		
		#region ISerializable Members
		protected InsertStaticData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			this.SetupRequestType (RequestType.InsertStaticData);
			
			this.col_names  = info.GetValue ("ColNames", typeof (string[])) as string[];
			this.col_values = info.GetValue ("ColValues", typeof (object[])) as object[];
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
			
			info.AddValue ("ColNames", this.col_names);
			info.AddValue ("ColValues", this.col_values);
		}
		#endregion
		
		private string[]						col_names;
		private object[]						col_values;
	}
}
