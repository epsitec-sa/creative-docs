//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Linq;

namespace Epsitec.Aider.Data.Job
{
    [System.Flags]
    internal enum UserRemovalMode
    {
        None            = 0b0000_0000,
        Empty           = 0b0000_0001,
        NoContact       = 0b0000_0010,
        NoEmail         = 0b0000_0100,
        NotAnEmployee   = 0b0000_1000,
        BrokenEmployee  = 0b0001_0000
    }
}
