namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CellBar permet de r�aliser des tool bars (tableau � une ligne).
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
