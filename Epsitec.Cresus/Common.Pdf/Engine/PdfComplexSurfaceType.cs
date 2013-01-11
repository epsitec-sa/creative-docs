
namespace Epsitec.Common.Engine.Pdf
{
	public enum PdfComplexSurfaceType
	{
		None            = -1,
		ExtGState       = 0,
		ExtGStateP1     = 1,
		ExtGStateP2     = 2,
		ExtGStateSmooth = 3,
		ShadingColor    = 4,
		ShadingGray     = 5,
		XObject         = 6,
		XObjectSmooth   = 7,
		XObjectMask     = 7,
	}
}
