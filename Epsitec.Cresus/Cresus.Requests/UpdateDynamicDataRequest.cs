//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// The <c>UpdateDynamicDataRequest</c> class describes a dynamic data update.
	/// The data is computed on the fly.
	/// </summary>
	
	[System.Serializable]
	
	public class UpdateDynamicDataRequest : AbstractDataRequest, System.Runtime.Serialization.ISerializable
	{
		public UpdateDynamicDataRequest()
		{
		}


		public override ReadOnlyList<string>	ColumnNames
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

			throw new System.NotImplementedException ();
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
