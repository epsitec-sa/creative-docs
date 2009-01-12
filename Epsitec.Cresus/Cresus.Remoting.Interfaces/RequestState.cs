//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>RequestState</c> structure describes the state of a request; this
	/// is only intended for the remoting layer, not for higher level consumption.
	/// </summary>
	[System.Serializable]
	
	public struct RequestState
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequestState"/> struct.
		/// </summary>
		/// <param name="id">The request id.</param>
		/// <param name="state">The request state.</param>
		public RequestState(long id, int state)
		{
			this.id    = id;
			this.state = state;
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
		/// Gets the request state.
		/// </summary>
		/// <value>The request state.</value>
		public int								State
		{
			get
			{
				return this.state;
			}
		}
		
		
		readonly long							id;
		readonly int							state;
	}
}
