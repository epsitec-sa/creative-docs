//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ValueStoreSetMode</c> enumeration defines the supported modes
	/// for the <see cref="IValueStore.SetValue"/> method.
	/// </summary>
	public enum ValueStoreSetMode
	{
		/// <summary>
		/// Default set mode.
		/// </summary>
		Default,

		/// <summary>
		/// Short-circuit the set logic and simply set the value without further
		/// handling.
		/// </summary>
		ShortCircuit
	}
}
