using System.Collections.Generic;
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
			this.labelsIndex = new List<string>();
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
				this.filterLabel.TextChanged += new EventHandler(this.HandleFilterTextChanged);
				this.filterLabel.TabIndex = tabIndex++;
				this.filterLabel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.filterLabel.Anchor = AnchorStyles.TopLeft;
				this.filterLabel.Margins = new Margins(6, 0, 6, 0);

				this.filterText = new TextFieldCombo(this.window.Root);
				this.filterText.PreferredWidth = 185;
				this.filterText.Name = "FilterText";
				this.filterText.TextChanged += new EventHandler(this.HandleFilterTextChanged);
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
				this.array.CellsQuantityChanged += new EventHandler(this.HandleArrayCellsQuantityChanged);
				this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
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

			this.UpdateLabelsIndex();
			this.UpdateArray();
			this.SelectArray();

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


		protected void UpdateLabelsIndex()
		{
			string filterLabel = TextLayout.ConvertToSimpleText(this.filterLabel.Text);
			string filterText  = TextLayout.ConvertToSimpleText(this.filterText.Text);
			this.UpdateLabelsIndex(filterLabel, filterText);
		}

		protected void UpdateLabelsIndex(string filterLabel, string filterText)
		{
			//	Construit l'index en fonction des ressources.
			ResourceBundleCollection bundles = this.mainWindow.CurrentModule.Bundles;
			this.primaryBundle = bundles[ResourceLevel.Default];

			this.labelsIndex.Clear();

			filterLabel = Searcher.RemoveAccent(filterLabel.ToLower());
			filterText  = Searcher.RemoveAccent(filterText.ToLower());

			foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
			{
				if (filterLabel != "")
				{
					int index = Searcher.IndexOf(field.Name, filterLabel, 0, Searcher.SearchingMode.None);
					if (index == -1)  continue;
				}

				this.labelsIndex.Add(field.Name);
			}
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.labelsIndex.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.labelsIndex.Count)
				{
					ResourceBundle.Field primaryField = this.primaryBundle[this.labelsIndex[first+i]];

					this.array.SetLineString(0, first+i, primaryField.Name);
					this.array.SetLineString(1, first+i, primaryField.AsString);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Normal);
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineString(1, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);
				}
			}
		}

		protected void SelectArray()
		{
			//	Sélectionne la bonne ressource dans le tableau.
			int sel = -1;

			if (this.ressource != "")
			{
				for (int i=0; i<this.labelsIndex.Count; i++)
				{
					if (this.labelsIndex[i] == this.ressource)
					{
						sel = i;
						break;
					}
				}
			}

			this.array.SelectedRow = sel;
			this.array.ShowSelectedRow();
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

			Misc.ComboMenuAdd(this.filterLabel);
			Misc.ComboMenuAdd(this.filterText);

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				this.ressource = "";
			}
			else
			{
				this.ressource = this.labelsIndex[sel];
			}
		}

		void HandleFilterTextChanged(object sender)
		{
			//	Le texte d'un filtre a changé.
			this.UpdateLabelsIndex();
			this.UpdateArray();
		}

		void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a changé.
			//?this.UpdateClientGeometry();
		}

		void HandleArrayCellsQuantityChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
		}


		protected TextFieldCombo				filterLabel;
		protected TextFieldCombo				filterText;
		protected MyWidgets.StringArray			array;
		protected string						ressource;
		protected ResourceBundle				primaryBundle;
		protected List<string>					labelsIndex;
	}
}
