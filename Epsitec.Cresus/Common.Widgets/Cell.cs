namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Cell implémente un conteneur pour peupler des tableaux et
	/// des grilles. Deux scénarios sont possibles :
	/// 
	/// - Seule une valeur est définie (Cell.Value), laquelle sert aussi de
	///   contenu. (cas Cell.IsSimple == true)
	/// 
	/// - En plus de la valeur, un contenu riche est défini (Cell.Text), lequel
	///   est affiché par la cellule elle-même, à la place de la valeur.
	///   (cas Cell.IsSimple == false)
	/// 
	/// Dans tous les cas, Cell.Contents retourne la chaîne formatée utilisée
	/// pour afficher le contenu.
	/// </summary>
	public class Cell : AbstractGroup
	{
		public Cell()
		{
			this.internal_state |= InternalState.AcceptTaggedText;
		}
		
		
		public bool						IsSimple
		{
			get { return this.text == null; }
		}
		
		public string					Value
		{
			get { return this.value; }
			set { this.value = value; }
		}
		
		public string					Contents
		{
			get
			{
				if (this.IsSimple)
				{
					if (this.AcceptTaggedText)
					{
						return TextLayout.ConvertToTaggedText (this.value);
					}
					else
					{
						return this.value;
					}
				}
				else
				{
					return this.Text;
				}
			}
		}
		
		public AbstractCellArray		CellArray
		{
			get
			{
				return this.Parent as AbstractCellArray;
			}
		}
		
		
		public int						RankColumn
		{
			get { return this.rank_column; }
		}
		
		public int						RankRow
		{
			get { return this.rank_row; }
		}
		
		
		internal void SetArrayRank(AbstractCellArray array, int column, int row)
		{
			this.Parent      = array;
			this.rank_column = column;
			this.rank_row    = row;
		}
		
		
		protected string				value;
		protected int					rank_column;
		protected int					rank_row;
	}
}
