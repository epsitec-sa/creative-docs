
[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.TextField))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextField implémente la ligne éditable simple.
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
