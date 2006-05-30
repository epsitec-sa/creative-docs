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

				StaticText fix1 = new StaticText(this.window.Root);
				fix1.PreferredWidth = 185;
				fix1.Text = Res.Strings.Dialog.TextSelector.Label;
				fix1.Anchor = AnchorStyles.TopLeft;
				fix1.Margins = new Margins(6, 0, 6, 0);

				StaticText fix2 = new StaticText(this.window.Root);
				fix2.PreferredWidth = 185;
				fix2.Text = Res.Strings.Dialog.TextSelector.Text;
				fix2.Anchor = AnchorStyles.TopLeft;
				fix2.Margins = new Margins(192, 0, 6, 0);

				this.filterLabel = new TextField(this.window.Root);
				this.filterLabel.PreferredWidth = 185;
				this.filterLabel.Name = "FilterLabel";
				this.filterLabel.TextChanged += new EventHandler(this.HandleFilterTextChanged);
				this.filterLabel.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFilterKeyboardFocusChanged);
				this.filterLabel.TabIndex = tabIndex++;
				this.filterLabel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.filterLabel.Anchor = AnchorStyles.TopLeft;
				this.filterLabel.Margins = new Margins(6, 0, 6+18, 0);

				this.filterText = new TextField(this.window.Root);
				this.filterText.PreferredWidth = 181;
				this.filterText.Name = "FilterText";
				this.filterText.TextChanged += new EventHandler(this.HandleFilterTextChanged);
				this.filterText.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFilterKeyboardFocusChanged);
				this.filterText.TabIndex = tabIndex++;
				this.filterText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.filterText.Anchor = AnchorStyles.TopLeft;
				this.filterText.Margins = new Margins(192, 0, 6+18, 0);

				this.buttonClear = new Button(this.window.Root);
				this.buttonClear.Text = Res.Strings.Dialog.TextSelector.Button.Clear;
				this.buttonClear.PreferredSize = new Size(20, 20);
				this.buttonClear.TabIndex = tabIndex++;
				this.buttonClear.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.buttonClear.Anchor = AnchorStyles.TopRight;
				this.buttonClear.Margins = new Margins(0, 6, 6+18, 0);
				this.buttonClear.Clicked += new MessageEventHandler(this.HandleButtonClearClicked);

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
				this.array.Margins = new Margins(6, 6, 6+18+24, 34);

				//	Boutons de fermeture.
				this.buttonUse = new Button(this.window.Root);
				this.buttonUse.PreferredWidth = 75;
				this.buttonUse.Text = Res.Strings.Dialog.TextSelector.Button.Use;
				this.buttonUse.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonUse.Anchor = AnchorStyles.BottomLeft;
				this.buttonUse.Margins = new Margins(6, 0, 0, 6);
				this.buttonUse.Clicked += new MessageEventHandler(this.HandleButtonUseClicked);
				this.buttonUse.TabIndex = tabIndex++;
				this.buttonUse.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonCreate = new Button(this.window.Root);
				this.buttonCreate.PreferredWidth = 75;
				this.buttonCreate.Text = Res.Strings.Dialog.TextSelector.Button.Create;
				this.buttonCreate.Anchor = AnchorStyles.BottomLeft;
				this.buttonCreate.Margins = new Margins(6+75+10, 0, 0, 6);
				this.buttonCreate.Clicked += new MessageEventHandler(this.HandleButtonCreateClicked);
				this.buttonCreate.TabIndex = tabIndex++;
				this.buttonCreate.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonCancel = new Button(this.window.Root);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.Anchor = AnchorStyles.BottomRight;
				this.buttonCancel.Margins = new Margins(0, 6, 0, 6);
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = tabIndex++;
				this.buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.ignoreChanged = true;
			this.filterLabel.Text = "";
			this.filterText.Text = "";
			this.ignoreChanged = false;

			this.UpdateLabelsIndex();
			this.UpdateArray();
			this.SelectArray();
			this.UpdateButtons();

			this.ignoreChanged = true;
			this.filterLabel.Text = this.ressource;
			this.filterText.Text = "";
			this.ignoreChanged = false;

			Widget widget = this.filterLabel;
			if (this.focusedWidget != null)
			{
				widget = this.focusedWidget;
			}

			widget.Focus();

			if (widget is AbstractTextField)
			{
				AbstractTextField text = widget as AbstractTextField;
				text.SelectAll();
			}

			this.window.ShowDialog();
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


		protected int UpdateLabelsIndex()
		{
			string filterLabel = TextLayout.ConvertToSimpleText(this.filterLabel.Text);
			string filterText  = TextLayout.ConvertToSimpleText(this.filterText.Text);
			return this.UpdateLabelsIndex(filterLabel, filterText);
		}

		protected int UpdateLabelsIndex(string filterLabel, string filterText)
		{
			//	Construit l'index en fonction des ressources.
			//	Retourne le rang de la ressource correspondant le mieux possible aux filtres.
			ResourceBundleCollection bundles = this.mainWindow.CurrentModule.Bundles;
			this.primaryBundle = bundles[ResourceLevel.Default];

			this.labelsIndex.Clear();

			filterLabel = Searcher.RemoveAccent(filterLabel.ToLower());
			filterText  = Searcher.RemoveAccent(filterText.ToLower());

			int min = 100000;
			int best = 0;
			foreach (ResourceBundle.Field field in this.primaryBundle.Fields)
			{
				bool add1 = false;
				bool add2 = false;

				if (filterLabel == "")
				{
					add1 = true;
				}
				else
				{
					int index = Searcher.IndexOf(field.Name, filterLabel, 0, Searcher.SearchingMode.None);
					if (index != -1)
					{
						add1 = true;

						int len = field.Name.Length - filterLabel.Length;
						if (min > len)
						{
							min = len;
							best = this.labelsIndex.Count;
						}
					}
				}

				if (filterText == "")
				{
					add2 = true;
				}
				else
				{
					int index = Searcher.IndexOf(field.AsString, filterText, 0, Searcher.SearchingMode.None);
					if (index != -1)
					{
						add2 = true;

						int len = field.AsString.Length - filterLabel.Length;
						if (min > len)
						{
							min = len;
							best = this.labelsIndex.Count;
						}
					}
				}

				if (add1 && add2)
				{
					this.labelsIndex.Add(field.Name);
				}
			}

			return best;
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

		protected void UpdateButtons()
		{
			this.buttonClear.Enable = (this.filterLabel.Text != "" || this.filterText.Text != "");
			this.buttonUse.Enable = (this.array.SelectedRow != -1);

			bool createEnable = false;
			string label = TextLayout.ConvertToSimpleText(this.filterLabel.Text);
			string text  = TextLayout.ConvertToSimpleText(this.filterText.Text);
			if (label != "" && text != "")
			{
				if (Misc.IsValidLabel(ref label))
				{
					if (!this.IsExistingName(label))
					{
						createEnable = true;
					}
				}
			}
			this.buttonCreate.Enable = createEnable;
		}

		protected bool IsExistingName(string baseName)
		{
			//	Indique si un nom existe.
			ResourceBundleCollection bundles = this.mainWindow.CurrentModule.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];

			ResourceBundle.Field field = defaultBundle[baseName];
			return (field != null && field.Name != null);
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

		private void HandleButtonClearClicked(object sender, MessageEventArgs e)
		{
			this.filterLabel.Text = "";
			this.filterText.Text = "";
		}

		private void HandleButtonUseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

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

		private void HandleButtonCreateClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			string label = TextLayout.ConvertToSimpleText(this.filterLabel.Text);
			string text  = TextLayout.ConvertToSimpleText(this.filterText.Text);
			if (label != "" && text != "")
			{
				if (Misc.IsValidLabel(ref label))
				{
					if (!this.IsExistingName(label))
					{
						this.mainWindow.CurrentModule.Modifier.Create(label, text);
						this.ressource = label;
					}
				}
			}
		}


		void HandleFilterTextChanged(object sender)
		{
			//	Le texte d'un filtre a changé.
			if (this.ignoreChanged)  return;

			int best = this.UpdateLabelsIndex();
			this.UpdateArray();
			this.UpdateButtons();

			this.array.SelectedRow = best;
			this.array.ShowSelectedRow();
		}

		protected void HandleFilterKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsqu'une ligne éditable voit son focus changer.
			bool focused = (bool) e.NewValue;

			if (focused)
			{
				this.focusedWidget = sender as AbstractTextField;
			}
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
			this.UpdateButtons();
		}


		protected TextField						filterLabel;
		protected TextField						filterText;
		protected Button						buttonClear;
		protected MyWidgets.StringArray			array;
		protected Button						buttonUse;
		protected Button						buttonCreate;
		protected Button						buttonCancel;

		protected string						ressource;
		protected ResourceBundle				primaryBundle;
		protected List<string>					labelsIndex;
		protected Widget						focusedWidget;
	}
}
