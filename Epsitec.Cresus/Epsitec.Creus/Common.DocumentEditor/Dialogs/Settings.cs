using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.Common.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;
	using Widgets        = Common.Widgets;

	/// <summary>
	/// Dialogue des informations sur le document.
	/// </summary>
	public class Settings : Abstract
	{
		public Settings(DocumentEditor editor) : base(editor)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("Settings", 300, 435);
				this.window.Text = Res.Strings.Dialog.Settings.Title;
				this.window.Owner = this.editor.Window;
				this.window.Icon = Bitmap.FromManifestResource ("Epsitec.Common.DocumentEditor.Images.Settings.icon", this.GetType ().Assembly);
				this.window.WindowCloseClicked += this.HandleWindowSettingsCloseClicked;
				this.window.Root.MinSize = new Size(300, 435);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, 0, 0, 0);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				Viewport topPart = new Viewport(this.window.Root);
				topPart.PreferredHeight = 20;
				topPart.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				topPart.Margins = new Margins(6, 6, 6, 0);

				//	Crée les boutons radio.
				RadioButton radio1 = new RadioButton(topPart);
				radio1.Name = "RadioGlobal";
				radio1.Text = Res.Strings.Dialog.Settings.RadioGlobal;
				radio1.PreferredWidth = 80;
				radio1.Dock = DockStyle.Left;
				radio1.ActiveStateChanged += this.HandleRadioSettingsChanged;
				radio1.Index = 1;

				RadioButton radio2 = new RadioButton(topPart);
				radio2.Name = "RadioDocument";
				radio2.Text = Res.Strings.Dialog.Settings.RadioDocument;
				radio2.PreferredWidth = 80;
				radio2.Dock = DockStyle.Left;
				radio2.ActiveState = ActiveState.Yes;
				radio2.ActiveStateChanged += this.HandleRadioSettingsChanged;
				radio2.Index = 2;

				//	Crée les onglets "global".
				TabBook bookGlobal = new TabBook(this.window.Root);
				bookGlobal.Name = "BookGlobal";
				bookGlobal.Arrows = TabBookArrows.Stretch;
				bookGlobal.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				bookGlobal.Margins = new Margins(6, 6, 6+20, 34);

				TabPage bookGeneral = new TabPage();
				bookGeneral.Name = "General";
				bookGeneral.TabTitle = Res.Strings.Dialog.Settings.TabPage.General;
				bookGlobal.Items.Add(bookGeneral);

				TabPage bookPeriph = new TabPage();
				bookPeriph.Name = "Periph";
				bookPeriph.TabTitle = Res.Strings.Dialog.Settings.TabPage.Periph;
				bookGlobal.Items.Add(bookPeriph);

				TabPage bookQuick = new TabPage();
				bookQuick.Name = "Quick";
				bookQuick.TabTitle = Res.Strings.Dialog.Settings.TabPage.Quick;
				bookGlobal.Items.Add(bookQuick);

				bookGlobal.ActivePage = bookGeneral;

				TextFieldCombo combo;
				TextFieldSlider field;
				CheckButton check;
				HToolBar toolBar;
				this.tabIndex = 0;

				//	Crée l'onglet "general".
				Common.Document.DocumentDialogs.CreateTitle(bookGeneral, Res.Strings.Dialog.Settings.Startup);

				combo = this.CreateCombo(bookGeneral, "FirstAction", Res.Strings.Dialog.Settings.StartupAction);
				for ( int i=0 ; i<GlobalSettings.FirstActionCount ; i++ )
				{
					Common.Document.Settings.FirstAction action = GlobalSettings.FirstActionType(i);
					combo.Items.Add(GlobalSettings.FirstActionString(action));
				}
				combo.SelectedItemIndex = GlobalSettings.FirstActionRank (this.globalSettings.FirstAction);

				check = this.CreateCheck(bookGeneral, "SplashScreen", Res.Strings.Dialog.Settings.SplashScreen);
				check.ActiveState = this.globalSettings.SplashScreen ? ActiveState.Yes : ActiveState.No;

				Common.Document.DocumentDialogs.CreateTitle(bookGeneral, Res.Strings.Dialog.Settings.AutoUpdate);

				check = this.CreateCheck(bookGeneral, "AutoChecker", Res.Strings.Dialog.Settings.AutoChecker);
				check.ActiveState = this.globalSettings.AutoChecker ? ActiveState.Yes : ActiveState.No;

				Common.Document.DocumentDialogs.CreateTitle(bookGeneral, Res.Strings.Dialog.Settings.PanelProperties);

				check = this.CreateCheck(bookGeneral, "LabelProperties", Res.Strings.Dialog.Settings.LabelProperties);
				check.ActiveState = this.globalSettings.LabelProperties ? ActiveState.Yes : ActiveState.No;

				Common.Document.DocumentDialogs.CreateSeparator(bookGeneral);

				//	Crée l'onglet "periph".
				Common.Document.DocumentDialogs.CreateTitle(bookPeriph, Res.Strings.Dialog.Settings.Mouse);

				combo = this.CreateCombo(bookPeriph, "MouseWheelAction", Res.Strings.Dialog.Settings.MouseWheel);
				for ( int i=0 ; i<GlobalSettings.MouseWheelActionCount ; i++ )
				{
					Common.Document.Settings.MouseWheelAction action = GlobalSettings.MouseWheelActionType(i);
					combo.Items.Add(GlobalSettings.MouseWheelActionString(action));
				}
				combo.SelectedItemIndex = GlobalSettings.MouseWheelActionRank (this.globalSettings.MouseWheelAction);

				field = this.CreateField(bookPeriph, "DefaultZoom", Res.Strings.Dialog.Settings.MouseZoom);
				field.MinValue = 1.1M;
				field.MaxValue = 4.0M;
				field.Step = 0.1M;
				field.Resolution = 0.1M;
				field.Value = (decimal) this.globalSettings.DefaultZoom;

				check = this.CreateCheck(bookPeriph, "FineCursor", Res.Strings.Dialog.Settings.FineCursor);
				check.ActiveState = this.globalSettings.FineCursor ? ActiveState.Yes : ActiveState.No;

				Common.Document.DocumentDialogs.CreateTitle(bookPeriph, Res.Strings.Dialog.Settings.Screen);

				combo = this.CreateCombo(bookPeriph, "Adorner", Res.Strings.Dialog.Settings.Adorner);
				string[] list = Widgets.Adorners.Factory.AdornerNames;
				int rank = 0;
				foreach ( string name in list )
				{
#if !DEBUG
					if ( name == "LookAquaMetal" )  continue;
					if ( name == "LookAquaDyna" )  continue;
					if ( name == "LookXP" )  continue;
#endif
					combo.Items.Add(name);
					if ( name == Widgets.Adorners.Factory.ActiveName )
					{
						combo.SelectedItemIndex = rank;
					}
					rank++;
				}

				field = this.CreateField(bookPeriph, "ScreenDpi", Res.Strings.Dialog.Settings.ScreenDpi);
				field.MinValue = 30.0M;
				field.MaxValue = 300.0M;
				field.Step = 1.0M;
				field.Resolution = 1.0M;
				field.Value = (decimal) this.globalSettings.ScreenDpi;
				
				Common.Document.DocumentDialogs.CreateSeparator(bookPeriph);

				//	Crée l'onglet "quick".
				Common.Document.DocumentDialogs.CreateTitle(bookQuick, Res.Strings.Dialog.Settings.TabPage.QuickHelp);

				toolBar = new HToolBar(bookQuick);
				toolBar.Dock = DockStyle.Top;
				toolBar.Margins = new Margins (10, 10, 2, -1);
				toolBar.TabIndex = tabIndex ++;
				toolBar.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

				this.buttonQuickFirst = new IconButton(Misc.Icon("First"));
				this.buttonQuickFirst.Clicked += this.HandleButtonQuickFirst;
				this.buttonQuickFirst.TabIndex = tabIndex++;
				this.buttonQuickFirst.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonQuickFirst);
				ToolTip.Default.SetToolTip(this.buttonQuickFirst, Res.Strings.Dialog.Settings.QuickFirst);

				this.buttonQuickUp = new IconButton(Misc.Icon("Up"));
				this.buttonQuickUp.Clicked += this.HandleButtonQuickUp;
				this.buttonQuickUp.TabIndex = tabIndex++;
				this.buttonQuickUp.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonQuickUp);
				ToolTip.Default.SetToolTip(this.buttonQuickUp, Res.Strings.Dialog.Settings.QuickUp);

				this.buttonQuickDown = new IconButton(Misc.Icon("Down"));
				this.buttonQuickDown.Clicked += this.HandleButtonQuickDown;
				this.buttonQuickDown.TabIndex = tabIndex++;
				this.buttonQuickDown.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonQuickDown);
				ToolTip.Default.SetToolTip(this.buttonQuickDown, Res.Strings.Dialog.Settings.QuickDown);

				this.buttonQuickLast = new IconButton(Misc.Icon("Last"));
				this.buttonQuickLast.Clicked += this.HandleButtonQuickLast;
				this.buttonQuickLast.TabIndex = tabIndex++;
				this.buttonQuickLast.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonQuickLast);
				ToolTip.Default.SetToolTip(this.buttonQuickLast, Res.Strings.Dialog.Settings.QuickLast);

				toolBar.Items.Add(new IconSeparator());

				this.buttonQuickDefault = new IconButton(Misc.Icon("QuickDefault"));
				this.buttonQuickDefault.Clicked += this.HandleButtonQuickDefault;
				this.buttonQuickDefault.TabIndex = tabIndex++;
				this.buttonQuickDefault.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonQuickDefault);
				ToolTip.Default.SetToolTip(this.buttonQuickDefault, Res.Strings.Dialog.Settings.QuickDefault);

				toolBar.Items.Add(new IconSeparator());

				this.buttonQuickClear = new IconButton(Misc.Icon("QuickClear"));
				this.buttonQuickClear.Clicked += this.HandleButtonQuickClear;
				this.buttonQuickClear.TabIndex = tabIndex++;
				this.buttonQuickClear.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonQuickClear);
				ToolTip.Default.SetToolTip(this.buttonQuickClear, Res.Strings.Dialog.Settings.QuickClear);

				toolBar.Items.Add(new IconSeparator());

				this.quickList = new CellTable(bookQuick);
				this.quickList.DefHeight = 21;
				this.quickList.Dock = DockStyle.Fill;
				this.quickList.StyleH = CellArrayStyles.Stretch | CellArrayStyles.Separator;
				this.quickList.StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine;
				this.quickList.Margins = new Margins (10, 10, 0, 0);
				this.quickList.SelectionChanged += this.HandleQuickListSelectionChanged;

				this.UpdateQuickList(-1);
				this.UpdateQuickButtons();

				//	Crée les onglets "document".
				TabBook bookDoc = new TabBook(this.window.Root);
				bookDoc.Name = "BookDocument";
				bookDoc.Arrows = TabBookArrows.Stretch;
				bookDoc.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				bookDoc.Margins = new Margins(6, 6, 6+20, 34);

				TabPage bookFormat = new TabPage();
				bookFormat.Name = "Format";
				bookFormat.TabTitle = Res.Strings.Dialog.Settings.TabPage.Format;
				bookDoc.Items.Add(bookFormat);

				TabPage bookGrid = new TabPage();
				bookGrid.Name = "Grid";
				bookGrid.TabTitle = Res.Strings.Dialog.Settings.TabPage.Grid;
				bookDoc.Items.Add(bookGrid);

				TabPage bookGuides = new TabPage();
				bookGuides.Name = "Guides";
				bookGuides.TabTitle = Res.Strings.Dialog.Settings.TabPage.Guides;
				bookDoc.Items.Add(bookGuides);

				TabPage bookMove = new TabPage();
				bookMove.Name = "Move";
				bookMove.TabTitle = Res.Strings.Dialog.Settings.TabPage.Move;
				bookDoc.Items.Add(bookMove);

				TabPage bookMisc = new TabPage();
				bookMisc.Name = "Misc";
				bookMisc.TabTitle = Res.Strings.Dialog.Settings.TabPage.Misc;
				bookDoc.Items.Add(bookMisc);

				TabPage bookFonts = new TabPage();
				bookFonts.Name = "Fonts";
				bookFonts.TabTitle = Res.Strings.Dialog.Settings.TabPage.Fonts;
				bookDoc.Items.Add(bookFonts);

				bookDoc.ActivePage = bookFormat;

				//	Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultAcceptAndCancel;
				buttonClose.Anchor = AnchorStyles.BottomRight;
				buttonClose.Margins = new Margins(0, 6, 0, 6);
				buttonClose.Clicked += this.HandleSettingsButtonCloseClicked;
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);

				this.ActiveBook("BookDocument");
			}

			if ( this.editor.HasCurrentDocument )
			{
				this.editor.CurrentDocument.Dialogs.BuildSettings(this.window);
			}

			this.window.Show();
		}

		public override void Save()
		{
			//	Enregistre la position de la fenêtre du dialogue.
			this.WindowSave("Settings");
		}

		public override void Rebuild()
		{
			//	Reconstruit le dialogue.
			if ( !this.editor.HasCurrentDocument )  return;
			if ( this.window == null )  return;
			this.editor.CurrentDocument.Dialogs.BuildSettings(this.window);
		}

		public void ShowPage(string book, string tab)
		{
			//	Montre une page donnée du dialogue.
			this.ActiveBook(book);

			this.ignoreChange = true;

			RadioButton radio1 = this.window.Root.FindChild("RadioGlobal") as RadioButton;
			if ( radio1 != null )
			{
				radio1.ActiveState = (book == "BookGlobal") ? ActiveState.Yes : ActiveState.No;
			}

			RadioButton radio2 = this.window.Root.FindChild("RadioDocument") as RadioButton;
			if ( radio2 != null )
			{
				radio2.ActiveState = (book == "BookDocument") ? ActiveState.Yes : ActiveState.No;
			}

			this.ignoreChange = false;

			TabBook tabBook = this.window.Root.FindChild(book) as TabBook;
			if ( tabBook == null )  return;

			TabPage tabPage = tabBook.FindChild(tab) as TabPage;
			if ( tabPage == null )  return;

			tabBook.ActivePage = tabPage;
		}


		protected TextFieldCombo CreateCombo(Widget parent, string name, string label)
		{
			//	Crée un widget combo.
			Viewport container = new Viewport(parent);
			container.PreferredHeight = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins (10, 10, 0, 5);

			StaticText text = new StaticText(container);
			text.Text = label;
			text.PreferredWidth = 100;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins (0, 0, 0, 0);

			TextFieldCombo field = new TextFieldCombo(container);
			field.PreferredWidth = 160;
			field.IsReadOnly = true;
			field.Name = name;
			field.SelectedItemChanged += this.HandleComboSettingsChanged;
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins (0, 0, 0, 0);
			return field;
		}

		protected TextFieldSlider CreateField(Widget parent, string name, string label)
		{
			//	Crée un widget textfield.
			Viewport container = new Viewport(parent);
			container.PreferredHeight = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins (10, 10, 0, 5);

			StaticText text = new StaticText(container);
			text.Text = label;
			text.PreferredWidth = 100;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins (0, 0, 0, 0);

			TextFieldSlider field = new TextFieldSlider(container);
			field.PreferredWidth = 50;
			field.Name = name;
			field.ValueChanged += this.HandleDoubleSettingsChanged;
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins (0, 0, 0, 0);
			return field;
		}

		protected CheckButton CreateCheck(Widget parent, string name, string text)
		{
			//	Crée un widget checkbutton.
			CheckButton check = new CheckButton(parent);
			check.Text = text;
			check.Name = name;
			check.Clicked +=this.HandleCheckSettingsClicked;
			check.TabIndex = this.tabIndex++;
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.Dock = DockStyle.Top;
			check.Margins = new Margins (10+100, 0, 0, 5);
			return check;
		}

		protected void ActiveBook(string book)
		{
			this.window.Root.FindChild("BookGlobal").Visibility = (book == "BookGlobal");
			this.window.Root.FindChild("BookDocument").Visibility = (book == "BookDocument");
		}

		private void HandleRadioSettingsChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			RadioButton radio = sender as RadioButton;
			
			if ( radio == null )  return;
			if ( radio.ActiveState != ActiveState.Yes )  return;
			
			if ( radio.Name == "RadioGlobal" )
			{
				this.ActiveBook("BookGlobal");
			}
			else
			{
				this.ActiveBook("BookDocument");
			}
		}
		
		private void HandleComboSettingsChanged(object sender)
		{
			TextFieldCombo combo = sender as TextFieldCombo;

			if ( combo.Name == "FirstAction" )
			{
				this.globalSettings.FirstAction = GlobalSettings.FirstActionType (combo.SelectedItemIndex);
			}

			if ( combo.Name == "MouseWheelAction" )
			{
				this.globalSettings.MouseWheelAction = GlobalSettings.MouseWheelActionType (combo.SelectedItemIndex);
			}

			if ( combo.Name == "Adorner" )
			{
				this.globalSettings.Adorner = combo.Text;
				Widgets.Adorners.Factory.SetActive(combo.Text);
			}
		}

		private void HandleDoubleSettingsChanged(object sender)
		{
			TextFieldSlider field = sender as TextFieldSlider;

			if ( field.Name == "ScreenDpi" )
			{
				this.globalSettings.ScreenDpi = (double) field.Value;
				if ( this.editor.HasCurrentDocument )
				{
					this.editor.CurrentDocument.Notifier.NotifyAllChanged();
				}
			}

			if ( field.Name == "DefaultZoom" )
			{
				this.globalSettings.DefaultZoom = (double) field.Value;
			}
		}

		private void HandleCheckSettingsClicked(object sender, MessageEventArgs e)
		{
			CheckButton check = sender as CheckButton;

			if ( check.Name == "SplashScreen" )
			{
				this.globalSettings.SplashScreen = !this.globalSettings.SplashScreen;
			}

			if ( check.Name == "FineCursor" )
			{
				this.globalSettings.FineCursor = !this.globalSettings.FineCursor;
			}

			if ( check.Name == "AutoChecker" )
			{
				this.globalSettings.AutoChecker = !this.globalSettings.AutoChecker;
			}

			if ( check.Name == "LabelProperties" )
			{
				this.globalSettings.LabelProperties = !this.globalSettings.LabelProperties;

				if ( this.editor.HasCurrentDocument )
				{
					this.editor.CurrentDocument.Notifier.NotifyLabelPropertiesChanged();
				}
			}
		}

		private void HandleWindowSettingsCloseClicked(object sender)
		{
			this.CloseWindow();
		}

		private void HandleSettingsButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.CloseWindow();
		}


		#region QuickCommands
		protected void UpdateQuickButtons()
		{
			//	Met à jour les boutons des commandes rapides.
			int sel = this.quickList.SelectedRow;

			if ( sel == -1 )
			{
				this.buttonQuickFirst.Enable = false;
				this.buttonQuickUp.Enable = false;
				this.buttonQuickDown.Enable = false;
				this.buttonQuickLast.Enable = false;
			}
			else
			{
				this.buttonQuickFirst.Enable = (sel > 0);
				this.buttonQuickUp.Enable = (sel > 0);
				this.buttonQuickDown.Enable = (sel < this.globalSettings.QuickCommands.Count-1);
				this.buttonQuickLast.Enable = (sel < this.globalSettings.QuickCommands.Count-1);
			}
		}

		/// <summary>
		/// Updates the quick list.
		/// </summary>
		/// <param name="sel">The sel.</param>
		protected void UpdateQuickList(int sel)
		{
			//	Met à jour la liste des commandes rapides.
			this.ignoreChange = true;

			int rows = this.globalSettings.QuickCommands.Count;

			this.quickList.SetArraySize(4, rows);
			this.quickList.SetWidthColumn(0,  22);
			this.quickList.SetWidthColumn(1,  29);
			this.quickList.SetWidthColumn(2,  21);
			this.quickList.SetWidthColumn(3, 168);

			CheckButton bt;
			StaticText st;
			IconButton ib;
			for ( int row=0 ; row<rows ; row++ )
			{
				for ( int column=0 ; column<this.quickList.Columns ; column++ )
				{
					if ( this.quickList[column, row].IsEmpty )
					{
						if ( column == 0 )  // bouton check pour déterminer la visibilité ?
						{
							bt = new CheckButton();
							bt.Name = row.ToString();
							bt.Dock = DockStyle.Fill;
							bt.Margins = new Margins(4, 0, 0, 0);
							bt.ActiveStateChanged += this.HandleQuickUsedChanged;
							this.quickList[column, row].Insert(bt);
						}
						else if ( column == 1 )  // icône ?
						{
							ib = new IconButton();
							ib.Name = row.ToString();
							ib.Dock = DockStyle.Fill;
							ib.Margins = new Margins(0, 0, 0, 0);
							this.quickList[column, row].Insert(ib);
						}
						else if ( column == 2 )  // bouton pour le séparateur ?
						{
							ib = new IconButton();
							ib.Name = row.ToString();
							ib.Dock = DockStyle.Fill;
							ib.Margins = new Margins(1, 1, 1, 1);
							ib.Clicked += this.HandleQuickSeparatorClicked;
							this.quickList[column, row].Insert(ib);
						}
						else if ( column == 3 )  // texte de la commande ?
						{
							st = new StaticText();
							st.ContentAlignment = ContentAlignment.MiddleLeft;
							st.Dock = DockStyle.Fill;
							st.Margins = new Margins(6, 0, 0, 0);
							this.quickList[column, row].Insert(st);
						}
					}
				}

				string xcmd = this.globalSettings.QuickCommands[row] as string;
				bool   used = GlobalSettings.QuickUsed(xcmd);
				bool   sep  = GlobalSettings.QuickSep(xcmd);
				string cmd  = GlobalSettings.QuickCmd(xcmd);

				Command c = Command.Find(cmd);

				//	Bouton check pour déterminer la visibilité.
				bt = this.quickList[0, row].Children[0] as CheckButton;
				bt.ActiveState = used ? ActiveState.Yes : ActiveState.No;

				//	Icône.
				ib = this.quickList[1, row].Children[0] as IconButton;
				ib.IconUri = c == null ? "" : c.Icon;

				//	Bouton pour le séparateur.
				ib = this.quickList[2, row].Children[0] as IconButton;
				ib.IconUri = Misc.Icon(sep ? "QuickSeparatorYes" : "QuickSeparatorNo");
				ib.Enable = (used);

				//	Texte de la commande.
				st = this.quickList[3, row].Children[0] as StaticText;
				st.Text = c == null ? "" : c.Description;

				this.quickList.SelectRow(row, row==sel);
			}

			if ( sel != -1 )
			{
				this.quickList.ShowSelect();  // montre la ligne sélectionnée
			}

			this.ignoreChange = false;
		}

		protected void MoveQuickCommand(int src, int dst)
		{
			//	Déplace une commande rapide.
			if ( src == -1 )  return;

			string xcmd = this.globalSettings.QuickCommands[src] as string;
			this.globalSettings.QuickCommands.RemoveAt(src);
			this.globalSettings.QuickCommands.Insert(dst, xcmd);

			this.UpdateQuickList(dst);
			this.UpdateQuickButtons();
			this.editor.UpdateQuickCommands();
		}

		private void HandleQuickListSelectionChanged(object sender)
		{
			//	Liste des commandes rapides cliquée.
			if ( this.ignoreChange )  return;
			this.UpdateQuickButtons();
		}

		private void HandleQuickUsedChanged(object sender)
		{
			//	Bouton "check" dans la liste des commandes rapides cliqué.
			if ( this.ignoreChange )  return;

			CheckButton bt = sender as CheckButton;
			int sel = System.Convert.ToInt32(bt.Name);
			string xcmd = this.globalSettings.QuickCommands[sel] as string;
			bool   used = GlobalSettings.QuickUsed(xcmd);
			bool   sep  = GlobalSettings.QuickSep(xcmd);
			string cmd  = GlobalSettings.QuickCmd(xcmd);

			used = !used;
			if ( !used )  sep = false;

			this.globalSettings.QuickCommands[sel] = GlobalSettings.QuickXcmd(used, sep, cmd);

			this.UpdateQuickList(sel);
			this.UpdateQuickButtons();
			this.editor.UpdateQuickCommands();
		}

		private void HandleQuickSeparatorClicked(object sender, MessageEventArgs e)
		{
			//	Bouton "séparateur" dans la liste des commandes rapides cliqué.
			if ( this.ignoreChange )  return;

			IconButton ib = sender as IconButton;
			int sel = System.Convert.ToInt32(ib.Name);
			string xcmd = this.globalSettings.QuickCommands[sel] as string;
			bool   used = GlobalSettings.QuickUsed(xcmd);
			bool   sep  = GlobalSettings.QuickSep(xcmd);
			string cmd  = GlobalSettings.QuickCmd(xcmd);

			sep = !sep;

			this.globalSettings.QuickCommands[sel] = GlobalSettings.QuickXcmd(used, sep, cmd);

			this.UpdateQuickList(sel);
			this.UpdateQuickButtons();
			this.editor.UpdateQuickCommands();
		}

		private void HandleButtonQuickDefault(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;

			this.globalSettings.QuickCommands = GlobalSettings.DefaultQuickCommands();

			this.UpdateQuickList(sel);
			this.UpdateQuickButtons();
			this.editor.UpdateQuickCommands();
		}

		private void HandleButtonQuickClear(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;

			for ( int i=0 ; i<this.globalSettings.QuickCommands.Count ; i++ )
			{
				string xcmd = this.globalSettings.QuickCommands[i] as string;
				bool   sep  = GlobalSettings.QuickSep(xcmd);
				string cmd  = GlobalSettings.QuickCmd(xcmd);

				this.globalSettings.QuickCommands[i] = GlobalSettings.QuickXcmd(false, sep, cmd);
			}

			this.UpdateQuickList(sel);
			this.UpdateQuickButtons();
			this.editor.UpdateQuickCommands();
		}

		private void HandleButtonQuickFirst(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;
			this.MoveQuickCommand(sel, 0);
		}

		private void HandleButtonQuickUp(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;
			this.MoveQuickCommand(sel, sel-1);
		}

		private void HandleButtonQuickDown(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;
			this.MoveQuickCommand(sel, sel+1);
		}

		private void HandleButtonQuickLast(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;
			this.MoveQuickCommand(sel, this.globalSettings.QuickCommands.Count-1);
		}
		#endregion


		protected int							tabIndex;
		protected bool							ignoreChange = false;
		protected CellTable						quickList;
		protected IconButton					buttonQuickDefault;
		protected IconButton					buttonQuickClear;
		protected IconButton					buttonQuickFirst;
		protected IconButton					buttonQuickUp;
		protected IconButton					buttonQuickDown;
		protected IconButton					buttonQuickLast;
	}
}
