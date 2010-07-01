//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using System.Collections.ObjectModel;

using System.Runtime.Serialization;


namespace Epsitec.Cresus.Requests
{
	
	
	/// <summary>
	/// The <c>UpdateDynamicDataRequest</c> class describes a dynamic data update.
	/// The data is computed on the fly.
	/// </summary>
	[System.Serializable]
	public class UpdateDynamicDataRequest : AbstractDataRequest
	{


		public UpdateDynamicDataRequest()
		{
			throw new System.NotImplementedException ();
		}


		public override ReadOnlyCollection<string> ColumnNames
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}


		public override ReadOnlyCollection<object> ColumnValues
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}
		
		
		public override void Execute(ExecutionEngine engine)
		{
			throw new System.NotImplementedException ();
		}
		
		
		#region ISerializable Members


		protected UpdateDynamicDataRequest(SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
			throw new System.NotImplementedException ();
		}


		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);

			throw new System.NotImplementedException ();
		}
		

		#endregion
	
	
	}


}
