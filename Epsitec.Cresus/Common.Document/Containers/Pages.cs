using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Pages contient tous les panneaux des pages.
	/// </summary>
	[SuppressBundleSupport]
	public class Pages : Abstract
	{
		public Pages(Document document) : base(document)
		{
			this.toolBar = new HToolBar(this);
			this.toolBar.Dock = DockStyle.Top;
			this.toolBar.DockMargins = new Margins(0, 0, 0, -1);

			this.buttonNew = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.PageNew.icon");
			this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNew);
			this.toolBar.Items.Add(this.buttonNew);
			ToolTip.Default.SetToolTip(this.buttonNew, "Nouvelle page <b>après</b> la page courante");

			this.buttonDuplicate = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.Duplicate.icon");
			this.buttonDuplicate.Clicked += new MessageEventHandler(this.HandleButtonDuplicate);
			this.toolBar.Items.Add(this.buttonDuplicate);
			ToolTip.Default.SetToolTip(this.buttonDuplicate, "Dupliquer la page");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.Up.icon");
			this.buttonUp.Clicked += new MessageEventHandler(this.HandleButtonUp);
			this.toolBar.Items.Add(this.buttonUp);
			ToolTip.Default.SetToolTip(this.buttonUp, "Page avant");

			this.buttonDown = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.Down.icon");
			this.buttonDown.Clicked += new MessageEventHandler(this.HandleButtonDown);
			this.toolBar.Items.Add(this.buttonDown);
			ToolTip.Default.SetToolTip(this.buttonDown, "Page après");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton("manifest:Epsitec.App.DocumentEditor.Images.Delete.icon");
			this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonDelete);
			this.toolBar.Items.Add(this.buttonDelete);
			ToolTip.Default.SetToolTip(this.buttonDelete, "Supprimer la page");

			this.table = new CellTable(this);
			this.table.Dock = DockStyle.Fill;
			this.table.SelectionChanged += new EventHandler(this.HandleTableSelectionChanged);
			this.table.StyleH  = CellArrayStyle.ScrollNorm;
			this.table.StyleH |= CellArrayStyle.Header;
			this.table.StyleH |= CellArrayStyle.Separator;
			this.table.StyleH |= CellArrayStyle.Mobile;
			this.table.StyleV  = CellArrayStyle.ScrollNorm;
			this.table.StyleV |= CellArrayStyle.Separator;
			this.table.StyleV |= CellArrayStyle.SelectLine;
			this.table.DefHeight = 16;

			this.panelPageName = new Panels.PageName(this.document);
			this.panelPageName.IsExtendedSize = false;
			this.panelPageName.IsLayoutDirect = true;
			this.panelPageName.TabIndex = 100;
			this.panelPageName.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			this.panelPageName.Dock = DockStyle.Bottom;
			this.panelPageName.DockMargins = new Margins(0, 0, 5, 0);
			this.panelPageName.Parent = this;
		}
		

		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			this.UpdateTable();
			this.UpdatePanel();
			this.UpdateToolBar();
		}

		// Effectue la mise à jour d'un objet.
		protected override void DoUpdateObject(Objects.Abstract obj)
		{
			Objects.Page page = obj as Objects.Page;
			UndoableList pages = this.document.GetObjects;
			int rank = pages.IndexOf(obj);
			this.TableUpdateRow(rank, page);
		}

		// Met à jour les boutons de la toolbar.
		protected void UpdateToolBar()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int total = this.table.Rows;
			int sel = context.CurrentPage;

			this.buttonDuplicate.SetEnabled(sel != -1);
			this.buttonUp.SetEnabled(sel != -1 && sel > 0);
			this.buttonDown.SetEnabled(sel != -1 && sel < total-1);
			this.buttonDelete.SetEnabled(sel != -1 && total > 1);
		}

		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;

			int rows = context.TotalPages();
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(2, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 40);
				this.table.SetWidthColumn(1, 172);
			}

			this.table.SetHeaderTextH(0, "No");
			this.table.SetHeaderTextH(1, "Nom");

			UndoableList doc = this.document.GetObjects;
			for ( int i=0 ; i<rows ; i++ )
			{
				Objects.Page page = doc[i] as Objects.Page;
				this.TableFillRow(i);
				this.TableUpdateRow(i, page);
			}
		}

		// Peuple une ligne de la table, si nécessaire.
		protected void TableFillRow(int row)
		{
			for ( int column=0 ; column<this.table.Columns ; column++ )
			{
				if ( this.table[column, row].IsEmpty )
				{
					StaticText st = new StaticText();
					st.Alignment = (column==0) ? ContentAlignment.MiddleCenter : ContentAlignment.MiddleLeft;
					st.Dock = DockStyle.Fill;
					st.DockMargins = new Margins(4, 4, 0, 0);
					this.table[column, row].Insert(st);
				}
			}
		}

		// Met à jour le contenu d'une ligne de la table.
		protected void TableUpdateRow(int row, Objects.Page page)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			StaticText st;

			st = this.table[0, row].Children[0] as StaticText;
			st.Text = (row+1).ToString();

			st = this.table[1, row].Children[0] as StaticText;
			st.Text = page.Name;

			this.table.SelectRow(row, row==context.CurrentPage);
		}

		// Met à jour le panneau pour éditer la propriété sélectionnée.
		protected void UpdatePanel()
		{
			this.panelPageName.UpdateValues();
		}


		// Crée une nouvelle page.
		private void HandleButtonNew(object sender, MessageEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;
			this.document.Modifier.PageCreate(sel+1, "");
		}

		// Duplique une page.
		private void HandleButtonDuplicate(object sender, MessageEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;
			this.document.Modifier.PageDuplicate(sel, "");
		}

		// Monte d'une ligne la page sélectionnée.
		private void HandleButtonUp(object sender, MessageEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;
			this.document.Modifier.PageSwap(sel, sel-1);
		}

		// Descend d'une ligne la page sélectionnée.
		private void HandleButtonDown(object sender, MessageEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;
			this.document.Modifier.PageSwap(sel, sel+1);
		}

		// Supprime la page sélectionnée.
		private void HandleButtonDelete(object sender, MessageEventArgs e)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentPage;
			this.document.Modifier.PageDelete(sel);
		}

		// Liste cliquée.
		private void HandleTableSelectionChanged(object sender)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			context.CurrentPage = this.table.SelectedRow;

			this.UpdateToolBar();
		}


		protected HToolBar				toolBar;
		protected IconButton			buttonNew;
		protected IconButton			buttonDuplicate;
		protected IconButton			buttonUp;
		protected IconButton			buttonDown;
		protected IconButton			buttonDelete;
		protected CellTable				table;
		protected Panels.PageName		panelPageName;
		protected bool					ignoreChanged = false;
	}
}
