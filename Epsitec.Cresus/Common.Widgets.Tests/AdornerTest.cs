using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	[TestFixture]
	public class AdornerTest
	{
		[Test] public void CheckAdornerWidgets()
		{
			this.CreateAdornerWidgets();
		}
		
		[Test] public void CheckAdornerWidgetsDisabled()
		{
			Window window = this.CreateAdornerWidgets();
			this.RecursiveDisable(window.Root, true);
		}
		
		[Test] public void CheckAdornerBigText()
		{
			this.CreateBigText();
		}
		
		
		void RecursiveDisable(Widget widget, bool top_level)
		{
			if (widget.IsEnabled)
			{
				widget.SetEnabled (top_level);

				foreach (Widget child in widget.Children)
				{
					if (! child.IsEmbedded)
					{
						this.RecursiveDisable(child, false);
					}
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("Already disabled: " + widget.CommandName + " / " + widget.GetType().Name + " / " + widget.Text);
			}
		}
		
		
		private Window CreateAdornerWidgets()
		{
			Pictogram.Engine.Initialise();

			Window window = new Window();
			
			window.ClientSize = new Size(600, 340);
			window.Text = "CheckAdornerWidgets";
			window.Name = "CheckAdornerWidgets";

			ToolTip tip = new ToolTip();
			tip.Behaviour = ToolTipBehaviour.Normal;
			
			HMenu menu = new HMenu();
			menu.Host = window;
//			menu.Location = new Point(0, window.ClientSize.Height-menu.DefaultHeight);
//			menu.Size = new Size(window.ClientSize.Width, menu.DefaultHeight);
			menu.Dock = DockStyle.Top;
			menu.Items.Add(new MenuItem("file", "Fichier"));
			menu.Items.Add(new MenuItem("edit", "Edition"));
			menu.Items.Add(new MenuItem("display", "Affichage"));
			menu.Items.Add(new MenuItem("debug", "Debug"));
			menu.Items.Add(new MenuItem("help", "Aide"));
			window.Root.Children.Add(menu);

			VMenu fileMenu = new VMenu();
			fileMenu.Host = window;
			fileMenu.Name = "file";
			fileMenu.Items.Add(new MenuItem("new", "", "Nouveau", "Ctrl+N"));
			fileMenu.Items.Add(new MenuItem("open", @"file:images/open.icon", "Ouvrir...", "Ctrl+O"));
			fileMenu.Items.Add(new MenuItem("close", "", "Fermer", ""));
			fileMenu.Items.Add(new MenuSeparator ());
			fileMenu.Items.Add(new MenuItem("save", @"file:images/save.icon", "Enregistrer", "Ctrl+S"));
			fileMenu.Items.Add(new MenuItem("saveas", "", "Enregistrer sous...", ""));
			fileMenu.Items.Add(new MenuSeparator ());
			fileMenu.Items.Add(new MenuItem("print", "", "Imprimer...", "Ctrl+P"));
			fileMenu.Items.Add(new MenuItem("preview", "", "Apercu avant impression", ""));
			fileMenu.Items.Add(new MenuItem("warning", "", "Mise en page...", ""));
			fileMenu.Items.Add(new MenuSeparator ());
			fileMenu.Items.Add(new MenuItem("quit", "", "Quitter", ""));
			fileMenu.AdjustSize();
			menu.Items[0].Submenu = fileMenu;
			fileMenu.Items[4].SetEnabled(false);

			VMenu editMenu = new VMenu();
			editMenu.Host = window;
			editMenu.Name = "edit";
			editMenu.Items.Add(new MenuItem("undo", "", "Annuler", "Ctrl+Z"));
			editMenu.Items.Add(new MenuSeparator ());
			editMenu.Items.Add(new MenuItem("cut", @"file:images/cut.icon", "Couper", "Ctrl+X"));
			editMenu.Items.Add(new MenuItem("copy", @"file:images/copy.icon", "Copier", "Ctrl+C"));
			editMenu.Items.Add(new MenuItem("paste", @"file:images/paste.icon", "Coller", "Ctrl+V"));
			editMenu.AdjustSize();
			menu.Items[1].Submenu = editMenu;

			VMenu showMenu = new VMenu();
			showMenu.Host = window;
			showMenu.Name = "show";
			showMenu.Items.Add(new MenuItem("addr", "", "Adresses", "F5"));
			showMenu.Items.Add(new MenuItem("objs", "", "Objets", "F6"));
			showMenu.Items.Add(new MenuSeparator ());
			showMenu.Items.Add(new MenuItem("opts", "", "Options", ""));
			showMenu.Items.Add(new MenuItem("set", "", "Reglages", ""));
			showMenu.AdjustSize();
			menu.Items[2].Submenu = showMenu;
			showMenu.Items[1].SetEnabled(false);

			VMenu optMenu = new VMenu();
			optMenu.Host = window;
			optMenu.Name = "opt";
			optMenu.Items.Add(new MenuItem("misc", "", "Divers...", ""));
			optMenu.Items.Add(new MenuItem("print", "", "Impression...", ""));
			optMenu.Items.Add(new MenuItem("open", "", "Fichiers...", ""));
			optMenu.AdjustSize();
			showMenu.Items[3].Submenu = optMenu;

			VMenu setupMenu = new VMenu();
			setupMenu.Host = window;
			setupMenu.Name = "setup";
			setupMenu.Items.Add(new MenuItem("base", "", "Base...", ""));
			setupMenu.Items.Add(new MenuItem("global", "", "Global...", ""));
			setupMenu.Items.Add(new MenuItem("list", "", "Liste...", ""));
			setupMenu.Items.Add(new MenuItem("edit", "", "Edition...", ""));
			setupMenu.Items.Add(new MenuItem("lang", "", "Langue...", ""));
			setupMenu.AdjustSize();
			showMenu.Items[4].Submenu = setupMenu;

			VMenu debugMenu = new VMenu();
			debugMenu.Host = window;
			debugMenu.Name = "debug";
			debugMenu.Items.Add(new MenuItem("colorA", "", "Couleur A", ""));
			debugMenu.Items.Add(new MenuItem("colorB", "", "Couleur B", ""));
			debugMenu.Items.Add(new MenuItem("colorC", "", "Couleur C", ""));
			debugMenu.AdjustSize();
			menu.Items[3].Submenu = debugMenu;

			VMenu debugMenu1 = new VMenu();
			debugMenu1.Host = window;
			debugMenu1.Name = "debug1";
			debugMenu1.Items.Add(new MenuItem("red", "", "Rouge", ""));
			debugMenu1.Items.Add(new MenuItem("green", "", "Vert", ""));
			debugMenu1.Items.Add(new MenuItem("blue", "", "Bleu", ""));
			debugMenu1.AdjustSize();
			debugMenu.Items[0].Submenu = debugMenu1;

			VMenu debugMenu2 = new VMenu();
			debugMenu2.Host = window;
			debugMenu2.Name = "debug2";
			debugMenu2.Items.Add(new MenuItem("red", "", "Rouge", ""));
			debugMenu2.Items.Add(new MenuItem("green", "", "Vert", ""));
			debugMenu2.Items.Add(new MenuItem("blue", "", "Bleu", ""));
			debugMenu2.AdjustSize();
			debugMenu.Items[1].Submenu = debugMenu2;

			VMenu debugMenu3 = new VMenu();
			debugMenu3.Host = window;
			debugMenu3.Name = "debug3";
			debugMenu3.Items.Add(new MenuItem("red", "", "Rouge", ""));
			debugMenu3.Items.Add(new MenuItem("green", "", "Vert", ""));
			debugMenu3.Items.Add(new MenuItem("blue", "", "Bleu", ""));
			debugMenu3.AdjustSize();
			debugMenu.Items[2].Submenu = debugMenu3;

			VMenu helpMenu = new VMenu();
			helpMenu.Host = window;
			helpMenu.Name = "help";
			helpMenu.Items.Add(new MenuItem("help", "", "Aide", "F1"));
			helpMenu.Items.Add(new MenuItem("ctxhelp", "", "Aide contextuelle", ""));
			helpMenu.Items.Add(new MenuItem("about", "", "A propos de...", ""));
			helpMenu.AdjustSize();
			menu.Items[4].Submenu = helpMenu;

			HToolBar tb = new HToolBar();
//			tb.Location = new Point(0, window.ClientSize.Height-menu.DefaultHeight-tb.DefaultHeight);
//			tb.Width = window.ClientSize.Width;
			tb.Dock  = DockStyle.Top;
			window.Root.Children.Add(tb);

			tb.Items.Add(new IconButton("open", @"file:images/open.icon"));
			tb.Items.Add(new IconButton("save", @"file:images/save.icon"));
			tb.Items.Add(new IconSeparator());

			TextFieldCombo t1 = new TextFieldCombo();
			t1.Width = 70;
			t1.Text = "Rouge";
			t1.Items.Add("red",   "Rouge");
			t1.Items.Add("green", "Vert");
			t1.Items.Add("blue",  "Bleu");

			tb.Items.Add(t1);
			tb.Items.Add(new IconSeparator());
			tb.Items.Add(new IconButton("cut",   @"file:images/cut.icon"));
			tb.Items.Add(new IconButton("copy",  @"file:images/copy.icon"));
			tb.Items.Add(new IconButton("paste", @"file:images/paste.icon"));

			StatusBar sb = new StatusBar();
//			sb.Location = new Point(0, 0);
//			sb.Width = window.ClientSize.Width;
			sb.Dock = DockStyle.Bottom;
			StatusField sf1 = new StatusField();
			sf1.Text = "Statuts 1";
			sb.Items.Add(sf1);
			StatusField sf2 = new StatusField();
			sf2.Text = "Statuts 2";
			sb.Items.Add(sf2);
			window.Root.Children.Add(sb);

			Button a = new Button();
//			a.Location = new Point(10, 30);
			a.Width = 75;
			a.Text = "OK";
			a.ButtonStyle = ButtonStyle.DefaultAccept;
			a.Anchor = AnchorStyles.BottomLeft;
			a.AnchorMargins = new Margins(10, 0, 0, 30);
			a.TabIndex = 20;
			a.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(a);
			tip.SetToolTip(a, "C'est d'accord, tout baigne");

			Button b = new Button();
//			b.Location = new Point(95, 30);
			b.Width = 75;
			b.Text = "<m>A</m>nnuler";
			b.Anchor = AnchorStyles.BottomLeft;
			b.AnchorMargins = new Margins(95, 0, 0, 30);
			b.TabIndex = 21;
			b.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(b);
			tip.SetToolTip(b, "Annule tout<br/>Deuxieme ligne, juste pour voir !");

			Button c = new Button();
//			c.Location = new Point(95+150, 30);
			c.Width = 75;
			c.Text = "Ai<m>d</m>e";
			c.SetEnabled(false);
			c.Anchor = AnchorStyles.BottomLeft;
			c.AnchorMargins = new Margins(245, 0, 0, 30);
			c.TabIndex = 22;
			c.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(c);
			tip.SetToolTip(c, "Au secours !");

			StaticText st = new StaticText();
//			st.Location = new Point(10, 265);
			st.Width = 150;
			st.Text = @"Choix du <b>look</b> de l'<i>interface</i> :";
			st.Anchor = AnchorStyles.TopLeft;
			st.AnchorMargins = new Margins(10, 0, 340 - st.Height - 265, 0);
			window.Root.Children.Add(st);

			this.CreateListLook(window.Root, 10, 80, tip, 1);
			
			Tag tag1 = new Tag("ExecuteTag", "TestTag");
			tag1.Bounds = new Drawing.Rectangle(115, 241, 18, 18);
			tag1.Parent = window.Root;
			tip.SetToolTip(tag1, "Je suis un <i>smart tag</i> maison.");

			Tag tag2 = new Tag("ExecuteTag", "TestTag");
			tag2.Bounds = new Drawing.Rectangle(115, 221, 18, 18);
			tag2.Parent = window.Root;
			tag2.Color = Drawing.Color.FromRGB(1,0,0);
			tip.SetToolTip(tag2, "Je suis un <i>smart tag</i> maison rouge.");

			Tag tag3 = new Tag("ExecuteTag", "TestTag");
			tag3.Bounds = new Drawing.Rectangle(115, 201, 18, 18);
			tag3.Parent = window.Root;
			tag3.Color = Drawing.Color.FromRGB(0,1,0);
			tip.SetToolTip(tag3, "Je suis un <i>smart tag</i> maison vert.");

			Tag tag4 = new Tag("ExecuteTag", "TestTag");
			tag4.Bounds = new Drawing.Rectangle(140, 241, 12, 12);
			tag4.Parent = window.Root;
			tip.SetToolTip(tag4, "Je suis un petit <i>smart tag</i> maison.");

			Tag tag5 = new Tag("ExecuteTag", "TestTag");
			tag5.Bounds = new Drawing.Rectangle(140, 221, 12, 12);
			tag5.Parent = window.Root;
			tag5.Color = Drawing.Color.FromRGB(0,0,1);
			tip.SetToolTip(tag5, "Je suis un petit <i>smart tag</i> maison bleu.");

			StaticText link = new StaticText();
//			link.Location = new Point(360, 36);
			link.Width = 200;
			link.Text = @"Visitez notre <a href=""http://www.epsitec.ch"">site web</a> !";
			link.Anchor = AnchorStyles.BottomRight;
			link.AnchorMargins = new Margins(0, 600 - 360 - 200, 0, 36);
			link.HypertextClicked += new MessageEventHandler(link_HypertextClicked);
			window.Root.Children.Add(link);

			GroupBox box = new GroupBox();
//			box.Location = new Point(10, 100);
			box.Size = new Size(100, 75);
			box.Text = "Couleur";
			box.Anchor = AnchorStyles.BottomLeft;
			box.AnchorMargins = new Margins(10, 0, 0, 100);
			box.TabIndex = 2;
			window.Root.Children.Add(box);

			RadioButton radio1 = new RadioButton();
			radio1.Location = new Point(10, 40);
			radio1.Width = 80;
			radio1.Text = "<font color=\"#ff0000\"><m>R</m>ouge</font>";
			radio1.ActiveState = WidgetState.ActiveYes;
			radio1.Group = "RGB";
			radio1.TabIndex = 1;
			radio1.Index = 1;
			radio1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio1.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio1);
			tip.SetToolTip(radio1, "Couleur rouge");

			RadioButton radio2 = new RadioButton();
			radio2.Location = new Point(10, 25);
			radio2.Width = 80;
			radio2.Text = "<font color=\"#00ff00\"><m>V</m>ert</font>";
			radio2.Group = "RGB";
			radio2.TabIndex = 1;
			radio2.Index = 2;
			radio2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio2.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio2);
			tip.SetToolTip(radio2, "Couleur verte");

			RadioButton radio3 = new RadioButton();
			radio3.Location = new Point(10, 10);
			radio3.Width = 80;
			radio3.Text = "<font color=\"#0000ff\"><m>B</m>leu</font>";
			radio3.Group = "RGB";
			radio3.TabIndex = 1;
			radio3.Index = 3;
			radio3.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			radio3.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio3);
			tip.SetToolTip(radio3, "Couleur bleue");

			CheckButton check = new CheckButton();
//			check.Location = new Point(10, 70);
			check.Width = 100;
			check.Text = "<m>C</m>ochez ici";
			check.ActiveState = WidgetState.ActiveYes;
			check.Anchor = AnchorStyles.BottomLeft;
			check.AnchorMargins = new Margins(10, 0, 0, 70);
			check.TabIndex = 3;
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.Clicked += new MessageEventHandler(this.HandleCheck);
			window.Root.Children.Add(check);
			tip.SetToolTip(check, "Juste pour voir");

			VScroller scrollv = new VScroller();
//			scrollv.Location = new Point(120, 70);
			scrollv.Size = new Size(17, 120);
			scrollv.MaxValue = 10;
			scrollv.VisibleRangeRatio = 0.3M;
			scrollv.Value = 1;
			scrollv.SmallChange = 1;
			scrollv.LargeChange = 2;
			scrollv.Anchor = AnchorStyles.Left | AnchorStyles.TopAndBottom;
			scrollv.AnchorMargins = new Margins(120, 0, 340 - 120 - 70, 70);
			window.Root.Children.Add(scrollv);
			tip.SetToolTip(scrollv, "Ascenseur vertical");

			HScroller scrollh = new HScroller();
//			scrollh.Location = new Point(140, 70);
			scrollh.Size = new Size(120, 17);
			scrollh.MaxValue = 10;
			scrollh.VisibleRangeRatio = 0.7M;
			scrollh.Value = 1;
			scrollh.SmallChange = 1;
			scrollh.LargeChange = 2;
			scrollh.Anchor = AnchorStyles.BottomLeft;
			scrollh.AnchorMargins = new Margins(140, 0, 0, 70);
			window.Root.Children.Add(scrollh);
			tip.SetToolTip(scrollh, "Ascenseur horizontal");

			TextFieldExList combo = new TextFieldExList();
			combo.PlaceHolder = "<b>&lt;autre&gt;</b>";
//			combo.Location = new Point(160, 220);
			combo.Width = 100;
			combo.Text = "Janvier";
			combo.Cursor = combo.Text.Length;
			combo.Items.Add("Janvier");
			combo.Items.Add("Fevrier");
			combo.Items.Add("Mars");
			combo.Items.Add("Avril");
			combo.Items.Add("Mai");
			combo.Items.Add("Juin");
			combo.Items.Add("Juillet");
			combo.Items.Add("Aout");
			combo.Items.Add("Septembre");
			combo.Items.Add("Octobre");
			combo.Items.Add("Novembre");
			combo.Items.Add("Decembre");
			combo.Items.Add("Lundi");
			combo.Items.Add("Mardi");
			combo.Items.Add("Mercredi");
			combo.Items.Add("Jeudi");
			combo.Items.Add("Vendredi");
			combo.Items.Add("Samedi");
			combo.Items.Add("Dimanche");
			combo.Items.Add("JusteUnLongTexte");
			combo.Items.Add("JusteUnLongTexte1");
			combo.Items.Add("JusteUnLongTexte12");
			combo.Items.Add("JusteUnLongTexte123");
			combo.Items.Add("JusteUnLongTextePourVoir");
			combo.Anchor = AnchorStyles.BottomLeft;
			combo.AnchorMargins = new Margins(160, 0, 0, 220);
			combo.TabIndex = 10;
			combo.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(combo);

			TextField text = new TextField();
//			text.Location = new Point(160, 190);
			text.Width = 100;
			text.Text = "Bonjour";
			text.Cursor = text.Text.Length;
			text.Anchor = AnchorStyles.BottomLeft;
			text.AnchorMargins = new Margins(160, 0, 0, 190);
			text.TabIndex = 11;
			text.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(text);

			TextFieldUpDown tud = new TextFieldUpDown();
//			tud.Location = new Point(160, 160);
			tud.Width = 45;
			
			tud.Value        =   5.00M;
			tud.DefaultValue =   0.00M;
			tud.MinValue     = -10.00M;
			tud.MaxValue     =  10.00M;
			tud.Step         =   2.50M;
			tud.Resolution   =   0.25M;
			
			tud.Anchor = AnchorStyles.BottomLeft;
			tud.AnchorMargins = new Margins(160, 0, 0, 160);
			tud.TabIndex = 12;
			tud.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(tud);

			TextFieldSlider slider = new TextFieldSlider();
//			slider.Location = new Point(215, 160);
			slider.Width = 45;
			slider.Value = 50;
			slider.MinValue = -100;
			slider.MaxValue = 100;
			slider.Step = 10;
			slider.Resolution = 5;
			slider.Anchor = AnchorStyles.BottomLeft;
			slider.AnchorMargins = new Margins(215, 0, 0, 160);
			slider.TabIndex = 13;
			slider.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(slider);

			TextFieldMulti multi = new TextFieldMulti();
//			multi.Location = new Point(160, 100);
			multi.Size = new Size(100, 50);
			multi.Text = "Ceci est une petite phrase ridicule.<br/>Mais elle est assez longue pour faire des essais.";
			//?multi.TextLayout.JustifMode = TextJustifMode.AllButLast;
			multi.Anchor = AnchorStyles.BottomLeft;
			multi.AnchorMargins = new Margins(160, 0, 0, 100);
			multi.TabIndex = 14;
			multi.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(multi);

			TabBook tab = new TabBook();
			tab.Arrows = TabBookArrows.Right;
//			tab.Location = new Point(280, 70);
//			tab.Size = new Size(300, 180);
			tab.Anchor = AnchorStyles.All;
			tab.AnchorMargins = new Margins(280, 600-280-300, 340-180-70, 70);
			tab.HasMenuButton = true;
			tab.HasCloseButton = true;
			tab.TabIndex = 15;
			tab.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren;
			window.Root.Children.Add(tab);

			Rectangle inside = tab.InnerBounds;

			// Cr�e l'onglet 1.
			TabPage page1 = new TabPage();
//			page1.Bounds = inside;
			page1.TabTitle = "<m>P</m>remier";
			page1.TabIndex = 1;
			page1.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			tab.Items.Add(page1);

			ScrollList sl = new ScrollList();
//			sl.Location = new Point(20, 10);
			sl.Size = new Size(90, 100);
			sl.AdjustHeight(ScrollAdjustMode.MoveDown);
			sl.Items.Add("Janvier");
			sl.Items.Add("Fevrier");
			sl.Items.Add("Mars <i>(A)</i>");
			sl.Items.Add("Avril");
			sl.Items.Add("Mai");
			sl.Items.Add("Juin");
			sl.Items.Add("Juillet <b>(B)</b>");
			sl.Items.Add("Aout");
			sl.Items.Add("Septembre");
			sl.Items.Add("Octobre");
			sl.Items.Add("Novembre");
			sl.Items.Add("Decembre");
			sl.SelectedIndex = 5;  // s�lectionne juin
			sl.ShowSelected(ScrollShowMode.Center);
			sl.Anchor = AnchorStyles.TopAndBottom|AnchorStyles.Left;
			sl.AnchorMargins = new Margins(10, 0, 10, 10);
			sl.TabIndex = 1;
			sl.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			page1.Children.Add(sl);
			tip.SetToolTip(sl, "Choix du mois");

			StaticText st2 = new StaticText();
//			st2.Location = new Point(160, 120);
			st2.Width = 90;
			st2.Text = "Non �ditable :";
			st2.Anchor = AnchorStyles.TopLeft;
			st2.AnchorMargins = new Margins(160, 0, 30, 0);
			page1.Children.Add(st2);

			TextField textfix = new TextField();
//			textfix.Location = new Point(160, 80);
			textfix.Width = 100;
			textfix.Text = "Texte fixe";
			textfix.IsReadOnly = true;
			textfix.Anchor = AnchorStyles.TopLeft;
			textfix.AnchorMargins = new Margins(160, 0, 50, 0);
			page1.Children.Add(textfix);

			TextFieldCombo combofix = new TextFieldCombo();
//			combofix.Location = new Point(160, 50);
			combofix.Width = 100;
			combofix.Text = "Mardi";
			combofix.IsReadOnly = true;
			combofix.Items.Add("Lundi");
			combofix.Items.Add("Mardi");
			combofix.Items.Add("Mercredi");
			combofix.Items.Add("Jeudi");
			combofix.Items.Add("Vendredi");
			combofix.Items.Add("Samedi");
			combofix.Items.Add("Dimanche");
			combofix.Items.Add("Juste un long texte pour voir ...");
			combofix.Items.Add("Encore un autre long texte ...");
			combofix.Anchor = AnchorStyles.TopLeft;
			combofix.AnchorMargins = new Margins(160, 0, 80, 0);
			combofix.TabIndex = 2;
			combofix.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			page1.Children.Add(combofix);

			// Cr�e l'onglet 2.
			TabPage page2 = new TabPage();
//			page2.Bounds = inside;
			page2.TabTitle = "<m>D</m>euxi�me";
			page2.TabIndex = 2;
			page2.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			tab.Items.Add(page2);

			CellTable table = new CellTable();
			table.StyleH  = CellArrayStyle.ScrollNorm;
			table.StyleH |= CellArrayStyle.Header;
			table.StyleH |= CellArrayStyle.Separator;
			table.StyleH |= CellArrayStyle.Mobile;
			table.StyleH |= CellArrayStyle.Sort;
			table.StyleV  = CellArrayStyle.ScrollNorm;
			table.StyleV |= CellArrayStyle.Separator;
			table.StyleV |= CellArrayStyle.SelectLine;
			table.StyleV |= CellArrayStyle.SelectMulti;
			table.StyleV |= CellArrayStyle.Sort;
//			table.Location = new Point(10, 10);
//			table.Size = new Size(inside.Width-20, inside.Height-20);
			table.SetArraySize(5, 12);

			table.SetHeaderTextH(0, "A");
			table.SetHeaderTextH(1, "B");
			table.SetHeaderTextH(2, "C");
			table.SetHeaderTextH(3, "D");
			table.SetHeaderTextH(4, "E");

			for ( int y=0 ; y<12 ; y++ )
			{
				for ( int x=0 ; x<5 ; x++ )
				{
					table.SetWidthColumn(x, 60);
					StaticText tx = new StaticText();
					tx.PaintTextStyle = PaintTextStyle.Array;
					tx.Text = string.Format("L{0} C{1}", x+1, y+1);
					tx.Alignment = ContentAlignment.MiddleLeft;
					tx.Dock = Widgets.DockStyle.Fill;
					table[x,y].Insert(tx);
				}
			}
			table.Anchor = AnchorStyles.All;
			table.AnchorMargins = new Margins(10, 10, 10, 10);
			table.TabIndex = 1;
			table.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			page2.Children.Add(table);

			// Cr�e l'onglet 3.
			TabPage page3 = new TabPage();
			page3.Bounds = inside;
			page3.TabTitle = "<m>T</m>roisi�me";
			
			TextFieldEx text_ex_1 = new TextFieldEx();
			text_ex_1.Parent = page3;
			text_ex_1.Bounds = new Drawing.Rectangle(10, page3.Height-30, 120, text_ex_1.Height);
			text_ex_1.TabIndex = 1;
			text_ex_1.EditionAccepted += new EventHandler(this.HandleTextExEditionAccepted);
			text_ex_1.EditionRejected += new EventHandler(this.HandleTextExEditionRejected);
			
			TextFieldEx text_ex_2 = new TextFieldEx();
			text_ex_2.Parent = page3;
			text_ex_2.Bounds = new Drawing.Rectangle(10, page3.Height-30-28, 120, text_ex_2.Height);
			text_ex_2.TabIndex = 2;
			
			TextFieldEx text_ex_3 = new TextFieldEx();
			text_ex_3.Parent = page3;
			text_ex_3.Bounds = new Drawing.Rectangle(10, page3.Height-30-28-28, 120, text_ex_3.Height);
			text_ex_3.SetEnabled(false);
			text_ex_3.TabIndex = 3;
			
			TextFieldEx text_ex_4 = new TextFieldEx();
			text_ex_4.Parent = page3;
			text_ex_4.Bounds = new Drawing.Rectangle(10, page3.Height-30-28-28-28, 120, text_ex_4.Height);
			text_ex_4.TabIndex = 4;
			text_ex_4.ButtonShowCondition = ShowCondition.WhenModified;
			
			TextFieldEx text_ex_5 = new TextFieldEx();
			text_ex_5.Parent = page3;
			text_ex_5.Bounds = new Drawing.Rectangle(10, page3.Height-30-28-28-28-28, 120, text_ex_5.Height);
			text_ex_5.TabIndex = 5;
			text_ex_5.ButtonShowCondition = ShowCondition.WhenFocused;
			text_ex_5.DefocusAction       = DefocusAction.AcceptEdition;
			
			TextFieldEx text_ex_6 = new TextFieldEx();
			text_ex_6.Parent = page3;
			text_ex_6.Bounds = new Drawing.Rectangle(10+120+5, page3.Height-30, 120, text_ex_6.Height);
			text_ex_6.TabIndex = 6;
			text_ex_6.ButtonShowCondition = ShowCondition.WhenModified;
			text_ex_6.DefocusAction       = DefocusAction.Modal;
			
			TextFieldEx text_ex_7 = new TextFieldEx();
			text_ex_7.Parent = page3;
			text_ex_7.Bounds = new Drawing.Rectangle(10+120+5, page3.Height-30-28, 120, text_ex_7.Height);
			text_ex_7.TabIndex = 7;
			text_ex_7.ButtonShowCondition = ShowCondition.Never;
			text_ex_7.DefocusAction       = DefocusAction.AcceptEdition;
			
			TextFieldEx text_ex_8 = new TextFieldEx();
			text_ex_8.Parent = page3;
			text_ex_8.Bounds = new Drawing.Rectangle(10+120+5, page3.Height-30-28-28, 120, text_ex_8.Height);
			text_ex_8.TabIndex = 8;
			text_ex_8.ButtonShowCondition = ShowCondition.WhenModified;
			text_ex_8.DefocusAction       = DefocusAction.AutoAcceptOrRejectEdition;
			
			new Validators.RegexValidator (text_ex_6, Support.RegexFactory.AlphaName);
			new Validators.RegexValidator (text_ex_7, Support.RegexFactory.AlphaName);
			new Validators.RegexValidator (text_ex_8, Support.RegexFactory.AlphaName);
			
			page3.TabIndex = 3;
			page3.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			tab.Items.Add(page3);

			// Cr�e l'onglet 4.
			TabPage page4 = new TabPage();
			page4.Bounds = inside;
			page4.TabTitle = "<m>Q</m>uatri�me";
			page4.TabIndex = 4;
			page4.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			tab.Items.Add(page4);

			// Cr�e l'onglet 5.
			TabPage page5 = new TabPage();
			page5.Bounds = inside;
			page5.TabTitle = "<m>C</m>inqui�me";
			page5.TabIndex = 5;
			page5.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			tab.Items.Add(page5);

			// Cr�e l'onglet 6.
			TabPage page6 = new TabPage();
			page6.Bounds = inside;
			page6.TabTitle = "<m>S</m>ixi�me";
			page6.TabIndex = 6;
			page6.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			tab.Items.Add(page6);

			tab.ActivePage = page2;
			window.FocusedWidget = a;

			window.Show();
			return window;
		}

		
		private void HandleCheck(object sender, MessageEventArgs e)
		{
			CheckButton button = sender as CheckButton;
//			button.Toggle();
		}

		private void HandleRadio(object sender, MessageEventArgs e)
		{
			RadioButton button = sender as RadioButton;
//			button.Toggle();
		}

		private void link_HypertextClicked(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			System.Diagnostics.Process.Start (widget.Hypertext);
		}


		private Window CreateBigText()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckAdornerBigText";

			TextFieldMulti multi = new TextFieldMulti();
			StaticText     stats = new StaticText();
			
			multi.Name = "Multi";
//			multi.Bounds = new Rectangle(10, 30, 380, 260);
			multi.MaxChar = 10000;
			
			string s = "";
			s += "On donnait ce jour-l� un grand d�ner, o�, pour la premi�re fois, je vis avec beaucoup d'�tonnement le ma�tre d'h�tel servir l'�p�e au c�t� et le chapeau sur la t�te. Par hasard on vint � parler de la devise de la maison de Solar, qui �tait sur la tapisserie avec les armoiries: <i>Tel fiert qui ne tue pas</i>. Comme les Pi�montais ne sont pas pour l'ordinaire consomm�s par la langue fran�aise, quelqu'un trouva dans cette devise une faute d'orthographe, et dit qu'au mot <i>fiert</i> il ne fallait point de <i>t</i>.<br/>";
			s += "Le vieux comte de Gouvon allait r�pondre; mais ayant jet� les yeux sur moi, il vit que je souriait sans oser rien dire: il m'ordonna de parler. Alors je dis que je ne croyait pas que le <i>t</i> f�t de trop, que <i>fiert</i> �tait un vieux mots fran�ais qui ne venait pas du nom <i>ferus</i>, fier, mena�ant, mais du verbe <i>ferit</i>, il frappe, il blesse; qu'ainsi la devise ne me paraissait pas dire: Tel menace, mais <i>tel frappe qui ne tue pas</i>.<br/>";
			s += "Tout le monde me regardait et se regardait sans rien dire. On ne vit de la vie un pareil �tonnement. Mais ce qui me flatta davantage fut de voir clairement sur le visage de Mlle de Breil un air de satisfaction. Cette personne si d�daigneuse daigna me jeter un second regard qui valait tout au moins le premier; puis, tournant les yeux vers son grand-papa, elle semblait attendre avec une sorte d'impatience la louange qu'il me devait, et qu'il me donna en effet si pleine et enti�re et d'un air si content, que toute la table s'empressa de faire chorus. Ce moment fut court, mais d�licieux � tous �gards. Ce fut un de ces moments trop rares qui replacent les choses dans leur ordre naturel, et vengent le m�rite avili des outrages de la fortune.<br/>";
			s += "<b>FIN</b><br/><br/>Voici une image <img src=\"file:images/icon.png\"/> int�gr�e dans le texte.";
			multi.Text = s;

			multi.Alignment = Drawing.ContentAlignment.TopLeft;
			multi.TextLayout.JustifMode = TextJustifMode.AllButLast;
			//?multi.Alignment = Drawing.ContentAlignment.TopRight;
			//?multi.TextLayout.JustifMode = TextJustifMode.None;
			multi.TextLayout.ShowLineBreak = true;
			multi.ScrollZone = 0.2;
			multi.Anchor = AnchorStyles.All;
			multi.AnchorMargins = new Margins(10, 10, 40, 30);
			multi.AnchorMargins = new Margins(60, 60, 40, 30);
			multi.Parent = window.Root;
			multi.SetProperty("stats", stats);
			multi.SelectionChanged += new EventHandler(this.HandleMultiSelectionOrCursorChanged);
			multi.CursorChanged    += new EventHandler(this.HandleMultiSelectionOrCursorChanged);
			this.bigText = multi;
			
//			stats.Bounds = new Rectangle(10, 2, 380, 26);
			stats.Height = 26;
			stats.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Bottom;
			stats.AnchorMargins = new Margins(10, 10, 0, 2);
			stats.Parent = window.Root;

			Button buttonBold = new Button();
			buttonBold.Text = "<b>B</b>";
			buttonBold.Width = 30;
			buttonBold.AutoFocus = false;
			buttonBold.Anchor = AnchorStyles.TopLeft;
			buttonBold.AnchorMargins = new Margins(10, 0, 10, 0);
			buttonBold.Parent = window.Root;
			buttonBold.Clicked += new MessageEventHandler(this.HandleMultiBold);
			
			Button buttonItalic = new Button();
			buttonItalic.Text = "<i>I</i>";
			buttonItalic.Width = 30;
			buttonItalic.AutoFocus = false;
			buttonItalic.Anchor = AnchorStyles.TopLeft;
			buttonItalic.AnchorMargins = new Margins(40, 0, 10, 0);
			buttonItalic.Parent = window.Root;
			buttonItalic.Clicked += new MessageEventHandler(this.HandleMultiItalic);
			
			Button buttonUnderline = new Button();
			buttonUnderline.Text = "<u>U</u>";
			buttonUnderline.Width = 30;
			buttonUnderline.AutoFocus = false;
			buttonUnderline.Anchor = AnchorStyles.TopLeft;
			buttonUnderline.AnchorMargins = new Margins(70, 0, 10, 0);
			buttonUnderline.Parent = window.Root;
			buttonUnderline.Clicked += new MessageEventHandler(this.HandleMultiUnderline);
			
			Button buttonFace1 = new Button();
			buttonFace1.Text = "<font face=\"Tahoma\">A</font>";
			buttonFace1.Width = 30;
			buttonFace1.AutoFocus = false;
			buttonFace1.Anchor = AnchorStyles.TopLeft;
			buttonFace1.AnchorMargins = new Margins(110, 0, 10, 0);
			buttonFace1.Parent = window.Root;
			buttonFace1.Clicked += new MessageEventHandler(this.HandleMultiFace1);
			
			Button buttonFace2 = new Button();
			buttonFace2.Text = "<font face=\"Courier New\">A</font>";
			buttonFace2.Width = 30;
			buttonFace2.AutoFocus = false;
			buttonFace2.Anchor = AnchorStyles.TopLeft;
			buttonFace2.AnchorMargins = new Margins(140, 0, 10, 0);
			buttonFace2.Parent = window.Root;
			buttonFace2.Clicked += new MessageEventHandler(this.HandleMultiFace2);
			
			Button buttonFace3 = new Button();
			buttonFace3.Text = "<font face=\"Times New Roman\">A</font>";
			buttonFace3.Width = 30;
			buttonFace3.AutoFocus = false;
			buttonFace3.Anchor = AnchorStyles.TopLeft;
			buttonFace3.AnchorMargins = new Margins(170, 0, 10, 0);
			buttonFace3.Parent = window.Root;
			buttonFace3.Clicked += new MessageEventHandler(this.HandleMultiFace3);
			
			Button buttonSize1 = new Button();
			buttonSize1.Text = "10";
			buttonSize1.Width = 30;
			buttonSize1.AutoFocus = false;
			buttonSize1.Anchor = AnchorStyles.TopLeft;
			buttonSize1.AnchorMargins = new Margins(210, 0, 10, 0);
			buttonSize1.Parent = window.Root;
			buttonSize1.Clicked += new MessageEventHandler(this.HandleMultiSize1);
			
			Button buttonSize2 = new Button();
			buttonSize2.Text = "20";
			buttonSize2.Width = 30;
			buttonSize2.AutoFocus = false;
			buttonSize2.Anchor = AnchorStyles.TopLeft;
			buttonSize2.AnchorMargins = new Margins(240, 0, 10, 0);
			buttonSize2.Parent = window.Root;
			buttonSize2.Clicked += new MessageEventHandler(this.HandleMultiSize2);
			
			Button buttonColor1 = new Button();
			buttonColor1.Text = "<b><font color=\"#000000\">o</font></b>";
			buttonColor1.Width = 30;
			buttonColor1.AutoFocus = false;
			buttonColor1.Anchor = AnchorStyles.TopLeft;
			buttonColor1.AnchorMargins = new Margins(280, 0, 10, 0);
			buttonColor1.Parent = window.Root;
			buttonColor1.Clicked += new MessageEventHandler(this.HandleMultiColor1);
			
			Button buttonColor2 = new Button();
			buttonColor2.Text = "<b><font color=\"#FF0000\">o</font></b>";
			buttonColor2.Width = 30;
			buttonColor2.AutoFocus = false;
			buttonColor2.Anchor = AnchorStyles.TopLeft;
			buttonColor2.AnchorMargins = new Margins(310, 0, 10, 0);
			buttonColor2.Parent = window.Root;
			buttonColor2.Clicked += new MessageEventHandler(this.HandleMultiColor2);
			
//			window.Root.DebugActive = true;
			window.FocusedWidget    = multi;
			window.Show();
			
//			multi.Text = @"abc <b>def</b> ghi.<br/>123 <i>456</i> 789 <b>qrs</b>.<br/>A<img src=""file:images/icon.png""/>B<br/>";
			
			return window;
		}
		
		private void HandleMultiSelectionOrCursorChanged(object sender)
		{
			AbstractTextField text  = sender as AbstractTextField;
			StaticText        stats = text.GetProperty("stats") as StaticText;
			
			stats.Text = string.Format("{0} - {1},  after={2}", text.CursorFrom, text.CursorTo, text.CursorAfter);
		}

		private void HandleMultiBold(object sender, MessageEventArgs e)
		{
			this.bigText.SelectionBold = !this.bigText.SelectionBold;
		}

		private void HandleMultiItalic(object sender, MessageEventArgs e)
		{
			this.bigText.SelectionItalic = !this.bigText.SelectionItalic;
		}

		private void HandleMultiUnderline(object sender, MessageEventArgs e)
		{
			this.bigText.SelectionUnderlined = !this.bigText.SelectionUnderlined;
		}

		private void HandleMultiFace1(object sender, MessageEventArgs e)
		{
			this.bigText.SelectionFontName = "Tahoma";
		}

		private void HandleMultiFace2(object sender, MessageEventArgs e)
		{
			this.bigText.SelectionFontName = "Courier New";
		}

		private void HandleMultiFace3(object sender, MessageEventArgs e)
		{
			this.bigText.SelectionFontName = "Times New Roman";
		}

		private void HandleMultiSize1(object sender, MessageEventArgs e)
		{
			this.bigText.SelectionFontSize = Drawing.Font.DefaultFontSize;
		}

		private void HandleMultiSize2(object sender, MessageEventArgs e)
		{
			this.bigText.SelectionFontSize = 20;
		}

		private void HandleMultiColor1(object sender, MessageEventArgs e)
		{
			this.bigText.SelectionFontColor = Drawing.Color.FromBrightness(0);
		}

		private void HandleMultiColor2(object sender, MessageEventArgs e)
		{
			this.bigText.SelectionFontColor = Drawing.Color.FromRGB(1,0,0);
		}


		[Test] public void CheckAdornerTab1()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckAdornerTab1";
			window.WindowClosed += new EventHandler(this.HandleWindowClosed);
			window.Root.DockPadding = new Margins(10, 10, 10, 10);

			TabBook tb = new TabBook();
			tb.Arrows = TabBookArrows.Right;
			tb.Name = "TabBook";
			tb.Text = "";
			tb.Dock = DockStyle.Fill;
			window.Root.Children.Add(tb);
			this.tabBook = tb;

			// Cr�e l'onglet 1.
			TabPage page1 = new TabPage();
			page1.Name = "p1";
			page1.TabTitle = "<m>P</m>remier";
			tb.Items.Add(page1);

			Button a = new Button();
			a.Name = "A";
			a.Location = new Point(10, 10);
			a.Size = new Size(75, 24);
			a.Text = "OK";
			a.ButtonStyle = ButtonStyle.DefaultAccept;
			page1.Children.Add(a);

			Button b = new Button();
			b.Name = "B";
			b.Location = new Point(95, 10);
			b.Size = new Size(75, 24);
			b.Text = "<m>A</m>nnuler";
			page1.Children.Add(b);

			TextFieldMulti multi = new TextFieldMulti();
			multi.Name = "Multi";
			multi.Location = new Point(10, 45);
			multi.Size = new Size(350, 200);
			multi.Text = "1. Introduction<br/><br/>Les onglets permettent de mettre beaucoup de widgets sur une petite surface, ce qui s'av�re extr�mement utile et diablement pratique.<br/><br/>2. Conclusion<br/><br/>Un truc chouette, qui sera certainement tr�s utile dans le nouveau Cr�sus !";
			multi.Anchor = AnchorStyles.All;
			multi.AnchorMargins = new Margins(10, 10, 20, 40);
			page1.Children.Add(multi);

			// Cr�e l'onglet 2.
			TabPage page2 = new TabPage();
			page2.Name = "p2";
			page2.TabTitle = "<m>D</m>euxi�me";
			tb.Items.Add(page2);

			VScroller scrollv = new VScroller();
			scrollv.Name = "Scroller";
			scrollv.Location = new Point(10, 10);
			scrollv.Size = new Size(17, tb.InnerBounds.Height-20);
			scrollv.MaxValue = 10;
			scrollv.VisibleRangeRatio = 0.3M;
			scrollv.Value = 1;
			scrollv.SmallChange = 1;
			scrollv.LargeChange = 2;
			page2.Children.Add(scrollv);

			// Cr�e l'onglet 3.
			TabPage page3 = new TabPage();
			page3.Name = "p3";
			page3.TabTitle = "<m>T</m>roisi�me";
			tb.Items.Add(page3);

			StaticText st = new StaticText();
			st.Name = "Static";
			st.Location = new Point(50, 130);
			st.Size = new Size(200, 15);
			st.Text = "<b>Onglet</b> volontairement <i>vide</i> !";
			page3.Children.Add(st);

			// Cr�e l'onglet 4.
			TabPage page4 = new TabPage();
			page4.Name = "p4";
			page4.TabTitle = "<m>L</m>ook";
			tb.Items.Add(page4);

			this.CreateListLook(page4, 10, 10, null, -1);

			StaticText link = new StaticText();
			link.Name = "Link";
			link.Location = new Point(10, 50);
			link.Size = new Size(200, 15);
			link.Text = "Voir sur <a href=\"www.epsitec.ch\">www.epsitec.ch</a> !";
			page4.Children.Add(link);

			// Cr�e l'onglet 5.
			TabPage page5 = new TabPage();
			page5.Name = "p5";
			page5.TabTitle = "<m>A</m>dd";
			tb.Items.Add(page5);

			Button add = new Button();
			add.Name = "Add";
			add.Location = new Point(100, 100);
			add.Size = new Size(140, 24);
			add.Text = "<m>A</m>jouter un onglet";
			add.ButtonStyle = ButtonStyle.DefaultAccept;
			add.Clicked += new MessageEventHandler(this.HandleAdd);
			page5.Children.Add(add);

#if true
			// Cr�e l'onglet 6.
			TabPage page6 = new TabPage();
			page6.Name = "p6";
			page6.TabTitle = "Titre long";
			tb.Items.Add(page6);

			// Cr�e l'onglet 7.
			TabPage page7 = new TabPage();
			page7.Name = "p7";
			page7.TabTitle = "Titre assez long";
			tb.Items.Add(page7);

			// Cr�e l'onglet 8.
			TabPage page8 = new TabPage();
			page8.Name = "p8";
			page8.TabTitle = "Titre encore plus long";
			tb.Items.Add(page8);
#endif

			tb.ActivePage = page1;

			window.FocusedWidget = tb;

			window.Show();
		}

		private void HandleWindowClosed(object sender)
		{
			this.tabBook = null;
		}

		private void HandleAdd(object sender, MessageEventArgs e)
		{
			Rectangle inside = this.tabBook.InnerBounds;
			TabPage page = new TabPage();
			page.Bounds = inside;
			page.TabTitle = "Nouveau";
			this.tabBook.Items.Add(page);
		}

		[Test] public void CheckAdornerCell1()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckAdornerCell1";

			this.CreateListLook(window.Root, 10, 10, null, -1);

			StaticText title = new StaticText();
//			title.Location = new Point(120, 245);
			title.Size = new Size(280, 15);
			title.Text = "S�lections possibles avec Ctrl et/ou Shift :";
			title.Anchor = AnchorStyles.TopLeft;
			title.AnchorMargins = new Margins(120, 0, 55, 0);
			window.Root.Children.Add(title);

			CellTable table = new CellTable();
			table.StyleH  = CellArrayStyle.ScrollNorm;
			table.StyleH |= CellArrayStyle.Separator;
			table.StyleH |= CellArrayStyle.SelectCell;
			table.StyleH |= CellArrayStyle.SelectMulti;
			table.StyleV  = CellArrayStyle.ScrollNorm;
			table.StyleV |= CellArrayStyle.Separator;
			table.Name = "Table";
			table.Location = new Point(10, 20);
			table.Size = new Size(380, 200);
			table.SetArraySize(5, 12);
			for ( int y=0 ; y<12 ; y++ )
			{
				for ( int x=0 ; x<5 ; x++ )
				{
					StaticText text = new StaticText();
					text.PaintTextStyle = PaintTextStyle.Array;
					if ( x != 0 || y != 0 )  text.Text = string.Format("{0}.{1}", y+1, x+1);
					text.Alignment = ContentAlignment.MiddleCenter;
					text.Dock = Widgets.DockStyle.Fill;
					
					if ( x == 2 && y == 2 )
					{
						CheckButton widget = new CheckButton();
						widget.Text = "surprise";
						widget.Dock = Widgets.DockStyle.Fill;
						table[x,y].Insert(widget);
					}
					else if ( x == 3 && y == 3 )
					{
						Button widget = new Button();
						widget.Text = "OK";
						widget.Dock = Widgets.DockStyle.Fill;
						table[x,y].Insert(widget);
					}
					else if ( x == 1 && y == 4 )
					{
						TextField widget = new TextField();
						widget.Text = "Standard";
						widget.Dock = Widgets.DockStyle.Fill;
						table[x,y].Insert(widget);
					}
					else if ( x == 2 && y == 5 )
					{
						TextField widget = new TextField();
						widget.TextFieldStyle = TextFieldStyle.Flat;
						widget.Text = "Flat";
						widget.Dock = Widgets.DockStyle.Fill;
						table[x,y].Insert(widget);
					}
					else if ( x == 1 && y == 6 )
					{
						TextField widget = new TextField();
						widget.TextFieldStyle = TextFieldStyle.Flat;
						widget.BackColor = Color.Transparent;
						widget.Text = "Flat/Transparent";
						widget.Dock = Widgets.DockStyle.Fill;
						table[x,y].Insert(widget);
					}
					else if ( x != 1 || y != 1 )
					{
						table[x,y].Insert(text);
					}
				}
			}
			table.Anchor = AnchorStyles.All;
			table.AnchorMargins = new Margins(10, 10, 80, 10);
			window.Root.Children.Add(table);

			window.FocusedWidget = table;

			window.Show();
		}

		[Test] public void CheckAdornerCell2()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(500, 300);
			window.Text = "CheckAdornerCell2";

			this.CreateListLook(window.Root, 10, 10, null, -1);

			StaticText title = new StaticText();
			title.Location = new Point(120, 245);
			title.Size = new Size(380, 15);
			title.Text = "Tableau de lignes editables et redimensionnable :";
			title.Anchor = AnchorStyles.TopLeft;
			title.AnchorMargins = new Margins(120, 0, 55, 0);
			window.Root.Children.Add(title);

			CellTable table = new CellTable();
			table.StyleH  = CellArrayStyle.Stretch;
			table.StyleH |= CellArrayStyle.Header;
			table.StyleH |= CellArrayStyle.Separator;
			table.StyleH |= CellArrayStyle.Mobile;
			table.StyleV  = CellArrayStyle.ScrollNorm;
			table.StyleV |= CellArrayStyle.Separator;
			table.DefHeight = 20;
			table.Name = "Table";
			table.Location = new Point(10, 20);
			table.Size = new Size(480, 200);
			table.SetArraySize(5, 6);
			table.SetWidthColumn(0, 30);
			table.SetWidthColumn(1, 200);
			table.SetWidthColumn(2, 50);
			table.SetWidthColumn(3, 50);
			table.SetWidthColumn(4, 50);
			table.SetHeaderTextH(0, "Nb");
			table.SetHeaderTextH(1, "Article");
			table.SetHeaderTextH(2, "TVA");
			table.SetHeaderTextH(3, "Prix");
			table.SetHeaderTextH(4, "Total");

			string[] texts =
			{
				"1",	"Tuyau BX-35",			"7.5",	"35.00",	"35.00",
				"1",	"Raccord 23'503",		"7.5",	"2.50",		"2.50",
				"20",	"Ecrou M8",				"7.5",	"0.50",		"10.00",
				"5",	"Peinture acrylique",	"7.5",	"15.00",	"75.00",
				"1",	"Equerre 30x50",		"7.5",	"12.00",	"12.00",
				"",		"",						"",		"",			"",
			};

			for ( int y=0 ; y<6 ; y++ )
			{
				for ( int x=0 ; x<5 ; x++ )
				{
					TextField text = new TextField();
					text.TextFieldStyle = TextFieldStyle.Flat;
					if ( x != 1 )
					{
						text.Alignment = ContentAlignment.MiddleRight;
					}
					text.Text = texts[y*5+x];
					text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					table[x,y].Insert(text);
				}
			}
			table.Anchor = AnchorStyles.All;
			table.AnchorMargins = new Margins(10, 10, 80, 10);
			window.Root.Children.Add(table);

			window.FocusedWidget = table;

			window.Show();
		}

		[Test] public void CheckAdornerCell3()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckAdornerCell3";

			this.CreateListLook(window.Root, 10, 10, null, -1);

			StaticText title = new StaticText();
			title.Location = new Point(120, 245);
			title.Size = new Size(280, 15);
			title.Text = "Tableau redimensionnable non �ditable :";
			title.Anchor = AnchorStyles.TopLeft;
			title.AnchorMargins = new Margins(120, 0, 55, 0);
			window.Root.Children.Add(title);

			CellTable table = new CellTable();
			table.StyleH  = CellArrayStyle.ScrollNorm;
			table.StyleH |= CellArrayStyle.Header;
			table.StyleH |= CellArrayStyle.Separator;
			table.StyleH |= CellArrayStyle.Mobile;
			table.StyleH |= CellArrayStyle.Sort;
			table.StyleV  = CellArrayStyle.ScrollNorm;
			table.StyleV |= CellArrayStyle.Header;
			table.StyleV |= CellArrayStyle.Separator;
			table.StyleV |= CellArrayStyle.SelectLine;
			table.StyleV |= CellArrayStyle.SelectMulti;
			table.StyleV |= CellArrayStyle.Mobile;
			table.StyleV |= CellArrayStyle.Sort;
			table.Name = "Table";
			table.Location = new Point(10, 20);
			table.Size = new Size(380, 200);
			table.SetArraySize(5, 12);

			table.SetHeaderTextH(0, "A");
			table.SetHeaderTextH(1, "B");
			table.SetHeaderTextH(2, "C");
			table.SetHeaderTextH(3, "D");
			table.SetHeaderTextH(4, "E");

			table.SetHeaderTextV(0, "1");
			table.SetHeaderTextV(1, "2");
			table.SetHeaderTextV(2, "3");
			table.SetHeaderTextV(3, "4");
			table.SetHeaderTextV(4, "5");
			table.SetHeaderTextV(5, "6");
			table.SetHeaderTextV(6, "7");
			table.SetHeaderTextV(7, "8");
			table.SetHeaderTextV(8, "9");
			table.SetHeaderTextV(9, "10");
			table.SetHeaderTextV(10, "11");
			table.SetHeaderTextV(11, "12");

			for ( int y=0 ; y<12 ; y++ )
			{
				for ( int x=0 ; x<5 ; x++ )
				{
#if false
					if ( x == 0 && y == 0 )
					{
						StaticText text = new StaticText();
						text.Text = "BUG";
						text.Alignment = ContentAlignment.BottomLeft;
						text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
						table[x,y].Insert(text);
					}
#else
					StaticText text = new StaticText();
					text.PaintTextStyle = PaintTextStyle.Array;
					text.Text = string.Format("L{0} C{1}", x+1, y+1);
					text.Alignment = ContentAlignment.MiddleLeft;
					//text.Alignment = ContentAlignment.BottomLeft;
					//text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					text.Dock = Widgets.DockStyle.Fill;
					table[x,y].Insert(text);
#endif
				}
			}
			table.Anchor = AnchorStyles.All;
			table.AnchorMargins = new Margins(10, 10, 80, 10);
			window.Root.Children.Add(table);

			window.FocusedWidget = table;

			window.Show();
		}

		[Test] public void CheckAdornerScrollArray()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckAdornerScrollArray";

			this.CreateListLook(window.Root, 10, 10, null, -1);

			StaticText title = new StaticText();
			title.Location = new Point(120, 245);
			title.Size = new Size(280, 15);
			title.Text = "Tableau rapide pour liste de gauche :";
			title.Anchor = AnchorStyles.TopLeft;
			title.AnchorMargins = new Margins(120, 0, 55, 0);
			window.Root.Children.Add(title);

			ScrollArray table = new ScrollArray();
			table.Location = new Point(10, 20);
			table.Size = new Size(380, 200);
			table.ColumnCount = 5;
			for ( int x=0 ; x<table.ColumnCount ; x++ )
			{
				string s = "C"+(x+1);
				table.SetHeaderText(x, s);
				table.SetColumnWidth(x, 80);
				//table.SetAlignmentColumn(x, ContentAlignment.MiddleCenter);
			}
			for ( int y=0 ; y<100 ; y++ )
			{
				for ( int x=0 ; x<table.ColumnCount ; x++ )
				{
					table[y, x] = string.Format ("Val {0}.{1}", y+1, x+1);
				}
			}
			//table.AdjustHeight(Widgets.ScrollArrayAdjust.MoveDown);
			//table.AdjustHeightToContent(Widgets.ScrollArrayAdjust.MoveDown, 10, 1000);
			table.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			table.AnchorMargins = new Margins(10, 10, 80, 10);
			window.Root.Children.Add(table);

			window.FocusedWidget = table;

			window.Show();
		}


		
		// Cr�e la liste pour changer de look.
		protected void CreateListLook(Widget parent, double mx, double my, ToolTip tooltip, int tab)
		{
			ScrollList sl = new ScrollList();
			
			sl.Parent = parent;
			sl.Size = new Size(100, 64);
			sl.Anchor = AnchorStyles.TopLeft;
			sl.AnchorMargins = new Margins(mx, 0, my, 0);
			sl.AdjustHeight(ScrollAdjustMode.MoveDown);
			if ( tab != -1 )
			{
				sl.TabIndex = tab;
				sl.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			string[] list = Widgets.Adorner.Factory.AdornerNames;
			int i = 0;
			int sel = 0;
			foreach ( string name in list )
			{
				sl.Items.Add(name);
				if ( name == Widgets.Adorner.Factory.ActiveName )  sel = i;
				i ++;
			}

			sl.SelectedIndex = sel;
			sl.ShowSelected(ScrollShowMode.Center);
			sl.SelectedIndexChanged += new EventHandler(this.HandleLook);

			if ( tooltip != null )
			{
				tooltip.SetToolTip(sl, "Choix du look de l'interface");
			}
		}

		private void HandleLook(object sender)
		{
			ScrollList sl = sender as ScrollList;
			int sel = sl.SelectedIndex;
			Widgets.Adorner.Factory.SetActive(sl.Items[sel]);
		}
		
		[Test] public void CheckAdornerPaneBook1()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(500, 300);
			window.Text = "CheckAdornerPaneBook1";
			window.Root.DockPadding = new Margins(10, 10, 10, 10);

			PaneBook book = new PaneBook();
			book.PaneBookStyle = PaneBookStyle.LeftRight;
			book.PaneBehaviour = PaneBookBehaviour.FollowMe;
			//book.PaneBehaviour = PaneBookBehaviour.Draft;
			book.Dock = DockStyle.Fill;
			book.Parent = window.Root;

			PanePage p1 = new PanePage();
			p1.PaneRelativeSize = 20;
			p1.PaneMinSize = 50;
			p1.PaneElasticity = 0;
			book.Items.Add(p1);

			Button button1 = new Button();
			button1.Location = new Point(10, 10);
			button1.Width = p1.Width-20;
			button1.Height = p1.Height-20;
			button1.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button1.Text = "P1";
			p1.Children.Add(button1);

			PanePage p2 = new PanePage();
			p2.PaneRelativeSize = 20;
			p2.PaneMinSize = 50;
			p2.PaneMaxSize = 200;
			p2.PaneElasticity = 0;
			p2.PaneToggle = true;
			book.Items.Add(p2);

			PanePage p3 = new PanePage();
			p3.PaneRelativeSize = 20;
			p3.PaneMinSize = 50;
			p3.PaneElasticity = 1;
			book.Items.Add(p3);

			Button button3 = new Button();
#if true
			button3.Dock = DockStyle.Fill;
			p3.DockPadding = new Margins (10, 10, 10, 10);
#else
			button3.Location = new Point(10, 10);
			button3.Width = p3.Width-20;
			button3.Height = p3.Height-20;
			button3.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
#endif
			button3.Text = "P3";
			p3.Children.Add(button3);

			PanePage p4 = new PanePage();
			p4.PaneRelativeSize = 40;
			p4.PaneMinSize = 50;
			p4.PaneElasticity = 1;
			book.Items.Add(p4);

			Button button4 = new Button();
			button4.Location = new Point(10, 10);
			button4.Width = p4.Width-20;
			button4.Height = p4.Height-20;
			button4.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button4.Text = "P4";
			p4.Children.Add(button4);

			// -----
			PaneBook bookv = new PaneBook();
			bookv.Location = new Point(0, 0);
			bookv.Size = p2.Size;
			bookv.PaneBookStyle = PaneBookStyle.BottomTop;
			bookv.PaneBehaviour = PaneBookBehaviour.FollowMe;
			//bookv.PaneBehaviour = PaneBookBehaviour.Draft;
			bookv.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			p2.Children.Add(bookv);

			PanePage v1 = new PanePage();
			v1.PaneRelativeSize = 30;
			v1.PaneMinSize = 50;
			bookv.Items.Add(v1);

			Button buttonv1 = new Button();
			buttonv1.Location = new Point(10, 10);
			buttonv1.Width = v1.Width-20;
			buttonv1.Height = v1.Height-20;
			buttonv1.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			buttonv1.Text = "P2.1";
			v1.Children.Add(buttonv1);

			PanePage v2 = new PanePage();
			v2.PaneRelativeSize = 70;
			v2.PaneMinSize = 50;
			bookv.Items.Add(v2);

			Button buttonv2 = new Button();
			buttonv2.Location = new Point(10, 10);
			buttonv2.Width = v2.Width-20;
			buttonv2.Height = v2.Height-20;
			buttonv2.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			buttonv2.Text = "P2.2";
			v2.Children.Add(buttonv2);

			window.Show();
		}

		[Test] public void CheckAdornerPaneBook2()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(500, 300);
			window.Text = "CheckAdornerPaneBook2";
			window.Root.DockPadding = new Margins(10, 10, 10, 10);

			PaneBook book = new PaneBook();
			book.PaneBookStyle = PaneBookStyle.LeftRight;
			book.PaneBehaviour = PaneBookBehaviour.FollowMe;
			book.Dock = DockStyle.Fill;
			book.Parent = window.Root;

			PanePage p1 = new PanePage();
			p1.PaneRelativeSize = 10;
			p1.PaneHideSize = 50;
			p1.PaneElasticity = 0;
			book.Items.Add(p1);

			Button button1 = new Button();
			button1.Location = new Point(10, 10);
			button1.Width = p1.Width-20;
			button1.Height = p1.Height-20;
			button1.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button1.Text = "P1";
			p1.Children.Add(button1);

			PanePage p2 = new PanePage();
			p2.PaneRelativeSize = 10;
			p2.PaneHideSize = 50;
			p2.PaneElasticity = 1;
			book.Items.Add(p2);

			Button button2 = new Button();
			button2.Location = new Point(10, 10);
			button2.Width = p2.Width-20;
			button2.Height = p2.Height-20;
			button2.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button2.Text = "P2";
			p2.Children.Add(button2);

			window.Show();
		}


		[Test] public void CheckAdornerPaneBook3()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(500, 300);
			window.Text = "CheckAdornerPaneBook3";
			window.Root.DockPadding = new Margins(10, 10, 10, 10);

			PaneBook book = new PaneBook();
			book.PaneBookStyle = PaneBookStyle.LeftRight;
			book.PaneBehaviour = PaneBookBehaviour.FollowMe;
			book.Dock = DockStyle.Fill;
			book.Parent = window.Root;

			PanePage p1 = new PanePage();
			p1.PaneRelativeSize = 10;
			p1.PaneMinSize = 50;
			p1.PaneElasticity = 1;
			book.Items.Add(p1);

			Button button1 = new Button();
			button1.Location = new Point(10, 10);
			button1.Width = p1.Width-20;
			button1.Height = p1.Height-20;
			button1.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button1.Text = "P1";
			p1.Children.Add(button1);

			PanePage p2 = new PanePage();
			p2.PaneRelativeSize = 10;
			p2.PaneAbsoluteSize = 200;
			p2.PaneMinSize = 50;
			p2.PaneElasticity = 0;
			book.Items.Add(p2);

			Button button2 = new Button();
			button2.Location = new Point(10, 10);
			button2.Width = p2.Width-20;
			button2.Height = p2.Height-20;
			button2.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button2.Text = "P2";
			p2.Children.Add(button2);

			window.Show();
		}
		
		private void HandleTextExEditionAccepted(object sender)
		{
			TextFieldEx text = sender as TextFieldEx;
			text.SelectAll ();
		}

		private void HandleTextExEditionRejected(object sender)
		{
			TextFieldEx text = sender as TextFieldEx;
			text.Text = "&lt;rejected&gt;";
			text.SelectAll ();
		}


		protected TabBook			tabBook;
		protected TextFieldMulti	bigText;

	}
}
