//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.BigList
{
    public class ItemListIndexEventArgs : EventArgs
    {
        public ItemListIndexEventArgs(int oldIndex, int newIndex)
        {
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
        }

        public int OldIndex
        {
            get { return this.oldIndex; }
        }

        public int NewIndex
        {
            get { return this.newIndex; }
        }

        private readonly int oldIndex;
        private readonly int newIndex;
    }
}
