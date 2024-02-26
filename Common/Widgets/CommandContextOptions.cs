//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
    [System.Flags]
    public enum CommandContextOptions
    {
        None = 0,

        Fence = 0x0001,
        ActivateWithoutFocus = 0x0002,
    }
}
