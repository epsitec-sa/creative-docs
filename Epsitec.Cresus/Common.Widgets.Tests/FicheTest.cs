using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class FicheTest
	{
		[Test] public void CheckFicheApplication()
		{
			Widgets.Adorner.Factory.SetActive("LookDany");
			
			this.window = new Window();
			this.window.Root.LayoutChanged += new EventHandler(this.Root_LayoutChanged);
			
			this.window.ClientSize = new Size(1024, 768);
			this.window.Text = "Crésus-fiche";
			this.window.WindowClosed += new EventHandler(this.HandleWindowClosed);

			this.db = new TinyDataBase();
			this.db.Title = "Adresses";
			this.CreateFields();
			this.CreateRecords();
			this.SetSort(1, SortMode.Down);

			this.CreateLayout();

			this.window.Show();
		}

		private void HandleWindowClosed(object sender)
		{
			this.allWidgets = false;

			this.window = null;
			this.tip = null;
			this.menu = null;
			this.toolBar = null;
			this.pane = null;
			this.subPane = null;
			this.leftPane = null;
			this.rightPane = null;
			this.topPane = null;
			this.bottomPane = null;
			this.title = null;
			this.editCrit = null;
			this.buttonSearch = null;
			this.listCrit = null;
			this.listLook = null;
			this.table = null;

			this.buttonCreate = null;
			this.buttonDuplicate = null;
			this.buttonDelete = null;
			this.buttonValidate = null;
			this.buttonCancel = null;

			this.staticTexts = null;
			this.textFields = null;
		}

		// Crée les rubriques dans la base.
		protected void CreateFields()
		{
			TinyDataBase.FieldDesc fd;

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Titre";
			fd.rank = 0;
			fd.link = true;
			fd.max = 50;
			fd.width = 80;
			fd.combo = "Monsieur$Madame$Mademoiselle$Famille";
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Nom";
			fd.rank = 1;
			fd.link = true;
			fd.max = 50;
			fd.width = 120;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Prenom";
			fd.rank = 2;
			fd.max = 50;
			fd.width = 120;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Adresse";
			fd.rank = 4;
			fd.width = 150;
			fd.max = 100;
			fd.lines = 3;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "NPA";
			fd.rank = 5;
			fd.link = true;
			fd.max = 10;
			fd.width = 40;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Ville";
			fd.rank = 6;
			fd.link = true;
			fd.max = 40;
			fd.width = 120;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Firme";
			fd.rank = 3;
			fd.max = 50;
			fd.width = 100;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Pays";
			fd.rank = 7;
			fd.max = 40;
			fd.width = 100;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Telephone prive";
			fd.rank = 8;
			fd.link = true;
			fd.max = 50;
			fd.width = 100;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Telephone prof";
			fd.rank = 9;
			fd.link = true;
			fd.max = 50;
			fd.width = 100;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Telephone mobile";
			fd.rank = 10;
			fd.link = true;
			fd.max = 50;
			fd.width = 100;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Fax";
			fd.rank = 11;
			fd.max = 50;
			fd.width = 100;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "e-mail prive";
			fd.rank = 12;
			fd.link = true;
			fd.max = 50;
			fd.width = 100;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "e-mail prof";
			fd.rank = 13;
			fd.link = true;
			fd.max = 50;
			fd.width = 100;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Site web";
			fd.rank = 14;
			fd.max = 50;
			fd.width = 100;
			this.db.CreateFieldDesc(fd);

			this.db.CreateEmptyFieldDesc(out fd);
			fd.name = "Commentaire";
			fd.rank = 15;
			fd.max = 500;
			fd.width = 100;
			fd.lines = 5;
			this.db.CreateFieldDesc(fd);
		}

		// Crée quelques fiches dans la base.
		protected void CreateRecords()
		{
			TinyDataBase.Record record;

			this.db.CreateEmptyRecord(out record);
			this.db.SetFieldInRecord(record, 0, "Monsieur");
			this.db.SetFieldInRecord(record, 1, "Roux");
			this.db.SetFieldInRecord(record, 2, "Daniel");
			this.db.SetFieldInRecord(record, 3, "Cresentine 33");
			this.db.SetFieldInRecord(record, 4, "1023");
			this.db.SetFieldInRecord(record, 5, "Crissier");
			this.db.CreateRecord(record);

			this.db.CreateEmptyRecord(out record);
			this.db.SetFieldInRecord(record, 0, "Monsieur");
			this.db.SetFieldInRecord(record, 1, "Dumoulin");
			this.db.SetFieldInRecord(record, 2, "Denis");
			this.db.SetFieldInRecord(record, 3, "Brume 3");
			this.db.SetFieldInRecord(record, 4, "1110");
			this.db.SetFieldInRecord(record, 5, "Morges");
			this.db.CreateRecord(record);

			this.db.CreateEmptyRecord(out record);
			this.db.SetFieldInRecord(record, 0, "Monsieur");
			this.db.SetFieldInRecord(record, 1, "Arnaud");
			this.db.SetFieldInRecord(record, 2, "Pierre");
			this.db.SetFieldInRecord(record, 3, "Fontenay 6");
			this.db.SetFieldInRecord(record, 4, "1400");
			this.db.SetFieldInRecord(record, 5, "Yverdon");
			this.db.CreateRecord(record);

			this.db.CreateEmptyRecord(out record);
			this.db.SetFieldInRecord(record, 0, "Monsieur");
			this.db.SetFieldInRecord(record, 1, "Raboud");
			this.db.SetFieldInRecord(record, 2, "Yves");
			this.db.SetFieldInRecord(record, 3, "Cornalles 2");
			this.db.SetFieldInRecord(record, 4, "1802");
			this.db.SetFieldInRecord(record, 5, "Corseaux");
			this.db.CreateRecord(record);

			this.db.CreateEmptyRecord(out record);
			this.db.SetFieldInRecord(record, 0, "Monsieur");
			this.db.SetFieldInRecord(record, 1, "Besuchet");
			this.db.SetFieldInRecord(record, 2, "David");
			this.db.SetFieldInRecord(record, 3, "Rte d'Orbe 28");
			this.db.SetFieldInRecord(record, 4, "1400");
			this.db.SetFieldInRecord(record, 5, "Yverdon");
			this.db.CreateRecord(record);

			this.db.CreateEmptyRecord(out record);
			this.db.SetFieldInRecord(record, 0, "Monsieur");
			this.db.SetFieldInRecord(record, 1, "Walz");
			this.db.SetFieldInRecord(record, 2, "Michael");
			this.db.SetFieldInRecord(record, 3, "EPFL-INF<br/>LAP");
			this.db.SetFieldInRecord(record, 4, "1015");
			this.db.SetFieldInRecord(record, 5, "Ecublens");
			this.db.CreateRecord(record);

			this.db.CreateEmptyRecord(out record);
			this.db.SetFieldInRecord(record, 0, "Madame");
			this.db.SetFieldInRecord(record, 1, "Nicoud");
			this.db.SetFieldInRecord(record, 2, "Cathi");
			this.db.SetFieldInRecord(record, 3, "Ch. de la Mouette 5");
			this.db.SetFieldInRecord(record, 4, "1092");
			this.db.SetFieldInRecord(record, 5, "Belmont");
			this.db.CreateRecord(record);

			for ( int i=0 ; i<100 ; i++ )
			{
				this.db.CreateEmptyRecord(out record);
				this.db.SetFieldInRecord(record, 0, "Monsieur");
				this.db.SetFieldInRecord(record, 1, "Dupond "+(i+1));
				this.db.SetFieldInRecord(record, 2, "Jean");
				this.db.SetFieldInRecord(record, 3, "Av. de la Gare 12");
				this.db.SetFieldInRecord(record, 4, "1000");
				this.db.SetFieldInRecord(record, 5, "Lausanne");
				this.db.CreateRecord(record);
			}
		}

		// Crée tous les widgets du layout.
		protected void CreateLayout()
		{
			Rectangle rect = this.window.Root.Client.Bounds;

			this.tip = new ToolTip();

			this.menu = new HMenu();
			this.menu.Host = this.window;
			this.menu.Name = "base";
			this.menu.Location = new Point(0, rect.Height-this.menu.DefaultHeight);
			this.menu.Size = new Size(rect.Width, this.menu.DefaultHeight);
			this.menu.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Top;
			this.menu.Items.Add(new MenuItem ("file", "Fichier"));
			this.menu.Items.Add(new MenuItem ("edit", "Edition"));
			this.menu.Items.Add(new MenuItem ("display", "Affichage"));
			this.menu.Items.Add(new MenuItem ("debug", "Debug"));
			this.menu.Items.Add(new MenuItem ("help", "Aide"));
			this.menu.Parent = this.window.Root;

			VMenu fileMenu = new VMenu();
			fileMenu.Host = this.window;
			fileMenu.Name = "fileMenu";
			fileMenu.Items.Add(new MenuItem ("new", "", "Nouveau", "Ctrl+N"));
			fileMenu.Items.Add(new MenuItem ("open", @"file:images/open.png", "Ouvrir...", "Ctrl+O"));
			fileMenu.Items.Add(new MenuItem ("close", "", "Fermer", ""));
			fileMenu.Items.Add(new MenuSeparator ());
			fileMenu.Items.Add(new MenuItem ("save", @"file:images/save.png", "Enregistrer", "Ctrl+S"));
			fileMenu.Items.Add(new MenuItem ("saveas", "", "Enregistrer sous...", ""));
			fileMenu.Items.Add(new MenuSeparator ());
			fileMenu.Items.Add(new MenuItem ("print", @"file:images/print.png", "Imprimer...", "Ctrl+P"));
			fileMenu.Items.Add(new MenuItem ("preview", @"file:images/preview.png", "Apercu avant impression", ""));
			fileMenu.Items.Add(new MenuItem ("warning", "", "Mise en page...", ""));
			fileMenu.Items.Add(new MenuSeparator ());
			fileMenu.Items.Add(new MenuItem ("quit", "", "Quitter", ""));
			fileMenu.AdjustSize();
			this.menu.Items[0].Submenu = fileMenu;

			VMenu editMenu = new VMenu();
			editMenu.Host = this.window;
			editMenu.Name = "editMenu";
			editMenu.Items.Add(new MenuItem ("undo", "", "Annuler", "Ctrl+Z"));
			editMenu.Items.Add(new MenuSeparator ());
			editMenu.Items.Add(new MenuItem ("cut", @"file:images/cut.png", "Couper", "Ctrl+X"));
			editMenu.Items.Add(new MenuItem ("copy", @"file:images/copy.png", "Copier", "Ctrl+C"));
			editMenu.Items.Add(new MenuItem ("paste", @"file:images/paste.png", "Coller", "Ctrl+V"));
			editMenu.AdjustSize();
			this.menu.Items[1].Submenu = editMenu;

			VMenu showMenu = new VMenu();
			showMenu.Host = this.window;
			showMenu.Name = "showMenu";
			showMenu.Items.Add(new MenuItem ("addr", "", "Adresses", "F5"));
			showMenu.Items.Add(new MenuItem ("objs", "", "Objets", "F6"));
			showMenu.Items.Add(new MenuSeparator ());
			showMenu.Items.Add(new MenuItem ("opts", "", "Options", ""));
			showMenu.Items.Add(new MenuItem ("set", "", "Reglages", ""));
			showMenu.AdjustSize();
			this.menu.Items[2].Submenu = showMenu;

			VMenu optMenu = new VMenu();
			optMenu.Host = this.window;
			optMenu.Name = "optMenu";
			optMenu.Items.Add(new MenuItem ("misc", "", "Divers...", ""));
			optMenu.Items.Add(new MenuItem ("print", @"file:images/print.png", "Impression...", ""));
			optMenu.Items.Add(new MenuItem ("open", @"file:images/open.png", "Fichiers...", ""));
			optMenu.AdjustSize();
			showMenu.Items[3].Submenu = optMenu;

			VMenu setupMenu = new VMenu();
			setupMenu.Host = this.window;
			setupMenu.Name = "setupMenu";
			setupMenu.Items.Add(new MenuItem ("base", "", "Base...", ""));
			setupMenu.Items.Add(new MenuItem ("global", "", "Global...", ""));
			setupMenu.Items.Add(new MenuItem ("list", "", "Liste...", ""));
			setupMenu.Items.Add(new MenuItem ("edit", "", "Edition...", ""));
			setupMenu.Items.Add(new MenuItem ("lang", "", "Langue...", ""));
			setupMenu.AdjustSize();
			showMenu.Items[4].Submenu = setupMenu;

			VMenu debugMenu = new VMenu();
			debugMenu.Host = this.window;
			debugMenu.Name = "debugMenu";
			debugMenu.Items.Add(new MenuItem ("colorA", "", "Couleur A", ""));
			debugMenu.Items.Add(new MenuItem ("colorB", "", "Couleur B", ""));
			debugMenu.Items.Add(new MenuItem ("colorC", "", "Couleur C", ""));
			debugMenu.AdjustSize();
			this.menu.Items[3].Submenu = debugMenu;

			VMenu debugMenu1 = new VMenu();
			debugMenu1.Host = this.window;
			debugMenu1.Name = "debugMenu1";
			debugMenu1.Items.Add(new MenuItem ("red", "", "Rouge", ""));
			debugMenu1.Items.Add(new MenuItem ("green", "", "Vert", ""));
			debugMenu1.Items.Add(new MenuItem ("blue", "", "Bleu", ""));
			debugMenu1.AdjustSize();
			debugMenu.Items[0].Submenu = debugMenu1;

			VMenu debugMenu2 = new VMenu();
			debugMenu2.Host = this.window;
			debugMenu2.Name = "debugMenu2";
			debugMenu2.Items.Add(new MenuItem ("red", "", "Rouge", ""));
			debugMenu2.Items.Add(new MenuItem ("green", "", "Vert", ""));
			debugMenu2.Items.Add(new MenuItem ("blue", "", "Bleu", ""));
			debugMenu2.AdjustSize();
			debugMenu.Items[1].Submenu = debugMenu2;

			VMenu debugMenu3 = new VMenu();
			debugMenu3.Host = this.window;
			debugMenu3.Name = "debugMenu3";
			debugMenu3.Items.Add(new MenuItem ("red", "", "Rouge", ""));
			debugMenu3.Items.Add(new MenuItem ("green", "", "Vert", ""));
			debugMenu3.Items.Add(new MenuItem ("blue", "", "Bleu", ""));
			debugMenu3.AdjustSize();
			debugMenu.Items[2].Submenu = debugMenu3;

			VMenu helpMenu = new VMenu();
			helpMenu.Host = this.window;
			helpMenu.Name = "helpMenu";
			helpMenu.Items.Add(new MenuItem ("help", "", "Aide", "F1"));
			helpMenu.Items.Add(new MenuItem ("ctxhelp", @"file:images/help.png", "Aide contextuelle", ""));
			helpMenu.Items.Add(new MenuItem ("about", "", "A propos de...", ""));
			helpMenu.AdjustSize();
			this.menu.Items[4].Submenu = helpMenu;

			this.toolBar = new HToolBar();
			this.toolBar.Location = new Point(0, rect.Height-this.menu.DefaultHeight-this.toolBar.DefaultHeight);
			this.toolBar.Size = new Size(rect.Width, this.toolBar.DefaultHeight);
			this.toolBar.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Top;
			this.toolBar.Items.Add (new IconButton (@"file:images/open.png"));
			this.toolBar.Items.Add (new IconButton (@"file:images/save.png"));
			this.toolBar.Items.Add (new IconSeparator());
			this.toolBar.Items.Add (new IconButton (@"file:images/cut.png"));
			this.toolBar.Items.Add (new IconButton (@"file:images/copy.png"));
			this.toolBar.Items.Add (new IconButton (@"file:images/paste.png"));
			this.toolBar.Parent = this.window.Root;

			Widget root = new Panel();
			root.SetClientAngle(0);
			root.SetClientZoom(1.0);
			root.Location = new Point(0, 0);
			root.Size = new Size(rect.Width, rect.Height-this.menu.DefaultHeight-this.toolBar.DefaultHeight);
			root.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			root.Parent = this.window.Root;
			
			this.pane = new PaneBook();
			this.pane.Location = new Point(0, 0);
			this.pane.Size = root.Size;
			this.pane.PaneBookStyle = PaneBookStyle.LeftRight;
			this.pane.PaneBehaviour = PaneBookBehaviour.Draft;
			//this.pane.PaneBehaviour = PaneBookBehaviour.FollowMe;
			this.pane.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			this.pane.SizeChanged += new EventHandler(this.pane_SizeChanged);
			this.pane.Parent = root;

			this.leftPane = new PanePage();
			this.leftPane.PaneRelativeSize = 10;
			this.leftPane.PaneElasticity = 1;
			this.leftPane.PaneHideSize = 150;
			this.pane.Items.Add(this.leftPane);

			this.rightPane = new PanePage();
			this.rightPane.PaneRelativeSize = 10;
			this.rightPane.PaneElasticity = 0;
			//this.rightPane.PaneMaxSize = 300;
			this.rightPane.PaneHideSize = 300;
			this.pane.Items.Add(this.rightPane);

			this.subPane = new PaneBook();
			this.subPane.Location = new Point(0, 0);
			this.subPane.Size = new Size(this.leftPane.Width, this.leftPane.Height);
			this.subPane.PaneBookStyle = PaneBookStyle.BottomTop;
			this.subPane.PaneBehaviour = PaneBookBehaviour.Draft;
			//this.subPane.PaneBehaviour = PaneBookBehaviour.FollowMe;
			this.subPane.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			this.subPane.SizeChanged += new EventHandler(this.pane_SizeChanged);
			this.subPane.Parent = this.leftPane;

			this.topPane = new PanePage();
			this.topPane.PaneToggle = true;
			this.topPane.PaneMinSize = 80;
			this.topPane.PaneMaxSize = 80+this.listCritHeight+10;
			this.topPane.PaneRelativeSize = 1;
			this.topPane.PaneElasticity = 0;
			this.subPane.Items.Add(this.topPane);

			this.bottomPane = new PanePage();
			this.bottomPane.PaneRelativeSize = 10;
			this.bottomPane.PaneElasticity = 1;
			this.subPane.Items.Add(this.bottomPane);

			this.title = new StaticText();
			this.title.SetClientZoom(3);
			this.title.Text = "<b>"+db.Title+"</b>";  // en gras
			this.topPane.Children.Add(this.title);

			this.editCrit = new TextField();
			this.editCrit.Text = "";
			this.editCrit.TextInserted += new EventHandler(this.editCrit_TextInserted);
			this.topPane.Children.Add(this.editCrit);
			this.tip.SetToolTip(this.editCrit, "Elément cherché n'importe où");

			this.buttonSearch = new Button();
			this.buttonSearch.Text = "Chercher";
			this.buttonSearch.Clicked += new MessageEventHandler(this.buttonSearch_Clicked);
			this.topPane.Children.Add(this.buttonSearch);
			this.tip.SetToolTip(this.buttonSearch, "Cherche la prochaine fiche");

			this.listCrit = new ScrollList();
			this.topPane.Children.Add(this.listCrit);
			
			this.listLook = new ScrollList();

			string[] list = Widgets.Adorner.Factory.AdornerNames;
			int i = 0;
			int sel = 0;
			foreach ( string name in list )
			{
				this.listLook.Items.Add(name);
				if ( name == Widgets.Adorner.Factory.ActiveName )  sel = i;
				i ++;
			}

			this.listLook.SelectedIndex = sel;
			this.listLook.SelectedIndexChanged += new EventHandler(this.HandleLook);
			this.topPane.Children.Add(this.listLook);

			this.table = new ScrollArray();
			this.table.SelectedIndexChanged += new EventHandler(this.table_SelectedIndexChanged);
			this.table.SortChanged += new EventHandler(this.table_SortChanged);
			this.InitTable();
			this.UpdateTable();
			this.bottomPane.Children.Add(this.table);

			this.buttonCreate = new Button();
			this.buttonCreate.Text = "Creer";
			this.buttonCreate.Clicked += new MessageEventHandler(this.buttonCreate_Clicked);
			this.rightPane.Children.Add(this.buttonCreate);
			this.tip.SetToolTip(this.buttonCreate, "Crée une nouvelle fiche");

			this.buttonDuplicate = new Button();
			this.buttonDuplicate.Text = "Dupliquer";
			this.buttonDuplicate.Clicked += new MessageEventHandler(this.buttonDuplicate_Clicked);
			this.rightPane.Children.Add(this.buttonDuplicate);
			this.tip.SetToolTip(this.buttonDuplicate, "Duplique une fiche existante");

			this.buttonDelete = new Button();
			this.buttonDelete.Text = "Supprimer";
			this.buttonDelete.Clicked += new MessageEventHandler(this.buttonDelete_Clicked);
			this.rightPane.Children.Add(this.buttonDelete);
			this.tip.SetToolTip(this.buttonDelete, "Supprime une fiche");

			this.staticTexts = new System.Collections.ArrayList();
			this.textFields = new System.Collections.ArrayList();
			this.listCrit.Items.Add("<i><b>Partout</b></i>");
			this.listCrit.SelectedIndex = 0;
			this.staticTexts.Clear();
			this.textFields.Clear();
			int nbField = this.db.TotalField;
			for ( int x=0 ; x<nbField ; x++ )
			{
				int fieldID = this.db.RetFieldID(x);
				TinyDataBase.FieldDesc fd = this.db.RetFieldDesc(fieldID);

				this.listCrit.Items.Add(fd.name);

				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleRight;
				st.Text = fd.name;
				this.rightPane.Children.Add(st);
				this.staticTexts.Add(st);

				TextFieldType type = TextFieldType.SingleLine;
				if ( fd.lines > 1 )  type = TextFieldType.MultiLine;
				if ( fd.combo != "" )  type = TextFieldType.Combo;
				AbstractTextField tf = TextFieldAny.FromType (type);
				tf.Name = fd.name;
				tf.Text = "";
				tf.Alignment = fd.alignment;
				tf.MaxChar = fd.max;
				if ( fd.combo != "" )  this.InitCombo(tf, fd.combo);
				tf.TextChanged += new EventHandler(this.tf_TextChanged);
				this.rightPane.Children.Add(tf);
				this.textFields.Add(tf);
			}

			this.buttonValidate = new Button();
			this.buttonValidate.Text = "Valider";
			this.buttonValidate.Clicked += new MessageEventHandler(this.buttonValidate_Clicked);
			this.rightPane.Children.Add(this.buttonValidate);
			this.tip.SetToolTip(this.buttonValidate, "Valide la fiche en édition");

			this.buttonCancel = new Button();
			this.buttonCancel.Text = "Annuler";
			this.buttonCancel.Clicked += new MessageEventHandler(this.buttonCancel_Clicked);
			this.rightPane.Children.Add(this.buttonCancel);
			this.tip.SetToolTip(this.buttonCancel, "Annule les modifications dans la fiche");

			this.allWidgets = true;

			this.ResizeLayout();
			this.UpdateButton();
		}

		private void Root_LayoutChanged(object sender)
		{
			this.ResizeLayout();
		}

		private void pane_SizeChanged(object sender)
		{
			PaneBook pane = (PaneBook)sender;

			if ( pane == this.pane )
			{
				this.listWidth = this.leftPane.Client.Width;
			}
			if ( pane == this.subPane )
			{
				this.critHeight = this.topPane.Client.Height;
			}
			this.ResizeLayout();
		}

		private void HandleLook(object sender)
		{
			ScrollList sl = sender as ScrollList;
			int sel = sl.SelectedIndex;
			Widgets.Adorner.Factory.SetActive(sl.Items[sel]);
		}

		protected void ResizeLayout()
		{
			if ( !this.allWidgets )  return;

			this.title.Location = new Point(10, this.topPane.Height-50);
			this.title.Size = new Size(this.topPane.Width, 50);

			this.editCrit.Location = new Point(10, this.topPane.Height-50-this.buttonHeight);
			this.editCrit.Size = new Size(this.topPane.Width-this.buttonWidth-30, this.buttonHeight);

			this.buttonSearch.Location = new Point(this.topPane.Width-this.buttonWidth-10, this.topPane.Height-50-this.buttonHeight);
			this.buttonSearch.Size = new Size(this.buttonWidth, this.buttonHeight);

			this.listCrit.Location = new Point(10, this.topPane.Height-50-this.buttonHeight-10-this.listCritHeight);
			this.listCrit.Size = new Size(200, this.listCritHeight);

			this.listLook.Location = new Point(220, this.topPane.Height-50-this.buttonHeight-10-this.listCritHeight);
			this.listLook.Size = new Size(100, this.listCritHeight);

			this.table.Location = new Point(10, 10);
			this.table.Size = new Size(this.bottomPane.Width-20, this.bottomPane.Height-20);

			double posy = this.rightPane.Height-10-this.buttonHeight;
			double posx = this.labelWidth;

			this.buttonCreate.Location = new Point(posx, posy);
			this.buttonCreate.Size = new Size(this.buttonWidth, this.buttonHeight);

			posx += this.buttonWidth+10;
			this.buttonDuplicate.Location = new Point(posx, posy);
			this.buttonDuplicate.Size = new Size(this.buttonWidth, this.buttonHeight);

			posx += this.buttonWidth+10;
			this.buttonDelete.Location = new Point(posx, posy);
			this.buttonDelete.Size = new Size(this.buttonWidth, this.buttonHeight);

			posy -= 10;
			double maxWidth = this.rightPane.Width-this.labelWidth-10;
			double defaultFontHeight = this.rightPane.DefaultFontHeight;
			int nbField = this.db.TotalField;
			for ( int x=0 ; x<nbField ; x++ )
			{
				int fieldID = this.db.RetFieldID(x);
				TinyDataBase.FieldDesc fd = this.db.RetFieldDesc(fieldID);

				double height = 8+defaultFontHeight*fd.lines;

				StaticText st = (StaticText)this.staticTexts[x];
				st.Location = new Point(0, posy-this.buttonHeight);
				st.Size = new Size(this.labelWidth-10, this.buttonHeight);

				AbstractTextField tf = this.textFields[x] as AbstractTextField;
				tf.Location = new Point(this.labelWidth, posy-height);
				double width = System.Math.Min(fd.max*7, maxWidth);
				tf.Size = new Size(width, height);

				double suppl = 4;
				if ( fd.link )  suppl = -1;
				posy -= height+suppl;
			}

			posy -= 10+this.buttonHeight;
			posx = this.labelWidth;
			this.buttonValidate.Location = new Point(posx, posy);
			this.buttonValidate.Size = new Size(this.buttonWidth, this.buttonHeight);

			posx += this.buttonWidth+10;
			this.buttonCancel.Location = new Point(posx, posy);
			this.buttonCancel.Size = new Size(this.buttonWidth, this.buttonHeight);
		}

		// Initialise la table.
		protected void InitTable()
		{
			this.table.TextProviderCallback = new TextProviderCallback(this.FillText);
			this.table.ColumnCount = this.db.TotalField;

			for ( int x=0 ; x<this.table.ColumnCount ; x++ )
			{
				int fieldID = this.db.RetFieldID(x);
				TinyDataBase.FieldDesc fd = this.db.RetFieldDesc(fieldID);

				this.table.SetHeaderText(x, fd.name);
				this.table.SetColumnWidth(x, fd.width);
				this.table.SetColumnAlignment(x, fd.alignment);
			}
		}

		// Appelé par ScrollArray pour remplir une cellule.
		protected string FillText(int row, int column)
		{
			if ( row >= this.db.TotalRecord )  return "";
			TinyDataBase.Record record = this.db.RetRecord(row);
			int fieldID = this.db.RetFieldID(column);
			return this.db.RetFieldInRecord(record, fieldID);
		}

		// Met à jour le contenu de la table.
		protected void UpdateTable()
		{
			this.table.Clear();
			this.table.RowCount = this.db.TotalRecord;

			int field;
			SortMode mode;
			this.db.GetSortField(0, out field, out mode);
			TinyDataBase.FieldDesc fd = this.db.RetFieldDesc(field);
			this.table.SetSortingHeader(fd.rank, mode);

			this.table.SelectedIndex = this.recordRank;
			this.table.ShowSelect(ScrollArrayShow.Extremity);
		}

		// Initialise la liste d'une ligne éditable "combo".
		protected void InitCombo(AbstractTextField tf, string combo)
		{
			TextFieldCombo cb = tf as TextFieldCombo;
			
			while ( true )
			{
				int index = combo.IndexOf('$');
				if ( index < 0 )  break;
				cb.Items.Add(combo.Substring(0, index));
				combo = combo.Remove(0, index+1);
			}
			cb.Items.Add(combo);
		}

		// Choix du mode de tri.
		protected void SetSort(int field, SortMode mode)
		{
			for ( int i=0 ; i<10 ; i++ )
			{
				if ( field+i >= this.db.TotalField )  break;
				this.db.SetSortField(i, field+i, mode);
			}
			this.db.Sort();

			if ( this.table != null )
			{
				this.table.RefreshContent();
			}
		}

		// Changement de sélection dans la liste.
		private void table_SelectedIndexChanged(object sender)
		{
			this.recordRank = this.table.SelectedIndex;
			this.recordCreated = false;
			this.UpdateLayout();
			this.UpdateButton();
		}

		// Changement de tri dans la liste.
		private void table_SortChanged(object sender)
		{
			int column;
			SortMode mode;
			this.table.GetSortingHeader(out column, out mode);
			int fieldID = this.db.RetFieldID(column);
			int sel = this.db.SortToInternal(this.recordRank);
			this.SetSort(fieldID, mode);
			this.recordRank = this.db.InternalToSort(sel);
			this.recordCreated = false;
			this.UpdateTable();
			this.UpdateLayout();
			this.UpdateButton();
			this.table.SetFocused(true);
		}

		// Critère de recherche complété.
		private void editCrit_TextInserted(object sender)
		{
			this.UpdateButton();
			string crit = this.editCrit.Text;
			if ( crit.Length == 0 )  return;
			string complete;
			int rank = System.Math.Max(this.recordRank-1, 0);
			int field;
			if ( this.db.SearchCritere(ref rank, out field, 1, crit, out complete) )
			{
				this.recordRank = rank;
				this.table.SelectedIndex = this.recordRank;
				this.table.ShowSelect(ScrollArrayShow.Extremity);
				this.UpdateLayout();

				this.editCrit.Text = complete;
				this.editCrit.CursorFrom = complete.Length;
				this.editCrit.CursorTo = crit.Length;
			}
		}

		// Bouton "Chercher" cliqué.
		private void buttonSearch_Clicked(object sender, MessageEventArgs e)
		{
			string crit = this.editCrit.Text;
			string complete;
			int rank = this.recordRank;
			int field;
			if ( this.db.SearchCritere(ref rank, out field, 1, crit, out complete) )
			{
				this.recordRank = rank;
				this.table.SelectedIndex = this.recordRank;
				this.table.ShowSelect(ScrollArrayShow.Extremity);
				this.UpdateLayout();
				this.UpdateButton();
				this.SetFocus(field);
			}
		}

		// Bouton "Créer" cliqué.
		private void buttonCreate_Clicked(object sender, MessageEventArgs e)
		{
			this.recordRank = -1;
			this.recordCreated = true;
			this.UpdateLayout();
			this.UpdateButton();
			this.SetFocus(0);
		}

		// Bouton "Dupliquer" cliqué.
		private void buttonDuplicate_Clicked(object sender, MessageEventArgs e)
		{
			TinyDataBase.Record newRecord;
			this.db.CreateCopyRecord(out newRecord, this.record);
			this.record = newRecord;
			this.recordRank = -1;
			this.recordCreated = true;
			this.UpdateButton();
			this.SetFocus(0);
		}

		// Bouton "Supprimer" cliqué.
		private void buttonDelete_Clicked(object sender, MessageEventArgs e)
		{
			if ( this.recordRank == -1 )  return;
			this.db.DeleteRecord(this.recordRank);
			if ( this.recordRank >= this.db.TotalRecord )
			{
				this.recordRank = this.db.TotalRecord-1;
			}
			this.recordCreated = false;
			this.UpdateLayout();
			this.UpdateTable();
			this.UpdateButton();
			this.table.SetFocused(true);
		}

		// Bouton "Valider" cliqué.
		private void buttonValidate_Clicked(object sender, MessageEventArgs e)
		{
			UpdateRecord();
			if ( this.recordRank == -1 )
			{
				this.recordRank = this.db.CreateRecord(this.record);
			}
			else
			{
				this.recordRank = this.db.SetRecord(this.recordRank, this.record);
			}
			this.recordModified = false;
			this.recordCreated = false;
			this.UpdateTable();
			this.UpdateButton();
			this.table.SetFocused(true);
		}

		// Bouton "Annuler" cliqué.
		private void buttonCancel_Clicked(object sender, MessageEventArgs e)
		{
			this.recordCreated = false;
			this.recordRank = this.table.SelectedIndex;
			this.UpdateLayout();
			this.UpdateButton();
			this.table.SetFocused(true);
		}

		// Texte d'une rubrique changé.
		private void tf_TextChanged(object sender)
		{
			this.recordModified = true;
			this.UpdateButton();
		}

		// this.record -> layout
		protected void UpdateLayout()
		{
			if ( this.recordRank == -1 )
			{
				this.db.CreateEmptyRecord(out this.record);
			}
			else
			{
				this.record = this.db.RetRecord(this.recordRank);
			}

			int nbFields = this.db.TotalField;
			for ( int i=0 ; i<nbFields ; i++ )
			{
				AbstractTextField tf = this.textFields[i] as AbstractTextField;
				int fieldID = this.db.RetFieldID(i);
				tf.Text = this.db.RetFieldInRecord(this.record, fieldID);
			}
			this.recordModified = false;
		}

		// layout -> this.record
		protected void UpdateRecord()
		{
			int nbFields = this.db.TotalField;
			for ( int i=0 ; i<nbFields ; i++ )
			{
				AbstractTextField tf = this.textFields[i] as AbstractTextField;
				int fieldID = this.db.RetFieldID(i);
				this.db.SetFieldInRecord(this.record, fieldID, tf.Text);
			}
		}

		// Met à jour tous les boutons.
		protected void UpdateButton()
		{
			bool enable;

			enable = !this.recordModified && !this.recordCreated;
			this.buttonCreate.SetEnabled(enable);

			enable = !this.recordModified && !this.recordCreated && this.recordRank != -1;
			this.buttonDuplicate.SetEnabled(enable);

			enable = this.recordRank != -1;
			this.buttonDelete.SetEnabled(enable);

			enable = this.recordModified;
			this.buttonValidate.SetEnabled(enable);

			enable = this.recordModified || this.recordCreated;
			this.buttonCancel.SetEnabled(enable);

			string crit = this.editCrit.Text;
			enable = crit.Length > 0;
			this.buttonSearch.SetEnabled(enable);
		}

		// Met le focus dans une rubrique éditable.
		protected void SetFocus(int rank)
		{
			AbstractTextField tf = this.textFields[rank] as AbstractTextField;
			tf.SelectAll();
			tf.SetFocused(true);
		}


		// -------------------------------------------------------------------------
		[Test] public void CheckTinyDataBase()
		{
			TinyDataBase mydb;
			mydb = new TinyDataBase();
			mydb.Title = "essai";

			TinyDataBase.FieldDesc fd;

			mydb.CreateEmptyFieldDesc(out fd);
			fd.name = "Nom";
			mydb.CreateFieldDesc(fd);

			mydb.CreateEmptyFieldDesc(out fd);
			fd.name = "Prénom";
			mydb.CreateFieldDesc(fd);

			Assertion.Assert(mydb.TotalField == 2);

			TinyDataBase.Record record;
			int rank;

			mydb.CreateEmptyRecord(out record);
			mydb.SetFieldInRecord(record, 0, "Roux");
			mydb.SetFieldInRecord(record, 1, "Daniel");
			Assertion.Assert(mydb.RetFieldInRecord(record, 0) == "Roux");
			Assertion.Assert(mydb.RetFieldInRecord(record, 1) == "Daniel");
			rank = mydb.CreateRecord(record);
			Assertion.Assert(rank == 0);

			mydb.CreateEmptyRecord(out record);
			mydb.SetFieldInRecord(record, 1, "Denis");
			mydb.SetFieldInRecord(record, 0, "Dumoulin");
			Assertion.Assert(mydb.RetFieldInRecord(record, 0) == "Dumoulin");
			Assertion.Assert(mydb.RetFieldInRecord(record, 1) == "Denis");
			rank = mydb.CreateRecord(record);
			Assertion.Assert(rank == 1);

			mydb.CreateEmptyRecord(out record);
			mydb.SetFieldInRecord(record, 0, "Walz");
			mydb.SetFieldInRecord(record, 1, "Michael");
			Assertion.Assert(mydb.RetFieldInRecord(record, 0) == "Walz");
			Assertion.Assert(mydb.RetFieldInRecord(record, 1) == "Michael");
			rank = mydb.CreateRecord(record);
			Assertion.Assert(rank == 2);

			Assertion.Assert(mydb.TotalRecord == 3);

			mydb.SetSortField(0, 0, SortMode.Down);  // tri par noms
			mydb.SetSortField(1, 1, SortMode.Down);  // puis par prénoms

			// Dumoulin, Roux, Walz
			TinyDataBase.Record r1;
			r1 = mydb.RetRecord(0);
			Assertion.Assert(mydb.RetFieldInRecord(r1, 0) == "Dumoulin");
			r1 = mydb.RetRecord(2);
			Assertion.Assert(mydb.RetFieldInRecord(r1, 0) == "Walz");
			r1 = mydb.RetRecord(1);
			Assertion.Assert(mydb.RetFieldInRecord(r1, 0) == "Roux");

			TinyDataBase.Record r2;
			r2 = mydb.RetRecord(2);
			mydb.SetFieldInRecord(r2, 0, "Raboud");  // Walz -> Raboud
			mydb.SetFieldInRecord(r2, 1, "Yves");  // Michael -> Yves
			// Dumoulin, Raboud, Roux
			rank = mydb.SetRecord(2, r2);
			Assertion.Assert(rank == 1);

			mydb.DeleteRecord(0);  // supprime Dumoulin
			Assertion.Assert(mydb.TotalRecord == 2);

			// Raboud, Roux
			r1 = mydb.RetRecord(1);
			Assertion.Assert(mydb.RetFieldInRecord(r1, 0) == "Roux");
			r1 = mydb.RetRecord(0);
			Assertion.Assert(mydb.RetFieldInRecord(r1, 0) == "Raboud");

			mydb.CreateEmptyRecord(out record);
			mydb.SetFieldInRecord(record, 0, "Arnaud");
			mydb.SetFieldInRecord(record, 1, "Pierre");
			rank = mydb.CreateRecord(record);

			mydb.SetSortField(0, 1, SortMode.Down);  // tri par prénoms
			mydb.SetSortField(1, 0, SortMode.Down);  // puis par noms

			// Roux, Arnaud, Raboud
			r1 = mydb.RetRecord(0);
			Assertion.Assert(mydb.RetFieldInRecord(r1, 1) == "Daniel");
			r1 = mydb.RetRecord(1);
			Assertion.Assert(mydb.RetFieldInRecord(r1, 1) == "Pierre");
			r1 = mydb.RetRecord(2);
			Assertion.Assert(mydb.RetFieldInRecord(r1, 1) == "Yves");
		}


		protected TinyDataBase					db;
		protected TinyDataBase.Record			record;
		protected int							recordRank = -1;
		protected bool							recordModified = false;
		protected bool							recordCreated = false;
		protected double						critHeight = 80;
		protected double						listCritHeight = 105;
		protected double						listWidth = 460;
		protected double						labelWidth = 120;
		protected double						buttonWidth = 80;
		protected double						buttonHeight = 21;
		protected Window						window;
		protected ToolTip						tip;
		protected HMenu							menu;
		protected HToolBar						toolBar;
		protected PaneBook						pane;
		protected PaneBook						subPane;
		protected PanePage						leftPane;
		protected PanePage						rightPane;
		protected PanePage						topPane;
		protected PanePage						bottomPane;
		protected StaticText					title;
		protected TextField						editCrit;
		protected Button						buttonSearch;
		protected ScrollList					listCrit;
		protected ScrollList					listLook;
		protected ScrollArray					table;
		protected Button						buttonCreate;
		protected Button						buttonDuplicate;
		protected Button						buttonDelete;
		protected Button						buttonValidate;
		protected Button						buttonCancel;
		protected System.Collections.ArrayList	staticTexts;
		protected System.Collections.ArrayList	textFields;
		protected bool							allWidgets = false;
	}
	
	
	public enum TextFieldType
	{
		SingleLine,						// ligne simple, scrollable horizontalement
		MultiLine,						// ligne multiple, scrollable verticalement
		UpDown,							// valeur numérique avec boutons +/-
		Combo,							// combo box
	}
	
	public class TextFieldAny
	{
		private TextFieldAny()
		{
		}
	
		public static AbstractTextField FromType(TextFieldType type)
		{
			switch (type)
			{
				case TextFieldType.Combo:
					return new TextFieldCombo ();

				case TextFieldType.SingleLine:
					return new TextField ();

				case TextFieldType.MultiLine:
					return new TextFieldMulti ();

				case TextFieldType.UpDown:
					return new TextFieldUpDown ();
			}
		
			throw new System.ArgumentException ("Unsupported type");
		}
	}
}
