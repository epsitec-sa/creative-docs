namespace Epsitec.Common.Drawing
{
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
	}
}
