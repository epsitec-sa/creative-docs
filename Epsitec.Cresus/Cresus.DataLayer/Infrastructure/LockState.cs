//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	
	
	/// <summary>
	/// The <c>LockState</c> describes the state of a <see cref="LockTransaction"/> object.
	/// </summary>
	public enum LockState
	{
		
		
		/// <summary>
		/// Lock is not active. It has never been acquired.
		/// </summary>
		Idle,


		/// <summary>
		/// The lock is active (successfully acquired).
		/// </summary>
		Locked,
		

		/// <summary>
		/// The lock is no longer active.
		/// </summary>
		Disposed,


	}


}
