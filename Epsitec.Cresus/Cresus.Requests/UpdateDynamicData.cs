//	Copyright � 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe UpdateDynamicData d�finit une mise � jour dynamique (par
	/// calcul) d'une ligne dans une table.
	/// </summary>
	
	[System.Serializable]
	
	public class UpdateDynamicData : AbstractData, System.Runtime.Serialization.ISerializable
	{
		public UpdateDynamicData() : base (RequestType.UpdateDynamicData)
		{
		}
		
		
		public override string[]				ColumnNames
		{
			get
			{
				return new string[0];
			}
		}
		
		public override object[]				ColumnValues
		{
			get
			{
				return new object[0];
			}
		}
		
		
		public override void Execute(ExecutionEngine engine)
		{
			//	TODO: � compl�ter...
			
			//	TODO: Ex�cute...
		}
		
		
		#region ISerializable Members
		protected UpdateDynamicData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			this.SetupRequestType (RequestType.UpdateDynamicData);
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
