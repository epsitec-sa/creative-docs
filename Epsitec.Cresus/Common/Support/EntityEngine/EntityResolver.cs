//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityResolver</c> class provides support for common resolution
	/// tasks related to the <see cref="IEntityResolver"/> interface.
	/// </summary>
	public static class EntityResolver
	{
		/// <summary>
		/// Finds the most appropriate entity based on the specified resolver
		/// and search template.
		/// </summary>
		/// <param name="resolver">The entity resolver.</param>
		/// <param name="template">The search template.</param>
		/// <returns>The most appropriate entity or <c>null</c>.</returns>
		public static EntityResolverResult Resolve(IEntityResolver resolver, AbstractEntity template)
		{
			if (resolver != null)
			{
				return new EntityResolverResult (resolver.Resolve (template));
			}

			return null;
		}

		/// <summary>
		/// Finds the most appropriate entity based on the specified resolver
		/// and search criteria.
		/// </summary>
		/// <param name="resolver">The entity resolver.</param>
		/// <param name="entityId">The entity id.</param>
		/// <param name="criteria">The search criteria.</param>
		/// <returns>The most appropriate entity or <c>null</c>.</returns>
		public static EntityResolverResult Resolve(IEntityResolver resolver, Druid entityId, string criteria)
		{
			if (resolver != null)
			{
				return new EntityResolverResult (resolver.Resolve (entityId, criteria));
			}

			return null;
		}
	}
}
