using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Dialogue permettant de choisir une ressource de type quelconque.
	/// </summary>
	public class ResourceSelector : Abstract
	{
		public ResourceSelector(MainWindow mainWindow) : base(mainWindow)
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
				this.window.PreventAutoClose = true;
				this.WindowInit("ResourceSelector", 400, 300, true);
				this.window.Text = Res.Strings.Dialog.TextSelector.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;

				Widget header = new Widget(this.window.Root);
				header.Margins = new Margins(0, 0, 0, 2);
				header.Dock = DockStyle.Top;

				this.header1 = new StaticText(header);
				this.header1.MinWidth = 100;
				this.header1.Dock = DockStyle.Left;

				this.header2 = new StaticText(header);
				this.header2.MinWidth = 100;
				this.header2.Dock = DockStyle.Left;

				header = new Widget(this.window.Root);
				header.Margins = new Margins(0, 0, 0, 4);
				header.Dock = DockStyle.Top;

				this.filterLabel = new TextField(header);
				this.filterLabel.MinWidth = 100;
				this.filterLabel.Name = "FilterLabel";
				this.filterLabel.TextChanged += new EventHandler(this.HandleFilterTextChanged);
				this.filterLabel.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFilterKeyboardFocusChanged);
				this.filterLabel.TabIndex = tabIndex++;
				this.filterLabel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.filterLabel.Dock = DockStyle.Left;
				this.filterLabel.Margins = new Margins(0, 1, 0, 0);

				this.filterText = new TextField(header);
				this.filterText.MinWidth = 100;
				this.filterText.Name = "FilterText";
				this.filterText.TextChanged += new EventHandler(this.HandleFilterTextChanged);
				this.filterText.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFilterKeyboardFocusChanged);
				this.filterText.TabIndex = tabIndex++;
				this.filterText.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.filterText.Dock = DockStyle.Left;
				this.filterText.Margins = new Margins(0, 1, 0, 0);

				this.buttonClear = new Button(header);
				this.buttonClear.Text = Res.Strings.Dialog.TextSelector.Button.Clear;
				this.buttonClear.PreferredSize = new Size(16, 20);
				this.buttonClear.TabIndex = tabIndex++;
				this.buttonClear.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.buttonClear.Dock = DockStyle.Fill;
				this.buttonClear.Clicked += new MessageEventHandler(this.HandleButtonClearClicked);

				//	Tableau principal.
				this.array = new MyWidgets.StringArray(this.window.Root);
				this.array.ColumnsWidthChanged += new EventHandler(this.HandleArrayColumnsWidthChanged);
				this.array.CellCountChanged += new EventHandler(this.HandleArrayCellCountChanged);
				this.array.CellsContentChanged += new EventHandler(this.HandleArrayCellsContentChanged);
				this.array.SelectedRowChanged += new EventHandler(this.HandleArraySelectedRowChanged);
				this.array.SelectedRowDoubleClicked += new EventHandler(this.HandleArraySelectedRowDoubleClicked);
				this.array.TabIndex = tabIndex++;
				this.array.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
				this.array.Dock = DockStyle.Fill;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonUse = new Button(footer);
				this.buttonUse.PreferredWidth = 75;
				this.buttonUse.Text = Res.Strings.Dialog.TextSelector.Button.Use;
				this.buttonUse.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonUse.Dock = DockStyle.Left;
				this.buttonUse.Margins = new Margins(0, 6, 0, 0);
				this.buttonUse.Clicked += new MessageEventHandler(this.HandleButtonUseClicked);
				this.buttonUse.TabIndex = tabIndex++;
				this.buttonUse.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonCreate = new Button(footer);
				this.buttonCreate.PreferredWidth = 75;
				this.buttonCreate.Text = Res.Strings.Dialog.TextSelector.Button.Create;
				this.buttonCreate.Dock = DockStyle.Left;
				this.buttonCreate.Margins = new Margins(0, 6, 0, 0);
				this.buttonCreate.Clicked += new MessageEventHandler(this.HandleButtonCreateClicked);
				this.buttonCreate.TabIndex = tabIndex++;
				this.buttonCreate.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = tabIndex++;
				this.buttonCancel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.ignoreChanged = true;
			this.filterLabel.Text = "";
			this.filterText.Text = "";
			this.ignoreChanged = false;

			this.UpdateResourceType();
			this.UpdateHeader();
			this.UpdateDruidsIndex();
			this.UpdateArray();
			this.SelectArray();

			string label = "";
			if (!this.resource.IsEmpty)
			{
				string text, icon;
				bool isDefined;
				this.access.GetBypassFilterStrings(this.resource, this.CurrentBundle, out label, out text, out icon, out isDefined);
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


		public void SetAccess(ResourceAccess access)
		{
			//	Détermine les ressources à afficher.
			this.access = access;
		}

		public Druid Resource
		{
			//	Druid de la ressource choisie.
			get
			{
				return this.resource;
			}
			set
			{
				System.Diagnostics.Debug.Assert(value.Type != DruidType.ModuleRelative);
				this.resource = value;
			}
		}


		protected void UpdateHeader()
		{
			//	Met à jour les textes fixes en haut.
			this.header1.Text = Res.Strings.Dialog.TextSelector.Label;

			string culture = Misc.CultureName(this.mainWindow.CurrentModule.ResourceManager.ActiveCulture);
			this.header2.Text = string.Format(Res.Strings.Dialog.TextSelector.Text, culture);
		}

		protected void UpdateColumnsWidth()
		{
			//	TODO: étrange problème lors du redimensionnement de la fenêtre...
			double w1 = this.array.GetColumnsAbsoluteWidth(0);
			double w2 = this.array.GetColumnsAbsoluteWidth(1);

			this.header1.PreferredWidth = w1;
			this.filterLabel.PreferredWidth = w1;

			this.header2.PreferredWidth = w2;
			this.filterText.PreferredWidth = w2;
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
			filterLabel = Searcher.RemoveAccent(filterLabel.ToLower());
			filterText  = Searcher.RemoveAccent(filterText.ToLower());

			ResourceBundle bundle = this.CurrentBundle;

			this.druidsIndex.Clear();
			int min = 100000;
			int best = 0;

			for (int i=0; i<this.access.TotalCount; i++)
			{
				Druid druid = this.access.GetBypassFilterDruid(i);
				string label, text, icon;
				bool isDefined;
				this.access.GetBypassFilterStrings(druid, bundle, out label, out text, out icon, out isDefined);

				bool add1 = false;
				bool add2 = false;

				if (filterLabel == "")
				{
					add1 = true;
				}
				else
				{
					int index = Searcher.IndexOf(label, filterLabel, 0, Searcher.SearchingMode.None);
					if (index != -1)
					{
						add1 = true;

						int len = label.Length - filterLabel.Length;
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
					int index = Searcher.IndexOf(text, filterText, 0, Searcher.SearchingMode.None);
					if (index != -1)
					{
						add2 = true;

						int len = text.Length - filterLabel.Length;
						if (min > len)
						{
							min = len;
							best = this.druidsIndex.Count;
						}
					}
				}

				if (add1 && add2)
				{
					this.druidsIndex.Add(druid);
				}
			}

			return best;
		}

		protected void UpdateResourceType()
		{
			//	Met à jour le tableau en fonction du type des ressources.
			int columns = 2;  // 2 colonnes pour les Strings
			if (this.access.ResourceType != ResourceAccess.Type.Strings)
			{
				columns = 3;  // 3 colonnes pour les Captions, COmmands et Types
			}

			if (this.array.Columns != columns)  // changement du nombre de colonnes ?
			{
				this.array.Columns = columns;

				if (columns == 2)
				{
					this.array.SetColumnsRelativeWidth(0, 0.5);
					this.array.SetColumnsRelativeWidth(1, 0.5);

					this.array.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
					this.array.SetColumnAlignment(1, ContentAlignment.MiddleLeft);

					this.array.SetDynamicsToolTips(0, true);
					this.array.SetDynamicsToolTips(1, false);

					this.array.LineHeight = 20;  // hauteur standard
				}
				else
				{
					this.array.SetColumnsRelativeWidth(0, 0.4);
					this.array.SetColumnsRelativeWidth(1, 0.5);
					this.array.SetColumnsRelativeWidth(2, 0.1);

					this.array.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
					this.array.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
					this.array.SetColumnAlignment(2, ContentAlignment.MiddleCenter);

					this.array.SetDynamicsToolTips(0, true);
					this.array.SetDynamicsToolTips(1, false);
					this.array.SetDynamicsToolTips(2, false);

					this.array.LineHeight = 30;  // plus haut, à cause des descriptions et des icônes
				}
			}
		}

		protected void UpdateArray()
		{
			//	Met à jour tout le contenu du tableau.
			this.array.TotalRows = this.druidsIndex.Count;

			ResourceBundle bundle = this.CurrentBundle;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.druidsIndex.Count)
				{
					string label, text, icon;
					bool isDefined;
					this.access.GetBypassFilterStrings(this.druidsIndex[first+i], bundle, out label, out text, out icon, out isDefined);

					this.array.SetLineString(0, first+i, label);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(1, first+i, text);
					this.array.SetLineState(1, first+i, isDefined ? MyWidgets.StringList.CellState.Normal : MyWidgets.StringList.CellState.Warning);

					if (this.access.ResourceType != ResourceAccess.Type.Strings)
					{
						if (string.IsNullOrEmpty(icon))
						{
							this.array.SetLineString(2, first+i, "");
						}
						else
						{
							this.array.SetLineString(2, first+i, Misc.ImageFull(icon));
						}
						this.array.SetLineState(2, first+i, isDefined ? MyWidgets.StringList.CellState.Normal : MyWidgets.StringList.CellState.Warning);
					}
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);

					this.array.SetLineString(1, first+i, "");
					this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);

					if (this.access.ResourceType != ResourceAccess.Type.Strings)
					{
						this.array.SetLineString(2, first+i, "");
						this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);
					}
				}
			}
		}

		protected void SelectArray()
		{
			//	Sélectionne la bonne ressource dans le tableau.
			int sel = -1;

			if (this.resource.Type == DruidType.Full)
			{
				for (int i=0; i<this.druidsIndex.Count; i++)
				{
					if (this.druidsIndex[i] == this.resource)
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
					if (!this.access.IsExistingName(label))
					{
						createEnable = true;
					}
				}
			}
			this.buttonCreate.Enable = createEnable;
		}

		protected ResourceBundle CurrentBundle
		{
			//	Retourne le bundle de la culture active, correspondant au choix fait dans
			//	l'onglet 'Cultures'.
			get
			{
				string culture = Misc.CultureBaseName(this.mainWindow.CurrentModule.ResourceManager.ActiveCulture);
				return this.access.GetCultureBundle(culture);
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
				this.resource = Druid.Empty;
			}
			else
			{
				this.resource = this.druidsIndex[sel];
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
					if (!this.access.IsExistingName(label))
					{
						this.resource = this.access.CreateBypassFilter(this.CurrentBundle, label, text);
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
			this.UpdateColumnsWidth();
		}

		void HandleArrayCellCountChanged(object sender)
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
				this.resource = Druid.Empty;
			}
			else
			{
				this.resource = this.druidsIndex[sel];
			}
		}


		protected ResourceAccess				access;
		protected StaticText					header1;
		protected StaticText					header2;
		protected TextField						filterLabel;
		protected TextField						filterText;
		protected Button						buttonClear;
		protected MyWidgets.StringArray			array;
		protected Button						buttonUse;
		protected Button						buttonCreate;
		protected Button						buttonCancel;

		protected Druid							resource;
		protected List<Druid>					druidsIndex;
		protected Widget						focusedWidget;
	}
}
