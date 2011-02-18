//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ApplicationStartStatus</c> enumeration specifies the status
	/// of a starting application (mainly, is it the only instance running?)
	/// </summary>
	public enum ApplicationStartStatus
	{
		/// <summary>
		/// This application instance is the only one running.
		/// </summary>
		RunningAlone,

		/// <summary>
		/// This application instance has siblings already running concurrently.
		/// </summary>
		RunningConcurrently,
	}
}
