//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>ResolutionMode</c> defines how the resolver behaves if no matching
	/// type can be found.
	/// </summary>
	public enum ResolutionMode
	{
		/// <summary>
		/// Returns <c>null</c> if the type cannot be resolved.
		/// </summary>
		NullOnError,

		/// <summary>
		/// Throws an exception if the type cannot be resolved.
		/// </summary>
		ThrowOnError,

		InspectOnly,
	}
}
