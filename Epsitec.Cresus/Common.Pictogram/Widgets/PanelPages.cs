using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelPages permet de représenter toutes les pages.
	/// </summary>
	public class PanelPages : Epsitec.Common.Widgets.Widget
	{
		public PanelPages(Drawer drawer, ToolTip toolTip)
		{
			this.drawer = drawer;
			this.toolTip = toolTip;

			this.toolBar = new HToolBar(this);

			this.buttonNew = new IconButton(@"file:images/pagenew.icon");
			this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNew);
			this.toolBar.Items.Add(this.buttonNew);
			this.toolTip.SetToolTip(this.buttonNew, "Nouvelle page vide");

			this.buttonDuplicate = new IconButton(@"file:images/duplicate.icon");
			this.buttonDuplicate.Clicked += new MessageEventHandler(this.HandleButtonDuplicate);
			this.toolBar.Items.Add(this.buttonDuplicate);
			this.toolTip.SetToolTip(this.buttonDuplicate, "Dupliquer la page");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton(@"file:images/up.icon");
			this.buttonUp.Clicked += new MessageEventHandler(this.HandleButtonUp);
			this.toolBar.Items.Add(this.buttonUp);
			this.toolTip.SetToolTip(this.buttonUp, "Page avant");

			this.buttonDown = new IconButton(@"file:images/down.icon");
			this.buttonDown.Clicked += new MessageEventHandler(this.HandleButtonDown);
			this.toolBar.Items.Add(this.buttonDown);
			this.toolTip.SetToolTip(this.buttonDown, "Page après");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton(@"file:images/delete.icon");
			this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonDelete);
			this.toolBar.Items.Add(this.buttonDelete);
			this.toolTip.SetToolTip(this.buttonDelete, "Supprimer la page");

			this.table = new CellTable(this);
			this.table.SelectionChanged += new EventHandler(this.HandleTableSelectionChanged);
			this.table.StyleH  = CellArrayStyle.ScrollNorm;
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleH |= CellArrayStyle.Mobile;
			this.table.StyleV  = CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.SelectLine;
			this.table.DefHeight = 20;
		}
		

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		// Met à jour tout le panneau.
		public void Update()
		{
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPage, true, false);
			this.UpdateToolBar();
		}

		// Sélectionne une page.
		public void PageSelect(int sel)
		{
			this.drawer.IconObjects.CurrentPage = sel;
			this.TableSelect(sel, true, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Construit le menu pour choisir une page.
		public VMenu CreateMenu()
		{
			VMenu menu = new VMenu();
			int total = this.drawer.IconObjects.TotalPages();
			for ( int i=0 ; i<total ; i++ )
			{
				ObjectPage page = this.drawer.IconObjects.Page(i);

				string name = string.Format("{0}: {1}", (i+1).ToString(), page.Name);

				string icon = @"file:images/activeno.icon";
				if ( i == this.drawer.IconObjects.CurrentPage )
				{
					icon = @"file:images/activeyes.icon";
				}

				MenuItem item = new MenuItem("SelectPage(this.Name)", icon, name, "", i.ToString());
				menu.Items.Add(item);
			}
			menu.AdjustSize();
			return menu;
		}


		// Met à jour les boutons de la toolbar.
		protected void UpdateToolBar()
		{
			int total = this.table.Rows;
			int sel = this.TableSelect();

			this.buttonUp.SetEnabled(sel != -1 && sel > 0);
			this.buttonDuplicate.SetEnabled(sel != -1);
			this.buttonDown.SetEnabled(sel != -1 && sel < total-1);
			this.buttonDelete.SetEnabled(sel != -1 && total > 1);
		}

		// Crée une nouvelle page.
		private void HandleButtonNew(object sender, MessageEventArgs e)
		{
			this.drawer.IconObjects.CreatePage(this.drawer.IconObjects.CurrentPage, false);
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPage, true, true);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Duplique une page.
		private void HandleButtonDuplicate(object sender, MessageEventArgs e)
		{
			this.drawer.IconObjects.CreatePage(this.drawer.IconObjects.CurrentPage, true);
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPage, true, true);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Monte d'une ligne la page sélectionnée.
		private void HandleButtonUp(object sender, MessageEventArgs e)
		{
			int sel = this.TableSelect();
			this.drawer.IconObjects.SwapPage(sel, sel-1);
			this.drawer.IconObjects.CurrentPage = sel-1;
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPage, true, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Descend d'une ligne la page sélectionnée.
		private void HandleButtonDown(object sender, MessageEventArgs e)
		{
			int sel = this.TableSelect();
			this.drawer.IconObjects.SwapPage(sel, sel+1);
			this.drawer.IconObjects.CurrentPage = sel+1;
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPage, true, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Supprime la page sélectionnée.
		private void HandleButtonDelete(object sender, MessageEventArgs e)
		{
			int sel = this.TableSelect();
			this.drawer.IconObjects.DeletePage(sel);
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPage, true, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}


		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			int rows = this.drawer.IconObjects.TotalPages();
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(2, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 60);
				this.table.SetWidthColumn(1, 134);
			}

			this.table.SetHeaderTextH(0, "Numéro");
			this.table.SetHeaderTextH(1, "Nom");

			Cell			cell;
			StaticText		st;
			TextField		edit;

			for ( int i=0 ; i<rows ; i++ )
			{
				ObjectPage page = this.drawer.IconObjects.Page(i);

				if ( this.table[0, i].Children.Count == 0 )
				{
					cell = new Cell();
					st = new StaticText();
					st.Alignment = Drawing.ContentAlignment.MiddleCenter;
					st.Dock = DockStyle.Fill;
					cell.Children.Add(st);
					this.table[0, i] = cell;
				}
				st = this.table[0, i].Children[0] as StaticText;
				st.Text = (i+1).ToString();

				if ( this.table[1, i].Children.Count == 0 )
				{
					cell = new Cell();
					edit = new TextField();
					edit.Dock = DockStyle.Fill;
					edit.Name = i.ToString();
					edit.Clicked += new MessageEventHandler(this.HandleListTextClicked);
					edit.TextChanged += new EventHandler(this.HandleListTextChanged);
					cell.Children.Add(edit);
					this.table[1, i] = cell;
				}
				edit = this.table[1, i].Children[0] as TextField;
				this.ignoreListTextChanged = true;
				edit.Text = page.Name;
				this.ignoreListTextChanged = false;
			}

			this.UpdateToolBar();
		}

		// Sélectionne une ligne dans la table.
		protected void TableSelect(int sel, bool showSelect, bool selectText)
		{
			int total = this.table.Rows;
			bool exist = false;
			for ( int i=0 ; i<total ; i++ )
			{
				this.table.SelectRow(i, i==sel);
				this.table.SelectCell(1, i, false);  // à cause du TextField
				exist |= (i==sel);
			}

			if ( exist )
			{
				if ( showSelect )  this.table.ShowSelect();

				Cell cell = this.table[1, sel];
				TextField edit = cell.Children[0] as TextField;
				if ( selectText )  edit.SelectAll();
				if ( edit.IsVisible)  edit.SetFocused(true);
			}
		}

		// Retourne la ligne sélectionnée dans la table.
		protected int TableSelect()
		{
			int total = this.table.Rows;
			for ( int i=0 ; i<total ; i++ )
			{
				if ( this.table.IsCellSelected(i, 0) )  return i;
			}
			return -1;
		}

		// Liste cliquée.
		private void HandleTableSelectionChanged(object sender)
		{
			int sel = this.TableSelect();
			if ( sel == -1 )  return;
			this.TableSelect(sel, false, true);
			this.drawer.IconObjects.CurrentPage = sel;
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Ligne éditable dans la liste cliquée.
		private void HandleListTextClicked(object sender, MessageEventArgs e)
		{
			TextField edit = sender as TextField;
			int sel = System.Convert.ToInt32(edit.Name);
			this.drawer.IconObjects.CurrentPage = sel;
			this.TableSelect(this.drawer.IconObjects.CurrentPage, false, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Texte de la ligne éditable dans la liste changé.
		private void HandleListTextChanged(object sender)
		{
			if ( this.ignoreListTextChanged )  return;
			TextField edit = sender as TextField;
			int sel = System.Convert.ToInt32(edit.Name);
			ObjectPage page = this.drawer.IconObjects.Page(sel);
			page.Name = edit.Text;
		}


		// Génère un événement pour dire qu'il faut changer les objets affichés.
		protected void OnObjectsChanged()
		{
			if ( this.ObjectsChanged != null )  // qq'un écoute ?
			{
				this.ObjectsChanged(this);
			}
		}

		public event EventHandler ObjectsChanged;


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.table == null )  return;

			double dy = this.toolBar.DefaultHeight;

			Drawing.Rectangle rect = this.Client.Bounds;
			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-dy;
			this.toolBar.Bounds = r;

			r.Top = r.Bottom+1;  // chevauchement d'un pixel
			r.Bottom = r.Top-300;
			this.table.Bounds = r;
		}


		protected HToolBar						toolBar;
		protected ToolTip						toolTip;
		protected IconButton					buttonNew;
		protected IconButton					buttonDuplicate;
		protected IconButton					buttonUp;
		protected IconButton					buttonDown;
		protected IconButton					buttonDelete;
		protected CellTable						table;
		protected Drawer						drawer;
		protected bool							ignoreListTextChanged = false;
	}
}
