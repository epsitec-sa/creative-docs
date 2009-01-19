//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types.Collections;
namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe UpdateDynamicData définit une mise à jour dynamique (par
	/// calcul) d'une ligne dans une table.
	/// </summary>
	
	[System.Serializable]
	
	public class UpdateDynamicDataRequest : AbstractDataRequest, System.Runtime.Serialization.ISerializable
	{
		public UpdateDynamicDataRequest()
		{
		}


		public override ReadOnlyList<string> ColumnNames
		{
			get
			{
				return ReadOnlyList<string>.Empty;
			}
		}
		
		public override ReadOnlyList<object>	ColumnValues
		{
			get
			{
				return ReadOnlyList<object>.Empty;
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
