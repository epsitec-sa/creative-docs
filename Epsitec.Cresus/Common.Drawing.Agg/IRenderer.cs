//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	public enum MaskComponent
	{
		None	= -1,
		
		A		= 0,
		Alpha	= 0,
		
		R		= 1,
		Red		= 1,
		G		= 2,
		Green	= 2,
		B		= 3,
		Blue	= 3
	}
	
	public interface IRenderer
	{
		Pixmap									Pixmap
		{
			get;
			set;
		}

		System.IntPtr							Handle
		{
			get;
		}
		
		
		void SetAlphaMask(Pixmap pixmap, MaskComponent component);
	}
}
