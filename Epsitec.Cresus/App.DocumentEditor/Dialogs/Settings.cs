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
				this.window.Text = "Réglages";
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
				radio1.Text = "Global";
				radio1.Width = 80;
				radio1.Dock = DockStyle.Left;
				radio1.Clicked += new MessageEventHandler(this.HandleRadioSettingsChanged);

				RadioButton radio2 = new RadioButton(topPart);
				radio2.Name = "RadioDocument";
				radio2.Text = "Document";
				radio2.Width = 80;
				radio2.Dock = DockStyle.Left;
				radio2.ActiveState = WidgetState.ActiveYes;
				radio2.Clicked += new MessageEventHandler(this.HandleRadioSettingsChanged);

				// Crée le panneau "global".
				Panel global = new Panel(this.window.Root);
				global.Name = "BookGlobal";
				global.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				global.AnchorMargins = new Margins(6, 6, 6+20, 34);
				global.SetVisible(false);

				TextFieldCombo combo;
				TextFieldSlider field;
				CheckButton check;
				this.tabIndex = 0;

				Common.Document.Dialogs.CreateTitle(global, "Que faire au démarrage du logiciel ?");

				combo = this.CreateCombo(global, "FirstAction", "Action");
				for ( int i=0 ; i<GlobalSettings.FirstActionCount ; i++ )
				{
					Common.Document.Settings.FirstAction action = GlobalSettings.FirstActionType(i);
					combo.Items.Add(GlobalSettings.FirstActionString(action));
				}
				combo.SelectedIndex = GlobalSettings.FirstActionRank(this.globalSettings.FirstAction);

				check = this.CreateCheck(global, "SplashScreen", "Ecran initial");
				check.ActiveState = this.globalSettings.SplashScreen ? WidgetState.ActiveYes : WidgetState.ActiveNo;

				Common.Document.Dialogs.CreateTitle(global, "Réglages de la souris");

				combo = this.CreateCombo(global, "MouseWheelAction", "Molette");
				for ( int i=0 ; i<GlobalSettings.MouseWheelActionCount ; i++ )
				{
					Common.Document.Settings.MouseWheelAction action = GlobalSettings.MouseWheelActionType(i);
					combo.Items.Add(GlobalSettings.MouseWheelActionString(action));
				}
				combo.SelectedIndex = GlobalSettings.MouseWheelActionRank(this.globalSettings.MouseWheelAction);

				field = this.CreateField(global, "DefaultZoom", "Grossissement");
				field.MinValue = 1.1M;
				field.MaxValue = 4.0M;
				field.Step = 0.1M;
				field.Resolution = 0.1M;
				field.Value = (decimal) this.globalSettings.DefaultZoom;

				check = this.CreateCheck(global, "FineCursor", "Curseur précis");
				check.ActiveState = this.globalSettings.FineCursor ? WidgetState.ActiveYes : WidgetState.ActiveNo;

				Common.Document.Dialogs.CreateTitle(global, "Réglages de l'écran");

				combo = this.CreateCombo(global, "Adorner", "Aspect de l'interface");
				string[] list = Widgets.Adorner.Factory.AdornerNames;
				int rank = 0;
				foreach ( string name in list )
				{
#if !DEBUG
					if ( name == "LookAquaMetal" )  continue;
					if ( name == "LookAquaDyna" )  continue;
					if ( name == "LookXP" )  continue;
#endif
					combo.Items.Add(name);
					if ( name == Widgets.Adorner.Factory.ActiveName )
					{
						combo.SelectedIndex = rank;
					}
					rank++;
				}

				field = this.CreateField(global, "ScreenDpi", "Résolution (dpi)");
				field.MinValue = 30.0M;
				field.MaxValue = 300.0M;
				field.Step = 1.0M;
				field.Resolution = 1.0M;
				field.Value = (decimal) this.globalSettings.ScreenDpi;

				// Crée les onglets "document".
				TabBook bookDoc = new TabBook(this.window.Root);
				bookDoc.Name = "BookDocument";
				bookDoc.Arrows = TabBookArrows.Stretch;
				bookDoc.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
				bookDoc.AnchorMargins = new Margins(6, 6, 6+20, 34);

				TabPage bookFormat = new TabPage();
				bookFormat.Name = "Format";
				bookFormat.TabTitle = "Format";
				bookDoc.Items.Add(bookFormat);

				TabPage bookGrid = new TabPage();
				bookGrid.Name = "Grid";
				bookGrid.TabTitle = "Grille";
				bookDoc.Items.Add(bookGrid);

				TabPage bookGuides = new TabPage();
				bookGuides.Name = "Guides";
				bookGuides.TabTitle = "Repères";
				bookDoc.Items.Add(bookGuides);

				TabPage bookMove = new TabPage();
				bookMove.Name = "Move";
				bookMove.TabTitle = "Déplacements";
				bookDoc.Items.Add(bookMove);

				TabPage bookMisc = new TabPage();
				bookMisc.Name = "Misc";
				bookMisc.TabTitle = "Divers";
				bookDoc.Items.Add(bookMisc);

				bookDoc.ActivePage = bookFormat;

				// Bouton de fermeture.
				Button buttonClose = new Button(this.window.Root);
				buttonClose.Width = 75;
				buttonClose.Text = "Fermer";
				buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.AnchorMargins = new Margins(6, 0, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleSettingsButtonCloseClicked);
				buttonClose.TabIndex = 1000;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				ToolTip.Default.SetToolTip(buttonClose, "Fermer les réglages");
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

		private void HandleRadioSettingsChanged(object sender, MessageEventArgs e)
		{
			RadioButton radio = sender as RadioButton;
			if ( radio.Name == "RadioGlobal" )
			{
				this.window.Root.FindChild("BookGlobal").SetVisible(true);
				this.window.Root.FindChild("BookDocument").SetVisible(false);
			}
			else
			{
				this.window.Root.FindChild("BookGlobal").SetVisible(false);
				this.window.Root.FindChild("BookDocument").SetVisible(true);
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
				Widgets.Adorner.Factory.SetActive(combo.Text);
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


		protected int				tabIndex;
	}
}
