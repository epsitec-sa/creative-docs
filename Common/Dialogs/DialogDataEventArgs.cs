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

namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// The <c>DialogDataEventArgs</c> class contains information about a field
    /// value change in a dialog.
    /// </summary>
    public class DialogDataEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogDataEventArgs"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="placeholder">The placeholder.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public DialogDataEventArgs(
            EntityFieldPath path,
            AbstractPlaceholder placeholder,
            object oldValue,
            object newValue
        )
        {
            this.path = path;
            this.placeholder = placeholder;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Gets the path of the edited value.
        /// </summary>
        /// <value>The path.</value>
        public EntityFieldPath Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public object OldValue
        {
            get { return this.oldValue; }
        }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public object NewValue
        {
            get { return this.newValue; }
        }

        public override string ToString()
        {
            return string.Format(
                "{0} : {1} -> {2}",
                path.ToString(),
                oldValue ?? "<null>",
                newValue ?? "<null>"
            );
        }

        private readonly EntityFieldPath path;
        private readonly AbstractPlaceholder placeholder;
        private readonly object oldValue;
        private readonly object newValue;
    }
}
