namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ProgressBar affiche un sablier (horizontal ou vertical).
	/// </summary>
	public class ProgressBar : Widget
	{
		public ProgressBar()
		{
		}
		
		public ProgressBar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
