namespace Epsitec.Common.Drawing
{
	public enum MaskComponent
	{
		None = -1,
		A = 0,
		R = 1,
		G = 2,
		B = 3
	}
	
	public interface IRenderer
	{
		Pixmap					Pixmap
		{
			get;
			set;
		}

		System.IntPtr			Handle
		{
			get;
		}
		
		void SetAlphaMask(Pixmap pixmap, MaskComponent component);
	}
}
