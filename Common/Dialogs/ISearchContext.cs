/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.UI;
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
        DialogSearchController SearchController { get; }

        /// <summary>
        /// Gets the search template data used by this search controller.
        /// </summary>
        /// <value>The search template data.</value>
        AbstractEntity SearchTemplate { get; }

        /// <summary>
        /// Gets the active suggestion.
        /// </summary>
        /// <value>The active suggestion.</value>
        AbstractEntity ActiveSuggestion { get; }

        /// <summary>
        /// Gets the active placeholders used by this search controller.
        /// </summary>
        /// <returns>A collection of <see cref="AbstractPlaceholder"/> instances.</returns>
        IEnumerable<AbstractPlaceholder> GetActivePlaceholders();

        /// <summary>
        /// Sets the suggestion.
        /// </summary>
        /// <param name="suggestion">The suggestion.</param>
        void SetSuggestion(AbstractEntity suggestion);

        /// <summary>
        /// Gets the ids of the entity types which are currently targeted by
        /// the search context.
        /// </summary>
        /// <returns>A collection of entity ids.</returns>
        IEnumerable<Support.Druid> GetEntityIds();
    }
}
