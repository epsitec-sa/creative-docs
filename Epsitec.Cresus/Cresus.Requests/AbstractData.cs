//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe AbstractData définit les méthodes communes aux diverses
	/// requêtes manipulant des données.
	/// </summary>
	
	[System.Serializable]
	
	public abstract class AbstractData : AbstractRequest, System.Runtime.Serialization.ISerializable
	{
		public AbstractData(RequestType type) : base (type)
		{
		}
		
		
		public string							TableName
		{
			get
			{
				return this.table_name;
			}
		}
		
		public abstract string[]				ColumnNames
		{
			get;
		}
		
		public abstract object[]				ColumnValues
		{
			get;
		}
		
		
		public void DefineTableName(string table_name)
		{
			this.table_name = table_name;
		}
		
		
		#region ISerializable Members
		protected AbstractData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			this.table_name = info.GetValue ("TableName", typeof (string)) as string;
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
			
			info.AddValue ("TableName", this.table_name);
		}
		#endregion
		
		private string							table_name;
	}
}
