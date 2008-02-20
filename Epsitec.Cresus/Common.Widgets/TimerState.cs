//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>TimerState</c> enumeration lists all possible timer states.
	/// </summary>
	public enum TimerState
	{
		/// <summary>
		/// The timer is in an invalid state.
		/// </summary>
		Invalid,
		
		/// <summary>
		/// The timer has been disposed.
		/// </summary>
		Disposed,
		
		/// <summary>
		/// The timer does not run. Calling <see cref="Timer.Start"/> will start
		/// a new delay.
		/// </summary>
		Stopped,

		/// <summary>
		/// The timer is running.
		/// </summary>
		Running,
		
		/// <summary>
		/// The timer does not run. Calling <see cref="Timer.Start"/> will resume
		/// the previous delay.
		/// </summary>
		Suspended,
		
		/// <summary>
		/// The timer does not run. The delay has elapsed. Calling
		/// <see cref="Timer.Start"/> will start a new delay.
		/// </summary>
		Elapsed
	}
}
