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


namespace Epsitec.Common.UI
{
    /// <summary>
    /// The <c>PlaceholderSuggestionMode</c> enumeration defines how the
    /// <see cref="Placeholder"/> class manages its text fields (e.g. if they
    /// should display hints while interactively searching for information).
    /// </summary>
    public enum PlaceholderSuggestionMode
    {
        /// <summary>
        /// No suggestion mode. This is the default behaviour when editing and
        /// inputting data.
        /// </summary>
        None,

        /// <summary>
        /// Suggestion mode; the placeholder value defines the hint text in
        /// an active search field.
        /// </summary>
        DisplayActiveHint,

        /// <summary>
        /// Suggestion mode; the placeholder value defines the hint text in
        /// a passive search field.
        /// </summary>
        DisplayPassiveHint,

        /// <summary>
        /// Suggestion mode; the placeholder value defines the text; this is a
        /// special mode used only to reset what the user typed into the fields.
        /// </summary>
        DisplayHintResetText,
    }
}
