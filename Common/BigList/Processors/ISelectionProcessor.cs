//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.BigList.Processors
{
    public interface ISelectionProcessor
    {
        bool IsSelected(int index);
        void Select(int index, ItemSelection selection);
    }
}
