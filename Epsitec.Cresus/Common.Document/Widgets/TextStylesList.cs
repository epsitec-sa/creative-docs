using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// TextStylesList est un widget "CellTable" pour les styles de texte.
	/// </summary>
	public class TextStylesList : CellTable
	{
		public TextStylesList()
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
					this.list = new UndoableList(this.document, UndoableListType.TextStylesInsideDocument);
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

		public int ExcludeRank
		{
			//	Ligne éventuelle à exclure.
			get
			{
				return this.excludeRank;
			}

			set
			{
				this.excludeRank = value;
			}
		}

		public bool IsDeep
		{
			//	Attributs cherchés en profondeur, dans les enfants.
			get
			{
				return this.isDeep;
			}

			set
			{
				this.isDeep = value;
			}
		}

		public bool IsNoneLine
		{
			//	Première ligne avec <aucun>.
			get
			{
				return this.isNoneLine;
			}

			set
			{
				this.isNoneLine = value;
			}
		}

		public bool IsHiliteColumn
		{
			//	Première colonne pour les mises en évidences.
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

		public bool IsOrderColumn
		{
			//	Première colonne pour les numéros d'ordre.
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

		public bool IsChildrensColumn
		{
			//	Colonne pour les enfants.
			get
			{
				return this.isChildrensColumn;
			}

			set
			{
				this.isChildrensColumn = value;
			}
		}

		public bool IsInitialSelection
		{
			//	Sélection initiale.
			get
			{
				return this.isInitialSelection;
			}

			set
			{
				this.isInitialSelection = value;
			}
		}

		public int TotalProperties
		{
			//	Nombre total de colonnes pour les propriétés.
			get
			{
				this.UpdateIndex();
				return this.types.Length;
			}
		}

		public Text.Properties.WellKnownType SelectedProperty
		{
			//	Retourne le type de la propriété sélectionnée.
			get
			{
				int row, column;
				this.GetSelectedRowColumn(out row, out column);
				if ( row == -1 || column == -1 )  return Common.Text.Properties.WellKnownType.Other;

				return this.types[column-this.FixColumns];
			}
		}

		public int SelectedPropertyRow
		{
			//	Retourne le rang de la ligne sélectionné.
			get
			{
				int row, column;
				this.GetSelectedRowColumn(out row, out column);
				return row;
			}
		}

		public int SelectedPropertyColumn
		{
			//	Retourne le rang de la colonne sélectionnée.
			get
			{
				int row, column;
				this.GetSelectedRowColumn(out row, out column);
				return column;
			}
		}

		public void GetSelectedRowColumn(out int row, out int column)
		{
			//	Retourne le rang de la ligne/colonne sélectionné.
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

		public bool UsedCell(int row, int column)
		{
			//	Indique si une cellule est utilisée.
			int nc = this.NameColumn;
			int fix = this.FixColumns;
			if ( column == nc )  return true;
			if ( column < fix )  return false;

			this.UpdateIndex();
			int rank = this.RowToRank(row);
			if ( rank == -1 )  return false;  // ligne "aucun" ?
			return true;
		}

		public void UpdateContent()
		{
			//	Met à jour le contenu de la table.
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
				//?this.SetHeaderTextH(fix+i, Misc.Image(TextPanels.Abstract.IconText(this.types[i])));
				//?ToolTip.Default.SetToolTip(this.FindButtonH(fix+i), TextPanels.Abstract.LabelText(this.types[i]));
			}

			for ( i=0 ; i<rows ; i++ )
			{
				this.FillRow(i);
				this.UpdateRow(i);
			}
		}

		protected void FillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
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
					TextStyleSample sm = new TextStyleSample();
					sm.Document = this.document;
					sm.Dock = DockStyle.Fill;
					this[fix+i, row].Insert(sm);
				}
			}
		}

		public void UpdateRow(int row)
		{
			//	Met à jour le contenu d'une ligne de la table.
			System.Diagnostics.Debug.Assert(this.document != null);
			System.Diagnostics.Debug.Assert(this.list != null);
			int rank = this.RowToRank(row);
			Common.Text.TextStyle style = null;
			if ( rank != -1 )
			{
				style = this.list[rank] as Common.Text.TextStyle;
			}
			bool selected = (rank == this.list.Selected && this.isInitialSelection);
			int nc = this.NameColumn;
			int fix = this.FixColumns;
			GlyphButton gb;
			StaticText st;
			TextStyleSample sm;

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
			st.Text = (style==null) ? Res.Strings.Aggregates.NoneLine : style.Name;
			this.SelectCell(nc, row, selected);

			if ( this.isChildrensColumn )
			{
				string text = "";
				st = this[fix-1, row].Children[0] as StaticText;
				st.Text = text;
				this.SelectCell(fix-1, row, selected);
			}

			this.UpdateIndex();
			for ( int i=0 ; i<this.types.Length ; i++ )  // colonnes des attributs
			{
				sm = this[fix+i, row].Children[0] as TextStyleSample;
				bool selectedColumn = false;
				if ( style == null )
				{
					sm.Type = Common.Text.Properties.WellKnownType.Other;
					sm.TextStyle = null;
				}
				else
				{
					sm.Type = this.types[i];
					sm.TextStyle = style;

#if false
					if ( selected )
					{
						int index = agg.Styles.IndexOf(sm.Property);
						if ( index != -1 && index == agg.Styles.Selected )
						{
							selectedColumn = true;
						}
					}
#endif
				}
				this.SelectCell(fix+i, row, selectedColumn);
				sm.Invalidate();
			}
		}

		public void HiliteRow(int row, bool hilite)
		{
			//	Hilite une ligne de la table.
			System.Diagnostics.Debug.Assert(this.list != null);
			if ( !this.isHiliteColumn )  return;

			if ( this[0, row].IsHilite != hilite )
			{
				this[0, row].IsHilite = hilite;
				GlyphButton gb = this[0, row].Children[0] as GlyphButton;
				gb.GlyphShape = hilite ? GlyphShape.ArrowRight : GlyphShape.None;
			}
		}


		protected void UpdateIndex()
		{
			//	Met à jour la table des types de propriétés.
			if ( !this.typesDirty )  return;
			this.typesDirty = false;

			this.types = new Text.Properties.WellKnownType[4];
			int i = 0;
			this.types[i++] = Common.Text.Properties.WellKnownType.Font;
			this.types[i++] = Common.Text.Properties.WellKnownType.Margins;
			this.types[i++] = Common.Text.Properties.WellKnownType.Leading;
			this.types[i++] = Common.Text.Properties.WellKnownType.Language;
		}


		public int RankToRow(int rank)
		{
			//	Conversion d'un rang d'agrégat en numéro de ligne.
			if ( this.isNoneLine )  rank ++;
			if ( this.excludeRank != -1 && rank-1 == this.excludeRank )  return -1;
			if ( this.excludeRank != -1 && rank > this.excludeRank )  rank --;
			return rank;
		}

		public int RowToRank(int row)
		{
			//	Conversion d'un numéro de ligne en rang d'agrégat.
			if ( this.isNoneLine )  row --;
			if ( this.excludeRank != -1 && row >= this.excludeRank )  row ++;
			return row;
		}


		protected int NameColumn
		{
			//	Retourne le rang de la colonne pour le nom.
			get
			{
				return (this.isHiliteColumn || this.IsOrderColumn) ? 1 : 0;
			}
		}

		protected int FixColumns
		{
			//	Retourne le nombre de colonnes initiales fixes.
			get
			{
				int fix = 1;
				if ( this.isHiliteColumn    )  fix ++;
				if ( this.IsOrderColumn     )  fix ++;
				if ( this.isChildrensColumn )  fix ++;
				return fix;
			}
		}


		protected Document							document;
		protected UndoableList						list;
		protected int								excludeRank = -1;
		protected bool								isDeep = false;
		protected bool								isNoneLine = false;
		protected bool								isHiliteColumn = true;
		protected bool								isOrderColumn = false;
		protected bool								isChildrensColumn = true;
		protected bool								isInitialSelection = true;
		protected bool								typesDirty = true;
		protected Text.Properties.WellKnownType[]	types;
	}
}
