using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class FicheTest
	{
		[Test] public void CheckFicheApplication()
		{
			this.window = new WindowFrame();
			this.window.Root.LayoutChanged += new EventHandler(this.Root_LayoutChanged);
			
			Widgets.Adorner.Factory.SetActive("LookDany");

			this.window.ClientSize = new System.Drawing.Size(1024, 768);
			this.window.Text = "Crésus-fiche";

			this.db = new TinyDataBase();
			this.db.Title = "Adresses";
			this.CreateFields();
			this.CreateRecords();
			this.SetSort(1, 1);

			this.CreateLayout();

			window.Show();
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

			this.menu = new Menu(MenuType.Horizontal);
			this.menu.Location = new Point(0, rect.Height-this.menu.DefaultHeight);
			this.menu.Size = new Size(rect.Width, this.menu.DefaultHeight);
			this.menu.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Top;
			this.menu.InsertItem("Fichier");
			this.menu.InsertItem("Edition");
			this.menu.InsertItem("Affichage");
			this.menu.InsertItem("Aide");
			this.menu.Parent = this.window.Root;

			Menu fileMenu = new Menu(MenuType.Vertical);
			fileMenu.InsertItem("", "Nouveau", "Ctrl+N");
			fileMenu.InsertItem("open", "Ouvrir...", "Ctrl+O");
			fileMenu.InsertItem("", "Fermer", "");
			fileMenu.InsertSep();
			fileMenu.InsertItem("save", "Enregistrer", "Ctrl+S");
			fileMenu.InsertItem("", "Enregistrer sous...", "");
			fileMenu.InsertSep();
			fileMenu.InsertItem("print", "Imprimer...", "Ctrl+P");
			fileMenu.InsertItem("preview", "Apercu avant impression", "");
			fileMenu.InsertItem("", "Mise en page...", "");
			fileMenu.InsertSep();
			fileMenu.InsertItem("", "Quitter", "");
			fileMenu.AdjustSize();
			this.menu.GetWidget(0).SonMenu = fileMenu;

			Menu editMenu = new Menu(MenuType.Vertical);
			editMenu.InsertItem("", "Annuler", "Ctrl+Z");
			editMenu.InsertSep();
			editMenu.InsertItem("cut", "Couper", "Ctrl+X");
			editMenu.InsertItem("copy", "Copier", "Ctrl+C");
			editMenu.InsertItem("paste", "Coller", "Ctrl+V");
			editMenu.AdjustSize();
			this.menu.GetWidget(1).SonMenu = editMenu;

			Menu showMenu = new Menu(MenuType.Vertical);
			showMenu.InsertItem("", "Adresses", "F5");
			showMenu.InsertItem("", "Objets", "F6");
			showMenu.InsertSep();
			showMenu.InsertItem("", "Options", "");
			showMenu.AdjustSize();
			this.menu.GetWidget(2).SonMenu = showMenu;

			Menu optMenu = new Menu(MenuType.Vertical);
			optMenu.InsertItem("", "Réglages...", "");
			optMenu.InsertItem("", "Machins...", "");
			optMenu.InsertItem("", "Bidules...", "");
			optMenu.AdjustSize();
			showMenu.GetWidget(3).SonMenu = optMenu;

			Menu helpMenu = new Menu(MenuType.Vertical);
			helpMenu.InsertItem("", "Aide", "F1");
			helpMenu.InsertItem("", "A propos de...", "");
			helpMenu.AdjustSize();
			this.menu.GetWidget(3).SonMenu = helpMenu;

			this.toolBar = new ToolBar();
			this.toolBar.Location = new Point(0, rect.Height-this.menu.DefaultHeight-this.toolBar.DefaultHeight);
			this.toolBar.Size = new Size(rect.Width, this.toolBar.DefaultHeight);
			this.toolBar.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Top;
			this.toolBar.InsertIconButton("open");
			this.toolBar.InsertIconButton("save");
			this.toolBar.InsertSep(5);
			this.toolBar.InsertIconButton("cut");
			this.toolBar.InsertIconButton("copy");
			this.toolBar.InsertIconButton("paste");
			this.toolBar.Parent = this.window.Root;

			Widget root = new Widget();
			root.SetClientAngle(0);
			root.SetClientZoom(1.0);
			root.Location = new Point(0, 0);
			root.Size = new Size(rect.Width, rect.Height-this.menu.DefaultHeight-this.toolBar.DefaultHeight);
			root.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			root.Parent = this.window.Root;
			
			this.pane = new Pane();
			this.pane.PaneStyle = PaneStyle.LeftRight;
			this.pane.DimensionChanged += new EventHandler(this.pane_DimensionChanged);
			this.pane.Parent = root;
			this.leftPane  = this.pane.RetPane(0);
			this.rightPane = this.pane.RetPane(1);

			this.subPane = new Pane();
			this.subPane.PaneStyle = PaneStyle.BottomTop;
			this.subPane.DimensionChanged += new EventHandler(this.pane_DimensionChanged);
			this.subPane.Parent = this.leftPane;
			this.topPane    = this.subPane.RetPane(1);
			this.bottomPane = this.subPane.RetPane(0);

			this.title = new StaticText();
			this.title.SetClientZoom(3);
			this.title.Text = "<b>"+db.Title+"</b>";  // en gras
			this.topPane.Children.Add(this.title);

			this.editCrit = new TextField(TextFieldType.SingleLine);
			this.editCrit.Text = "";
			this.editCrit.TextInserted += new EventHandler(this.editCrit_TextInserted);
			this.topPane.Children.Add(this.editCrit);

			this.buttonSearch = new Button();
			this.buttonSearch.Text = "Chercher";
			this.buttonSearch.Clicked += new MessageEventHandler(this.buttonSearch_Clicked);
			this.topPane.Children.Add(this.buttonSearch);

			this.listCrit = new ScrollList();
			
			
			//	Génère les infos de debug pour le fond de la fenêtre, ce qui permet de
			//	réaliser des timings.
			
//-			this.window.Root.DebugActive = true;

#if false
			this.listCrit.Scroller.ArrowDown.Name = "Down";
			this.listCrit.Scroller.ArrowDown.DebugActive = true;
			this.listCrit.Scroller.ArrowUp.Name = "Up";
			this.listCrit.Scroller.ArrowUp.DebugActive = true;
#endif
			
			this.topPane.Children.Add(this.listCrit);

			this.table = new ScrollArray();
			this.table.SelectChanged += new EventHandler(this.table_SelectChanged);
			this.table.SortChanged += new EventHandler(this.table_SortChanged);
			this.InitTable();
			this.UpdateTable();
			this.bottomPane.Children.Add(this.table);

			this.buttonCreate = new Button();
			this.buttonCreate.Text = "Creer";
			this.buttonCreate.Clicked += new MessageEventHandler(this.buttonCreate_Clicked);
			this.rightPane.Children.Add(this.buttonCreate);

			this.buttonDuplicate = new Button();
			this.buttonDuplicate.Text = "Dupliquer";
			this.buttonDuplicate.Clicked += new MessageEventHandler(this.buttonDuplicate_Clicked);
			this.rightPane.Children.Add(this.buttonDuplicate);

			this.buttonDelete = new Button();
			this.buttonDelete.Text = "Supprimer";
			this.buttonDelete.Clicked += new MessageEventHandler(this.buttonDelete_Clicked);
			this.rightPane.Children.Add(this.buttonDelete);

			this.listCrit.AddText("<i><b>Partout</b></i>");
			this.listCrit.Select = 0;
			this.staticTexts.Clear();
			this.textFields.Clear();
			int nbField = this.db.TotalField;
			for ( int x=0 ; x<nbField ; x++ )
			{
				int fieldID = this.db.RetFieldID(x);
				TinyDataBase.FieldDesc fd = this.db.RetFieldDesc(fieldID);

				this.listCrit.AddText(fd.name);

				StaticText st = new StaticText();
				st.Alignment = ContentAlignment.MiddleRight;
				st.Text = fd.name;
				this.rightPane.Children.Add(st);
				this.staticTexts.Add(st);

				TextFieldType type = TextFieldType.SingleLine;
				if ( fd.lines > 1 )  type = TextFieldType.MultiLine;
				if ( fd.combo != "" )  type = TextFieldType.Combo;
				TextField tf = new TextField(type);
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

			this.buttonCancel = new Button();
			this.buttonCancel.Text = "Annuler";
			this.buttonCancel.Clicked += new MessageEventHandler(this.buttonCancel_Clicked);
			this.rightPane.Children.Add(this.buttonCancel);

			this.ResizeLayout();
			this.UpdateButton();
		}

		private void Root_LayoutChanged(object sender)
		{
			this.ResizeLayout();
		}

		private void pane_DimensionChanged(object sender)
		{
			Pane pane = (Pane)sender;

			if ( pane == this.pane )
			{
				this.listWidth = this.pane.RetDimension(0);
			}
			if ( pane == this.subPane )
			{
				this.critHeight = this.subPane.RetDimension(1);
			}
			this.ResizeLayout();
		}

		protected void ResizeLayout()
		{
			if ( this.pane == null )  return;

			Size windowDim = this.pane.Parent.Client.Size;

			this.pane.Location = new Point(0, 0);
			this.pane.Size = new Size(windowDim.Width, windowDim.Height);
			this.pane.SetHideDimension(0, 100);
			this.pane.SetMinDimension(1, 300);
			this.pane.SetDimension(0, this.listWidth);

			this.subPane.Location = new Point(0, 0);
			this.subPane.Size = new Size(this.leftPane.Width, this.leftPane.Height);
			this.subPane.FlipFlop = true;
			this.subPane.SetMinDimension(1, 80);
			this.subPane.SetMaxDimension(1, 80+this.listCritHeight+10);
			this.subPane.SetDimension(1, this.critHeight);

			this.title.Location = new Point(10, this.topPane.Height-50);
			this.title.Size = new Size(this.topPane.Width, 50);

			this.editCrit.Location = new Point(10, this.topPane.Height-50-this.buttonHeight);
			this.editCrit.Size = new Size(this.topPane.Width-this.buttonWidth-30, this.buttonHeight);

			this.buttonSearch.Location = new Point(this.topPane.Width-this.buttonWidth-10, this.topPane.Height-50-this.buttonHeight);
			this.buttonSearch.Size = new Size(this.buttonWidth, this.buttonHeight);

			this.listCrit.Location = new Point(10, this.topPane.Height-50-this.buttonHeight-10-this.listCritHeight);
			this.listCrit.Size = new Size(200, this.listCritHeight);

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

				double height = 6+defaultFontHeight*fd.lines;

				StaticText st = (StaticText)this.staticTexts[x];
				st.Location = new Point(0, posy-20);
				st.Size = new Size(this.labelWidth-10, 20);

				TextField tf = (TextField)this.textFields[x];
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
			this.table.FuncFillText = new Epsitec.Common.Widgets.ScrollArray.FillText(this.FillText);
			this.table.Columns = this.db.TotalField;

			for ( int x=0 ; x<this.table.Columns ; x++ )
			{
				int fieldID = this.db.RetFieldID(x);
				TinyDataBase.FieldDesc fd = this.db.RetFieldDesc(fieldID);

				this.table.SetHeaderText(x, fd.name);
				this.table.SetWidthColumn(x, fd.width);
				this.table.SetAlignmentColumn(x, fd.alignment);
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
			this.table.Reset();
			this.table.Rows = this.db.TotalRecord;

			int field, mode;
			this.db.GetSortField(0, out field, out mode);
			TinyDataBase.FieldDesc fd = this.db.RetFieldDesc(field);
			this.table.SetHeaderSort(fd.rank, mode);

			this.table.Select = this.recordRank;
			this.table.ShowSelect(ScrollArrayShow.Extremity);
		}

		// Initialise la liste d'une ligne éditable "combo".
		protected void InitCombo(TextField tf, string combo)
		{
			while ( true )
			{
				int index = combo.IndexOf('$');
				if ( index < 0 )  break;
				tf.ComboAddText(combo.Substring(0, index));
				combo = combo.Remove(0, index+1);
			}
			tf.ComboAddText(combo);
		}

		// Choix du mode de tri.
		protected void SetSort(int field, int mode)
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
		private void table_SelectChanged(object sender)
		{
			this.recordRank = this.table.Select;
			this.recordCreated = false;
			this.UpdateLayout();
			this.UpdateButton();
		}

		// Changement de tri dans la liste.
		private void table_SortChanged(object sender)
		{
			int column, mode;
			this.table.GetHeaderSort(out column, out mode);
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
				this.table.Select = this.recordRank;
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
				this.table.Select = this.recordRank;
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
			this.recordRank = this.table.Select;
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
				TextField tf = (TextField)this.textFields[i];
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
				TextField tf = (TextField)this.textFields[i];
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
			TextField tf = (TextField)this.textFields[rank];
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

			mydb.SetSortField(0, 0, 1);  // tri par noms
			mydb.SetSortField(1, 1, 1);  // puis par prénoms

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

			mydb.SetSortField(0, 1, 1);  // tri par prénoms
			mydb.SetSortField(1, 0, 1);  // puis par noms

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
		protected double						buttonHeight = 20;
		protected WindowFrame					window;
		protected Menu							menu;
		protected ToolBar						toolBar;
		protected Pane							pane;
		protected Pane							subPane;
		protected Widget						leftPane;
		protected Widget						rightPane;
		protected Widget						topPane;
		protected Widget						bottomPane;
		protected StaticText					title;
		protected TextField						editCrit;
		protected Button						buttonSearch;
		protected ScrollList					listCrit;
		protected ScrollArray					table;
		protected Button						buttonCreate;
		protected Button						buttonDuplicate;
		protected Button						buttonDelete;
		protected Button						buttonValidate;
		protected Button						buttonCancel;
		protected System.Collections.ArrayList	staticTexts = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	textFields = new System.Collections.ArrayList();
	}
}
