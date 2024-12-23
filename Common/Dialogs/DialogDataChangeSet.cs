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
using Epsitec.Common.Types;

namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// The <c>DialogDataChangeSet</c> class describes a single modification
    /// in a <see cref="DialogData"/> record.
    /// </summary>
    public sealed class DialogDataChangeSet : System.IEquatable<DialogDataChangeSet>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogDataChangeSet"/> class.
        /// </summary>
        /// <param name="path">The field path.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public DialogDataChangeSet(EntityFieldPath path, object oldValue, object newValue)
        {
            this.path = path;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Gets the field path.
        /// </summary>
        /// <value>The field path.</value>
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
            get { return this.oldValue ?? UndefinedValue.Value; }
        }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public object NewValue
        {
            get { return this.newValue ?? UndefinedValue.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether the values are different.
        /// </summary>
        /// <value><c>true</c> if the values are different; otherwise, <c>false</c>.</value>
        public bool DifferentValues
        {
            get
            {
                object oldValue = this.OldValue;
                object newValue = this.NewValue;

                if (oldValue != newValue)
                {
                    if (oldValue.Equals(newValue) == false)
                    {
                        if (
                            (
                                UndefinedValue.IsUndefinedValue(oldValue)
                                || UnknownValue.IsUnknownValue(oldValue)
                            )
                            && (
                                UndefinedValue.IsUndefinedValue(newValue)
                                || UnknownValue.IsUnknownValue(newValue)
                            )
                        )
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        #region IEquatable<DialogDataChangeSet> Members

        public bool Equals(DialogDataChangeSet other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            if (object.ReferenceEquals(other, this))
            {
                return true;
            }

            if (this.path.Equals(other.path) == false)
            {
                return false;
            }

            if (this.oldValue != other.oldValue)
            {
                if (this.oldValue == null)
                {
                    return false;
                }

                if (this.oldValue.Equals(other.oldValue) == false)
                {
                    return false;
                }
            }

            if (this.newValue != other.newValue)
            {
                if (this.newValue == null)
                {
                    return false;
                }

                if (this.newValue.Equals(other.newValue) == false)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DialogDataChangeSet);
        }

        public override int GetHashCode()
        {
            return this.path.GetHashCode();
        }

        public override string ToString()
        {
            return string.Concat(
                this.path.ToString(),
                " ",
                this.oldValue == null ? "<null>" : this.oldValue.ToString(),
                " -> ",
                this.newValue == null ? "<null>" : this.newValue.ToString()
            );
        }

        private readonly EntityFieldPath path;
        private readonly object oldValue;
        private readonly object newValue;
    }
}
