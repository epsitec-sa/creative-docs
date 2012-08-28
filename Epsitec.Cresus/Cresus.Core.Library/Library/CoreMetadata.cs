//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>CoreMetadata</c> class is used as a baseclass for various metadata, which
	/// can be retrieved through the <see cref="CoreContext"/>.
	/// </summary>
	public abstract class CoreMetadata
	{
		/// <summary>
		/// Adds the specified metadata to this instance of metadata. This is used to
		/// merge two sets of metadata.
		/// </summary>
		/// <param name="metadata">The metadata.</param>
		public abstract void Add(CoreMetadata metadata);
	}
}
