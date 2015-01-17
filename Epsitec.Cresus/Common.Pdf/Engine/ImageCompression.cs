//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Pdf.Engine
{
	public enum ImageCompression
	{
		None       = 0,
		
		ZipFast    = 1,
		ZipDefault = 5,
		ZipBest    = 9,

		JPEG       = 20,
	}
}
