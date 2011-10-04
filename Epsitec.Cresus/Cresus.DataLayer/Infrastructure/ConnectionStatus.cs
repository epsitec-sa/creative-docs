//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	/// <summary>
	/// The <c>ConnectionStatus</c> enumeration defines the different states that a connection
	/// (as defined by class <see cref="Connection"/>) can have.
	/// </summary>
	public enum ConnectionStatus
	{
		/// <summary>
		/// The connection is open and thus active.
		/// </summary>
		Open = 0,
		
		/// <summary>
		/// The connection has been properly closed and is thus inactive.
		/// </summary>
		Closed = 1,
		
		/// <summary>
		/// The connection has been interrupted after a time out and is thus inactive.
		/// </summary>
		Interrupted = 2,
	}
}
