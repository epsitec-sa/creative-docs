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
			this.druidsIndex = new List<Druid>();
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

				this.header1 = new StaticText(this.window.Root);
				this.header1.PreferredWidth = 185;
				this.header1.Anchor = AnchorStyles.TopLeft;
				this.header1.Margins = new Margins(6, 0, 6, 0);

				this.header2 = new StaticText(this.window.Root);
				this.header2.PreferredWidth = 185;
				this.header2.Anchor = AnchorStyles.TopLeft;
				this.header2.Margins = new Margins(192, 0, 6, 0);

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
				this.array.SelectedRowDoubleClicked += new EventHandler(this.HandleArraySelectedRowDoubleClicked);
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

			this.UpdateHeader();
			this.UpdateDruidsIndex();
			this.UpdateArray();
			this.SelectArray();

			string label = "";
			if (!this.ressource.IsEmpty)
			{
				ResourceBundle.Field field = this.primaryBundle[this.ressource];
				if (!field.IsEmpty)
				{
					label = field.Name;
				}
			}

			this.ignoreChanged = true;
			this.filterLabel.Text = label;
			this.filterText.Text = "";
			this.ignoreChanged = false;

			this.UpdateButtons();

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


		public Druid Ressource
		{
			get
			{
				return this.ressource;
			}
			set
			{
				System.Diagnostics.Debug.Assert(value.Type != DruidType.ModuleRelative);
				this.ressource = value;
			}
		}


		protected void UpdateHeader()
		{
			//	Met à jour les textes fixes en haut.
			this.header1.Text = Res.Strings.Dialog.TextSelector.Label;

			string culture = Misc.CultureName(this.mainWindow.CurrentModule.ResourceManager.ActiveCulture);
			this.header2.Text = string.Format(Res.Strings.Dialog.TextSelector.Text, culture);
		}

		protected int UpdateDruidsIndex()
		{
			string filterLabel = TextLayout.ConvertToSimpleText(this.filterLabel.Text);
			string filterText  = TextLayout.ConvertToSimpleText(this.filterText.Text);
			return this.UpdateDruidsIndex(filterLabel, filterText);
		}

		protected int UpdateDruidsIndex(string filterLabel, string filterText)
		{
			//	Construit l'index en fonction des ressources.
			//	Retourne le rang de la ressource correspondant le mieux possible aux filtres.
			ResourceBundleCollection bundles = this.mainWindow.CurrentModule.Bundles;
			this.primaryBundle = bundles[ResourceLevel.Merged];

			this.druidsIndex.Clear();

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
							best = this.druidsIndex.Count;
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
							best = this.druidsIndex.Count;
						}
					}
				}

				if (add1 && add2)
				{
					Druid fullDruid = new Druid(field.Druid, this.primaryBundle.Module.Id);
					this.druidsIndex.Add(fullDruid);
				}
			}

			return best;
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.druidsIndex.Count;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.druidsIndex.Count)
				{
					ResourceBundle.Field field = this.primaryBundle[this.druidsIndex[first+i]];

					this.array.SetLineString(0, first+i, field.Name);
					this.array.SetLineString(1, first+i, field.AsString);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);
					this.array.SetLineState(1, first+i, (field.DataLevel == ResourceLevel.Default) ? MyWidgets.StringList.CellState.Warning : MyWidgets.StringList.CellState.Normal);
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

			if (this.ressource.Type == DruidType.Full)
			{
				for (int i=0; i<this.druidsIndex.Count; i++)
				{
					if (this.druidsIndex[i] == this.ressource)
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
			string label = TextLayout.ConvertToSimpleText(this.filterLabel.Text);
			string text  = TextLayout.ConvertToSimpleText(this.filterText.Text);

			this.buttonClear.Enable = (label != "" || text != "");
			this.buttonUse.Enable = (this.array.SelectedRow != -1);

			bool createEnable = false;
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
				this.ressource = Druid.Empty;
			}
			else
			{
				this.ressource = this.druidsIndex[sel];
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
						Druid druid = this.mainWindow.CurrentModule.Modifier.Create(label, text);
						this.ressource = druid;
					}
				}
			}
		}


		void HandleFilterTextChanged(object sender)
		{
			//	Le texte d'un filtre a changé.
			if (this.ignoreChanged)  return;

			int best = this.UpdateDruidsIndex();
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

		void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.UpdateButtons();

			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			int sel = this.array.SelectedRow;
			if (sel == -1)
			{
				this.ressource = Druid.Empty;
			}
			else
			{
				this.ressource = this.druidsIndex[sel];
			}
		}


		protected StaticText					header1;
		protected StaticText					header2;
		protected TextField						filterLabel;
		protected TextField						filterText;
		protected Button						buttonClear;
		protected MyWidgets.StringArray			array;
		protected Button						buttonUse;
		protected Button						buttonCreate;
		protected Button						buttonCancel;

		protected Druid							ressource;
		protected ResourceBundle				primaryBundle;
		protected List<Druid>					druidsIndex;
		protected Widget						focusedWidget;
	}
}
