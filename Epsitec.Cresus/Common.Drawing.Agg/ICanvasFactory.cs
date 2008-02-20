//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The ICanvasFactory interface is used to decouple the Common.Drawing
	/// and Common.Drawing.Agg assemblies; the Bitmap class can create Canvas
	/// instances without having to know its exact type.
	/// </summary>
	public interface ICanvasFactory
	{
		Image CreateCanvas(byte[] data);
	}
}
