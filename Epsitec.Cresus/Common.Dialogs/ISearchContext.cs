//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.UI;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>ISearchContext</c> interface describes a search context. See
	/// <see cref="DialogSearchController"/> for an implementation.
	/// </summary>
	public interface ISearchContext
	{
		/// <summary>
		/// Gets the search controller associated with this search context.
		/// </summary>
		/// <value>The search controller.</value>
		DialogSearchController SearchController
		{
			get;
		}

		/// <summary>
		/// Gets the search template data used by this search controller.
		/// </summary>
		/// <value>The search template data.</value>
		AbstractEntity SearchTemplate
		{
			get;
		}

		/// <summary>
		/// Gets the active placeholders used by this search controller.
		/// </summary>
		/// <returns>A collection of <see cref="AbstractPlaceholder"/> instances.</returns>
		IEnumerable<AbstractPlaceholder> GetActivePlaceholders();
	}
}
