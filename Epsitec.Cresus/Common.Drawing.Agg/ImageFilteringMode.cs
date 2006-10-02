//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		Sync = 11,
		Lanczos = 12,
		Blackman = 13
	}
}
