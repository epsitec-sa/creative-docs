using NUnit.Framework;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;
using Epsitec.Common.Widgets.Helpers;
using Epsitec.Common.Widgets.Adorners;

namespace Epsitec.Common.Tests.Widgets
{
	[TestFixture]
	public class AdornerTest
	{
		[SetUp] public void Initialize()
		{
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
			
			DynamicImage image = new DynamicImage (new Size (30, 18), new DynamicImagePaintCallback (this.DynamicImageXyz));
			
			image.IsCacheEnabled = false;
			
			Epsitec.Common.Support.ImageProvider.Default.AddDynamicImage ("Xyz", image);
		}

		private bool DynamicImageXyz(Graphics graphics, Size size, string argument, GlyphPaintStyle style, Color color, object adorner)
		{
			//	Méthode de test pour peindre une image dynamique selon un
			//	modèle nommé "Xyz"; l'argument reçu en entrée permet de
			//	déterminer exactement ce qui doit être peint.

			switch (style)
			{
				case GlyphPaintStyle.Normal:
				case GlyphPaintStyle.Disabled:
					break;
				default:
					return false;
			}

			if (graphics != null)
			{
				int hue;
				double saturation = (style == GlyphPaintStyle.Disabled) ? 0.2 : 1.0;
				double value      = (style == GlyphPaintStyle.Disabled) ? 0.7 : 1.0;

				if (argument == "random")
				{
					System.Random random = new System.Random ();
					hue = random.Next (360);
				}
				else
				{
					hue = int.Parse (argument);
				}

				graphics.AddFilledRectangle (0, 0, size.Width, size.Height);
				graphics.RenderSolid (Color.FromHsv (hue, saturation, value));
				graphics.LineWidth = 2.0;
				graphics.AddRectangle (1, 1, size.Width-2, size.Height-2);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
			
			return true;
		}

		[Test] public void AutomatedTestEnvironment()
		{
			Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
		}
		
		[Test] public void CheckAdornerWidgets()
		{
			Window.RunInTestEnvironment (AdornerTest.CreateAdornerWidgets ());
		}
		
		[Test] public void CheckAdornerWidgetsDisabled()
		{
			Window window = AdornerTest.CreateAdornerWidgets ();
			this.RecursiveDisable(window.Root, true);
			Window.RunInTestEnvironment (window);
		}
		
		[Test] public void CheckAdornerBigText()
		{
			Window.RunInTestEnvironment (this.CreateBigText());
		}
		
		
		void RecursiveDisable(Widget widget, bool top_level)
		{
			if (widget.IsEnabled)
			{
				widget.Enable = (top_level);

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
		
		
		public static Window CreateAdornerWidgets()
		{
			Document.Engine.Initialize ();

			Window window = new Window();
			
			window.ClientSize = new Size(700, 340);
			window.Text = "CheckAdornerWidgets";
			window.Name = "CheckAdornerWidgets";

			ToolTip tip = ToolTip.Default;
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
			fileMenu.Items[4].Enable = false;

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
			showMenu.Items[1].Enable = false;

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
			t1.PreferredWidth = 70;
			t1.Text = "Rouge";
			t1.Items.Add("red",   "Rouge");
			t1.Items.Add("green", "Vert");
			t1.Items.Add("blue",  "Bleu");
			t1.ButtonShowCondition = ButtonShowCondition.Always;
			t1.ComboArrowMode = ComboArrowMode.Cycle;
			t1.IsLiveUpdateEnabled = false;
			t1.TabIndex = 1;
			t1.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			TextFieldCombo t2 = new TextFieldCombo();
			t2.PreferredWidth = 70;
			t2.Text = "Lecture";
			t2.Items.Add("readonly",  "Lecture");
			t2.Items.Add("writeonly", "Ecriture");
			t2.Items.Add("readwrite", "Lecture &amp; Ecriture");
			t2.ButtonShowCondition = ButtonShowCondition.Always;
			t2.ComboArrowMode = ComboArrowMode.Cycle;
			t2.IsReadOnly = true;
			t2.TabIndex = 2;
			t2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			
			tb.Items.Add(t1);
			tb.Items.Add(t2);
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
			a.PreferredWidth = 75;
			a.Text = "OK";
			a.ButtonStyle = ButtonStyle.DefaultAccept;
			a.Anchor = AnchorStyles.BottomLeft;
			a.Margins = new Margins(10, 0, 0, 30);
			a.TabIndex = 20;
			a.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			Assert.IsFalse (a.IsVisible);
			Assert.IsTrue (a.Visibility);
			
			window.Root.Children.Add(a);

			Assert.IsFalse (window.Root.IsVisible);
			Assert.IsFalse (a.IsVisible);
			
			tip.SetToolTip(a, "C'est d'accord, tout baigne");

			Button b = new Button();
//			b.Location = new Point(95, 30);
			b.PreferredWidth = 75;
			b.Text = "<m>A</m>nnuler";
			b.Anchor = AnchorStyles.BottomLeft;
			b.Margins = new Margins(95, 0, 0, 30);
			b.TabIndex = 21;
			b.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(b);
			tip.SetToolTip(b, "Annule tout<br/>Deuxieme ligne, juste pour voir !");

			Button c = new Button();
//			c.Location = new Point(95+150, 30);
			c.PreferredWidth = 75;
			c.Text = "Ai<m>d</m>e";
//			c.Enable = false;
			c.Anchor = AnchorStyles.BottomLeft;
			c.Margins = new Margins(245, 0, 0, 30);
			c.TabIndex = 22;
			c.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(c);
			
			Widget      tip_widget = new ToolTip.Contents ();
			CheckButton tip_check1 = new CheckButton (tip_widget);
			CheckButton tip_check2 = new CheckButton (tip_widget);
			
			tip_check1.Text    = "premier secours";
			tip_check1.Dock    = DockStyle.Top;
			tip_check1.Margins = new Margins (4, 4, 4, 0);
			
			tip_check2.Text    = "aide détaillée";
			tip_check2.Dock    = DockStyle.Top;
			tip_check2.Margins = new Margins (4, 4, 1, 0);
			
			tip_widget.BackColor     = Color.FromRgb (1.0, 0.5, 0.5);
			tip_widget.PreferredSize = new Size(120, 4+tip_check1.PreferredHeight+1+tip_check2.PreferredHeight+4);
			
			tip.SetToolTip(c, tip_widget);

			StaticText st = new StaticText();
//			st.Location = new Point(10, 265);
			st.PreferredWidth = 150;
			st.Text = @"Choix du <b>look</b> de l'<i>interface</i> :";
			st.Anchor = AnchorStyles.TopLeft;
			st.Margins = new Margins(10, 0, 340 - st.PreferredHeight - 265, 0);
			window.Root.Children.Add(st);

			AdornerTest.CreateListLook(window.Root, 10, 80, tip, 1);
			
			Tag tag1 = new Tag("ExecuteTag", "TestTag");
			tag1.SetManualBounds(new Rectangle(115, 246, 18, 18));
			tag1.SetParent (window.Root);
			tip.SetToolTip(tag1, "Je suis un <i>smart tag</i> maison.");

			Tag tag2 = new Tag("ExecuteTag", "TestTag");
			tag2.SetManualBounds(new Rectangle(115, 226, 18, 18));
			tag2.SetParent (window.Root);
			tag2.Color = Color.FromRgb(1,0,0);
			tip.SetToolTip(tag2, "Je suis un <i>smart tag</i> maison rouge.");

			Tag tag3 = new Tag("ExecuteTag", "TestTag");
			tag3.SetManualBounds(new Rectangle(115, 206, 18, 18));
			tag3.SetParent (window.Root);
			tag3.Color = Color.FromRgb(0,1,0);
			tip.SetToolTip(tag3, "Je suis un <i>smart tag</i> maison vert.");

			Tag tag4 = new Tag("ExecuteTag", "TestTag");
			tag4.SetManualBounds(new Rectangle(140, 246, 12, 12));
			tag4.SetParent (window.Root);
			tip.SetToolTip(tag4, "Je suis un petit <i>smart tag</i> maison.");

			Tag tag5 = new Tag("ExecuteTag", "TestTag");
			tag5.SetManualBounds(new Rectangle(140, 226, 12, 12));
			tag5.SetParent (window.Root);
			tag5.Color = Color.FromRgb(0,0,1);
			tip.SetToolTip(tag5, "Je suis un petit <i>smart tag</i> maison bleu.");

			StaticText link = new StaticText();
			link.PreferredWidth = 120;
			link.Text = @"Visitez notre <a href=""http://www.epsitec.ch"">site web</a> !";
			link.Anchor = AnchorStyles.BottomRight;
			link.Margins = new Margins(0, 600-360-120+100, 0, 36);
			link.HypertextClicked += AdornerTest.link_HypertextClicked;
			window.Root.Children.Add(link);
			
			SpecialWidget spec = new SpecialWidget();
			spec.SetManualBounds(new Rectangle(540, 30, 40, spec.PreferredHeight));
			window.Root.Children.Add(spec);
			tip.SetToolTip(spec, "*");
			
			StaticImage image1 = new StaticImage ();
			StaticImage image2 = new StaticImage ();
			StaticImage image3 = new StaticImage ();
			StaticImage image4 = new StaticImage ();
			StaticImage image5 = new StaticImage ();
			StaticImage image6 = new StaticImage ();
			
			image1.SetManualBounds(new Rectangle (590, 15, 20, 20));
			image1.ImageName = @"file:images/cut.png";
			image1.ContentAlignment = ContentAlignment.BottomCenter;
			image1.VerticalOffset = 0;

			image2.SetManualBounds(new Rectangle (600, 15, 20, 20));
			image2.ImageName = @"file:images/cut.png";
			image2.VerticalOffset = 4;
			image2.ContentAlignment = ContentAlignment.BottomCenter;

			image3.SetManualBounds(new Rectangle (610, 15, 20, 20));
			image3.ImageName = @"file:images/cut.png";
			image3.VerticalOffset = 8;
			image3.ContentAlignment = ContentAlignment.BottomCenter;
			
//-			Widget.ObsoleteBaseLineAlign (image1, image2);
//-			Widget.ObsoleteBaseLineAlign (image1, image3);

//-			Assert.AreEqual((int) ((image1.ActualLocation.Y+image2.VerticalOffset)*100+0.5), (int) (image2.ActualLocation.Y*100+0.5));
//-			Assert.AreEqual((int) ((image1.ActualLocation.Y+image3.VerticalOffset)*100+0.5), (int) (image3.ActualLocation.Y*100+0.5));

			image4.SetManualBounds(new Rectangle (630, 15, 40, 20));
			image4.ImageName = @"dyn:Xyz/random";
			image4.ContentAlignment = ContentAlignment.BottomCenter;

			image5.SetManualBounds(new Rectangle (630, 35, 40, 20));
			image5.ImageName = @"dyn:Xyz/80";
			image5.ContentAlignment = ContentAlignment.BottomCenter;

			image6.SetManualBounds(new Rectangle (630, 55, 40, 20));
			image6.ImageName = @"dyn:Xyz/60";
			image6.ImageSize = new Size (20, 12);
			image6.ContentAlignment = ContentAlignment.BottomCenter;
			
			window.Root.Children.Add(image1);
			window.Root.Children.Add(image2);
			window.Root.Children.Add(image3);
			window.Root.Children.Add(image4);
			window.Root.Children.Add(image5);
			window.Root.Children.Add(image6);

			GroupBox box = new GroupBox();
			box.PreferredSize = new Size(100, 75);
			box.Text = "Couleur";
			box.Anchor = AnchorStyles.BottomLeft;
			box.Margins = new Margins(10, 0, 0, 100);
			box.TabIndex = 2;
			window.Root.Children.Add(box);

			RadioButton radio1 = new RadioButton();
			radio1.SetManualBounds(new Rectangle(10, 40, 80, radio1.PreferredHeight));
			radio1.Text = "<font color=\"#ff0000\"><m>R</m>ouge</font>";
			radio1.Group = "RGB";
			radio1.TabIndex = 1;
			radio1.Index = 1;
			radio1.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio1.Clicked += AdornerTest.HandleRadio;
			box.Children.Add(radio1);
			tip.SetToolTip(radio1, "Couleur rouge");

			RadioButton radio2 = new RadioButton();
			radio2.SetManualBounds(new Rectangle(10, 25, 80, radio2.PreferredHeight));
			radio2.Text = "<font color=\"#00ff00\"><m>V</m>ert</font>";
			radio2.Group = "RGB";
			radio2.TabIndex = 1;
			radio2.Index = 2;
			radio2.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio2.Clicked += AdornerTest.HandleRadio;
			box.Children.Add(radio2);
			tip.SetToolTip(radio2, "Couleur verte");

			RadioButton radio3 = new RadioButton();
			radio3.SetManualBounds(new Rectangle(10, 10, 80, radio3.PreferredHeight));
			radio3.Text = "<font color=\"#0000ff\"><m>B</m>leu</font>";
			radio3.Group = "RGB";
			radio3.TabIndex = 1;
			radio3.Index = 3;
			radio3.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio3.Clicked += AdornerTest.HandleRadio;
			box.Children.Add(radio3);
			tip.SetToolTip(radio3, "Couleur bleue");
			
			radio1.ActiveState = ActiveState.Yes;

			CheckButton check = new CheckButton();
			check.PreferredWidth = 100;
			check.Text = "<m>C</m>ochez ici";
			check.ActiveState = ActiveState.Yes;
			check.Anchor = AnchorStyles.BottomLeft;
			check.Margins = new Margins(10, 0, 0, 70);
			check.TabIndex = 3;
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.Clicked += AdornerTest.HandleCheck;
			window.Root.Children.Add(check);
			tip.SetToolTip(check, "Juste pour voir");

			VScroller scrollv = new VScroller();
			scrollv.PreferredSize = new Size(17, 120);
			scrollv.MaxValue = 10;
			scrollv.VisibleRangeRatio = 0.3M;
			scrollv.Value = 1;
			scrollv.SmallChange = 1;
			scrollv.LargeChange = 2;
			scrollv.Anchor = AnchorStyles.Left | AnchorStyles.TopAndBottom;
			scrollv.Margins = new Margins(120, 0, 340 - 120 - 70, 70);
			window.Root.Children.Add(scrollv);
			tip.SetToolTip(scrollv, "Ascenseur vertical");

			HScroller scrollh = new HScroller();
			scrollh.PreferredSize = new Size(120, 17);
			scrollh.MaxValue = 10;
			scrollh.VisibleRangeRatio = 0.7M;
			scrollh.Value = 1;
			scrollh.SmallChange = 1;
			scrollh.LargeChange = 2;
			scrollh.Anchor = AnchorStyles.BottomLeft;
			scrollh.Margins = new Margins(140, 0, 0, 70);
			window.Root.Children.Add(scrollh);
			tip.SetToolTip(scrollh, "Ascenseur horizontal");

			VSlider slidev = new VSlider();
			slidev.PreferredSize = new Size(16, 100);
			slidev.MaxValue = 10;
			slidev.Value = 4;
			slidev.SmallChange = 1;
			slidev.LargeChange = 2;
			slidev.Anchor = AnchorStyles.Right | AnchorStyles.TopAndBottom;
			slidev.Margins = new Margins(0, 50, 100, 140);
			slidev.ShowMinMaxButtons = true;
			window.Root.Children.Add (slidev);
			tip.SetToolTip(slidev, "Slider vertical");

			HSlider slideh = new HSlider();
			slideh.PreferredSize = new Size(100, 16);
			slideh.MaxValue = 10;
			slideh.Value = 4;
			slideh.SmallChange = 1;
			slideh.LargeChange = 2;
			slideh.Anchor = AnchorStyles.BottomRight;
			slideh.Margins = new Margins(0, 10, 0, 100);
			slideh.ShowMinMaxButtons = true;
			window.Root.Children.Add (slideh);
			tip.SetToolTip(slideh, "Slider horizontal");

			TextField secret = new TextField ();
			secret.PreferredWidth = 100;
			secret.Text = "Password";
			secret.IsPassword = true;
			secret.Cursor = secret.Text.Length;
			secret.Anchor = AnchorStyles.BottomLeft;
			secret.Margins = new Margins (160, 0, 0, 250);
			window.Root.Children.Add (secret);

			TextFieldExList combo = new TextFieldExList ();
			combo.PlaceHolder = "<b>&lt;autre&gt;</b>";
			combo.PreferredWidth = 100;
			combo.Text = "Janvier";
			combo.Cursor = combo.Text.Length;
			combo.Items.Add("Janvier");
			combo.Items.Add("Fevrier");
			combo.Items.Add("Mars");
			combo.Items.Add("Avril");
			combo.Items.Add("Mai");
			combo.Items.Add("Juin");
			combo.Items.Add("Juillet");
			combo.Items.Add("Août");
			combo.Items.Add("Septembre");
			combo.Items.Add("Octobre");
			combo.Items.Add("Novembre");
			combo.Items.Add("Décembre");
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
			combo.Margins = new Margins(160, 0, 0, 220);
			combo.TabIndex = 10;
			combo.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			combo.ButtonShowCondition = ButtonShowCondition.Always;
			combo.ComboArrowMode = ComboArrowMode.Open;
			window.Root.Children.Add(combo);

			TextField text = new TextField();
			text.PreferredWidth = 100;
			text.Text = "Bonjour";
			text.Cursor = text.Text.Length;
			text.Anchor = AnchorStyles.BottomLeft;
			text.Margins = new Margins(160, 0, 0, 190);
			text.TabIndex = 11;
			text.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			text.HintText = "Bonjour !";
			text.TextDisplayMode = TextFieldDisplayMode.ActiveHint;
			window.Root.Children.Add(text);

			TextFieldUpDown tud = new TextFieldUpDown();
			tud.PreferredWidth = 52;
			tud.TextSuffix = "%";
			
			tud.Value        =   5.00M;
			tud.DefaultValue =   0.00M;
			tud.MinValue     =  -2.50M;
			tud.MaxValue     = 100.00M;
			tud.Step         =   2.50M;
			tud.Resolution   =   0.25M;
			
			tud.ClearText();
			
			tud.Anchor = AnchorStyles.BottomLeft;
			tud.Margins = new Margins(160, 0, 0, 160);
			tud.TabIndex = 12;
			tud.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(tud);

			TextFieldSlider slider = new TextFieldSlider();
			slider.PreferredWidth = 45;
			slider.Value = 50;
			slider.MinValue = -100;
			slider.MaxValue = 100;
			slider.Step = 10;
			slider.Resolution = 5;
			slider.Anchor = AnchorStyles.BottomLeft;
			slider.Margins = new Margins(215, 0, 0, 160);
			slider.TabIndex = 13;
			slider.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(slider);

			TextFieldMulti multi = new TextFieldMulti();
			multi.PreferredSize = new Size(100, 50);
			multi.Text = "Ceci est une petite phrase ridicule.<br/>Mais elle est assez longue pour faire des essais.";
			//?multi.TextLayout.JustifMode = TextJustifMode.AllButLast;
			multi.Anchor = AnchorStyles.BottomLeft;
			multi.Margins = new Margins(160, 0, 0, 100);
			multi.TabIndex = 14;
			multi.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			window.Root.Children.Add(multi);

			TabBook tab = new TabBook();
			tab.Arrows = TabBookArrows.Right;
			tab.Anchor = AnchorStyles.All;
			tab.Margins = new Margins(280, 600-280-300+100, 340-180-70, 70);
			tab.HasMenuButton = true;
			tab.HasCloseButton = true;
			tab.TabIndex = 15;
			tab.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren;
			window.Root.Children.Add(tab);

			window.ForceLayout ();

			Rectangle inside = tab.Client.Bounds;
			inside.Deflate (tab.GetInternalPadding ());

			//	Crée l'onglet 1.
			TabPage page1 = new TabPage();
//			page1.SetManualBounds(inside);
			page1.TabTitle = "<m>P</m>remier";
			page1.TabIndex = 1;
			page1.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			tab.Items.Add(page1);

			ScrollList sl = new ScrollList();
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
			sl.MaxSize = new Size (90, 100);
			sl.PreferredSize = sl.GetBestFitSize ();
			sl.SelectedItemIndex = 5;  // sélectionne juin
			sl.ShowSelected(ScrollShowMode.Center);
			sl.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			sl.Margins = new Margins(10, 0, 10, 10);
			sl.TabIndex = 1;
			sl.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			page1.Children.Add(sl);
			tip.SetToolTip(sl, "Choix du mois");

			StaticText st2 = new StaticText();
			st2.PreferredWidth = 90;
			st2.Text = "Non éditable :";
			st2.Anchor = AnchorStyles.TopLeft;
			st2.Margins = new Margins(160, 0, 30, 0);
			page1.Children.Add(st2);

			TextField textfix = new TextField();
			textfix.PreferredWidth = 100;
			textfix.Text = "Texte fixe";
			textfix.IsReadOnly = true;
			textfix.Anchor = AnchorStyles.TopLeft;
			textfix.Margins = new Margins(160, 0, 50, 0);
			page1.Children.Add(textfix);

			TextFieldCombo combofix = new TextFieldCombo();
			combofix.PreferredWidth = 100;
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
			combofix.Margins = new Margins(160, 0, 80, 0);
			combofix.TabIndex = 2;
			combofix.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			combofix.ButtonShowCondition = ButtonShowCondition.Always;
			page1.Children.Add(combofix);

			//	Crée l'onglet 2.
			TabPage page2 = new TabPage();
//			page2.SetManualBounds(inside);
			page2.TabTitle = "<m>D</m>euxième";
			page2.TabIndex = 2;
			page2.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			tab.Items.Add(page2);

			CellTable table = new CellTable();
			table.StyleH  = CellArrayStyles.ScrollNorm;
			table.StyleH |= CellArrayStyles.Header;
			table.StyleH |= CellArrayStyles.Separator;
			table.StyleH |= CellArrayStyles.Mobile;
			table.StyleH |= CellArrayStyles.Sort;
			table.StyleV  = CellArrayStyles.ScrollNorm;
			table.StyleV |= CellArrayStyles.Separator;
			table.StyleV |= CellArrayStyles.SelectLine;
			table.StyleV |= CellArrayStyles.SelectMulti;
			table.StyleV |= CellArrayStyles.Sort;
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
					tx.ContentAlignment = ContentAlignment.MiddleLeft;
					tx.Dock = DockStyle.Fill;
					table[x,y].Insert(tx);
				}
			}
			table.Anchor = AnchorStyles.All;
			table.Margins = new Margins(10, 10, 10, 10);
			table.TabIndex = 1;
			table.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			page2.Children.Add(table);

			//	Crée l'onglet 3.
			TabPage page3 = new TabPage();
			page3.SetManualBounds(inside);
			page3.TabTitle = "<m>T</m>roisième";
			
			TextFieldEx text_ex_1 = new TextFieldEx();
			text_ex_1.SetParent (page3);
			text_ex_1.SetManualBounds(new Rectangle(10, page3.ActualHeight-30, 120, text_ex_1.PreferredHeight));
			text_ex_1.TabIndex = 1;
			text_ex_1.EditionAccepted += AdornerTest.HandleTextExEditionAccepted;
			text_ex_1.EditionRejected += AdornerTest.HandleTextExEditionRejected;
			
			TextFieldEx text_ex_2 = new TextFieldEx();
			text_ex_2.SetParent (page3);
			text_ex_2.SetManualBounds(new Rectangle(10, page3.ActualHeight-30-28, 120, text_ex_2.PreferredHeight));
			text_ex_2.TabIndex = 2;
			
			TextFieldEx text_ex_3 = new TextFieldEx();
			text_ex_3.SetParent (page3);
			text_ex_3.SetManualBounds(new Rectangle(10, page3.ActualHeight-30-28-28, 120, text_ex_3.PreferredHeight));
			text_ex_3.Enable = false;
			text_ex_3.TabIndex = 3;
			
			TextFieldEx text_ex_4 = new TextFieldEx();
			text_ex_4.SetParent (page3);
			text_ex_4.SetManualBounds(new Rectangle(10, page3.ActualHeight-30-28-28-28, 120, text_ex_4.PreferredHeight));
			text_ex_4.TabIndex = 4;
			text_ex_4.ButtonShowCondition = ButtonShowCondition.WhenModified;
			
			TextFieldEx text_ex_5 = new TextFieldEx();
			text_ex_5.SetParent (page3);
			text_ex_5.SetManualBounds(new Rectangle(10, page3.ActualHeight-30-28-28-28-28, 120, text_ex_5.PreferredHeight));
			text_ex_5.TabIndex = 5;
			text_ex_5.ButtonShowCondition = ButtonShowCondition.WhenFocused;
			text_ex_5.DefocusAction       = DefocusAction.AcceptEdition;
			
			TextFieldEx text_ex_6 = new TextFieldEx();
			text_ex_6.SetParent (page3);
			text_ex_6.SetManualBounds(new Rectangle(10+120+5, page3.ActualHeight-30, 120, text_ex_6.PreferredHeight));
			text_ex_6.TabIndex = 6;
			text_ex_6.ButtonShowCondition = ButtonShowCondition.WhenModified;
			text_ex_6.DefocusAction       = DefocusAction.Modal;
			
			TextFieldEx text_ex_7 = new TextFieldEx();
			text_ex_7.SetParent (page3);
			text_ex_7.SetManualBounds(new Rectangle(10+120+5, page3.ActualHeight-30-28, 120, text_ex_7.PreferredHeight));
			text_ex_7.TabIndex = 7;
			text_ex_7.ButtonShowCondition = ButtonShowCondition.Never;
			text_ex_7.DefocusAction       = DefocusAction.AcceptEdition;
			
			TextFieldEx text_ex_8 = new TextFieldEx();
			text_ex_8.SetParent (page3);
			text_ex_8.SetManualBounds(new Rectangle(10+120+5, page3.ActualHeight-30-28-28, 120, text_ex_8.PreferredHeight));
			text_ex_8.TabIndex = 8;
			text_ex_8.ButtonShowCondition = ButtonShowCondition.WhenModified;
			text_ex_8.DefocusAction       = DefocusAction.AutoAcceptOrRejectEdition;
			
			new RegexValidator (text_ex_6, RegexFactory.AlphaName);
			new RegexValidator (text_ex_7, RegexFactory.AlphaName);
			new RegexValidator (text_ex_8, RegexFactory.AlphaName);
			
			page3.TabIndex = 3;
			page3.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			tab.Items.Add(page3);

			//	Crée l'onglet 4.
			TabPage page4 = new TabPage();
			page4.SetManualBounds(inside);
			page4.TabTitle = "<m>Q</m>uatrième";
			page4.TabIndex = 4;
			page4.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			tab.Items.Add(page4);
			
			TextFieldUpDown text_up_down = new TextFieldUpDown (page4);
			text_up_down.MinValue   = -1000M;
			text_up_down.MaxValue   =  1000000000000000M;
			text_up_down.Resolution = 0.0000000000000001M;
			text_up_down.SetManualBounds(new Rectangle(10, 10, 200, text_up_down.PreferredHeight));
			text_up_down.TextChanged += AdornerTest.HandleTextUpDownTextChanged;
			

			//	Crée l'onglet 5.
			TabPage page5 = new TabPage();
			page5.SetManualBounds(inside);
			page5.TabTitle = "<m>C</m>inquième";
			page5.TabIndex = 5;
			page5.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			tab.Items.Add(page5);

			TextField text_1 = new TextField();
			text_1.SetParent(page5);
			text_1.SetManualBounds(new Rectangle(10, page5.ActualHeight-30, 120, text_1.PreferredHeight-4));
			text_1.TabIndex = 1;

			TextField text_2 = new TextField();
			text_2.SetParent(page5);
			text_2.SetManualBounds(new Rectangle(10, page5.ActualHeight-30-28, 120, text_2.PreferredHeight-2));
			text_2.TabIndex = 2;

			TextField text_3 = new TextField();
			text_3.SetParent(page5);
			text_3.SetManualBounds(new Rectangle(10, page5.ActualHeight-30-28-28, 120, text_3.PreferredHeight));
			text_3.TabIndex = 3;

			TextField text_4 = new TextField();
			text_4.SetParent(page5);
			text_4.SetManualBounds(new Rectangle(10, page5.ActualHeight-30-28-28-28, 120, text_4.PreferredHeight+2));
			text_4.TabIndex = 4;

			//	Crée l'onglet 6.
			TabPage page6 = new TabPage();
			page6.SetManualBounds(inside);
			page6.TabTitle = "<m>S</m>ixième";
			page6.TabIndex = 6;
			page6.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			tab.Items.Add(page6);

			tab.ActivePage = page2;
			window.FocusWidget (a);

			Assert.IsFalse (window.IsVisible);
			Assert.IsFalse (window.Root.IsVisible);
			Assert.IsFalse (a.IsVisible);
			
			window.Show ();
			
			Assert.IsTrue (window.IsVisible);
			Assert.IsTrue (window.Root.IsVisible);
			Assert.IsTrue (a.IsVisible);
			
			return window;
		}
		
		private static void HandleTextUpDownTextChanged(object sender)
		{
			TextFieldUpDown up_down = sender as TextFieldUpDown;
			System.Diagnostics.Debug.WriteLine (string.Format ("'{0}', value={1}, valid={2}", up_down.Text, up_down.Value, up_down.IsValid));
		}
		
		private static void HandleCheck(object sender, MessageEventArgs e)
		{
			CheckButton button = sender as CheckButton;
//			button.Toggle();
		}

		private static void HandleRadio(object sender, MessageEventArgs e)
		{
			RadioButton button = sender as RadioButton;
//			button.Toggle();
		}

		private static void link_HypertextClicked(object sender, MessageEventArgs e)
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
			this.stats = new StaticText();
			
			multi.Name = "Multi";
//			multi.SetManualBounds(new Rectangle(10, 30, 380, 260));
			multi.MaxLength = 10000;
			
			string s = "";
#if true
			s += "<br/>";
			s += "<b>FIN</b><br/><br/>aaaaaaaaaaaaaaaaaaaaaaaVoici une image <img src=\"file:images/icon.png\"/> intégrée dans le texte.";
#else
			s += "On donnait ce jour-là un grand dîner, où, pour la première fois, je vis avec beaucoup d'étonnement le maître d'hôtel servir l'épée au côté et le chapeau sur la tête. Par hasard on vint à parler de la devise de la maison de Solar, qui était sur la tapisserie avec les armoiries: <i>Tel fiert qui ne tue pas</i>. Comme les Piémontais ne sont pas pour l'ordinaire consommés par la langue française, quelqu'un trouva dans cette devise une faute d'orthographe, et dit qu'au mot <i>fiert</i> il ne fallait point de <i>t</i>.<br/>";
			s += "Le vieux comte de Gouvon allait répondre; mais ayant jeté les yeux sur moi, il vit que je souriait sans oser rien dire: il m'ordonna de parler. Alors je dis que je ne croyait pas que le <i>t</i> fût de trop, que <i>fiert</i> était un vieux mots français qui ne venait pas du nom <i>ferus</i>, fier, menaçant, mais du verbe <i>ferit</i>, il frappe, il blesse; qu'ainsi la devise ne me paraissait pas dire: Tel menace, mais <i>tel frappe qui ne tue pas</i>.<br/>";
			s += "Tout le monde me regardait et se regardait sans rien dire. On ne vit de la vie un pareil étonnement. Mais ce qui me flatta davantage fut de voir clairement sur le visage de Mlle de Breil un air de satisfaction. Cette personne si dédaigneuse daigna me jeter un second regard qui valait tout au moins le premier; puis, tournant les yeux vers son grand-papa, elle semblait attendre avec une sorte d'impatience la louange qu'il me devait, et qu'il me donna en effet si pleine et entière et d'un air si content, que toute la table s'empressa de faire chorus. Ce moment fut court, mais délicieux à tous égards. Ce fut un de ces moments trop rares qui replacent les choses dans leur ordre naturel, et vengent le mérite avili des outrages de la fortune.<br/>";
			s += "<b>FIN</b><br/><br/>Voici une image <img src=\"file:images/icon.png\"/> intégrée dans le texte.";
#endif
			multi.Text = s;

			multi.ContentAlignment = ContentAlignment.TopLeft;
			multi.TextLayout.JustifMode = TextJustifMode.AllButLast;
			//?multi.ContentAlignment = Drawing.ContentAlignment.TopRight;
			//?multi.TextLayout.JustifMode = TextJustifMode.NoLine;
			multi.TextLayout.ShowLineBreak = true;
			multi.TextLayout.ShowTab = true;
//-			Assert.IsNotNull (multi.OpletQueue);
			multi.ScrollZone = 0.2;
			multi.Anchor = AnchorStyles.All;
			multi.Margins = new Margins(10, 10, 40, 30);
			multi.Margins = new Margins(60, 60, 40, 30);
			multi.SetParent (window.Root);
			multi.SelectionChanged += this.HandleMultiSelectionOrCursorChanged1;
			multi.CursorChanged    += this.HandleMultiSelectionOrCursorChanged1;
			multi.TextChanged      += this.HandleMultiSelectionOrCursorChanged1;
			this.bigText = multi;
			
//			stats.SetManualBounds(new Rectangle(10, 2, 380, 26));
			this.stats.PreferredHeight = 26;
			this.stats.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Bottom;
			this.stats.Margins = new Margins (10, 10, 0, 2);
			this.stats.SetParent (window.Root);

			Button buttonBold = new Button();
			buttonBold.Text = "<b>B</b>";
			buttonBold.PreferredWidth = 30;
			buttonBold.AutoFocus = false;
			buttonBold.Anchor = AnchorStyles.TopLeft;
			buttonBold.Margins = new Margins(10, 0, 10, 0);
			buttonBold.SetParent (window.Root);
			buttonBold.Clicked += this.HandleMultiBold;
			
			Button buttonItalic = new Button();
			buttonItalic.Text = "<i>I</i>";
			buttonItalic.PreferredWidth = 30;
			buttonItalic.AutoFocus = false;
			buttonItalic.Anchor = AnchorStyles.TopLeft;
			buttonItalic.Margins = new Margins(40, 0, 10, 0);
			buttonItalic.SetParent (window.Root);
			buttonItalic.Clicked += this.HandleMultiItalic;
			
			Button buttonUnderline = new Button();
			buttonUnderline.Text = "<u>U</u>";
			buttonUnderline.PreferredWidth = 30;
			buttonUnderline.AutoFocus = false;
			buttonUnderline.Anchor = AnchorStyles.TopLeft;
			buttonUnderline.Margins = new Margins(70, 0, 10, 0);
			buttonUnderline.SetParent (window.Root);
			buttonUnderline.Clicked += this.HandleMultiUnderline;
			
			Button buttonFace1 = new Button();
			buttonFace1.Text = "<font face=\"Tahoma\">A</font>";
			buttonFace1.PreferredWidth = 30;
			buttonFace1.AutoFocus = false;
			buttonFace1.Anchor = AnchorStyles.TopLeft;
			buttonFace1.Margins = new Margins(110, 0, 10, 0);
			buttonFace1.SetParent (window.Root);
			buttonFace1.Clicked += this.HandleMultiFace1;
			
			Button buttonFace2 = new Button();
			buttonFace2.Text = "<font face=\"Courier New\">A</font>";
			buttonFace2.PreferredWidth = 30;
			buttonFace2.AutoFocus = false;
			buttonFace2.Anchor = AnchorStyles.TopLeft;
			buttonFace2.Margins = new Margins(140, 0, 10, 0);
			buttonFace2.SetParent (window.Root);
			buttonFace2.Clicked += this.HandleMultiFace2;
			
			Button buttonFace3 = new Button();
			buttonFace3.Text = "<font face=\"Times New Roman\">A</font>";
			buttonFace3.PreferredWidth = 30;
			buttonFace3.AutoFocus = false;
			buttonFace3.Anchor = AnchorStyles.TopLeft;
			buttonFace3.Margins = new Margins(170, 0, 10, 0);
			buttonFace3.SetParent (window.Root);
			buttonFace3.Clicked += this.HandleMultiFace3;
			
			Button buttonSize1 = new Button();
			buttonSize1.Text = "10";
			buttonSize1.PreferredWidth = 30;
			buttonSize1.AutoFocus = false;
			buttonSize1.Anchor = AnchorStyles.TopLeft;
			buttonSize1.Margins = new Margins(210, 0, 10, 0);
			buttonSize1.SetParent (window.Root);
			buttonSize1.Clicked += this.HandleMultiSize1;
			
			Button buttonSize2 = new Button();
			buttonSize2.Text = "20";
			buttonSize2.PreferredWidth = 30;
			buttonSize2.AutoFocus = false;
			buttonSize2.Anchor = AnchorStyles.TopLeft;
			buttonSize2.Margins = new Margins(240, 0, 10, 0);
			buttonSize2.SetParent (window.Root);
			buttonSize2.Clicked += this.HandleMultiSize2;
			
			Button buttonColor1 = new Button();
			buttonColor1.Text = "<b><font color=\"#000000\">o</font></b>";
			buttonColor1.PreferredWidth = 30;
			buttonColor1.AutoFocus = false;
			buttonColor1.Anchor = AnchorStyles.TopLeft;
			buttonColor1.Margins = new Margins(280, 0, 10, 0);
			buttonColor1.SetParent (window.Root);
			buttonColor1.Clicked += this.HandleMultiColor1;
			
			Button buttonColor2 = new Button();
			buttonColor2.Text = "<b><font color=\"#FF0000\">o</font></b>";
			buttonColor2.PreferredWidth = 30;
			buttonColor2.AutoFocus = false;
			buttonColor2.Anchor = AnchorStyles.TopLeft;
			buttonColor2.Margins = new Margins(310, 0, 10, 0);
			buttonColor2.SetParent (window.Root);
			buttonColor2.Clicked += this.HandleMultiColor2;
			
			Button buttonUndo = new Button();
			buttonUndo.Text = "U";
			buttonUndo.PreferredWidth = 30;
			buttonUndo.AutoFocus = false;
			buttonUndo.Anchor = AnchorStyles.TopLeft;
			buttonUndo.Margins = new Margins(350, 0, 10, 0);
			buttonUndo.SetParent (window.Root);
			buttonUndo.Clicked += this.HandleMultiUndo;
			
			Button buttonRedo = new Button();
			buttonRedo.Text = "R";
			buttonRedo.PreferredWidth = 30;
			buttonRedo.AutoFocus = false;
			buttonRedo.Anchor = AnchorStyles.TopLeft;
			buttonRedo.Margins = new Margins(380, 0, 10, 0);
			buttonRedo.SetParent (window.Root);
			buttonRedo.Clicked += this.HandleMultiRedo;
//			window.Root.DebugActive = true;
			window.FocusWidget (multi);

			Assert.IsFalse (window.IsVisible);
			Assert.IsFalse (window.Root.IsVisible);
			Assert.IsFalse (multi.IsVisible);
			
			window.Show();

			Assert.IsTrue (window.IsVisible);
			Assert.IsTrue (window.Root.IsVisible);
			Assert.IsTrue (multi.IsVisible);

//			multi.Text = @"abc <b>def</b> ghi.<br/>123 <i>456</i> 789 <b>qrs</b>.<br/>A<img src=""file:images/icon.png""/>B<br/>";
			
			return window;
		}
		
		private void HandleMultiSelectionOrCursorChanged1(object sender)
		{
			AbstractTextField text  = sender as AbstractTextField;

			this.stats.Text = string.Format ("from={0},  to={1},  after={2}", text.CursorFrom, text.CursorTo, text.CursorAfter);
		}

		private void HandleMultiBold(object sender, MessageEventArgs e)
		{
			this.bigText.TextNavigator.SelectionBold = !this.bigText.TextNavigator.SelectionBold;
		}

		private void HandleMultiItalic(object sender, MessageEventArgs e)
		{
			this.bigText.TextNavigator.SelectionItalic = !this.bigText.TextNavigator.SelectionItalic;
		}

		private void HandleMultiUnderline(object sender, MessageEventArgs e)
		{
			this.bigText.TextNavigator.SelectionUnderline = !this.bigText.TextNavigator.SelectionUnderline;
		}

		private void HandleMultiFace1(object sender, MessageEventArgs e)
		{
			this.bigText.TextNavigator.SelectionFontName = "Tahoma";
		}

		private void HandleMultiFace2(object sender, MessageEventArgs e)
		{
			this.bigText.TextNavigator.SelectionFontName = "Courier New";
		}

		private void HandleMultiFace3(object sender, MessageEventArgs e)
		{
			this.bigText.TextNavigator.SelectionFontName = "Times New Roman";
		}

		private void HandleMultiSize1(object sender, MessageEventArgs e)
		{
			this.bigText.TextNavigator.SelectionFontScale = 1;
		}

		private void HandleMultiSize2(object sender, MessageEventArgs e)
		{
			this.bigText.TextNavigator.SelectionFontScale = 2;
		}

		private void HandleMultiColor1(object sender, MessageEventArgs e)
		{
			this.bigText.TextNavigator.SelectionFontColor = Color.FromBrightness(0);
		}

		private void HandleMultiColor2(object sender, MessageEventArgs e)
		{
			this.bigText.TextNavigator.SelectionFontColor = Color.FromRgb(1,0,0);
		}

		private void HandleMultiUndo(object sender, MessageEventArgs e)
		{
			this.bigText.OpletQueue.UndoAction();
		}

		private void HandleMultiRedo(object sender, MessageEventArgs e)
		{
			this.bigText.OpletQueue.RedoAction();
		}

		
		private class SpecialWidget : Widget, IToolTipHost
		{
			public SpecialWidget()
			{
			}
			
			
			#region IToolTipHost Members
			public object GetToolTipCaption(Point pos)
			{
				if (pos.X < 10)  return "Gauche";
				if (pos.X > this.ActualWidth-10)  return "Droite";
				return null;
			}
			#endregion
			
			protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clip_rect)
			{
				graphics.AddRectangle (0, 0, this.ActualWidth, this.ActualHeight);
				graphics.RenderSolid (Color.FromName ("Black"));
			}
		}
		

		[Test] public void CheckAdornerTab1()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckAdornerTab1";
			window.WindowClosed += this.HandleWindowClosed;
			window.Root.Padding = new Margins(10, 10, 10, 10);

			TabBook tb = new TabBook();
			tb.Arrows = TabBookArrows.Right;
			tb.Name = "TabBook";
			tb.Text = "";
			tb.Dock = DockStyle.Fill;
			window.Root.Children.Add(tb);
			this.tabBook = tb;

			//	Crée l'onglet 1.
			TabPage page1 = new TabPage();
			page1.Name = "p1";
			page1.TabTitle = "<m>P</m>remier";
			tb.Items.Add(page1);

			Button a = new Button();
			a.Name = "A";
			a.SetManualBounds(new Rectangle (10, 10, 75, 24));
			a.Text = "OK";
			a.ButtonStyle = ButtonStyle.DefaultAccept;
			page1.Children.Add(a);

			Button b = new Button();
			b.Name = "B";
			b.SetManualBounds(new Rectangle (95, 10, 75, 24));
			b.Text = "<m>A</m>nnuler";
			page1.Children.Add(b);

			TextFieldMulti multi = new TextFieldMulti();
			multi.Name = "Multi";
			multi.SetManualBounds(new Rectangle (10, 45, 350, 200));
			multi.Text = "1. Introduction<br/><br/>Les onglets permettent de mettre beaucoup de widgets sur une petite surface, ce qui s'avère extrèmement utile et diablement pratique.<br/><br/>2. Conclusion<br/><br/>Un truc chouette, qui sera certainement très utile dans le nouveau Crésus !";
			multi.Anchor = AnchorStyles.All;
			multi.Margins = new Margins(10, 10, 20, 40);
			page1.Children.Add(multi);

			//	Crée l'onglet 2.
			TabPage page2 = new TabPage();
			page2.Name = "p2";
			page2.TabTitle = "<m>D</m>euxième";
			tb.Items.Add(page2);

			VScroller scrollv = new VScroller();
			scrollv.Name = "Scroller";
			scrollv.SetManualBounds(new Rectangle (10, 10, 17, tb.Client.Bounds.Height-tb.GetInternalPadding ().Height-20));
			scrollv.MaxValue = 10;
			scrollv.VisibleRangeRatio = 0.3M;
			scrollv.Value = 1;
			scrollv.SmallChange = 1;
			scrollv.LargeChange = 2;
			page2.Children.Add(scrollv);

			//	Crée l'onglet 3.
			TabPage page3 = new TabPage();
			page3.Name = "p3";
			page3.TabTitle = "<m>T</m>roisième";
			tb.Items.Add(page3);

			StaticText st = new StaticText();
			st.Name = "Static";
			st.SetManualBounds(new Rectangle (50, 130, 200, 15));
			st.Text = "<b>Onglet</b> volontairement <i>vide</i> !";
			page3.Children.Add(st);

			//	Crée l'onglet 4.
			TabPage page4 = new TabPage();
			page4.Name = "p4";
			page4.TabTitle = "<m>L</m>ook";
			tb.Items.Add(page4);

			AdornerTest.CreateListLook(page4, 10, 10, null, -1);

			StaticText link = new StaticText();
			link.Name = "Link";
			link.SetManualBounds(new Rectangle (10, 50, 200, 15));
			link.Text = "Voir sur <a href=\"www.epsitec.ch\">www.epsitec.ch</a> !";
			page4.Children.Add(link);

			//	Crée l'onglet 5.
			TabPage page5 = new TabPage();
			page5.Name = "p5";
			page5.TabTitle = "<m>A</m>dd";
			tb.Items.Add(page5);

			Button add = new Button();
			add.Name = "Add";
			add.SetManualBounds(new Rectangle (100, 100, 140, 24));
			add.Text = "<m>A</m>jouter un onglet";
			add.ButtonStyle = ButtonStyle.DefaultAccept;
			add.Clicked += this.HandleAdd;
			page5.Children.Add(add);

#if true
			//	Crée l'onglet 6.
			TabPage page6 = new TabPage();
			page6.Name = "p6";
			page6.TabTitle = "Titre long";
			tb.Items.Add(page6);

			//	Crée l'onglet 7.
			TabPage page7 = new TabPage();
			page7.Name = "p7";
			page7.TabTitle = "Titre assez long";
			tb.Items.Add(page7);

			//	Crée l'onglet 8.
			TabPage page8 = new TabPage();
			page8.Name = "p8";
			page8.TabTitle = "Titre encore plus long";
			tb.Items.Add(page8);
#endif

			tb.ActivePage = page1;

			window.FocusWidget (tb);

			window.Show();
			Window.RunInTestEnvironment (window);
		}

		private void HandleWindowClosed(object sender)
		{
			this.tabBook = null;
		}

		private void HandleAdd(object sender, MessageEventArgs e)
		{
			Rectangle inside = this.tabBook.Client.Bounds;
			inside.Deflate(this.tabBook.GetInternalPadding ());
			TabPage page = new TabPage();
			page.SetManualBounds(inside);
			page.TabTitle = "Nouveau";
			this.tabBook.Items.Add(page);
		}

		[Test] public void CheckAdornerCell1()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckAdornerCell1";

			AdornerTest.CreateListLook(window.Root, 10, 10, null, -1);

			StaticText title = new StaticText();
			title.PreferredWidth = 280;
			title.PreferredHeight = 15;
			title.Text = "Sélections possibles avec Ctrl et/ou Shift :";
			title.Anchor = AnchorStyles.TopLeft;
			title.Margins = new Margins(120, 0, 55, 0);
			window.Root.Children.Add(title);

			CellTable table = new CellTable();
			table.StyleH  = CellArrayStyles.ScrollNorm;
			table.StyleH |= CellArrayStyles.Separator;
			table.StyleH |= CellArrayStyles.SelectCell;
			table.StyleH |= CellArrayStyles.SelectMulti;
			table.StyleV  = CellArrayStyles.ScrollNorm;
			table.StyleV |= CellArrayStyles.Separator;
			table.Name = "Table";
			table.SetManualBounds(new Rectangle (10, 20, 380, 200));
			table.SetArraySize(5, 12);
			for ( int y=0 ; y<12 ; y++ )
			{
				for ( int x=0 ; x<5 ; x++ )
				{
					StaticText text = new StaticText();
					text.PaintTextStyle = PaintTextStyle.Array;
					if ( x != 0 || y != 0 )  text.Text = string.Format("{0}.{1}", y+1, x+1);
					text.ContentAlignment = ContentAlignment.MiddleCenter;
					text.Dock = DockStyle.Fill;
					
					if ( x == 2 && y == 2 )
					{
						CheckButton widget = new CheckButton();
						widget.Text = "surprise";
						widget.Dock = DockStyle.Fill;
						table[x,y].Insert(widget);
					}
					else if ( x == 3 && y == 3 )
					{
						Button widget = new Button();
						widget.Text = "OK";
						widget.Dock = DockStyle.Fill;
						table[x,y].Insert(widget);
					}
					else if ( x == 1 && y == 4 )
					{
						TextField widget = new TextField();
						widget.Text = "Standard";
						widget.Dock = DockStyle.Fill;
						table[x,y].Insert(widget);
					}
					else if ( x == 2 && y == 5 )
					{
						TextField widget = new FormTextField();
						widget.Text = "Flat";
						widget.Dock = DockStyle.Fill;
						table[x,y].Insert(widget);
					}
					else if ( x == 1 && y == 6 )
					{
						TextField widget = new FormTextField();
						widget.BackColor = Color.Transparent;
						widget.Text = "Flat/Transparent";
						widget.Dock = DockStyle.Fill;
						table[x,y].Insert(widget);
					}
					else if ( x != 1 || y != 1 )
					{
						table[x,y].Insert(text);
					}
				}
			}
			table.Anchor = AnchorStyles.All;
			table.Margins = new Margins(10, 10, 80, 10);
			window.Root.Children.Add(table);

			window.FocusWidget (table);

			window.Show();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckAdornerCell2()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(500, 300);
			window.Text = "CheckAdornerCell2";

			AdornerTest.CreateListLook(window.Root, 10, 10, null, -1);

			StaticText title = new StaticText();
			title.PreferredWidth = 380;
			title.PreferredHeight = 15;
			title.Text = "Tableau de lignes editables et redimensionnable :";
			title.Anchor = AnchorStyles.TopLeft;
			title.Margins = new Margins(120, 0, 55, 0);
			window.Root.Children.Add(title);

			CellTable table = new CellTable();
			table.StyleH  = CellArrayStyles.Stretch;
			table.StyleH |= CellArrayStyles.Header;
			table.StyleH |= CellArrayStyles.Separator;
			table.StyleH |= CellArrayStyles.Mobile;
			table.StyleV  = CellArrayStyles.ScrollNorm;
			table.StyleV |= CellArrayStyles.Separator;
			table.DefHeight = 20;
			table.Name = "Table";
			table.SetManualBounds(new Rectangle (10, 20, 480, 200));
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
					TextField text = new FormTextField();
					if ( x != 1 )
					{
						text.ContentAlignment = ContentAlignment.MiddleRight;
					}
					text.Text = texts[y*5+x];
					text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					table[x,y].Insert(text);
				}
			}
			table.Anchor = AnchorStyles.All;
			table.Margins = new Margins(10, 10, 80, 10);
			window.Root.Children.Add(table);

			window.FocusWidget (table);

			window.Show();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckAdornerCell3()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckAdornerCell3";

			AdornerTest.CreateListLook (window.Root, 10, 10, null, -1);

			StaticText title = new StaticText();
			title.PreferredWidth = 280;
			title.PreferredHeight = 15;
			title.Text = "Tableau redimensionnable non éditable :";
			title.Anchor = AnchorStyles.TopLeft;
			title.Margins = new Margins(120, 0, 55, 0);
			window.Root.Children.Add(title);

			CellTable table = new CellTable();
			table.StyleH  = CellArrayStyles.ScrollNorm;
			table.StyleH |= CellArrayStyles.Header;
			table.StyleH |= CellArrayStyles.Separator;
			table.StyleH |= CellArrayStyles.Mobile;
			table.StyleH |= CellArrayStyles.Sort;
			table.StyleV  = CellArrayStyles.ScrollNorm;
			table.StyleV |= CellArrayStyles.Header;
			table.StyleV |= CellArrayStyles.Separator;
			table.StyleV |= CellArrayStyles.SelectLine;
			table.StyleV |= CellArrayStyles.SelectMulti;
			table.StyleV |= CellArrayStyles.Mobile;
			table.StyleV |= CellArrayStyles.Sort;
			table.Name = "Table";
			table.SetManualBounds(new Rectangle (10, 20, 380, 200));
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
					text.ContentAlignment = ContentAlignment.MiddleLeft;
					//text.ContentAlignment = ContentAlignment.BottomLeft;
					//text.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
					text.Dock = DockStyle.Fill;
					table[x,y].Insert(text);
#endif
				}
			}
			table.Anchor = AnchorStyles.All;
			table.Margins = new Margins(10, 10, 80, 10);
			window.Root.Children.Add(table);

			window.FocusWidget (table);

			window.Show();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckAdornerScrollArray()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(400, 300);
			window.Text = "CheckAdornerScrollArray";

			AdornerTest.CreateListLook (window.Root, 10, 10, null, -1);

			StaticText title = new StaticText();
			title.SetManualBounds(new Rectangle (120, 245, 280, 15));
			title.Text = "Tableau rapide pour liste de gauche :";
			title.Anchor = AnchorStyles.TopLeft;
			title.Margins = new Margins(120, 0, 55, 0);
			window.Root.Children.Add(title);

			ScrollArray table = new ScrollArray();
			table.SetManualBounds(new Rectangle (10, 20, 380, 200));
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
			table.Margins = new Margins(10, 10, 80, 10);
			window.Root.Children.Add(table);

			window.FocusWidget (table);

			window.Show();
			Window.RunInTestEnvironment (window);
		}


		
		public static void CreateListLook(Widget parent, double mx, double my, ToolTip tooltip, int tab)
		{
			//	Crée la liste pour changer de look.
			ScrollList sl = new ScrollList();
			
			sl.SetParent (parent);
			
			if ( tab != -1 )
			{
				sl.TabIndex = tab;
				sl.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			}

			string[] list = Factory.AdornerNames;
			int i = 0;
			int sel = 0;
			foreach ( string name in list )
			{
				sl.Items.Add(name);
				if ( name == Factory.ActiveName )  sel = i;
				i ++;
			}

			sl.MaxSize = new Size (100, 64);
			sl.PreferredSize = sl.GetBestFitSize ();
			sl.Anchor = AnchorStyles.TopLeft;
			sl.Margins = new Margins (mx, 0, my, 0);

			sl.SelectedItemIndex = sel;
			sl.ShowSelected(ScrollShowMode.Center);
			sl.SelectedItemChanged += AdornerTest.HandleLook;

			if ( tooltip != null )
			{
				tooltip.SetToolTip(sl, "Choix du look de l'interface");
			}
		}

		private static void HandleLook(object sender)
		{
			ScrollList sl = sender as ScrollList;
			int sel = sl.SelectedItemIndex;
			Factory.SetActive(sl.Items[sel]);
		}

		private static void HandleTextExEditionAccepted(object sender)
		{
			TextFieldEx text = sender as TextFieldEx;
			text.SelectAll ();
		}

		private static void HandleTextExEditionRejected(object sender)
		{
			TextFieldEx text = sender as TextFieldEx;
			text.Text = "&lt;rejected&gt;";
			text.SelectAll ();
		}
		
		[Test] public void CheckAdornerPaneBook1()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(500, 300);
			window.Text = "CheckAdornerPaneBook1";
			window.Root.Padding = new Margins(10, 10, 10, 10);

			PaneBook book = new PaneBook();
			book.PaneBookStyle = PaneBookStyle.LeftRight;
			book.PaneBehaviour = PaneBookBehaviour.FollowMe;
			//book.PaneBehaviour = PaneBookBehaviour.Draft;
			book.Dock = DockStyle.Fill;
			book.SetParent (window.Root);

			PanePage p1 = new PanePage();
			p1.PaneRelativeSize = 20;
			p1.PaneMinSize = 50;
			p1.PaneElasticity = 0;
			book.Items.Add(p1);

			Button button1 = new Button();
			button1.SetManualBounds(new Rectangle(10, 10, p1.PreferredWidth-20, p1.PreferredHeight-20));
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
			button3.Dock = DockStyle.Fill;
			p3.Padding = new Margins (10, 10, 10, 10);
			button3.Text = "P3";
			p3.Children.Add(button3);

			PanePage p4 = new PanePage();
			p4.PaneRelativeSize = 40;
			p4.PaneMinSize = 50;
			p4.PaneElasticity = 1;
			book.Items.Add(p4);

			Button button4 = new Button();
			button4.SetManualBounds(new Rectangle(10, 10, p4.PreferredWidth-20, p4.PreferredHeight-20));
			button4.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button4.Text = "P4";
			p4.Children.Add(button4);

			//	-----
			PaneBook bookv = new PaneBook();
			bookv.SetManualBounds(new Rectangle(0, 0, p2.PreferredWidth, p2.PreferredHeight));
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
			buttonv1.SetManualBounds(new Rectangle(10, 10, v1.PreferredWidth-20, v1.PreferredHeight-20));
			buttonv1.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			buttonv1.Text = "P2.1";
			v1.Children.Add(buttonv1);

			PanePage v2 = new PanePage();
			v2.PaneRelativeSize = 70;
			v2.PaneMinSize = 50;
			bookv.Items.Add(v2);

			Button buttonv2 = new Button();
			buttonv2.SetManualBounds(new Rectangle(10, 10, v2.PreferredWidth-20, v2.PreferredHeight-20));
			buttonv2.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			buttonv2.Text = "P2.2";
			v2.Children.Add(buttonv2);

			window.Show();
			Window.RunInTestEnvironment (window);
		}

		[Test] public void CheckAdornerPaneBook2()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(500, 300);
			window.Text = "CheckAdornerPaneBook2";
			window.Root.Padding = new Margins(10, 10, 10, 10);

			PaneBook book = new PaneBook();
			book.PaneBookStyle = PaneBookStyle.LeftRight;
			book.PaneBehaviour = PaneBookBehaviour.FollowMe;
			book.Dock = DockStyle.Fill;
			book.SetParent (window.Root);

			PanePage p1 = new PanePage();
			p1.PaneRelativeSize = 10;
			p1.PaneHideSize = 50;
			p1.PaneElasticity = 0;
			book.Items.Add(p1);

			Button button1 = new Button();
			button1.SetManualBounds(new Rectangle(10, 10, p1.PreferredWidth-20, p1.PreferredHeight-20));
			button1.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button1.Text = "P1";
			p1.Children.Add(button1);

			PanePage p2 = new PanePage();
			p2.PaneRelativeSize = 10;
			p2.PaneHideSize = 50;
			p2.PaneElasticity = 1;
			book.Items.Add(p2);

			Button button2 = new Button();
			button2.SetManualBounds(new Rectangle(10, 10, p2.PreferredWidth-20, p2.PreferredHeight-20));
			button2.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button2.Text = "P2";
			p2.Children.Add(button2);

			window.Show();
			Window.RunInTestEnvironment (window);
		}


		[Test] public void CheckAdornerPaneBook3()
		{
			Window window = new Window();
			
			window.ClientSize = new Size(500, 300);
			window.Text = "CheckAdornerPaneBook3";
			window.Root.Padding = new Margins(10, 10, 10, 10);

			PaneBook book = new PaneBook();
			book.PaneBookStyle = PaneBookStyle.LeftRight;
			book.PaneBehaviour = PaneBookBehaviour.FollowMe;
			book.Dock = DockStyle.Fill;
			book.SetParent (window.Root);

			PanePage p1 = new PanePage();
			p1.PaneRelativeSize = 10;
			p1.PaneMinSize = 50;
			p1.PaneElasticity = 1;
			book.Items.Add(p1);

			Button button1 = new Button();
			button1.SetManualBounds(new Rectangle(10, 10, p1.PreferredWidth-20, p1.PreferredHeight-20));
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
			button2.SetManualBounds(new Rectangle(10, 10, p2.PreferredWidth-20, p2.PreferredHeight-20));
			button2.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.TopAndBottom;
			button2.Text = "P2";
			p2.Children.Add(button2);

			window.Show();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckAdornerSplitter1()
		{
			Window window = new Window ();

			window.ClientSize = new Size (500, 300);
			window.Text = "CheckAdornerSplitter1";
			window.Root.Padding = new Margins (8, 8, 5, 5);

			StaticText text = new StaticText ();
			text.PreferredHeight = 20;
			text.Dock = DockStyle.Top;
			text.Text = "Docking utilisé: Left + Fill + Right";
			window.Root.Children.Add (text);

			Button button;
			VSplitter splitter;

			button = new Button ();
			button.Text = "1";
			button.Dock = DockStyle.Left;
			button.MinWidth = 20;
			AbstractSplitter.SetAutoCollapseEnable (button, true);
			window.Root.Children.Add (button);
			
			splitter = new VSplitter ();
			splitter.PreferredWidth = 8;
			splitter.Dock = DockStyle.Left;
			window.Root.Children.Add (splitter);

			button = new Button ();
			button.Text = "2";
			button.Dock = DockStyle.Fill;
			button.MinWidth = 20;
			AbstractSplitter.SetAutoCollapseEnable (button, true);
			window.Root.Children.Add (button);

			button = new Button ();
			button.Text = "3";
			button.Dock = DockStyle.Right;
			button.MinWidth = 20;
			window.Root.Children.Add (button);

			splitter = new VSplitter ();
			splitter.PreferredWidth = 8;
			splitter.Dock = DockStyle.Right;
			window.Root.Children.Add (splitter);

			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckAdornerSplitter2()
		{
			Window window = new Window ();

			window.ClientSize = new Size (500, 300);
			window.Text = "CheckAdornerSplitter2";
			window.Root.Padding = new Margins (8, 8, 5, 5);

			StaticText text = new StaticText ();
			text.PreferredHeight = 20;
			text.Dock = DockStyle.Top;
			text.Text = "Docking utilisé: Left + Left + Fill";
			window.Root.Children.Add (text);

			Button button;
			VSplitter splitter;

			button = new Button ();
			button.Text = "1";
			button.Dock = DockStyle.Left;
			button.MinWidth = 20;
			AbstractSplitter.SetAutoCollapseEnable (button, true);
			window.Root.Children.Add (button);

			splitter = new VSplitter ();
			splitter.PreferredWidth = 8;
			splitter.Dock = DockStyle.Left;
			window.Root.Children.Add (splitter);

			button = new Button ();
			button.Text = "2";
			button.Dock = DockStyle.Left;
			button.MinWidth = 20;
			AbstractSplitter.SetAutoCollapseEnable (button, true);
			window.Root.Children.Add (button);

			splitter = new VSplitter ();
			splitter.PreferredWidth = 8;
			splitter.Dock = DockStyle.Left;
			window.Root.Children.Add (splitter);

			button = new Button ();
			button.Text = "3";
			button.Dock = DockStyle.Fill;
			button.MinWidth = 20;
			window.Root.Children.Add (button);

			window.Show ();
			Window.RunInTestEnvironment (window);
		}
		
		[Test]
		public void CheckAdornerSplitter3()
		{
			Window window = new Window ();

			window.ClientSize = new Size (500, 300);
			window.Text = "CheckAdornerSplitter3";
			window.Root.Padding = new Margins (8, 8, 5, 5);

			StaticText text = new StaticText ();
			text.PreferredHeight = 20;
			text.Dock = DockStyle.Top;
			text.Text = "Docking utilisé: Left + Left + Left";
			window.Root.Children.Add (text);

			Button button;
			VSplitter splitter;

			button = new Button ();
			button.Text = "1";
			button.Dock = DockStyle.Left;
			button.MinWidth = 20;
			AbstractSplitter.SetAutoCollapseEnable (button, true);
			window.Root.Children.Add (button);

			splitter = new VSplitter ();
			splitter.PreferredWidth = 8;
			splitter.Dock = DockStyle.Left;
			window.Root.Children.Add (splitter);

			button = new Button ();
			button.Text = "2";
			button.Dock = DockStyle.Left;
			button.MinWidth = 20;
			AbstractSplitter.SetAutoCollapseEnable (button, true);
			window.Root.Children.Add (button);

			splitter = new VSplitter ();
			splitter.PreferredWidth = 8;
			splitter.Dock = DockStyle.Left;
			window.Root.Children.Add (splitter);

			button = new Button ();
			button.Text = "3";
			button.Dock = DockStyle.Left;
			button.MinWidth = 20;
			window.Root.Children.Add (button);

			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void CheckAdornerSplitter4()
		{
			Window window = new Window ();

			window.ClientSize = new Size (500, 300);
			window.Text = "CheckAdornerSplitter4";
			window.Root.Padding = new Margins (8, 8, 5, 5);

			StaticText text = new StaticText ();
			text.PreferredHeight = 20;
			text.Dock = DockStyle.Top;
			text.Text = "Docking utilisé: Top + Fill + Bottom";
			window.Root.Children.Add (text);

			Button button;
			HSplitter splitter;

			button = new Button ();
			button.Text = "1";
			button.Dock = DockStyle.Top;
			button.MinHeight = 20;
			AbstractSplitter.SetAutoCollapseEnable (button, true);
			window.Root.Children.Add (button);

			splitter = new HSplitter ();
			splitter.PreferredHeight = 8;
			splitter.Dock = DockStyle.Top;
			window.Root.Children.Add (splitter);

			button = new Button ();
			button.Text = "2";
			button.Dock = DockStyle.Fill;
			button.MinHeight = 20;
			AbstractSplitter.SetAutoCollapseEnable (button, true);
			window.Root.Children.Add (button);

			button = new Button ();
			button.Text = "3";
			button.Dock = DockStyle.Bottom;
			button.MinHeight = 20;
			window.Root.Children.Add (button);

			splitter = new HSplitter ();
			splitter.PreferredHeight = 8;
			splitter.Dock = DockStyle.Bottom;
			window.Root.Children.Add (splitter);

			window.Show ();
			Window.RunInTestEnvironment (window);
		}

		protected TabBook tabBook;
		protected TextFieldMulti	bigText;

		protected StaticText		stats;
	}
}
