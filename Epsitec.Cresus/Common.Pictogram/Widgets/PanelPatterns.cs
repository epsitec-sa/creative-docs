using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelPatterns permet de représenter tous les patterns.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelPatterns : Epsitec.Common.Widgets.Widget
	{
		public PanelPatterns(Drawer drawer, ToolTip toolTip)
		{
			this.drawer = drawer;
			this.toolTip = toolTip;

			this.radioDocument = new RadioButton(this);
			this.radioDocument.Text = "Document";
			this.radioDocument.Clicked += new MessageEventHandler(this.HandleRadio);

			this.radioPattern = new RadioButton(this);
			this.radioPattern.Text = "Motifs :";
			this.radioPattern.Clicked += new MessageEventHandler(this.HandleRadio);

			this.toolBar = new HToolBar(this);

			this.buttonNew = new IconButton(@"file:images/patternnew.icon");
			this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNew);
			this.toolBar.Items.Add(this.buttonNew);
			this.toolTip.SetToolTip(this.buttonNew, "Nouveau motif");

			this.buttonDuplicate = new IconButton(@"file:images/duplicate.icon");
			this.buttonDuplicate.Clicked += new MessageEventHandler(this.HandleButtonDuplicate);
			this.toolBar.Items.Add(this.buttonDuplicate);
			this.toolTip.SetToolTip(this.buttonDuplicate, "Dupliquer le motif");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton(@"file:images/up.icon");
			this.buttonUp.Clicked += new MessageEventHandler(this.HandleButtonUp);
			this.toolBar.Items.Add(this.buttonUp);
			this.toolTip.SetToolTip(this.buttonUp, "Motif avant");

			this.buttonDown = new IconButton(@"file:images/down.icon");
			this.buttonDown.Clicked += new MessageEventHandler(this.HandleButtonDown);
			this.toolBar.Items.Add(this.buttonDown);
			this.toolTip.SetToolTip(this.buttonDown, "Motif après");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton(@"file:images/delete.icon");
			this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonDelete);
			this.toolBar.Items.Add(this.buttonDelete);
			this.toolTip.SetToolTip(this.buttonDelete, "Supprimer le motif");

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
				this.radioDocument.Clicked -= new MessageEventHandler(this.HandleRadio);
				this.radioPattern.Clicked -= new MessageEventHandler(this.HandleRadio);
				this.buttonNew.Clicked -= new MessageEventHandler(this.HandleButtonNew);
				this.buttonDuplicate.Clicked -= new MessageEventHandler(this.HandleButtonDuplicate);
				this.buttonUp.Clicked -= new MessageEventHandler(this.HandleButtonUp);
				this.buttonDown.Clicked -= new MessageEventHandler(this.HandleButtonDown);
				this.buttonDelete.Clicked -= new MessageEventHandler(this.HandleButtonDelete);
			}
			
			base.Dispose(disposing);
		}


		// Met à jour tout le panneau.
		public void Update()
		{
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPattern, true, false);
			this.UpdateToolBar();
		}

		// Sélectionne un pattern.
		public void PatternSelect(int sel)
		{
			this.drawer.CreateEnding();
			this.drawer.IconObjects.CurrentPattern = sel;
			this.TableSelect(sel, true, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}


		// Met à jour les boutons de la toolbar.
		protected void UpdateToolBar()
		{
			int total = this.table.Rows;
			int sel = this.TableSelect();

			this.buttonDuplicate.SetEnabled(sel != -1);
			this.buttonUp.SetEnabled(sel != -1 && sel > 1 && total > 1);
			this.buttonDown.SetEnabled(sel != -1 && sel < total && total > 1);
			this.buttonDelete.SetEnabled(sel != -1 && total != 0);
		}

		// Bouton radio cliqué.
		private void HandleRadio(object sender, MessageEventArgs e)
		{
			RadioButton radio = sender as RadioButton;
			int sel = (radio == this.radioDocument) ? 0:this.lastSelectedPattern;
			this.TableSelect(sel, true, false);
			if ( sel >= this.drawer.IconObjects.TotalPatterns() )  sel = 0;
			this.drawer.IconObjects.CurrentPattern = sel;
			this.UpdateToolBar();
			this.OnObjectsChanged();
			this.drawer.OnInfoDocumentChanged();
		}

		// Crée un nouveau pattern.
		private void HandleButtonNew(object sender, MessageEventArgs e)
		{
			this.drawer.IconObjects.CreatePattern(this.drawer.IconObjects.CurrentPattern, false);
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPattern, true, true);
			this.UpdateToolBar();
			this.OnObjectsChanged();
			this.drawer.OnInfoDocumentChanged();
		}

		// Duplique un pattern.
		private void HandleButtonDuplicate(object sender, MessageEventArgs e)
		{
			this.drawer.IconObjects.CreatePattern(this.drawer.IconObjects.CurrentPattern, true);
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPattern, true, true);
			this.UpdateToolBar();
			this.OnObjectsChanged();
			this.drawer.OnInfoDocumentChanged();
		}

		// Monte d'une ligne le pattern sélectionné.
		private void HandleButtonUp(object sender, MessageEventArgs e)
		{
			int sel = this.TableSelect();
			this.drawer.IconObjects.SwapPattern(sel, sel-1);
			this.drawer.IconObjects.CurrentPattern = sel-1;
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPattern, true, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Descend d'une ligne le pattern sélectionné.
		private void HandleButtonDown(object sender, MessageEventArgs e)
		{
			int sel = this.TableSelect();
			this.drawer.IconObjects.SwapPattern(sel, sel+1);
			this.drawer.IconObjects.CurrentPattern = sel+1;
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPattern, true, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Supprime le pattern sélectionné.
		private void HandleButtonDelete(object sender, MessageEventArgs e)
		{
			int sel = this.TableSelect();
			this.drawer.IconObjects.DeletePattern(sel);
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentPattern, true, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
			this.drawer.OnInfoDocumentChanged();
		}


		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			int rows = this.drawer.IconObjects.TotalPatterns();
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(2, rows-1);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 60);
				this.table.SetWidthColumn(1, 134);
			}

			this.table.SetHeaderTextH(0, "Rang");
			this.table.SetHeaderTextH(1, "Nom");

			StaticText		st;
			TextField		edit;

			for ( int i=0 ; i<rows-1 ; i++ )
			{
				ObjectPattern pattern = this.drawer.IconObjects.Pattern(i+1);

				if ( this.table[0, i].IsEmpty )
				{
					st = new StaticText();
					st.Alignment = Drawing.ContentAlignment.MiddleCenter;
					st.Dock = DockStyle.Fill;
					this.table[0, i].Insert(st);
				}
				st = this.table[0, i].Children[0] as StaticText;
				st.Text = (i+1).ToString();

				if ( this.table[1, i].IsEmpty )
				{
					edit = new TextField();
					edit.Dock = DockStyle.Fill;
					edit.Name = (i+1).ToString();
					edit.Clicked += new MessageEventHandler(this.HandleListTextClicked);
					edit.TextChanged += new EventHandler(this.HandleListTextChanged);
					this.table[1, i].Insert(edit);
				}
				edit = this.table[1, i].Children[0] as TextField;
				this.ignoreListTextChanged = true;
				edit.Text = pattern.Name;
				this.ignoreListTextChanged = false;
			}

			this.UpdateToolBar();
		}

		// Sélectionne une ligne dans la table.
		protected void TableSelect(int sel, bool showSelect, bool selectText)
		{
			this.radioDocument.ActiveState = (sel == 0) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioPattern .ActiveState = (sel != 0) ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			int total = this.table.Rows;
			bool exist = false;
			for ( int i=0 ; i<total ; i++ )
			{
				this.table.SelectRow(i, i==sel-1);
				exist |= (i==sel-1);
			}

			if ( exist )
			{
				if ( showSelect )  this.table.ShowSelect();
				this.table.Update();

				Cell cell = this.table[1, sel-1];
				TextField edit = cell.Children[0] as TextField;
				if ( selectText )  edit.SelectAll();
				if ( edit.IsVisible)  edit.Focus();
			}

			this.toolBar.SetEnabled(sel != 0);
			this.table.SetEnabled(sel != 0);

			if ( sel != 0 )  this.lastSelectedPattern = sel;
		}

		// Retourne la ligne sélectionnée dans la table.
		protected int TableSelect()
		{
			if ( this.radioDocument.ActiveState == WidgetState.ActiveYes )
			{
				return 0;
			}

			int total = this.table.Rows;
			for ( int i=0 ; i<total ; i++ )
			{
				if ( this.table.IsCellSelected(i, 0) )  return i+1;
			}
			return -1;
		}

		// Liste cliquée.
		private void HandleTableSelectionChanged(object sender)
		{
			int sel = this.TableSelect();
			if ( sel == -1 )  return;
			this.TableSelect(sel, false, true);
			this.drawer.IconObjects.CurrentPattern = sel;
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Ligne éditable dans la liste cliquée.
		private void HandleListTextClicked(object sender, MessageEventArgs e)
		{
			TextField edit = sender as TextField;
			int sel = System.Convert.ToInt32(edit.Name);
			this.drawer.IconObjects.CurrentPattern = sel;
			this.TableSelect(this.drawer.IconObjects.CurrentPattern, false, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Texte de la ligne éditable dans la liste changé.
		private void HandleListTextChanged(object sender)
		{
			if ( this.ignoreListTextChanged )  return;
			TextField edit = sender as TextField;
			int sel = System.Convert.ToInt32(edit.Name);
			ObjectPattern pattern = this.drawer.IconObjects.Pattern(sel);
			pattern.Name = edit.Text;
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

			Drawing.Rectangle r = this.Client.Bounds;

			r.Left += 20;
			r.Bottom = r.Top-this.radioDocument.DefaultHeight;
			this.radioDocument.Bounds = r;

			r.Top = r.Bottom;
			r.Bottom = r.Top-this.radioDocument.DefaultHeight;
			this.radioPattern.Bounds = r;

			r.Left -= 20;
			r.Top = r.Bottom-6;
			r.Bottom = r.Top-this.toolBar.DefaultHeight;
			this.toolBar.Bounds = r;

			r.Top = r.Bottom+1;  // chevauchement d'un pixel
			r.Bottom = r.Top-300;
			this.table.Bounds = r;
		}


		protected RadioButton					radioDocument;
		protected RadioButton					radioPattern;
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
		protected int							lastSelectedPattern = 1;
	}
}
