//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Requests
{
	/// <summary>
	/// La classe InsertStaticData définit une insertion de données statiques
	/// (pré-calculées) dans une table.
	/// </summary>
	
	[System.Serializable]
	
	public class InsertStaticData : AbstractData, System.Runtime.Serialization.ISerializable
	{
		public InsertStaticData() : base (Type.InsertStaticData)
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
		protected InsertStaticData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
			this.SetupType (Type.InsertStaticData);
		}
		
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData (info, context);
		}
		#endregion
	}
}
