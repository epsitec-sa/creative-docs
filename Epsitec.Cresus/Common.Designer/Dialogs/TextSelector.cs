using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir une ressource de type texte.
	/// </summary>
	public class TextSelector : Abstract
	{
		public TextSelector(MainWindow mainWindow) : base(mainWindow)
		{
		}

		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.MakeFixedSizeWindow();
				this.window.Root.WindowStyles = WindowStyles.None;
				this.window.PreventAutoClose = true;
				this.WindowInit("TextSelector", 400, 300, true);
				this.window.Text = Res.Strings.Dialog.TextSelector.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);

				int tabIndex = 0;

				this.filterLabel = new TextFieldCombo(this.window.Root);
				this.filterLabel.PreferredWidth = 185;
				this.filterLabel.Name = "FilterLabel";
				this.filterLabel.TextChanged += new EventHandler(this.HandleFilterLabelTextChanged);
				this.filterLabel.TabIndex = tabIndex++;
				this.filterLabel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.filterLabel.Anchor = AnchorStyles.TopLeft;
				this.filterLabel.Margins = new Margins(6, 0, 6, 0);

				this.filterText = new TextFieldCombo(this.window.Root);
				this.filterText.PreferredWidth = 185;
				this.filterText.Name = "FilterText";
				this.filterText.TextChanged += new EventHandler(this.HandleFilterTextTextChanged);
				this.filterText.TabIndex = tabIndex++;
				this.filterText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.filterText.Anchor = AnchorStyles.TopLeft;
				this.filterText.Margins = new Margins(192, 0, 6, 0);

				this.array = new MyWidgets.StringArray(this.window.Root);
				this.array.Columns = 2;
				this.array.SetColumnsRelativeWidth(0, 0.5);
				this.array.SetColumnsRelativeWidth(1, 0.5);
				this.array.SetDynamicsToolTips(0, true);
				this.array.SetDynamicsToolTips(1, false);
				this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
				this.array.TabIndex = tabIndex++;
				this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.array.Anchor = AnchorStyles.All;
				this.array.Margins = new Margins(6, 6, 30, 34);

				//	Boutons de fermeture.
				Button buttonOk = new Button(this.window.Root);
				buttonOk.PreferredWidth = 75;
				buttonOk.Text = Res.Strings.Dialog.Button.OK;
				buttonOk.ButtonStyle = ButtonStyle.DefaultAccept;
				buttonOk.Anchor = AnchorStyles.BottomLeft;
				buttonOk.Margins = new Margins(6, 0, 0, 6);
				buttonOk.Clicked += new MessageEventHandler(this.HandleButtonOkClicked);
				buttonOk.TabIndex = tabIndex++;
				buttonOk.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				Button buttonClose = new Button(this.window.Root);
				buttonClose.PreferredWidth = 75;
				buttonClose.Text = Res.Strings.Dialog.Button.Cancel;
				buttonClose.Anchor = AnchorStyles.BottomLeft;
				buttonClose.Margins = new Margins(6+75+10, 0, 0, 6);
				buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				buttonClose.TabIndex = tabIndex++;
				buttonClose.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.window.ShowDialog();

			this.filterLabel.Focus();
			this.filterLabel.SelectAll();
		}


		public string Ressource
		{
			get
			{
				return this.ressource;
			}
			set
			{
				this.ressource = value;
			}
		}


		private void HandleWindowCloseClicked(object sender)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		void HandleFilterLabelTextChanged(object sender)
		{
			//	Le texte du filtre du label a changé.
		}

		void HandleFilterTextTextChanged(object sender)
		{
			//	Le texte du filtre du label a changé.
		}

		void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a changé.
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
		}


		protected TextFieldCombo				filterLabel;
		protected TextFieldCombo				filterText;
		protected MyWidgets.StringArray			array;
		protected string						ressource;
	}
}
