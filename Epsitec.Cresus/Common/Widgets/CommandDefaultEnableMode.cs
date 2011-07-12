//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandDefaultEnableMode</c> enumeration defines how commands should be
	/// enabled by default (when nothing is specified in any <see cref="CommandContext"/>
	/// in the active command context chain.
	/// </summary>
	public enum CommandDefaultEnableMode
	{
		/// <summary>
		/// Undefined mode (will default to enabled by default).
		/// </summary>
		None,

		/// <summary>
		/// The command will be disabled by default.
		/// </summary>
		Disabled,

		/// <summary>
		/// The command will be enabled by default.
		/// </summary>
		Enabled,
	}
}
