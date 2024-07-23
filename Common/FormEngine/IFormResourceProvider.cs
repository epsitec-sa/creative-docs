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


using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.FormEngine
{
    /// <summary>
    /// The <c>IFormResourceProvider</c> interface defines the methods needed
    /// by the form engine to access data stored in the resources.
    /// </summary>
    public interface IFormResourceProvider : IStructuredTypeResolver, ICaptionResolver
    {
        /// <summary>
        /// Clears the cached information.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Gets the XML source for the specified form.
        /// </summary>
        /// <param name="formId">The form id.</param>
        /// <returns>The XML source or <c>null</c>.</returns>
        string GetFormXmlSource(Druid formId);
    }
}
