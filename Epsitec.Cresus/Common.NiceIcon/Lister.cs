using Epsitec.Common.NiceIcon;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Lister permet de représenter un tableau des icônes.
	/// </summary>
	public class Lister : Widget
	{
		public Lister()
		{
			this.table = new CellTable(this);
			this.table.Bounds = this.Bounds;
			this.table.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			this.table.SelectionChanged += new EventHandler(this.TableSelectionChanged);

			this.table.StyleH  = CellArrayStyle.ScrollNorm;
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleH |= CellArrayStyle.Mobile;
			this.table.StyleV  = CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.SelectLine;
			this.table.StyleV |= CellArrayStyle.SelectMulti;
			this.table.DefHeight = 20;

			this.objects = new IconObjects();
		}
		
		public Lister(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		// Liste des objets.
		public System.Collections.ArrayList Objects
		{
			get
			{
				return this.objects.Objects;
			}

			set
			{
				this.objects.Objects = value;
			}
		}

		public override void Invalidate()
		{
			this.UpdateContent();  // TODO: faudrait éviter de refaire toute la liste !!!
			base.Invalidate();
		}

		// Met à jour le contenu du tableau.
		public void UpdateContent()
		{
			if ( this.objects == null )  return;
			if ( !this.IsVisible )  return;

			ObjectMemory memo = new ObjectMemory();
			memo.CreateProperties();

			int ir = this.table.Columns;
			int tp = memo.TotalProperty;
			int total = this.objects.Count;
			int rows = this.table.Rows;
			this.table.SetArraySize(tp+2, total);

			if ( ir != tp+2 )
			{
				this.table.SetWidthColumn(0, 35);
				this.table.SetWidthColumn(1, 35);

				for ( int p=0 ; p<tp; p++ )
				{
					this.table.SetWidthColumn(p+2, 50);
				}
			}

			if ( ir != tp+2 || total != rows )
			{
				this.table.SetHeaderTextH(0, "Rang");
				this.table.SetHeaderTextH(1, "Type");

				for ( int p=0 ; p<tp; p++ )
				{
					AbstractProperty property = memo.Property(p);
					this.table.SetHeaderTextH(p+2, property.Text);
				}
			}

			Cell			cell;
			StaticText		st;
			IconButton		ib;
			ColorSample		cs;
			GradientSample	gs;

			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = this.objects[index];

				if ( this.table[0, index].Children.Count == 0 )
				{
					cell = new Cell();
					st = new StaticText();
					st.Alignment = Drawing.ContentAlignment.MiddleCenter;
					st.Dock = Widgets.DockStyle.Fill;
					cell.Children.Add(st);
					this.table[0, index] = cell;
				}
				st = this.table[0, index].Children[0] as StaticText;
				st.Text = System.Convert.ToString(index+1);

				if ( this.table[1, index].Children.Count == 0 )
				{
					cell = new Cell();
					ib = new IconButton();
					ib.SetFrozen(true);
					ib.Dock = Widgets.DockStyle.Fill;
					cell.Children.Add(ib);
					this.table[1, index] = cell;
				}
				ib = this.table[1, index].Children[0] as IconButton;
				ib.IconName = obj.IconName;

				for ( int p=0 ; p<tp; p++ )
				{
					AbstractProperty property = memo.Property(p);
					AbstractProperty po = obj.GetProperty(property.Type);

					if ( po is PropertyBool )
					{
						if ( this.table[p+2, index].Children.Count == 0 )
						{
							cell = new Cell();
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = Widgets.DockStyle.Fill;
							cell.Children.Add(st);
							this.table[p+2, index] = cell;
						}
						PropertyBool prop = po as PropertyBool;
						st = this.table[p+2, index].Children[0] as StaticText;
						if ( prop.Bool )  st.Text = "Oui";
						else              st.Text = "Non";
					}
					else if ( po is PropertyDouble )
					{
						if ( this.table[p+2, index].Children.Count == 0 )
						{
							cell = new Cell();
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = Widgets.DockStyle.Fill;
							cell.Children.Add(st);
							this.table[p+2, index] = cell;
						}
						PropertyDouble prop = po as PropertyDouble;
						st = this.table[p+2, index].Children[0] as StaticText;
						st.Text = System.Convert.ToString(prop.Value);
					}
					else if ( po is PropertyLine )
					{
						if ( this.table[p+2, index].Children.Count == 0 )
						{
							cell = new Cell();
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = Widgets.DockStyle.Fill;
							cell.Children.Add(st);
							this.table[p+2, index] = cell;
						}
						PropertyLine prop = po as PropertyLine;
						st = this.table[p+2, index].Children[0] as StaticText;
						st.Text = System.Convert.ToString(prop.Width);
					}
					else if ( po is PropertyString )
					{
						if ( this.table[p+2, index].Children.Count == 0 )
						{
							cell = new Cell();
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = Widgets.DockStyle.Fill;
							cell.Children.Add(st);
							this.table[p+2, index] = cell;
						}
						PropertyString prop = po as PropertyString;
						st = this.table[p+2, index].Children[0] as StaticText;
						st.Text = prop.String;
					}
					else if ( po is PropertyList )
					{
						if ( this.table[p+2, index].Children.Count == 0 )
						{
							cell = new Cell();
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = Widgets.DockStyle.Fill;
							cell.Children.Add(st);
							this.table[p+2, index] = cell;
						}
						PropertyList prop = po as PropertyList;
						st = this.table[p+2, index].Children[0] as StaticText;
						st.Text = prop.Get(prop.Choice);
					}
					else if ( po is PropertyColor )
					{
						if ( this.table[p+2, index].Children.Count == 0 )
						{
							cell = new Cell();
							cs = new ColorSample();
							cs.SetFrozen(true);
							cs.Dock = Widgets.DockStyle.Fill;
							cell.Children.Add(cs);
							this.table[p+2, index] = cell;
						}
						PropertyColor prop = po as PropertyColor;
						cs = this.table[p+2, index].Children[0] as ColorSample;
						cs.Color = prop.Color;
					}
					else if ( po is PropertyGradient )
					{
						if ( this.table[p+2, index].Children.Count == 0 )
						{
							cell = new Cell();
							gs = new GradientSample();
							gs.SetFrozen(true);
							gs.Dock = Widgets.DockStyle.Fill;
							cell.Children.Add(gs);
							this.table[p+2, index] = cell;
						}
						PropertyGradient prop = po as PropertyGradient;
						gs = this.table[p+2, index].Children[0] as GradientSample;
						gs.Gradient = prop;
					}
					else
					{
						this.table[p+2, index] = new Cell();  // cellule vide
					}

					this.table.SelectRow(index, obj.IsSelected());
				}
			}
		}

		// Ligne de la table sélectionnée ou déselectionnée.
		private void TableSelectionChanged(object sender)
		{
			for ( int i=0 ; i<this.table.Rows ; i++ )
			{
				this.objects[i].SelectObject(this.table.IsCellSelected(i, 0));
			}

			this.OnPanelChanged();
		}

		// Génère un événement pour dire qu'il faut changer les panneaux.
		protected virtual void OnPanelChanged()
		{
			if ( this.PanelChanged != null )  // qq'un écoute ?
			{
				this.PanelChanged(this);
			}
		}

		public event EventHandler PanelChanged;


		protected CellTable			table;
		protected IconObjects		objects;
	}
}
