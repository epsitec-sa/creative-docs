//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.BigList
{
    public interface IItemDataProvider<T> : IItemDataProvider
    {
        bool Resolve(int index, out T value);
    }
}
