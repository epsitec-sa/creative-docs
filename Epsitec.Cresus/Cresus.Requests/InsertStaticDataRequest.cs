//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe InsertStaticData d�finit une insertion de donn�es statiques
	/// (pr�-calcul�es) dans une table.
	/// </summary>
	
	[System.Serializable]
	
	public class InsertStaticDataRequest : AbstractDataRequest, System.Runtime.Serialization.ISerializable
	{
		public InsertStaticDataRequest() : base (RequestType.InsertStaticData)
		{
		}
		
		public InsertStaticDataRequest(System.Data.DataRow row) : this ()
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
		
		
		public override void Execute(ExecutionEngine engine)
		{
			engine.GenerateInsertDataCommand (this.TableName, this.col_names, this.col_values);
		}
		
		
		#region ISerializable Members
		protected InsertStaticDataRequest(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
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
