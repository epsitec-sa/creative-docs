namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for Panel.
	/// </summary>
	public class Panel : AbstractGroup
	{
		public Panel()
		{
		}
		
		public Panel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
