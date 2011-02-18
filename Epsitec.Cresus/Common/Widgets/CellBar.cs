namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CellBar permet de réaliser des tool bars (tableau à une ligne).
	/// </summary>
	public class CellBar : AbstractCellArray
	{
		public CellBar()
		{
		}
		
		public CellBar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
