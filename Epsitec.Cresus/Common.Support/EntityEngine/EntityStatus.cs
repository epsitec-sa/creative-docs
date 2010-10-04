//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityStatus</c> enumeration describes the status of an
	/// entity (empty, valid, not valid, etc.)
	/// </summary>
	public enum EntityStatus
	{
		/// <summary>
		/// The status of the entity is not known.
		/// </summary>
		Unknown = -1,
		
		/// <summary>
		/// The entity is empty. See also <see cref="AbstractEntity.IsValidWhenEmpty"/>.
		/// </summary>
		Empty,
		
		/// <summary>
		/// The entity is valid.
		/// </summary>
		Valid,
		
		/// <summary>
		/// The entity is not valid.
		/// </summary>
		Invalid,
	}
}
