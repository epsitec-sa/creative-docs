//	Copyright © 2005-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// AbstractStyleList est la classe de base pour toutes les listes de styles.
	/// </summary>
	public abstract class AbstractStyleList : CellTable
	{
		public AbstractStyleList()
		{
			this.StyleH |= CellArrayStyles.ScrollNorm;
			this.StyleH |= CellArrayStyles.Header;
			this.StyleH |= CellArrayStyles.Separator;
			this.StyleH |= CellArrayStyles.Mobile;

			this.StyleV |= CellArrayStyles.ScrollNorm;
			this.StyleV |= CellArrayStyles.Separator;
			this.StyleV |= CellArrayStyles.SelectLine;

			this.DefHeight = 32;
			this.HeaderHeight = 16;
			this.IsFlyOverHilite = true;
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

		public bool HScroller
		{
			get
			{
				return (this.StyleH & CellArrayStyles.ScrollNorm) != 0;
			}

			set
			{
				if ( value )
				{
					this.StyleH |= CellArrayStyles.ScrollNorm;
					this.StyleH |= CellArrayStyles.Mobile;
				}
				else
				{
					this.StyleH &= ~CellArrayStyles.ScrollNorm;
					this.StyleH &= ~CellArrayStyles.Mobile;
				}
			}
		}

		public bool VScroller
		{
			get
			{
				return (this.StyleV & CellArrayStyles.ScrollNorm) != 0;
			}

			set
			{
				if ( value )
				{
					this.StyleV |= CellArrayStyles.ScrollNorm;
				}
				else
				{
					this.StyleV &= ~CellArrayStyles.ScrollNorm;
				}
			}
		}

		public int SelectedRank
		{
			//	Ligne sélectionnée.
			get
			{
				return this.selectedRank;
			}

			set
			{
				this.selectedRank = value;
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

		public bool IsHeader
		{
			//	En-tête en haut de la liste.
			get
			{
				return this.isHeader;
			}

			set
			{
				if ( this.isHeader != value )
				{
					this.isHeader = value;

					if ( this.isHeader )
					{
						this.StyleH |= CellArrayStyles.Header;
						this.StyleH |= CellArrayStyles.Mobile;
					}
					else
					{
						this.StyleH &= ~CellArrayStyles.Header;
						this.StyleH &= ~CellArrayStyles.Mobile;
					}
				}
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

		public double FixWidth
		{
			//	Largeur fixe pour toutes les colonnes.
			get
			{
				return this.fixWidth;
			}

			set
			{
				this.fixWidth = value;
			}
		}

		public void UpdateContents()
		{
			//	Met à jour le contenu de la table.
			System.Diagnostics.Debug.Assert(this.document != null);

			int fix = this.FixColumns;
			int rows = this.ListCount;
			int initialColumns = this.Columns;
			this.SetArraySize(fix+1, rows);
			int i;

			if ( initialColumns != this.Columns )
			{
				double widthUsed = 0;
				i = 0;
				if ( this.isHiliteColumn )
				{
					this.SetWidthColumn(i++, 12);
					widthUsed += 12;
				}

				if ( this.isOrderColumn )
				{
					this.SetWidthColumn(i++, 20);
					widthUsed += 20;
				}
				
				this.SetWidthColumn(i++, 109);  // noms
				widthUsed += 109;
				
				double w = 96;
				if ( this.fixWidth != 0 )  // largeur fixe pour toutes les colonnes ?
				{
					w = this.fixWidth-widthUsed-7;  // largeur restante
				}
				this.SetWidthColumn(i++, w);  // échantillons
			}

			i = 0;
			if ( this.isHiliteColumn || this.isOrderColumn )
			{
				this.SetHeaderTextH(i++, "");
			}

			this.SetHeaderTextH(i++, Res.Strings.Aggregates.Header.Name);
			this.SetHeaderTextH(i++, Res.Strings.Aggregates.Header.Sample);
			
			for ( i=0 ; i<rows ; i++ )
			{
				this.FillRow(i);
				this.UpdateRow(i, true);
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
					st.ContentAlignment = ContentAlignment.MiddleCenter;
					st.Dock = DockStyle.Fill;
					st.Margins = new Margins(2, 2, 0, 0);
					this[0, row].Insert(st);
				}
			}

			if ( this[nc, row].IsEmpty )
			{
				StaticText st = new StaticText();
				st.ContentAlignment = ContentAlignment.MiddleLeft;
				st.Dock = DockStyle.Fill;
				st.Margins = new Margins(4, 4, 0, 0);
				this[nc, row].Insert(st);
			}

			if ( this[fix, row].IsEmpty )
			{
				AbstractSample sm = this.CreateSample();
				sm.Document = this.document;
				sm.IsDeep = this.isDeep;
				sm.Dock = DockStyle.Fill;
				this[fix, row].Insert(sm);
			}
		}

		public void UpdateRow(int row, bool updateSelection)
		{
			//	Met à jour le contenu d'une ligne de la table.
			System.Diagnostics.Debug.Assert(this.document != null);
			int rank = this.RowToRank(row);
			bool selected = (rank == this.selectedRank);
			int nc = this.NameColumn;
			int fix = this.FixColumns;
			GlyphButton gb;
			StaticText st;
			AbstractSample sm;

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
			}

			st = this[nc, row].Children[0] as StaticText;
			st.Text = this.ListName(rank);

			sm = this[fix, row].Children[0] as AbstractSample;
			this.ListSample(sm, rank);

			if ( updateSelection )
			{
				this.SelectRow(row, selected);
			}
		}

		public void HiliteRow(int row, bool hilite)
		{
			//	Hilite une ligne de la table.
			if ( !this.isHiliteColumn )  return;

			if ( this[0, row].IsHilite != hilite )
			{
				this[0, row].IsHilite = hilite;
				GlyphButton gb = this[0, row].Children[0] as GlyphButton;
				gb.GlyphShape = hilite ? GlyphShape.ArrowRight : GlyphShape.None;
			}
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
				if ( this.isHiliteColumn )  fix ++;
				if ( this.IsOrderColumn  )  fix ++;
				return fix;
			}
		}


		public override Size GetBestFitSize()
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Margins margins = adorner.GeometryArrayMargins;

			double w = 109+96+margins.Left+margins.Right;
			double h = this.ListCount*32+1+margins.Bottom+margins.Top;
			
			return new Drawing.Size(w, h);
		}

		
		protected virtual int ListCount
		{
			//	Nombre de lignes de la liste.
			get
			{
				return 0;
			}
		}

		protected virtual string ListName(int rank)
		{
			//	Nom d'une ligne de la liste.
			return Res.Strings.Aggregates.NoneLine;
		}

		protected virtual AbstractSample CreateSample()
		{
			//	Crée un échantillon.
			return null;
		}

		protected virtual void ListSample(AbstractSample sample, int rank)
		{
			//	 Met à jour l'échantillon d'une ligne de la liste.
		}


		protected Document						document;
		protected int							selectedRank = -1;
		protected int							excludeRank = -1;
		protected double						fixWidth;
		protected bool							isDeep;
		protected bool							isHeader = true;
		protected bool							isNoneLine;
		protected bool							isHiliteColumn = true;
		protected bool							isOrderColumn;
	}
}
