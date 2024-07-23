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


namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>IDocumentInfo</c> interface gives access to document information
    /// (a descriptive text and a thumbnail image).
    /// </summary>
    public interface IDocumentInfo
    {
        /// <summary>
        /// Gets the description of the document as a formatted string.
        /// </summary>
        /// <returns>The description of the document.</returns>
        string GetDescription();

        /// <summary>
        /// Gets the thumbnail image of the document.
        /// </summary>
        /// <returns>The thumbnail image or <c>null</c>.</returns>
        Drawing.Image GetThumbnail();

        /// <summary>
        /// Asynchronously gets the image thumbnail of the document.
        /// </summary>
        /// <param name="callback">The callback which will be invoked with the
        /// thumbnail image, as soon as it will be available; this may happen
        /// synchronously when calling <c>GetAsyncThumbnail</c>, or later.</param>
        void GetAsyncThumbnail(SimpleCallback<Drawing.Image> callback);
    }
}
