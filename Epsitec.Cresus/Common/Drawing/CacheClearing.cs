//	Copyright © 2007-2008, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
    public enum CacheClearing
    {
        Default,

        ReleaseLargeBuffers,
        ReleaseMediumBuffers,
        ReleaseSmallBuffers,
        ReleaseEverything,
    }
}
