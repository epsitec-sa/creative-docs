//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		ShortCircuit,

		/// <summary>
		/// Special set mode used to define the initial collection (when the first
		/// read access is made on a collection field).
		/// </summary>
		InitialCollection,
	}
}
