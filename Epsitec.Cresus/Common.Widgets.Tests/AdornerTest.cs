using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class AdornerTest
	{
		[Test] public void CheckAdornerWidgets()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(600, 300);
			window.Text = "CheckAdornerWidgets";

			ToolTip tip = new ToolTip();

			Button a = new Button();
			a.Location = new Point(10, 10);
			a.Width = 75;
			a.Text = "O<m>K</m>";
			a.ButtonStyle = ButtonStyle.DefaultActive;
			a.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(a);
			tip.SetToolTip(a, "C'est d'accord, tout baigne");

			Button b = new Button();
			b.Location = new Point(95, 10);
			b.Width = 75;
			b.Text = "<m>A</m>nnuler";
			b.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(b);
			tip.SetToolTip(b, "Annule tout<br/>Deuxieme ligne, juste pour voir !");

			Button c = new Button();
			c.Location = new Point(95+150, 10);
			c.Width = 75;
			c.Text = "Ai<m>d</m>e";
			c.SetEnabled(false);
			c.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(c);
			tip.SetToolTip(c, "Au secours !");

			StaticText st = new StaticText();
			st.Location = new Point(10, 265);
			st.Width = 150;
			st.Text = @"Choix du <b>look</b> de l'<i>interface</i> :";
			st.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			window.Root.Children.Add(st);

			StaticText link = new StaticText();
			link.Location = new Point(360, 16);
			link.Width = 200;
			link.Text = @"Visitez notre <a href=""http://www.epsitec.ch"">site web</a> !";
			link.Anchor = AnchorStyles.Bottom|AnchorStyles.Right;
			link.HyperTextClicked += new MessageEventHandler(link_HyperTextClicked);
			window.Root.Children.Add(link);

			CreateListLook(window.Root.Children, new Point(10, 195), tip);

			CheckButton check = new CheckButton();
			check.Location = new Point(10, 50);
			check.Width = 100;
			check.Text = "<m>C</m>ochez ici";
			check.ActiveState = WidgetState.ActiveYes;
			check.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			check.Clicked += new MessageEventHandler(this.HandleCheck);
			window.Root.Children.Add(check);
			tip.SetToolTip(check, "Juste pour voir");

			GroupBox box = new GroupBox();
			box.Location = new Point(10, 80);
			box.Size = new Size(100, 75);
			box.Text = "Couleur";
			box.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(box);

			RadioButton radio1 = new RadioButton();
			radio1.Location = new Point(10, 40);
			radio1.Width = 80;
			radio1.Text = "<m>R</m>ouge";
			radio1.ActiveState = WidgetState.ActiveYes;
			radio1.Group = "RGB";
			radio1.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			radio1.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio1);
			tip.SetToolTip(radio1, "Couleur rouge");

			RadioButton radio2 = new RadioButton();
			radio2.Location = new Point(10, 25);
			radio2.Width = 80;
			radio2.Text = "<m>V</m>ert";
			radio2.Group = "RGB";
			radio2.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			radio2.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio2);
			tip.SetToolTip(radio2, "Couleur verte");

			RadioButton radio3 = new RadioButton();
			radio3.Location = new Point(10, 10);
			radio3.Width = 80;
			radio3.Text = "<m>B</m>leu";
			radio3.Group = "RGB";
			radio3.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			radio3.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio3);
			tip.SetToolTip(radio3, "Couleur bleue");

			VScroller scrollv = new VScroller();
			scrollv.Location = new Point(120, 50);
			scrollv.Size = new Size(17, 120);
			scrollv.Range = 10;
			scrollv.Display = 3;
			scrollv.Value = 1;
			scrollv.SmallChange = 1;
			scrollv.LargeChange = 2;
			scrollv.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(scrollv);
			tip.SetToolTip(scrollv, "Ascenseur vertical");

			HScroller scrollh = new HScroller();
			scrollh.Location = new Point(140, 50);
			scrollh.Size = new Size(120, 17);
			scrollh.Range = 10;
			scrollh.Display = 7;
			scrollh.Value = 1;
			scrollh.SmallChange = 1;
			scrollh.LargeChange = 2;
			scrollh.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(scrollh);
			tip.SetToolTip(scrollh, "Ascenseur horizontal");

			TextFieldCombo combo = new TextFieldCombo();
			combo.Location = new Point(160, 180);
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
			combo.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(combo);

			TextField text = new TextField();
			text.Location = new Point(160, 150);
			text.Width = 100;
			text.Text = "Bonjour";
			text.Cursor = text.Text.Length;
			text.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(text);

			TextFieldUpDown tud = new TextFieldUpDown();
			tud.Location = new Point(160, 125);
			tud.Width = 50;
			tud.Value = 50;
			tud.MinRange = -100;
			tud.MaxRange = 100;
			tud.Step = 10;
			tud.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(tud);

			TextFieldMulti multi = new TextFieldMulti();
			multi.Location = new Point(160, 70);
			multi.Size = new Size(100, 50);
			multi.Text = "Ceci est une petite phrase ridicule.<br/>Mais elle est assez longue pour faire des essais.";
			multi.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(multi);

			ScrollList sl = new ScrollList();
			sl.Location = new Point(270, 70);
			sl.Size = new Size(90, 100);
			sl.AdjustHeight(ScrollListAdjust.MoveDown);
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
			sl.SelectedIndex = 5;  // sélectionne juin
			sl.ShowSelectedLine(ScrollListShow.Middle);
			sl.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(sl);
			tip.SetToolTip(sl, "Choix du mois");

			StaticText st2 = new StaticText();
			st2.Location = new Point(160, 220+4);
			st2.Width = 90;
			st2.Text = "Non editable :";
			st2.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			window.Root.Children.Add(st2);

			TextField textfix = new TextField();
			textfix.Location = new Point(260, 220);
			textfix.Width = 100;
			textfix.Text = "Texte fixe";
			textfix.IsReadOnly = true;
			textfix.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(textfix);

			TextFieldCombo combofix = new TextFieldCombo();
			combofix.Location = new Point(370, 220);
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
			combofix.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(combofix);


			ToolBar tb = new ToolBar();
			tb.Location = new Point(160, 260);
			tb.Width = 300;
			tb.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(tb);

			tb.SetSize(20);
			tb.InsertIconButton(@"file:images\open.png");
			tb.InsertIconButton(@"file:images\save.png");
			tb.InsertSep(5);

			TextFieldCombo t1 = new TextFieldCombo();
			t1.Width = 70;
			t1.Text = "Rouge";
			t1.Items.Add("red",   "Rouge");
			t1.Items.Add("green", "Vert");
			t1.Items.Add("blue",  "Bleu");

			tb.Insert(t1);
			tb.InsertSep(5);
			tb.InsertIconButton(@"file:images\cut.png");
			tb.InsertIconButton(@"file:images\copy.png");
			tb.InsertIconButton(@"file:images\paste.png");

#if true
			VMenu fileMenu = new VMenu();
			fileMenu.Name = "0 (root menu)";
			fileMenu.Items.Add(new MenuItem ("open", @"file:images\open.png", "Ouvrir...", "Ctrl+O"));
			fileMenu.Items.Add(new MenuItem ("save", @"file:images\save.png", "Enregistrer...", "Ctrl+S"));
			fileMenu.Items.Add(new MenuSeparator ());
			fileMenu.Items.Add(new MenuItem ("opt1", "", "Options 1", ""));
			fileMenu.Items.Add(new MenuItem ("opt2", "", "Options 2", ""));
			fileMenu.Items.Add (new MenuSeparator ());
			fileMenu.Items.Add(new MenuItem ("quit", "", "Quitter", ""));
			fileMenu.AdjustSize();
			fileMenu.Location = new Point(370, 70);
			window.Root.Children.Add(fileMenu);

			VMenu optMenu1 = new VMenu();
			optMenu1.Name = "1a";
			optMenu1.Items.Add(new MenuItem ("set1A", "", "Reglages 1.A", ""));
			optMenu1.Items.Add(new MenuItem ("set1B", "", "Reglages 1.B", ""));
			optMenu1.Items.Add(new MenuItem ("print", @"file:images\print.png", "Impression...", ""));
			optMenu1.Items.Add(new MenuItem ("open",  @"file:images\open.png", "Fichiers...", ""));
			optMenu1.AdjustSize();
			fileMenu.Items[3].Submenu = optMenu1;

			VMenu optMenu2 = new VMenu();
			optMenu2.Name = "1b";
			optMenu2.Items.Add(new MenuItem ("set2A", "", "Reglages 2.A", ""));
			optMenu2.Items.Add(new MenuItem ("set2B", "", "Reglages 2.B", ""));
			optMenu2.Items.Add(new MenuItem ("print", @"file:images\print.png", "Impression...", ""));
			optMenu2.Items.Add(new MenuItem ("open",  @"file:images\open.png", "Fichiers...", ""));
			optMenu2.AdjustSize();
			fileMenu.Items[4].Submenu = optMenu2;

			VMenu setupMenu1A = new VMenu();
			setupMenu1A.Name = "2a";
			setupMenu1A.Items.Add(new MenuItem ("set1Aa", "", "Reglage 1.A.a", ""));
			setupMenu1A.Items.Add(new MenuItem ("set1Ab", "", "Reglage 1.A.b", ""));
			setupMenu1A.Items.Add(new MenuItem ("set1Ac", "", "Reglage 1.A.c", ""));
			setupMenu1A.Items.Add(new MenuItem ("set1Ad", "", "Reglage 1.A.d", ""));
			setupMenu1A.AdjustSize();
			optMenu1.Items[0].Submenu = setupMenu1A;

			VMenu setupMenu1B = new VMenu();
			setupMenu1B.Name = "2b";
			setupMenu1B.Items.Add(new MenuItem ("set1Ba", "", "Reglage 1.B.a", ""));
			setupMenu1B.Items.Add(new MenuItem ("set1Bb", "", "Reglage 1.B.b", ""));
			setupMenu1B.Items.Add(new MenuItem ("set1Bc", "", "Reglage 1.B.c", ""));
			setupMenu1B.Items.Add(new MenuItem ("set1Bd", "", "Reglage 1.B.d", ""));
			setupMenu1B.AdjustSize();
			optMenu1.Items[1].Submenu = setupMenu1B;

			VMenu setupMenu2A = new VMenu();
			setupMenu2A.Name = "2c";
			setupMenu2A.Items.Add(new MenuItem ("set2Aa", "", "Reglage 2.A.a", ""));
			setupMenu2A.Items.Add(new MenuItem ("set2Ab", "", "Reglage 2.A.b", ""));
			setupMenu2A.Items.Add(new MenuItem ("set2Ac", "", "Reglage 2.A.c", ""));
			setupMenu2A.Items.Add(new MenuItem ("set2Ad", "", "Reglage 2.A.d", ""));
			setupMenu2A.AdjustSize();
			optMenu2.Items[0].Submenu = setupMenu2A;

			VMenu setupMenu2B = new VMenu();
			setupMenu2B.Name = "2d";
			setupMenu2B.Items.Add(new MenuItem ("set2Ba", "", "Reglage 2.B.a", ""));
			setupMenu2B.Items.Add(new MenuItem ("set2Bb", "", "Reglage 2.B.b", ""));
			setupMenu2B.Items.Add(new MenuItem ("set2Bc", "", "Reglage 2.B.c", ""));
			setupMenu2B.Items.Add(new MenuItem ("set2Bd", "", "Reglage 2.B.d", ""));
			setupMenu2B.AdjustSize();
			optMenu2.Items[1].Submenu = setupMenu2B;
#endif

			window.FocusedWidget = a;

			window.Show();
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

		private void link_HyperTextClicked(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			System.Diagnostics.Process.Start ("IExplore.exe", widget.HyperText);
		}


		[Test] public void CheckAdornerBigText()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerBigText";

			TextFieldMulti multi = new TextFieldMulti();
			multi.Name = "Multi";
			multi.Location = new Point(10, 10);
			multi.Size = new Size(380, 280);
			multi.MaxChar = 10000;
			string s = "";
#if true
			s += "On donnait ce jour-là un grand dîner, où, pour la première fois, je vis avec beaucoup d'étonnement le maître d'hôtel servir l'épée au côté et le chapeau sur la tête. Par hasard on vint à parler de la devise de la maison de Solar, qui était sur la tapisserie avec les armoiries: Tel fiert qui ne tue pas. Comme les Piémontais ne sont pas pour l'ordinaire consommés par la langue française, quelqu'un trouva dans cette devise une faute d'orthographe, et dit qu'au mot fiert il ne fallait point de t.<br/>";
			s += "Le vieux comte de Gouvon allait répondre; mais ayant jeté les yeux sur moi, il vit que je souriait sans oser rien dire: il m'ordonna de parler. Alors je dis que je ne croyait pas que le t fût de trop, que fiert était un vieux mots français qui ne venait pas du nom ferus, fier, menaçant, mais du verbe ferit, il frappe, il blesse; qu'ainsi la devise ne me paraissait pas dire: Tel menace, mais tel frappe qui ne tue pas.<br/>";
			s += "Tout le monde me regardait et se regardait sans rien dire. On ne vit de la vie un pareil étonnement. Mais ce qui me flatta davantage fut de voir clairement sur le visage de Mlle de Breil un air de satisfaction. Cette personne si dédaigneuse daigna me jeter un second regard qui valait tout au moins le premier; puis, tournant les yeux vers son grand-papa, elle semblait attendre avec une sorte d'impatience la louange qu'il me devait, et qu'il me donna en effet si pleine et entière et d'un air si content, que toute la table s'empressa de faire chorus. Ce moment fut cours, mais délicieux à tous égards. Ce fut un de ces moments trop rares qui replacent les choses dans leur ordre naturel, et vengent le mérite avili des outrages de la fortune. FIN";
#else
			s += "aa<br/><br/>bb";
#endif
			multi.Text = s;
			//multi.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			multi.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(multi);
			window.Root.DebugActive = true;

			window.FocusedWidget = multi;

			window.Show();
		}

		[Test] public void CheckAdornerTab()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerTab";
			window.Closed += new System.EventHandler(this.HandleWindowClosed);

			TabBook tb = new TabBook(TabBookStyle.Normal);
			//TabBook tb = new TabBook(TabBookStyle.Right);
			tb.Name = "TabBook";
			tb.Location = new Point(10, 10);
			tb.Size = new Size(380, 280);
//PA			tb.Size = new Size(280, 380);
//PA			tb.SetClientAngle (90);
			tb.Text = "";
			tb.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(tb);
			this.tabBook = tb;  // TODO: fait qu'il reste TabBook dans la liste "alive" !

			Rectangle inside = tb.Inside;

			// Crée l'onglet 1.
			TabPage page1 = new TabPage();
			page1.Name = "p1";
			page1.Bounds = inside;
			page1.TabTitle = "<m>P</m>remier";
			tb.Add(page1);

			Button a = new Button();
			a.Name = "A";
			a.Location = new Point(10, 10);
			a.Size = new Size(75, 24);
			a.Text = "O<m>K</m>";
			a.ButtonStyle = ButtonStyle.DefaultActive;
			//a.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			page1.Children.Add(a);

			Button b = new Button();
			b.Name = "B";
			b.Location = new Point(95, 10);
			b.Size = new Size(75, 24);
			b.Text = "<m>A</m>nnuler";
			//b.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			page1.Children.Add(b);

			TextFieldMulti multi = new TextFieldMulti();
			multi.Name = "Multi";
			multi.Location = new Point(10, 45);
			multi.Size = new Size(350, 200);
			multi.Text = "1. Introduction<br/><br/>Les onglets permettent de mettre beaucoup de widgets sur une petite surface, ce qui s'avère extrèmement utile et diablement pratique.<br/><br/>2. Conclusion<br/><br/>Un truc chouette, qui sera certainement très utile dans le nouveau Crésus !";
			//multi.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			page1.Children.Add(multi);

			// Crée l'onglet 2.
			TabPage page2 = new TabPage();
			page2.Name = "p2";
			page2.Bounds = inside;
			page2.TabTitle = "<m>D</m>euxieme";
			tb.Add(page2);

			VScroller scrollv = new VScroller();
			scrollv.Name = "Scroller";
			scrollv.Location = new Point(10, 10);
			scrollv.Size = new Size(17, inside.Height-20);
			scrollv.Range = 10;
			scrollv.Display = 3;
			scrollv.Value = 1;
			scrollv.SmallChange = 1;
			scrollv.LargeChange = 2;
			//scrollv.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			page2.Children.Add(scrollv);

			// Crée l'onglet 3.
			TabPage page3 = new TabPage();
			page3.Name = "p3";
			page3.Bounds = inside;
			page3.TabTitle = "<m>T</m>roisieme";
			tb.Add(page3);

			StaticText st = new StaticText();
			st.Name = "Static";
			st.Location = new Point(50, 130);
			st.Size = new Size(200, 15);
			st.Text = "<b>Onglet</b> volontairement <i>vide</i> !";
			//st.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			page3.Children.Add(st);

			// Crée l'onglet 4.
			TabPage page4 = new TabPage();
			page4.Name = "p4";
			page4.Bounds = inside;
			page4.TabTitle = "<m>L</m>ook";
			tb.Add(page4);

			CreateListLook(page4.Children, new Point(10, 95), null);

			StaticText link = new StaticText();
			link.Name = "Link";
			link.Location = new Point(10, 50);
			link.Size = new Size(200, 15);
			link.Text = "Voir sur <a href=\"www.epsitec.ch\">www.epsitec.ch</a> !";
			//link.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			page4.Children.Add(link);

			// Crée l'onglet 5.
			TabPage page5 = new TabPage();
			page5.Name = "p5";
			page5.Bounds = inside;
			page5.TabTitle = "<m>A</m>dd";
			tb.Add(page5);

			Button add = new Button();
			add.Name = "Add";
			add.Location = new Point(100, 100);
			add.Size = new Size(140, 24);
			add.Text = "<m>A</m>jouter un onglet";
			add.ButtonStyle = ButtonStyle.DefaultActive;
			add.Clicked += new MessageEventHandler(HandleAdd);
			//add.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			page5.Children.Add(add);

#if true
			// Crée l'onglet 6.
			TabPage page6 = new TabPage();
			page6.Name = "p6";
			page6.Bounds = inside;
			page6.TabTitle = "Titre long";
			tb.Add(page6);

			// Crée l'onglet 7.
			TabPage page7 = new TabPage();
			page7.Name = "p7";
			page7.Bounds = inside;
			page7.TabTitle = "Titre assez long";
			tb.Add(page7);

			// Crée l'onglet 8.
			TabPage page8 = new TabPage();
			page8.Name = "p8";
			page8.Bounds = inside;
			page8.TabTitle = "Titre encore plus long";
			tb.Add(page8);
#endif

			tb.ActivePage = page1;

			window.FocusedWidget = tb;

			window.Show();
		}

		private void HandleWindowClosed(object sender, System.EventArgs e)
		{
			this.tabBook = null;
		}

		private void HandleAdd(object sender, MessageEventArgs e)
		{
			Rectangle inside = this.tabBook.Inside;
			TabPage page = new TabPage();
			page.Bounds = inside;
			page.TabTitle = "Nouveau";
			this.tabBook.Add(page);
		}

		[Test] public void CheckAdornerCell1()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerCell1";

			CreateListLook(window.Root.Children, new Point(10, 230), null);

			StaticText title = new StaticText();
			title.Location = new Point(120, 245);
			title.Size = new Size(280, 15);
			title.Text = "Selections possibles avec Ctrl et/ou Shift :";
			title.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			window.Root.Children.Add(title);

			CellTable table = new CellTable();
			table.StyleH  = AbstractCellArrayStyle.ScrollNorm;
			table.StyleH |= AbstractCellArrayStyle.Separator;
			table.StyleH |= AbstractCellArrayStyle.SelectCell;
			table.StyleH |= AbstractCellArrayStyle.SelectMulti;
			table.StyleV  = AbstractCellArrayStyle.ScrollNorm;
			table.StyleV |= AbstractCellArrayStyle.Separator;
			table.Name = "Table";
			table.Location = new Point(10, 20);
			table.Size = new Size(380, 200);
			table.SetArraySize(5, 12);
			for ( int y=0 ; y<12 ; y++ )
			{
				for ( int x=0 ; x<5 ; x++ )
				{
					StaticText text = new StaticText();
					if ( x != 0 || y != 0 )  text.Text = string.Format("{0}.{1}", y+1, x+1);
					text.Alignment = ContentAlignment.MiddleCenter;
					text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					
					if ( x == 2 && y == 2 )
					{
						CheckButton widget = new CheckButton();
						widget.Text = "surprise";
						widget.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
						table[x,y].Insert(widget);
					}
					else if ( x == 3 && y == 3 )
					{
						Button widget = new Button();
						widget.Text = "OK";
						widget.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
						table[x,y].Insert(widget);
					}
					else if ( x != 1 || y != 1 )
					{
						table[x,y].Insert(text);
					}
				}
			}
			table.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(table);

			window.FocusedWidget = table;

			window.Show();
		}

		[Test] public void CheckAdornerCell2()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(500, 300);
			window.Text = "CheckAdornerCell2";

			CreateListLook(window.Root.Children, new Point(10, 230), null);

			StaticText title = new StaticText();
			title.Location = new Point(120, 245);
			title.Size = new Size(380, 15);
			title.Text = "Tableau de lignes editables et redimensionnable :";
			title.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			window.Root.Children.Add(title);

			CellTable table = new CellTable();
			table.StyleH  = AbstractCellArrayStyle.Stretch;
			table.StyleH |= AbstractCellArrayStyle.Header;
			table.StyleH |= AbstractCellArrayStyle.Separator;
			table.StyleH |= AbstractCellArrayStyle.Mobile;
			table.StyleV  = AbstractCellArrayStyle.ScrollNorm;
			table.StyleV |= AbstractCellArrayStyle.Separator;
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
					Cell cell = new Cell();
					TextField text = new TextField();
					text.TextFieldStyle = TextFieldStyle.Flat;
					if ( x != 1 )
					{
						text.Alignment = ContentAlignment.MiddleRight;
					}
					text.Text = texts[y*5+x];
					text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					cell.Children.Add(text);
					table[x,y] = cell;
				}
			}
			table.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(table);

			window.FocusedWidget = table;

			window.Show();
		}

		[Test] public void CheckAdornerCell3()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerCell3";

			CreateListLook(window.Root.Children, new Point(10, 230), null);

			StaticText title = new StaticText();
			title.Location = new Point(120, 245);
			title.Size = new Size(280, 15);
			title.Text = "Tableau redimensionnable non editable :";
			title.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			window.Root.Children.Add(title);

			CellTable table = new CellTable();
			table.StyleH  = AbstractCellArrayStyle.ScrollNorm;
			table.StyleH |= AbstractCellArrayStyle.Header;
			table.StyleH |= AbstractCellArrayStyle.Separator;
			table.StyleH |= AbstractCellArrayStyle.Mobile;
			table.StyleH |= AbstractCellArrayStyle.Sort;
			table.StyleV  = AbstractCellArrayStyle.ScrollNorm;
			table.StyleV |= AbstractCellArrayStyle.Header;
			table.StyleV |= AbstractCellArrayStyle.Separator;
			table.StyleV |= AbstractCellArrayStyle.SelectLine;
			table.StyleV |= AbstractCellArrayStyle.Mobile;
			table.StyleV |= AbstractCellArrayStyle.Sort;
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
					text.Text = string.Format("L{0} C{1}", x+1, y+1);
					text.Alignment = ContentAlignment.MiddleLeft;
					//text.Alignment = ContentAlignment.BottomLeft;
					text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					table[x,y].Insert(text);
#endif
				}
			}
			table.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(table);

			window.FocusedWidget = table;

			window.Show();
		}

		[Test] public void CheckAdornerScrollArray()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerScrollArray";

			CreateListLook(window.Root.Children, new Point(10, 230), null);

			StaticText title = new StaticText();
			title.Location = new Point(120, 245);
			title.Size = new Size(280, 15);
			title.Text = "Tableau rapide pour liste de gauche :";
			title.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			window.Root.Children.Add(title);

			ScrollArray table = new ScrollArray();
			table.Location = new Point(10, 20);
			table.Size = new Size(380, 200);
			table.Columns = 5;
			for ( int x=0 ; x<table.Columns ; x++ )
			{
				string s = "C"+(x+1);
				table.SetHeaderText(x, s);
				table.SetWidthColumn(x, 80);
				//table.SetAlignmentColumn(x, ContentAlignment.MiddleCenter);
			}
			for ( int y=0 ; y<100 ; y++ )
			{
				for ( int x=0 ; x<table.Columns ; x++ )
				{
					string s = "Val ";
					s += y+1;
					s += ".";
					s += x+1;
					table.SetText(y, x, s);
				}
			}
			//table.AdjustHeight(Widgets.ScrollArrayAdjust.MoveDown);
			//table.AdjustHeightToContent(Widgets.ScrollArrayAdjust.MoveDown, 10, 1000);
			table.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(table);

			window.FocusedWidget = table;

			window.Show();
		}


		
		// Crée la liste pour changer de look.
		protected void CreateListLook(Widget.WidgetCollection collection, Point origine, ToolTip tooltip)
		{
			ScrollList sl = new ScrollList();
			sl.Location = origine;
			sl.Size = new Size(100, 64);
			sl.AdjustHeight(ScrollListAdjust.MoveDown);

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
			sl.ShowSelectedLine(ScrollListShow.Middle);
			sl.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			sl.SelectedIndexChanged += new EventHandler(this.HandleLook);
			collection.Add(sl);

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
			sl.RootParent.Invalidate();  // redessine toute la fenêtre
		}

		[Test] public void CheckAdornerBug1()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerBug1";

			Button button1 = new Button();
			//button1.Text = "";
			button1.Location = new Point(50, 50);
			button1.Size = new Size(100, 30);
			button1.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(button1);

			window.Show();
		}

		[Test] public void CheckAdornerBug2()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerBug2";

			TextField text = new TextField();
			text.Name = "TextField";
			text.Location = new Point(160, 150);
			text.Width = 100;
			text.Text = "Bonjour";
			text.Cursor = text.Text.Length;
			text.Alignment = ContentAlignment.MiddleRight;
			text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(text);

			window.Show();
		}

		[Test] public void CheckAdornerBugAlive1()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(600, 300);
			window.Text = "CheckAdornerBugAlive1";

			Button a = new Button();
			a.Location = new Point(10, 10);
			a.Width = 75;
			a.Text = "OK";
			window.Root.Children.Add(a);

			window.Show();
		}

		[Test] public void CheckAdornerBugAlive2()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerBugAlive2";

			CellTable table = new CellTable();
#if true
			table.StyleH  = AbstractCellArrayStyle.ScrollNorm;
			table.StyleH |= AbstractCellArrayStyle.Separator;
			table.StyleH |= AbstractCellArrayStyle.SelectCell;
			table.StyleH |= AbstractCellArrayStyle.SelectMulti;
			table.StyleV  = AbstractCellArrayStyle.ScrollNorm;
			table.StyleV |= AbstractCellArrayStyle.Separator;
#else
			table.StyleH  = AbstractCellArrayStyle.ScrollNorm;
			table.StyleH |= AbstractCellArrayStyle.Header;
			table.StyleH |= AbstractCellArrayStyle.Separator;
			table.StyleH |= AbstractCellArrayStyle.Mobile;
			table.StyleH |= AbstractCellArrayStyle.Sort;
			table.StyleV  = AbstractCellArrayStyle.ScrollNorm;
			table.StyleV |= AbstractCellArrayStyle.Header;
			table.StyleV |= AbstractCellArrayStyle.Separator;
			table.StyleV |= AbstractCellArrayStyle.SelectLine;
			table.StyleV |= AbstractCellArrayStyle.Mobile;
			table.StyleV |= AbstractCellArrayStyle.Sort;
#endif
			table.Name = "Table";
			table.Location = new Point(10, 20);
			table.Size = new Size(380, 200);
			table.SetArraySize(2, 2);
			for ( int y=0 ; y<2 ; y++ )
			{
				for ( int x=0 ; x<2 ; x++ )
				{
#if true
					TextField text = new TextField();
					text.TextFieldStyle = TextFieldStyle.Flat;
					text.Text = "abc";
					text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					table[x,y].Insert(text);
#else
					Cell cell = new Cell();
					TextField text = new TextField(TextFieldType.SingleLine);
					text.TextFieldStyle = TextFieldStyle.Flat;
					text.Text = "abc";
					text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					cell.Children.Add(text);
					table[x,y] = cell;
#endif
				}
			}
			table.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(table);

			window.Show();
		}

		[Test] public void CheckAdornerBugAlive3()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerTab";

			TabBook tb = new TabBook(TabBookStyle.Normal);
			//TabBook tb = new TabBook(TabBookStyle.Right);
			tb.Name = "TabBook";
			tb.Location = new Point(10, 10);
			tb.Size = new Size(380, 280);
			tb.Text = "";
			tb.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(tb);
			//this.tabBook = tb;

			Rectangle inside = tb.Inside;

			// Crée l'onglet 1.
			TabPage page1 = new TabPage();
			page1.Name = "p1";
			page1.Bounds = inside;
			page1.TabTitle = "<m>P</m>remier";
			tb.Add(page1);

#if true
			Button a = new Button();
			a.Name = "A";
			a.Location = new Point(10, 10);
			a.Size = new Size(75, 24);
			a.Text = "O<m>K</m>";
			a.ButtonStyle = ButtonStyle.DefaultActive;
			page1.Children.Add(a);
#endif

			// Crée l'onglet 2.
			TabPage page2 = new TabPage();
			page2.Name = "p2";
			page2.Bounds = inside;
			page2.TabTitle = "<m>D</m>euxieme";
			tb.Add(page2);

#if true
			VScroller scrollv = new VScroller();
			scrollv.Name = "Scroller";
			scrollv.Location = new Point(10, 10);
			scrollv.Size = new Size(17, inside.Height-20);
			scrollv.Range = 10;
			scrollv.Display = 3;
			scrollv.Value = 1;
			scrollv.SmallChange = 1;
			scrollv.LargeChange = 2;
			page2.Children.Add(scrollv);
#endif

			tb.ActivePage = page1;
			window.FocusedWidget = tb;

			window.Show();
		}

		[Test] public void CheckAdornerTestParents1()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerTestParents1";

			Button button1 = new Button();
			button1.Text = "Pere";
			button1.Location = new Point(50, 50);
			button1.Size = new Size(300, 200);
			button1.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(button1);

			Button button2 = new Button();
			button2.Text = "Fils";
			button2.Location = new Point(220, 10);
			button2.Size = new Size(100, 30);
			button2.Anchor = AnchorStyles.Left|AnchorStyles.Bottom;
			button1.Children.Add(button2);

			window.Show();
		}

		[Test] public void CheckAdornerTestParents2()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(300, 300);
			window.Text = "CheckAdornerTestParents2";

			Button button1 = new Button();
			button1.Location = new Point(50, 50);
			button1.Size = new Size(200, 200);
			button1.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button1.Name = "B1";
			button1.Parent = window.Root;
			
			Button button2 = new Button();
			button2.Location = new Point(50, 50);
			button2.Size = new Size(100, 100);
			button2.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button2.Name = "B2";
			button2.Parent = button1;

			Button button3 = new Button();
			button3.Location = new Point(20, 20);
			button3.Size = new Size(60, 60);
			button3.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button3.Name = "B3";
			button3.Parent = button2;

			Button button4 = new Button();
			button4.Location = new Point(20, 20);
			button4.Size = new Size(20, 20);
			button4.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button4.Name = "B4";
			button4.Parent = button3;

			window.Show();
		}

		[Test] public void CheckAdornerDisabled()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(600, 300);
			window.Text = "CheckAdornerDisabled";

			StaticText st = new StaticText();
			st.SetClientZoom(2.0);
			st.Location = new Point(10, 260);
			st.Width = 500;
			st.Height = 30;
			st.Text = "Teste tous les widgets dans l'etat disabled !";
			st.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			st.SetEnabled(false);
			window.Root.Children.Add(st);

			CreateListLook(window.Root.Children, new Point(10, 180), null);

			Button a = new Button();
			a.Location = new Point(10, 10);
			a.Width = 75;
			a.Text = "O<m>K</m>";
			a.SetEnabled(false);
			window.Root.Children.Add(a);

			CheckButton check = new CheckButton();
			check.Location = new Point(10, 50);
			check.Width = 100;
			check.Text = "<m>C</m>ochez ici";
			check.ActiveState = WidgetState.ActiveYes;
			check.SetEnabled(false);
			window.Root.Children.Add(check);

			GroupBox box = new GroupBox();
			box.Location = new Point(10, 80);
			box.Size = new Size(100, 75);
			box.Text = "Couleur";
			box.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			box.SetEnabled(false);
			window.Root.Children.Add(box);

			RadioButton radio1 = new RadioButton();
			radio1.Location = new Point(10, 40);
			radio1.Width = 80;
			radio1.Text = "<m>R</m>ouge";
			radio1.ActiveState = WidgetState.ActiveYes;
			radio1.Group = "RGB";
			radio1.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			radio1.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio1);

			RadioButton radio2 = new RadioButton();
			radio2.Location = new Point(10, 25);
			radio2.Width = 80;
			radio2.Text = "<m>V</m>ert";
			radio2.Group = "RGB";
			radio2.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			radio2.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio2);

			RadioButton radio3 = new RadioButton();
			radio3.Location = new Point(10, 10);
			radio3.Width = 80;
			radio3.Text = "<m>B</m>leu";
			radio3.Group = "RGB";
			radio3.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			radio3.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio3);

			TextFieldCombo combo = new TextFieldCombo();
			combo.Location = new Point(160, 180);
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
			combo.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			combo.SetEnabled(false);
			window.Root.Children.Add(combo);

			TextField text = new TextField();
			text.Location = new Point(160, 150);
			text.Width = 100;
			text.Text = "Bonjour";
			text.Cursor = text.Text.Length;
			text.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			text.SetEnabled(false);
			window.Root.Children.Add(text);

			TextFieldUpDown tud = new TextFieldUpDown();
			tud.Location = new Point(160, 125);
			tud.Width = 50;
			tud.Value = 50;
			tud.MinRange = -100;
			tud.MaxRange = 100;
			tud.Step = 10;
			tud.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			tud.SetEnabled(false);
			window.Root.Children.Add(tud);

			TextFieldMulti multi = new TextFieldMulti();
			multi.Location = new Point(160, 70);
			multi.Size = new Size(100, 50);
			multi.Text = "Ceci est une petite phrase ridicule.<br/>Mais elle est assez longue pour faire des essais.";
			multi.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			multi.SetEnabled(false);
			window.Root.Children.Add(multi);

			ScrollList sl = new ScrollList();
			sl.Location = new Point(270, 70);
			sl.Size = new Size(90, 100);
			sl.AdjustHeight(ScrollListAdjust.MoveDown);
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
			sl.SelectedIndex = 5;  // sélectionne juin
			sl.ShowSelectedLine(ScrollListShow.Middle);
			sl.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			sl.SetEnabled(false);
			window.Root.Children.Add(sl);

			ToolBar tb = new ToolBar();
			tb.Location = new Point(160, 220);
			tb.Width = 300;
			tb.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(tb);

			tb.SetSize(20);
			tb.InsertIconButton(@"file:images\open.png");
			tb.InsertIconButton(@"file:images\save.png");
			tb.InsertSep(5);
			tb.InsertIconButton(@"file:images\cut.png");
			tb.InsertIconButton(@"file:images\copy.png");
			tb.InsertIconButton(@"file:images\paste.png");
			tb[1].SetEnabled(false);

			VMenu fileMenu = new VMenu();
			fileMenu.Items.Add(new MenuItem ("open", @"file:images\open.png", "Ouvrir...", "Ctrl+O"));
			fileMenu.Items.Add(new MenuItem ("save", @"file:images\save.png", "Enregistrer...", "Ctrl+S"));
			fileMenu.Items.Add (new MenuSeparator ());
			fileMenu.Items.Add(new MenuItem ("quit", "", "Quitter", ""));
			fileMenu.AdjustSize();
			fileMenu.Location = new Point(370, 70);
			fileMenu.Items[1].SetEnabled(false);
			window.Root.Children.Add(fileMenu);

			window.Show();
		}


		protected TabBook	tabBook;
	}
}
