//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.BigList
{
    public class IndexedArray<TValue> : List<TValue>
    {
        public IndexedArray(int capacity)
            : base(capacity) { }
    }
}
