namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CellList représente une liste de cellules (tableau à une
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
