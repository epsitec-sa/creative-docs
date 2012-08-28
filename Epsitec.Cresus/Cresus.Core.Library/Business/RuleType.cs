//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>RuleType</c> enumeration defines the various business rule operations
	/// supported by <see cref="GenericBusinessRule"/>.
	/// </summary>
	public enum RuleType
	{
		/// <summary>
		/// Set up the entity, so that it has meaningful default values.
		/// </summary>
		Setup,

		/// <summary>
		/// Bind the entity to the user interface.
		/// </summary>
		Bind,

		/// <summary>
		/// Update the entity, after some editing occurred in the business context.
		/// </summary>
		Update,

		/// <summary>
		/// Validate the entity, to make sure that the data in the business context is
		/// coherent. This is usually called before the entities get persisted to the
		/// database.
		/// </summary>
		Validate,

		/// <summary>
		/// Unbind the entity from the user interface.
		/// </summary>
		Unbind,
	}
}
