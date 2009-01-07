//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>ProgressState</c> enumeration defines the possible states of
	/// a <see cref="ProgressInformation"/>.
	/// </summary>
	public enum ProgressState
	{
		/// <summary>
		/// The operation did not start yet.
		/// </summary>
		None,

		/// <summary>
		/// The operation is running.
		/// </summary>
		Running		= 1,

		/// <summary>
		/// The operation is no longer running; it finished successfully.
		/// </summary>
		Succeeded	= 2,
		
		/// <summary>
		/// The operation is no longer running; it was cancelled.
		/// </summary>
		Cancelled	= 3,
		
		/// <summary>
		/// The operation is no longer running; it failed.
		/// </summary>
		Failed		= 4,
	}
}
