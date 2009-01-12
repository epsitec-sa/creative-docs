//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;
using System.Runtime.Serialization;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>SerializedRequest</c> structure describes a request; this is only
	/// intended for the remoting layer, not for higher level consumption.
	/// </summary>
	[System.Serializable]
	
	public struct SerializedRequest
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SerializedRequest"/> struct.
		/// </summary>
		/// <param name="id">The request id.</param>
		/// <param name="data">The serialized request data.</param>
		public SerializedRequest(long id, byte[] data)
		{
			this.id   = id;
			this.data = data;
		}


		/// <summary>
		/// Gets the request id.
		/// </summary>
		/// <value>The request id.</value>
		public long								RequestId
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets the serialized request data.
		/// </summary>
		/// <value>The serialized request data.</value>
		public byte[]							Data
		{
			get
			{
				return this.data;
			}
		}
		
		
		readonly long							id;
		readonly byte[]							data;
	}
}
