using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// AggregateList est un widget "CellTable" pour les agrégats.
	/// </summary>
	public class AggregateList : CellTable
	{
		public AggregateList()
		{
			this.StyleH |= CellArrayStyle.ScrollNorm;
			this.StyleH |= CellArrayStyle.Header;
			this.StyleH |= CellArrayStyle.Separator;
			this.StyleH |= CellArrayStyle.Mobile;

			this.StyleV |= CellArrayStyle.ScrollNorm;
			this.StyleV |= CellArrayStyle.Separator;
			this.StyleV |= CellArrayStyle.SelectCell;

			this.DefHeight = 16;
		}

		public Document Document
		{
			get
			{
				return this.document;
			}

			set
			{
				this.document = value;
			}
		}

		public UndoableList List
		{
			get
			{
				return this.list;
			}

			set
			{
				if ( value == null )
				{
					this.list = new UndoableList(this.document, UndoableListType.AggregatesChildrens);
				}
				else
				{
					this.list = value;
				}
			}
		}

		public bool HScroller
		{
			get
			{
				return (this.StyleH & CellArrayStyle.ScrollNorm) != 0;
			}

			set
			{
				if ( value )
				{
					this.StyleH |= CellArrayStyle.ScrollNorm;
					this.StyleH |= CellArrayStyle.Mobile;
				}
				else
				{
					this.StyleH &= ~CellArrayStyle.ScrollNorm;
					this.StyleH &= ~CellArrayStyle.Mobile;
				}
			}
		}

		public bool VScroller
		{
			get
			{
				return (this.StyleV & CellArrayStyle.ScrollNorm) != 0;
			}

			set
			{
				if ( value )
				{
					this.StyleV |= CellArrayStyle.ScrollNorm;
				}
				else
				{
					this.StyleV &= ~CellArrayStyle.ScrollNorm;
				}
			}
		}

		// Ligne éventuelle à exclure.
		public int ExcludeRank
		{
			get
			{
				return this.excludeRank;
			}

			set
			{
				this.excludeRank = value;
			}
		}

		// Attributs cherchés en profondeur, dans les enfants.
		public bool IsDeep
		{
			get
			{
				return this.isDeep;
			}

			set
			{
				this.isDeep = value;
			}
		}

		// Première ligne avec <aucun>.
		public bool IsNoneLine
		{
			get
			{
				return this.isNoneLine;
			}

			set
			{
				this.isNoneLine = value;
			}
		}

		// Première colonne pour les mises en évidences.
		public bool IsHiliteColumn
		{
			get
			{
				return this.isHiliteColumn;
			}

			set
			{
				this.isHiliteColumn = value;
				if ( this.isHiliteColumn )  this.IsOrderColumn = false;
			}
		}

		// Première colonne pour les numéros d'ordre.
		public bool IsOrderColumn
		{
			get
			{
				return this.isOrderColumn;
			}

			set
			{
				this.isOrderColumn = value;
				if ( this.isOrderColumn )  this.isHiliteColumn = false;
			}
		}

		// Colonne pour les enfants.
		public bool IsChildrensColumn
		{
			get
			{
				return this.isChildrensColumn;
			}

			set
			{
				this.isChildrensColumn = value;
			}
		}

		// Sélection initiale.
		public bool IsInitialSelection
		{
			get
			{
				return this.isInitialSelection;
			}

			set
			{
				this.isInitialSelection = value;
			}
		}

		// Nombre total de colonnes pour les propriétés.
		public int TotalProperties
		{
			get
			{
				this.UpdateIndex();
				return this.types.Length;
			}
		}

		// Retourne le type de la propriété sélectionnée.
		public Properties.Type SelectedProperty
		{
			get
			{
				int row, column;
				this.GetSelectedRowColumn(out row, out column);
				if ( row == -1 || column == -1 )  return Properties.Type.None;

				return this.types[column-this.FixColumns];
			}
		}

		// Retourne le rang de la ligne sélectionné.
		public int SelectedPropertyRow
		{
			get
			{
				int row, column;
				this.GetSelectedRowColumn(out row, out column);
				return row;
			}
		}

		// Retourne le rang de la colonne sélectionnée.
		public int SelectedPropertyColumn
		{
			get
			{
				int row, column;
				this.GetSelectedRowColumn(out row, out column);
				return column;
			}
		}

		// Retourne le rang de la ligne/colonne sélectionné.
		public void GetSelectedRowColumn(out int row, out int column)
		{
			int nc = this.NameColumn;
			int fix = this.FixColumns;

			row = -1;
			column = -1;
			for ( int i=0 ; i<this.Rows ; i++ )
			{
				if ( this.IsCellSelected(i, nc) )
				{
					row = i;
					break;
				}
			}
			if ( row == -1 )  return;

			for ( int i=fix ; i<this.Columns ; i++ )
			{
				if ( this.IsCellSelected(row, i) )
				{
					column = i;
					break;
				}
			}
		}

		// Indique si une cellule est utilisée.
		public bool UsedCell(int row, int column)
		{
			int nc = this.NameColumn;
			int fix = this.FixColumns;
			if ( column == nc )  return true;
			if ( column < fix )  return false;

			this.UpdateIndex();
			int rank = this.RowToRank(row);
			if ( rank == -1 )  return false;  // ligne "aucun" ?
			Properties.Aggregate agg = this.list[rank] as Properties.Aggregate;
			return (agg.Property(this.types[column-fix], this.isDeep) != null);
		}

		// Met à jour le contenu de la table.
		public void UpdateContent()
		{
			System.Diagnostics.Debug.Assert(this.document != null);
			System.Diagnostics.Debug.Assert(this.list != null);
			this.typesDirty = true;
			this.UpdateIndex();

			int fix = this.FixColumns;
			int rows = this.list.Count;
			int initialColumns = this.Columns;
			this.SetArraySize(fix+this.types.Length, rows);
			int i;

			if ( initialColumns != this.Columns )
			{
				i = 0;
				if ( this.isHiliteColumn )     this.SetWidthColumn(i++,  12);
				if ( this.isOrderColumn )      this.SetWidthColumn(i++,  20);
				                               this.SetWidthColumn(i++, 125);
				if ( this.isChildrensColumn )  this.SetWidthColumn(i++,  20);

				for ( i=0 ; i<this.types.Length ; i++ )
				{
					this.SetWidthColumn(fix+i, 20);
				}
			}

			i = 0;
			if ( this.isHiliteColumn || this.isOrderColumn )
			{
				this.SetHeaderTextH(i++, "");
			}

			this.SetHeaderTextH(i++, Res.Strings.Aggregates.Header.Name);
			
			if ( this.isChildrensColumn )
			{
				this.SetHeaderTextH(i, Misc.Image("AggregateChildrens"));
				ToolTip.Default.SetToolTip(this.FindButtonH(i), Res.Strings.Panel.AggregateChildrens.Label.Name);
			}

			for ( i=0 ; i<this.types.Length ; i++ )
			{
				this.SetHeaderTextH(fix+i, Misc.Image(Properties.Abstract.IconText(this.types[i])));
				ToolTip.Default.SetToolTip(this.FindButtonH(fix+i), Properties.Abstract.Text(this.types[i]));
			}

			for ( i=0 ; i<rows ; i++ )
			{
				this.FillRow(i);
				this.UpdateRow(i);
			}
		}

		// Peuple une ligne de la table, si nécessaire.
		protected void FillRow(int row)
		{
			int nc = this.NameColumn;
			int fix = this.FixColumns;

			if ( this.isHiliteColumn )
			{
				if ( this[0, row].IsEmpty )
				{
					GlyphButton gb = new GlyphButton();
					gb.ButtonStyle = ButtonStyle.None;
					gb.Dock = DockStyle.Fill;
					this[0, row].Insert(gb);
				}
			}

			if ( this.isOrderColumn )
			{
				if ( this[0, row].IsEmpty )
				{
					StaticText st = new StaticText();
					st.Alignment = ContentAlignment.MiddleCenter;
					st.Dock = DockStyle.Fill;
					st.DockMargins = new Margins(2, 2, 0, 0);
					this[0, row].Insert(st);
				}
			}

			if ( this[nc, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleLeft;
				st.Dock = DockStyle.Fill;
				st.DockMargins = new Margins(4, 4, 0, 0);
				this[nc, row].Insert(st);
			}

			if ( this.isChildrensColumn )
			{
				if ( this[fix-1, row].IsEmpty )
				{
					StaticText st = new StaticText();
					st.Alignment = ContentAlignment.MiddleCenter;
					st.Dock = DockStyle.Fill;
					st.DockMargins = new Margins(2, 2, 0, 0);
					this[fix-1, row].Insert(st);
				}
			}

			for ( int i=0 ; i<this.types.Length ; i++ )  // colonnes des attributs
			{
				if ( this[fix+i, row].IsEmpty )
				{
					Sample sm = new Sample();
					sm.Document = this.document;
					sm.Dock = DockStyle.Fill;
					this[fix+i, row].Insert(sm);
				}
			}
		}

		// Met à jour le contenu d'une ligne de la table.
		public void UpdateRow(int row)
		{
			System.Diagnostics.Debug.Assert(this.document != null);
			System.Diagnostics.Debug.Assert(this.list != null);
			int rank = this.RowToRank(row);
			Properties.Aggregate agg = null;
			if ( rank != -1 )
			{
				agg = this.list[rank] as Properties.Aggregate;
			}
			bool selected = (rank == this.list.Selected && this.isInitialSelection);
			int nc = this.NameColumn;
			int fix = this.FixColumns;
			GlyphButton gb;
			StaticText st;
			Sample sm;

			if ( this.isHiliteColumn )
			{
				gb = this[0, row].Children[0] as GlyphButton;
				gb.GlyphShape = GlyphShape.None;
				this[0, row].IsHilite = false;
			}

			if ( this.isOrderColumn )
			{
				st = this[0, row].Children[0] as StaticText;
				st.Text = (row+1).ToString();
				this.SelectCell(0, row, selected);
			}

			st = this[nc, row].Children[0] as StaticText;
			st.Text = (agg==null) ? Res.Strings.Aggregates.NoneLine : agg.AggregateName;
			this.SelectCell(nc, row, selected);

			if ( this.isChildrensColumn )
			{
				string text = "";
				if ( agg != null )
				{
					int count = agg.Childrens.Count;
					if ( count != 0 )
					{
						text = count.ToString();
					}
				}
				st = this[fix-1, row].Children[0] as StaticText;
				st.Text = text;
				this.SelectCell(fix-1, row, selected);
			}

			this.UpdateIndex();
			for ( int i=0 ; i<this.types.Length ; i++ )  // colonnes des attributs
			{
				sm = this[fix+i, row].Children[0] as Sample;
				bool selectedColumn = false;
				if ( agg == null )
				{
					sm.Property = null;
				}
				else
				{
					if ( this.isDeep )
					{
						sm.Property = agg.Property(this.types[i], true);
					}
					else
					{
						sm.Property = agg.Property(this.types[i], false);
						sm.Dots = (agg.Property(this.types[i], true) != null);
					}

					if ( selected )
					{
						int index = agg.Styles.IndexOf(sm.Property);
						if ( index != -1 && index == agg.Styles.Selected )
						{
							selectedColumn = true;
						}
					}
				}
				this.SelectCell(fix+i, row, selectedColumn);
				sm.Invalidate();
			}
		}

		// Hilite une ligne de la table.
		public void HiliteRow(int row, bool hilite)
		{
			System.Diagnostics.Debug.Assert(this.list != null);
			if ( !this.isHiliteColumn )  return;

			if ( this[0, row].IsHilite != hilite )
			{
				this[0, row].IsHilite = hilite;
				GlyphButton gb = this[0, row].Children[0] as GlyphButton;
				gb.GlyphShape = hilite ? GlyphShape.ArrowRight : GlyphShape.None;
			}
		}


		// Met à jour la table des types de propriétés.
		protected void UpdateIndex()
		{
			if ( !this.typesDirty )  return;
			this.typesDirty = false;

			Properties.Type[] table = new Properties.Type[100];
			int total = 0;
			foreach ( Properties.Aggregate agg in this.list )
			{
				foreach ( Properties.Abstract property in agg.Styles )
				{
					int order = Properties.Abstract.SortOrder(property.Type);
					if ( table[order] == 0 )
					{
						table[order] = property.Type;
						total ++;
					}
				}
			}

			this.types = new Properties.Type[total];
			int j = 0;
			for ( int i=0 ; i<100 ; i++ )
			{
				if ( table[i] != 0 )  this.types[j++] = table[i];
			}
		}


		// Conversion d'un rang d'agrégat en numéro de ligne.
		public int RankToRow(int rank)
		{
			if ( this.isNoneLine )  rank ++;
			if ( this.excludeRank != -1 && rank-1 == this.excludeRank )  return -1;
			if ( this.excludeRank != -1 && rank > this.excludeRank )  rank --;
			return rank;
		}

		// Conversion d'un numéro de ligne en rang d'agrégat.
		public int RowToRank(int row)
		{
			if ( this.isNoneLine )  row --;
			if ( this.excludeRank != -1 && row >= this.excludeRank )  row ++;
			return row;
		}


		// Retourne le rang de la colonne pour le nom.
		protected int NameColumn
		{
			get
			{
				return (this.isHiliteColumn || this.IsOrderColumn) ? 1 : 0;
			}
		}

		// Retourne le nombre de colonnes initiales fixes.
		protected int FixColumns
		{
			get
			{
				int fix = 1;
				if ( this.isHiliteColumn    )  fix ++;
				if ( this.IsOrderColumn     )  fix ++;
				if ( this.isChildrensColumn )  fix ++;
				return fix;
			}
		}


		protected Document						document;
		protected UndoableList					list;
		protected int							excludeRank = -1;
		protected bool							isDeep = false;
		protected bool							isNoneLine = false;
		protected bool							isHiliteColumn = true;
		protected bool							isOrderColumn = false;
		protected bool							isChildrensColumn = true;
		protected bool							isInitialSelection = true;
		protected bool							typesDirty = true;
		protected Properties.Type[]				types;
	}
}
