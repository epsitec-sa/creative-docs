//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
    public static class DrawingColorExtensions
    {
        public static bool IsGray(this System.Drawing.Color color)
        {
            return color.R == color.G && color.G == color.B;
        }
    }
}
