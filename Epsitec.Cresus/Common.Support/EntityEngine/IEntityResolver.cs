//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>IEntityResolver</c> interface is used to resolve entities based
	/// on partial information.
	/// </summary>
	public interface IEntityResolver
	{
		/// <summary>
		/// Gets a collection of entities matching the specified template.
		/// </summary>
		/// <param name="template">The template.</param>
		/// <returns>The collection of entities.</returns>
		IEnumerable<AbstractEntity> Resolve(AbstractEntity template);

		/// <summary>
		/// Gets a collection of entities matching the specified criteria.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <param name="criteria">The search criteria.</param>
		/// <returns>The collection of entities.</returns>
		IEnumerable<AbstractEntity> Resolve(Druid entityId, string criteria);
	}
}
