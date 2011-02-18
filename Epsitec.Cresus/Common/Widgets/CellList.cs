namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CellList repr�sente une liste de cellules (tableau � une
	/// colonne).
	/// </summary>
	public class CellList : AbstractCellArray
	{
		public CellList()
		{
		}
		
		public CellList(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
