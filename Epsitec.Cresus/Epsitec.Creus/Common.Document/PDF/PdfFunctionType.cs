//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Document.PDF
{
	public enum PdfFunctionType
	{
		SampledColor = 0,
		SampledAlpha = 1,
		Color1 = 2,
		// red or cyan or gray
		Color2 = 3,
		// green or magenta
		Color3 = 4,
		// blue or yellow
		Color4 = 5,
		// black
		Alpha = 6
	}
}
