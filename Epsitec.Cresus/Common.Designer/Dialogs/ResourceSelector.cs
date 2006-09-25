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
				this.window.Text = Res.Strings.Dialog.ResourceSelector.Title;
				this.window.Owner = this.parentWindow;
				this.window.WindowCloseClicked += new EventHandler(this.HandleWindowCloseClicked);
				this.window.Root.Padding = new Margins(8, 8, 8, 8);

				ResizeKnob resize = new ResizeKnob(this.window.Root);
				resize.Anchor = AnchorStyles.BottomRight;
				resize.Margins = new Margins(0, -8, 0, -8);
				ToolTip.Default.SetToolTip(resize, Res.Strings.Dialog.Tooltip.Resize);

				int tabIndex = 0;
				Widget header;

				//	Titre supérieur.
				header = new Widget(this.window.Root);
				header.Margins = new Margins(0, 0, 0, 8);
				header.PreferredHeight = 26;
				header.Dock = DockStyle.Top;

				this.title = new StaticText(header);
				this.title.PreferredWidth = 180;
				this.title.Dock = DockStyle.Left;

				this.fieldModule = new TextFieldCombo(header);
				this.fieldModule.IsReadOnly = true;
				this.fieldModule.Dock = DockStyle.Fill;
				this.fieldModule.Margins = new Margins(0, 0, 5, 1);
				this.fieldModule.ComboClosed += new EventHandler(this.HandleFieldModuleComboClosed);

				//	Trait horizontal de séparation.
				Separator sep = new Separator(this.window.Root);
				sep.PreferredHeight = 1;
				sep.Dock = DockStyle.Top;

				//	En-tête avec les textes fixes.
				header = new Widget(this.window.Root);
				header.Margins = new Margins(0, 0, 0, 2);
				header.Dock = DockStyle.Top;

				this.header1 = new StaticText(header);
				this.header1.MinWidth = 100;
				this.header1.Dock = DockStyle.Left;

				this.header2 = new StaticText(header);
				this.header2.MinWidth = 100;
				this.header2.Dock = DockStyle.Left;

				//	En-tête avec les lignes éditables.
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
				this.buttonClear.Text = Res.Strings.Dialog.ResourceSelector.Button.Clear;
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
				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
				this.buttonUse.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonUse.Dock = DockStyle.Left;
				this.buttonUse.Margins = new Margins(0, 6, 0, 0);
				this.buttonUse.Clicked += new MessageEventHandler(this.HandleButtonUseClicked);
				this.buttonUse.TabIndex = tabIndex++;
				this.buttonUse.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonCreate = new Button(footer);
				this.buttonCreate.PreferredWidth = 75;
				this.buttonCreate.Text = Res.Strings.Dialog.ResourceSelector.Button.Create;
				this.buttonCreate.Dock = DockStyle.Left;
				this.buttonCreate.Margins = new Margins(0, 6, 0, 0);
				this.buttonCreate.Clicked += new MessageEventHandler(this.HandleButtonCreateClicked);
				this.buttonCreate.TabIndex = tabIndex++;
				this.buttonCreate.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
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
			this.UpdateTitle();
			this.UpdateHeader();
			this.UpdateDruidsIndex();
			this.UpdateArray();
			this.SelectArray();

			string label = "";
			if (!this.resource.IsEmpty)
			{
				string text;
				bool isDefined;
				this.access.GetBypassFilterStrings(this.resource, this.CurrentBundle, out label, out text, out isDefined);
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


		public void SetAccess(Module module, ResourceAccess.Type type, Druid ressource)
		{
			//	Détermine les ressources à afficher.
			Module m = this.ModuleSearch(ressource);
			if (m != null)
			{
				module = m;
			}

			this.module = module;
			this.resourceType = type;

			System.Diagnostics.Debug.Assert(ressource.Type != DruidType.ModuleRelative);
			this.resource = ressource;

			this.access = this.module.PrepareAccess(this.resourceType);
		}

		public Module Module
		{
			//	Retourne le module utilisé.
			get
			{
				return this.module;
			}
		}

		public Druid Resource
		{
			//	Retourne le Druid de la ressource choisie.
			get
			{
				return this.resource;
			}
		}


		protected void UpdateTitle()
		{
			//	Met à jour le titre qui dépend du type des ressources éditées.
			this.title.Text = string.Concat("<font size=\"200%\"><b>", ResourceAccess.TypeDisplayName(this.access.ResourceType), "</b></font>");

			this.fieldModule.Items.Clear();

			List<MainWindow.ModuleInfo> list = this.mainWindow.OpeningListModule;
			foreach (MainWindow.ModuleInfo info in list)
			{
				this.fieldModule.Items.Add(info.Module.ModuleInfo.Name);
			}

			this.fieldModule.Text = this.module.ModuleInfo.Name;
		}

		protected void UpdateHeader()
		{
			//	Met à jour les textes fixes en haut.
			this.header1.Text = Res.Strings.Dialog.ResourceSelector.Label;

			string culture = Misc.CultureName(this.mainWindow.CurrentModule.ResourceManager.ActiveCulture);
			this.header2.Text = string.Format(Res.Strings.Dialog.ResourceSelector.Text, culture);
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

			System.Diagnostics.Debug.WriteLine (string.Format ("Widths : {0}, {1}", w1, w2));
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
				string label, text;
				bool isDefined;
				this.access.GetBypassFilterStrings(druid, bundle, out label, out text, out isDefined);

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

					this.array.SetDynamicToolTips(0, true);
					this.array.SetDynamicToolTips(1, false);

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

					this.array.SetDynamicToolTips(0, true);
					this.array.SetDynamicToolTips(1, false);
					this.array.SetDynamicToolTips(2, false);

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
					string label, text;
					bool isDefined;
					this.access.GetBypassFilterStrings(this.druidsIndex[first+i], bundle, out label, out text, out isDefined);

					this.array.SetLineString(0, first+i, label);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);

					this.array.SetLineString(1, first+i, text);
					this.array.SetLineState(1, first+i, isDefined ? MyWidgets.StringList.CellState.Normal : MyWidgets.StringList.CellState.Warning);

					if (this.access.ResourceType != ResourceAccess.Type.Strings)
					{
						string icon = this.access.GetBypassFilterIcon(this.druidsIndex[first+i]);

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


		protected Module ModuleSearch(Druid druid)
		{
			//	Cherche à quel module appartient un druid.
			if (druid.IsEmpty)
			{
				return null;
			}

			List<MainWindow.ModuleInfo> list = this.mainWindow.OpeningListModule;
			foreach (MainWindow.ModuleInfo info in list)
			{
				ResourceAccess access = info.Module.PrepareAccess(this.resourceType);

				for (int i=0; i<access.TotalCount; i++)
				{
					if (druid == access.GetBypassFilterDruid(i))
					{
						return info.Module;
					}
				}
			}

			return null;
		}


		protected void HandleFieldModuleComboClosed(object sender)
		{
			//	Choix d'un module dans le menu-combo.
			string text = this.fieldModule.Text;

			List<MainWindow.ModuleInfo> list = this.mainWindow.OpeningListModule;
			foreach (MainWindow.ModuleInfo info in list)
			{
				if (text == info.Module.ModuleInfo.Name)
				{
					this.module = info.Module;
					this.access = this.module.PrepareAccess(this.resourceType);

					this.ignoreChanged = true;
					this.filterLabel.Text = "";
					this.filterText.Text = "";
					this.ignoreChanged = false;

					this.UpdateDruidsIndex();
					this.UpdateArray();
					this.SelectArray();
					return;
				}
			}
		}

		protected void HandleWindowCloseClicked(object sender)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		protected void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();
		}

		protected void HandleButtonClearClicked(object sender, MessageEventArgs e)
		{
			this.filterLabel.Text = "";
			this.filterText.Text = "";
		}

		protected void HandleButtonUseClicked(object sender, MessageEventArgs e)
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

		protected void HandleButtonCreateClicked(object sender, MessageEventArgs e)
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


		protected void HandleFilterTextChanged(object sender)
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


		protected void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a changé.
			this.UpdateColumnsWidth();
		}

		protected void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a changé.
			this.UpdateArray();
		}

		protected void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a changé.
			this.UpdateArray();
		}

		protected void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne sélectionnée a changé.
			this.UpdateButtons();
		}

		protected void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	La ligne sélectionnée a été double cliquée.
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


		protected Module						module;
		protected ResourceAccess.Type			resourceType;
		protected ResourceAccess				access;
		protected StaticText					title;
		protected TextFieldCombo				fieldModule;
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
