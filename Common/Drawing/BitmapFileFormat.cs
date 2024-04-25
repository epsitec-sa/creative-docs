//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Platform
{
    public class BitmapFileFormat
    {
        // TODO bl-net8-cross this class can probably be deleted
        public BitmapFileFormat()
        {
            this.Type = BitmapFileType.Unknown;
        }

        public BitmapFileType Type { get; set; }

        public TiffCompressionOption TiffCompression { get; set; }

        public int Quality { get; set; }

        public bool TiffCmyk { get; set; }
    }
}
