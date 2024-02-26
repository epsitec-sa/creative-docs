//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.BigList
{
    public interface IItemDataRenderer
    {
        void Render(
            ItemData data,
            ItemState state,
            ItemListRow row,
            Graphics graphics,
            Rectangle bounds
        );
    }
}
