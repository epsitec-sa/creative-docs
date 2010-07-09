//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.DataLayer.Context
{
	/// <summary>
	/// The <c>EntityResolutionMode</c> specifies how entity references should
	/// be resolved.
	/// </summary>
	public enum EntityResolutionMode
	{
		/// <summary>
		/// Finds only loaded entities; if no entity can be found in memory,
		/// then simply return <c>null</c>.
		/// </summary>
		Find,

		/// <summary>
		/// Finds loaded entities and loads the missing ones.
		/// </summary>
		Load,

		/// <summary>
		/// Finds loaded entities and creates proxies for the missing ones.
		/// The proxies will be transparently resolved when they are accessed
		/// for the first time.
		/// </summary>
		DelayLoad
	}
}
