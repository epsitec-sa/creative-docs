//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Requests
{
	/// <summary>
	/// La classe UpdateDynamicData définit une mise à jour dynamique (par
	/// calcul) d'une ligne dans une table.
	/// </summary>
	
	[System.Serializable]
	
	public class UpdateDynamicData : AbstractData, System.Runtime.Serialization.ISerializable
	{
		public UpdateDynamicData() : base (Type.UpdateDynamicData)
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
		
		
		#region ISerializable Members
		protected UpdateDynamicData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			this.SetupType (Type.UpdateDynamicData);
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
