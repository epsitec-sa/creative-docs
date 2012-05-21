namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe CellTable représente un tableau à deux dimensions contenant
	/// des cellules.
	/// </summary>
	public class CellTable : AbstractCellArray
	{
		public CellTable()
		{
		}
		
		public CellTable(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
	}
}
