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
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerWidgets";

			Button a = new Button();
			a.Name = "A";
			a.Location = new Point(10, 10);
			a.Size = new Size(75, 24);
			a.Text = "O<m>K</m>";
			a.ButtonStyle = ButtonStyle.DefaultActive;
			a.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(a);

			Button b = new Button();
			b.Name = "B";
			b.Location = new Point(95, 10);
			b.Size = new Size(75, 24);
			b.Text = "<m>A</m>nnuler";
			b.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(b);

			Button c = new Button();
			c.Name = "C";
			c.Location = new Point(95+150, 10);
			c.Size = new Size(75, 24);
			c.Text = "Ai<m>d</m>e";
			c.SetEnabled(false);
			c.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(c);

			StaticText st = new StaticText();
			st.Name = "Static";
			st.Location = new Point(10, 265);
			st.Size = new Size(200, 15);
			st.Text = "Choix du <b>look</b> de l'<i>interface</i> :";
			st.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			window.Root.Children.Add(st);

			CreateRadioLook(window.Root.Children, new Point(10, 215));

			CheckButton check = new CheckButton();
			check.Name = "Check";
			check.Location = new Point(10, 50);
			check.Size = new Size(100, 13);
			check.Text = "<m>C</m>ochez ici";
			check.ActiveState = WidgetState.ActiveYes;
			check.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			check.Clicked += new MessageEventHandler(this.HandleCheck);
			window.Root.Children.Add(check);

			GroupBox box = new GroupBox();
			box.Name = "Box";
			box.Location = new Point(10, 80);
			box.Size = new Size(100, 75);
			box.Text = "Couleur";
			box.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(box);

			RadioButton radio1 = new RadioButton();
			radio1.Name = "Radio";
			radio1.Location = new Point(10, 40);
			radio1.Size = new Size(80, 13);
			radio1.Text = "<m>R</m>ouge";
			radio1.ActiveState = WidgetState.ActiveYes;
			radio1.Group = "RGB";
			radio1.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			radio1.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio1);

			RadioButton radio2 = new RadioButton();
			radio2.Name = "Radio";
			radio2.Location = new Point(10, 25);
			radio2.Size = new Size(80, 13);
			radio2.Text = "<m>V</m>ert";
			radio2.Group = "RGB";
			radio2.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			radio2.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio2);

			RadioButton radio3 = new RadioButton();
			radio3.Name = "Radio";
			radio3.Location = new Point(10, 10);
			radio3.Size = new Size(80, 13);
			radio3.Text = "<m>B</m>leu";
			radio3.Group = "RGB";
			radio3.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			radio3.Clicked += new MessageEventHandler(this.HandleRadio);
			box.Children.Add(radio3);

			Scroller scrollv = new Scroller();
			scrollv.Name = "Scroller";
			scrollv.Location = new Point(120, 50);
			scrollv.Size = new Size(15, 120);
			scrollv.Range = 10;
			scrollv.Display = 3;
			scrollv.Position = 1;
			scrollv.ButtonStep = 1;
			scrollv.PageStep = 2;
			scrollv.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(scrollv);

			Scroller scrollh = new Scroller();
			scrollh.Name = "Scroller";
			scrollh.Location = new Point(140, 50);
			scrollh.Size = new Size(120, 15);
			scrollh.Range = 10;
			scrollh.Display = 7;
			scrollh.Position = 1;
			scrollh.ButtonStep = 1;
			scrollh.PageStep = 2;
			scrollh.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(scrollh);

#if true
			TextField combo = new TextField(TextFieldType.Combo);
			combo.Name = "Combo";
			combo.Location = new Point(160, 180);
			combo.Size = new Size(100, 20);
			combo.Text = "Janvier";
			combo.Cursor = combo.Text.Length;
			combo.ComboAddText("Janvier");
			combo.ComboAddText("Fevrier");
			combo.ComboAddText("Mars");
			combo.ComboAddText("Avril");
			combo.ComboAddText("Mai");
			combo.ComboAddText("Juin");
			combo.ComboAddText("Juillet");
			combo.ComboAddText("Aout");
			combo.ComboAddText("Septembre");
			combo.ComboAddText("Octobre");
			combo.ComboAddText("Novembre");
			combo.ComboAddText("Decembre");
			combo.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(combo);
#endif

			TextField text = new TextField(TextFieldType.SingleLine);
			text.Name = "TextField";
			text.Location = new Point(160, 150);
			text.Size = new Size(100, 20);
			text.Text = "Bonjour";
			text.Cursor = text.Text.Length;
			text.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(text);

			TextField tud = new TextField(TextFieldType.UpDown);
			tud.Name = "TextField Up/Down";
			tud.Location = new Point(160, 125);
			tud.Size = new Size(50, 20);
			tud.Value = 50;
			tud.MinRange = -100;
			tud.MaxRange = 100;
			tud.Step = 10;
			tud.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(tud);

			TextField multi = new TextField(TextFieldType.MultiLine);
			multi.Name = "Multi";
			multi.Location = new Point(160, 70);
			multi.Size = new Size(100, 50);
			multi.Text = "Ceci est une petite phrase ridicule.<br/>Mais elle est assez longue pour faire des essais.";
			multi.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(multi);

			ScrollList sl = new ScrollList();
			sl.Name = "ScrollList";
			sl.Location = new Point(270, 70);
			sl.Size = new Size(90, 100);
			sl.AdjustToMultiple(ScrollListAdjust.MoveDown);
			sl.AddText("Janvier");
			sl.AddText("Fevrier");
			sl.AddText("Mars <i>(A)</i>");
			sl.AddText("Avril");
			sl.AddText("Mai");
			sl.AddText("Juin");
#if true
			sl.AddText("Juillet <b>(B)</b>");
			sl.AddText("Aout");
			sl.AddText("Septembre");
			sl.AddText("Octobre");
			sl.AddText("Novembre");
			sl.AddText("Decembre");
#endif
			sl.Select = 5;  // sélectionne juin
			if ( !sl.IsShowSelect() )  sl.ShowSelect(ScrollListShow.Middle);
			sl.Anchor = AnchorStyles.Bottom|AnchorStyles.Left;
			window.Root.Children.Add(sl);

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


		[Test] public void CheckAdornerBigText()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerBigText";

			TextField multi = new TextField(TextFieldType.MultiLine);
			multi.Name = "Multi";
			multi.Location = new Point(10, 10);
			multi.Size = new Size(380, 280);
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

			window.FocusedWidget = multi;

			window.Show();
		}

		[Test] public void CheckAdornerTab()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerTab";

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
			this.tabBook = tb;

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

			TextField multi = new TextField(TextFieldType.MultiLine);
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

			Scroller scrollv = new Scroller();
			scrollv.Name = "Scroller";
			scrollv.Location = new Point(10, 10);
			scrollv.Size = new Size(15, inside.Height-20);
			scrollv.Range = 10;
			scrollv.Display = 3;
			scrollv.Position = 1;
			scrollv.ButtonStep = 1;
			scrollv.PageStep = 2;
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

			CreateRadioLook(page4.Children, new Point(10, 115));

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

			CreateRadioLook(window.Root.Children, new Point(10, 245));

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
#if true
					StaticText text = new StaticText();
					if ( x != 0 || y != 0 ) text.Text = string.Format("{0}.{1}", y+1, x+1);
					text.Alignment = ContentAlignment.MiddleCenter;
					text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					
					if ( x == 2 && y == 2 )
					{
						CheckButton widget = new CheckButton ();
						widget.Text = "surprise";
						widget.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
						table[x,y].Insert (widget);
					}
					else if ( x != 1 || y != 1 )
					{
						table[x,y].Insert (text);
					}
#endif
#if false
					Cell cell = new Cell();
					TextField text = new TextField(TextFieldType.SingleLine);
					text.TextFieldStyle = TextFieldStyle.Flat;
					string s = "";
					s += y+1;
					s += ".";
					s += x+1;
					text.Text = s;
					text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					cell.Children.Add(text);
					table[x,y] = cell;
#endif
#if false
					Cell cell = new Cell();
					Button button = new Button();
					string s = "";
					s += y+1;
					s += ".";
					s += x+1;
					button.Text = s;
					button.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					cell.Children.Add(button);
					table[x,y] = cell;
#endif
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

			CreateRadioLook(window.Root.Children, new Point(10, 245));

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
					TextField text = new TextField(TextFieldType.SingleLine);
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

			CreateRadioLook(window.Root.Children, new Point(10, 245));

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
					StaticText text = new StaticText();
					text.Text = string.Format("L{0} C{1}", x+1, y+1);
					text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					table[x,y].Insert (text);
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

			CreateRadioLook(window.Root.Children, new Point(10, 245));

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
			//table.AdjustToMultiple(Widgets.ScrollArrayAdjust.MoveDown);
			//table.AdjustToContent(Widgets.ScrollArrayAdjust.MoveDown, 10, 1000);
			table.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(table);

			window.FocusedWidget = table;

			window.Show();
		}


		
		// Crée les 3 boutons radio pour changer de look.
		protected void CreateRadioLook(Widget.WidgetCollection collection, Point origine)
		{
			RadioButton look1 = new RadioButton();
			look1.Name = "Default";
			look1.Location = new Point(origine.X, origine.Y+30);
			look1.Size = new Size(100, 13);
			look1.Text = "Look <m>s</m>tandard";
			look1.ActiveState = WidgetState.ActiveYes;
			look1.Group = "Look";
			look1.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			look1.ActiveStateChanged += new EventHandler(this.HandleLook);
			collection.Add(look1);

			RadioButton look2 = new RadioButton();
			look2.Name = "LookXP";
			look2.Location = new Point(origine.X, origine.Y+15);
			look2.Size = new Size(100, 13);
			look2.Text = "Look <m>X</m>P";
			look2.Group = "Look";
			look2.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			look2.ActiveStateChanged += new EventHandler(this.HandleLook);
			collection.Add(look2);

			RadioButton look3 = new RadioButton();
			look3.Name = "LookDany";
			look3.Location = new Point(origine.X, origine.Y+0);
			look3.Size = new Size(100, 13);
			look3.Text = "Look <m>D</m>any";
			look3.Group = "Look";
			look3.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			look3.ActiveStateChanged += new EventHandler(this.HandleLook);
			collection.Add(look3);
		}

		private void HandleLook(object sender)
		{
			RadioButton button = sender as RadioButton;
			if (button.ActiveState == WidgetState.ActiveYes)
			{
				Widgets.Adorner.Factory.SetActive(button.Name);
				button.RootParent.Invalidate();  // redessine toute la fenêtre
			}
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

			TextField text = new TextField(TextFieldType.SingleLine);
			text.Name = "TextField";
			text.Location = new Point(160, 150);
			text.Size = new Size(100, 20);
			text.Text = "Bonjour";
			text.Cursor = text.Text.Length;
			text.Alignment = ContentAlignment.MiddleRight;
			text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			window.Root.Children.Add(text);

			window.Show();
		}

		[Test] public void CheckAdornerTestParents()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(400, 300);
			window.Text = "CheckAdornerTestParents";

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

		[Test] public void CheckAdornerBase()
		{
			WindowFrame window = new WindowFrame();
			
			window.ClientSize = new System.Drawing.Size(300, 200);
			window.Text = "CheckAdornerBase";
			window.Root.PaintForeground += new PaintEventHandler(CheckPaint_Paint1);

			window.Show();
		}

		private void CheckPaint_Paint1(object sender, PaintEventArgs e)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Widgets.Adorner.Factory.SetActive("Default");

			Rectangle rect = new Rectangle(10, 10, 30, 30);
			WidgetState state = WidgetState.Enabled;
			Direction shadow = Direction.Up;
			Direction dir = Direction.Up;
			adorner.PaintArrow(e.Graphics, rect, state, shadow, dir);

			rect.Offset(70, 0);
			adorner.PaintButtonBackground(e.Graphics, rect, state, shadow, ButtonStyle.Normal);

			rect.Offset(70, 0);
			adorner.PaintButtonBackground(e.Graphics, rect, state, shadow, ButtonStyle.Flat);

			rect.Offset(-70, 40);
			rect.Offset(0.5, 0.5);
			adorner.PaintButtonBackground(e.Graphics, rect, state, shadow, ButtonStyle.Normal);
		}


		protected TabBook	tabBook;
	}
}
