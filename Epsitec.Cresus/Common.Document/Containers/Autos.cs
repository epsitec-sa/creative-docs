using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Autos contient tous les panneaux des styles.
	/// </summary>
	[SuppressBundleSupport]
	public class Autos : Abstract
	{
		public Autos(Document document) : base(document)
		{
			StaticText t1 = new StaticText(this);
			t1.Text = "<b>[ Debug ]</b>   <i>Montre les styles automatiques:</i>";
			t1.Dock = DockStyle.Top;
			t1.DockMargins = new Margins(0, 0, 0, 10);

			this.table = new CellTable(this);
			this.table.Dock = DockStyle.Fill;
			this.table.FlyOverChanged += new EventHandler(this.HandleTableFlyOverChanged);
			this.table.StyleH  = CellArrayStyle.ScrollNorm;
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleH |= CellArrayStyle.Mobile;
			this.table.StyleV  = CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.DefHeight = 16;
		}
		

		// Met en évidence l'objet survolé par la souris.
		public override void Hilite(Objects.Abstract hiliteObject)
		{
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

		
		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			Viewer viewer = this.document.Modifier.ActiveViewer;
			DrawingContext context = viewer.DrawingContext;

			this.UpdateTable();
		}

		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			int rows = 0;
			rows += this.document.PropertiesAuto.Count;
			rows += this.document.PropertiesSel.Count;
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(4, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 12);
				this.table.SetWidthColumn(1, 94);
				this.table.SetWidthColumn(2, 84);
				this.table.SetWidthColumn(3, 22);
			}

			this.table.SetHeaderTextH(0, "");
			this.table.SetHeaderTextH(1, "Type");
			this.table.SetHeaderTextH(2, "Nom");
			this.table.SetHeaderTextH(3, "Nb");

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
				st.Text = property.StyleName;

				st = this.table[3, i].Children[0] as StaticText;
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

		// Peuple une ligne de la table, si nécessaire.
		protected void TableFillRow(int row)
		{
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
					st.Alignment = (column==3) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
					st.Dock = DockStyle.Fill;
					this.table[column, row].Insert(st);
				}
			}
		}

		// Hilite une ligne de la table.
		protected void TableHiliteRow(int row, bool hilite)
		{
			if ( this.table[0, row].IsHilite != hilite )
			{
				this.table[0, row].IsHilite = hilite;
				GlyphButton gb = this.table[0, row].Children[0] as GlyphButton;
				gb.GlyphShape = hilite ? GlyphShape.ArrowRight : GlyphShape.None;
			}
		}


		// La cellule survolée a changé.
		private void HandleTableFlyOverChanged(object sender)
		{
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
