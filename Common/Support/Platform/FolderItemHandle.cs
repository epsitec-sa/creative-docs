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


namespace Epsitec.Common.Support.Platform
{
    internal abstract class FolderItemHandle
        : System.IDisposable,
            System.IEquatable<FolderItemHandle>
    {
        public FolderItemHandle() { }

        ~FolderItemHandle()
        {
            this.Dispose(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        #endregion

        #region IEquatable<FolderItemHandle> Members

        public bool Equals(FolderItemHandle other)
        {
            if (System.Object.ReferenceEquals(other, null))
            {
                return false;
            }
            if (System.Object.ReferenceEquals(other, this))
            {
                return true;
            }

            return this.InternalEquals(other);
        }

        #endregion

        public override bool Equals(object obj)
        {
            FolderItemHandle other = obj as FolderItemHandle;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected abstract void Dispose(bool disposing);
        protected abstract bool InternalEquals(FolderItemHandle other);
    }
}
