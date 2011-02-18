//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The <c>ImageFilteringMode</c> enumeration lists the available image filtering
	/// modes used when interpolating.
	/// </summary>
	public enum ImageFilteringMode
	{
		None = 0,
		Bilinear = 1,
		Bicubic = 2,
		Spline16 = 3,
		Spline36 = 4,
		Kaiser = 5,
		Quadric = 6,
		Catrom = 7,
		Gaussian = 8,
		Bessel = 9,
		Mitchell = 10,
		Sinc = 11,
		Lanczos = 12,
		Blackman = 13,
		
		ResamplingBilinear = -1,
		ResamplingBicubic = -2,
		ResamplingSpline16 = -3,
		ResamplingSpline36 = -4,
		ResamplingKaiser = -5,
		ResamplingQuadric = -6,
		ResamplingCatrom = -7,
		ResamplingGaussian = -8,
		ResamplingBessel = -9,
		ResamplingMitchell = -10,
		ResamplingSinc = -11,
		ResamplingLanczos = -12,
		ResamplingBlackman = -13
	}
}
