namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextField impl�mente la ligne �ditable simple.
	/// </summary>
	public class TextField : AbstractTextField
	{
		public TextField()
		{
		}
		
		public TextField(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
