namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for FrameBox.
	/// </summary>
	public class FrameBox : AbstractGroup
	{
		public FrameBox()
		{
		}
		
		public FrameBox(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
