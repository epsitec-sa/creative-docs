//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>DataLifetimeExpectancy</c> enumeration defines how long data
	/// is expected to live.
	/// </summary>
	public enum DataLifetimeExpectancy
	{
		/// <summary>
		/// The lifetime expectancy is unknown.
		/// </summary>
		Unknown,

		/// <summary>
		/// The data is volatile and it might change frequently. Avoid storing it
		/// over a long time.
		/// </summary>
		Volatile,

		/// <summary>
		/// The data is stable and it won't change frequently. It might be meaningful
		/// to cache it over a longer period of time.
		/// </summary>
		Stable,

		/// <summary>
		/// The data is immutable. It will not change, or only after a restart (e.g.
		/// after the application resumed from maintenance mode).
		/// </summary>
		Immutable,
	}
}
