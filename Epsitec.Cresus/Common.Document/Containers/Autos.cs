using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Autos contient tous les panneaux des styles.
	/// </summary>
	public class Autos : Abstract
	{
		public Autos(Document document) : base(document)
		{
			StaticText t1 = new StaticText(this);
			t1.Text = "<b>[ Debug ]</b>   <i>Montre les styles automatiques:</i>";
			t1.Dock = DockStyle.Top;
			t1.Margins = new Margins(0, 0, 0, 10);

			this.table = new CellTable(this);
			this.table.Dock = DockStyle.Fill;
			this.table.FlyOverChanged += this.HandleTableFlyOverChanged;
			this.table.StyleH  = CellArrayStyles.ScrollNorm;
			this.table.StyleH |= CellArrayStyles.Header;
			this.table.StyleH |= CellArrayStyles.Separator;
			this.table.StyleH |= CellArrayStyles.Mobile;
			this.table.StyleV  = CellArrayStyles.ScrollNorm;
			this.table.StyleV |= CellArrayStyles.Separator;
			this.table.DefHeight = 16;
		}
		

		public override void Hilite(Objects.Abstract hiliteObject)
		{
			//	Met en évidence l'objet survolé par la souris.
			if ( !this.IsVisible )  return;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.table.HiliteColor = context.HiliteSurfaceColor;

			int total = this.document.PropertiesAuto.Count + this.document.PropertiesSel.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Properties.Abstract property;
				
				if ( i < this.document.PropertiesAuto.Count )
				{
					property = this.document.PropertiesAuto[i] as Properties.Abstract;
				}
				else
				{
					int ii = i-this.document.PropertiesAuto.Count;
					property = this.document.PropertiesSel[ii] as Properties.Abstract;
				}

				bool hilite = Containers.Principal.IsObjectUseByProperty(property, hiliteObject);
				this.TableHiliteRow(i, hilite);
			}
		}

		
		protected override void DoUpdateContent()
		{
			//	Effectue la mise à jour du contenu.
			Viewer viewer = this.document.Modifier.ActiveViewer;
			DrawingContext context = viewer.DrawingContext;

			this.UpdateTable();
		}

		protected void UpdateTable()
		{
			//	Met à jour le contenu de la table.
			int rows = 0;
			rows += this.document.PropertiesAuto.Count;
			rows += this.document.PropertiesSel.Count;
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(3, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0,  12);
				this.table.SetWidthColumn(1, 170);
				this.table.SetWidthColumn(2,  30);
			}

			this.table.SetHeaderTextH(0, "");
			this.table.SetHeaderTextH(1, "Type");
			this.table.SetHeaderTextH(2, "Nb");

			for ( int i=0 ; i<rows ; i++ )
			{
				this.TableFillRow(i);

				Properties.Abstract property;
				GlyphButton gb;
				StaticText st;
				bool select = false;
				
				if ( i < this.document.PropertiesAuto.Count )
				{
					property = this.document.PropertiesAuto[i] as Properties.Abstract;
				}
				else
				{
					int j = i-this.document.PropertiesAuto.Count;
					property = this.document.PropertiesSel[j] as Properties.Abstract;
					select = true;
				}

				gb = this.table[0, i].Children[0] as GlyphButton;
				gb.GlyphShape = GlyphShape.None;
				this.table[0, i].IsHilite = false;

				st = this.table[1, i].Children[0] as StaticText;
				if ( select )
				{
					st.Text = string.Format("<b>{0}</b>", Properties.Abstract.Text(property.Type));
				}
				else
				{
					st.Text = Properties.Abstract.Text(property.Type);
				}

				st = this.table[2, i].Children[0] as StaticText;
				if ( select )
				{
					st.Text = string.Format("<b>{0}</b>", property.Owners.Count);
				}
				else
				{
					st.Text = property.Owners.Count.ToString();
				}
			}
		}

		protected void TableFillRow(int row)
		{
			//	Peuple une ligne de la table, si nécessaire.
			if ( this.table[0, row].IsEmpty )
			{
				GlyphButton gb = new GlyphButton();
				gb.ButtonStyle = ButtonStyle.None;
				gb.Dock = DockStyle.Fill;
				this.table[0, row].Insert(gb);
			}

			for ( int column=1 ; column<this.table.Columns ; column++ )
			{
				if ( this.table[column, row].IsEmpty )
				{
					StaticText st = new StaticText();
					st.ContentAlignment = (column==2) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
					st.Dock = DockStyle.Fill;
					this.table[column, row].Insert(st);
				}
			}
		}

		protected void TableHiliteRow(int row, bool hilite)
		{
			//	Hilite une ligne de la table.
			if ( row >= this.table.Rows )  return;

			if ( this.table[0, row].IsHilite != hilite )
			{
				this.table[0, row].IsHilite = hilite;
				GlyphButton gb = this.table[0, row].Children[0] as GlyphButton;
				gb.GlyphShape = hilite ? GlyphShape.ArrowRight : GlyphShape.None;
			}
		}


		private void HandleTableFlyOverChanged(object sender)
		{
			//	La cellule survolée a changé.
			int rank = this.table.FlyOverRow;

			Properties.Abstract property = null;
			if ( rank != -1 )
			{
				if ( rank < this.document.PropertiesAuto.Count )
				{
					property = this.document.PropertiesAuto[rank] as Properties.Abstract;
				}
				else
				{
					rank -= this.document.PropertiesAuto.Count;
					property = this.document.PropertiesSel[rank] as Properties.Abstract;
				}
			}

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer) )
			{
				obj.IsHilite = obj.PropertyExist(property);
			}
		}


		protected CellTable				table;
		protected bool					ignoreChanged = false;
	}
}
