//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe UpdateDynamicData définit une mise à jour dynamique (par
	/// calcul) d'une ligne dans une table.
	/// </summary>
	
	[System.Serializable]
	
	public class UpdateDynamicDataRequest : AbstractDataRequest, System.Runtime.Serialization.ISerializable
	{
		public UpdateDynamicDataRequest() : base (RequestType.UpdateDynamicData)
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
			//	TODO: à compléter...
			
			//	TODO: Exécute...
		}
		
		
		#region ISerializable Members
		protected UpdateDynamicDataRequest(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
