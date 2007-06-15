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
			//	Cr�e et montre la fen�tre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				this.window.MakeSecondaryWindow();
				this.window.PreventAutoClose = true;
				this.WindowInit("ResourceSelector", 500, 300, true);
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

				//	Titre sup�rieur.
				header = new Widget(this.window.Root);
				header.Margins = new Margins(0, 0, 0, 8);
				header.PreferredHeight = 26;
				header.Dock = DockStyle.Top;

				this.title = new StaticText(header);
				this.title.Dock = DockStyle.Fill;

				this.fieldModule = new TextFieldCombo(header);
				this.fieldModule.IsReadOnly = true;
				this.fieldModule.PreferredWidth = 180;
				this.fieldModule.Dock = DockStyle.Right;
				this.fieldModule.Margins = new Margins(0, 0, 5, 1);
				this.fieldModule.ComboClosed += new EventHandler(this.HandleFieldModuleComboClosed);

				//	Trait horizontal de s�paration.
				Separator sep = new Separator(this.window.Root);
				sep.PreferredHeight = 1;
				sep.Dock = DockStyle.Top;

				//	En-t�te avec les textes fixes.
				header = new Widget(this.window.Root);
				header.Margins = new Margins(0, 0, 0, 2);
				header.Dock = DockStyle.Top;

				this.header1 = new StaticText(header);
				this.header1.MinWidth = 100;
				this.header1.Dock = DockStyle.Left;

				this.header2 = new StaticText(header);
				this.header2.MinWidth = 100;
				this.header2.Dock = DockStyle.Left;

				//	En-t�te avec les lignes �ditables.
				header = new Widget(this.window.Root);
				header.Margins = new Margins(0, 0, 0, 4);
				header.Dock = DockStyle.Top;

				this.filterLabel = new TextField(header);
				this.filterLabel.MinWidth = 100;
				this.filterLabel.Name = "FilterLabel";
				this.filterLabel.TextChanged += new EventHandler(this.HandleFilterTextChanged);
				this.filterLabel.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFilterKeyboardFocusChanged);
				this.filterLabel.TabIndex = tabIndex++;
				this.filterLabel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.filterLabel.Dock = DockStyle.Left;
				this.filterLabel.Margins = new Margins(0, 1, 0, 0);

				this.filterText = new TextField(header);
				this.filterText.MinWidth = 100;
				this.filterText.Name = "FilterText";
				this.filterText.TextChanged += new EventHandler(this.HandleFilterTextChanged);
				this.filterText.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleFilterKeyboardFocusChanged);
				this.filterText.TabIndex = tabIndex++;
				this.filterText.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.filterText.Dock = DockStyle.Left;
				this.filterText.Margins = new Margins(0, 1, 0, 0);

				this.buttonClear = new Button(header);
				this.buttonClear.Text = Res.Strings.Dialog.ResourceSelector.Button.Clear;
				this.buttonClear.PreferredSize = new Size(16, 20);
				this.buttonClear.TabIndex = tabIndex++;
				this.buttonClear.TabNavigationMode = TabNavigationMode.ActivateOnTab;
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
				this.array.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.array.Dock = DockStyle.Fill;

				//	Boutons de fermeture.
				Widget footer = new Widget(this.window.Root);
				footer.PreferredHeight = 22;
				footer.Margins = new Margins(0, 0, 8, 0);
				footer.Dock = DockStyle.Bottom;

				this.buttonUse = new Button(footer);
				this.buttonUse.PreferredWidth = 75;
				this.buttonUse.Text = Res.Strings.Dialog.ResourceSelector.Button.Use;
				this.buttonUse.Dock = DockStyle.Left;
				this.buttonUse.Margins = new Margins(0, 6, 0, 0);
				this.buttonUse.Clicked += new MessageEventHandler(this.HandleButtonUseClicked);
				this.buttonUse.TabIndex = tabIndex++;
				this.buttonUse.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonCreate = new Button(footer);
				this.buttonCreate.PreferredWidth = 75;
				this.buttonCreate.Text = Res.Strings.Dialog.ResourceSelector.Button.Create;
				this.buttonCreate.Dock = DockStyle.Left;
				this.buttonCreate.Margins = new Margins(0, 6, 0, 0);
				this.buttonCreate.Clicked += new MessageEventHandler(this.HandleButtonCreateClicked);
				this.buttonCreate.TabIndex = tabIndex++;
				this.buttonCreate.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.buttonCancel = new Button(footer);
				this.buttonCancel.PreferredWidth = 75;
				this.buttonCancel.Text = Res.Strings.Dialog.Button.Cancel;
				this.buttonCancel.ButtonStyle = ButtonStyle.DefaultCancel;
				this.buttonCancel.Dock = DockStyle.Right;
				this.buttonCancel.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
				this.buttonCancel.TabIndex = tabIndex++;
				this.buttonCancel.TabNavigationMode = TabNavigationMode.ActivateOnTab;
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
				this.access.BypassFilterGetStrings(this.resource, this.CurrentBundle, this.array.LineHeight, out label, out text, out isDefined);
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


		public void AccessOpenList(Module baseModule, ResourceAccess.Type type, Common.Types.TypeCode typeCode, List<Druid> resources, List<Druid> exclude, string includePrefix)
		{
			//	D�but de l'acc�s 'bypass' aux ressources pour le dialogue.
			System.Diagnostics.Debug.Assert(type == ResourceAccess.Type.Captions || type == ResourceAccess.Type.Fields || type == ResourceAccess.Type.Commands || type == ResourceAccess.Type.Values);

			this.resourceType = type;
			this.resourceTypeCode = typeCode;
			this.resource = Druid.Empty;
			this.resources = resources;
			this.exclude = exclude;
			this.includePrefix = includePrefix;

			//	Cherche le module contenant le Druid de la ressource.
			this.baseModule = baseModule;
			this.module = baseModule;  // utilise le module de base

			this.access = this.module.AccessCaptions;

			this.access.BypassFilterOpenAccess(this.resourceType, this.resourceTypeCode, this.exclude, this.includePrefix);
		}

		public void AccessOpen(Module baseModule, ResourceAccess.Type type, Common.Types.TypeCode typeCode, Druid resource, List<Druid> exclude, string includePrefix)
		{
			//	D�but de l'acc�s 'bypass' aux ressources pour le dialogue.
			//	Le type peut �tre inconnu ou la ressource inconnue, mais pas les deux.
			System.Diagnostics.Debug.Assert(type == ResourceAccess.Type.Unknow || type == ResourceAccess.Type.Captions || type == ResourceAccess.Type.Fields || type == ResourceAccess.Type.Commands || type == ResourceAccess.Type.Values || type == ResourceAccess.Type.Types || type == ResourceAccess.Type.Panels);
			System.Diagnostics.Debug.Assert(resource.Type != DruidType.ModuleRelative);

			this.resourceTypeCode = typeCode;
			this.resource = resource;
			this.resources = null;
			this.exclude = exclude;
			this.includePrefix = includePrefix;

			//	Cherche le module contenant le Druid de la ressource.
			this.baseModule = baseModule;
			this.module = this.mainWindow.SearchModule(this.resource);

			if (this.module == null)  // module inconnu ?
			{
				if (this.lastModule == null)
				{
					this.module = this.baseModule;  // utilise le module de base
				}
				else
				{
					this.module = this.lastModule;  // utilise le dernier module utilis�
				}
			}

			if (type == ResourceAccess.Type.Panels)
			{
				this.access = this.module.AccessPanels;
			}
			else
			{
				this.access = this.module.AccessCaptions;
			}

			//	Utilise le type sp�cifi� s'il est d�fini, ou le type de la ressource dans le cas contraire.
			if (type == ResourceAccess.Type.Unknow)
			{
				this.resourceType = this.access.DirectGetType(this.resource);
				System.Diagnostics.Debug.Assert(this.resourceType != ResourceAccess.Type.Unknow);
			}
			else
			{
				this.resourceType = type;
			}

			this.access.BypassFilterOpenAccess(this.resourceType, this.resourceTypeCode, this.exclude, this.includePrefix);
		}

		protected void AccessChange(Module module)
		{
			//	Change l'acc�s 'bypass' aux ressources dans un autre module.
			this.AccessClose();

			this.module = module;
			this.lastModule = module;

			if (this.resourceType == ResourceAccess.Type.Panels)
			{
				this.access = this.module.AccessPanels;
			}
			else
			{
				this.access = this.module.AccessCaptions;
			}

			this.access.BypassFilterOpenAccess(this.resourceType, this.resourceTypeCode, this.exclude, this.includePrefix);
		}

		public List<Druid> AccessCloseList()
		{
			//	Fin de l'acc�s 'bypass' aux ressources pour le dialogue.
			this.access.BypassFilterCloseAccess();
			return this.resources;
		}

		public Druid AccessClose()
		{
			//	Fin de l'acc�s 'bypass' aux ressources pour le dialogue.
			this.access.BypassFilterCloseAccess();
			return this.resource;
		}


		protected void UpdateTitle()
		{
			//	Met � jour le titre qui d�pend du type des ressources �dit�es.
			string text = string.Concat("<font size=\"200%\"><b>", ResourceAccess.TypeDisplayName(this.resourceType), "</b></font>");
			this.title.Text = text;

			this.fieldModule.Items.Clear();

			List<Module> list = this.mainWindow.OpeningListModule;
			foreach (Module module in list)
			{
				text = module.ModuleInfo.Name;

				if (module == this.baseModule)
				{
					text = Misc.Bold(text);
				}
				else
				{
					text = Misc.Italic(text);
				}

				this.fieldModule.Items.Add(text);
			}

			text = this.module.ModuleInfo.Name;

			if (this.module == this.baseModule)
			{
				text = Misc.Bold(text);
			}
			else
			{
				text = Misc.Italic(text);
			}

			this.fieldModule.Text = text;
		}

		protected void UpdateHeader()
		{
			//	Met � jour les textes fixes en haut.
			this.header1.Text = Res.Strings.Dialog.ResourceSelector.Label;

			string culture = Misc.CultureName(this.mainWindow.CurrentModule.ResourceManager.ActiveCulture);
			this.header2.Text = string.Format(Res.Strings.Dialog.ResourceSelector.Text, culture);
		}

		protected void UpdateColumnsWidth()
		{
			//	Place les widgets en dessus du tableau en fonction des largeurs des colonnes.
			if (this.array.Columns == 1)
			{
				this.header2.Visibility = false;
				this.filterText.Visibility = false;
				this.buttonCreate.Visibility = false;

				double w1 = this.array.GetColumnsAbsoluteWidth(0)-20;
				this.header1.PreferredWidth = w1;
				this.filterLabel.PreferredWidth = w1;
			}
			else
			{
				this.header2.Visibility = true;
				this.filterText.Visibility = true;
				this.buttonCreate.Visibility = true;

				//	TODO: �trange probl�me lors du redimensionnement de la fen�tre...
				double w1 = this.array.GetColumnsAbsoluteWidth(0);
				double w2 = this.array.GetColumnsAbsoluteWidth(1);

				this.header1.PreferredWidth = w1;
				this.filterLabel.PreferredWidth = w1;

				this.header2.PreferredWidth = w2;
				this.filterText.PreferredWidth = w2;

				System.Diagnostics.Debug.WriteLine(string.Format("Widths : {0}, {1}", w1, w2));
			}
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

			for (int i=0; i<this.access.BypassFilterCount; i++)
			{
				Druid druid = this.access.BypassFilterGetDruid(i);

				string label, text;
				bool isDefined;
				this.access.BypassFilterGetStrings(druid, bundle, this.array.LineHeight, out label, out text, out isDefined);

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

			if (this.array.AllowMultipleSelection)
			{
				best = 0;
			}

			return best;
		}

		protected void UpdateResourceType()
		{
			//	Met � jour le tableau en fonction du type des ressources.
			this.fieldModule.Visibility = (this.resourceType != ResourceAccess.Type.Fields);

			int columns;
			if (this.resourceType == ResourceAccess.Type.Strings)
			{
				columns = 2;  // 2 colonnes pour les Strings
			}
			else if (this.resourceType == ResourceAccess.Type.Panels)
			{
				columns = 1;  // 1 colonne pour les Panels
			}
			else
			{
				columns = 3;  // 3 colonnes pour les Captions, Commands et Types
			}

			if (this.array.Columns != columns)  // changement du nombre de colonnes ?
			{
				this.array.Columns = columns;

				if (columns == 1)
				{
					this.array.SetColumnsRelativeWidth(0, 1.0);
					this.array.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
					this.array.SetDynamicToolTips(0, false);  // tellement large que le tooltip est inutile
					this.array.LineHeight = 20;  // hauteur standard
				}
				else if (columns == 2)
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
					this.array.SetColumnsRelativeWidth(0, 0.5);
					this.array.SetColumnsRelativeWidth(1, 0.4);
					this.array.SetColumnsRelativeWidth(2, 0.1);

					this.array.SetColumnAlignment(0, ContentAlignment.MiddleLeft);
					this.array.SetColumnAlignment(1, ContentAlignment.MiddleLeft);
					this.array.SetColumnAlignment(2, ContentAlignment.MiddleCenter);

					this.array.SetDynamicToolTips(0, true);
					this.array.SetDynamicToolTips(1, false);
					this.array.SetDynamicToolTips(2, false);

					this.array.LineHeight = 30;  // plus haut, � cause des descriptions et des ic�nes
				}
			}
		}

		protected void UpdateArray()
		{
			//	Met � jour tout le contenu du tableau.
			this.array.AllowMultipleSelection = (this.resources != null);
			this.array.TotalRows = this.druidsIndex.Count;

			ResourceBundle bundle = this.CurrentBundle;

			int first = this.array.FirstVisibleRow;
			for (int i=0; i<this.array.LineCount; i++)
			{
				if (first+i < this.druidsIndex.Count)
				{
					string label, text;
					bool isDefined;
					this.access.BypassFilterGetStrings(this.druidsIndex[first+i], bundle, this.array.LineHeight, out label, out text, out isDefined);

					this.array.SetLineString(0, first+i, label);
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Normal);

					if (this.resourceType != ResourceAccess.Type.Panels)
					{
						this.array.SetLineString(1, first+i, text);
						this.array.SetLineState(1, first+i, isDefined ? MyWidgets.StringList.CellState.Normal : MyWidgets.StringList.CellState.Warning);

						if (this.resourceType != ResourceAccess.Type.Strings)
						{
							string icon = this.access.DirectGetIcon(this.druidsIndex[first+i]);

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
				}
				else
				{
					this.array.SetLineString(0, first+i, "");
					this.array.SetLineState(0, first+i, MyWidgets.StringList.CellState.Disabled);

					if (this.resourceType != ResourceAccess.Type.Panels)
					{
						this.array.SetLineString(1, first+i, "");
						this.array.SetLineState(1, first+i, MyWidgets.StringList.CellState.Disabled);

						if (this.resourceType != ResourceAccess.Type.Strings)
						{
							this.array.SetLineString(2, first+i, "");
							this.array.SetLineState(2, first+i, MyWidgets.StringList.CellState.Disabled);
						}
					}
				}
			}
		}

		protected void SelectArray()
		{
			//	S�lectionne la bonne ressource dans le tableau.
			int sel = -1;

			if (this.array.AllowMultipleSelection)
			{
				List<int> sels = new List<int>();

				for (int i=0; i<this.druidsIndex.Count; i++)
				{
					if (this.resources.Contains(this.druidsIndex[i]))
					{
						sels.Add(i);
					}
				}

				this.array.SelectedRows = sels;
			}
			else
			{
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
		}

		protected void UpdateButtons()
		{
			string label = TextLayout.ConvertToSimpleText(this.filterLabel.Text);
			string text  = TextLayout.ConvertToSimpleText(this.filterText.Text);

			this.buttonClear.Enable = (label != "" || text != "");

			if (this.array.AllowMultipleSelection)
			{
				this.buttonUse.Enable = (this.array.SelectedRows.Count > 0);
			}
			else
			{
				this.buttonUse.Enable = (this.array.SelectedRow != -1);
			}

			bool createEnable = false;
			if (this.resourceType != ResourceAccess.Type.Panels &&
				label != "" && text != "" && this.access.IsCorrectNewName(ref label, true))
			{
				createEnable = true;
			}
			this.buttonCreate.Enable = createEnable;

			if (createEnable)
			{
				this.buttonUse.ButtonStyle = ButtonStyle.Normal;
				this.buttonCreate.ButtonStyle = ButtonStyle.DefaultAccept;
			}
			else
			{
				this.buttonUse.ButtonStyle = ButtonStyle.DefaultAccept;
				this.buttonCreate.ButtonStyle = ButtonStyle.Normal;
			}
		}

		protected ResourceBundle CurrentBundle
		{
			//	Retourne le bundle de la culture active, correspondant au choix fait dans
			//	l'onglet 'Cultures'.
			get
			{
				if (this.resourceType == ResourceAccess.Type.Panels)
				{
					return null;
				}
				else
				{
					string culture = Misc.CultureBaseName(this.mainWindow.CurrentModule.ResourceManager.ActiveCulture);
					return this.access.GetCultureBundle(culture);
				}
			}
		}


		private void HandleFieldModuleComboClosed(object sender)
		{
			//	Choix d'un module dans le menu-combo.
			string text = Misc.RemoveTags(this.fieldModule.Text);  // nom sans les tags <b> ou <i>

			List<Module> list = this.mainWindow.OpeningListModule;
			foreach (Module module in list)
			{
				if (text == module.ModuleInfo.Name)
				{
					this.AccessChange(module);

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

			this.resource = Druid.Empty;
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

			if (this.array.AllowMultipleSelection)
			{
				this.resources = new List<Druid>();

				List<int> sels = this.array.SelectedRows;
				foreach (int sel in sels)
				{
					this.resources.Add(this.druidsIndex[sel]);
				}
			}
			else
			{
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
		}

		private void HandleButtonCreateClicked(object sender, MessageEventArgs e)
		{
			this.parentWindow.MakeActive();
			this.window.Hide();
			this.OnClosed();

			string label = TextLayout.ConvertToSimpleText(this.filterLabel.Text);
			string fullLabel = label;
			if (this.includePrefix != null && !fullLabel.StartsWith(this.includePrefix))
			{
				fullLabel = string.Concat(this.includePrefix, fullLabel);
			}

			string text  = TextLayout.ConvertToSimpleText(this.filterText.Text);
			
			if (label != "" && text != "" && this.access.IsCorrectNewName(ref fullLabel, true))
			{
				if (this.array.AllowMultipleSelection)
				{
					//	TODO:
				}
				else
				{
					this.resource = this.access.BypassFilterCreate(this.CurrentBundle, fullLabel, text);
				}
			}
		}


		private void HandleFilterTextChanged(object sender)
		{
			//	Le texte d'un filtre a chang�.
			if (this.ignoreChanged)  return;

			int best = this.UpdateDruidsIndex();
			this.UpdateArray();
			this.UpdateButtons();

			this.array.SelectedRow = best;
			this.array.ShowSelectedRow();
		}

		private void HandleFilterKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appel� lorsqu'une ligne �ditable voit son focus changer.
			bool focused = (bool) e.NewValue;

			if (focused)
			{
				this.focusedWidget = sender as AbstractTextField;
			}
		}


		private void HandleArrayColumnsWidthChanged(object sender)
		{
			//	La largeur des colonnes a chang�.
			this.UpdateColumnsWidth();
		}

		private void HandleArrayCellCountChanged(object sender)
		{
			//	Le nombre de lignes a chang�.
			this.UpdateArray();
		}

		private void HandleArrayCellsContentChanged(object sender)
		{
			//	Le contenu des cellules a chang�.
			this.UpdateArray();
		}

		private void HandleArraySelectedRowChanged(object sender)
		{
			//	La ligne s�lectionn�e a chang�.
			this.UpdateButtons();
		}

		private void HandleArraySelectedRowDoubleClicked(object sender)
		{
			//	La ligne s�lectionn�e a �t� double cliqu�e.
			if (this.array.AllowMultipleSelection)
			{
				return;
			}

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


		protected Module						baseModule;
		protected Module						lastModule;
		protected Module						module;
		protected ResourceAccess.Type			resourceType;
		protected Common.Types.TypeCode			resourceTypeCode;
		protected ResourceAccess				access;
		protected List<Druid>					exclude;
		protected string						includePrefix;
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
		protected List<Druid>					resources;
		protected List<Druid>					druidsIndex;
		protected Widget						focusedWidget;
	}
}
