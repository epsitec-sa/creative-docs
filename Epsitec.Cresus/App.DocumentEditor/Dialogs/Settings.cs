using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
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

		// Crée et montre la fenêtre du dialogue.
		public override void Show()
		{
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.MakeFixedSizeWindow();
				this.window.MakeToolWindow();
				this.WindowInit("Settings", 300, 412);
				this.window.Text = Res.Strings.Dialog.Settings.Title;
				this.window.PreventAutoClose = true;
				this.window.Owner = this.editor.Window;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowSettingsCloseClicked);

				Panel topPart = new Panel(this.window.Root);
				topPart.Height = 20;
				topPart.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
				topPart.AnchorMargins = new Margins(6, 6, 6, 0);

				// Crée les boutons radio.
				RadioButton radio1 = new RadioButton(topPart);
				radio1.Name = "RadioGlobal";
				radio1.Text = Res.Strings.Dialog.Settings.RadioGlobal;
				radio1.Width = 80;
				radio1.Dock = DockStyle.Left;
				radio1.ActiveStateChanged += new EventHandler(this.HandleRadioSettingsChanged);
				radio1.Index = 1;

				RadioButton radio2 = new RadioButton(topPart);
				radio2.Name = "RadioDocument";
				radio2.Text = Res.Strings.Dialog.Settings.RadioDocument;
				radio2.Width = 80;
				radio2.Dock = DockStyle.Left;
				radio2.ActiveState = WidgetState.ActiveYes;
				radio2.ActiveStateChanged += new EventHandler(this.HandleRadioSettingsChanged);
				radio2.Index = 2;

				// Crée les onglest "global".
				TabBook bookGlobal = new TabBook(this.window.Root);
				bookGlobal.Name = "BookGlobal";
				bookGlobal.Arrows = TabBookArrows.Stretch;
				bookGlobal.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				bookGlobal.AnchorMargins = new Margins(6, 6, 6+20, 34);

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
				this.tabIndex = 0;

				// Crée l'onglet "general".
				Common.Document.Dialogs.CreateTitle(bookGeneral, Res.Strings.Dialog.Settings.Startup);

				combo = this.CreateCombo(bookGeneral, "FirstAction", "Action");
				for ( int i=0 ; i<GlobalSettings.FirstActionCount ; i++ )
				{
					Common.Document.Settings.FirstAction action = GlobalSettings.FirstActionType(i);
					combo.Items.Add(GlobalSettings.FirstActionString(action));
				}
				combo.SelectedIndex = GlobalSettings.FirstActionRank(this.globalSettings.FirstAction);

				check = this.CreateCheck(bookGeneral, "SplashScreen", Res.Strings.Dialog.Settings.SplashScreen);
				check.ActiveState = this.globalSettings.SplashScreen ? WidgetState.ActiveYes : WidgetState.ActiveNo;

				Common.Document.Dialogs.CreateTitle(bookGeneral, Res.Strings.Dialog.Settings.AutoUpdate);

				check = this.CreateCheck(bookGeneral, "AutoChecker", Res.Strings.Dialog.Settings.AutoChecker);
				check.ActiveState = this.globalSettings.AutoChecker ? WidgetState.ActiveYes : WidgetState.ActiveNo;

				Common.Document.Dialogs.CreateTitle(bookGeneral, Res.Strings.Dialog.Settings.PanelProperties);

				check = this.CreateCheck(bookGeneral, "LabelProperties", Res.Strings.Dialog.Settings.LabelProperties);
				check.ActiveState = this.globalSettings.LabelProperties ? WidgetState.ActiveYes : WidgetState.ActiveNo;

				Common.Document.Dialogs.CreateSeparator(bookGeneral);

				// Crée l'onglet "periph".
				Common.Document.Dialogs.CreateTitle(bookPeriph, Res.Strings.Dialog.Settings.Mouse);

				combo = this.CreateCombo(bookPeriph, "MouseWheelAction", Res.Strings.Dialog.Settings.MouseWheel);
				for ( int i=0 ; i<GlobalSettings.MouseWheelActionCount ; i++ )
				{
					Common.Document.Settings.MouseWheelAction action = GlobalSettings.MouseWheelActionType(i);
					combo.Items.Add(GlobalSettings.MouseWheelActionString(action));
				}
				combo.SelectedIndex = GlobalSettings.MouseWheelActionRank(this.globalSettings.MouseWheelAction);

				field = this.CreateField(bookPeriph, "DefaultZoom", Res.Strings.Dialog.Settings.MouseZoom);
				field.MinValue = 1.1M;
				field.MaxValue = 4.0M;
				field.Step = 0.1M;
				field.Resolution = 0.1M;
				field.Value = (decimal) this.globalSettings.DefaultZoom;

				check = this.CreateCheck(bookPeriph, "FineCursor", Res.Strings.Dialog.Settings.FineCursor);
				check.ActiveState = this.globalSettings.FineCursor ? WidgetState.ActiveYes : WidgetState.ActiveNo;

				Common.Document.Dialogs.CreateTitle(bookPeriph, Res.Strings.Dialog.Settings.Screen);

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
						combo.SelectedIndex = rank;
					}
					rank++;
				}

				field = this.CreateField(bookPeriph, "ScreenDpi", Res.Strings.Dialog.Settings.ScreenDpi);
				field.MinValue = 30.0M;
				field.MaxValue = 300.0M;
				field.Step = 1.0M;
				field.Resolution = 1.0M;
				field.Value = (decimal) this.globalSettings.ScreenDpi;
				
				Common.Document.Dialogs.CreateSeparator(bookPeriph);

				// Crée l'onglet "quick".
				HToolBar toolBar = new HToolBar(bookQuick);
				toolBar.Dock = DockStyle.Top;
				toolBar.DockMargins = new Margins(10, 10, 10, -1);
				toolBar.TabIndex = tabIndex ++;
				toolBar.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

				this.buttonFirst = new IconButton(Misc.Icon("First"));
				this.buttonFirst.Clicked += new MessageEventHandler(this.HandleQuickButtonFirst);
				this.buttonFirst.TabIndex = tabIndex++;
				this.buttonFirst.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonFirst);
				ToolTip.Default.SetToolTip(this.buttonFirst, Res.Strings.Dialog.Settings.QuickFirst);

				this.buttonUp = new IconButton(Misc.Icon("Up"));
				this.buttonUp.Clicked += new MessageEventHandler(this.HandleQuickButtonUp);
				this.buttonUp.TabIndex = tabIndex++;
				this.buttonUp.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonUp);
				ToolTip.Default.SetToolTip(this.buttonUp, Res.Strings.Dialog.Settings.QuickUp);

				this.buttonDown = new IconButton(Misc.Icon("Down"));
				this.buttonDown.Clicked += new MessageEventHandler(this.HandleQuickButtonDown);
				this.buttonDown.TabIndex = tabIndex++;
				this.buttonDown.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonDown);
				ToolTip.Default.SetToolTip(this.buttonDown, Res.Strings.Dialog.Settings.QuickDown);

				this.buttonLast = new IconButton(Misc.Icon("Last"));
				this.buttonLast.Clicked += new MessageEventHandler(this.HandleQuickButtonLast);
				this.buttonLast.TabIndex = tabIndex++;
				this.buttonLast.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonLast);
				ToolTip.Default.SetToolTip(this.buttonLast, Res.Strings.Dialog.Settings.QuickLast);

				toolBar.Items.Add(new IconSeparator());

				this.buttonDefault = new IconButton(Misc.Icon("QuickDefault"));
				this.buttonDefault.Clicked += new MessageEventHandler(this.HandleQuickButtonDefault);
				this.buttonDefault.TabIndex = tabIndex++;
				this.buttonDefault.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonDefault);
				ToolTip.Default.SetToolTip(this.buttonDefault, Res.Strings.Dialog.Settings.QuickDefault);

				toolBar.Items.Add(new IconSeparator());

				this.buttonClear = new IconButton(Misc.Icon("QuickClear"));
				this.buttonClear.Clicked += new MessageEventHandler(this.HandleQuickButtonClear);
				this.buttonClear.TabIndex = tabIndex++;
				this.buttonClear.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				toolBar.Items.Add(this.buttonClear);
				ToolTip.Default.SetToolTip(this.buttonClear, Res.Strings.Dialog.Settings.QuickClear);

				toolBar.Items.Add(new IconSeparator());

				this.quickList = new CellTable(bookQuick);
				this.quickList.Height = 280;
				this.quickList.DefHeight = 21;
				this.quickList.Dock = DockStyle.Top;
				this.quickList.StyleH = CellArrayStyle.Stretch | CellArrayStyle.Separator;
				this.quickList.StyleV = CellArrayStyle.ScrollNorm | CellArrayStyle.Separator | CellArrayStyle.SelectLine;
				this.quickList.DockMargins = new Margins(10, 10, 0, 0);
				this.quickList.SelectionChanged += new EventHandler(this.HandleQuickListSelectionChanged);

				this.UpdateQuickList(-1);
				this.UpdateQuickButtons();
				
				// Crée les onglets "document".
				TabBook bookDoc = new TabBook(this.window.Root);
				bookDoc.Name = "BookDocument";
				bookDoc.Arrows = TabBookArrows.Stretch;
				bookDoc.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				bookDoc.AnchorMargins = new Margins(6, 6, 6+20, 34);

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

				bookDoc.ActivePage = bookFormat;

				// Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Close;
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(6, 0, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleSettingsButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, Res.Strings.Dialog.Tooltip.Close);

				this.ActiveBook("BookDocument");
			}

			if ( this.editor.IsCurrentDocument )
			{
				this.editor.CurrentDocument.Dialogs.BuildSettings(this.window);
			}

			this.window.Show();
		}

		// Enregistre la position de la fenêtre du dialogue.
		public override void Save()
		{
			this.WindowSave("Settings");
		}

		// Reconstruit le dialogue.
		public override void Rebuild()
		{
			if ( !this.editor.IsCurrentDocument )  return;
			if ( this.window == null )  return;
			this.editor.CurrentDocument.Dialogs.BuildSettings(this.window);
		}


		// Crée un widget combo.
		protected TextFieldCombo CreateCombo(Widget parent, string name, string label)
		{
			Panel container = new Panel(parent);
			container.Height = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 5);

			StaticText text = new StaticText(container);
			text.Text = label;
			text.Width = 100;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			TextFieldCombo field = new TextFieldCombo(container);
			field.Width = 160;
			field.IsReadOnly = true;
			field.Name = name;
			field.SelectedIndexChanged += new EventHandler(this.HandleComboSettingsChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			return field;
		}

		// Crée un widget textfield.
		protected TextFieldSlider CreateField(Widget parent, string name, string label)
		{
			Panel container = new Panel(parent);
			container.Height = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 5);

			StaticText text = new StaticText(container);
			text.Text = label;
			text.Width = 100;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			TextFieldSlider field = new TextFieldSlider(container);
			field.Width = 50;
			field.Name = name;
			field.ValueChanged += new EventHandler(this.HandleDoubleSettingsChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			return field;
		}

		// Crée un widget checkbutton.
		protected CheckButton CreateCheck(Widget parent, string name, string text)
		{
			CheckButton check = new CheckButton(parent);
			check.Text = text;
			check.Name = name;
			check.Clicked +=new MessageEventHandler(this.HandleCheckSettingsClicked);
			check.TabIndex = this.tabIndex++;
			check.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			check.Dock = DockStyle.Top;
			check.DockMargins = new Margins(10+100, 0, 0, 5);
			return check;
		}

		protected void ActiveBook(string book)
		{
			this.window.Root.FindChild("BookGlobal").SetVisible(book == "BookGlobal");
			this.window.Root.FindChild("BookDocument").SetVisible(book == "BookDocument");
		}

		private void HandleRadioSettingsChanged(object sender)
		{
			RadioButton radio = sender as RadioButton;
			
			if ( radio == null )  return;
			if ( radio.ActiveState != WidgetState.ActiveYes )  return;
			
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
				this.globalSettings.FirstAction = GlobalSettings.FirstActionType(combo.SelectedIndex);
			}

			if ( combo.Name == "MouseWheelAction" )
			{
				this.globalSettings.MouseWheelAction = GlobalSettings.MouseWheelActionType(combo.SelectedIndex);
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
				if ( this.editor.IsCurrentDocument )
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

				if ( this.editor.IsCurrentDocument )
				{
					this.editor.CurrentDocument.Notifier.NotifyLabelPropertiesChanged();
				}
			}
		}

		private void HandleWindowSettingsCloseClicked(object sender)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleSettingsButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.editor.Window.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}


		#region QuickCommands
		// Met à jour les boutons des commandes rapides.
		protected void UpdateQuickButtons()
		{
			int sel = this.quickList.SelectedRow;

			if ( sel == -1 )
			{
				this.buttonFirst.SetEnabled(false);
				this.buttonUp.SetEnabled(false);
				this.buttonDown.SetEnabled(false);
				this.buttonLast.SetEnabled(false);
			}
			else
			{
				this.buttonFirst.SetEnabled(sel > 0);
				this.buttonUp.SetEnabled(sel > 0);
				this.buttonDown.SetEnabled(sel < this.globalSettings.QuickCommands.Count-1);
				this.buttonLast.SetEnabled(sel < this.globalSettings.QuickCommands.Count-1);
			}
		}

		// Met à jour la liste des commandes rapides.
		protected void UpdateQuickList(int sel)
		{
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
							bt.DockMargins = new Margins(4, 0, 0, 0);
							bt.ActiveStateChanged += new EventHandler(this.HandleQuickUsedChanged);
							this.quickList[column, row].Insert(bt);
						}
						else if ( column == 1 )  // icône ?
						{
							ib = new IconButton();
							ib.Name = row.ToString();
							ib.Dock = DockStyle.Fill;
							ib.DockMargins = new Margins(0, 0, 0, 0);
							this.quickList[column, row].Insert(ib);
						}
						else if ( column == 2 )  // bouton pour le séparateur ?
						{
							ib = new IconButton();
							ib.Name = row.ToString();
							ib.Dock = DockStyle.Fill;
							ib.DockMargins = new Margins(1, 1, 1, 1);
							ib.Clicked += new MessageEventHandler(this.HandleQuickSeparatorClicked);
							this.quickList[column, row].Insert(ib);
						}
						else if ( column == 3 )  // texte de la commande ?
						{
							st = new StaticText();
							st.Alignment = ContentAlignment.MiddleLeft;
							st.Dock = DockStyle.Fill;
							st.DockMargins = new Margins(6, 0, 0, 0);
							this.quickList[column, row].Insert(st);
						}
					}
				}

				string xcmd = this.globalSettings.QuickCommands[row] as string;
				bool   used = GlobalSettings.QuickUsed(xcmd);
				bool   sep  = GlobalSettings.QuickSep(xcmd);
				string cmd  = GlobalSettings.QuickCmd(xcmd);

				// Bouton check pour déterminer la visibilité.
				bt = this.quickList[0, row].Children[0] as CheckButton;
				bt.ActiveState = used ? WidgetState.ActiveYes : WidgetState.ActiveNo;

				// Icône.
				ib = this.quickList[1, row].Children[0] as IconButton;
				ib.IconName = Misc.Icon(cmd);

				// Bouton pour le séparateur.
				ib = this.quickList[2, row].Children[0] as IconButton;
				ib.IconName = Misc.Icon(sep ? "QuickSeparatorYes" : "QuickSeparatorNo");
				ib.SetEnabled(used);

				// Texte de la commande.
				st = this.quickList[3, row].Children[0] as StaticText;
				st.Text = DocumentEditor.GetRes("Action."+cmd);

				this.quickList.SelectRow(row, row==sel);
			}

			if ( sel != -1 )
			{
				this.quickList.ShowSelect();  // montre la ligne sélectionnée
			}

			this.ignoreChange = false;
		}

		// Liste des commandes rapides cliquée.
		private void HandleQuickListSelectionChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			this.UpdateQuickButtons();
		}

		// Déplace une commande rapide.
		protected void MoveQuickCommand(int src, int dst)
		{
			if ( src == -1 )  return;

			string xcmd = this.globalSettings.QuickCommands[src] as string;
			this.globalSettings.QuickCommands.RemoveAt(src);
			this.globalSettings.QuickCommands.Insert(dst, xcmd);

			this.UpdateQuickList(dst);
			this.UpdateQuickButtons();
			this.editor.UpdateQuickCommands();
		}

		// Bouton "check" dans la liste des commandes rapides cliqué.
		private void HandleQuickUsedChanged(object sender)
		{
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

		// Bouton "séparateur" dans la liste des commandes rapides cliqué.
		private void HandleQuickSeparatorClicked(object sender, MessageEventArgs e)
		{
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

		private void HandleQuickButtonDefault(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;

			this.globalSettings.QuickCommands = GlobalSettings.DefaultQuickCommands();

			this.UpdateQuickList(sel);
			this.UpdateQuickButtons();
			this.editor.UpdateQuickCommands();
		}

		private void HandleQuickButtonClear(object sender, MessageEventArgs e)
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

		private void HandleQuickButtonFirst(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;
			this.MoveQuickCommand(sel, 0);
		}

		private void HandleQuickButtonUp(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;
			this.MoveQuickCommand(sel, sel-1);
		}

		private void HandleQuickButtonDown(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;
			this.MoveQuickCommand(sel, sel+1);
		}

		private void HandleQuickButtonLast(object sender, MessageEventArgs e)
		{
			int sel = this.quickList.SelectedRow;
			this.MoveQuickCommand(sel, this.globalSettings.QuickCommands.Count-1);
		}
		#endregion


		protected int							tabIndex;
		protected bool							ignoreChange = false;
		protected CellTable						quickList;
		protected IconButton					buttonDefault;
		protected IconButton					buttonClear;
		protected IconButton					buttonFirst;
		protected IconButton					buttonUp;
		protected IconButton					buttonDown;
		protected IconButton					buttonLast;
	}
}
