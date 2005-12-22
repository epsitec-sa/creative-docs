using System;

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// Summary description for ICanvasFactory.
	/// </summary>
	public interface ICanvasFactory
	{
		Image CreateCanvas(byte[] data);
	}
}
