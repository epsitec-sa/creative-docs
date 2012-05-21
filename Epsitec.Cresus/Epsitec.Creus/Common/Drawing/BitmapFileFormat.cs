//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Epsitec.Common.Drawing.Platform
{
	public class BitmapFileFormat
	{
		public BitmapFileFormat()
		{
			this.Type = BitmapFileType.Unknown;
		}


		public BitmapFileType Type
		{
			get;
			set;
		}

		public TiffCompressionOption TiffCompression
		{
			get;
			set;
		}

		public int Quality
		{
			get;
			set;
		}

		public bool TiffCmyk
		{
			get;
			set;
		}
	}
}
