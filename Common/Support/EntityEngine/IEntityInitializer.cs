//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
    public interface IEntityInitializer
    {
        void InitializeDefaultValues();
    }
}
