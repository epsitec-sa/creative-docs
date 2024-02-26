//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.BigList
{
    public interface IItemDataMapper<T> : IItemDataMapper
    {
        ItemData<T> Map(T value);
    }
}
