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

namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// The <c>DialogFocusEventArgs</c> class contains information about a focus
    /// change related to dialog data fields.
    /// </summary>
    public class DialogFocusEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogFocusEventArgs"/> class.
        /// </summary>
        /// <param name="oldPath">The old path.</param>
        /// <param name="newPath">The new path.</param>
        public DialogFocusEventArgs(EntityFieldPath oldPath, EntityFieldPath newPath)
        {
            this.oldPath = oldPath;
            this.newPath = newPath;
        }

        /// <summary>
        /// Gets the old path.
        /// </summary>
        /// <value>The old path.</value>
        public EntityFieldPath OldPath
        {
            get { return this.oldPath; }
        }

        /// <summary>
        /// Gets the new path.
        /// </summary>
        /// <value>The new path.</value>
        public EntityFieldPath NewPath
        {
            get { return this.newPath; }
        }

        public override string ToString()
        {
            return string.Format(
                "{0} -> {1}",
                oldPath == null ? "<null>" : oldPath.ToString(),
                newPath == null ? "<null>" : newPath.ToString()
            );
        }

        private readonly EntityFieldPath oldPath;
        private readonly EntityFieldPath newPath;
    }
}
