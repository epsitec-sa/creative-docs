using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelLayers permet de représenter tous les calques d'une page.
	/// </summary>
	public class PanelLayers : Epsitec.Common.Widgets.Widget
	{
		public PanelLayers(Drawer drawer, ToolTip toolTip)
		{
			this.drawer = drawer;
			this.toolTip = toolTip;

			this.toolBar = new HToolBar(this);

			this.buttonNew = new IconButton(@"file:images/layernew.icon");
			this.buttonNew.Clicked += new MessageEventHandler(this.HandleButtonNew);
			this.toolBar.Items.Add(this.buttonNew);
			this.toolTip.SetToolTip(this.buttonNew, "Nouveau calque vide");

			this.buttonDuplicate = new IconButton(@"file:images/duplicate.icon");
			this.buttonDuplicate.Clicked += new MessageEventHandler(this.HandleButtonDuplicate);
			this.toolBar.Items.Add(this.buttonDuplicate);
			this.toolTip.SetToolTip(this.buttonDuplicate, "Dupliquer le calque");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonUp = new IconButton(@"file:images/up.icon");
			this.buttonUp.Clicked += new MessageEventHandler(this.HandleButtonUp);
			this.toolBar.Items.Add(this.buttonUp);
			this.toolTip.SetToolTip(this.buttonUp, "Calque dessus");

			this.buttonDown = new IconButton(@"file:images/down.icon");
			this.buttonDown.Clicked += new MessageEventHandler(this.HandleButtonDown);
			this.toolBar.Items.Add(this.buttonDown);
			this.toolTip.SetToolTip(this.buttonDown, "Calque dessous");

			this.toolBar.Items.Add(new IconSeparator());

			this.buttonDelete = new IconButton(@"file:images/delete.icon");
			this.buttonDelete.Clicked += new MessageEventHandler(this.HandleButtonDelete);
			this.toolBar.Items.Add(this.buttonDelete);
			this.toolTip.SetToolTip(this.buttonDelete, "Supprimer le calque");

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

			this.radioGroup = new GroupBox(this);
			this.radioGroup.Text = "Si le calque est inactif :";

			this.radioShow = new RadioButton(this.radioGroup);
			this.radioShow.Text = "Afficher normalement";
			this.radioShow.Name = "RadioShow";
			this.radioShow.Clicked += new MessageEventHandler(this.HandleRadioClicked);

			this.radioDimmed = new RadioButton(this.radioGroup);
			this.radioDimmed.Text = "Afficher estompé";
			this.radioDimmed.Name = "RadioDimmed";
			this.radioDimmed.Clicked += new MessageEventHandler(this.HandleRadioClicked);

			this.radioHide = new RadioButton(this.radioGroup);
			this.radioHide.Text = "Cacher";
			this.radioHide.Name = "RadioHide";
			this.radioHide.Clicked += new MessageEventHandler(this.HandleRadioClicked);

			this.buttonShow = new Button(this);
			this.buttonShow.Text = "Afficher normalement tous les calques";
			this.buttonShow.Name = "ButtonShow";
			this.buttonShow.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonDimmed = new Button(this);
			this.buttonDimmed.Text = "Afficher estompé les autres calques";
			this.buttonDimmed.Name = "ButtonDimmed";
			this.buttonDimmed.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonHide = new Button(this);
			this.buttonHide.Text = "Cacher les autres calques";
			this.buttonHide.Name = "ButtonHide";
			this.buttonHide.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.panel = new Widget(this);
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
			this.TableSelect(this.drawer.IconObjects.CurrentLayer, true, false);
			this.UpdateToolBar();
			this.UpdateRadio();
			this.UpdatePanels();
		}

		// Sélectionne un calque.
		public void LayerSelect(int sel)
		{
			this.drawer.CreateEnding();
			this.drawer.IconObjects.CurrentLayer = sel;
			this.TableSelect(sel, true, false);
			this.UpdateRadio();
			this.UpdatePanels();
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Construit le menu pour choisir un calque.
		public VMenu CreateMenu()
		{
			VMenu menu = new VMenu();
			int total = this.drawer.IconObjects.TotalLayers();
			for ( int i=0 ; i<total ; i++ )
			{
				int ii = total-i-1;
				ObjectLayer layer = this.drawer.IconObjects.Layer(ii);

				string name = "";
				if ( layer.Name == "" )
				{
					name = string.Format("{0}: {1}", ((char)('A'+i)).ToString(), this.LayerPosition(ii));
				}
				else
				{
					name = string.Format("{0}: {1}", ((char)('A'+i)).ToString(), layer.Name);
				}

				string icon = @"file:images/activeno.icon";
				if ( ii == this.drawer.IconObjects.CurrentLayer )
				{
					icon = @"file:images/activeyes.icon";
				}

				MenuItem item = new MenuItem("SelectLayer(this.Name)", icon, name, "", ii.ToString());
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

			this.buttonUp.SetEnabled(sel != -1 && sel < total-1);
			this.buttonDuplicate.SetEnabled(sel != -1);
			this.buttonDown.SetEnabled(sel != -1 && sel > 0);
			this.buttonDelete.SetEnabled(sel != -1 && total > 1);
		}

		// Crée un nouveau calque.
		private void HandleButtonNew(object sender, MessageEventArgs e)
		{
			this.drawer.IconObjects.CreateLayer(this.drawer.IconObjects.CurrentLayer, false);
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentLayer, true, true);
			this.UpdateToolBar();
			this.UpdateRadio();
			this.UpdatePanels();
			this.OnObjectsChanged();
		}

		// Duplique un calque.
		private void HandleButtonDuplicate(object sender, MessageEventArgs e)
		{
			this.drawer.IconObjects.CreateLayer(this.drawer.IconObjects.CurrentLayer, true);
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentLayer, true, true);
			this.UpdateToolBar();
			this.UpdateRadio();
			this.UpdatePanels();
			this.OnObjectsChanged();
		}

		// Monte d'une ligne le calque sélectionné.
		private void HandleButtonUp(object sender, MessageEventArgs e)
		{
			int sel = this.TableSelect();
			this.drawer.IconObjects.SwapLayer(sel, sel+1);
			this.drawer.IconObjects.CurrentLayer = sel+1;
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentLayer, true, false);
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Descend d'une ligne le calque sélectionné.
		private void HandleButtonDown(object sender, MessageEventArgs e)
		{
			int sel = this.TableSelect();
			this.drawer.IconObjects.SwapLayer(sel, sel-1);
			this.drawer.IconObjects.CurrentLayer = sel-1;
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentLayer, true, false);
			this.UpdateToolBar();
			this.UpdatePanels();
			this.OnObjectsChanged();
		}

		// Supprime le calque sélectionné.
		private void HandleButtonDelete(object sender, MessageEventArgs e)
		{
			int sel = this.TableSelect();
			this.drawer.IconObjects.DeleteLayer(sel);
			this.UpdateTable();
			this.TableSelect(this.drawer.IconObjects.CurrentLayer, true, false);
			this.UpdateRadio();
			this.UpdatePanels();
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}


		// Retourne la chaîne pour nommer la position d'un calque.
		protected string LayerPosition(int rank)
		{
			int total = this.drawer.IconObjects.TotalLayers();
			if ( total == 1 )
			{
				return "Unique";
			}
			else
			{
				if ( rank == 0 )
				{
					return "Fond";
				}
				else if ( rank < total-1 )
				{
					return string.Format("Fond+{0}", rank.ToString());
				}
				else
				{
					return "Dessus";
				}
			}
		}

		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			int rows = this.drawer.IconObjects.TotalLayers();
			int initialColumns = this.table.Columns;
			this.table.SetArraySize(3, rows);

			if ( initialColumns == 0 )
			{
				this.table.SetWidthColumn(0, 20);
				this.table.SetWidthColumn(1, 60);
				this.table.SetWidthColumn(2, 114);
			}

			this.table.SetHeaderTextH(0, "");
			this.table.SetHeaderTextH(1, "Position");
			this.table.SetHeaderTextH(2, "Nom");

			StaticText		st;
			TextField		edit;

			for ( int i=0 ; i<rows ; i++ )
			{
				int ii = rows-i-1;
				ObjectLayer layer = this.drawer.IconObjects.Layer(ii);

				if ( this.table[0, i].IsEmpty )
				{
					st = new StaticText();
					st.Alignment = Drawing.ContentAlignment.MiddleCenter;
					st.Dock = DockStyle.Fill;
					this.table[0, i].Insert(st);
				}
				st = this.table[0, i].Children[0] as StaticText;
				st.Text = ((char)('A'+i)).ToString();

				if ( this.table[1, i].IsEmpty )
				{
					st = new StaticText();
					st.Alignment = Drawing.ContentAlignment.MiddleCenter;
					st.Dock = DockStyle.Fill;
					this.table[1, i].Insert(st);
				}
				st = this.table[1, i].Children[0] as StaticText;
				st.Text = this.LayerPosition(ii);

				if ( this.table[2, i].IsEmpty )
				{
					edit = new TextField();
					edit.Dock = DockStyle.Fill;
					edit.Clicked += new MessageEventHandler(this.HandleListTextClicked);
					edit.TextChanged += new EventHandler(this.HandleListTextChanged);
					this.table[2, i].Insert(edit);
				}
				edit = this.table[2, i].Children[0] as TextField;
				edit.Name = ii.ToString();
				this.ignoreListTextChanged = true;
				edit.Text = layer.Name;
				this.ignoreListTextChanged = false;
			}

			this.UpdateToolBar();
		}

		// Sélectionne une ligne dans la table.
		protected void TableSelect(int sel, bool showSelect, bool selectText)
		{
			int total = this.table.Rows;
			sel = total-sel-1;
			bool exist = false;
			for ( int i=0 ; i<total ; i++ )
			{
				this.table.SelectRow(i, i==sel);
				exist |= (i==sel);
			}

			if ( exist )
			{
				if ( showSelect )  this.table.ShowSelect();
				this.table.Update();

				Cell cell = this.table[2, sel];
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
				if ( this.table.IsCellSelected(i, 0) )  return total-i-1;
			}
			return -1;
		}

		// Liste cliquée.
		private void HandleTableSelectionChanged(object sender)
		{
			int sel = this.TableSelect();
			if ( sel == -1 )  return;
			this.TableSelect(sel, false, true);
			this.drawer.IconObjects.CurrentLayer = sel;
			this.UpdateRadio();
			this.UpdatePanels();
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Ligne éditable dans la liste cliquée.
		private void HandleListTextClicked(object sender, MessageEventArgs e)
		{
			TextField edit = sender as TextField;
			int sel = System.Convert.ToInt32(edit.Name);
			this.drawer.IconObjects.CurrentLayer = sel;
			this.TableSelect(this.drawer.IconObjects.CurrentLayer, false, false);
			this.UpdateRadio();
			this.UpdatePanels();
			this.UpdateToolBar();
			this.OnObjectsChanged();
		}

		// Texte de la ligne éditable dans la liste changé.
		private void HandleListTextChanged(object sender)
		{
			if ( this.ignoreListTextChanged )  return;
			TextField edit = sender as TextField;
			int sel = System.Convert.ToInt32(edit.Name);
			ObjectLayer layer = this.drawer.IconObjects.Layer(sel);
			layer.Name = edit.Text;
		}


		// Un bouton radio a été cliqué.
		private void HandleRadioClicked(object sender, MessageEventArgs e)
		{
			RadioButton radio = sender as RadioButton;
			LayerType type = LayerType.None;
			if ( radio.Name == "RadioShow"   )  type = LayerType.Show;
			if ( radio.Name == "RadioDimmed" )  type = LayerType.Dimmed;
			if ( radio.Name == "RadioHide"   )  type = LayerType.Hide;
			int sel = this.TableSelect();
			ObjectLayer layer = this.drawer.IconObjects.Layer(sel);
			layer.Type = type;
			this.drawer.InvalidateAll();
		}

		// Met à jour les boutons radio.
		private void UpdateRadio()
		{
			int sel = this.TableSelect();
			ObjectLayer layer = this.drawer.IconObjects.Layer(sel);
			LayerType type = layer.Type;
			this.radioShow.ActiveState   = (type == LayerType.Show  ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioDimmed.ActiveState = (type == LayerType.Dimmed) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.radioHide.ActiveState   = (type == LayerType.Hide  ) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}


		// Un bouton a été cliqué.
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			LayerType type = LayerType.None;
			if ( button.Name == "ButtonShow"   )  type = LayerType.Show;
			if ( button.Name == "ButtonDimmed" )  type = LayerType.Dimmed;
			if ( button.Name == "ButtonHide"   )  type = LayerType.Hide;
			int total = this.drawer.IconObjects.TotalLayers();
			for ( int i=0 ; i<total ; i++ )
			{
				ObjectLayer layer = this.drawer.IconObjects.Layer(i);
				layer.Type = type;
			}
			this.UpdateRadio();
			this.drawer.InvalidateAll();
		}


		// Crée le panneau pour le calque sélectionné.
		protected void UpdatePanels()
		{
			// Supprime tous les panneaux.
			AbstractPanel panel;
			int i = 0;
			while ( i < this.panel.Children.Count )
			{
				panel = this.panel.Children[i] as AbstractPanel;
				if ( panel != null )
				{
					panel.Changed -= new EventHandler(this.HandlePanelChanged);
				}
				this.panel.Children.RemoveAt(i);
			}

			// Crée le panneau pour le calque.
			PropertyModColor modColor = this.drawer.IconObjects.LayerModColor();

			Drawing.Rectangle rect;
			panel = new PanelModColor();
			panel.Drawer = this.drawer;
			panel.ExtendedSize = true;
			panel.SetProperty(modColor);
			panel.LayoutDirect = true;

			rect = new Drawing.Rectangle();
			rect.Left   = 1;
			rect.Right  = this.panel.Width-1;
			rect.Bottom = this.panel.Height-1-panel.DefaultHeight;
			rect.Top    = this.panel.Height-1;
			panel.Bounds = rect;
			panel.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Top;
			panel.Changed += new EventHandler(this.HandlePanelChanged);
			panel.Parent = this.panel;
		}

		// Le contenu du panneau a été changé.
		private void HandlePanelChanged(object sender)
		{
			AbstractPanel panel = sender as AbstractPanel;

			int sel = this.TableSelect();
			ObjectLayer layer = this.drawer.IconObjects.Layer(sel);
			layer.SetProperty(panel.GetProperty());
			this.drawer.InvalidateAll();
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
			r.Bottom = r.Top-160;
			this.table.Bounds = r;

			r.Top = r.Bottom-10;
			r.Bottom = r.Top-this.radioShow.DefaultHeight*3-20;
			this.radioGroup.Bounds = r;

			r.Top = r.Bottom-10;
			r.Bottom = r.Top-this.buttonShow.DefaultHeight;
			this.buttonShow.Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-this.buttonDimmed.DefaultHeight;
			this.buttonDimmed.Bounds = r;

			r.Top = r.Bottom-5;
			r.Bottom = r.Top-this.buttonHide.DefaultHeight;
			this.buttonHide.Bounds = r;

			r.Top = r.Bottom-10;
			r.Bottom = rect.Bottom;
			this.panel.Bounds = r;

			r = this.radioGroup.Client.Bounds;
			r.Deflate(20, 0);
			r.Top -= 16;
			r.Bottom = r.Top-this.radioShow.DefaultHeight;
			this.radioShow.Bounds = r;
			r.Top = r.Bottom;
			r.Bottom = r.Top-this.radioShow.DefaultHeight;
			this.radioDimmed.Bounds = r;
			r.Top = r.Bottom;
			r.Bottom = r.Top-this.radioShow.DefaultHeight;
			this.radioHide.Bounds = r;
		}


		protected HToolBar						toolBar;
		protected ToolTip						toolTip;
		protected IconButton					buttonNew;
		protected IconButton					buttonDuplicate;
		protected IconButton					buttonUp;
		protected IconButton					buttonDown;
		protected IconButton					buttonDelete;
		protected CellTable						table;
		protected GroupBox						radioGroup;
		protected RadioButton					radioShow;
		protected RadioButton					radioDimmed;
		protected RadioButton					radioHide;
		protected Button						buttonShow;
		protected Button						buttonDimmed;
		protected Button						buttonHide;
		protected Widget						panel;
		protected Drawer						drawer;
		protected bool							ignoreListTextChanged = false;
	}
}
