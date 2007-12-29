//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		public static AbstractEntity Resolve(IEntityResolver resolver, AbstractEntity template)
		{
			if (resolver != null)
			{
				foreach (AbstractEntity result in resolver.Resolve (template))
				{
					return result;
				}
			}

			return null;
		}
	}
}
