//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using System.Runtime.Serialization;


namespace Epsitec.Cresus.Requests
{
	
	
	/// <summary>
	/// The <c>AbstractRequest</c> class is the base class for all requests.
	/// </summary>
	[System.Serializable]
	public abstract class AbstractRequest : ISerializable
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


		protected AbstractRequest(SerializationInfo info, StreamingContext context)
		{
		}


		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}
		

		#endregion
	
	
	}


}
