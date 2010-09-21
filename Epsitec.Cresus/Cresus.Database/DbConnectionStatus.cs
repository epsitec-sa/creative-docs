namespace Epsitec.Cresus.Database
{


	/// <summary>
	/// The <c>DbConnectionStatus</c> enum describes the different states in which a connection as
	/// defined by <see cref="DbConnectionManager"/> can have.
	/// </summary>
	public enum DbConnectionStatus : int
	{


		/// <summary>
		/// The connection is active.
		/// </summary>
		Open = 0,


		/// <summary>
		/// The connection has been closed properly and thus is inactive.
		/// </summary>
		Closed = 1,


		/// <summary>
		/// The connection has been interrupted after some timeout and thus is inactive.
		/// </summary>
		Interrupted = 2,


	}


}
