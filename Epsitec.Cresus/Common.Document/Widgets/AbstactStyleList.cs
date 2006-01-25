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
			this.StyleH |= CellArrayStyle.ScrollNorm;
			this.StyleH |= CellArrayStyle.Header;
			this.StyleH |= CellArrayStyle.Separator;
			this.StyleH |= CellArrayStyle.Mobile;

			this.StyleV |= CellArrayStyle.ScrollNorm;
			this.StyleV |= CellArrayStyle.Separator;
			this.StyleV |= CellArrayStyle.SelectLine;

			this.DefHeight = 32;
			this.headerHeight = 16;
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
			//	Ligne �ventuelle � exclure.
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
			//	Attributs cherch�s en profondeur, dans les enfants.
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
			//	Premi�re ligne avec <aucun>.
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
			//	Premi�re colonne pour les mises en �vidences.
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
			//	Premi�re colonne pour les num�ros d'ordre.
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
			//	S�lection initiale.
			get
			{
				return this.isInitialSelection;
			}

			set
			{
				this.isInitialSelection = value;
			}
		}

		public int FixSelection
		{
			//	S�lection initiale fixe (pour un menu).
			get
			{
				return this.fixSelection;
			}

			set
			{
				this.fixSelection = value;
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

		public void UpdateContent()
		{
			//	Met � jour le contenu de la table.
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
				
				this.SetWidthColumn(i++, 115);  // noms
				widthUsed += 115;
				
				if ( this.isChildrensColumn )
				{
					this.SetWidthColumn(i++, 20);
					widthUsed += 20;
				}

				double w = 128;
				if ( this.fixWidth != 0 )  // largeur fixe pour toutes les colonnes ?
				{
					w = this.fixWidth-widthUsed-7;  // largeur restante
				}
				this.SetWidthColumn(i++, w);  // �chantillons
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

			for ( i=0 ; i<rows ; i++ )
			{
				this.FillRow(i);
				this.UpdateRow(i);
			}
		}

		protected void FillRow(int row)
		{
			//	Peuple une ligne de la table, si n�cessaire.
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

			if ( this[fix, row].IsEmpty )
			{
				AbstractSample sm = this.CreateSample();
				sm.Document = this.document;
				sm.Dock = DockStyle.Fill;
				this[fix, row].Insert(sm);
			}
		}

		public void UpdateRow(int row)
		{
			//	Met � jour le contenu d'une ligne de la table.
			System.Diagnostics.Debug.Assert(this.document != null);
			int rank = this.RowToRank(row);
			bool selected = ((rank == this.ListSelected && this.isInitialSelection) || rank == this.fixSelection);
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

			if ( this.isChildrensColumn )
			{
				st = this[fix-1, row].Children[0] as StaticText;
				st.Text = this.ListChildrensCount(rank);
			}

			sm = this[fix, row].Children[0] as AbstractSample;
			this.ListSample(sm, rank);

			this.SelectRow(row, selected);
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
			//	Conversion d'un rang d'agr�gat en num�ro de ligne.
			if ( this.isNoneLine )  rank ++;
			if ( this.excludeRank != -1 && rank-1 == this.excludeRank )  return -1;
			if ( this.excludeRank != -1 && rank > this.excludeRank )  rank --;
			return rank;
		}

		public int RowToRank(int row)
		{
			//	Conversion d'un num�ro de ligne en rang d'agr�gat.
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


		protected virtual int ListCount
		{
			//	Nombre le lignes de la liste.
			get
			{
				return 0;
			}
		}

		protected virtual int ListSelected
		{
			//	Ligne s�lectionn�e dans la liste.
			get
			{
				return -1;
			}
		}

		protected virtual string ListName(int rank)
		{
			//	Nom d'une ligne de la liste.
			return Res.Strings.Aggregates.NoneLine;
		}

		protected virtual string ListChildrensCount(int rank)
		{
			//	Nombre d'enfants d'une ligne de la liste.
			return "";
		}

		protected virtual AbstractSample CreateSample()
		{
			//	Cr�e un �chantillon.
			return null;
		}

		protected virtual void ListSample(AbstractSample sample, int rank)
		{
			//	 Met � jour l'�chantillon d'une ligne de la liste.
		}


		protected Document						document;
		protected int							excludeRank = -1;
		protected double						fixWidth = 0;
		protected bool							isDeep = false;
		protected bool							isNoneLine = false;
		protected bool							isHiliteColumn = true;
		protected bool							isOrderColumn = false;
		protected bool							isChildrensColumn = true;
		protected bool							isInitialSelection = true;
		protected int							fixSelection = -1;
	}
}
