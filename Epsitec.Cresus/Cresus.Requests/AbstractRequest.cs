//	Copyright � 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// The <c>AbstractRequest</c> class is the base class for all requests.
	/// </summary>
	
	[System.Serializable]
	
	public abstract class AbstractRequest : System.Runtime.Serialization.ISerializable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractRequest"/> class.
		/// </summary>
		protected AbstractRequest()
		{
		}


		/// <summary>
		/// Executes the request using the specified execution engine.
		/// </summary>
		/// <param name="engine">The execution engine.</param>
		public abstract void Execute(ExecutionEngine engine);
		
		
		#region ISerializable Members
		
		protected AbstractRequest(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
		}
		
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
		}
		
		#endregion
	}
}
