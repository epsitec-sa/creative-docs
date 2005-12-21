using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe Lister permet de représenter un tableau des icônes.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class Lister : Epsitec.Common.Widgets.Widget
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

			this.iconObjects = new IconObjects();
		}
		
		public Lister(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public IconObjects IconObjects
		{
			//	Liste des objets.
			get
			{
				return this.iconObjects;
			}

			set
			{
				this.iconObjects = value;
			}
		}

		public override void Invalidate()
		{
			this.UpdateContent();  // TODO: faudrait éviter de refaire toute la liste !!!
			base.Invalidate();
		}

		public void UpdateContent()
		{
			//	Met à jour le contenu du tableau.
			if ( this.iconObjects == null )  return;
			if ( !this.IsVisible )  return;

			ObjectMemory memo = new ObjectMemory();

			int ir = this.table.Columns;
			int tp = memo.TotalProperty;
			int total = this.iconObjects.Count;
			int rows = this.table.Rows;
			this.table.SetArraySize(tp+4, total);

			if ( ir != tp+4 )
			{
				this.table.SetWidthColumn(0, 35);
				this.table.SetWidthColumn(1, 35);
				this.table.SetWidthColumn(2, 40);
				this.table.SetWidthColumn(3, 40);

				for ( int p=0 ; p<tp; p++ )
				{
					this.table.SetWidthColumn(p+4, 50);
				}
			}

			if ( ir != tp+4 || total != rows )
			{
				this.table.SetHeaderTextH(0, "Rang");
				this.table.SetHeaderTextH(1, "Type");
				this.table.SetHeaderTextH(2, "Etat");
				this.table.SetHeaderTextH(3, "Nom");

				for ( int p=0 ; p<tp; p++ )
				{
					AbstractProperty property = memo.Property(p);
					this.table.SetHeaderTextH(p+4, property.Text);
				}
			}

			StaticText		st;
			IconButton		ib;
			ColorSample		cs;
			GradientSample	gs;

			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = this.iconObjects[index];

				if ( this.table[0, index].IsEmpty )
				{
					st = new StaticText();
					st.Alignment = Drawing.ContentAlignment.MiddleCenter;
					st.Dock = DockStyle.Fill;
					this.table[0, index].Insert(st);
				}
				st = this.table[0, index].Children[0] as StaticText;
				st.Text = System.Convert.ToString(index+1);

				if ( this.table[1, index].IsEmpty )
				{
					ib = new IconButton();
					ib.SetFrozen(true);
					ib.Dock = DockStyle.Fill;
					this.table[1, index].Insert(ib);
				}
				ib = this.table[1, index].Children[0] as IconButton;
				ib.IconName = obj.IconName;

				if ( this.table[2, index].IsEmpty )
				{
					st = new StaticText();
					st.Alignment = Drawing.ContentAlignment.MiddleCenter;
					st.Dock = DockStyle.Fill;
					this.table[2, index].Insert(st);
				}
				st = this.table[2, index].Children[0] as StaticText;
				st.Text = obj.IsHide ? "Caché" : "Visible";

				if ( this.table[3, index].IsEmpty )
				{
					st = new StaticText();
					st.Alignment = Drawing.ContentAlignment.MiddleCenter;
					st.Dock = DockStyle.Fill;
					this.table[3, index].Insert(st);
				}
				PropertyName name = obj.GetProperty(PropertyType.Name) as PropertyName;
				if ( name != null )
				{
					st = this.table[3, index].Children[0] as StaticText;
					st.Text = name.String;
				}

				for ( int p=0 ; p<tp; p++ )
				{
					AbstractProperty property = memo.Property(p);
					AbstractProperty po = obj.GetProperty(property.Type);

					if ( po is PropertyName )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(st);
						}
						PropertyName prop = po as PropertyName;
						st = this.table[p+4, index].Children[0] as StaticText;
						st.Text = prop.String;
					}
					else if ( po is PropertyBool )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(st);
						}
						PropertyBool prop = po as PropertyBool;
						st = this.table[p+4, index].Children[0] as StaticText;
						if ( prop.Bool )  st.Text = "Oui";
						else              st.Text = "Non";
					}
					else if ( po is PropertyDouble )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(st);
						}
						PropertyDouble prop = po as PropertyDouble;
						st = this.table[p+4, index].Children[0] as StaticText;
						st.Text = System.Convert.ToString(prop.Value);
					}
					else if ( po is PropertyLine )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(st);
						}
						PropertyLine prop = po as PropertyLine;
						st = this.table[p+4, index].Children[0] as StaticText;
						st.Text = System.Convert.ToString(prop.Width);
					}
					else if ( po is PropertyString )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(st);
						}
						PropertyString prop = po as PropertyString;
						st = this.table[p+4, index].Children[0] as StaticText;
						st.Text = prop.String;
					}
					else if ( po is PropertyList )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(st);
						}
						PropertyList prop = po as PropertyList;
						st = this.table[p+4, index].Children[0] as StaticText;
						st.Text = prop.GetListName();
					}
					else if ( po is PropertyCombo )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(st);
						}
						PropertyCombo prop = po as PropertyCombo;
						st = this.table[p+4, index].Children[0] as StaticText;
						st.Text = prop.GetListName();
					}
					else if ( po is PropertyColor )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							cs = new ColorSample();
							cs.SetFrozen(true);
							cs.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(cs);
						}
						PropertyColor prop = po as PropertyColor;
						cs = this.table[p+4, index].Children[0] as ColorSample;
						cs.Color = prop.Color;
					}
					else if ( po is PropertyGradient )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							gs = new GradientSample();
							gs.SetFrozen(true);
							gs.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(gs);
						}
						PropertyGradient prop = po as PropertyGradient;
						gs = this.table[p+4, index].Children[0] as GradientSample;
						gs.Gradient = prop;
					}
					else if ( po is PropertyArrow )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(st);
						}
						PropertyArrow prop = po as PropertyArrow;
						st = this.table[p+4, index].Children[0] as StaticText;
						st.Text = prop.GetListName();
					}
					else if ( po is PropertyCorner )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(st);
						}
						PropertyCorner prop = po as PropertyCorner;
						st = this.table[p+4, index].Children[0] as StaticText;
						st.Text = prop.GetListName();
					}
					else if ( po is PropertyRegular )
					{
						if ( this.table[p+4, index].IsEmpty )
						{
							st = new StaticText();
							st.Alignment = Drawing.ContentAlignment.MiddleCenter;
							st.Dock = DockStyle.Fill;
							this.table[p+4, index].Insert(st);
						}
						PropertyRegular prop = po as PropertyRegular;
						st = this.table[p+4, index].Children[0] as StaticText;
						st.Text = prop.GetListName();
					}
					else
					{
						this.table[p+4, index].Clear ();	// cellule vide
					}

					this.table.SelectRow(index, obj.IsSelected());
				}
			}
		}

		private void TableSelectionChanged(object sender)
		{
			//	Ligne de la table sélectionnée ou déselectionnée.
			for ( int i=0 ; i<this.table.Rows ; i++ )
			{
				this.iconObjects[i].Select(this.table.IsCellSelected(i, 0));
			}

			this.OnPanelChanged();
		}

		protected virtual void OnPanelChanged()
		{
			//	Génère un événement pour dire qu'il faut changer les panneaux.
			if ( this.PanelChanged != null )  // qq'un écoute ?
			{
				this.PanelChanged(this);
			}
		}

		public event EventHandler PanelChanged;


		public override Drawing.Rectangle GetShapeBounds()
		{
			if ( table == null )
			{
				return new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			}
			else
			{
				return table.GetShapeBounds();
			}
		}

		
		protected CellTable			table;
		protected IconObjects		iconObjects;
	}
}
